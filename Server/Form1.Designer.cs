namespace Server
{
    partial class Form1
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
            label1 = new Label();
            lblStatus = new Label();
            lblPort = new Label();
            txtPort = new TextBox();
            label2 = new Label();
            lblClients = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnClearLog = new Button();
            label3 = new Label();
            listBoxLog = new ListBox();
            label4 = new Label();
            dataGridClients = new DataGridView();
            btnRefresh = new Button();
            btnBan = new Button();
            btnKick = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridClients).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20F);
            label1.Location = new Point(59, 9);
            label1.Name = "label1";
            label1.Size = new Size(441, 37);
            label1.TabIndex = 0;
            label1.Text = "CHAT TCP - SERVER (ARMIN PANEL)";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 12F);
            lblStatus.Location = new Point(12, 80);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(103, 21);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Server status:";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 12F);
            lblPort.Location = new Point(12, 111);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(91, 21);
            lblPort.TabIndex = 2;
            lblPort.Text = "Server port:";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(109, 109);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(53, 23);
            txtPort.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(236, 115);
            label2.Name = "label2";
            label2.Size = new Size(75, 21);
            label2.TabIndex = 4;
            label2.Text = "Server IP:";
            // 
            // lblClients
            // 
            lblClients.AutoSize = true;
            lblClients.Font = new Font("Segoe UI", 12F);
            lblClients.Location = new Point(12, 144);
            lblClients.Name = "lblClients";
            lblClients.Size = new Size(135, 21);
            lblClients.TabIndex = 5;
            lblClients.Text = "Clients connected:";
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 12F);
            btnStart.Location = new Point(12, 213);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(178, 59);
            btnStart.TabIndex = 6;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 12F);
            btnStop.Location = new Point(196, 213);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(178, 59);
            btnStop.TabIndex = 7;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // btnClearLog
            // 
            btnClearLog.Font = new Font("Segoe UI", 12F);
            btnClearLog.Location = new Point(378, 213);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(178, 59);
            btnClearLog.TabIndex = 8;
            btnClearLog.Text = "Clear log";
            btnClearLog.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 20F);
            label3.Location = new Point(243, 284);
            label3.Name = "label3";
            label3.Size = new Size(68, 37);
            label3.TabIndex = 9;
            label3.Text = "LOG";
            // 
            // listBoxLog
            // 
            listBoxLog.FormattingEnabled = true;
            listBoxLog.ItemHeight = 15;
            listBoxLog.Location = new Point(12, 334);
            listBoxLog.Name = "listBoxLog";
            listBoxLog.Size = new Size(544, 199);
            listBoxLog.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 552);
            label4.Name = "label4";
            label4.Size = new Size(138, 21);
            label4.TabIndex = 11;
            label4.Text = "Connected Clients:";
            // 
            // dataGridClients
            // 
            dataGridClients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridClients.Location = new Point(12, 576);
            dataGridClients.Name = "dataGridClients";
            dataGridClients.Size = new Size(544, 150);
            dataGridClients.TabIndex = 12;
            // 
            // btnRefresh
            // 
            btnRefresh.Font = new Font("Segoe UI", 12F);
            btnRefresh.Location = new Point(378, 732);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(178, 59);
            btnRefresh.TabIndex = 15;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnBan
            // 
            btnBan.Font = new Font("Segoe UI", 12F);
            btnBan.Location = new Point(196, 732);
            btnBan.Name = "btnBan";
            btnBan.Size = new Size(178, 59);
            btnBan.TabIndex = 14;
            btnBan.Text = "Ban";
            btnBan.UseVisualStyleBackColor = true;
            // 
            // btnKick
            // 
            btnKick.Font = new Font("Segoe UI", 12F);
            btnKick.Location = new Point(12, 732);
            btnKick.Name = "btnKick";
            btnKick.Size = new Size(178, 59);
            btnKick.TabIndex = 13;
            btnKick.Text = "Kick";
            btnKick.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(568, 798);
            Controls.Add(btnRefresh);
            Controls.Add(btnBan);
            Controls.Add(btnKick);
            Controls.Add(dataGridClients);
            Controls.Add(label4);
            Controls.Add(listBoxLog);
            Controls.Add(label3);
            Controls.Add(btnClearLog);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(lblClients);
            Controls.Add(label2);
            Controls.Add(txtPort);
            Controls.Add(lblPort);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridClients).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label lblStatus;
        private Label lblPort;
        private TextBox txtPort;
        private Label label2;
        private Label lblClients;
        private Button btnStart;
        private Button btnStop;
        private Button btnClearLog;
        private Label label3;
        private ListBox listBoxLog;
        private Label label4;
        private DataGridView dataGridClients;
        private Button btnRefresh;
        private Button btnBan;
        private Button btnKick;
    }
}
