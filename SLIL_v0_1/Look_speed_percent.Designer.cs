namespace SLIL.SLIL_v0_1
{
    partial class Look_speed_percent
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
            this.text = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // text
            // 
            this.text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.text.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.text.Location = new System.Drawing.Point(0, 0);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(47, 26);
            this.text.TabIndex = 0;
            this.text.Text = "4,25";
            this.text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // look_speed_percent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(47, 26);
            this.Controls.Add(this.text);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "look_speed_percent";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "look_speed_percent";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label text;
    }
}