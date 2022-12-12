
namespace Torn5.Controls
{
    partial class PointPercentEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.points = new System.Windows.Forms.NumericUpDown();
            this.percentage = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.points)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentage)).BeginInit();
            this.SuspendLayout();
            // 
            // points
            // 
            this.points.Location = new System.Drawing.Point(3, 2);
            this.points.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.points.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.points.Name = "points";
            this.points.Size = new System.Drawing.Size(46, 20);
            this.points.TabIndex = 40;
            this.points.Tag = "";
            this.points.ValueChanged += new System.EventHandler(this.OnChanged);
            // 
            // percentage
            // 
            this.percentage.Cursor = System.Windows.Forms.Cursors.Default;
            this.percentage.Location = new System.Drawing.Point(77, 2);
            this.percentage.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.percentage.Name = "percentage";
            this.percentage.Size = new System.Drawing.Size(70, 20);
            this.percentage.TabIndex = 41;
            this.percentage.Tag = "";
            this.percentage.ValueChanged += new System.EventHandler(this.OnChanged);
            // 
            // PointPercentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.percentage);
            this.Controls.Add(this.points);
            this.Name = "PointPercentEditor";
            this.Size = new System.Drawing.Size(150, 25);
            ((System.ComponentModel.ISupportInitialize)(this.points)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown points;
        private System.Windows.Forms.NumericUpDown percentage;
    }
}
