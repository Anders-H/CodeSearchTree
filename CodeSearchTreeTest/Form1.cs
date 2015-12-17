using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeSearchTreeTest
{
   public partial class Form1 : Form
   {
      private CodeSearchTree.NodeList code_tree;

      public Form1()
      {
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         cboSearchFrom.Items.Clear();
         cboSearchFrom.Items.Add("Root");
         cboSearchFrom.Items.Add("Selected node");
         cboSearchFrom.Items.Add("Deep (from root)");
         cboSearchFrom.SelectedIndex = 0;

         cboViewSource.Items.Clear();
         cboViewSource.Items.Add("Context menu");
         cboViewSource.Items.Add("Node select");
         cboViewSource.SelectedIndex = 0;
      }

      private void Form1_Shown(object sender, EventArgs e)
      {
         treeView1.Nodes.Add("Drop cs files here.");
         txtInput.Focus();
      }

      private void LoadCs(string filename)
      {
         code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
         treeView1.BeginUpdate();
         treeView1.Nodes.Clear();
         foreach (var code_tree_node in code_tree)
         {
            var treeview_node = treeView1.Nodes.Add(code_tree_node.ToString());
            treeview_node.Tag = code_tree_node;
            this.ConstructChildren(treeview_node, code_tree_node);
         }
         treeView1.EndUpdate(); //Detta är av någon märklig anlednign leading för treeView1.EndUpdate();

      }

      private void ConstructChildren(TreeNode parent_treeview, CodeSearchTree.Node parent_code)
      {
         foreach (var code_tree_node in parent_code.Children)
         {
            var treeview_node = parent_treeview.Nodes.Add(code_tree_node.ToString());
            treeview_node.Tag = code_tree_node;
            this.ConstructChildren(treeview_node, code_tree_node);
         }
      }

      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         propertyGrid1.SelectedObject = e.Node.Tag as CodeSearchTree.Node;
         if (cboViewSource.SelectedIndex == 1)
            viewSourceToolStripMenuItem_Click(sender, new EventArgs());
      }

      private void DoSearch(string search)
      {
         txtResult.WordWrap = false;
         var search_from_selected = cboSearchFrom.SelectedIndex == 1 && !(treeView1.SelectedNode == null);
         var deep_search = cboSearchFrom.SelectedIndex == 2;
         TreeNode n = null;
         if (search_from_selected)
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
         var node = n.Tag as CodeSearchTree.Node;
         if (node == null)
         {
            txtInput.WriteLine("No document loaded.");
            return;
         }
#if !DEBUG
         try
         {
#endif
            var svar = search_from_selected
                     ? node.GetChild(search) //Måste vara korrekt sökväg från val nod.
                     : deep_search
                     ? this.code_tree.DeepSearch(search).FirstOrDefault() //Rekrusiv sökning från rooten.
                     : this.code_tree.GetChild(search); //Korrekt sökväg från rooten.
            txtResult.Text = "RESULT:\n";
            if (svar == null)
            {
               txtResult.AppendText("Nothing.");
               txtInput.WriteLine("Nothing.");
               return;
            }
            var one_line_result = System.Text.RegularExpressions.Regex.Replace(svar.Source, @"\s+", " ").Trim();
            if (one_line_result.Length > 20)
               one_line_result = ($"{one_line_result.Substring(0, 20).Trim()}...");
            txtInput.WriteLine($"{one_line_result} ({svar.Source.Length} characters)");
            txtResult.AppendText(svar.Source);
            txtResult.AppendText("\n");
            if (svar.LeadingTrivia.Count > 0)
            {
               txtResult.AppendText("\nLEADING:\n");
               svar.LeadingTrivia.ForEach(x => txtResult.AppendText(x + "\n"));
            }
            if (svar.TrailingTrivia.Count > 0)
            {
               txtResult.AppendText("\nTRAILING:\n");
               svar.TrailingTrivia.ForEach(x => txtResult.AppendText(x + "\n"));
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
            MessageBox.Show("Nothing loadable found.", "Open file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }
         var data = e.Data.GetData("FileNameW") as string[];
         if (data == null || data.Length <= 0)
         {
            MessageBox.Show("Nothing loadable found.", "Open file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }
#if DEBUG
         this.LoadCs(data[0]);
#else
         try
         {
            this.LoadCs(data[0]);
         }
         catch (Exception ex)
         {
            this.code_tree = null;
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
         var current_button = (int)e.Button;
         var right_button = (int)MouseButtons.Right;
         if ((current_button & right_button) <= 0)
            return;

         var n = treeView1.GetNodeAt(e.X, e.Y);
         if (n == null)
            return;
         if (n.Tag as CodeSearchTree.Node == null)
            return;

         treeView1.SelectedNode = n;
         contextMenuStrip1.Show(treeView1, e.X, e.Y);
      }

      private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var n = treeView1.SelectedNode?.Tag as CodeSearchTree.Node;
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
         txtResult.Text = @"SEARCH EXPRESSIONS
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
         var n = treeView1.SelectedNode.Tag as CodeSearchTree.Node;
         if (n == null)
            return;
         txtResult.Text = n.RoslynNodePropertiesString;
      }

      private void btnFindFile_Click(object sender, EventArgs e)
      {
         using (var x = new FindFileDialog())
         {
            if (x.ShowDialog(this) == DialogResult.OK)
               this.LoadCs(x.SelectedFilename);
         }
      }

        private void txtInput_Entered(object arg1, TextEnteredEventArgs arg2)
        {
            txtInput.WriteLine($"Search for \"{arg2.Entered}\".");
            this.DoSearch(arg2.Entered);
        }
    }
}
