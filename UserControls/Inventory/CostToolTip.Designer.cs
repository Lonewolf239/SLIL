namespace SLIL.UserControls.Inventory
{
    partial class CostToolTip
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
            this.cost_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cost_label
            // 
            this.cost_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cost_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cost_label.ForeColor = System.Drawing.Color.White;
            this.cost_label.Location = new System.Drawing.Point(0, 0);
            this.cost_label.Name = "cost_label";
            this.cost_label.Size = new System.Drawing.Size(75, 50);
            this.cost_label.TabIndex = 1;
            this.cost_label.Text = "0$";
            this.cost_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CostToolTip
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.cost_label);
            this.Name = "CostToolTip";
            this.Size = new System.Drawing.Size(75, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label cost_label;
    }
}
