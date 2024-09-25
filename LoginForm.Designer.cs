namespace SLIL
{
    partial class LoginForm
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
            this.create_account = new System.Windows.Forms.Button();
            this.login_btn = new System.Windows.Forms.Button();
            this.password_label = new System.Windows.Forms.Label();
            this.password_input = new System.Windows.Forms.TextBox();
            this.nickname_input_label = new System.Windows.Forms.Label();
            this.nickname_input = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hide_show_pas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // create_account
            // 
            this.create_account.AutoSize = true;
            this.create_account.BackColor = System.Drawing.Color.Black;
            this.create_account.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.create_account.ForeColor = System.Drawing.Color.White;
            this.create_account.Location = new System.Drawing.Point(12, 188);
            this.create_account.Name = "create_account";
            this.create_account.Size = new System.Drawing.Size(170, 34);
            this.create_account.TabIndex = 72;
            this.create_account.TabStop = false;
            this.create_account.Text = "Создать аккаунт";
            this.create_account.UseVisualStyleBackColor = false;
            this.create_account.Click += new System.EventHandler(this.Create_account_Click);
            // 
            // login_btn
            // 
            this.login_btn.AutoSize = true;
            this.login_btn.BackColor = System.Drawing.Color.Black;
            this.login_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.login_btn.ForeColor = System.Drawing.Color.White;
            this.login_btn.Location = new System.Drawing.Point(302, 181);
            this.login_btn.Name = "login_btn";
            this.login_btn.Size = new System.Drawing.Size(140, 41);
            this.login_btn.TabIndex = 71;
            this.login_btn.TabStop = false;
            this.login_btn.Text = "Войти";
            this.login_btn.UseVisualStyleBackColor = false;
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.password_label.ForeColor = System.Drawing.Color.White;
            this.password_label.Location = new System.Drawing.Point(12, 138);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(105, 29);
            this.password_label.TabIndex = 70;
            this.password_label.Text = "Пароль:";
            this.password_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // password_input
            // 
            this.password_input.Location = new System.Drawing.Point(123, 138);
            this.password_input.MaxLength = 24;
            this.password_input.Name = "password_input";
            this.password_input.Size = new System.Drawing.Size(213, 29);
            this.password_input.TabIndex = 69;
            this.password_input.TabStop = false;
            this.password_input.UseSystemPasswordChar = true;
            // 
            // nickname_input_label
            // 
            this.nickname_input_label.AutoSize = true;
            this.nickname_input_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nickname_input_label.ForeColor = System.Drawing.Color.White;
            this.nickname_input_label.Location = new System.Drawing.Point(28, 90);
            this.nickname_input_label.Name = "nickname_input_label";
            this.nickname_input_label.Size = new System.Drawing.Size(89, 29);
            this.nickname_input_label.TabIndex = 68;
            this.nickname_input_label.Text = "Логин:";
            this.nickname_input_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nickname_input
            // 
            this.nickname_input.Location = new System.Drawing.Point(123, 91);
            this.nickname_input.MaxLength = 14;
            this.nickname_input.Name = "nickname_input";
            this.nickname_input.Size = new System.Drawing.Size(213, 29);
            this.nickname_input.TabIndex = 67;
            this.nickname_input.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(447, 88);
            this.label2.TabIndex = 66;
            this.label2.Text = "Чтобы начать играть пожалуйста зайдите в свой аккаунт";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hide_show_pas
            // 
            this.hide_show_pas.BackColor = System.Drawing.Color.Black;
            this.hide_show_pas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.hide_show_pas.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.hide_show_pas.ForeColor = System.Drawing.Color.White;
            this.hide_show_pas.Location = new System.Drawing.Point(342, 127);
            this.hide_show_pas.Name = "hide_show_pas";
            this.hide_show_pas.Size = new System.Drawing.Size(40, 40);
            this.hide_show_pas.TabIndex = 73;
            this.hide_show_pas.TabStop = false;
            this.hide_show_pas.UseVisualStyleBackColor = false;
            this.hide_show_pas.Click += new System.EventHandler(this.Hide_show_pas_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(447, 234);
            this.Controls.Add(this.hide_show_pas);
            this.Controls.Add(this.create_account);
            this.Controls.Add(this.login_btn);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.password_input);
            this.Controls.Add(this.nickname_input_label);
            this.Controls.Add(this.nickname_input);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label nickname_input_label;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button login_btn;
        public System.Windows.Forms.TextBox password_input;
        public System.Windows.Forms.TextBox nickname_input;
        public System.Windows.Forms.Button hide_show_pas;
        private System.Windows.Forms.Button create_account;
    }
}