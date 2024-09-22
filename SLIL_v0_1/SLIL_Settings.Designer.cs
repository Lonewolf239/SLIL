namespace SLIL.SLIL_v0_1
{
    partial class SLIL_Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.look_speed_text = new System.Windows.Forms.Label();
            this.look_speed = new System.Windows.Forms.TrackBar();
            this.difficulty_text = new System.Windows.Forms.Label();
            this.difficulty_list = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.look_speed)).BeginInit();
            this.SuspendLayout();
            // 
            // look_speed_text
            // 
            this.look_speed_text.AutoSize = true;
            this.look_speed_text.Location = new System.Drawing.Point(3, 21);
            this.look_speed_text.Name = "look_speed_text";
            this.look_speed_text.Size = new System.Drawing.Size(178, 24);
            this.look_speed_text.TabIndex = 0;
            this.look_speed_text.Text = "Чувствительность";
            this.look_speed_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // look_speed
            // 
            this.look_speed.Location = new System.Drawing.Point(187, 12);
            this.look_speed.Maximum = 450;
            this.look_speed.Minimum = 100;
            this.look_speed.Name = "look_speed";
            this.look_speed.Size = new System.Drawing.Size(131, 45);
            this.look_speed.TabIndex = 1;
            this.look_speed.TabStop = false;
            this.look_speed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.look_speed.Value = 175;
            this.look_speed.Scroll += new System.EventHandler(this.Look_speed_Scroll);
            this.look_speed.Enter += new System.EventHandler(this.Look_speed_Enter);
            this.look_speed.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Look_speed_MouseUp);
            // 
            // difficulty_text
            // 
            this.difficulty_text.AutoSize = true;
            this.difficulty_text.Location = new System.Drawing.Point(3, 69);
            this.difficulty_text.Name = "difficulty_text";
            this.difficulty_text.Size = new System.Drawing.Size(111, 24);
            this.difficulty_text.TabIndex = 2;
            this.difficulty_text.Text = "Сложность";
            this.difficulty_text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // difficulty_list
            // 
            this.difficulty_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.difficulty_list.FormattingEnabled = true;
            this.difficulty_list.Items.AddRange(new object[] {
            "Сложно",
            "Нормально",
            "Легко"});
            this.difficulty_list.Location = new System.Drawing.Point(161, 66);
            this.difficulty_list.Name = "difficulty_list";
            this.difficulty_list.Size = new System.Drawing.Size(157, 32);
            this.difficulty_list.TabIndex = 3;
            this.difficulty_list.TabStop = false;
            // 
            // SLIL_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 130);
            this.Controls.Add(this.difficulty_list);
            this.Controls.Add(this.difficulty_text);
            this.Controls.Add(this.look_speed);
            this.Controls.Add(this.look_speed_text);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SLIL_Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SLIL_Settings_FormClosing);
            this.Load += new System.EventHandler(this.SLIL_Settings_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SLIL_Settings_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.look_speed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label look_speed_text;
        private System.Windows.Forms.TrackBar look_speed;
        private System.Windows.Forms.Label difficulty_text;
        private System.Windows.Forms.ComboBox difficulty_list;
    }
}