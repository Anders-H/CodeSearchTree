using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CodeSearchTreeTest
{
    public partial class FindFileDialog : Form
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        public string SelectedFilename { get; private set; }
        private List<FileInfo> FileList { get; set; }
        private List<FileInfo> SearchResult { get; set; }
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
            
            if (!(listView1.SelectedItems[0].Tag is FileInfo selectedFile))
                return;
            
            SelectedFilename = selectedFile.FullName;
            DialogResult = DialogResult.OK;
        }

        //Svarar på när användaren vill påbörja sökning eller sätter cancel-flaggan om sökning pågår.
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (InSearch)
            {
                CancelFlag = true;
                return;
            }
            
            txtSearch.Text = txtSearch.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return;

            var nodes = CodeSearchTree.Node.ParseSearchExpression(txtSearch.Text, out var success);
            
            if (!(success))
            {
                MessageBox.Show(@"Search query contains errors.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Får inte innehålla några unknowns.
            if (nodes.Exists(x => x.NodeType == CodeSearchTree.NodeType.UnknownNode))
            {
                MessageBox.Show(@"Search expression has an unknown node type.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DirectoryInfo dir;
            
            try
            {
                dir = new DirectoryInfo(txtDirectory.Text);
                
                if (!dir.Exists)
                {
                    MessageBox.Show(@"Directory does not exist.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show(@"Invalid directory.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            listView1.Items.Clear();
            btnSearch.Text = @"Stop";
            btnOK.Enabled = false;
            InSearch = true;
            Cursor = Cursors.WaitCursor;
            var s = new Action<DirectoryInfo, string, bool>(PerformSearch);
            s.BeginInvoke(dir, txtSearch.Text, cboScope.SelectedIndex == 1, SearchDone, null);
        }

        //Utför sökningen. Lyssnar på cancel-flaggan för eventuellt avbrott.
        private void PerformSearch(DirectoryInfo dir, string searchExpression, bool deep)
        {
            try
            {
                FileList = new List<FileInfo>();
                SearchResult = new List<FileInfo>();

                //Hämta C#-filerna i den angivna mappen.
                dir.GetFiles("*.cs").ToList().ForEach(x => FileList.Add(x));
                
                if (CancelFlag)
                    StopSearch();

                //Hämta undermappar.
                dir.GetDirectories().ToList().ForEach(PerformSearchAddChildFilesAndFolders);
                
                if (CancelFlag)
                    StopSearch();

                foreach (var file in FileList)
                {
                    var tree = CodeSearchTree.Node.CreateTreeFromFile(file.FullName);
                    
                    if (deep)
                    {
                        if (tree.DeepSearch(searchExpression).Count > 0)
                            SearchResult.Add(file);
                    }
                    else
                    {
                        if (tree.GetChild(searchExpression) != null)
                            SearchResult.Add(file);
                    }
                }
            }
            catch
            {
                if (InvokeRequired)
                    Invoke(new Action(FailSearchGui));
                else
                    FailSearchGui();
            }
        }

        //Söker i childkataloger efter C#-filer. Anropas från PerformSearch i söktråden.
        private void PerformSearchAddChildFilesAndFolders(DirectoryInfo parent)
        {
            parent.GetFiles("*.cs").ToList().ForEach(x => FileList.Add(x));
            
            if (CancelFlag)
                return;
            
            parent.GetDirectories().ToList().ForEach(PerformSearchAddChildFilesAndFolders);
            
            if (CancelFlag)
                StopSearch();
        }

        //Callback för avslutad sökning. Anropar StopSearchGui för att återställa GUI och PresentSearchResult för att presentera resultatet.
        private void SearchDone(object arg)
        {
            InSearch = false;
            if (InvokeRequired)
                Invoke(new Action(StopSearchGui));
            else
                StopSearchGui();

            if (InvokeRequired)
                Invoke(new Action(PresentSearchResult));
            else
                PresentSearchResult();
        }

        //Anropas för att avbryta sökning.
        private void StopSearch()
        {
            InSearch = false;
            if (InvokeRequired)
                Invoke(new Action(StopSearchGui));
            else
                StopSearchGui();
        }

        //GUI-förändringar vid avslutad sökning.
        private void StopSearchGui()
        {
            btnSearch.Text = @"Search";
            btnOK.Enabled = true;
            Cursor = Cursors.Default;
        }

        //Lista sökresultatet och meddela användaren att vi är färdiga.
        private void PresentSearchResult()
        {
            foreach (var f in SearchResult)
            {
                var shortFilename = CompactPath(f.FullName, 70);
                var item = listView1.Items.Add(shortFilename);
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
            btnSearch.Text = @"Search";
            btnOK.Enabled = true;
            Cursor = Cursors.Default;
            MessageBox.Show(@"Search failed.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void FindFileDialog_Load(object sender, EventArgs e)
        {
            InSearch = false;
            CancelFlag = false;
            txtDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            cboScope.Items.Clear();
            cboScope.Items.Add("Search from root only");
            cboScope.Items.Add("Deep search");
            cboScope.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) =>
            btnSearch.Enabled = InSearch || !(string.IsNullOrWhiteSpace(txtSearch.Text));

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.GetItemAt(e.X, e.Y) != null)
                btnOK_Click(sender, EventArgs.Empty);
        }
    }
}