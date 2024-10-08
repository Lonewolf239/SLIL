﻿namespace SLIL
{
    partial class SLIL_Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SLIL_Editor));
            this.editor_interface = new System.Windows.Forms.Panel();
            this.random_btn = new System.Windows.Forms.Button();
            this.reset_btn = new System.Windows.Forms.Button();
            this.export_btn = new System.Windows.Forms.Button();
            this.accept_button = new System.Windows.Forms.Button();
            this.import_btn = new System.Windows.Forms.Button();
            this.question = new System.Windows.Forms.Button();
            this.size_label = new System.Windows.Forms.Label();
            this.height = new System.Windows.Forms.NumericUpDown();
            this.separator = new System.Windows.Forms.Label();
            this.width = new System.Windows.Forms.NumericUpDown();
            this.accept_size_btn = new System.Windows.Forms.Button();
            this.elements_panel = new System.Windows.Forms.Panel();
            this.about = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.width)).BeginInit();
            this.elements_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // editor_interface
            // 
            this.editor_interface.AutoSize = true;
            this.editor_interface.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editor_interface.Location = new System.Drawing.Point(0, 0);
            this.editor_interface.MinimumSize = new System.Drawing.Size(330, 243);
            this.editor_interface.Name = "editor_interface";
            this.editor_interface.Size = new System.Drawing.Size(330, 243);
            this.editor_interface.TabIndex = 0;
            this.editor_interface.MouseEnter += new System.EventHandler(this.SLIL_Editor_MouseEnter);
            // 
            // random_btn
            // 
            this.random_btn.BackgroundImage = global::SLIL.Properties.Resources.random;
            this.random_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.random_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.random_btn.Location = new System.Drawing.Point(112, 249);
            this.random_btn.Name = "random_btn";
            this.random_btn.Size = new System.Drawing.Size(50, 50);
            this.random_btn.TabIndex = 17;
            this.random_btn.TabStop = false;
            this.random_btn.UseVisualStyleBackColor = true;
            this.random_btn.Click += new System.EventHandler(this.Random_btn_Click);
            // 
            // reset_btn
            // 
            this.reset_btn.BackgroundImage = global::SLIL.Properties.Resources.reset;
            this.reset_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.reset_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reset_btn.Location = new System.Drawing.Point(56, 249);
            this.reset_btn.Name = "reset_btn";
            this.reset_btn.Size = new System.Drawing.Size(50, 50);
            this.reset_btn.TabIndex = 16;
            this.reset_btn.TabStop = false;
            this.reset_btn.UseVisualStyleBackColor = true;
            this.reset_btn.Click += new System.EventHandler(this.Reset_btn_Click);
            // 
            // export_btn
            // 
            this.export_btn.BackgroundImage = global::SLIL.Properties.Resources.export;
            this.export_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.export_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.export_btn.Location = new System.Drawing.Point(224, 249);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(50, 50);
            this.export_btn.TabIndex = 12;
            this.export_btn.TabStop = false;
            this.export_btn.UseVisualStyleBackColor = true;
            this.export_btn.Click += new System.EventHandler(this.Export_btn_Click);
            // 
            // accept_button
            // 
            this.accept_button.BackgroundImage = global::SLIL.Properties.Resources.done;
            this.accept_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.accept_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accept_button.Location = new System.Drawing.Point(0, 249);
            this.accept_button.Name = "accept_button";
            this.accept_button.Size = new System.Drawing.Size(50, 50);
            this.accept_button.TabIndex = 13;
            this.accept_button.TabStop = false;
            this.accept_button.UseVisualStyleBackColor = true;
            this.accept_button.Click += new System.EventHandler(this.Accept_button_Click);
            // 
            // import_btn
            // 
            this.import_btn.BackgroundImage = global::SLIL.Properties.Resources.import;
            this.import_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.import_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.import_btn.Location = new System.Drawing.Point(168, 249);
            this.import_btn.Name = "import_btn";
            this.import_btn.Size = new System.Drawing.Size(50, 50);
            this.import_btn.TabIndex = 11;
            this.import_btn.TabStop = false;
            this.import_btn.UseVisualStyleBackColor = true;
            this.import_btn.Click += new System.EventHandler(this.Import_btn_Click);
            // 
            // question
            // 
            this.question.BackgroundImage = global::SLIL.Properties.Resources.question;
            this.question.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.question.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.question.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.question.Location = new System.Drawing.Point(280, 249);
            this.question.Name = "question";
            this.question.Size = new System.Drawing.Size(50, 50);
            this.question.TabIndex = 40;
            this.question.TabStop = false;
            this.question.UseVisualStyleBackColor = true;
            this.question.Click += new System.EventHandler(this.Question_Click);
            // 
            // size_label
            // 
            this.size_label.AutoSize = true;
            this.size_label.Location = new System.Drawing.Point(3, 302);
            this.size_label.Name = "size_label";
            this.size_label.Size = new System.Drawing.Size(129, 24);
            this.size_label.TabIndex = 41;
            this.size_label.Text = "Размер поля:";
            // 
            // height
            // 
            this.height.Location = new System.Drawing.Point(7, 326);
            this.height.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.height.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(50, 29);
            this.height.TabIndex = 42;
            this.height.TabStop = false;
            this.height.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // separator
            // 
            this.separator.AutoSize = true;
            this.separator.Location = new System.Drawing.Point(63, 328);
            this.separator.Name = "separator";
            this.separator.Size = new System.Drawing.Size(20, 24);
            this.separator.TabIndex = 43;
            this.separator.Text = "x";
            // 
            // width
            // 
            this.width.Location = new System.Drawing.Point(89, 326);
            this.width.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.width.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(50, 29);
            this.width.TabIndex = 44;
            this.width.TabStop = false;
            this.width.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // accept_size_btn
            // 
            this.accept_size_btn.AutoSize = true;
            this.accept_size_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.accept_size_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accept_size_btn.Location = new System.Drawing.Point(145, 322);
            this.accept_size_btn.Name = "accept_size_btn";
            this.accept_size_btn.Size = new System.Drawing.Size(99, 36);
            this.accept_size_btn.TabIndex = 45;
            this.accept_size_btn.TabStop = false;
            this.accept_size_btn.Text = "Принять";
            this.accept_size_btn.UseVisualStyleBackColor = true;
            this.accept_size_btn.Click += new System.EventHandler(this.Accept_size_btn_Click);
            // 
            // elements_panel
            // 
            this.elements_panel.AutoScroll = true;
            this.elements_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.elements_panel.Controls.Add(this.about);
            this.elements_panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.elements_panel.Location = new System.Drawing.Point(338, 0);
            this.elements_panel.MinimumSize = new System.Drawing.Size(250, 363);
            this.elements_panel.Name = "elements_panel";
            this.elements_panel.Size = new System.Drawing.Size(250, 363);
            this.elements_panel.TabIndex = 46;
            // 
            // about
            // 
            this.about.Dock = System.Windows.Forms.DockStyle.Top;
            this.about.Location = new System.Drawing.Point(0, 0);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(246, 24);
            this.about.TabIndex = 0;
            this.about.Text = "Элементы:";
            this.about.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SLIL_Editor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(588, 358);
            this.Controls.Add(this.elements_panel);
            this.Controls.Add(this.accept_size_btn);
            this.Controls.Add(this.width);
            this.Controls.Add(this.separator);
            this.Controls.Add(this.height);
            this.Controls.Add(this.size_label);
            this.Controls.Add(this.question);
            this.Controls.Add(this.random_btn);
            this.Controls.Add(this.reset_btn);
            this.Controls.Add(this.export_btn);
            this.Controls.Add(this.accept_button);
            this.Controls.Add(this.import_btn);
            this.Controls.Add(this.editor_interface);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SLIL_Editor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Редактор";
            this.Load += new System.EventHandler(this.SLIL_Editor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SLIL_Editor_KeyDown);
            this.MouseEnter += new System.EventHandler(this.SLIL_Editor_MouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.width)).EndInit();
            this.elements_panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel editor_interface;
        private System.Windows.Forms.Button import_btn;
        private System.Windows.Forms.Button export_btn;
        private System.Windows.Forms.Button accept_button;
        private System.Windows.Forms.Button reset_btn;
        private System.Windows.Forms.Button random_btn;
        private System.Windows.Forms.Button question;
        private System.Windows.Forms.Label size_label;
        private System.Windows.Forms.NumericUpDown height;
        private System.Windows.Forms.Label separator;
        private System.Windows.Forms.NumericUpDown width;
        private System.Windows.Forms.Button accept_size_btn;
        private System.Windows.Forms.Panel elements_panel;
        private System.Windows.Forms.Label about;
    }
}