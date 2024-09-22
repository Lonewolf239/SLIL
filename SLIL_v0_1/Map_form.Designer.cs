namespace SLIL.SLIL_v0_1
{
    partial class Map_form
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
            this.components = new System.ComponentModel.Container();
            this.map_picturebox = new System.Windows.Forms.PictureBox();
            this.map_refresh = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.map_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // map_picturebox
            // 
            this.map_picturebox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.map_picturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map_picturebox.Location = new System.Drawing.Point(0, 0);
            this.map_picturebox.Name = "map_picturebox";
            this.map_picturebox.Size = new System.Drawing.Size(250, 300);
            this.map_picturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.map_picturebox.TabIndex = 0;
            this.map_picturebox.TabStop = false;
            // 
            // map_refresh
            // 
            this.map_refresh.Interval = 1;
            this.map_refresh.Tick += new System.EventHandler(this.Map_refresh_Tick);
            // 
            // map_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(250, 300);
            this.Controls.Add(this.map_picturebox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "map_form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "map_form";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Map_form_FormClosing);
            this.Load += new System.EventHandler(this.Map_form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.map_picturebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox map_picturebox;
        private System.Windows.Forms.Timer map_refresh;
    }
}