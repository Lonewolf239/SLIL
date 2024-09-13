using System.Windows.Forms;

namespace SLIL
{
    partial class SLIL
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SLIL));
            this.shop_panel = new System.Windows.Forms.Panel();
            this.ShopInterface_panel = new System.Windows.Forms.Panel();
            this.shop_tab_control = new System.Windows.Forms.TabControl();
            this.weapon_shop_page = new System.Windows.Forms.TabPage();
            this.pet_shop_page = new System.Windows.Forms.TabPage();
            this.consumables_shop_page = new System.Windows.Forms.TabPage();
            this.shop_title_panel = new System.Windows.Forms.Panel();
            this.shop_title = new System.Windows.Forms.Label();
            this.shop_money = new System.Windows.Forms.Label();
            this.pause_panel = new System.Windows.Forms.Panel();
            this.pause_btn = new System.Windows.Forms.Button();
            this.exit_btn = new System.Windows.Forms.Button();
            this.pause_text = new System.Windows.Forms.Label();
            this.game_over_text = new System.Windows.Forms.Label();
            this.raycast = new System.Windows.Forms.Timer(this.components);
            this.step_sound_timer = new System.Windows.Forms.Timer(this.components);
            this.stamina_timer = new System.Windows.Forms.Timer(this.components);
            this.mouse_timer = new System.Windows.Forms.Timer(this.components);
            this.shot_timer = new System.Windows.Forms.Timer(this.components);
            this.reload_timer = new System.Windows.Forms.Timer(this.components);
            this.status_refresh = new System.Windows.Forms.Timer(this.components);
            this.chill_timer = new System.Windows.Forms.Timer(this.components);
            this.stage_timer = new System.Windows.Forms.Timer(this.components);
            this.game_over_panel = new System.Windows.Forms.Panel();
            this.exit_restart_btn = new System.Windows.Forms.Button();
            this.restart_btn = new System.Windows.Forms.Button();
            this.shotgun_pull_timer = new System.Windows.Forms.Timer(this.components);
            this.mouse_hold_timer = new System.Windows.Forms.Timer(this.components);
            this.parkour_timer = new System.Windows.Forms.Timer(this.components);
            this.shop_panel.SuspendLayout();
            this.ShopInterface_panel.SuspendLayout();
            this.shop_tab_control.SuspendLayout();
            this.shop_title_panel.SuspendLayout();
            this.pause_panel.SuspendLayout();
            this.game_over_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // shop_panel
            // 
            this.shop_panel.Controls.Add(this.ShopInterface_panel);
            this.shop_panel.Controls.Add(this.shop_title_panel);
            this.shop_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shop_panel.Location = new System.Drawing.Point(0, 0);
            this.shop_panel.Name = "shop_panel";
            this.shop_panel.Size = new System.Drawing.Size(454, 302);
            this.shop_panel.TabIndex = 4;
            this.shop_panel.Visible = false;
            this.shop_panel.VisibleChanged += new System.EventHandler(this.Shop_panel_VisibleChanged);
            // 
            // ShopInterface_panel
            // 
            this.ShopInterface_panel.Controls.Add(this.shop_tab_control);
            this.ShopInterface_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShopInterface_panel.Location = new System.Drawing.Point(0, 24);
            this.ShopInterface_panel.Name = "ShopInterface_panel";
            this.ShopInterface_panel.Size = new System.Drawing.Size(454, 278);
            this.ShopInterface_panel.TabIndex = 1;
            // 
            // shop_tab_control
            // 
            this.shop_tab_control.Controls.Add(this.weapon_shop_page);
            this.shop_tab_control.Controls.Add(this.pet_shop_page);
            this.shop_tab_control.Controls.Add(this.consumables_shop_page);
            this.shop_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shop_tab_control.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shop_tab_control.Location = new System.Drawing.Point(0, 0);
            this.shop_tab_control.Multiline = true;
            this.shop_tab_control.Name = "shop_tab_control";
            this.shop_tab_control.SelectedIndex = 0;
            this.shop_tab_control.Size = new System.Drawing.Size(454, 278);
            this.shop_tab_control.TabIndex = 0;
            this.shop_tab_control.TabStop = false;
            // 
            // weapon_shop_page
            // 
            this.weapon_shop_page.AutoScroll = true;
            this.weapon_shop_page.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.weapon_shop_page.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_shop_page.Location = new System.Drawing.Point(4, 25);
            this.weapon_shop_page.Name = "weapon_shop_page";
            this.weapon_shop_page.Padding = new System.Windows.Forms.Padding(3);
            this.weapon_shop_page.Size = new System.Drawing.Size(446, 249);
            this.weapon_shop_page.TabIndex = 0;
            this.weapon_shop_page.Text = "Оружие";
            // 
            // pet_shop_page
            // 
            this.pet_shop_page.AutoScroll = true;
            this.pet_shop_page.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.pet_shop_page.Location = new System.Drawing.Point(4, 25);
            this.pet_shop_page.Name = "pet_shop_page";
            this.pet_shop_page.Padding = new System.Windows.Forms.Padding(3);
            this.pet_shop_page.Size = new System.Drawing.Size(446, 249);
            this.pet_shop_page.TabIndex = 1;
            this.pet_shop_page.Text = "Питомцы";
            // 
            // consumables_shop_page
            // 
            this.consumables_shop_page.AutoScroll = true;
            this.consumables_shop_page.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.consumables_shop_page.Location = new System.Drawing.Point(4, 25);
            this.consumables_shop_page.Name = "consumables_shop_page";
            this.consumables_shop_page.Padding = new System.Windows.Forms.Padding(3);
            this.consumables_shop_page.Size = new System.Drawing.Size(446, 249);
            this.consumables_shop_page.TabIndex = 2;
            this.consumables_shop_page.Text = "Прочее";
            // 
            // shop_title_panel
            // 
            this.shop_title_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shop_title_panel.Controls.Add(this.shop_title);
            this.shop_title_panel.Controls.Add(this.shop_money);
            this.shop_title_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.shop_title_panel.Location = new System.Drawing.Point(0, 0);
            this.shop_title_panel.Name = "shop_title_panel";
            this.shop_title_panel.Size = new System.Drawing.Size(454, 24);
            this.shop_title_panel.TabIndex = 2;
            // 
            // shop_title
            // 
            this.shop_title.AutoSize = true;
            this.shop_title.Dock = System.Windows.Forms.DockStyle.Left;
            this.shop_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shop_title.ForeColor = System.Drawing.Color.White;
            this.shop_title.Location = new System.Drawing.Point(0, 0);
            this.shop_title.Name = "shop_title";
            this.shop_title.Size = new System.Drawing.Size(102, 24);
            this.shop_title.TabIndex = 1;
            this.shop_title.Text = "МАГАЗИН";
            this.shop_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // shop_money
            // 
            this.shop_money.AutoSize = true;
            this.shop_money.Dock = System.Windows.Forms.DockStyle.Right;
            this.shop_money.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shop_money.ForeColor = System.Drawing.Color.White;
            this.shop_money.Location = new System.Drawing.Point(412, 0);
            this.shop_money.Name = "shop_money";
            this.shop_money.Size = new System.Drawing.Size(40, 24);
            this.shop_money.TabIndex = 0;
            this.shop_money.Text = "$: 0";
            this.shop_money.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pause_panel
            // 
            this.pause_panel.Controls.Add(this.pause_btn);
            this.pause_panel.Controls.Add(this.exit_btn);
            this.pause_panel.Controls.Add(this.pause_text);
            this.pause_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pause_panel.Location = new System.Drawing.Point(0, 0);
            this.pause_panel.Name = "pause_panel";
            this.pause_panel.Size = new System.Drawing.Size(454, 302);
            this.pause_panel.TabIndex = 5;
            this.pause_panel.Visible = false;
            // 
            // pause_btn
            // 
            this.pause_btn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pause_btn.AutoSize = true;
            this.pause_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pause_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pause_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pause_btn.ForeColor = System.Drawing.Color.White;
            this.pause_btn.Location = new System.Drawing.Point(126, 140);
            this.pause_btn.Name = "pause_btn";
            this.pause_btn.Size = new System.Drawing.Size(198, 41);
            this.pause_btn.TabIndex = 38;
            this.pause_btn.TabStop = false;
            this.pause_btn.Text = "ПРОДОЛЖИТЬ";
            this.pause_btn.UseVisualStyleBackColor = true;
            this.pause_btn.Click += new System.EventHandler(this.Pause_btn_Click);
            // 
            // exit_btn
            // 
            this.exit_btn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exit_btn.AutoSize = true;
            this.exit_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.exit_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exit_btn.ForeColor = System.Drawing.Color.White;
            this.exit_btn.Location = new System.Drawing.Point(169, 187);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(112, 41);
            this.exit_btn.TabIndex = 37;
            this.exit_btn.TabStop = false;
            this.exit_btn.Text = "ВЫЙТИ";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.Exit_btn_Click);
            // 
            // pause_text
            // 
            this.pause_text.Dock = System.Windows.Forms.DockStyle.Top;
            this.pause_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pause_text.ForeColor = System.Drawing.Color.White;
            this.pause_text.Location = new System.Drawing.Point(0, 0);
            this.pause_text.Name = "pause_text";
            this.pause_text.Size = new System.Drawing.Size(454, 80);
            this.pause_text.TabIndex = 0;
            this.pause_text.Text = "ПАУЗА";
            this.pause_text.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // game_over_text
            // 
            this.game_over_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.game_over_text.Dock = System.Windows.Forms.DockStyle.Top;
            this.game_over_text.Font = new System.Drawing.Font("Consolas", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.game_over_text.ForeColor = System.Drawing.Color.White;
            this.game_over_text.Location = new System.Drawing.Point(0, 0);
            this.game_over_text.Name = "game_over_text";
            this.game_over_text.Size = new System.Drawing.Size(454, 137);
            this.game_over_text.TabIndex = 2;
            this.game_over_text.Text = "GAME OVER";
            this.game_over_text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // raycast
            // 
            this.raycast.Interval = 25;
            this.raycast.Tick += new System.EventHandler(this.Raycast_Tick);
            // 
            // step_sound_timer
            // 
            this.step_sound_timer.Interval = 1;
            this.step_sound_timer.Tick += new System.EventHandler(this.Step_sound_timer_Tick);
            // 
            // stamina_timer
            // 
            this.stamina_timer.Interval = 10;
            this.stamina_timer.Tick += new System.EventHandler(this.Stamina_timer_Tick);
            // 
            // mouse_timer
            // 
            this.mouse_timer.Interval = 10;
            this.mouse_timer.Tick += new System.EventHandler(this.Mouse_timer_Tick);
            // 
            // shot_timer
            // 
            this.shot_timer.Interval = 200;
            this.shot_timer.Tick += new System.EventHandler(this.Shot_timer_Tick);
            // 
            // reload_timer
            // 
            this.reload_timer.Interval = 750;
            this.reload_timer.Tick += new System.EventHandler(this.Reload_gun_Tick);
            // 
            // status_refresh
            // 
            this.status_refresh.Enabled = true;
            this.status_refresh.Interval = 5;
            this.status_refresh.Tick += new System.EventHandler(this.Status_refresh_Tick);
            // 
            // chill_timer
            // 
            this.chill_timer.Interval = 750;
            this.chill_timer.Tick += new System.EventHandler(this.Chill_timer_Tick);
            // 
            // stage_timer
            // 
            this.stage_timer.Interval = 200;
            this.stage_timer.Tick += new System.EventHandler(this.Stage_timer_Tick);
            // 
            // game_over_panel
            // 
            this.game_over_panel.Controls.Add(this.exit_restart_btn);
            this.game_over_panel.Controls.Add(this.restart_btn);
            this.game_over_panel.Controls.Add(this.game_over_text);
            this.game_over_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.game_over_panel.Location = new System.Drawing.Point(0, 0);
            this.game_over_panel.Name = "game_over_panel";
            this.game_over_panel.Size = new System.Drawing.Size(454, 302);
            this.game_over_panel.TabIndex = 6;
            this.game_over_panel.Visible = false;
            // 
            // exit_restart_btn
            // 
            this.exit_restart_btn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exit_restart_btn.AutoSize = true;
            this.exit_restart_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.exit_restart_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_restart_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exit_restart_btn.ForeColor = System.Drawing.Color.White;
            this.exit_restart_btn.Location = new System.Drawing.Point(169, 187);
            this.exit_restart_btn.Name = "exit_restart_btn";
            this.exit_restart_btn.Size = new System.Drawing.Size(112, 41);
            this.exit_restart_btn.TabIndex = 40;
            this.exit_restart_btn.TabStop = false;
            this.exit_restart_btn.Text = "ВЫЙТИ";
            this.exit_restart_btn.UseVisualStyleBackColor = true;
            this.exit_restart_btn.Click += new System.EventHandler(this.Exit_btn_Click);
            // 
            // restart_btn
            // 
            this.restart_btn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.restart_btn.AutoSize = true;
            this.restart_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.restart_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restart_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.restart_btn.ForeColor = System.Drawing.Color.White;
            this.restart_btn.Location = new System.Drawing.Point(126, 140);
            this.restart_btn.Name = "restart_btn";
            this.restart_btn.Size = new System.Drawing.Size(198, 41);
            this.restart_btn.TabIndex = 39;
            this.restart_btn.TabStop = false;
            this.restart_btn.Text = "ПОВТОРИТЬ";
            this.restart_btn.UseVisualStyleBackColor = true;
            this.restart_btn.Click += new System.EventHandler(this.Restart_btn_Click);
            // 
            // shotgun_pull_timer
            // 
            this.shotgun_pull_timer.Interval = 350;
            this.shotgun_pull_timer.Tick += new System.EventHandler(this.Shotgun_pull_timer_Tick);
            // 
            // mouse_hold_timer
            // 
            this.mouse_hold_timer.Interval = 500;
            this.mouse_hold_timer.Tick += new System.EventHandler(this.Mouse_hold_timer_Tick);
            // 
            // parkour_timer
            // 
            this.parkour_timer.Interval = 350;
            this.parkour_timer.Tick += new System.EventHandler(this.Parkour_timer_Tick);
            // 
            // SLIL
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(454, 302);
            this.Controls.Add(this.shop_panel);
            this.Controls.Add(this.game_over_panel);
            this.Controls.Add(this.pause_panel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "SLIL";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLIL";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.SLIL_Activated);
            this.Deactivate += new System.EventHandler(this.SLIL_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SLIL_FormClosing);
            this.LocationChanged += new System.EventHandler(this.SLIL_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SLIL_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SLIL_KeyUp);
            this.shop_panel.ResumeLayout(false);
            this.ShopInterface_panel.ResumeLayout(false);
            this.shop_tab_control.ResumeLayout(false);
            this.shop_title_panel.ResumeLayout(false);
            this.shop_title_panel.PerformLayout();
            this.pause_panel.ResumeLayout(false);
            this.pause_panel.PerformLayout();
            this.game_over_panel.ResumeLayout(false);
            this.game_over_panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer raycast;
        private System.Windows.Forms.Timer step_sound_timer;
        private System.Windows.Forms.Label game_over_text;
        private System.Windows.Forms.Timer stamina_timer;
        private System.Windows.Forms.Timer mouse_timer;
        private System.Windows.Forms.Timer shot_timer;
        private System.Windows.Forms.Timer reload_timer;
        private System.Windows.Forms.Timer status_refresh;
        private Panel shop_panel;
        private Label shop_money;
        private Panel ShopInterface_panel;
        private Panel shop_title_panel;
        private Label shop_title;
        private Timer chill_timer;
        private Timer stage_timer;
        private Panel pause_panel;
        private Label pause_text;
        private Button exit_btn;
        private Button pause_btn;
        private TabControl shop_tab_control;
        private TabPage weapon_shop_page;
        private TabPage pet_shop_page;
        private TabPage consumables_shop_page;
        private Panel game_over_panel;
        private Button restart_btn;
        private Button exit_restart_btn;
        private Timer shotgun_pull_timer;
        private Timer mouse_hold_timer;
        private Timer parkour_timer;
    }
}