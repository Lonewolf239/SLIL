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
            this.close_btn = new System.Windows.Forms.PictureBox();
            this.progress_label = new System.Windows.Forms.Label();
            this.background_progress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.close_btn)).BeginInit();
            this.SuspendLayout();
            // 
            // status_label
            // 
            this.status_label.BackColor = System.Drawing.Color.Transparent;
            this.status_label.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.status_label.Font = new System.Drawing.Font("Comic Sans MS", 18F);
            this.status_label.ForeColor = System.Drawing.Color.White;
            this.status_label.Location = new System.Drawing.Point(93, 165);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(391, 36);
            this.status_label.TabIndex = 0;
            this.status_label.Text = "Check for updates...";
            this.status_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.status_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseDown);
            this.status_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseMove);
            this.status_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseUp);
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
            this.background_progress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseDown);
            this.background_progress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseMove);
            this.background_progress.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseUp);
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(213)))), ((int)(((byte)(248)))));
            this.progress.Dock = System.Windows.Forms.DockStyle.Left;
            this.progress.Location = new System.Drawing.Point(0, 0);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(0, 8);
            this.progress.TabIndex = 0;
            this.progress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseDown);
            this.progress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseMove);
            this.progress.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseUp);
            // 
            // progress_refresh
            // 
            this.progress_refresh.Enabled = true;
            this.progress_refresh.Interval = 10;
            this.progress_refresh.Tick += new System.EventHandler(this.Progress_refresh_Tick);
            // 
            // close_btn
            // 
            this.close_btn.BackColor = System.Drawing.Color.Transparent;
            this.close_btn.Image = global::SLIL.Properties.Resources.close;
            this.close_btn.Location = new System.Drawing.Point(448, 6);
            this.close_btn.Margin = new System.Windows.Forms.Padding(1);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(30, 30);
            this.close_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.close_btn.TabIndex = 2;
            this.close_btn.TabStop = false;
            this.close_btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Close_btn_MouseClick);
            this.close_btn.MouseEnter += new System.EventHandler(this.Close_btn_MouseEnter);
            this.close_btn.MouseLeave += new System.EventHandler(this.Close_btn_MouseLeave);
            // 
            // progress_label
            // 
            this.progress_label.BackColor = System.Drawing.Color.Transparent;
            this.progress_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.progress_label.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.progress_label.ForeColor = System.Drawing.Color.White;
            this.progress_label.Location = new System.Drawing.Point(0, 0);
            this.progress_label.Name = "progress_label";
            this.progress_label.Size = new System.Drawing.Size(93, 201);
            this.progress_label.TabIndex = 3;
            this.progress_label.Text = "100%";
            this.progress_label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.progress_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseDown);
            this.progress_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseMove);
            this.progress_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseUp);
            // 
            // Loading
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::SLIL.Properties.Resources.loading_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(484, 211);
            this.ControlBox = false;
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.progress_label);
            this.Controls.Add(this.background_progress);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Loading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Loading_FormClosing);
            this.Load += new System.EventHandler(this.Loading_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Loading_MouseUp);
            this.background_progress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.close_btn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label status_label;
        private System.Windows.Forms.Panel background_progress;
        private System.Windows.Forms.Panel progress;
        private System.Windows.Forms.Timer progress_refresh;
        private System.Windows.Forms.PictureBox close_btn;
        private System.Windows.Forms.Label progress_label;
    }
}