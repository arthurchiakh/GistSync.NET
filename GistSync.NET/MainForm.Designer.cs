namespace GistSync.NET
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dgv_SyncTasks = new System.Windows.Forms.DataGridView();
            this.GistId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SyncMode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdatedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Directory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Add = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_SaveToFile = new System.Windows.Forms.Button();
            this.rtb_ActivityLog = new System.Windows.Forms.RichTextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.nofityIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncTasks)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.nofityIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_SyncTasks
            // 
            this.dgv_SyncTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_SyncTasks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dgv_SyncTasks.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgv_SyncTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SyncTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GistId,
            this.Enabled,
            this.SyncMode,
            this.UpdatedAt,
            this.Directory});
            this.dgv_SyncTasks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dgv_SyncTasks.GridColor = System.Drawing.SystemColors.Window;
            this.dgv_SyncTasks.Location = new System.Drawing.Point(6, 40);
            this.dgv_SyncTasks.Name = "dgv_SyncTasks";
            this.dgv_SyncTasks.ReadOnly = true;
            this.dgv_SyncTasks.RowTemplate.Height = 25;
            this.dgv_SyncTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_SyncTasks.Size = new System.Drawing.Size(666, 457);
            this.dgv_SyncTasks.TabIndex = 0;
            this.dgv_SyncTasks.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dgv_SyncTasks_CellContextMenuStripNeeded);
            // 
            // GistId
            // 
            this.GistId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.GistId.DataPropertyName = "GistId";
            this.GistId.HeaderText = "Gist Id";
            this.GistId.Name = "GistId";
            this.GistId.ReadOnly = true;
            this.GistId.Width = 65;
            // 
            // Enabled
            // 
            this.Enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Enabled.DataPropertyName = "IsEnabled";
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.Name = "Enabled";
            this.Enabled.ReadOnly = true;
            this.Enabled.Width = 74;
            // 
            // SyncMode
            // 
            this.SyncMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SyncMode.DataPropertyName = "SyncMode";
            this.SyncMode.HeaderText = "SyncMode";
            this.SyncMode.Name = "SyncMode";
            this.SyncMode.ReadOnly = true;
            this.SyncMode.Width = 88;
            // 
            // UpdatedAt
            // 
            this.UpdatedAt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.UpdatedAt.DataPropertyName = "UpdatedAt";
            this.UpdatedAt.HeaderText = "Updated At";
            this.UpdatedAt.Name = "UpdatedAt";
            this.UpdatedAt.ReadOnly = true;
            this.UpdatedAt.Width = 92;
            // 
            // Directory
            // 
            this.Directory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Directory.DataPropertyName = "Directory";
            this.Directory.HeaderText = "Directory";
            this.Directory.Name = "Directory";
            this.Directory.ReadOnly = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Add.Location = new System.Drawing.Point(3, 3);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(75, 23);
            this.btn_Add.TabIndex = 1;
            this.btn_Add.Text = "Add";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btn_Add);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(666, 28);
            this.flowLayoutPanel1.TabIndex = 2;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 531);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flowLayoutPanel1);
            this.tabPage1.Controls.Add(this.dgv_SyncTasks);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(678, 503);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tasks";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_SaveToFile);
            this.tabPage2.Controls.Add(this.rtb_ActivityLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(678, 503);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Activity Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_SaveToFile
            // 
            this.btn_SaveToFile.Location = new System.Drawing.Point(597, 474);
            this.btn_SaveToFile.Name = "btn_SaveToFile";
            this.btn_SaveToFile.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveToFile.TabIndex = 1;
            this.btn_SaveToFile.Text = "Save to File";
            this.btn_SaveToFile.UseVisualStyleBackColor = true;
            this.btn_SaveToFile.Click += new System.EventHandler(this.btn_SaveToFile_Click);
            // 
            // rtb_ActivityLog
            // 
            this.rtb_ActivityLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtb_ActivityLog.Location = new System.Drawing.Point(6, 6);
            this.rtb_ActivityLog.Name = "rtb_ActivityLog";
            this.rtb_ActivityLog.ReadOnly = true;
            this.rtb_ActivityLog.Size = new System.Drawing.Size(666, 462);
            this.rtb_ActivityLog.TabIndex = 0;
            this.rtb_ActivityLog.Text = "";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.nofityIconContextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "GistSync.NET";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // nofityIconContextMenu
            // 
            this.nofityIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.nofityIconContextMenu.Name = "nofityIconContextMenu";
            this.nofityIconContextMenu.Size = new System.Drawing.Size(94, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 548);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "GistSync.NET";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncTasks)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.nofityIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Button btn_Add;
        private DataGridView dgv_SyncTasks;
        private DataGridViewTextBoxColumn Id;
        private FlowLayoutPanel flowLayoutPanel1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip nofityIconContextMenu;
        private ToolStripMenuItem exitToolStripMenuItem;
        private RichTextBox rtb_ActivityLog;
        private DataGridViewTextBoxColumn GistId;
        private DataGridViewTextBoxColumn Enabled;
        private DataGridViewTextBoxColumn SyncMode;
        private DataGridViewTextBoxColumn UpdatedAt;
        private DataGridViewTextBoxColumn Directory;
        private Button btn_SaveToFile;
        private SaveFileDialog saveFileDialog;
    }
}