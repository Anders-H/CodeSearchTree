using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CodeSearchTree;

namespace CodeSearchTreeTest
{
   public partial class Form1 : Form
   {
      private NodeList _codeTree;

      public Form1()
      {
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         cboSearchFrom.Items.AddRange(new object[] {"Root", "Selected node", "Deep (from root)"});
         cboSearchFrom.SelectedIndex = 0;

         cboViewSource.Items.AddRange(new object[] {"Context menu", "Node select"});
         cboViewSource.SelectedIndex = 0;
      }

      private void Form1_Shown(object sender, EventArgs e)
      {
         treeView1.Nodes.Add("Drop cs files here.");
         txtInput.Focus();
      }

      private void LoadCs(string filename)
      {
         _codeTree = Node.CreateTreeFromFile(filename);
         treeView1.BeginUpdate();
         treeView1.Nodes.Clear();
         foreach (var codeTreeNode in _codeTree)
         {
            var treeviewNode = treeView1.Nodes.Add(codeTreeNode.ToString());
            treeviewNode.Tag = codeTreeNode;
            ConstructChildren(treeviewNode, codeTreeNode);
         }
         treeView1.EndUpdate(); //Detta är av någon märklig anlednign leading för treeView1.EndUpdate();

      }

      private void ConstructChildren(TreeNode parentTreeview, Node parentCode)
      {
         foreach (var codeTreeNode in parentCode.Children)
         {
            var treeviewNode = parentTreeview.Nodes.Add(codeTreeNode.ToString());
            treeviewNode.Tag = codeTreeNode;
            ConstructChildren(treeviewNode, codeTreeNode);
         }
      }

      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         propertyGrid1.SelectedObject = e.Node.Tag as Node;
         if (cboViewSource.SelectedIndex == 1)
            viewSourceToolStripMenuItem_Click(sender, new EventArgs());
      }

      private void DoSearch(string search)
      {
         txtResult.WordWrap = false;
         var searchFromSelected = cboSearchFrom.SelectedIndex == 1 && treeView1.SelectedNode != null;
         var deepSearch = cboSearchFrom.SelectedIndex == 2;
         TreeNode n = null;
         if (searchFromSelected)
            n = treeView1.SelectedNode;
         else
         {
            //Både deep (rekrusiv) och root utgår från rooten.
            if (treeView1.Nodes.Count > 0)
               n = treeView1.Nodes[0];
         }
         if (n == null)
         {
            txtInput.WriteLine("No document loaded.");
            return;
         }
         var node = n.Tag as Node;
         if (node == null)
         {
            txtInput.WriteLine("No document loaded.");
            return;
         }
#if !DEBUG
         try
         {
#endif
            var resp = searchFromSelected
                     ? node.GetChild(search) //Måste vara korrekt sökväg från val nod.
                     : deepSearch
                     ? _codeTree.DeepSearch(search).FirstOrDefault() //Rekrusiv sökning från rooten.
                     : _codeTree.GetChild(search); //Korrekt sökväg från rooten.
            txtResult.Text = @"RESULT:
";
            if (resp == null)
            {
               txtResult.AppendText("Nothing.");
               txtInput.WriteLine("Nothing.");
               return;
            }
            var oneLineResult = System.Text.RegularExpressions.Regex.Replace(resp.Source, @"\s+", " ").Trim();
            if (oneLineResult.Length > 20)
               oneLineResult = ($"{oneLineResult.Substring(0, 20).Trim()}...");
            txtInput.WriteLine($"{oneLineResult} ({resp.Source.Length} characters)");
            txtResult.AppendText(resp.Source);
            txtResult.AppendText("\n");
            if (resp.LeadingTrivia.Count > 0)
            {
               txtResult.AppendText("\nLEADING:\n");
               resp.LeadingTrivia.ForEach(x => txtResult.AppendText(x + "\n"));
            }
            if (resp.TrailingTrivia.Count > 0)
            {
               txtResult.AppendText("\nTRAILING:\n");
               resp.TrailingTrivia.ForEach(x => txtResult.AppendText(x + "\n"));
            }
            txtResult.SelectionStart = 0;
            txtResult.ScrollToCaret();
#if !DEBUG
         }
         catch (Exception ex)
         {
            var s = new StringBuilder();
            s.AppendLine("EXCEPTION:");
            s.AppendLine($"Type: {ex.GetType().Name}");
            s.AppendLine($"Message: {ex.Message}");
            s.AppendLine();
            s.AppendLine(ex.ToString());
            txtResult.Text = s.ToString();
            txtResult.SelectionStart = 0;
            txtResult.ScrollToCaret();
         }
#endif
      }

      private void treeView1_DragEnter(object sender, DragEventArgs e)
      {
         e.Effect = DragDropEffects.Link;
      }

      private void treeView1_DragDrop(object sender, DragEventArgs e)
      {
         if (!(e.Data.GetDataPresent("FileNameW")))
         {
            MessageBox.Show(@"Nothing loadable found.", @"Open file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }
         var data = e.Data.GetData("FileNameW") as string[];
         if (data == null || data.Length <= 0)
         {
            MessageBox.Show(@"Nothing loadable found.", @"Open file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }
#if DEBUG
         LoadCs(data[0]);
#else
         try
         {
            this.LoadCs(data[0]);
         }
         catch (Exception ex)
         {
            _codeTree = null;
            var s = new StringBuilder();
            s.AppendLine("EXCEPTION:");
                s.AppendLine($"Type: {ex.GetType().Name}");
                s.AppendLine($"Message: {ex.Message}");
                s.AppendLine();
            s.AppendLine(ex.ToString());
            txtResult.Text = s.ToString();
            MessageBox.Show($"Failed to load \"{data[0]}\".", "Open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
#endif

      }

      private void treeView1_MouseDown(object sender, MouseEventArgs e)
      {
         var currentButton = (int)e.Button;
         const int rightButton = (int)MouseButtons.Right;
         if ((currentButton & rightButton) <= 0)
            return;

         var n = treeView1.GetNodeAt(e.X, e.Y);
          if (!(n?.Tag is Node))
            return;

         treeView1.SelectedNode = n;
         contextMenuStrip1.Show(treeView1, e.X, e.Y);
      }

      private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var n = treeView1.SelectedNode?.Tag as Node;
         if (n == null)
            return;
         txtResult.WordWrap = false;
         txtResult.Text = n.Source;
      }

      private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
      {
         viewSourceToolStripMenuItem.Enabled = cboViewSource.SelectedIndex == 0;
      }

      private void btnHelp_Click(object sender, EventArgs e)
      {
         txtResult.WordWrap = true;
         txtResult.Text = $@"SEARCH TREE version {Assembly.GetExecutingAssembly().GetName().Version}
Project homepage: https://github.com/Anders-H/CodeSearchTree

SEARCH EXPRESSIONS
A search expression is a slash separated list of nodes. To get from a namespace to a method via a class, type:

namespace/class/method

Or:

ns/cls/method

If your namespace contains several classes, and your classes contains several methods, you can add an index. This will give you the third (index 2) method in the second (index 1) class.

ns/cls[1]/method[2]

You can also specify names instead of indexes. This will give you the method called MyFunction in the second class:

ns/cls[1]/method[MyFunction]

The node type can be unspecified. This will give you anything named MyFunction in the first class in the namespace.

ns/cls/*[MyFunction]

And this will deliver the first node with two parent nodes, regardless of name and type:

*/*/*

C# EXAMPLES
Get class node from file:

   var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
   var my_class = code_tree.GetChild(""ns/cls"");

Search for file that contains node a class named MyClass:

   var result = CodeSearchTree.FileSystem.CreateTreesFromFolder(foldername, ""*/cls[MyClass]"");
   if (result.Count > 0)
      Console.WriteLine(""Success!"");";
      }

      private void viewRoslynToolStripMenuItem_Click(object sender, EventArgs e)
      {
         txtResult.Text = "";
         txtResult.WordWrap = false;
         var n = treeView1.SelectedNode.Tag as Node;
         if (n == null)
            return;
         txtResult.Text = n.RoslynNodePropertiesString;
      }

      private void btnFindFile_Click(object sender, EventArgs e)
      {
         using (var x = new FindFileDialog())
            if (x.ShowDialog(this) == DialogResult.OK)
               LoadCs(x.SelectedFilename);
      }

        private void txtInput_Entered(object arg1, TextEnteredEventArgs arg2)
        {
            txtInput.WriteLine($"Search for \"{arg2.Entered}\".");
            DoSearch(arg2.Entered);
        }
    }
}
