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
            label1.Location = new Point(67, 12);
            label1.Name = "label1";
            label1.Size = new Size(555, 46);
            label1.TabIndex = 0;
            label1.Text = "CHAT TCP - SERVER (ARMIN PANEL)";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 12F);
            lblStatus.Location = new Point(14, 107);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(127, 28);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Server status:";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 12F);
            lblPort.Location = new Point(14, 148);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(114, 28);
            lblPort.TabIndex = 2;
            lblPort.Text = "Server port:";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(125, 145);
            txtPort.Margin = new Padding(3, 4, 3, 4);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(60, 27);
            txtPort.TabIndex = 3;
            txtPort.TextChanged += txtPort_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(270, 153);
            label2.Name = "label2";

            label2.Size = new Size(142, 21);

            //label2.Size = new Size(92, 28);

            label2.TabIndex = 4;
            label2.Text = "Server IP: 127.0.0.1";
            // 
            // lblClients
            // 
            lblClients.AutoSize = true;
            lblClients.Font = new Font("Segoe UI", 12F);
            lblClients.Location = new Point(14, 192);
            lblClients.Name = "lblClients";
            lblClients.Size = new Size(170, 28);
            lblClients.TabIndex = 5;
            lblClients.Text = "Clients connected:";
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 12F);
            btnStart.Location = new Point(14, 284);
            btnStart.Margin = new Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(203, 79);
            btnStart.TabIndex = 6;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 12F);
            btnStop.Location = new Point(224, 284);
            btnStop.Margin = new Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(203, 79);
            btnStop.TabIndex = 7;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnClearLog
            // 
            btnClearLog.Font = new Font("Segoe UI", 12F);
            btnClearLog.Location = new Point(432, 284);
            btnClearLog.Margin = new Padding(3, 4, 3, 4);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(203, 79);
            btnClearLog.TabIndex = 8;
            btnClearLog.Text = "Clear log";
            btnClearLog.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 20F);
            label3.Location = new Point(278, 379);
            label3.Name = "label3";
            label3.Size = new Size(84, 46);
            label3.TabIndex = 9;
            label3.Text = "LOG";
            // 
            // listBoxLog
            // 
            listBoxLog.FormattingEnabled = true;
            listBoxLog.Location = new Point(14, 445);
            listBoxLog.Margin = new Padding(3, 4, 3, 4);
            listBoxLog.Name = "listBoxLog";
            listBoxLog.Size = new Size(621, 264);
            listBoxLog.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(14, 736);
            label4.Name = "label4";
            label4.Size = new Size(173, 28);
            label4.TabIndex = 11;
            label4.Text = "Connected Clients:";
            // 
            // dataGridClients
            // 
            dataGridClients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridClients.Location = new Point(14, 768);
            dataGridClients.Margin = new Padding(3, 4, 3, 4);
            dataGridClients.Name = "dataGridClients";
            dataGridClients.RowHeadersWidth = 51;
            dataGridClients.Size = new Size(622, 200);
            dataGridClients.TabIndex = 12;
            // 
            // btnRefresh
            // 
            btnRefresh.Font = new Font("Segoe UI", 12F);
            btnRefresh.Location = new Point(432, 976);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(203, 79);
            btnRefresh.TabIndex = 15;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnBan
            // 
            btnBan.Font = new Font("Segoe UI", 12F);
            btnBan.Location = new Point(224, 976);
            btnBan.Margin = new Padding(3, 4, 3, 4);
            btnBan.Name = "btnBan";
            btnBan.Size = new Size(203, 79);
            btnBan.TabIndex = 14;
            btnBan.Text = "Ban";
            btnBan.UseVisualStyleBackColor = true;
            // 
            // btnKick
            // 
            btnKick.Font = new Font("Segoe UI", 12F);
            btnKick.Location = new Point(14, 976);
            btnKick.Margin = new Padding(3, 4, 3, 4);
            btnKick.Name = "btnKick";
            btnKick.Size = new Size(203, 79);
            btnKick.TabIndex = 13;
            btnKick.Text = "Kick";
            btnKick.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(649, 1055);
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
            Margin = new Padding(3, 4, 3, 4);
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
