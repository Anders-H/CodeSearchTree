using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeSearchTreeTest
{
   public partial class FindFileDialog : Form
   {
      [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
      static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

      public string SelectedFilename { get; private set; }
      private CodeSearchTree.SearchNodeList SearchExpression { get; set; }
      private List<System.IO.FileInfo> FileList { get; set; }
      private List<System.IO.FileInfo> SearchResult { get; set; }
      private bool InSearch { get; set; }
      private bool CancelFlag { get; set; }

      public FindFileDialog()
      {
         InitializeComponent();
      }

      private void btnPickDir_Click(object sender, EventArgs e)
      {
         using (var x = new FolderBrowserDialog())
         {
            x.SelectedPath = txtDirectory.Text;
            if (x.ShowDialog(this) == DialogResult.OK)
               txtDirectory.Text = x.SelectedPath;
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         if (listView1.SelectedItems.Count <= 0)
            return;
         var selected_file = listView1.SelectedItems[0].Tag as System.IO.FileInfo;
         if (selected_file == null)
            return;
         this.SelectedFilename = selected_file.FullName;
         this.DialogResult = DialogResult.OK;
      }

      //Svarar på när användaren vill påbörja sökning eller sätter cancel-flaggan om sökning pågår.
      private void btnSearch_Click(object sender, EventArgs e)
      {
         if (this.InSearch)
         {
            this.CancelFlag = true;
            return;
         }
         txtSearch.Text = txtSearch.Text.Trim();
         if (txtSearch.Text == "")
            return;
         var success = false;
         var nodes = CodeSearchTree.Node.ParseSearchExpression(txtSearch.Text, out success);
         if (!(success))
         {
            MessageBox.Show("Search query contains errors.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         //Får inte innehålla några unknowns.
         if (nodes.Exists(x => x.NodeType == CodeSearchTree.NodeType.UnknownNode))
         {
            MessageBox.Show("Search expression has an unknown node type.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         System.IO.DirectoryInfo dir = null;
         try
         {
            dir = new System.IO.DirectoryInfo(txtDirectory.Text);
            if (!(dir.Exists))
            {
               MessageBox.Show("Directory does not exist.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }
         }
         catch
         {
            MessageBox.Show("Invalid directory.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         listView1.Items.Clear();
         btnSearch.Text = "Stop";
         btnOK.Enabled = false;
         this.InSearch = true;
         this.Cursor = Cursors.WaitCursor;
         var s = new Action<System.IO.DirectoryInfo, string, bool>(this.PerformSearch);
         s.BeginInvoke(dir, txtSearch.Text, cboScope.SelectedIndex == 1, this.SearchDone, null);
      }

      //Utför sökningen. Lyssnar på cancel-flaggan för eventuellt avbrott.
      private void PerformSearch(System.IO.DirectoryInfo dir, string search_expression, bool deep)
      {
         try
         {
            this.FileList = new List<System.IO.FileInfo>();
            this.SearchResult = new List<System.IO.FileInfo>();

            //Hämta C#-filerna i den angivna mappen.
            dir.GetFiles("*.cs").ToList().ForEach(x => this.FileList.Add(x));
            if (this.CancelFlag) this.StopSearch(true);

            //Hämta undermappar.
            dir.GetDirectories().ToList().ForEach(x => this.PerformSearchAddChildFilesAndFolders(x));
            if (this.CancelFlag) this.StopSearch(true);

            foreach (var file in this.FileList)
            {
               var tree = CodeSearchTree.Node.CreateTreeFromFile(file.FullName);
               if (deep)
               {
                  if (tree.DeepSearch(search_expression).Count > 0)
                     this.SearchResult.Add(file);
               }
               else
               {
                  if (!(tree.GetChild(search_expression) == null))
                     this.SearchResult.Add(file);
               }
            }
         }
         catch
         {
            if (this.InvokeRequired)
               this.Invoke(new Action(this.FailSearchGui));
            else
               this.FailSearchGui();
         }
      }

      //Söker i childkataloger efter C#-filer. Anropas från PerformSearch i söktråden.
      private void PerformSearchAddChildFilesAndFolders(System.IO.DirectoryInfo parent)
      {
         parent.GetFiles("*.cs").ToList().ForEach(x => this.FileList.Add(x));
         if (this.CancelFlag) return;
         parent.GetDirectories().ToList().ForEach(x => this.PerformSearchAddChildFilesAndFolders(x));
         if (this.CancelFlag) this.StopSearch(true);
      }

      //Callback för avslutad sökning. Anropar StopSearchGui för att återställa GUI och PresentSearchResult för att presentera resultatet.
      private void SearchDone(object arg)
      {
         this.InSearch = false;
         if (this.InvokeRequired)
            this.Invoke(new Action(this.StopSearchGui));
         else
            this.StopSearchGui();
         if (this.InvokeRequired)
            this.Invoke(new Action(this.PresentSearchResult));
         else
            this.PresentSearchResult();
      }

      //Anropas för att avbryta sökning.
      private void StopSearch(bool cancel)
      {
         this.InSearch = false;
         if (this.InvokeRequired)
            this.Invoke(new Action(this.StopSearchGui));
         else
            this.StopSearchGui();
      }

      //GUI-förändringar vid avslutad sökning.
      private void StopSearchGui()
      {
         btnSearch.Text = "Search";
         btnOK.Enabled = true;
         this.Cursor = Cursors.Default;
      }

      //Lista sökresultatet och meddela användaren att vi är färdiga.
      private void PresentSearchResult()
      {
         foreach (var f in this.SearchResult)
         {
            var short_filename = CompactPath(f.FullName, 70);
            var item = listView1.Items.Add(short_filename);
            item.Tag = f;
         }
      }

      private static string CompactPath(string longPathName, int wantedLength)
      {
         var s = new StringBuilder(wantedLength + 1);
         PathCompactPathEx(s, longPathName, wantedLength + 1, 0);
         return s.ToString();
      }

      //Anropas i catch vid misslyckad sökning.
      private void FailSearchGui()
      {
         btnSearch.Text = "Search";
         btnOK.Enabled = true;
         this.Cursor = Cursors.Default;
         MessageBox.Show("Search failed.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      private void FindFileDialog_Load(object sender, EventArgs e)
      {
         this.InSearch = false;
         this.CancelFlag = false;
         txtDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
         cboScope.Items.Clear();
         cboScope.Items.Add("Search from root only");
         cboScope.Items.Add("Deep search");
         cboScope.SelectedIndex = 0;
      }

      private void txtSearch_TextChanged(object sender, EventArgs e)
      {
         btnSearch.Enabled = this.InSearch || !(string.IsNullOrWhiteSpace(txtSearch.Text));
      }

      private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (!(listView1.GetItemAt(e.X, e.Y) == null))
            this.btnOK_Click(sender, new EventArgs());
      }
   }
}
