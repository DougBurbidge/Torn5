using System;
using System.Collections.Generic;
using System.Linq;
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
				if (!Exists(ft => ft.Id == lt.Id))
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
				sb.Remove(sb.Length - 2, 2);
			return sb.ToString();
		}
	}

	public class FixtureTeam
	{
		public LeagueTeam LeagueTeam { get; set; }
		string name;
		public string Name { get { return LeagueTeam == null ? name : LeagueTeam.Name; } set { name = value; } }
		public int Id { get; set; }

		public override string ToString()
		{
			return "FixtureTeam " + Name ?? Id.ToString();
		}
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

		// Import past games from a league.
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

		// This parses a grid. Each game will be a column; each team will be a row. Each character in the grid is a letter
		// representing that team's colour in that game, or is a non-colour character if that team does not play in that game.
		public string[] Parse(string[] lines, FixtureTeams teams, DateTime? firstGame, TimeSpan? duration)
		{
			int minLength = int.MaxValue;

			for (int row = 0; row < lines.Length && row < teams.Count; row++)
			{
				int pos = lines[row].LastIndexOf('\t');
				while (pos > -1)
				{
					lines[row] = lines[row].Remove(pos, 1);
					pos = lines[row].LastIndexOf('\t');
				}
				minLength = Math.Min(minLength, lines[row].Length);
			}

			for (int col = 0; col < minLength; col ++)
			{
				var game = new FixtureGame();
				for (int row = 0; row < lines.Length && row < teams.Count; row++)
				{
					if (lines[row][col] == 'x')
						game.Teams.Add(teams[col], Colour.None);
					else 
					{
						Colour c = ColourExtensions.ToColour(lines[row][col]);
						if (c != Colour.None)
							game.Teams.Add(teams[row], c);
					}
				}
				if (firstGame != null)
				{
					game.Time = (DateTime)firstGame;
					firstGame += duration ?? TimeSpan.Zero;
				}
				Add(game);
			}

			return lines;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (FixtureGame fg in this)
			{
				sb.Append(fg.Time);
				sb.Append('\t');

				for (var i = Colour.Red; i <= Colour.White; i++)
				{
					var ft = fg.Teams.FirstOrDefault(x => x.Value == i).Key;
					if (ft != null)
					{
						sb.Append(ft.Id);
						sb.Append('\t');
					}
				}

				sb.Remove(sb.Length - 1, 1);
				sb.Append("\r\n");
			}
			
			return sb.ToString();
		}

		public string[] ToGrid(FixtureTeams teams)
		{
			int teamsCount = Math.Max(teams.Count, (int)this.Max(fg => fg.Teams.Max(ft => (int?)ft.Key.Id)));
			var lines = new string[teamsCount];

			for (int col = 0; col < Count; col++)
			{
				var fg = this[col];
				for (int row = 0; row < teamsCount; row++)
				{
					if (lines[row] == null) 
						lines[row] = "";

					if (row < teams.Count && fg.Teams.ContainsKey(teams[row]))
					    lines[row] += fg.Teams[teams[row]].ToChar();
					else
						lines[row] += '.';
				}
			}
			return lines;
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
