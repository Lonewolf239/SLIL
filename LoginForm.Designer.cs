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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
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
            this.interface_panel = new System.Windows.Forms.Panel();
            this.hide_btn = new System.Windows.Forms.PictureBox();
            this.exit_btn = new System.Windows.Forms.PictureBox();
            this.title_form_label = new System.Windows.Forms.Label();
            this.login_panel.SuspendLayout();
            this.error_panel.SuspendLayout();
            this.buy_panel.SuspendLayout();
            this.interface_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hide_btn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exit_btn)).BeginInit();
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
            this.password_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.password_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.password_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.password_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Password_input_KeyDown);
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
            this.nickname_input_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.nickname_input_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.nickname_input_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.nickname_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Nickname_input_KeyDown);
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
            this.title_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.title_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.title_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.status_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.status_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.status_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.login_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.login_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.login_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.error_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.error_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.error_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.error_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.error_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.error_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.buy_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.buy_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.buy_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
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
            this.buy_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.buy_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.buy_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
            // 
            // interface_panel
            // 
            this.interface_panel.Controls.Add(this.login_panel);
            this.interface_panel.Controls.Add(this.error_panel);
            this.interface_panel.Controls.Add(this.buy_panel);
            this.interface_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.interface_panel.Location = new System.Drawing.Point(0, 30);
            this.interface_panel.Name = "interface_panel";
            this.interface_panel.Size = new System.Drawing.Size(447, 234);
            this.interface_panel.TabIndex = 75;
            // 
            // hide_btn
            // 
            this.hide_btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.hide_btn.Image = ((System.Drawing.Image)(resources.GetObject("hide_btn.Image")));
            this.hide_btn.Location = new System.Drawing.Point(387, 0);
            this.hide_btn.Name = "hide_btn";
            this.hide_btn.Size = new System.Drawing.Size(30, 30);
            this.hide_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.hide_btn.TabIndex = 76;
            this.hide_btn.TabStop = false;
            this.hide_btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Hide_btn_MouseClick);
            this.hide_btn.MouseEnter += new System.EventHandler(this.Hide_btn_MouseEnter);
            this.hide_btn.MouseLeave += new System.EventHandler(this.Hide_btn_MouseLeave);
            // 
            // exit_btn
            // 
            this.exit_btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.exit_btn.Image = global::SLIL.Properties.Resources.close;
            this.exit_btn.Location = new System.Drawing.Point(417, 0);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(30, 30);
            this.exit_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.exit_btn.TabIndex = 77;
            this.exit_btn.TabStop = false;
            this.exit_btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Exit_btn_MouseClick);
            this.exit_btn.MouseEnter += new System.EventHandler(this.Exit_btn_MouseEnter);
            this.exit_btn.MouseLeave += new System.EventHandler(this.Exit_btn_MouseLeave);
            // 
            // title_form_label
            // 
            this.title_form_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.title_form_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.title_form_label.ForeColor = System.Drawing.Color.White;
            this.title_form_label.Location = new System.Drawing.Point(0, 0);
            this.title_form_label.Name = "title_form_label";
            this.title_form_label.Size = new System.Drawing.Size(216, 30);
            this.title_form_label.TabIndex = 78;
            this.title_form_label.Text = "Login...";
            this.title_form_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.title_form_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.title_form_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.title_form_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(447, 264);
            this.Controls.Add(this.title_form_label);
            this.Controls.Add(this.hide_btn);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.interface_panel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login...";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
            this.login_panel.ResumeLayout(false);
            this.login_panel.PerformLayout();
            this.error_panel.ResumeLayout(false);
            this.error_panel.PerformLayout();
            this.buy_panel.ResumeLayout(false);
            this.buy_panel.PerformLayout();
            this.interface_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hide_btn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exit_btn)).EndInit();
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
        private System.Windows.Forms.Panel interface_panel;
        private System.Windows.Forms.PictureBox hide_btn;
        private System.Windows.Forms.PictureBox exit_btn;
        private System.Windows.Forms.Label title_form_label;
    }
}