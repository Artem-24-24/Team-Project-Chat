namespace Team_Project_Chat
{
    partial class From_Login
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
            tb_login = new TextBox();
            tb_password = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_login = new Button();
            SuspendLayout();
            // 
            // tb_login
            // 
            tb_login.Location = new Point(473, 247);
            tb_login.Name = "tb_login";
            tb_login.Size = new Size(205, 27);
            tb_login.TabIndex = 0;
            // 
            // tb_password
            // 
            tb_password.Location = new Point(473, 338);
            tb_password.Name = "tb_password";
            tb_password.Size = new Size(205, 27);
            tb_password.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(473, 224);
            label1.Name = "label1";
            label1.Size = new Size(47, 20);
            label1.TabIndex = 2;
            label1.Text = "Логін";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(473, 315);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 3;
            label2.Text = "Пароль";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 42);
            label3.Name = "label3";
            label3.Size = new Size(50, 20);
            label3.TabIndex = 4;
            label3.Text = "label3";
            // 
            // btn_login
            // 
            btn_login.DialogResult = DialogResult.OK;
            btn_login.Location = new Point(473, 413);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(205, 60);
            btn_login.TabIndex = 5;
            btn_login.Text = "Увійти";
            btn_login.UseVisualStyleBackColor = true;
            // 
            // From_Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1163, 675);
            Controls.Add(btn_login);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb_password);
            Controls.Add(tb_login);
            Name = "From_Login";
            Text = "From_Login";
            Load += From_Login_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public TextBox tb_login;
        private TextBox tb_password;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_login;
    }
}