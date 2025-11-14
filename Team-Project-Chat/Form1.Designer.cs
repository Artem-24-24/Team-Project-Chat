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
            lblLoggedIn = new Label();
            btnSend = new Button();
            txtMessage = new TextBox();
            lblUserList = new Label();
            lblChatWith = new Label();
            listBoxUsers = new ListBox();
            listBoxMessages = new ListBox();
            SuspendLayout();
            // 
            // lblLoggedIn
            // 
            lblLoggedIn.AutoSize = true;
            lblLoggedIn.Font = new Font("Segoe UI", 12F);
            lblLoggedIn.Location = new Point(10, 7);
            lblLoggedIn.Name = "lblLoggedIn";
            lblLoggedIn.Size = new Size(116, 21);
            lblLoggedIn.TabIndex = 1;
            lblLoggedIn.Text = "User: Unknown";
            // 
            // btnSend
            // 
            btnSend.Font = new Font("Segoe UI", 12F);
            btnSend.Location = new Point(554, 450);
            btnSend.Margin = new Padding(3, 2, 3, 2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(111, 56);
            btnSend.TabIndex = 4;
            btnSend.Text = "Відправити";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(192, 450);
            txtMessage.Margin = new Padding(3, 2, 3, 2);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(357, 56);
            txtMessage.TabIndex = 5;
            txtMessage.KeyPress += txtMessage_KeyPress;
            // 
            // lblUserList
            // 
            lblUserList.AutoSize = true;
            lblUserList.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblUserList.Location = new Point(36, 89);
            lblUserList.Name = "lblUserList";
            lblUserList.Size = new Size(107, 21);
            lblUserList.TabIndex = 6;
            lblUserList.Text = "Користувачі";
            // 
            // lblChatWith
            // 
            lblChatWith.AutoSize = true;
            lblChatWith.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChatWith.Location = new Point(331, 89);
            lblChatWith.Name = "lblChatWith";
            lblChatWith.Size = new Size(184, 21);
            lblChatWith.TabIndex = 7;
            lblChatWith.Text = "Виберіть користувача";
            // 
            // listBoxUsers
            // 
            listBoxUsers.FormattingEnabled = true;
            listBoxUsers.ItemHeight = 15;
            listBoxUsers.Location = new Point(10, 112);
            listBoxUsers.Margin = new Padding(3, 2, 3, 2);
            listBoxUsers.Name = "listBoxUsers";
            listBoxUsers.Size = new Size(177, 394);
            listBoxUsers.TabIndex = 2;
            listBoxUsers.SelectedIndexChanged += listBoxUsers_SelectedIndexChanged;
            // 
            // listBoxMessages
            // 
            listBoxMessages.FormattingEnabled = true;
            listBoxMessages.ItemHeight = 15;
            listBoxMessages.Location = new Point(192, 112);
            listBoxMessages.Margin = new Padding(3, 2, 3, 2);
            listBoxMessages.Name = "listBoxMessages";
            listBoxMessages.Size = new Size(473, 334);
            listBoxMessages.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(682, 518);
            Controls.Add(lblChatWith);
            Controls.Add(lblUserList);
            Controls.Add(txtMessage);
            Controls.Add(btnSend);
            Controls.Add(listBoxMessages);
            Controls.Add(listBoxUsers);
            Controls.Add(lblLoggedIn);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Chat Client";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblLoggedIn;
        private ListBox listBoxUsers;
        private ListBox listBoxMessages;
        private Button btnSend;
        private TextBox txtMessage;
        private Label lblUserList;
        private Label lblChatWith;
    }
}
