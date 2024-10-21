namespace SLIL.UserControls.Inventory
{
    partial class WeaponToolTip
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
            this.parametrs_image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).BeginInit();
            this.SuspendLayout();
            // 
            // parametrs_image
            // 
            this.parametrs_image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametrs_image.Location = new System.Drawing.Point(0, 0);
            this.parametrs_image.Name = "parametrs_image";
            this.parametrs_image.Size = new System.Drawing.Size(390, 118);
            this.parametrs_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.parametrs_image.TabIndex = 0;
            this.parametrs_image.TabStop = false;
            // 
            // WeaponToolTip
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.parametrs_image);
            this.DoubleBuffered = true;
            this.Name = "WeaponToolTip";
            this.Size = new System.Drawing.Size(390, 118);
            this.Load += new System.EventHandler(this.WeaponToolTip_Load);
            ((System.ComponentModel.ISupportInitialize)(this.parametrs_image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox parametrs_image;
    }
}
