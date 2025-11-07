namespace Team_Project_Chat
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
            lblLoggedIn = new Label();
            listBox1 = new ListBox();
            btnSend = new Button();
            txtMessage = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20F);
            label1.Location = new Point(81, 31);
            label1.Name = "label1";
            label1.Size = new Size(244, 37);
            label1.TabIndex = 0;
            label1.Text = "CHAT TCP - CLIENT";
            // 
            // lblLoggedIn
            // 
            lblLoggedIn.AutoSize = true;
            lblLoggedIn.Font = new Font("Segoe UI", 15F);
            lblLoggedIn.Location = new Point(331, 38);
            lblLoggedIn.Name = "lblLoggedIn";
            lblLoggedIn.Size = new Size(192, 28);
            lblLoggedIn.TabIndex = 1;
            lblLoggedIn.Text = "Logged in: Unknown";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(12, 71);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(576, 454);
            listBox1.TabIndex = 2;
            // 
            // btnSend
            // 
            btnSend.Font = new Font("Segoe UI", 15F);
            btnSend.Location = new Point(448, 531);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(140, 57);
            btnSend.TabIndex = 3;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(12, 531);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(430, 57);
            txtMessage.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 598);
            Controls.Add(txtMessage);
            Controls.Add(btnSend);
            Controls.Add(listBox1);
            Controls.Add(lblLoggedIn);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label lblLoggedIn;
        private ListBox listBox1;
        private Button btnSend;
        private TextBox txtMessage;
    }
}
