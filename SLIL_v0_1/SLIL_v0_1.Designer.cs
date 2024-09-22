namespace SLIL.SLIL_v0_1
{
    partial class SLILv0_1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SLILv0_1));
            this.top_panel = new System.Windows.Forms.Panel();
            this.game_over_text = new System.Windows.Forms.Label();
            this.display = new System.Windows.Forms.PictureBox();
            this.status_text = new System.Windows.Forms.Label();
            this.start_btn = new System.Windows.Forms.Button();
            this.raycast = new System.Windows.Forms.Timer(this.components);
            this.time_remein = new System.Windows.Forms.Timer(this.components);
            this.step_sound_timer = new System.Windows.Forms.Timer(this.components);
            this.map_timer = new System.Windows.Forms.Timer(this.components);
            this.show_settings = new System.Windows.Forms.PictureBox();
            this.question = new System.Windows.Forms.Button();
            this.by = new System.Windows.Forms.Label();
            this.developer_name = new System.Windows.Forms.Label();
            this.top_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.display)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.show_settings)).BeginInit();
            this.SuspendLayout();
            // 
            // top_panel
            // 
            this.top_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.top_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.top_panel.Controls.Add(this.game_over_text);
            this.top_panel.Controls.Add(this.display);
            this.top_panel.Controls.Add(this.status_text);
            this.top_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.top_panel.Location = new System.Drawing.Point(0, 0);
            this.top_panel.Name = "top_panel";
            this.top_panel.Size = new System.Drawing.Size(348, 256);
            this.top_panel.TabIndex = 0;
            // 
            // game_over_text
            // 
            this.game_over_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.game_over_text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.game_over_text.Font = new System.Drawing.Font("Consolas", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.game_over_text.ForeColor = System.Drawing.Color.White;
            this.game_over_text.Location = new System.Drawing.Point(0, 24);
            this.game_over_text.Name = "game_over_text";
            this.game_over_text.Size = new System.Drawing.Size(344, 228);
            this.game_over_text.TabIndex = 2;
            this.game_over_text.Text = "GAME OVER";
            this.game_over_text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.game_over_text.Visible = false;
            // 
            // display
            // 
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.Location = new System.Drawing.Point(0, 24);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(344, 228);
            this.display.TabIndex = 0;
            this.display.TabStop = false;
            // 
            // status_text
            // 
            this.status_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(50)))));
            this.status_text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.status_text.Dock = System.Windows.Forms.DockStyle.Top;
            this.status_text.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_text.ForeColor = System.Drawing.Color.White;
            this.status_text.Location = new System.Drawing.Point(0, 0);
            this.status_text.Name = "status_text";
            this.status_text.Size = new System.Drawing.Size(344, 24);
            this.status_text.TabIndex = 1;
            this.status_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // start_btn
            // 
            this.start_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.start_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.start_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.start_btn.Location = new System.Drawing.Point(80, 262);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(105, 40);
            this.start_btn.TabIndex = 36;
            this.start_btn.TabStop = false;
            this.start_btn.Text = "СТАРТ";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.Start_btn_Click);
            // 
            // raycast
            // 
            this.raycast.Interval = 1;
            this.raycast.Tick += new System.EventHandler(this.Raycast_Tick);
            // 
            // time_remein
            // 
            this.time_remein.Interval = 1000;
            this.time_remein.Tick += new System.EventHandler(this.Time_remein_Tick);
            // 
            // step_sound_timer
            // 
            this.step_sound_timer.Interval = 1;
            this.step_sound_timer.Tick += new System.EventHandler(this.Step_sound_timer_Tick);
            // 
            // map_timer
            // 
            this.map_timer.Interval = 1;
            this.map_timer.Tick += new System.EventHandler(this.Map_timer_Tick);
            // 
            // show_settings
            // 
            this.show_settings.Image = global::SLIL.Properties.Resources.setting_btn;
            this.show_settings.Location = new System.Drawing.Point(0, 262);
            this.show_settings.Name = "show_settings";
            this.show_settings.Size = new System.Drawing.Size(40, 40);
            this.show_settings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.show_settings.TabIndex = 38;
            this.show_settings.TabStop = false;
            this.show_settings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Show_settings_MouseClick);
            this.show_settings.MouseEnter += new System.EventHandler(this.Show_settings_MouseEnter);
            this.show_settings.MouseLeave += new System.EventHandler(this.Show_settings_MouseLeave);
            // 
            // question
            // 
            this.question.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.question.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.question.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.question.Location = new System.Drawing.Point(40, 262);
            this.question.Name = "question";
            this.question.Size = new System.Drawing.Size(40, 40);
            this.question.TabIndex = 39;
            this.question.TabStop = false;
            this.question.Text = "?";
            this.question.UseVisualStyleBackColor = true;
            this.question.Click += new System.EventHandler(this.Question_Click);
            // 
            // by
            // 
            this.by.Cursor = System.Windows.Forms.Cursors.Default;
            this.by.Font = new System.Drawing.Font("Lucida Handwriting", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.by.Location = new System.Drawing.Point(237, 260);
            this.by.Name = "by";
            this.by.Size = new System.Drawing.Size(111, 21);
            this.by.TabIndex = 41;
            this.by.Text = "By.";
            this.by.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // developer_name
            // 
            this.developer_name.AutoSize = true;
            this.developer_name.Cursor = System.Windows.Forms.Cursors.Help;
            this.developer_name.Font = new System.Drawing.Font("Lucida Handwriting", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.developer_name.Location = new System.Drawing.Point(214, 281);
            this.developer_name.Name = "developer_name";
            this.developer_name.Size = new System.Drawing.Size(134, 21);
            this.developer_name.TabIndex = 40;
            this.developer_name.Text = "Lonewolf239";
            this.developer_name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.developer_name.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Developer_name_MouseClick);
            // 
            // SLILv0_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 302);
            this.Controls.Add(this.by);
            this.Controls.Add(this.developer_name);
            this.Controls.Add(this.question);
            this.Controls.Add(this.show_settings);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.top_panel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SLILv0_1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLIL v0.1";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.SLIL_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SLIL_FormClosing);
            this.Load += new System.EventHandler(this.SLIL_Load);
            this.LocationChanged += new System.EventHandler(this.SLIL_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SLIL_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SLIL_KeyUp);
            this.top_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.display)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.show_settings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel top_panel;
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.Timer raycast;
        private System.Windows.Forms.PictureBox display;
        private System.Windows.Forms.Label status_text;
        private System.Windows.Forms.Timer time_remein;
        private System.Windows.Forms.Timer step_sound_timer;
        private System.Windows.Forms.Timer map_timer;
        private System.Windows.Forms.PictureBox show_settings;
        private System.Windows.Forms.Button question;
        private System.Windows.Forms.Label by;
        private System.Windows.Forms.Label developer_name;
        private System.Windows.Forms.Label game_over_text;
    }
}