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
            this.damage_text = new System.Windows.Forms.Label();
            this.update_button = new System.Windows.Forms.Button();
            this.weapon_icon = new System.Windows.Forms.PictureBox();
            this.accuracy_text = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.weapon_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // weapon_name
            // 
            this.weapon_name.AutoSize = true;
            this.weapon_name.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weapon_name.ForeColor = System.Drawing.Color.White;
            this.weapon_name.Location = new System.Drawing.Point(166, 0);
            this.weapon_name.Name = "weapon_name";
            this.weapon_name.Size = new System.Drawing.Size(70, 22);
            this.weapon_name.TabIndex = 1;
            this.weapon_name.Text = "[NAME]";
            // 
            // buy_button
            // 
            this.buy_button.AutoSize = true;
            this.buy_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buy_button.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buy_button.ForeColor = System.Drawing.Color.White;
            this.buy_button.Location = new System.Drawing.Point(166, 66);
            this.buy_button.Name = "buy_button";
            this.buy_button.Size = new System.Drawing.Size(138, 31);
            this.buy_button.TabIndex = 3;
            this.buy_button.TabStop = false;
            this.buy_button.Text = "Купить оружие";
            this.buy_button.UseVisualStyleBackColor = true;
            this.buy_button.Click += new System.EventHandler(this.Buy_button_Click);
            // 
            // ammo_count
            // 
            this.ammo_count.AutoSize = true;
            this.ammo_count.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ammo_count.ForeColor = System.Drawing.Color.White;
            this.ammo_count.Location = new System.Drawing.Point(253, 20);
            this.ammo_count.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ammo_count.Name = "ammo_count";
            this.ammo_count.Size = new System.Drawing.Size(117, 19);
            this.ammo_count.TabIndex = 4;
            this.ammo_count.Text = "[AMMO_COUNT]";
            // 
            // damage_text
            // 
            this.damage_text.AutoSize = true;
            this.damage_text.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.damage_text.ForeColor = System.Drawing.Color.White;
            this.damage_text.Location = new System.Drawing.Point(166, 20);
            this.damage_text.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.damage_text.Name = "damage_text";
            this.damage_text.Size = new System.Drawing.Size(81, 19);
            this.damage_text.TabIndex = 5;
            this.damage_text.Text = "[DAMAGE]";
            // 
            // update_button
            // 
            this.update_button.AutoSize = true;
            this.update_button.BackgroundImage = global::SLIL.Properties.Resources.level_up;
            this.update_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.update_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.update_button.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.update_button.ForeColor = System.Drawing.Color.White;
            this.update_button.Location = new System.Drawing.Point(310, 61);
            this.update_button.Name = "update_button";
            this.update_button.Size = new System.Drawing.Size(58, 36);
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
            this.weapon_icon.Size = new System.Drawing.Size(157, 94);
            this.weapon_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.weapon_icon.TabIndex = 0;
            this.weapon_icon.TabStop = false;
            // 
            // accuracy_text
            // 
            this.accuracy_text.AutoSize = true;
            this.accuracy_text.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.accuracy_text.ForeColor = System.Drawing.Color.White;
            this.accuracy_text.Location = new System.Drawing.Point(166, 42);
            this.accuracy_text.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.accuracy_text.Name = "accuracy_text";
            this.accuracy_text.Size = new System.Drawing.Size(99, 19);
            this.accuracy_text.TabIndex = 7;
            this.accuracy_text.Text = "[ACCURACY]";
            // 
            // SLIL_ShopInterface
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.accuracy_text);
            this.Controls.Add(this.update_button);
            this.Controls.Add(this.damage_text);
            this.Controls.Add(this.ammo_count);
            this.Controls.Add(this.buy_button);
            this.Controls.Add(this.weapon_name);
            this.Controls.Add(this.weapon_icon);
            this.Name = "SLIL_ShopInterface";
            this.Size = new System.Drawing.Size(454, 100);
            this.VisibleChanged += new System.EventHandler(this.SLIL_ShopInterface_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.weapon_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox weapon_icon;
        private System.Windows.Forms.Label weapon_name;
        private System.Windows.Forms.Button buy_button;
        private System.Windows.Forms.Label ammo_count;
        private System.Windows.Forms.Label damage_text;
        private System.Windows.Forms.Button update_button;
        private System.Windows.Forms.Label accuracy_text;
    }
}
