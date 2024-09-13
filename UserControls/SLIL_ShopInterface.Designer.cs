namespace SLIL.UserControls
{
    partial class SLIL_ShopInterface
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
            this.weapon_name = new System.Windows.Forms.Label();
            this.buy_button = new System.Windows.Forms.Button();
            this.ammo_count = new System.Windows.Forms.Label();
            this.update_button = new System.Windows.Forms.Button();
            this.weapon_icon = new System.Windows.Forms.PictureBox();
            this.ammo_icon = new System.Windows.Forms.PictureBox();
            this.ammo_panel = new System.Windows.Forms.Panel();
            this.parametrs_image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammo_icon)).BeginInit();
            this.ammo_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).BeginInit();
            this.SuspendLayout();
            // 
            // weapon_name
            // 
            this.weapon_name.AutoEllipsis = true;
            this.weapon_name.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_name.ForeColor = System.Drawing.Color.White;
            this.weapon_name.Location = new System.Drawing.Point(3, 128);
            this.weapon_name.Name = "weapon_name";
            this.weapon_name.Size = new System.Drawing.Size(245, 24);
            this.weapon_name.TabIndex = 1;
            this.weapon_name.Text = "[NAME]";
            this.weapon_name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buy_button
            // 
            this.buy_button.AutoSize = true;
            this.buy_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buy_button.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buy_button.ForeColor = System.Drawing.Color.White;
            this.buy_button.Location = new System.Drawing.Point(253, 137);
            this.buy_button.Name = "buy_button";
            this.buy_button.Size = new System.Drawing.Size(221, 44);
            this.buy_button.TabIndex = 3;
            this.buy_button.TabStop = false;
            this.buy_button.Text = "Купить оружие";
            this.buy_button.UseVisualStyleBackColor = true;
            this.buy_button.Click += new System.EventHandler(this.Buy_button_Click);
            // 
            // ammo_count
            // 
            this.ammo_count.Dock = System.Windows.Forms.DockStyle.Right;
            this.ammo_count.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ammo_count.ForeColor = System.Drawing.Color.White;
            this.ammo_count.Location = new System.Drawing.Point(35, 0);
            this.ammo_count.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ammo_count.Name = "ammo_count";
            this.ammo_count.Size = new System.Drawing.Size(115, 30);
            this.ammo_count.TabIndex = 4;
            this.ammo_count.Text = "[AMMO]";
            this.ammo_count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // update_button
            // 
            this.update_button.AutoSize = true;
            this.update_button.BackgroundImage = global::SLIL.Properties.Resources.level_up;
            this.update_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.update_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.update_button.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Bold);
            this.update_button.ForeColor = System.Drawing.Color.White;
            this.update_button.Location = new System.Drawing.Point(480, 137);
            this.update_button.Name = "update_button";
            this.update_button.Size = new System.Drawing.Size(71, 44);
            this.update_button.TabIndex = 6;
            this.update_button.TabStop = false;
            this.update_button.Text = "$20";
            this.update_button.UseVisualStyleBackColor = true;
            this.update_button.Click += new System.EventHandler(this.Update_button_Click);
            // 
            // weapon_icon
            // 
            this.weapon_icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.weapon_icon.Location = new System.Drawing.Point(3, 3);
            this.weapon_icon.Name = "weapon_icon";
            this.weapon_icon.Size = new System.Drawing.Size(245, 122);
            this.weapon_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.weapon_icon.TabIndex = 0;
            this.weapon_icon.TabStop = false;
            // 
            // ammo_icon
            // 
            this.ammo_icon.Dock = System.Windows.Forms.DockStyle.Left;
            this.ammo_icon.Location = new System.Drawing.Point(0, 0);
            this.ammo_icon.Name = "ammo_icon";
            this.ammo_icon.Size = new System.Drawing.Size(30, 30);
            this.ammo_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ammo_icon.TabIndex = 8;
            this.ammo_icon.TabStop = false;
            // 
            // ammo_panel
            // 
            this.ammo_panel.Controls.Add(this.ammo_count);
            this.ammo_panel.Controls.Add(this.ammo_icon);
            this.ammo_panel.Location = new System.Drawing.Point(25, 155);
            this.ammo_panel.Name = "ammo_panel";
            this.ammo_panel.Size = new System.Drawing.Size(150, 30);
            this.ammo_panel.TabIndex = 15;
            // 
            // parametrs_image
            // 
            this.parametrs_image.Location = new System.Drawing.Point(254, 8);
            this.parametrs_image.Name = "parametrs_image";
            this.parametrs_image.Size = new System.Drawing.Size(500, 120);
            this.parametrs_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.parametrs_image.TabIndex = 16;
            this.parametrs_image.TabStop = false;
            // 
            // SLIL_ShopInterface
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.parametrs_image);
            this.Controls.Add(this.ammo_panel);
            this.Controls.Add(this.update_button);
            this.Controls.Add(this.buy_button);
            this.Controls.Add(this.weapon_name);
            this.Controls.Add(this.weapon_icon);
            this.Name = "SLIL_ShopInterface";
            this.Size = new System.Drawing.Size(884, 188);
            this.VisibleChanged += new System.EventHandler(this.SLIL_ShopInterface_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.weapon_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammo_icon)).EndInit();
            this.ammo_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox weapon_icon;
        private System.Windows.Forms.Label weapon_name;
        private System.Windows.Forms.Button buy_button;
        private System.Windows.Forms.Label ammo_count;
        private System.Windows.Forms.Button update_button;
        private System.Windows.Forms.PictureBox ammo_icon;
        private System.Windows.Forms.Panel ammo_panel;
        private System.Windows.Forms.PictureBox parametrs_image;
    }
}
