namespace GistSync.NET
{
    partial class SettingsForm
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
            this.tlp_Setttings = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_StatusRefreshIntervalSeconds = new System.Windows.Forms.NumericUpDown();
            this.tb_MaxConcurrentGistStatusCheck = new System.Windows.Forms.NumericUpDown();
            this.btn_Save = new System.Windows.Forms.Button();
            this.tlp_Setttings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StatusRefreshIntervalSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_MaxConcurrentGistStatusCheck)).BeginInit();
            this.SuspendLayout();
            // 
            // tlp_Setttings
            // 
            this.tlp_Setttings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tlp_Setttings.ColumnCount = 2;
            this.tlp_Setttings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 219F));
            this.tlp_Setttings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Setttings.Controls.Add(this.label1, 0, 0);
            this.tlp_Setttings.Controls.Add(this.label2, 0, 1);
            this.tlp_Setttings.Controls.Add(this.tb_StatusRefreshIntervalSeconds, 1, 0);
            this.tlp_Setttings.Controls.Add(this.tb_MaxConcurrentGistStatusCheck, 1, 1);
            this.tlp_Setttings.Location = new System.Drawing.Point(12, 13);
            this.tlp_Setttings.Name = "tlp_Setttings";
            this.tlp_Setttings.RowCount = 3;
            this.tlp_Setttings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_Setttings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_Setttings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Setttings.Size = new System.Drawing.Size(547, 77);
            this.tlp_Setttings.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status Refresh Interval (seconds):";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Maximum Concurrent Status Check:";
            // 
            // tb_StatusRefreshIntervalSeconds
            // 
            this.tb_StatusRefreshIntervalSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_StatusRefreshIntervalSeconds.Location = new System.Drawing.Point(222, 6);
            this.tb_StatusRefreshIntervalSeconds.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.tb_StatusRefreshIntervalSeconds.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.tb_StatusRefreshIntervalSeconds.Name = "tb_StatusRefreshIntervalSeconds";
            this.tb_StatusRefreshIntervalSeconds.Size = new System.Drawing.Size(322, 23);
            this.tb_StatusRefreshIntervalSeconds.TabIndex = 4;
            this.tb_StatusRefreshIntervalSeconds.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // tb_MaxConcurrentGistStatusCheck
            // 
            this.tb_MaxConcurrentGistStatusCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_MaxConcurrentGistStatusCheck.Location = new System.Drawing.Point(222, 41);
            this.tb_MaxConcurrentGistStatusCheck.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tb_MaxConcurrentGistStatusCheck.Name = "tb_MaxConcurrentGistStatusCheck";
            this.tb_MaxConcurrentGistStatusCheck.Size = new System.Drawing.Size(322, 23);
            this.tb_MaxConcurrentGistStatusCheck.TabIndex = 5;
            this.tb_MaxConcurrentGistStatusCheck.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.Location = new System.Drawing.Point(484, 99);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 130);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.tlp_Setttings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tlp_Setttings.ResumeLayout(false);
            this.tlp_Setttings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StatusRefreshIntervalSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_MaxConcurrentGistStatusCheck)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlp_Setttings;
        private Button btn_Save;
        private Label label1;
        private Label label2;
        private NumericUpDown tb_StatusRefreshIntervalSeconds;
        private NumericUpDown tb_MaxConcurrentGistStatusCheck;
    }
}