using System;
using System.Collections;
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
		private ListViewColumnSorter sorter;
		public Func<BaseBox> GetMoveTarget { get; set; }
		public Action RankTeams { get; set; }
		public Action SortTeamsByRank { get; set; }

		public BaseBox()
		{
			InitializeComponent();

			sorter = new ListViewColumnSorter();
			this.listView1.ListViewItemSorter = sorter;
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

		void ListView1ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column == sorter.SortColumn)
				SetSort(e.Column, sorter.SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);  // Reverse the current sort direction for this column.
			else
				SetSort(e.Column, SortOrder.Ascending);  // Set the column number that is to be sorted; default to ascending.
		}

		// https://stackoverflow.com/questions/254129/how-to-i-display-a-sort-arrow-in-the-header-of-a-list-view-column-using-c
		protected void SetSort(int column, SortOrder order)
		{
			const string ascendingArrow = " ▲";
			const string descendingArrow = " ▼";

			// Remove existing arrow.
			foreach (ColumnHeader col in ListView.Columns)
				if (col.Text.EndsWith(ascendingArrow) || col.Text.EndsWith(descendingArrow))
					col.Text = col.Text.Substring(0, col.Text.Length - 2);
		
			// Add arrow.
			switch (order)
			{
		 		case SortOrder.Ascending:  ListView.Columns[column].Text += ascendingArrow; break;
				case SortOrder.Descending: ListView.Columns[column].Text += descendingArrow; break;
			}

			sorter.SortColumn = column;
			sorter.SortOrder = order;
			ListView.Sort();
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
				listView1.Sort();

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
			target.ListView.Sort();

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

	// https://support.microsoft.com/en-us/help/319401/how-to-sort-a-listview-control-by-a-column-in-visual-c
	public class ListViewColumnSorter : IComparer
	{
		public int SortColumn { get; set; }
		public SortOrder SortOrder { get; set; }

		private CaseInsensitiveComparer comparer;

		public ListViewColumnSorter()
		{
			SortColumn = 0;
			SortOrder = SortOrder.None;
			comparer = new CaseInsensitiveComparer();
		}

		public int Compare(object x, object y)
		{
			ListViewItem itemX = (ListViewItem)x;
			ListViewItem itemY = (ListViewItem)y;

			int compareResult;
			switch (SortColumn) {
				case 0:  // Pack 
					compareResult = comparer.Compare(((ServerPlayer)itemX.Tag).Colour, ((ServerPlayer)itemY.Tag).Colour);
					if (compareResult == 0)
						compareResult = comparer.Compare(itemX.SubItems[SortColumn].Text, itemY.SubItems[SortColumn].Text);
					break;
				case 2: compareResult = comparer.Compare(int.Parse(itemX.SubItems[SortColumn].Text), int.Parse(itemY.SubItems[SortColumn].Text));  break;  // Score
				default: compareResult = comparer.Compare(itemX.SubItems[SortColumn].Text, itemY.SubItems[SortColumn].Text);  break;  // Player
			}

			return SortOrder == SortOrder.Ascending ? compareResult :
				   SortOrder == SortOrder.Descending ? -compareResult : 0;
		}
	}
}
