using System;
using System.Collections.Generic;
using System.Text;
using Torn;

namespace Torn
{
	/// <summary>
	/// This class models a fixture: a list of games, some or all of which are not yet played.
	/// For each game, it lists the teams involved in that game (and perhaps their colours).
	/// </summary>
	public class Fixture
	{
		public FixtureTeams Teams { get; set; }
		public FixtureGames Games { get; set; }

		public Fixture()
		{
			Teams = new FixtureTeams();
			Games = new FixtureGames();
		}

		public FixtureGame BestMatch(Game game)
		{
			return null;
		}
	}

	public class FixtureTeams: List<FixtureTeam>
	{
		public void Parse(string s, League league)
		{
			string[] lines = s.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < lines.Length; i++)
			{
				FixtureTeam ft = new FixtureTeam();

				ft.Name = lines[i];
				ft.Id = i + 1;
				if (league != null)
					ft.LeagueTeam = league.Teams.Find(x => x.Name == ft.Name);

				Add(ft);
			}
		}

		public void Parse(League league)
		{
			foreach (LeagueTeam lt in league.Teams)
			{
				FixtureTeam ft = new FixtureTeam();

				ft.Name = lt.Name;
				ft.Id = lt.Id;
				ft.LeagueTeam = lt;

				Add(ft);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (FixtureTeam ft in this)
			{
				sb.Append(ft.Name);
				sb.Append("\r\n");
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
	}

	public class FixtureTeam
	{
		public LeagueTeam LeagueTeam { get; set; }
		string name;
		public string Name { get { return LeagueTeam == null ? name : LeagueTeam.Name; } set { name = value; } }
		public int Id { get; set; }
	}

	public class FixtureGames: List<FixtureGame>
	{
		// A game will be a time or date/time, followed by a separator, followed
		// by a separated list of numbers, which are the teams in that game. e.g.:
		// 8:00	1	2	3
		public void Parse(string s, FixtureTeams teams, char separator = '\t')
		{
			string[] lines = s.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				FixtureGame fg = new FixtureGame();
				
				string[] fields = line.Split('\t');
				fg.Time = DateTime.Parse(fields[0]);
				for (int i = 1; i < fields.Length; i++)
					if (!string.IsNullOrEmpty(fields[i]))
					{
						int teamnum = int.Parse(fields[i]);
						FixtureTeam ft = teams.Find(x => x.Id == teamnum);
						if (ft == null)
						{
							ft = new FixtureTeam();
							ft.Id = teamnum;
							ft.Name = "Team " + fields[i];
						}
						if (fields.Length <= 4)  // If there are four or less teams per game,
							fg.Teams.Add(ft, (Colour)i);  // assign colours to teams.
						else
							fg.Teams.Add(ft, Colour.None);
					}

				Add(fg);
			}
		}

		public void Parse(League league, FixtureTeams teams)
		{
			foreach (Game lg in league.Games(false))
			{
				FixtureGame fg = new FixtureGame();

				fg.Time = lg.Time;

				foreach (GameTeam gt in lg.Teams)
				{
					FixtureTeam ft = teams.Find(x => x.LeagueTeam == league.LeagueTeam(gt));
					if (ft != null)
						fg.Teams.Add(ft, gt.Colour);
				}

				Add(fg);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (FixtureGame fg in this)
			{
				sb.Append(fg.Time);
				sb.Append('\t');

				foreach (FixtureTeam ft in fg.Teams.Keys)
				{
					sb.Append(ft.Id);
					sb.Append('\t');
				}

				sb.Remove(sb.Length - 1, 1);
				sb.Append("\r\n");
			}
			
			return sb.ToString();
		}
	}

	public class FixtureGame
	{
		public DateTime Time { get; set; }
		public Dictionary<FixtureTeam, Colour> Teams { get; set; }
		
		public FixtureGame()
		{
			Teams = new Dictionary<FixtureTeam, Colour>();
		}
	}
}
