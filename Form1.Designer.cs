﻿
namespace DICOMScanner
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnTestConvert = new System.Windows.Forms.Button();
            this.tbresult = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnFindPatient = new System.Windows.Forms.Button();
            this.tbSearchName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnTestConvert
            // 
            this.btnTestConvert.Location = new System.Drawing.Point(42, 88);
            this.btnTestConvert.Name = "btnTestConvert";
            this.btnTestConvert.Size = new System.Drawing.Size(109, 23);
            this.btnTestConvert.TabIndex = 0;
            this.btnTestConvert.Text = "Datei Umwandeln";
            this.btnTestConvert.UseVisualStyleBackColor = true;
            this.btnTestConvert.Click += new System.EventHandler(this.btnTestConvert_Click);
            // 
            // tbresult
            // 
            this.tbresult.Location = new System.Drawing.Point(417, 32);
            this.tbresult.Multiline = true;
            this.tbresult.Name = "tbresult";
            this.tbresult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbresult.Size = new System.Drawing.Size(371, 406);
            this.tbresult.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(42, 143);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(351, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnFindPatient
            // 
            this.btnFindPatient.Location = new System.Drawing.Point(269, 88);
            this.btnFindPatient.Name = "btnFindPatient";
            this.btnFindPatient.Size = new System.Drawing.Size(75, 23);
            this.btnFindPatient.TabIndex = 3;
            this.btnFindPatient.Text = "Suche Patient";
            this.btnFindPatient.UseVisualStyleBackColor = true;
            this.btnFindPatient.Click += new System.EventHandler(this.btnFindPatient_Click);
            // 
            // tbSearchName
            // 
            this.tbSearchName.Location = new System.Drawing.Point(269, 62);
            this.tbSearchName.Name = "tbSearchName";
            this.tbSearchName.Size = new System.Drawing.Size(100, 20);
            this.tbSearchName.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbSearchName);
            this.Controls.Add(this.btnFindPatient);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tbresult);
            this.Controls.Add(this.btnTestConvert);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnTestConvert;
        private System.Windows.Forms.TextBox tbresult;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnFindPatient;
        private System.Windows.Forms.TextBox tbSearchName;
    }
}

