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
            this.transport_shop_page = new System.Windows.Forms.TabPage();
            this.storage_shop_page = new System.Windows.Forms.TabPage();
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
            this.game_over_interface = new System.Windows.Forms.Panel();
            this.cause_of_death_icon = new System.Windows.Forms.PictureBox();
            this.cause_of_death_label = new System.Windows.Forms.Label();
            this.total_time_label = new System.Windows.Forms.Label();
            this.last_stage_label = new System.Windows.Forms.Label();
            this.total_killed_label = new System.Windows.Forms.Label();
            this.total_time_icon = new System.Windows.Forms.PictureBox();
            this.last_stage_icon = new System.Windows.Forms.PictureBox();
            this.total_killed_icon = new System.Windows.Forms.PictureBox();
            this.restart_btn = new System.Windows.Forms.Button();
            this.exit_restart_btn = new System.Windows.Forms.Button();
            this.shotgun_pull_timer = new System.Windows.Forms.Timer(this.components);
            this.mouse_hold_timer = new System.Windows.Forms.Timer(this.components);
            this.camera_shaking_timer = new System.Windows.Forms.Timer(this.components);
            this.inventory_panel = new System.Windows.Forms.Panel();
            this.inventory_content_panel = new System.Windows.Forms.Panel();
            this.hide_weapon_picture = new System.Windows.Forms.PictureBox();
            this.pet_panel = new System.Windows.Forms.Panel();
            this.pet_icon = new System.Windows.Forms.PictureBox();
            this.pet_label = new System.Windows.Forms.Label();
            this.pet_title = new System.Windows.Forms.Label();
            this.helmet_count = new System.Windows.Forms.Label();
            this.helmet_icon = new System.Windows.Forms.PictureBox();
            this.adrenalin_count = new System.Windows.Forms.Label();
            this.adrenalin_icon = new System.Windows.Forms.PictureBox();
            this.medkit_count = new System.Windows.Forms.Label();
            this.medkit_icon = new System.Windows.Forms.PictureBox();
            this.items_title = new System.Windows.Forms.Label();
            this.weapon_title = new System.Windows.Forms.Label();
            this.weapon_1_panel = new System.Windows.Forms.Panel();
            this.weapon_1_icon = new System.Windows.Forms.PictureBox();
            this.weapon_1_ammo_icon = new System.Windows.Forms.PictureBox();
            this.weapon_1_label = new System.Windows.Forms.Label();
            this.weapon_1_ammo_count = new System.Windows.Forms.Label();
            this.weapon_0_panel = new System.Windows.Forms.Panel();
            this.weapon_0_icon = new System.Windows.Forms.PictureBox();
            this.weapon_0_ammo_icon = new System.Windows.Forms.PictureBox();
            this.weapon_0_label = new System.Windows.Forms.Label();
            this.weapon_0_ammo_count = new System.Windows.Forms.Label();
            this.pistol_panel = new System.Windows.Forms.Panel();
            this.pistol_icon = new System.Windows.Forms.PictureBox();
            this.pistol_ammo_icon = new System.Windows.Forms.PictureBox();
            this.pistol_label = new System.Windows.Forms.Label();
            this.pistol_ammo_count = new System.Windows.Forms.Label();
            this.inventory_label = new System.Windows.Forms.Label();
            this.shop_panel.SuspendLayout();
            this.ShopInterface_panel.SuspendLayout();
            this.shop_tab_control.SuspendLayout();
            this.shop_title_panel.SuspendLayout();
            this.pause_panel.SuspendLayout();
            this.game_over_panel.SuspendLayout();
            this.game_over_interface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cause_of_death_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.total_time_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.last_stage_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.total_killed_icon)).BeginInit();
            this.inventory_panel.SuspendLayout();
            this.inventory_content_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hide_weapon_picture)).BeginInit();
            this.pet_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pet_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helmet_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.adrenalin_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.medkit_icon)).BeginInit();
            this.weapon_1_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_1_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_1_ammo_icon)).BeginInit();
            this.weapon_0_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_0_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_0_ammo_icon)).BeginInit();
            this.pistol_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_ammo_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // shop_panel
            // 
            this.shop_panel.Controls.Add(this.ShopInterface_panel);
            this.shop_panel.Controls.Add(this.shop_title_panel);
            this.shop_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shop_panel.Location = new System.Drawing.Point(0, 0);
            this.shop_panel.Name = "shop_panel";
            this.shop_panel.Size = new System.Drawing.Size(1105, 633);
            this.shop_panel.TabIndex = 4;
            this.shop_panel.Visible = false;
            this.shop_panel.VisibleChanged += new System.EventHandler(this.Shop_panel_VisibleChanged);
            // 
            // ShopInterface_panel
            // 
            this.ShopInterface_panel.Controls.Add(this.shop_tab_control);
            this.ShopInterface_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShopInterface_panel.Location = new System.Drawing.Point(0, 35);
            this.ShopInterface_panel.Name = "ShopInterface_panel";
            this.ShopInterface_panel.Size = new System.Drawing.Size(1105, 598);
            this.ShopInterface_panel.TabIndex = 1;
            // 
            // shop_tab_control
            // 
            this.shop_tab_control.Controls.Add(this.weapon_shop_page);
            this.shop_tab_control.Controls.Add(this.pet_shop_page);
            this.shop_tab_control.Controls.Add(this.consumables_shop_page);
            this.shop_tab_control.Controls.Add(this.transport_shop_page);
            this.shop_tab_control.Controls.Add(this.storage_shop_page);
            this.shop_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shop_tab_control.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shop_tab_control.Location = new System.Drawing.Point(0, 0);
            this.shop_tab_control.Multiline = true;
            this.shop_tab_control.Name = "shop_tab_control";
            this.shop_tab_control.SelectedIndex = 0;
            this.shop_tab_control.Size = new System.Drawing.Size(1105, 598);
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
            this.weapon_shop_page.Size = new System.Drawing.Size(1097, 569);
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
            this.pet_shop_page.Size = new System.Drawing.Size(1097, 569);
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
            this.consumables_shop_page.Size = new System.Drawing.Size(1097, 569);
            this.consumables_shop_page.TabIndex = 2;
            this.consumables_shop_page.Text = "Прочее";
            // 
            // transport_shop_page
            // 
            this.transport_shop_page.AutoScroll = true;
            this.transport_shop_page.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.transport_shop_page.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.transport_shop_page.Location = new System.Drawing.Point(4, 25);
            this.transport_shop_page.Name = "transport_shop_page";
            this.transport_shop_page.Padding = new System.Windows.Forms.Padding(3);
            this.transport_shop_page.Size = new System.Drawing.Size(1097, 569);
            this.transport_shop_page.TabIndex = 3;
            this.transport_shop_page.Text = "Транспорт";
            // 
            // storage_shop_page
            // 
            this.storage_shop_page.AutoScroll = true;
            this.storage_shop_page.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.storage_shop_page.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.storage_shop_page.Location = new System.Drawing.Point(4, 25);
            this.storage_shop_page.Name = "storage_shop_page";
            this.storage_shop_page.Padding = new System.Windows.Forms.Padding(3);
            this.storage_shop_page.Size = new System.Drawing.Size(1097, 569);
            this.storage_shop_page.TabIndex = 4;
            this.storage_shop_page.Text = "Хранилище";
            // 
            // shop_title_panel
            // 
            this.shop_title_panel.Controls.Add(this.shop_title);
            this.shop_title_panel.Controls.Add(this.shop_money);
            this.shop_title_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.shop_title_panel.Location = new System.Drawing.Point(0, 0);
            this.shop_title_panel.Name = "shop_title_panel";
            this.shop_title_panel.Size = new System.Drawing.Size(1105, 35);
            this.shop_title_panel.TabIndex = 2;
            // 
            // shop_title
            // 
            this.shop_title.AutoSize = true;
            this.shop_title.Dock = System.Windows.Forms.DockStyle.Left;
            this.shop_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shop_title.ForeColor = System.Drawing.Color.White;
            this.shop_title.Location = new System.Drawing.Point(0, 0);
            this.shop_title.Name = "shop_title";
            this.shop_title.Size = new System.Drawing.Size(153, 33);
            this.shop_title.TabIndex = 1;
            this.shop_title.Text = "МАГАЗИН";
            this.shop_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // shop_money
            // 
            this.shop_money.AutoSize = true;
            this.shop_money.Dock = System.Windows.Forms.DockStyle.Right;
            this.shop_money.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.shop_money.ForeColor = System.Drawing.Color.White;
            this.shop_money.Location = new System.Drawing.Point(1042, 0);
            this.shop_money.Name = "shop_money";
            this.shop_money.Size = new System.Drawing.Size(63, 33);
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
            this.pause_panel.Size = new System.Drawing.Size(1105, 633);
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
            this.pause_btn.Location = new System.Drawing.Point(126, 305);
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
            this.exit_btn.Location = new System.Drawing.Point(169, 352);
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
            this.pause_text.Size = new System.Drawing.Size(1105, 80);
            this.pause_text.TabIndex = 0;
            this.pause_text.Text = "ПАУЗА";
            this.pause_text.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // game_over_text
            // 
            this.game_over_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.game_over_text.Dock = System.Windows.Forms.DockStyle.Top;
            this.game_over_text.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.game_over_text.ForeColor = System.Drawing.Color.White;
            this.game_over_text.Location = new System.Drawing.Point(0, 0);
            this.game_over_text.Name = "game_over_text";
            this.game_over_text.Size = new System.Drawing.Size(630, 61);
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
            this.game_over_panel.Controls.Add(this.game_over_interface);
            this.game_over_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.game_over_panel.Location = new System.Drawing.Point(0, 0);
            this.game_over_panel.Name = "game_over_panel";
            this.game_over_panel.Size = new System.Drawing.Size(1105, 633);
            this.game_over_panel.TabIndex = 6;
            this.game_over_panel.Visible = false;
            // 
            // game_over_interface
            // 
            this.game_over_interface.Controls.Add(this.cause_of_death_icon);
            this.game_over_interface.Controls.Add(this.cause_of_death_label);
            this.game_over_interface.Controls.Add(this.total_time_label);
            this.game_over_interface.Controls.Add(this.last_stage_label);
            this.game_over_interface.Controls.Add(this.total_killed_label);
            this.game_over_interface.Controls.Add(this.total_time_icon);
            this.game_over_interface.Controls.Add(this.last_stage_icon);
            this.game_over_interface.Controls.Add(this.total_killed_icon);
            this.game_over_interface.Controls.Add(this.game_over_text);
            this.game_over_interface.Controls.Add(this.restart_btn);
            this.game_over_interface.Controls.Add(this.exit_restart_btn);
            this.game_over_interface.Location = new System.Drawing.Point(226, 175);
            this.game_over_interface.Name = "game_over_interface";
            this.game_over_interface.Size = new System.Drawing.Size(630, 363);
            this.game_over_interface.TabIndex = 41;
            // 
            // cause_of_death_icon
            // 
            this.cause_of_death_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cause_of_death_icon.Location = new System.Drawing.Point(443, 114);
            this.cause_of_death_icon.Name = "cause_of_death_icon";
            this.cause_of_death_icon.Size = new System.Drawing.Size(128, 128);
            this.cause_of_death_icon.TabIndex = 48;
            this.cause_of_death_icon.TabStop = false;
            // 
            // cause_of_death_label
            // 
            this.cause_of_death_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.cause_of_death_label.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cause_of_death_label.ForeColor = System.Drawing.Color.White;
            this.cause_of_death_label.Location = new System.Drawing.Point(374, 61);
            this.cause_of_death_label.Name = "cause_of_death_label";
            this.cause_of_death_label.Size = new System.Drawing.Size(256, 50);
            this.cause_of_death_label.TabIndex = 47;
            this.cause_of_death_label.Text = "Причина смерти:";
            this.cause_of_death_label.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // total_time_label
            // 
            this.total_time_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.total_time_label.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.total_time_label.ForeColor = System.Drawing.Color.White;
            this.total_time_label.Location = new System.Drawing.Point(78, 192);
            this.total_time_label.Name = "total_time_label";
            this.total_time_label.Size = new System.Drawing.Size(195, 50);
            this.total_time_label.TabIndex = 46;
            this.total_time_label.Text = "00:00:00";
            this.total_time_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // last_stage_label
            // 
            this.last_stage_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.last_stage_label.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.last_stage_label.ForeColor = System.Drawing.Color.White;
            this.last_stage_label.Location = new System.Drawing.Point(78, 136);
            this.last_stage_label.Name = "last_stage_label";
            this.last_stage_label.Size = new System.Drawing.Size(195, 50);
            this.last_stage_label.TabIndex = 45;
            this.last_stage_label.Text = "999";
            this.last_stage_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // total_killed_label
            // 
            this.total_killed_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.total_killed_label.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.total_killed_label.ForeColor = System.Drawing.Color.White;
            this.total_killed_label.Location = new System.Drawing.Point(78, 80);
            this.total_killed_label.Name = "total_killed_label";
            this.total_killed_label.Size = new System.Drawing.Size(195, 50);
            this.total_killed_label.TabIndex = 44;
            this.total_killed_label.Text = "999";
            this.total_killed_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // total_time_icon
            // 
            this.total_time_icon.Image = global::SLIL.Properties.Resources.time;
            this.total_time_icon.Location = new System.Drawing.Point(22, 192);
            this.total_time_icon.Name = "total_time_icon";
            this.total_time_icon.Size = new System.Drawing.Size(50, 50);
            this.total_time_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.total_time_icon.TabIndex = 43;
            this.total_time_icon.TabStop = false;
            // 
            // last_stage_icon
            // 
            this.last_stage_icon.Image = global::SLIL.Properties.Resources.stage;
            this.last_stage_icon.Location = new System.Drawing.Point(22, 136);
            this.last_stage_icon.Name = "last_stage_icon";
            this.last_stage_icon.Size = new System.Drawing.Size(50, 50);
            this.last_stage_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.last_stage_icon.TabIndex = 42;
            this.last_stage_icon.TabStop = false;
            // 
            // total_killed_icon
            // 
            this.total_killed_icon.Image = global::SLIL.Properties.Resources.kills;
            this.total_killed_icon.Location = new System.Drawing.Point(22, 80);
            this.total_killed_icon.Name = "total_killed_icon";
            this.total_killed_icon.Size = new System.Drawing.Size(50, 50);
            this.total_killed_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.total_killed_icon.TabIndex = 41;
            this.total_killed_icon.TabStop = false;
            // 
            // restart_btn
            // 
            this.restart_btn.AutoSize = true;
            this.restart_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.restart_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restart_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.restart_btn.ForeColor = System.Drawing.Color.White;
            this.restart_btn.Location = new System.Drawing.Point(228, 272);
            this.restart_btn.Name = "restart_btn";
            this.restart_btn.Size = new System.Drawing.Size(198, 41);
            this.restart_btn.TabIndex = 39;
            this.restart_btn.TabStop = false;
            this.restart_btn.Text = "ПОВТОРИТЬ";
            this.restart_btn.UseVisualStyleBackColor = true;
            this.restart_btn.Click += new System.EventHandler(this.Restart_btn_Click);
            // 
            // exit_restart_btn
            // 
            this.exit_restart_btn.AutoSize = true;
            this.exit_restart_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.exit_restart_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_restart_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exit_restart_btn.ForeColor = System.Drawing.Color.White;
            this.exit_restart_btn.Location = new System.Drawing.Point(264, 319);
            this.exit_restart_btn.Name = "exit_restart_btn";
            this.exit_restart_btn.Size = new System.Drawing.Size(112, 41);
            this.exit_restart_btn.TabIndex = 40;
            this.exit_restart_btn.TabStop = false;
            this.exit_restart_btn.Text = "ВЫЙТИ";
            this.exit_restart_btn.UseVisualStyleBackColor = true;
            this.exit_restart_btn.Click += new System.EventHandler(this.Exit_btn_Click);
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
            // camera_shaking_timer
            // 
            this.camera_shaking_timer.Interval = 75;
            this.camera_shaking_timer.Tick += new System.EventHandler(this.Camera_shaking_timer_Tick);
            // 
            // inventory_panel
            // 
            this.inventory_panel.Controls.Add(this.inventory_content_panel);
            this.inventory_panel.Controls.Add(this.inventory_label);
            this.inventory_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inventory_panel.Location = new System.Drawing.Point(0, 0);
            this.inventory_panel.Name = "inventory_panel";
            this.inventory_panel.Size = new System.Drawing.Size(1105, 633);
            this.inventory_panel.TabIndex = 0;
            this.inventory_panel.Visible = false;
            // 
            // inventory_content_panel
            // 
            this.inventory_content_panel.Controls.Add(this.hide_weapon_picture);
            this.inventory_content_panel.Controls.Add(this.pet_panel);
            this.inventory_content_panel.Controls.Add(this.pet_title);
            this.inventory_content_panel.Controls.Add(this.helmet_count);
            this.inventory_content_panel.Controls.Add(this.helmet_icon);
            this.inventory_content_panel.Controls.Add(this.adrenalin_count);
            this.inventory_content_panel.Controls.Add(this.adrenalin_icon);
            this.inventory_content_panel.Controls.Add(this.medkit_count);
            this.inventory_content_panel.Controls.Add(this.medkit_icon);
            this.inventory_content_panel.Controls.Add(this.items_title);
            this.inventory_content_panel.Controls.Add(this.weapon_title);
            this.inventory_content_panel.Controls.Add(this.weapon_1_panel);
            this.inventory_content_panel.Controls.Add(this.weapon_0_panel);
            this.inventory_content_panel.Controls.Add(this.pistol_panel);
            this.inventory_content_panel.Location = new System.Drawing.Point(12, 63);
            this.inventory_content_panel.Name = "inventory_content_panel";
            this.inventory_content_panel.Size = new System.Drawing.Size(987, 513);
            this.inventory_content_panel.TabIndex = 27;
            // 
            // hide_weapon_picture
            // 
            this.hide_weapon_picture.Location = new System.Drawing.Point(0, 0);
            this.hide_weapon_picture.Name = "hide_weapon_picture";
            this.hide_weapon_picture.Size = new System.Drawing.Size(987, 256);
            this.hide_weapon_picture.TabIndex = 14;
            this.hide_weapon_picture.TabStop = false;
            this.hide_weapon_picture.Visible = false;
            // 
            // pet_panel
            // 
            this.pet_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pet_panel.Controls.Add(this.pet_icon);
            this.pet_panel.Controls.Add(this.pet_label);
            this.pet_panel.Location = new System.Drawing.Point(662, 292);
            this.pet_panel.Name = "pet_panel";
            this.pet_panel.Size = new System.Drawing.Size(325, 216);
            this.pet_panel.TabIndex = 13;
            // 
            // pet_icon
            // 
            this.pet_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pet_icon.Location = new System.Drawing.Point(3, 3);
            this.pet_icon.Name = "pet_icon";
            this.pet_icon.Size = new System.Drawing.Size(317, 178);
            this.pet_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pet_icon.TabIndex = 4;
            this.pet_icon.TabStop = false;
            this.pet_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.pet_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // pet_label
            // 
            this.pet_label.AutoEllipsis = true;
            this.pet_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pet_label.ForeColor = System.Drawing.Color.White;
            this.pet_label.Location = new System.Drawing.Point(3, 188);
            this.pet_label.Name = "pet_label";
            this.pet_label.Size = new System.Drawing.Size(315, 24);
            this.pet_label.TabIndex = 1;
            this.pet_label.Text = "PET";
            this.pet_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pet_title
            // 
            this.pet_title.AutoEllipsis = true;
            this.pet_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pet_title.ForeColor = System.Drawing.Color.White;
            this.pet_title.Location = new System.Drawing.Point(660, 259);
            this.pet_title.Name = "pet_title";
            this.pet_title.Size = new System.Drawing.Size(327, 30);
            this.pet_title.TabIndex = 12;
            this.pet_title.Text = "Питомец";
            this.pet_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // helmet_count
            // 
            this.helmet_count.AutoEllipsis = true;
            this.helmet_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helmet_count.ForeColor = System.Drawing.Color.White;
            this.helmet_count.Location = new System.Drawing.Point(200, 301);
            this.helmet_count.Name = "helmet_count";
            this.helmet_count.Size = new System.Drawing.Size(82, 32);
            this.helmet_count.TabIndex = 11;
            this.helmet_count.Text = "0/0";
            this.helmet_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // helmet_icon
            // 
            this.helmet_icon.Image = global::SLIL.Properties.Resources.helmet_count_icon;
            this.helmet_icon.Location = new System.Drawing.Point(144, 292);
            this.helmet_icon.Name = "helmet_icon";
            this.helmet_icon.Size = new System.Drawing.Size(50, 50);
            this.helmet_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.helmet_icon.TabIndex = 10;
            this.helmet_icon.TabStop = false;
            this.helmet_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.helmet_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // adrenalin_count
            // 
            this.adrenalin_count.AutoEllipsis = true;
            this.adrenalin_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.adrenalin_count.ForeColor = System.Drawing.Color.White;
            this.adrenalin_count.Location = new System.Drawing.Point(56, 357);
            this.adrenalin_count.Name = "adrenalin_count";
            this.adrenalin_count.Size = new System.Drawing.Size(82, 32);
            this.adrenalin_count.TabIndex = 9;
            this.adrenalin_count.Text = "0/0";
            this.adrenalin_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // adrenalin_icon
            // 
            this.adrenalin_icon.Image = global::SLIL.Properties.Resources.adrenalin_count_icon;
            this.adrenalin_icon.Location = new System.Drawing.Point(0, 348);
            this.adrenalin_icon.Name = "adrenalin_icon";
            this.adrenalin_icon.Size = new System.Drawing.Size(50, 50);
            this.adrenalin_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.adrenalin_icon.TabIndex = 8;
            this.adrenalin_icon.TabStop = false;
            this.adrenalin_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.adrenalin_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // medkit_count
            // 
            this.medkit_count.AutoEllipsis = true;
            this.medkit_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medkit_count.ForeColor = System.Drawing.Color.White;
            this.medkit_count.Location = new System.Drawing.Point(56, 301);
            this.medkit_count.Name = "medkit_count";
            this.medkit_count.Size = new System.Drawing.Size(82, 32);
            this.medkit_count.TabIndex = 7;
            this.medkit_count.Text = "0/0";
            this.medkit_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // medkit_icon
            // 
            this.medkit_icon.Image = global::SLIL.Properties.Resources.first_aid;
            this.medkit_icon.Location = new System.Drawing.Point(0, 292);
            this.medkit_icon.Name = "medkit_icon";
            this.medkit_icon.Size = new System.Drawing.Size(50, 50);
            this.medkit_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.medkit_icon.TabIndex = 6;
            this.medkit_icon.TabStop = false;
            this.medkit_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.medkit_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // items_title
            // 
            this.items_title.AutoEllipsis = true;
            this.items_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.items_title.ForeColor = System.Drawing.Color.White;
            this.items_title.Location = new System.Drawing.Point(0, 259);
            this.items_title.Name = "items_title";
            this.items_title.Size = new System.Drawing.Size(325, 30);
            this.items_title.TabIndex = 5;
            this.items_title.Text = "Предметы";
            this.items_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // weapon_title
            // 
            this.weapon_title.AutoEllipsis = true;
            this.weapon_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_title.ForeColor = System.Drawing.Color.White;
            this.weapon_title.Location = new System.Drawing.Point(0, 0);
            this.weapon_title.Name = "weapon_title";
            this.weapon_title.Size = new System.Drawing.Size(342, 30);
            this.weapon_title.TabIndex = 4;
            this.weapon_title.Text = "Оружие";
            this.weapon_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // weapon_1_panel
            // 
            this.weapon_1_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.weapon_1_panel.Controls.Add(this.weapon_1_icon);
            this.weapon_1_panel.Controls.Add(this.weapon_1_ammo_icon);
            this.weapon_1_panel.Controls.Add(this.weapon_1_label);
            this.weapon_1_panel.Controls.Add(this.weapon_1_ammo_count);
            this.weapon_1_panel.Location = new System.Drawing.Point(662, 33);
            this.weapon_1_panel.Name = "weapon_1_panel";
            this.weapon_1_panel.Size = new System.Drawing.Size(325, 216);
            this.weapon_1_panel.TabIndex = 3;
            // 
            // weapon_1_icon
            // 
            this.weapon_1_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.weapon_1_icon.Location = new System.Drawing.Point(3, 3);
            this.weapon_1_icon.Name = "weapon_1_icon";
            this.weapon_1_icon.Size = new System.Drawing.Size(317, 178);
            this.weapon_1_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.weapon_1_icon.TabIndex = 4;
            this.weapon_1_icon.TabStop = false;
            this.weapon_1_icon.MouseEnter += new System.EventHandler(this.Weapon_icon_MouseEnter);
            this.weapon_1_icon.MouseLeave += new System.EventHandler(this.Weapon_icon_MouseLeave);
            // 
            // weapon_1_ammo_icon
            // 
            this.weapon_1_ammo_icon.Location = new System.Drawing.Point(294, 183);
            this.weapon_1_ammo_icon.Name = "weapon_1_ammo_icon";
            this.weapon_1_ammo_icon.Size = new System.Drawing.Size(24, 24);
            this.weapon_1_ammo_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.weapon_1_ammo_icon.TabIndex = 3;
            this.weapon_1_ammo_icon.TabStop = false;
            // 
            // weapon_1_label
            // 
            this.weapon_1_label.AutoEllipsis = true;
            this.weapon_1_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_1_label.ForeColor = System.Drawing.Color.White;
            this.weapon_1_label.Location = new System.Drawing.Point(3, 183);
            this.weapon_1_label.Name = "weapon_1_label";
            this.weapon_1_label.Size = new System.Drawing.Size(199, 24);
            this.weapon_1_label.TabIndex = 1;
            this.weapon_1_label.Text = "WEAPON_1";
            this.weapon_1_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // weapon_1_ammo_count
            // 
            this.weapon_1_ammo_count.AutoEllipsis = true;
            this.weapon_1_ammo_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_1_ammo_count.ForeColor = System.Drawing.Color.White;
            this.weapon_1_ammo_count.Location = new System.Drawing.Point(208, 183);
            this.weapon_1_ammo_count.Name = "weapon_1_ammo_count";
            this.weapon_1_ammo_count.Size = new System.Drawing.Size(82, 24);
            this.weapon_1_ammo_count.TabIndex = 2;
            this.weapon_1_ammo_count.Text = "0/0";
            this.weapon_1_ammo_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // weapon_0_panel
            // 
            this.weapon_0_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.weapon_0_panel.Controls.Add(this.weapon_0_icon);
            this.weapon_0_panel.Controls.Add(this.weapon_0_ammo_icon);
            this.weapon_0_panel.Controls.Add(this.weapon_0_label);
            this.weapon_0_panel.Controls.Add(this.weapon_0_ammo_count);
            this.weapon_0_panel.Location = new System.Drawing.Point(331, 33);
            this.weapon_0_panel.Name = "weapon_0_panel";
            this.weapon_0_panel.Size = new System.Drawing.Size(325, 216);
            this.weapon_0_panel.TabIndex = 2;
            // 
            // weapon_0_icon
            // 
            this.weapon_0_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.weapon_0_icon.Location = new System.Drawing.Point(3, 3);
            this.weapon_0_icon.Name = "weapon_0_icon";
            this.weapon_0_icon.Size = new System.Drawing.Size(317, 178);
            this.weapon_0_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.weapon_0_icon.TabIndex = 4;
            this.weapon_0_icon.TabStop = false;
            this.weapon_0_icon.MouseEnter += new System.EventHandler(this.Weapon_icon_MouseEnter);
            this.weapon_0_icon.MouseLeave += new System.EventHandler(this.Weapon_icon_MouseLeave);
            // 
            // weapon_0_ammo_icon
            // 
            this.weapon_0_ammo_icon.Location = new System.Drawing.Point(294, 183);
            this.weapon_0_ammo_icon.Name = "weapon_0_ammo_icon";
            this.weapon_0_ammo_icon.Size = new System.Drawing.Size(24, 24);
            this.weapon_0_ammo_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.weapon_0_ammo_icon.TabIndex = 3;
            this.weapon_0_ammo_icon.TabStop = false;
            // 
            // weapon_0_label
            // 
            this.weapon_0_label.AutoEllipsis = true;
            this.weapon_0_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_0_label.ForeColor = System.Drawing.Color.White;
            this.weapon_0_label.Location = new System.Drawing.Point(3, 183);
            this.weapon_0_label.Name = "weapon_0_label";
            this.weapon_0_label.Size = new System.Drawing.Size(199, 24);
            this.weapon_0_label.TabIndex = 1;
            this.weapon_0_label.Text = "WEAPON_0";
            this.weapon_0_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // weapon_0_ammo_count
            // 
            this.weapon_0_ammo_count.AutoEllipsis = true;
            this.weapon_0_ammo_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_0_ammo_count.ForeColor = System.Drawing.Color.White;
            this.weapon_0_ammo_count.Location = new System.Drawing.Point(208, 183);
            this.weapon_0_ammo_count.Name = "weapon_0_ammo_count";
            this.weapon_0_ammo_count.Size = new System.Drawing.Size(82, 24);
            this.weapon_0_ammo_count.TabIndex = 2;
            this.weapon_0_ammo_count.Text = "0/0";
            this.weapon_0_ammo_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pistol_panel
            // 
            this.pistol_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pistol_panel.Controls.Add(this.pistol_icon);
            this.pistol_panel.Controls.Add(this.pistol_ammo_icon);
            this.pistol_panel.Controls.Add(this.pistol_label);
            this.pistol_panel.Controls.Add(this.pistol_ammo_count);
            this.pistol_panel.Location = new System.Drawing.Point(0, 33);
            this.pistol_panel.Name = "pistol_panel";
            this.pistol_panel.Size = new System.Drawing.Size(325, 216);
            this.pistol_panel.TabIndex = 1;
            // 
            // pistol_icon
            // 
            this.pistol_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pistol_icon.Location = new System.Drawing.Point(3, 3);
            this.pistol_icon.Name = "pistol_icon";
            this.pistol_icon.Size = new System.Drawing.Size(317, 178);
            this.pistol_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pistol_icon.TabIndex = 4;
            this.pistol_icon.TabStop = false;
            this.pistol_icon.MouseEnter += new System.EventHandler(this.Weapon_icon_MouseEnter);
            this.pistol_icon.MouseLeave += new System.EventHandler(this.Weapon_icon_MouseLeave);
            // 
            // pistol_ammo_icon
            // 
            this.pistol_ammo_icon.Image = global::SLIL.Properties.Resources.bullet;
            this.pistol_ammo_icon.Location = new System.Drawing.Point(294, 183);
            this.pistol_ammo_icon.Name = "pistol_ammo_icon";
            this.pistol_ammo_icon.Size = new System.Drawing.Size(24, 24);
            this.pistol_ammo_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pistol_ammo_icon.TabIndex = 3;
            this.pistol_ammo_icon.TabStop = false;
            // 
            // pistol_label
            // 
            this.pistol_label.AutoEllipsis = true;
            this.pistol_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pistol_label.ForeColor = System.Drawing.Color.White;
            this.pistol_label.Location = new System.Drawing.Point(3, 183);
            this.pistol_label.Name = "pistol_label";
            this.pistol_label.Size = new System.Drawing.Size(199, 24);
            this.pistol_label.TabIndex = 1;
            this.pistol_label.Text = "Пистолет";
            this.pistol_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pistol_ammo_count
            // 
            this.pistol_ammo_count.AutoEllipsis = true;
            this.pistol_ammo_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pistol_ammo_count.ForeColor = System.Drawing.Color.White;
            this.pistol_ammo_count.Location = new System.Drawing.Point(208, 183);
            this.pistol_ammo_count.Name = "pistol_ammo_count";
            this.pistol_ammo_count.Size = new System.Drawing.Size(82, 24);
            this.pistol_ammo_count.TabIndex = 2;
            this.pistol_ammo_count.Text = "0/0";
            this.pistol_ammo_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // inventory_label
            // 
            this.inventory_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.inventory_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.inventory_label.ForeColor = System.Drawing.Color.White;
            this.inventory_label.Location = new System.Drawing.Point(0, 0);
            this.inventory_label.Name = "inventory_label";
            this.inventory_label.Size = new System.Drawing.Size(1105, 33);
            this.inventory_label.TabIndex = 0;
            this.inventory_label.Text = "Инвентарь";
            this.inventory_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SLIL
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1105, 633);
            this.Controls.Add(this.game_over_panel);
            this.Controls.Add(this.inventory_panel);
            this.Controls.Add(this.shop_panel);
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
            this.game_over_interface.ResumeLayout(false);
            this.game_over_interface.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cause_of_death_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.total_time_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.last_stage_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.total_killed_icon)).EndInit();
            this.inventory_panel.ResumeLayout(false);
            this.inventory_content_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hide_weapon_picture)).EndInit();
            this.pet_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pet_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helmet_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.adrenalin_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.medkit_icon)).EndInit();
            this.weapon_1_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.weapon_1_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_1_ammo_icon)).EndInit();
            this.weapon_0_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.weapon_0_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_0_ammo_icon)).EndInit();
            this.pistol_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pistol_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_ammo_icon)).EndInit();
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
        private Panel game_over_panel;
        private Button restart_btn;
        private Button exit_restart_btn;
        private Timer shotgun_pull_timer;
        private Timer mouse_hold_timer;
        private Timer camera_shaking_timer;
        private Panel inventory_panel;
        private Label inventory_label;
        private Panel pistol_panel;
        private Label pistol_label;
        private Label pistol_ammo_count;
        private PictureBox pistol_icon;
        private PictureBox pistol_ammo_icon;
        private Label weapon_title;
        private Panel weapon_1_panel;
        private PictureBox weapon_1_icon;
        private PictureBox weapon_1_ammo_icon;
        private Label weapon_1_label;
        private Label weapon_1_ammo_count;
        private Panel weapon_0_panel;
        private PictureBox weapon_0_icon;
        private PictureBox weapon_0_ammo_icon;
        private Label weapon_0_label;
        private Label weapon_0_ammo_count;
        private PictureBox medkit_icon;
        private Label items_title;
        private Label medkit_count;
        private Label adrenalin_count;
        private PictureBox adrenalin_icon;
        private Label helmet_count;
        private PictureBox helmet_icon;
        private Label pet_title;
        private Panel pet_panel;
        private PictureBox pet_icon;
        private Label pet_label;
        private Panel inventory_content_panel;
        private TabControl shop_tab_control;
        private TabPage weapon_shop_page;
        private TabPage pet_shop_page;
        private TabPage consumables_shop_page;
        private TabPage transport_shop_page;
        private TabPage storage_shop_page;
        private PictureBox hide_weapon_picture;
        private Panel game_over_interface;
        private Label total_killed_label;
        private PictureBox total_time_icon;
        private PictureBox last_stage_icon;
        private PictureBox total_killed_icon;
        private PictureBox cause_of_death_icon;
        private Label cause_of_death_label;
        private Label total_time_label;
        private Label last_stage_label;
    }
}