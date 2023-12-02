namespace LoginForm
{
    partial class ProcessForm
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
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToTrashCanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnOpenTrashFiles = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RecoverMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePermanentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFileTyle = new System.Windows.Forms.ComboBox();
            this.txtSearchFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbOrderFiled = new System.Windows.Forms.ComboBox();
            this.cmbOrderType = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 135);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(557, 292);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop2);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            this.listView1.DragOver += new System.Windows.Forms.DragEventHandler(this.listView1_DragOver);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.moveToTrashCanToolStripMenuItem,
            this.downloadToToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 92);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // moveToTrashCanToolStripMenuItem
            // 
            this.moveToTrashCanToolStripMenuItem.Name = "moveToTrashCanToolStripMenuItem";
            this.moveToTrashCanToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.moveToTrashCanToolStripMenuItem.Text = "Move to trash can";
            this.moveToTrashCanToolStripMenuItem.Click += new System.EventHandler(this.moveToTrashCanToolStripMenuItem_Click);
            // 
            // downloadToToolStripMenuItem
            // 
            this.downloadToToolStripMenuItem.Name = "downloadToToolStripMenuItem";
            this.downloadToToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.downloadToToolStripMenuItem.Text = "Download to";
            this.downloadToToolStripMenuItem.Click += new System.EventHandler(this.dowloadToToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(560, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(430, 80);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnOpenTrashFiles
            // 
            this.btnOpenTrashFiles.Location = new System.Drawing.Point(340, 80);
            this.btnOpenTrashFiles.Name = "btnOpenTrashFiles";
            this.btnOpenTrashFiles.Size = new System.Drawing.Size(75, 23);
            this.btnOpenTrashFiles.TabIndex = 4;
            this.btnOpenTrashFiles.Text = "Trash";
            this.btnOpenTrashFiles.UseVisualStyleBackColor = true;
            this.btnOpenTrashFiles.Click += new System.EventHandler(this.btnOpenTrashFiles_Click);
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(260, 80);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 23);
            this.btnHome.TabIndex = 5;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RecoverMenu,
            this.deletePermanentToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(169, 48);
            // 
            // RecoverMenu
            // 
            this.RecoverMenu.Name = "RecoverMenu";
            this.RecoverMenu.Size = new System.Drawing.Size(168, 22);
            this.RecoverMenu.Text = "Recover";
            // 
            // deletePermanentToolStripMenuItem
            // 
            this.deletePermanentToolStripMenuItem.Name = "deletePermanentToolStripMenuItem";
            this.deletePermanentToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.deletePermanentToolStripMenuItem.Text = "Delete Permanent";
            this.deletePermanentToolStripMenuItem.Click += new System.EventHandler(this.deletePermanentToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbFileTyle);
            this.groupBox1.Controls.Add(this.txtSearchFileName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(542, 71);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(328, 14);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(58, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(163, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // cmbFileTyle
            // 
            this.cmbFileTyle.FormattingEnabled = true;
            this.cmbFileTyle.Items.AddRange(new object[] {
            "All",
            "File",
            "Folder"});
            this.cmbFileTyle.Location = new System.Drawing.Point(212, 17);
            this.cmbFileTyle.Margin = new System.Windows.Forms.Padding(2);
            this.cmbFileTyle.Name = "cmbFileTyle";
            this.cmbFileTyle.Size = new System.Drawing.Size(83, 21);
            this.cmbFileTyle.TabIndex = 2;
            this.cmbFileTyle.SelectedIndexChanged += new System.EventHandler(this.cmbFileTyle_SelectedIndexChanged);
            // 
            // txtSearchFileName
            // 
            this.txtSearchFileName.Location = new System.Drawing.Point(58, 19);
            this.txtSearchFileName.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearchFileName.Name = "txtSearchFileName";
            this.txtSearchFileName.Size = new System.Drawing.Size(76, 20);
            this.txtSearchFileName.TabIndex = 1;
            this.txtSearchFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(351, 119);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Order by";
            // 
            // cmbOrderFiled
            // 
            this.cmbOrderFiled.FormattingEnabled = true;
            this.cmbOrderFiled.Items.AddRange(new object[] {
            "createdTime",
            "folder",
            "modifiedTime",
            "name"});
            this.cmbOrderFiled.Location = new System.Drawing.Point(400, 112);
            this.cmbOrderFiled.Margin = new System.Windows.Forms.Padding(2);
            this.cmbOrderFiled.Name = "cmbOrderFiled";
            this.cmbOrderFiled.Size = new System.Drawing.Size(92, 21);
            this.cmbOrderFiled.TabIndex = 8;
            this.cmbOrderFiled.SelectedIndexChanged += new System.EventHandler(this.cmbOrderFiled_SelectedIndexChanged);
            // 
            // cmbOrderType
            // 
            this.cmbOrderType.FormattingEnabled = true;
            this.cmbOrderType.Items.AddRange(new object[] {
            "desc",
            "asc"});
            this.cmbOrderType.Location = new System.Drawing.Point(498, 112);
            this.cmbOrderType.Margin = new System.Windows.Forms.Padding(2);
            this.cmbOrderType.Name = "cmbOrderType";
            this.cmbOrderType.Size = new System.Drawing.Size(54, 21);
            this.cmbOrderType.TabIndex = 9;
            this.cmbOrderType.SelectedIndexChanged += new System.EventHandler(this.cmbOrderType_SelectedIndexChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(272, 110);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(35, 13);
            this.labelStatus.TabIndex = 10;
            this.labelStatus.Text = "label4";
            // 
            // ProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 431);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.cmbOrderType);
            this.Controls.Add(this.cmbOrderFiled);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnOpenTrashFiles);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.listView1);
            this.Name = "ProcessForm";
            this.Text = "ProcessForm";
            this.Load += new System.EventHandler(this.ProcessForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.ToolStripMenuItem moveToTrashCanToolStripMenuItem;
        private System.Windows.Forms.Button btnOpenTrashFiles;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem RecoverMenu;
        private System.Windows.Forms.ToolStripMenuItem deletePermanentToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSearchFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFileTyle;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbOrderFiled;
        private System.Windows.Forms.ComboBox cmbOrderType;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ToolStripMenuItem downloadToToolStripMenuItem;
    }
}