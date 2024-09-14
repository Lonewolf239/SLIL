namespace SLIL.UserControls
{
    partial class SLIL_TransportStoreInterface
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
            this.parametrs_image = new System.Windows.Forms.PictureBox();
            this.icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // buy_button
            // 
            this.buy_button.AutoSize = true;
            this.buy_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buy_button.Font = new System.Drawing.Font("Consolas", 20.25F);
            this.buy_button.ForeColor = System.Drawing.Color.White;
            this.buy_button.Location = new System.Drawing.Point(237, 137);
            this.buy_button.Name = "buy_button";
            this.buy_button.Size = new System.Drawing.Size(116, 44);
            this.buy_button.TabIndex = 9;
            this.buy_button.TabStop = false;
            this.buy_button.Text = "Купить";
            this.buy_button.UseVisualStyleBackColor = true;
            this.buy_button.Click += new System.EventHandler(this.Buy_button_Click);
            // 
            // name
            // 
            this.name.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.name.ForeColor = System.Drawing.Color.White;
            this.name.Location = new System.Drawing.Point(3, 134);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(228, 47);
            this.name.TabIndex = 8;
            this.name.Text = "[NAME]";
            this.name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // parametrs_image
            // 
            this.parametrs_image.Location = new System.Drawing.Point(237, 8);
            this.parametrs_image.Name = "parametrs_image";
            this.parametrs_image.Size = new System.Drawing.Size(500, 117);
            this.parametrs_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.parametrs_image.TabIndex = 17;
            this.parametrs_image.TabStop = false;
            // 
            // icon
            // 
            this.icon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.icon.Location = new System.Drawing.Point(3, 3);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(228, 128);
            this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.icon.TabIndex = 7;
            this.icon.TabStop = false;
            // 
            // SLIL_TransportStoreInterface
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.parametrs_image);
            this.Controls.Add(this.buy_button);
            this.Controls.Add(this.name);
            this.Controls.Add(this.icon);
            this.Name = "SLIL_TransportStoreInterface";
            this.Size = new System.Drawing.Size(884, 188);
            this.VisibleChanged += new System.EventHandler(this.SLIL_TransportStoreInterface_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button buy_button;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.PictureBox icon;
        private System.Windows.Forms.PictureBox parametrs_image;
    }
}
