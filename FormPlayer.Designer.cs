
namespace Torn.UI
{
	partial class FormPlayer
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.listViewPlayers = new System.Windows.Forms.ListView();
			this.colAlias = new System.Windows.Forms.ColumnHeader();
			this.colName = new System.Windows.Forms.ColumnHeader();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.textId = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listViewPlayers
			// 
			this.listViewPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewPlayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.colAlias,
									this.colName});
			this.listViewPlayers.Location = new System.Drawing.Point(12, 12);
			this.listViewPlayers.Name = "listViewPlayers";
			this.listViewPlayers.Size = new System.Drawing.Size(266, 483);
			this.listViewPlayers.TabIndex = 2;
			this.listViewPlayers.UseCompatibleStateImageBehavior = false;
			this.listViewPlayers.View = System.Windows.Forms.View.Details;
			this.listViewPlayers.SelectedIndexChanged += new System.EventHandler(this.ListViewPlayersSelectedIndexChanged);
			this.listViewPlayers.DoubleClick += new System.EventHandler(this.ListViewPlayersDoubleClick);
			// 
			// colAlias
			// 
			this.colAlias.Text = "Alias";
			this.colAlias.Width = 120;
			// 
			// colName
			// 
			this.colName.Text = "Name";
			this.colName.Width = 120;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(122, 527);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(203, 527);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// textSearch
			// 
			this.textSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSearch.Location = new System.Drawing.Point(12, 501);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(120, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextSearchKeyUp);
			// 
			// textId
			// 
			this.textId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textId.Location = new System.Drawing.Point(138, 501);
			this.textId.Name = "textId";
			this.textId.ReadOnly = true;
			this.textId.Size = new System.Drawing.Size(140, 20);
			this.textId.TabIndex = 1;
			// 
			// FormPlayer
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(290, 562);
			this.Controls.Add(this.textId);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listViewPlayers);
			this.Name = "FormPlayer";
			this.Text = "Select Player";
			this.Shown += new System.EventHandler(this.FormPlayerShown);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox textId;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colAlias;
		private System.Windows.Forms.ListView listViewPlayers;
	}
}
