namespace SLIL.UserControls
{
    partial class PlayerPanel
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
            this.player_name = new System.Windows.Forms.Label();
            this.host_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // player_name
            // 
            this.player_name.Dock = System.Windows.Forms.DockStyle.Left;
            this.player_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.player_name.ForeColor = System.Drawing.Color.White;
            this.player_name.Location = new System.Drawing.Point(0, 0);
            this.player_name.Name = "player_name";
            this.player_name.Size = new System.Drawing.Size(318, 30);
            this.player_name.TabIndex = 0;
            this.player_name.Text = "[PLAYER NAME]";
            this.player_name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // host_btn
            // 
            this.host_btn.AutoSize = true;
            this.host_btn.BackColor = System.Drawing.Color.Black;
            this.host_btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.host_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.host_btn.ForeColor = System.Drawing.Color.White;
            this.host_btn.Location = new System.Drawing.Point(360, 0);
            this.host_btn.Name = "host_btn";
            this.host_btn.Size = new System.Drawing.Size(135, 30);
            this.host_btn.TabIndex = 60;
            this.host_btn.TabStop = false;
            this.host_btn.Text = "Кикнуть игрока";
            this.host_btn.UseVisualStyleBackColor = false;
            // 
            // PlayerPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.host_btn);
            this.Controls.Add(this.player_name);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(495, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label player_name;
        public System.Windows.Forms.Button host_btn;
    }
}
