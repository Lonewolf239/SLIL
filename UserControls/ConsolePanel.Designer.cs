namespace SLIL.UserControls
{
    partial class ConsolePanel
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
            this.console = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.console.Cursor = System.Windows.Forms.Cursors.Default;
            this.console.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.console.ForeColor = System.Drawing.Color.Lime;
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.console.Size = new System.Drawing.Size(450, 252);
            this.console.TabIndex = 32;
            this.console.TabStop = false;
            this.console.Text = "";
            this.console.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Console_LinkClicked);
            this.console.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Console_MouseClick);
            this.console.TextChanged += new System.EventHandler(this.Console_TextChanged);
            this.console.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Console_KeyDown);
            this.console.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Console_KeyUp);
            // 
            // ConsolePanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.console);
            this.Name = "ConsolePanel";
            this.Size = new System.Drawing.Size(450, 252);
            this.Load += new System.EventHandler(this.Console_Load);
            this.VisibleChanged += new System.EventHandler(this.Console_panel_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox console;
    }
}
