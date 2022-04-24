namespace GistSync.NET
{
    partial class NewTaskForm
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
            this.tb_GistId = new System.Windows.Forms.TextBox();
            this.tlp_NewTask = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tb_Path = new System.Windows.Forms.TextBox();
            this.btn_Browse = new System.Windows.Forms.Button();
            this.cb_SyncMode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbl_Files = new System.Windows.Forms.CheckedListBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_PersonalAccessToken = new System.Windows.Forms.TextBox();
            this.ll_PersonalAccessToken = new System.Windows.Forms.LinkLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tlp_NewTask.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gist Id/Url:";
            // 
            // tb_GistId
            // 
            this.tb_GistId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_GistId.Location = new System.Drawing.Point(153, 6);
            this.tb_GistId.Name = "tb_GistId";
            this.tb_GistId.Size = new System.Drawing.Size(406, 23);
            this.tb_GistId.TabIndex = 1;
            this.tb_GistId.TextChanged += new System.EventHandler(this.tb_GistId_TextChanged);
            // 
            // tlp_NewTask
            // 
            this.tlp_NewTask.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlp_NewTask.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp_NewTask.ColumnCount = 2;
            this.tlp_NewTask.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_NewTask.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_NewTask.Controls.Add(this.label2, 0, 1);
            this.tlp_NewTask.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tlp_NewTask.Controls.Add(this.tb_GistId, 1, 0);
            this.tlp_NewTask.Controls.Add(this.label1, 0, 0);
            this.tlp_NewTask.Controls.Add(this.cb_SyncMode, 1, 1);
            this.tlp_NewTask.Controls.Add(this.label3, 0, 2);
            this.tlp_NewTask.Controls.Add(this.cbl_Files, 1, 3);
            this.tlp_NewTask.Controls.Add(this.btn_Add, 1, 5);
            this.tlp_NewTask.Controls.Add(this.label4, 0, 3);
            this.tlp_NewTask.Controls.Add(this.tb_PersonalAccessToken, 1, 4);
            this.tlp_NewTask.Controls.Add(this.ll_PersonalAccessToken, 0, 4);
            this.tlp_NewTask.Location = new System.Drawing.Point(12, 12);
            this.tlp_NewTask.Name = "tlp_NewTask";
            this.tlp_NewTask.RowCount = 6;
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_NewTask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_NewTask.Size = new System.Drawing.Size(562, 340);
            this.tlp_NewTask.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sync Mode:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel2.Controls.Add(this.tb_Path, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btn_Browse, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(153, 73);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(406, 29);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // tb_Path
            // 
            this.tb_Path.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_Path.Location = new System.Drawing.Point(3, 3);
            this.tb_Path.Name = "tb_Path";
            this.tb_Path.Size = new System.Drawing.Size(325, 23);
            this.tb_Path.TabIndex = 0;
            this.tb_Path.WordWrap = false;
            // 
            // btn_Browse
            // 
            this.btn_Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Browse.Location = new System.Drawing.Point(334, 3);
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Size = new System.Drawing.Size(69, 23);
            this.btn_Browse.TabIndex = 1;
            this.btn_Browse.Text = "Browse";
            this.btn_Browse.UseVisualStyleBackColor = true;
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // cb_SyncMode
            // 
            this.cb_SyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_SyncMode.FormattingEnabled = true;
            this.cb_SyncMode.Location = new System.Drawing.Point(153, 41);
            this.cb_SyncMode.Name = "cb_SyncMode";
            this.cb_SyncMode.Size = new System.Drawing.Size(406, 23);
            this.cb_SyncMode.TabIndex = 3;
            this.cb_SyncMode.SelectedIndexChanged += new System.EventHandler(this.cb_SyncMode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Path:";
            // 
            // cbl_Files
            // 
            this.cbl_Files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbl_Files.CheckOnClick = true;
            this.cbl_Files.FormattingEnabled = true;
            this.cbl_Files.Location = new System.Drawing.Point(153, 108);
            this.cbl_Files.Name = "cbl_Files";
            this.cbl_Files.Size = new System.Drawing.Size(406, 148);
            this.cbl_Files.TabIndex = 6;
            this.cbl_Files.ThreeDCheckBoxes = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Add.Enabled = false;
            this.btn_Add.Location = new System.Drawing.Point(481, 310);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(78, 25);
            this.btn_Add.TabIndex = 3;
            this.btn_Add.Text = "Add";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Files:";
            // 
            // tb_PersonalAccessToken
            // 
            this.tb_PersonalAccessToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_PersonalAccessToken.Enabled = false;
            this.tb_PersonalAccessToken.Location = new System.Drawing.Point(153, 276);
            this.tb_PersonalAccessToken.Name = "tb_PersonalAccessToken";
            this.tb_PersonalAccessToken.Size = new System.Drawing.Size(406, 23);
            this.tb_PersonalAccessToken.TabIndex = 9;
            // 
            // ll_PersonalAccessToken
            // 
            this.ll_PersonalAccessToken.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ll_PersonalAccessToken.AutoSize = true;
            this.ll_PersonalAccessToken.Location = new System.Drawing.Point(19, 280);
            this.ll_PersonalAccessToken.Name = "ll_PersonalAccessToken";
            this.ll_PersonalAccessToken.Size = new System.Drawing.Size(128, 15);
            this.ll_PersonalAccessToken.TabIndex = 10;
            this.ll_PersonalAccessToken.TabStop = true;
            this.ll_PersonalAccessToken.Text = "Personal Access Token:";
            this.ll_PersonalAccessToken.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ll_PersonalAccessToken_LinkClicked);
            // 
            // NewTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.tlp_NewTask);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "NewTaskForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Task";
            this.tlp_NewTask.ResumeLayout(false);
            this.tlp_NewTask.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private TextBox tb_GistId;
        private TableLayoutPanel tlp_NewTask;
        private Label label2;
        private ComboBox cb_SyncMode;
        private Label label3;
        private FolderBrowserDialog folderBrowserDialog1;
        private TextBox tb_Path;
        private Button btn_Browse;
        private Label label4;
        private CheckedListBox cbl_Files;
        private Button btn_Add;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox tb_PersonalAccessToken;
        private LinkLabel ll_PersonalAccessToken;
    }
}