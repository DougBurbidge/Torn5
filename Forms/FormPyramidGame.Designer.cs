﻿
namespace Torn.UI
{
	partial class FormPyramidGame
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPyramidGame));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelGameTime = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.labelTeamsInGame = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.checkBoxSecret = new System.Windows.Forms.CheckBox();
			this.numericTeamsToTake = new System.Windows.Forms.NumericUpDown();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.radioRound = new System.Windows.Forms.RadioButton();
			this.radioRepechage = new System.Windows.Forms.RadioButton();
			this.buttonClear = new System.Windows.Forms.Button();
			this.radioPlanB = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsToTake)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(517, 39);
			this.label1.TabIndex = 0;
			this.label1.Text = "Set properties for the selected games.\r\n\r\nProperties you set here will control wh" +
    "ich teams from this/these game(s) go into the next round or repêchage.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 74);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Game Time:";
			// 
			// labelGameTime
			// 
			this.labelGameTime.AutoSize = true;
			this.labelGameTime.Location = new System.Drawing.Point(180, 74);
			this.labelGameTime.Name = "labelGameTime";
			this.labelGameTime.Size = new System.Drawing.Size(80, 13);
			this.labelGameTime.TabIndex = 2;
			this.labelGameTime.Text = "labelGameTime";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Description:";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Location = new System.Drawing.Point(180, 97);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(200, 20);
			this.textBoxDescription.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 126);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "# of teams in game";
			// 
			// labelTeamsInGame
			// 
			this.labelTeamsInGame.AutoSize = true;
			this.labelTeamsInGame.Location = new System.Drawing.Point(180, 126);
			this.labelTeamsInGame.Name = "labelTeamsInGame";
			this.labelTeamsInGame.Size = new System.Drawing.Size(98, 13);
			this.labelTeamsInGame.TabIndex = 6;
			this.labelTeamsInGame.Text = "labelTeamsInGame";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 152);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(160, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "# teams to take from each game";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(12, 178);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "Priority";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(12, 247);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(101, 13);
			this.label9.TabIndex = 15;
			this.label9.Text = "Make game secret?";
			// 
			// checkBoxSecret
			// 
			this.checkBoxSecret.AutoSize = true;
			this.checkBoxSecret.Location = new System.Drawing.Point(180, 246);
			this.checkBoxSecret.Name = "checkBoxSecret";
			this.checkBoxSecret.Size = new System.Drawing.Size(57, 17);
			this.checkBoxSecret.TabIndex = 16;
			this.checkBoxSecret.Text = "Secret";
			this.checkBoxSecret.UseVisualStyleBackColor = true;
			// 
			// numericTeamsToTake
			// 
			this.numericTeamsToTake.Location = new System.Drawing.Point(180, 150);
			this.numericTeamsToTake.Name = "numericTeamsToTake";
			this.numericTeamsToTake.Size = new System.Drawing.Size(64, 20);
			this.numericTeamsToTake.TabIndex = 8;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(456, 292);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 18;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOKClick);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(537, 292);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 19;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// radioRound
			// 
			this.radioRound.AutoSize = true;
			this.radioRound.Checked = true;
			this.radioRound.Location = new System.Drawing.Point(180, 176);
			this.radioRound.Name = "radioRound";
			this.radioRound.Size = new System.Drawing.Size(57, 17);
			this.radioRound.TabIndex = 11;
			this.radioRound.TabStop = true;
			this.radioRound.Text = "Round";
			this.radioRound.UseVisualStyleBackColor = true;
			this.radioRound.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// radioRepechage
			// 
			this.radioRepechage.AutoSize = true;
			this.radioRepechage.Location = new System.Drawing.Point(243, 176);
			this.radioRepechage.Name = "radioRepechage";
			this.radioRepechage.Size = new System.Drawing.Size(81, 17);
			this.radioRepechage.TabIndex = 12;
			this.radioRepechage.Text = "Repêchage";
			this.radioRepechage.UseVisualStyleBackColor = true;
			this.radioRepechage.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// buttonClear
			// 
			this.buttonClear.Location = new System.Drawing.Point(250, 150);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(64, 20);
			this.buttonClear.TabIndex = 9;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.ButtonClearClick);
			// 
			// radioPlanB
			// 
			this.radioPlanB.AutoSize = true;
			this.radioPlanB.Location = new System.Drawing.Point(330, 176);
			this.radioPlanB.Name = "radioPlanB";
			this.radioPlanB.Size = new System.Drawing.Size(56, 17);
			this.radioPlanB.TabIndex = 13;
			this.radioPlanB.Text = "Plan B";
			this.radioPlanB.UseVisualStyleBackColor = true;
			this.radioPlanB.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(177, 196);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(416, 39);
			this.label3.TabIndex = 14;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(177, 266);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(290, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "Games marked as Secret will not be shown in public reports.";
			// 
			// FormPyramidGame
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(624, 321);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.radioPlanB);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.radioRepechage);
			this.Controls.Add(this.radioRound);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.numericTeamsToTake);
			this.Controls.Add(this.checkBoxSecret);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.labelTeamsInGame);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelGameTime);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormPyramidGame";
			this.Text = "Pyramid Game(s)";
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsToTake)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelGameTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelTeamsInGame;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox checkBoxSecret;
		private System.Windows.Forms.NumericUpDown numericTeamsToTake;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioRound;
		private System.Windows.Forms.RadioButton radioRepechage;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.RadioButton radioPlanB;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
	}
}