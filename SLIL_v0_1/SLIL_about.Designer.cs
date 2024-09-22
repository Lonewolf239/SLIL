namespace SLIL.SLIL_v0_1
{
    partial class SLIL_about
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
            this.control_panel = new System.Windows.Forms.Panel();
            this.control_space = new System.Windows.Forms.Label();
            this.control_pic = new System.Windows.Forms.PictureBox();
            this.control_text = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.fruit_panel = new System.Windows.Forms.Panel();
            this.rules_text = new System.Windows.Forms.Label();
            this.control_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.control_pic)).BeginInit();
            this.fruit_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // control_panel
            // 
            this.control_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.control_panel.Controls.Add(this.control_space);
            this.control_panel.Controls.Add(this.control_pic);
            this.control_panel.Controls.Add(this.control_text);
            this.control_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.control_panel.Location = new System.Drawing.Point(0, 0);
            this.control_panel.Margin = new System.Windows.Forms.Padding(2);
            this.control_panel.Name = "control_panel";
            this.control_panel.Size = new System.Drawing.Size(526, 184);
            this.control_panel.TabIndex = 1;
            // 
            // control_space
            // 
            this.control_space.Dock = System.Windows.Forms.DockStyle.Top;
            this.control_space.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.control_space.Location = new System.Drawing.Point(0, 144);
            this.control_space.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.control_space.Name = "control_space";
            this.control_space.Size = new System.Drawing.Size(524, 40);
            this.control_space.TabIndex = 2;
            this.control_space.Text = "Чтобы открыть карту нажмите Space \\ Tab \\ M\r\nДля остановки игры нажмите: ESC";
            this.control_space.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // control_pic
            // 
            this.control_pic.BackColor = System.Drawing.Color.White;
            this.control_pic.Dock = System.Windows.Forms.DockStyle.Top;
            this.control_pic.Image = global::SLIL.Properties.Resources.control;
            this.control_pic.Location = new System.Drawing.Point(0, 20);
            this.control_pic.Margin = new System.Windows.Forms.Padding(2);
            this.control_pic.Name = "control_pic";
            this.control_pic.Size = new System.Drawing.Size(524, 124);
            this.control_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.control_pic.TabIndex = 0;
            this.control_pic.TabStop = false;
            // 
            // control_text
            // 
            this.control_text.Dock = System.Windows.Forms.DockStyle.Top;
            this.control_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.control_text.Location = new System.Drawing.Point(0, 0);
            this.control_text.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.control_text.Name = "control_text";
            this.control_text.Size = new System.Drawing.Size(524, 20);
            this.control_text.TabIndex = 1;
            this.control_text.Text = "Управление:";
            this.control_text.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ok
            // 
            this.ok.AutoSize = true;
            this.ok.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ok.Location = new System.Drawing.Point(419, 241);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(95, 41);
            this.ok.TabIndex = 27;
            this.ok.TabStop = false;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // fruit_panel
            // 
            this.fruit_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fruit_panel.Controls.Add(this.rules_text);
            this.fruit_panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.fruit_panel.Location = new System.Drawing.Point(0, 184);
            this.fruit_panel.Margin = new System.Windows.Forms.Padding(2);
            this.fruit_panel.Name = "fruit_panel";
            this.fruit_panel.Size = new System.Drawing.Size(526, 52);
            this.fruit_panel.TabIndex = 28;
            // 
            // rules_text
            // 
            this.rules_text.AutoEllipsis = true;
            this.rules_text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rules_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rules_text.Location = new System.Drawing.Point(0, 0);
            this.rules_text.Name = "rules_text";
            this.rules_text.Size = new System.Drawing.Size(524, 50);
            this.rules_text.TabIndex = 0;
            this.rules_text.Text = "Цель игры - пройти случайно сгенерированный лабиринт за определённое время.";
            // 
            // SLIL_about
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ok;
            this.ClientSize = new System.Drawing.Size(526, 288);
            this.Controls.Add(this.fruit_panel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.control_panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SLIL_about";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Правила игры";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SLIL_about_Load);
            this.control_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.control_pic)).EndInit();
            this.fruit_panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel control_panel;
        private System.Windows.Forms.Label control_space;
        private System.Windows.Forms.PictureBox control_pic;
        private System.Windows.Forms.Label control_text;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Panel fruit_panel;
        private System.Windows.Forms.Label rules_text;
    }
}