namespace Torn5.Forms
{
    partial class NewVersion
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
            this.tornDownload = new System.Windows.Forms.LinkLabel();
            this.latest = new System.Windows.Forms.Label();
            this.current = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "A new version of Torn is now available!";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Download at the link below";
            // 
            // tornDownload
            // 
            this.tornDownload.AutoSize = true;
            this.tornDownload.Location = new System.Drawing.Point(12, 65);
            this.tornDownload.Name = "tornDownload";
            this.tornDownload.Size = new System.Drawing.Size(215, 13);
            this.tornDownload.TabIndex = 7;
            this.tornDownload.TabStop = true;
            this.tornDownload.Text = "https://torn.lasersports.au/downloads/latest";
            this.tornDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tornDownload_LinkClicked);
            // 
            // latest
            // 
            this.latest.AutoSize = true;
            this.latest.Location = new System.Drawing.Point(122, 111);
            this.latest.Name = "latest";
            this.latest.Size = new System.Drawing.Size(32, 13);
            this.latest.TabIndex = 14;
            this.latest.Text = "latest";
            // 
            // current
            // 
            this.current.AutoSize = true;
            this.current.Location = new System.Drawing.Point(122, 89);
            this.current.Name = "current";
            this.current.Size = new System.Drawing.Size(40, 13);
            this.current.TabIndex = 13;
            this.current.Text = "current";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Current Version";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Latest Version";
            // 
            // NewVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 134);
            this.Controls.Add(this.latest);
            this.Controls.Add(this.current);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tornDownload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "NewVersion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NewVersion";
            this.Load += new System.EventHandler(this.NewVersion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel tornDownload;
        private System.Windows.Forms.Label latest;
        private System.Windows.Forms.Label current;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}