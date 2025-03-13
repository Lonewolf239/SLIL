namespace SLIL.UserControls
{
    partial class StartShopInterface
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.stop_start_shop_btn = new System.Windows.Forms.Button();
            this.start_shop_title = new System.Windows.Forms.Label();
            this.start_shop_panel = new System.Windows.Forms.Panel();
            this.items_panel = new System.Windows.Forms.Panel();
            this.medical_kit_minus_btn = new System.Windows.Forms.Button();
            this.medical_kit_plus_btn = new System.Windows.Forms.Button();
            this.helmet_minus_btn = new System.Windows.Forms.Button();
            this.helmet_plus_btn = new System.Windows.Forms.Button();
            this.adrenalin_minus_btn = new System.Windows.Forms.Button();
            this.adrenalin_plus_btn = new System.Windows.Forms.Button();
            this.medkit_minus_btn = new System.Windows.Forms.Button();
            this.medkit_plus_btn = new System.Windows.Forms.Button();
            this.medical_kit_count = new System.Windows.Forms.Label();
            this.medical_kit_icon = new System.Windows.Forms.PictureBox();
            this.helmet_count = new System.Windows.Forms.Label();
            this.helmet_icon = new System.Windows.Forms.PictureBox();
            this.adrenalin_count = new System.Windows.Forms.Label();
            this.adrenalin_icon = new System.Windows.Forms.PictureBox();
            this.medkit_count = new System.Windows.Forms.Label();
            this.medkit_icon = new System.Windows.Forms.PictureBox();
            this.items_title = new System.Windows.Forms.Label();
            this.pistol_panel = new System.Windows.Forms.Panel();
            this.minus_level_btn = new System.Windows.Forms.Button();
            this.plus_level_btn = new System.Windows.Forms.Button();
            this.level_title = new System.Windows.Forms.Label();
            this.minus_ammo_btn = new System.Windows.Forms.Button();
            this.plus_ammo_btn = new System.Windows.Forms.Button();
            this.pistol_ammo_icon = new System.Windows.Forms.PictureBox();
            this.pistol_label = new System.Windows.Forms.Label();
            this.pistol_ammo_count = new System.Windows.Forms.Label();
            this.pistol_icon = new System.Windows.Forms.PictureBox();
            this.money_count = new System.Windows.Forms.Label();
            this.start_shop_panel.SuspendLayout();
            this.items_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.medical_kit_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helmet_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.adrenalin_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.medkit_icon)).BeginInit();
            this.pistol_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_ammo_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // stop_start_shop_btn
            // 
            this.stop_start_shop_btn.AutoSize = true;
            this.stop_start_shop_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.stop_start_shop_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stop_start_shop_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.stop_start_shop_btn.ForeColor = System.Drawing.Color.White;
            this.stop_start_shop_btn.Location = new System.Drawing.Point(650, 508);
            this.stop_start_shop_btn.Name = "stop_start_shop_btn";
            this.stop_start_shop_btn.Size = new System.Drawing.Size(128, 41);
            this.stop_start_shop_btn.TabIndex = 41;
            this.stop_start_shop_btn.TabStop = false;
            this.stop_start_shop_btn.Text = "ГОТОВО";
            this.stop_start_shop_btn.UseVisualStyleBackColor = true;
            this.stop_start_shop_btn.Click += new System.EventHandler(this.Stop_start_shop_btn_Click);
            // 
            // start_shop_title
            // 
            this.start_shop_title.Dock = System.Windows.Forms.DockStyle.Top;
            this.start_shop_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.start_shop_title.ForeColor = System.Drawing.Color.White;
            this.start_shop_title.Location = new System.Drawing.Point(0, 0);
            this.start_shop_title.Name = "start_shop_title";
            this.start_shop_title.Size = new System.Drawing.Size(1434, 33);
            this.start_shop_title.TabIndex = 42;
            this.start_shop_title.Text = "Стартовый закуп";
            this.start_shop_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // start_shop_panel
            // 
            this.start_shop_panel.Controls.Add(this.items_panel);
            this.start_shop_panel.Controls.Add(this.pistol_panel);
            this.start_shop_panel.Controls.Add(this.money_count);
            this.start_shop_panel.Controls.Add(this.stop_start_shop_btn);
            this.start_shop_panel.Location = new System.Drawing.Point(245, 120);
            this.start_shop_panel.Name = "start_shop_panel";
            this.start_shop_panel.Size = new System.Drawing.Size(781, 552);
            this.start_shop_panel.TabIndex = 43;
            // 
            // items_panel
            // 
            this.items_panel.Controls.Add(this.medical_kit_minus_btn);
            this.items_panel.Controls.Add(this.medical_kit_plus_btn);
            this.items_panel.Controls.Add(this.helmet_minus_btn);
            this.items_panel.Controls.Add(this.helmet_plus_btn);
            this.items_panel.Controls.Add(this.adrenalin_minus_btn);
            this.items_panel.Controls.Add(this.adrenalin_plus_btn);
            this.items_panel.Controls.Add(this.medkit_minus_btn);
            this.items_panel.Controls.Add(this.medkit_plus_btn);
            this.items_panel.Controls.Add(this.medical_kit_count);
            this.items_panel.Controls.Add(this.medical_kit_icon);
            this.items_panel.Controls.Add(this.helmet_count);
            this.items_panel.Controls.Add(this.helmet_icon);
            this.items_panel.Controls.Add(this.adrenalin_count);
            this.items_panel.Controls.Add(this.adrenalin_icon);
            this.items_panel.Controls.Add(this.medkit_count);
            this.items_panel.Controls.Add(this.medkit_icon);
            this.items_panel.Controls.Add(this.items_title);
            this.items_panel.Location = new System.Drawing.Point(3, 218);
            this.items_panel.Name = "items_panel";
            this.items_panel.Size = new System.Drawing.Size(570, 196);
            this.items_panel.TabIndex = 44;
            // 
            // medical_kit_minus_btn
            // 
            this.medical_kit_minus_btn.AutoSize = true;
            this.medical_kit_minus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.medical_kit_minus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.medical_kit_minus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medical_kit_minus_btn.ForeColor = System.Drawing.Color.White;
            this.medical_kit_minus_btn.Location = new System.Drawing.Point(477, 100);
            this.medical_kit_minus_btn.Name = "medical_kit_minus_btn";
            this.medical_kit_minus_btn.Size = new System.Drawing.Size(33, 36);
            this.medical_kit_minus_btn.TabIndex = 52;
            this.medical_kit_minus_btn.TabStop = false;
            this.medical_kit_minus_btn.Text = "-";
            this.medical_kit_minus_btn.UseVisualStyleBackColor = true;
            this.medical_kit_minus_btn.Click += new System.EventHandler(this.Medical_kit_minus_btn_Click);
            this.medical_kit_minus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.medical_kit_minus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // medical_kit_plus_btn
            // 
            this.medical_kit_plus_btn.AutoSize = true;
            this.medical_kit_plus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.medical_kit_plus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.medical_kit_plus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medical_kit_plus_btn.ForeColor = System.Drawing.Color.White;
            this.medical_kit_plus_btn.Location = new System.Drawing.Point(438, 100);
            this.medical_kit_plus_btn.Name = "medical_kit_plus_btn";
            this.medical_kit_plus_btn.Size = new System.Drawing.Size(33, 36);
            this.medical_kit_plus_btn.TabIndex = 51;
            this.medical_kit_plus_btn.TabStop = false;
            this.medical_kit_plus_btn.Text = "+";
            this.medical_kit_plus_btn.UseVisualStyleBackColor = true;
            this.medical_kit_plus_btn.Click += new System.EventHandler(this.Medical_kit_plus_btn_Click);
            this.medical_kit_plus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.medical_kit_plus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // helmet_minus_btn
            // 
            this.helmet_minus_btn.AutoSize = true;
            this.helmet_minus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.helmet_minus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.helmet_minus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helmet_minus_btn.ForeColor = System.Drawing.Color.White;
            this.helmet_minus_btn.Location = new System.Drawing.Point(477, 44);
            this.helmet_minus_btn.Name = "helmet_minus_btn";
            this.helmet_minus_btn.Size = new System.Drawing.Size(33, 36);
            this.helmet_minus_btn.TabIndex = 50;
            this.helmet_minus_btn.TabStop = false;
            this.helmet_minus_btn.Text = "-";
            this.helmet_minus_btn.UseVisualStyleBackColor = true;
            this.helmet_minus_btn.Click += new System.EventHandler(this.Helmet_minus_btn_Click);
            this.helmet_minus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.helmet_minus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // helmet_plus_btn
            // 
            this.helmet_plus_btn.AutoSize = true;
            this.helmet_plus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.helmet_plus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.helmet_plus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helmet_plus_btn.ForeColor = System.Drawing.Color.White;
            this.helmet_plus_btn.Location = new System.Drawing.Point(438, 44);
            this.helmet_plus_btn.Name = "helmet_plus_btn";
            this.helmet_plus_btn.Size = new System.Drawing.Size(33, 36);
            this.helmet_plus_btn.TabIndex = 49;
            this.helmet_plus_btn.TabStop = false;
            this.helmet_plus_btn.Text = "+";
            this.helmet_plus_btn.UseVisualStyleBackColor = true;
            this.helmet_plus_btn.Click += new System.EventHandler(this.Helmet_plus_btn_Click);
            this.helmet_plus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.helmet_plus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // adrenalin_minus_btn
            // 
            this.adrenalin_minus_btn.AutoSize = true;
            this.adrenalin_minus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.adrenalin_minus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.adrenalin_minus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.adrenalin_minus_btn.ForeColor = System.Drawing.Color.White;
            this.adrenalin_minus_btn.Location = new System.Drawing.Point(191, 100);
            this.adrenalin_minus_btn.Name = "adrenalin_minus_btn";
            this.adrenalin_minus_btn.Size = new System.Drawing.Size(33, 36);
            this.adrenalin_minus_btn.TabIndex = 48;
            this.adrenalin_minus_btn.TabStop = false;
            this.adrenalin_minus_btn.Text = "-";
            this.adrenalin_minus_btn.UseVisualStyleBackColor = true;
            this.adrenalin_minus_btn.Click += new System.EventHandler(this.Adrenalin_minus_btn_Click);
            this.adrenalin_minus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.adrenalin_minus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // adrenalin_plus_btn
            // 
            this.adrenalin_plus_btn.AutoSize = true;
            this.adrenalin_plus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.adrenalin_plus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.adrenalin_plus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.adrenalin_plus_btn.ForeColor = System.Drawing.Color.White;
            this.adrenalin_plus_btn.Location = new System.Drawing.Point(152, 100);
            this.adrenalin_plus_btn.Name = "adrenalin_plus_btn";
            this.adrenalin_plus_btn.Size = new System.Drawing.Size(33, 36);
            this.adrenalin_plus_btn.TabIndex = 47;
            this.adrenalin_plus_btn.TabStop = false;
            this.adrenalin_plus_btn.Text = "+";
            this.adrenalin_plus_btn.UseVisualStyleBackColor = true;
            this.adrenalin_plus_btn.Click += new System.EventHandler(this.Adrenalin_plus_btn_Click);
            this.adrenalin_plus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.adrenalin_plus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // medkit_minus_btn
            // 
            this.medkit_minus_btn.AutoSize = true;
            this.medkit_minus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.medkit_minus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.medkit_minus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medkit_minus_btn.ForeColor = System.Drawing.Color.White;
            this.medkit_minus_btn.Location = new System.Drawing.Point(191, 44);
            this.medkit_minus_btn.Name = "medkit_minus_btn";
            this.medkit_minus_btn.Size = new System.Drawing.Size(33, 36);
            this.medkit_minus_btn.TabIndex = 46;
            this.medkit_minus_btn.TabStop = false;
            this.medkit_minus_btn.Text = "-";
            this.medkit_minus_btn.UseVisualStyleBackColor = true;
            this.medkit_minus_btn.Click += new System.EventHandler(this.Medkit_minus_btn_Click);
            this.medkit_minus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.medkit_minus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // medkit_plus_btn
            // 
            this.medkit_plus_btn.AutoSize = true;
            this.medkit_plus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.medkit_plus_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.medkit_plus_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medkit_plus_btn.ForeColor = System.Drawing.Color.White;
            this.medkit_plus_btn.Location = new System.Drawing.Point(152, 44);
            this.medkit_plus_btn.Name = "medkit_plus_btn";
            this.medkit_plus_btn.Size = new System.Drawing.Size(33, 36);
            this.medkit_plus_btn.TabIndex = 45;
            this.medkit_plus_btn.TabStop = false;
            this.medkit_plus_btn.Text = "+";
            this.medkit_plus_btn.UseVisualStyleBackColor = true;
            this.medkit_plus_btn.Click += new System.EventHandler(this.Medkit_plus_btn_Click);
            this.medkit_plus_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.medkit_plus_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // medical_kit_count
            // 
            this.medical_kit_count.AutoEllipsis = true;
            this.medical_kit_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medical_kit_count.ForeColor = System.Drawing.Color.White;
            this.medical_kit_count.Location = new System.Drawing.Point(350, 98);
            this.medical_kit_count.Name = "medical_kit_count";
            this.medical_kit_count.Size = new System.Drawing.Size(82, 32);
            this.medical_kit_count.TabIndex = 24;
            this.medical_kit_count.Text = "0/0";
            this.medical_kit_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // medical_kit_icon
            // 
            this.medical_kit_icon.Image = global::SLIL.Properties.Resources.super_medical_kit_icon;
            this.medical_kit_icon.Location = new System.Drawing.Point(294, 89);
            this.medical_kit_icon.Name = "medical_kit_icon";
            this.medical_kit_icon.Size = new System.Drawing.Size(50, 50);
            this.medical_kit_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.medical_kit_icon.TabIndex = 23;
            this.medical_kit_icon.TabStop = false;
            this.medical_kit_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.medical_kit_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // helmet_count
            // 
            this.helmet_count.AutoEllipsis = true;
            this.helmet_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helmet_count.ForeColor = System.Drawing.Color.White;
            this.helmet_count.Location = new System.Drawing.Point(350, 42);
            this.helmet_count.Name = "helmet_count";
            this.helmet_count.Size = new System.Drawing.Size(82, 32);
            this.helmet_count.TabIndex = 22;
            this.helmet_count.Text = "0/0";
            this.helmet_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // helmet_icon
            // 
            this.helmet_icon.Image = global::SLIL.Properties.Resources.helmet_count_icon;
            this.helmet_icon.Location = new System.Drawing.Point(294, 33);
            this.helmet_icon.Name = "helmet_icon";
            this.helmet_icon.Size = new System.Drawing.Size(50, 50);
            this.helmet_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.helmet_icon.TabIndex = 21;
            this.helmet_icon.TabStop = false;
            this.helmet_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.helmet_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // adrenalin_count
            // 
            this.adrenalin_count.AutoEllipsis = true;
            this.adrenalin_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.adrenalin_count.ForeColor = System.Drawing.Color.White;
            this.adrenalin_count.Location = new System.Drawing.Point(64, 98);
            this.adrenalin_count.Name = "adrenalin_count";
            this.adrenalin_count.Size = new System.Drawing.Size(82, 32);
            this.adrenalin_count.TabIndex = 20;
            this.adrenalin_count.Text = "0/0";
            this.adrenalin_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // adrenalin_icon
            // 
            this.adrenalin_icon.Image = global::SLIL.Properties.Resources.adrenalin_count_icon;
            this.adrenalin_icon.Location = new System.Drawing.Point(8, 89);
            this.adrenalin_icon.Name = "adrenalin_icon";
            this.adrenalin_icon.Size = new System.Drawing.Size(50, 50);
            this.adrenalin_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.adrenalin_icon.TabIndex = 19;
            this.adrenalin_icon.TabStop = false;
            this.adrenalin_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.adrenalin_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // medkit_count
            // 
            this.medkit_count.AutoEllipsis = true;
            this.medkit_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.medkit_count.ForeColor = System.Drawing.Color.White;
            this.medkit_count.Location = new System.Drawing.Point(64, 42);
            this.medkit_count.Name = "medkit_count";
            this.medkit_count.Size = new System.Drawing.Size(82, 32);
            this.medkit_count.TabIndex = 18;
            this.medkit_count.Text = "0/0";
            this.medkit_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // medkit_icon
            // 
            this.medkit_icon.Image = global::SLIL.Properties.Resources.first_aid;
            this.medkit_icon.Location = new System.Drawing.Point(8, 33);
            this.medkit_icon.Name = "medkit_icon";
            this.medkit_icon.Size = new System.Drawing.Size(50, 50);
            this.medkit_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.medkit_icon.TabIndex = 17;
            this.medkit_icon.TabStop = false;
            this.medkit_icon.MouseEnter += new System.EventHandler(this.Info_icon_MouseEnter);
            this.medkit_icon.MouseLeave += new System.EventHandler(this.Info_icon_MouseLeave);
            // 
            // items_title
            // 
            this.items_title.AutoEllipsis = true;
            this.items_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.items_title.ForeColor = System.Drawing.Color.White;
            this.items_title.Location = new System.Drawing.Point(3, 0);
            this.items_title.Name = "items_title";
            this.items_title.Size = new System.Drawing.Size(325, 30);
            this.items_title.TabIndex = 6;
            this.items_title.Text = "Предметы";
            this.items_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pistol_panel
            // 
            this.pistol_panel.Controls.Add(this.minus_level_btn);
            this.pistol_panel.Controls.Add(this.plus_level_btn);
            this.pistol_panel.Controls.Add(this.level_title);
            this.pistol_panel.Controls.Add(this.minus_ammo_btn);
            this.pistol_panel.Controls.Add(this.plus_ammo_btn);
            this.pistol_panel.Controls.Add(this.pistol_ammo_icon);
            this.pistol_panel.Controls.Add(this.pistol_label);
            this.pistol_panel.Controls.Add(this.pistol_ammo_count);
            this.pistol_panel.Controls.Add(this.pistol_icon);
            this.pistol_panel.Location = new System.Drawing.Point(3, 3);
            this.pistol_panel.Name = "pistol_panel";
            this.pistol_panel.Size = new System.Drawing.Size(570, 209);
            this.pistol_panel.TabIndex = 43;
            // 
            // minus_level_btn
            // 
            this.minus_level_btn.AutoSize = true;
            this.minus_level_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.minus_level_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minus_level_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.minus_level_btn.ForeColor = System.Drawing.Color.White;
            this.minus_level_btn.Location = new System.Drawing.Point(481, 121);
            this.minus_level_btn.Name = "minus_level_btn";
            this.minus_level_btn.Size = new System.Drawing.Size(33, 36);
            this.minus_level_btn.TabIndex = 47;
            this.minus_level_btn.TabStop = false;
            this.minus_level_btn.Text = "-";
            this.minus_level_btn.UseVisualStyleBackColor = true;
            this.minus_level_btn.Click += new System.EventHandler(this.Minus_level_btn_Click);
            this.minus_level_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.minus_level_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // plus_level_btn
            // 
            this.plus_level_btn.AutoSize = true;
            this.plus_level_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.plus_level_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plus_level_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.plus_level_btn.ForeColor = System.Drawing.Color.White;
            this.plus_level_btn.Location = new System.Drawing.Point(442, 121);
            this.plus_level_btn.Name = "plus_level_btn";
            this.plus_level_btn.Size = new System.Drawing.Size(33, 36);
            this.plus_level_btn.TabIndex = 46;
            this.plus_level_btn.TabStop = false;
            this.plus_level_btn.Text = "+";
            this.plus_level_btn.UseVisualStyleBackColor = true;
            this.plus_level_btn.Click += new System.EventHandler(this.Plus_level_btn_Click);
            this.plus_level_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.plus_level_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // level_title
            // 
            this.level_title.AutoEllipsis = true;
            this.level_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.level_title.ForeColor = System.Drawing.Color.White;
            this.level_title.Location = new System.Drawing.Point(326, 94);
            this.level_title.Name = "level_title";
            this.level_title.Size = new System.Drawing.Size(120, 24);
            this.level_title.TabIndex = 45;
            this.level_title.Text = "Уровень";
            this.level_title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // minus_ammo_btn
            // 
            this.minus_ammo_btn.AutoSize = true;
            this.minus_ammo_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.minus_ammo_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minus_ammo_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.minus_ammo_btn.ForeColor = System.Drawing.Color.White;
            this.minus_ammo_btn.Location = new System.Drawing.Point(481, 30);
            this.minus_ammo_btn.Name = "minus_ammo_btn";
            this.minus_ammo_btn.Size = new System.Drawing.Size(33, 36);
            this.minus_ammo_btn.TabIndex = 44;
            this.minus_ammo_btn.TabStop = false;
            this.minus_ammo_btn.Text = "-";
            this.minus_ammo_btn.UseVisualStyleBackColor = true;
            this.minus_ammo_btn.Click += new System.EventHandler(this.Minus_ammo_btn_Click);
            this.minus_ammo_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.minus_ammo_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // plus_ammo_btn
            // 
            this.plus_ammo_btn.AutoSize = true;
            this.plus_ammo_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.plus_ammo_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plus_ammo_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.plus_ammo_btn.ForeColor = System.Drawing.Color.White;
            this.plus_ammo_btn.Location = new System.Drawing.Point(442, 30);
            this.plus_ammo_btn.Name = "plus_ammo_btn";
            this.plus_ammo_btn.Size = new System.Drawing.Size(33, 36);
            this.plus_ammo_btn.TabIndex = 42;
            this.plus_ammo_btn.TabStop = false;
            this.plus_ammo_btn.Text = "+";
            this.plus_ammo_btn.UseVisualStyleBackColor = true;
            this.plus_ammo_btn.Click += new System.EventHandler(this.Plus_ammo_btn_Click);
            this.plus_ammo_btn.MouseEnter += new System.EventHandler(this.Plus_btn_MouseEnter);
            this.plus_ammo_btn.MouseLeave += new System.EventHandler(this.Plus_btn_MouseLeave);
            // 
            // pistol_ammo_icon
            // 
            this.pistol_ammo_icon.Image = global::SLIL.Properties.Resources.bullet;
            this.pistol_ammo_icon.Location = new System.Drawing.Point(412, 36);
            this.pistol_ammo_icon.Name = "pistol_ammo_icon";
            this.pistol_ammo_icon.Size = new System.Drawing.Size(24, 24);
            this.pistol_ammo_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pistol_ammo_icon.TabIndex = 8;
            this.pistol_ammo_icon.TabStop = false;
            // 
            // pistol_label
            // 
            this.pistol_label.AutoEllipsis = true;
            this.pistol_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pistol_label.ForeColor = System.Drawing.Color.White;
            this.pistol_label.Location = new System.Drawing.Point(326, 3);
            this.pistol_label.Name = "pistol_label";
            this.pistol_label.Size = new System.Drawing.Size(199, 24);
            this.pistol_label.TabIndex = 6;
            this.pistol_label.Text = "Пистолет";
            this.pistol_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pistol_ammo_count
            // 
            this.pistol_ammo_count.AutoEllipsis = true;
            this.pistol_ammo_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pistol_ammo_count.ForeColor = System.Drawing.Color.White;
            this.pistol_ammo_count.Location = new System.Drawing.Point(326, 36);
            this.pistol_ammo_count.Name = "pistol_ammo_count";
            this.pistol_ammo_count.Size = new System.Drawing.Size(82, 24);
            this.pistol_ammo_count.TabIndex = 7;
            this.pistol_ammo_count.Text = "0/0";
            this.pistol_ammo_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pistol_icon
            // 
            this.pistol_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pistol_icon.Location = new System.Drawing.Point(3, 3);
            this.pistol_icon.Name = "pistol_icon";
            this.pistol_icon.Size = new System.Drawing.Size(317, 178);
            this.pistol_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pistol_icon.TabIndex = 5;
            this.pistol_icon.TabStop = false;
            this.pistol_icon.MouseEnter += new System.EventHandler(this.Pistol_icon_MouseEnter);
            this.pistol_icon.MouseLeave += new System.EventHandler(this.Pistol_icon_MouseLeave);
            // 
            // money_count
            // 
            this.money_count.AutoEllipsis = true;
            this.money_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.money_count.ForeColor = System.Drawing.Color.White;
            this.money_count.Location = new System.Drawing.Point(578, 475);
            this.money_count.Name = "money_count";
            this.money_count.Size = new System.Drawing.Size(200, 30);
            this.money_count.TabIndex = 42;
            this.money_count.Text = "Деньги 0$";
            this.money_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StartShopInterface
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.start_shop_panel);
            this.Controls.Add(this.start_shop_title);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "StartShopInterface";
            this.Size = new System.Drawing.Size(1434, 675);
            this.Load += new System.EventHandler(this.StartShopInterface_Load);
            this.start_shop_panel.ResumeLayout(false);
            this.start_shop_panel.PerformLayout();
            this.items_panel.ResumeLayout(false);
            this.items_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.medical_kit_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helmet_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.adrenalin_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.medkit_icon)).EndInit();
            this.pistol_panel.ResumeLayout(false);
            this.pistol_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_ammo_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pistol_icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button stop_start_shop_btn;
        private System.Windows.Forms.Label start_shop_title;
        private System.Windows.Forms.Panel start_shop_panel;
        private System.Windows.Forms.Label money_count;
        private System.Windows.Forms.Panel pistol_panel;
        private System.Windows.Forms.Panel items_panel;
        private System.Windows.Forms.PictureBox pistol_icon;
        private System.Windows.Forms.PictureBox pistol_ammo_icon;
        private System.Windows.Forms.Label pistol_label;
        private System.Windows.Forms.Label pistol_ammo_count;
        internal System.Windows.Forms.Button minus_ammo_btn;
        internal System.Windows.Forms.Button plus_ammo_btn;
        private System.Windows.Forms.Label items_title;
        internal System.Windows.Forms.Button minus_level_btn;
        internal System.Windows.Forms.Button plus_level_btn;
        private System.Windows.Forms.Label level_title;
        private System.Windows.Forms.Label medical_kit_count;
        private System.Windows.Forms.PictureBox medical_kit_icon;
        private System.Windows.Forms.Label helmet_count;
        private System.Windows.Forms.PictureBox helmet_icon;
        private System.Windows.Forms.Label adrenalin_count;
        private System.Windows.Forms.PictureBox adrenalin_icon;
        private System.Windows.Forms.Label medkit_count;
        private System.Windows.Forms.PictureBox medkit_icon;
        internal System.Windows.Forms.Button medical_kit_minus_btn;
        internal System.Windows.Forms.Button medical_kit_plus_btn;
        internal System.Windows.Forms.Button helmet_minus_btn;
        internal System.Windows.Forms.Button helmet_plus_btn;
        internal System.Windows.Forms.Button adrenalin_minus_btn;
        internal System.Windows.Forms.Button adrenalin_plus_btn;
        internal System.Windows.Forms.Button medkit_minus_btn;
        internal System.Windows.Forms.Button medkit_plus_btn;
    }
}
