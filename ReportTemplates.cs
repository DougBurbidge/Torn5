using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Torn;

namespace Torn.Report
{
	public enum ReportType { None = 0, TeamLadder, TeamsVsTeams, SoloLadder, GameByGame, GameGrid, Ascension, Pyramid, Packs, Everything };

	/// <summary>Holds details for a single report template -- a team ladder, a solo ladder, etc.</summary>
	public class ReportTemplate
	{
		public ReportType ReportType { get; set; }
		public List<string> Settings { get; private set; }
		public Drops Drops { get; set; }
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }

		public ReportTemplate()
		{
			Settings = new List<string>();
		}

		public ReportTemplate(ReportType reportType, string[] settings): this()
		{
			ReportType = reportType;
			for (int i = 1; i < settings.Length; i++)
				Settings.Add(settings[i].Trim());

			From = ParseDateSetting("from ");
			To = ParseDateSetting("to ");

			int index = Settings.FindIndex(s => s.StartsWith("Drop "));
			if (index > -1)
			{
				Drops = new Drops();
				string drops = Settings[index];
				index = drops.IndexOf(" worst ");
				if (index > -1)
				{
					if (drops.Contains("%"))
						Drops.PercentWorst = double.Parse(drops.Substring(index + " worst ".Length, drops.IndexOf('%', index) - index - " worst ".Length));
					else
						Drops.CountWorst = int.Parse(drops.Substring(index + " worst ".Length, drops.IndexOf(' ', index) - index - " worst ".Length));
				}

				index = drops.IndexOf(" best ");
				if (index > -1)
				{
					if (drops.Contains("%"))
						Drops.PercentBest = double.Parse(drops.Substring(index + " best ".Length, drops.IndexOf('%', index) - index - " best ".Length));
					else
						Drops.CountBest = int.Parse(drops.Substring(index + " best ".Length, drops.IndexOf(' ', index) - index - " best ".Length));
				}
			}
		}

		public string Setting(string name)
		{
			int index = Settings.FindIndex(s => s.StartsWith(name));
			return index > -1 ? Settings[index].Substring(name.Length + 1) : null;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(ReportType.ToString());
			sb.Append(", ");
			sb.Append(string.Join(", ", Settings));
			sb.Append(", ");
			if (Drops != null && Drops.HasDrops())
			{
				sb.Append(", Drop");
				if (Drops.CountWorst > 0)
				{
					sb.Append(" worst ");
					sb.Append(Drops.CountWorst);
				}
				else if (Drops.PercentWorst > 0)
				{
					sb.Append(" worst ");
					sb.Append(Drops.PercentWorst);
					sb.Append('%');
				}
				if (Drops.CountBest > 0)
				{
					sb.Append(" best ");
					sb.Append(Drops.CountBest);
				}
				else if (Drops.PercentBest > 0)
				{
					sb.Append(" best ");
					sb.Append(Drops.PercentBest);
					sb.Append('%');
				}
				sb.Append(" , ");
			}
			if (From.HasValue)
			{
				sb.Append("from ");
				sb.Append(((DateTime)From).ToString("yyyy-MM-dd HH:mm"));
				sb.Append(", ");
			}
			if (To.HasValue)
			{
				sb.Append("to ");
				sb.Append(((DateTime)To).ToString("yyyy-MM-dd HH:mm"));
				sb.Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);
			return sb.ToString();
		}

		public void Validate()
		{
			if (ReportType != ReportType.TeamLadder)
			{
				Settings.Remove("ScaleGames");
				Settings.Remove("ShowColours");
			}
			if (ReportType != ReportType.TeamsVsTeams)
				Settings.Remove("ShowPoints");
			if (ReportType != ReportType.TeamLadder && ReportType != ReportType.SoloLadder)
			{
				Settings.Remove("ShowComments");
				int i = Settings.FindIndex(x => x.StartsWith("TopN", StringComparison.OrdinalIgnoreCase));
				if (i != -1)
					Settings.RemoveAt(i);
				i = Settings.FindIndex(x => x.StartsWith("AtLeastN", StringComparison.OrdinalIgnoreCase));
				if (i != -1)
					Settings.RemoveAt(i);
			}
			if (ReportType != ReportType.TeamLadder && ReportType != ReportType.SoloLadder && ReportType != ReportType.GameGrid)
				Drops = null;
		}

		DateTime? ParseDateSetting(string name)
		{
			int index = Settings.FindIndex(s => s.StartsWith(name));
			if (index > 0)
			{
				DateTime dt;
				DateTime.TryParse(Settings[index].Substring(name.Length), out dt);
				Settings.RemoveAt(index);
				return dt;
			}
			return null;
		}
	}
	
	public class ReportTemplates: List<ReportTemplate>
	{
		public void Parse(string s)
		{
			string[] ss = s.Split('&');
			foreach (string s1 in ss)
			{
				ReportType reportType;
				string[] s2 = s1.Split(',');
				if (s2.Length > 0 && Enum.TryParse(s2[0], out reportType))
					Add(new ReportTemplate(reportType, s2));
			}
		}

		public override string ToString()
		{
			return string.Join("&", this);
		}
	}

	public class Holders: List<Holder>
	{
		public Holder MostRecent()
		{
			//DateTime mostRecent = this.Max(x => x.League.AllGames.MostRecent());
			//return this.Find(x => x.League.AllGames.MostRecent() == mostRecent);

			if (this.Count() == 0)
				return null;

			DateTime? mostRecent = DateTime.MinValue;
			Holder Result = this[0];
			foreach (Holder holder in this)
			{
				DateTime? thisRecent = holder.League.AllGames.MostRecent();
				if (mostRecent < thisRecent)
				{
					mostRecent = thisRecent;
					Result = holder;
				}
			}

			return Result;
		}
	}
		
	/// <summary>
	/// Holds a league and its settings.
	/// </summary>
	[Serializable()]  
	public class Holder
	{
		public string Key { get; set; }  // This is the same as the first column in the list view -- it's the folder we'll bulk-export the league to.
		string fileName;
		public string FileName { 
			get { return fileName; } 
			set 
			{
				fileName = value;
				Watcher.Path = new FileInfo(fileName).Directory.FullName;//Path.GetDirectoryName(fileName);
				Watcher.Filter = Path.GetFileName(fileName);
				Watcher.EnableRaisingEvents = true;
			}
		}

		//[NonSerialized]
		public League League { get; set; }
		public FileSystemWatcher Watcher { get; set; }
		public ReportTemplates ReportTemplates { get; private set; }
		public Fixture Fixture { get; private set; }

		public Holder()
		{
			ReportTemplates = new ReportTemplates();
			
			Watcher = new FileSystemWatcher();
			Watcher.NotifyFilter = NotifyFilters.LastWrite;// | NotifyFilters.CreationTime | NotifyFilters.Size;
			Watcher.Changed += new FileSystemEventHandler(OnFileChanged);
			
			Fixture = new Fixture();
		}
		
		public Holder(string key, string fileName, League league): this()
		{
			Key = key;
			FileName = fileName;
			League = league;

			foreach (LeagueTeam lt in League.Teams)
			{
				FixtureTeam ft = new FixtureTeam();
				ft.Id = Fixture.Teams.Count + 1;
				ft.LeagueTeam = lt;
				Fixture.Teams.Add(ft);
			}
		}

		void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			League.Load(fileName);
		}
	}
}
