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
            this.start_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // status_label
            // 
            this.status_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.status_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.status_label.Location = new System.Drawing.Point(0, 0);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(334, 61);
            this.status_label.TabIndex = 0;
            this.status_label.Text = "Check for updates...";
            this.status_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.status_label.UseWaitCursor = true;
            // 
            // start_timer
            // 
            this.start_timer.Enabled = true;
            this.start_timer.Interval = 1000;
            this.start_timer.Tick += new System.EventHandler(this.Start_timer_Tick);
            // 
            // Loading
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(334, 61);
            this.ControlBox = false;
            this.Controls.Add(this.status_label);
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
            this.UseWaitCursor = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label status_label;
        private System.Windows.Forms.Timer start_timer;
    }
}