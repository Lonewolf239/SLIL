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
            this.create_account_l = new System.Windows.Forms.Button();
            this.login_btn_r = new System.Windows.Forms.Button();
            this.password_label = new System.Windows.Forms.Label();
            this.password_input = new System.Windows.Forms.TextBox();
            this.nickname_input_label = new System.Windows.Forms.Label();
            this.nickname_input = new System.Windows.Forms.TextBox();
            this.title_label = new System.Windows.Forms.Label();
            this.hide_show_pas_c = new System.Windows.Forms.Button();
            this.status_label = new System.Windows.Forms.Label();
            this.login_panel = new System.Windows.Forms.Panel();
            this.error_panel = new System.Windows.Forms.Panel();
            this.exit_btn_cp = new System.Windows.Forms.Button();
            this.error_label = new System.Windows.Forms.Label();
            this.buy_panel = new System.Windows.Forms.Panel();
            this.buy_btn_cp = new System.Windows.Forms.Button();
            this.buy_label = new System.Windows.Forms.Label();
            this.login_panel.SuspendLayout();
            this.error_panel.SuspendLayout();
            this.buy_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // create_account_l
            // 
            this.create_account_l.AutoSize = true;
            this.create_account_l.BackColor = System.Drawing.Color.Black;
            this.create_account_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.create_account_l.ForeColor = System.Drawing.Color.White;
            this.create_account_l.Location = new System.Drawing.Point(12, 188);
            this.create_account_l.Name = "create_account_l";
            this.create_account_l.Size = new System.Drawing.Size(170, 34);
            this.create_account_l.TabIndex = 72;
            this.create_account_l.TabStop = false;
            this.create_account_l.Text = "Создать аккаунт";
            this.create_account_l.UseVisualStyleBackColor = false;
            this.create_account_l.Click += new System.EventHandler(this.Create_account_Click);
            // 
            // login_btn_r
            // 
            this.login_btn_r.AutoSize = true;
            this.login_btn_r.BackColor = System.Drawing.Color.Black;
            this.login_btn_r.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.login_btn_r.ForeColor = System.Drawing.Color.White;
            this.login_btn_r.Location = new System.Drawing.Point(340, 183);
            this.login_btn_r.Name = "login_btn_r";
            this.login_btn_r.Size = new System.Drawing.Size(95, 39);
            this.login_btn_r.TabIndex = 71;
            this.login_btn_r.TabStop = false;
            this.login_btn_r.Text = "Войти";
            this.login_btn_r.UseVisualStyleBackColor = false;
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.password_label.ForeColor = System.Drawing.Color.White;
            this.password_label.Location = new System.Drawing.Point(3, 65);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(71, 20);
            this.password_label.TabIndex = 70;
            this.password_label.Text = "Пароль:";
            this.password_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // password_input
            // 
            this.password_input.Location = new System.Drawing.Point(3, 88);
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
            this.nickname_input_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nickname_input_label.ForeColor = System.Drawing.Color.White;
            this.nickname_input_label.Location = new System.Drawing.Point(3, 9);
            this.nickname_input_label.Name = "nickname_input_label";
            this.nickname_input_label.Size = new System.Drawing.Size(59, 20);
            this.nickname_input_label.TabIndex = 68;
            this.nickname_input_label.Text = "Логин:";
            this.nickname_input_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nickname_input
            // 
            this.nickname_input.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nickname_input.Location = new System.Drawing.Point(3, 32);
            this.nickname_input.MaxLength = 14;
            this.nickname_input.Name = "nickname_input";
            this.nickname_input.Size = new System.Drawing.Size(213, 26);
            this.nickname_input.TabIndex = 67;
            this.nickname_input.TabStop = false;
            // 
            // title_label
            // 
            this.title_label.AutoEllipsis = true;
            this.title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.title_label.ForeColor = System.Drawing.Color.White;
            this.title_label.Location = new System.Drawing.Point(273, 0);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(174, 133);
            this.title_label.TabIndex = 66;
            this.title_label.Text = "Чтобы начать играть, пожалуйста, войдите в свою учетную запись";
            this.title_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hide_show_pas_c
            // 
            this.hide_show_pas_c.BackColor = System.Drawing.Color.Black;
            this.hide_show_pas_c.BackgroundImage = global::SLIL.Properties.Resources.hide_pas;
            this.hide_show_pas_c.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.hide_show_pas_c.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.hide_show_pas_c.ForeColor = System.Drawing.Color.White;
            this.hide_show_pas_c.Location = new System.Drawing.Point(222, 81);
            this.hide_show_pas_c.Name = "hide_show_pas_c";
            this.hide_show_pas_c.Size = new System.Drawing.Size(40, 40);
            this.hide_show_pas_c.TabIndex = 73;
            this.hide_show_pas_c.TabStop = false;
            this.hide_show_pas_c.UseVisualStyleBackColor = false;
            this.hide_show_pas_c.Click += new System.EventHandler(this.Hide_show_pas_Click);
            // 
            // status_label
            // 
            this.status_label.AutoSize = true;
            this.status_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.status_label.ForeColor = System.Drawing.Color.Red;
            this.status_label.Location = new System.Drawing.Point(8, 124);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(229, 20);
            this.status_label.TabIndex = 74;
            this.status_label.Text = "Неверный логин или пароль!";
            this.status_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.status_label.Visible = false;
            // 
            // login_panel
            // 
            this.login_panel.Controls.Add(this.status_label);
            this.login_panel.Controls.Add(this.hide_show_pas_c);
            this.login_panel.Controls.Add(this.create_account_l);
            this.login_panel.Controls.Add(this.login_btn_r);
            this.login_panel.Controls.Add(this.password_label);
            this.login_panel.Controls.Add(this.password_input);
            this.login_panel.Controls.Add(this.nickname_input_label);
            this.login_panel.Controls.Add(this.nickname_input);
            this.login_panel.Controls.Add(this.title_label);
            this.login_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.login_panel.Location = new System.Drawing.Point(0, 0);
            this.login_panel.Name = "login_panel";
            this.login_panel.Size = new System.Drawing.Size(447, 234);
            this.login_panel.TabIndex = 75;
            // 
            // error_panel
            // 
            this.error_panel.Controls.Add(this.exit_btn_cp);
            this.error_panel.Controls.Add(this.error_label);
            this.error_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.error_panel.Location = new System.Drawing.Point(0, 0);
            this.error_panel.Name = "error_panel";
            this.error_panel.Size = new System.Drawing.Size(447, 234);
            this.error_panel.TabIndex = 76;
            this.error_panel.Visible = false;
            // 
            // exit_btn_cp
            // 
            this.exit_btn_cp.AutoSize = true;
            this.exit_btn_cp.BackColor = System.Drawing.Color.Black;
            this.exit_btn_cp.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exit_btn_cp.ForeColor = System.Drawing.Color.White;
            this.exit_btn_cp.Location = new System.Drawing.Point(174, 136);
            this.exit_btn_cp.Name = "exit_btn_cp";
            this.exit_btn_cp.Size = new System.Drawing.Size(98, 39);
            this.exit_btn_cp.TabIndex = 72;
            this.exit_btn_cp.TabStop = false;
            this.exit_btn_cp.Text = "Выйти";
            this.exit_btn_cp.UseVisualStyleBackColor = false;
            // 
            // error_label
            // 
            this.error_label.AutoEllipsis = true;
            this.error_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.error_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.error_label.ForeColor = System.Drawing.Color.White;
            this.error_label.Location = new System.Drawing.Point(0, 0);
            this.error_label.Name = "error_label";
            this.error_label.Size = new System.Drawing.Size(447, 102);
            this.error_label.TabIndex = 67;
            this.error_label.Text = "Чтобы начать играть, пожалуйста, войдите в свою учетную запись";
            this.error_label.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // buy_panel
            // 
            this.buy_panel.Controls.Add(this.buy_btn_cp);
            this.buy_panel.Controls.Add(this.buy_label);
            this.buy_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buy_panel.Location = new System.Drawing.Point(0, 0);
            this.buy_panel.Name = "buy_panel";
            this.buy_panel.Size = new System.Drawing.Size(447, 234);
            this.buy_panel.TabIndex = 77;
            this.buy_panel.Visible = false;
            // 
            // buy_btn_cp
            // 
            this.buy_btn_cp.AutoSize = true;
            this.buy_btn_cp.BackColor = System.Drawing.Color.Black;
            this.buy_btn_cp.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buy_btn_cp.ForeColor = System.Drawing.Color.White;
            this.buy_btn_cp.Location = new System.Drawing.Point(146, 136);
            this.buy_btn_cp.Name = "buy_btn_cp";
            this.buy_btn_cp.Size = new System.Drawing.Size(162, 39);
            this.buy_btn_cp.TabIndex = 72;
            this.buy_btn_cp.TabStop = false;
            this.buy_btn_cp.Text = "Купить игру";
            this.buy_btn_cp.UseVisualStyleBackColor = false;
            // 
            // buy_label
            // 
            this.buy_label.AutoEllipsis = true;
            this.buy_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.buy_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buy_label.ForeColor = System.Drawing.Color.White;
            this.buy_label.Location = new System.Drawing.Point(0, 0);
            this.buy_label.Name = "buy_label";
            this.buy_label.Size = new System.Drawing.Size(447, 102);
            this.buy_label.TabIndex = 67;
            this.buy_label.Text = "Вы не приобрели игру.\r\nПожалуйста, приобретите лицензию на игру в боте и попробуй" +
    "те еще раз.";
            this.buy_label.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(447, 234);
            this.Controls.Add(this.login_panel);
            this.Controls.Add(this.buy_panel);
            this.Controls.Add(this.error_panel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login...";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.login_panel.ResumeLayout(false);
            this.login_panel.PerformLayout();
            this.error_panel.ResumeLayout(false);
            this.error_panel.PerformLayout();
            this.buy_panel.ResumeLayout(false);
            this.buy_panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label nickname_input_label;
        private System.Windows.Forms.Label title_label;
        public System.Windows.Forms.Button login_btn_r;
        public System.Windows.Forms.TextBox password_input;
        public System.Windows.Forms.TextBox nickname_input;
        public System.Windows.Forms.Button hide_show_pas_c;
        private System.Windows.Forms.Button create_account_l;
        public System.Windows.Forms.Label status_label;
        public System.Windows.Forms.Button exit_btn_cp;
        private System.Windows.Forms.Label error_label;
        public System.Windows.Forms.Panel login_panel;
        public System.Windows.Forms.Panel error_panel;
        public System.Windows.Forms.Panel buy_panel;
        public System.Windows.Forms.Button buy_btn_cp;
        private System.Windows.Forms.Label buy_label;
    }
}