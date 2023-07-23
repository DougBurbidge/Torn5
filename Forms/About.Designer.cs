namespace Torn5.Forms
{
    partial class About
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dougsApps = new System.Windows.Forms.LinkLabel();
            this.github = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tornDownload = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.current = new System.Windows.Forms.Label();
            this.latest = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "A tournament scores editor by AJ Horsman and Doug Burbidge.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dougs Apps";
            // 
            // dougsApps
            // 
            this.dougsApps.AutoSize = true;
            this.dougsApps.Location = new System.Drawing.Point(12, 56);
            this.dougsApps.Name = "dougsApps";
            this.dougsApps.Size = new System.Drawing.Size(187, 13);
            this.dougsApps.TabIndex = 2;
            this.dougsApps.TabStop = true;
            this.dougsApps.Text = "http://www.dougburbidge.com/Apps/";
            this.dougsApps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // github
            // 
            this.github.AutoSize = true;
            this.github.Location = new System.Drawing.Point(12, 97);
            this.github.Name = "github";
            this.github.Size = new System.Drawing.Size(217, 13);
            this.github.TabIndex = 3;
            this.github.TabStop = true;
            this.github.Text = "https://github.com/MrMeeseeks200/Torn5/";
            this.github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.github_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Source Code";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Latest Version";
            // 
            // tornDownload
            // 
            this.tornDownload.AutoSize = true;
            this.tornDownload.Location = new System.Drawing.Point(12, 137);
            this.tornDownload.Name = "tornDownload";
            this.tornDownload.Size = new System.Drawing.Size(172, 13);
            this.tornDownload.TabIndex = 6;
            this.tornDownload.TabStop = true;
            this.tornDownload.Text = "https://gofile.me/71oml/eKEf4elhv";
            this.tornDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tornDownload_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Current Version";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Download Latest Version";
            // 
            // current
            // 
            this.current.AutoSize = true;
            this.current.Location = new System.Drawing.Point(122, 161);
            this.current.Name = "current";
            this.current.Size = new System.Drawing.Size(40, 13);
            this.current.TabIndex = 9;
            this.current.Text = "current";
            // 
            // latest
            // 
            this.latest.AutoSize = true;
            this.latest.Location = new System.Drawing.Point(122, 183);
            this.latest.Name = "latest";
            this.latest.Size = new System.Drawing.Size(32, 13);
            this.latest.TabIndex = 10;
            this.latest.Text = "latest";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 206);
            this.Controls.Add(this.latest);
            this.Controls.Add(this.current);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tornDownload);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.github);
            this.Controls.Add(this.dougsApps);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Torn 5";
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel dougsApps;
        private System.Windows.Forms.LinkLabel github;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel tornDownload;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label current;
        private System.Windows.Forms.Label latest;
    }
}