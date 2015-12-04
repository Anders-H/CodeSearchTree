namespace CodeSearchTreeTest
{
   partial class FindFileDialog
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.label1 = new System.Windows.Forms.Label();
         this.txtDirectory = new System.Windows.Forms.TextBox();
         this.btnPickDir = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.txtSearch = new System.Windows.Forms.TextBox();
         this.listView1 = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.label3 = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnSearch = new System.Windows.Forms.Button();
         this.label4 = new System.Windows.Forms.Label();
         this.cboScope = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(8, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(52, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Directory:";
         // 
         // txtDirectory
         // 
         this.txtDirectory.Location = new System.Drawing.Point(8, 24);
         this.txtDirectory.Name = "txtDirectory";
         this.txtDirectory.Size = new System.Drawing.Size(356, 20);
         this.txtDirectory.TabIndex = 1;
         // 
         // btnPickDir
         // 
         this.btnPickDir.Location = new System.Drawing.Point(368, 22);
         this.btnPickDir.Name = "btnPickDir";
         this.btnPickDir.Size = new System.Drawing.Size(28, 23);
         this.btnPickDir.TabIndex = 2;
         this.btnPickDir.Text = "...";
         this.btnPickDir.UseVisualStyleBackColor = true;
         this.btnPickDir.Click += new System.EventHandler(this.btnPickDir_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(8, 48);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(97, 13);
         this.label2.TabIndex = 3;
         this.label2.Text = "Search expression:";
         // 
         // txtSearch
         // 
         this.txtSearch.Location = new System.Drawing.Point(8, 64);
         this.txtSearch.Name = "txtSearch";
         this.txtSearch.Size = new System.Drawing.Size(308, 20);
         this.txtSearch.TabIndex = 4;
         this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
         // 
         // listView1
         // 
         this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listView1.FullRowSelect = true;
         this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
         this.listView1.HideSelection = false;
         this.listView1.Location = new System.Drawing.Point(8, 152);
         this.listView1.MultiSelect = false;
         this.listView1.Name = "listView1";
         this.listView1.Size = new System.Drawing.Size(388, 208);
         this.listView1.TabIndex = 9;
         this.listView1.UseCompatibleStateImageBehavior = false;
         this.listView1.View = System.Windows.Forms.View.Details;
         this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Filename";
         this.columnHeader1.Width = 360;
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(8, 136);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(40, 13);
         this.label3.TabIndex = 8;
         this.label3.Text = "Result:";
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(240, 368);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 10;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(320, 368);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 11;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // btnSearch
         // 
         this.btnSearch.Enabled = false;
         this.btnSearch.Location = new System.Drawing.Point(320, 62);
         this.btnSearch.Name = "btnSearch";
         this.btnSearch.Size = new System.Drawing.Size(75, 23);
         this.btnSearch.TabIndex = 5;
         this.btnSearch.Text = "Search";
         this.btnSearch.UseVisualStyleBackColor = true;
         this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(8, 88);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(41, 13);
         this.label4.TabIndex = 6;
         this.label4.Text = "Scope:";
         // 
         // cboScope
         // 
         this.cboScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboScope.FormattingEnabled = true;
         this.cboScope.Location = new System.Drawing.Point(8, 104);
         this.cboScope.Name = "cboScope";
         this.cboScope.Size = new System.Drawing.Size(388, 21);
         this.cboScope.TabIndex = 7;
         // 
         // FindFileDialog
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(403, 399);
         this.Controls.Add(this.cboScope);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.btnSearch);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.listView1);
         this.Controls.Add(this.txtSearch);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.btnPickDir);
         this.Controls.Add(this.txtDirectory);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "FindFileDialog";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Find file";
         this.Load += new System.EventHandler(this.FindFileDialog_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtDirectory;
      private System.Windows.Forms.Button btnPickDir;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox txtSearch;
      private System.Windows.Forms.ListView listView1;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Button btnSearch;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.ComboBox cboScope;
   }
}