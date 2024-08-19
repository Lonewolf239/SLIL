namespace SLIL.UserControls
{
    partial class EditorElementSelector
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
            this.element_color = new System.Windows.Forms.Panel();
            this.element_name = new System.Windows.Forms.Label();
            this.select_btn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.select_btn)).BeginInit();
            this.SuspendLayout();
            // 
            // element_color
            // 
            this.element_color.Dock = System.Windows.Forms.DockStyle.Left;
            this.element_color.Location = new System.Drawing.Point(0, 0);
            this.element_color.MaximumSize = new System.Drawing.Size(30, 30);
            this.element_color.MinimumSize = new System.Drawing.Size(30, 30);
            this.element_color.Name = "element_color";
            this.element_color.Size = new System.Drawing.Size(30, 30);
            this.element_color.TabIndex = 0;
            // 
            // element_name
            // 
            this.element_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.element_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.element_name.Location = new System.Drawing.Point(30, 0);
            this.element_name.Name = "element_name";
            this.element_name.Size = new System.Drawing.Size(140, 30);
            this.element_name.TabIndex = 1;
            this.element_name.Text = "[NAME]";
            this.element_name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // select_btn
            // 
            this.select_btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.select_btn.Location = new System.Drawing.Point(170, 0);
            this.select_btn.MaximumSize = new System.Drawing.Size(30, 30);
            this.select_btn.MinimumSize = new System.Drawing.Size(30, 30);
            this.select_btn.Name = "select_btn";
            this.select_btn.Size = new System.Drawing.Size(30, 30);
            this.select_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.select_btn.TabIndex = 15;
            this.select_btn.TabStop = false;
            // 
            // EditorElementSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.element_name);
            this.Controls.Add(this.select_btn);
            this.Controls.Add(this.element_color);
            this.Name = "EditorElementSelector";
            this.Size = new System.Drawing.Size(200, 30);
            this.Load += new System.EventHandler(this.EditorElementSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.select_btn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Panel element_color;
        public System.Windows.Forms.Label element_name;
        public System.Windows.Forms.PictureBox select_btn;
    }
}
