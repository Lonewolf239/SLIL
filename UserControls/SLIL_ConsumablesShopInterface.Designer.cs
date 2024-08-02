namespace SLIL.UserControls
{
    partial class SLIL_ConsumablesShopInterface
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
            this.buy_button = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.Label();
            this.icon = new System.Windows.Forms.PictureBox();
            this.descryption = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // buy_button
            // 
            this.buy_button.AutoSize = true;
            this.buy_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buy_button.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buy_button.ForeColor = System.Drawing.Color.White;
            this.buy_button.Location = new System.Drawing.Point(199, 90);
            this.buy_button.Name = "buy_button";
            this.buy_button.Size = new System.Drawing.Size(82, 34);
            this.buy_button.TabIndex = 5;
            this.buy_button.TabStop = false;
            this.buy_button.Text = "Купить";
            this.buy_button.UseVisualStyleBackColor = true;
            this.buy_button.Click += new System.EventHandler(this.Buy_button_Click);
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.name.ForeColor = System.Drawing.Color.White;
            this.name.Location = new System.Drawing.Point(199, 3);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(70, 22);
            this.name.TabIndex = 6;
            this.name.Text = "[NAME]";
            // 
            // icon
            // 
            this.icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.icon.Location = new System.Drawing.Point(3, 3);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(190, 121);
            this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.icon.TabIndex = 2;
            this.icon.TabStop = false;
            // 
            // descryption
            // 
            this.descryption.AutoEllipsis = true;
            this.descryption.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.descryption.ForeColor = System.Drawing.Color.White;
            this.descryption.Location = new System.Drawing.Point(199, 28);
            this.descryption.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.descryption.Name = "descryption";
            this.descryption.Size = new System.Drawing.Size(250, 59);
            this.descryption.TabIndex = 7;
            this.descryption.Text = "[DESCRIPTION]\r\n[DESCRIPTION]\r\n";
            // 
            // SLIL_ConsumablesShopInterface
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.descryption);
            this.Controls.Add(this.name);
            this.Controls.Add(this.buy_button);
            this.Controls.Add(this.icon);
            this.Name = "SLIL_ConsumablesShopInterface";
            this.Size = new System.Drawing.Size(452, 127);
            this.VisibleChanged += new System.EventHandler(this.SLIL_ConsumablesShopInterface_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox icon;
        public System.Windows.Forms.Button buy_button;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label descryption;
    }
}
