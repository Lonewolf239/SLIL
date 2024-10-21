namespace SLIL.UserControls.Inventory
{
    partial class InfoToolTip
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
            this.description_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // description_label
            // 
            this.description_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.description_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.description_label.ForeColor = System.Drawing.Color.White;
            this.description_label.Location = new System.Drawing.Point(0, 0);
            this.description_label.Name = "description_label";
            this.description_label.Size = new System.Drawing.Size(500, 90);
            this.description_label.TabIndex = 0;
            this.description_label.Text = "[DESCRIPTION]";
            this.description_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InfoToolTip
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.description_label);
            this.Name = "InfoToolTip";
            this.Size = new System.Drawing.Size(500, 90);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label description_label;
    }
}
