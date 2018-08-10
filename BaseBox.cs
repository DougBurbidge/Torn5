using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Torn.UI
{
	/// <summary>
	/// A Torn team box: a list view with a list of players. The box knows the team score and rank.
	/// </summary>
	public partial class BaseBox : UserControl
	{
		public ListView.ListViewItemCollection Items { get { return listView1.Items; } }
		public ImageList Images { get { return listView1.SmallImageList; } set { listView1.SmallImageList = value; } }
		protected ListView ListView { get {return listView1; } }
		public Func<BaseBox> GetMoveTarget { get; set; }
		public Action RankTeams { get; set; }
		public Action SortTeamsByRank { get; set; }
		
		public BaseBox()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		void ListView1SizeChanged(object sender, EventArgs e)
		{
			colPlayer.Width = listView1.Width - colPack.Width - colScore.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth - 4;
		}

		void ListView1ItemDrag(object sender, ItemDragEventArgs e)
		{
			listView1.DoDragDrop(listView1.SelectedItems, DragDropEffects.Move);
		}

		void ListView1DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
				e.Effect = DragDropEffects.Move;
		}

		///<summary>Move selected items from another BaseBox to us.</summary>
		protected virtual void ListView1DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection).ToString(), false))
			{
				BaseBox target = null;

				ListView.SelectedListViewItemCollection items = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
				foreach (ListViewItem item in items)
				{
					target = item.ListView.GetContainerControl() as BaseBox;
					item.Remove();
					listView1.Items.Add(item);
				}
				listView1.SelectedItems.Clear();

				Recalculate();
				target.Recalculate();

				if (RankTeams != null)
					RankTeams();
			}
		}

		///<summary>Move selected items from us to another BaseBox.</summary>
		protected virtual void ListView1DoubleClick(object sender, EventArgs e)
		{
			if (GetMoveTarget == null)
				return;

			BaseBox target = GetMoveTarget();

			if (target == null)
				return;

			foreach (ListViewItem item in listView1.SelectedItems)
			{
				item.Remove();
				target.Items.Add(item);
			}

			target.ListView.SelectedItems.Clear();
			
			Recalculate();
			target.Recalculate();

			if (RankTeams != null)
				RankTeams();
		}

		public virtual void Clear()
		{
			Items.Clear();
		}

		protected virtual void Recalculate(bool guessTeam = true) {}

		///<summary>Move items from another BaseBox to us.</summary>
		public void Accept(List<ListViewItem> items)
		{
			foreach (ListViewItem item in items)
			{
				item.Remove();
				listView1.Items.Add(item);
			}
			listView1.SelectedItems.Clear();

			Recalculate();
		}

		///<summary>Move players from another BaseBox to us.</summary>
		public void Accept(List<ServerPlayer> players)
		{
			foreach (var player in players)
			{
				player.Item.Remove();
				listView1.Items.Add(player.Item);
			}
			listView1.SelectedItems.Clear();

			Recalculate();
		}

		void MenuSortTeamsClick(object sender, EventArgs e)
		{
			if (SortTeamsByRank != null)
				SortTeamsByRank();
		}
		
		public List<ServerPlayer> Players()
		{
			var players = new List<ServerPlayer>();

			foreach (ListViewItem item in Items)
				players.Add((ServerPlayer)item.Tag);

			return players;
		}
	}
}
