namespace SLIL.UserControls
{
    partial class Display
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RenderTarget?.Dispose();
                RenderFactory?.Dispose();
                Buffer?.Dispose();
                Screen?.Dispose();
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
            this.SuspendLayout();
            // 
            // Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.DoubleBuffered = true;
            this.Name = "Display";
            this.Size = new System.Drawing.Size(454, 256);
            this.ResumeLayout(false);

        }

        #endregion
    }
}