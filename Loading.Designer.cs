namespace SLIL
{
    partial class Loading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loading));
            this.status_label = new System.Windows.Forms.Label();
            this.background_progress = new System.Windows.Forms.Panel();
            this.progress = new System.Windows.Forms.Panel();
            this.progress_refresh = new System.Windows.Forms.Timer(this.components);
            this.background_progress.SuspendLayout();
            this.SuspendLayout();
            // 
            // status_label
            // 
            this.status_label.BackColor = System.Drawing.Color.Transparent;
            this.status_label.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.status_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.status_label.ForeColor = System.Drawing.Color.White;
            this.status_label.Location = new System.Drawing.Point(0, 171);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(484, 30);
            this.status_label.TabIndex = 0;
            this.status_label.Text = "Check for updates...";
            this.status_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // background_progress
            // 
            this.background_progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(131)))), ((int)(((byte)(182)))));
            this.background_progress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.background_progress.Controls.Add(this.progress);
            this.background_progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.background_progress.Location = new System.Drawing.Point(0, 201);
            this.background_progress.Name = "background_progress";
            this.background_progress.Size = new System.Drawing.Size(484, 10);
            this.background_progress.TabIndex = 1;
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(213)))), ((int)(((byte)(248)))));
            this.progress.Dock = System.Windows.Forms.DockStyle.Left;
            this.progress.Location = new System.Drawing.Point(0, 0);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(0, 8);
            this.progress.TabIndex = 0;
            // 
            // progress_refresh
            // 
            this.progress_refresh.Enabled = true;
            this.progress_refresh.Interval = 10;
            this.progress_refresh.Tick += new System.EventHandler(this.Progress_refresh_Tick);
            // 
            // Loading
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::SLIL.Properties.Resources.loading_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(484, 211);
            this.ControlBox = false;
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.background_progress);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Loading";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading...";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Loading_Load);
            this.background_progress.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label status_label;
        private System.Windows.Forms.Panel background_progress;
        private System.Windows.Forms.Panel progress;
        private System.Windows.Forms.Timer progress_refresh;
    }
}