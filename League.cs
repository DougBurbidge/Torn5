using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Torn
{
	public enum Colour { None = 0, Red, Blue, Green, Yellow, Purple, Pink, Cyan, Orange, White };
	public static class ColourExtensions
	{
		public static Color ToColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0xA0, 0xA0), Color.FromArgb(0xA0, 0xD0, 0xFF), 
				Color.FromArgb(0xA0, 0xFF, 0xA0), Color.FromArgb(0xFF, 0xFF, 0x90), Color.FromArgb(0xC0, 0xA0, 0xFF), 
				Color.FromArgb(0xFF, 0xA0, 0xF0), Color.FromArgb(0xA0, 0xFF, 0xFF), Color.FromArgb(0xFF, 0xD0, 0xA0), Color.FromArgb(0xFF, 0xFF, 0xFF) };
			return Colors[(int)colour];
		}

		public static Color ToSaturatedColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0x50, 0x50), Color.FromArgb(0x60, 0x80, 0xFF), 
				Color.FromArgb(0x20, 0xFF, 0x20), Color.FromArgb(0xFF, 0xFF, 0x00), Color.FromArgb(0x80, 0x00, 0xFF), 
				Color.FromArgb(0xFF, 0x10, 0xB0), Color.FromArgb(0x00, 0xFF, 0xFF), Color.FromArgb(0xFF, 0x80, 0x50), Color.FromArgb(0xEE, 0xEE, 0xEE) };
			return Colors[(int)colour];
		}

		public static Color ToDarkColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0x40, 0x40), Color.FromArgb(0x40, 0x50, 0xFF), 
				Color.FromArgb(0x00, 0xA0, 0x00), Color.FromArgb(0xA0, 0xA0, 0x00), Color.FromArgb(0x80, 0x00, 0xFF), 
				Color.FromArgb(0xFF, 0x10, 0xB0), Color.FromArgb(0x00, 0xC0, 0xC0), Color.FromArgb(0xFF, 0x60, 0x30), Color.FromArgb(0xDD, 0xDD, 0xDD) };
			return Colors[(int)colour];
		}

		public static Colour ToColour(string s)
		{
			if (string.IsNullOrEmpty(s)) 
				return Colour.None;

			var dict = new Dictionary<string, Colour> { 
				{ "red", Colour.Red }, { "blue", Colour.Blue }, { "green", Colour.Green }, { "yellow", Colour.Yellow },
				{ "purple", Colour.Purple }, { "pink", Colour.Pink }, { "cyan", Colour.Cyan }, { "orange", Colour.Orange }, { "white", Colour.White },
				{ "blu", Colour.Blue }, { "grn", Colour.Green }, { "yel", Colour.Yellow } 
			};

			Colour c;
			dict.TryGetValue(s.ToLower(CultureInfo.InvariantCulture), out c);
			return c;
		}

		public static Colour ToColour(char ch)
		{
			var dict = new Dictionary<char, Colour> { 
				{ 'r', Colour.Red }, { 'b', Colour.Blue }, { 'g', Colour.Green }, { 'y', Colour.Yellow },
				{ 'p', Colour.Purple }, { 'i', Colour.Pink }, { 'c', Colour.Cyan }, { 'o', Colour.Orange }, { 'w', Colour.White }
			};

			Colour c;
			dict.TryGetValue(char.ToLower(ch, CultureInfo.InvariantCulture), out c);
			return c;
		}

		public static char ToChar(this Colour c)
		{
			return "xRBGYPICOW"[(int)c];
		}

		public static Colour ToColour(int i) // Converts from a Laserforce colour index number.
		{
			switch (i)
			{
				case 1: return Colour.Red;
				case 2: return Colour.Green;
				case 3: return Colour.Yellow;
				case 4: return Colour.Blue;
				case 5: return Colour.Cyan;
				case 6: return Colour.Purple;
				case 7: return Colour.White;
				case 8: return Colour.Orange;
				case 9: return Colour.Pink;
				default: return Colour.None;
			}
		}
	}

	public enum HandicapStyle { Percent, Plus, Minus };
	public static class HandicapExtensions
	{
		public static HandicapStyle ToHandicapStyle(string s)
		{
		    var dict = new Dictionary<string, HandicapStyle> { 
				{ "%", HandicapStyle.Percent }, { "+", HandicapStyle.Plus }, { "-", HandicapStyle.Minus }
			};

			HandicapStyle h;
			dict.TryGetValue(s.ToLower(CultureInfo.InvariantCulture), out h);
			return h;
		}
		
		public static string ToString(this HandicapStyle handicapStyle)
		{
			switch (handicapStyle) {
				case HandicapStyle.Percent: return "%";
				case HandicapStyle.Plus: return "+";
				case HandicapStyle.Minus: return "-";
			}
			return "";
		}
	}

	public enum GroupPlayersBy { Alias, Colour, Lotr };

	public class Handicap
	{
		public double? Value { get; set; }
		public HandicapStyle Style { get; set; }

		public Handicap() {}

		public Handicap(double? value, HandicapStyle style)
		{
			Value = value;
			Style = style;
		}

		public double Apply(double score)
		{
			return Value == null ? score :
				Style == HandicapStyle.Percent ? score * (double)Value / 100 :
				Style == HandicapStyle.Plus ?    score + (double)Value :
				                                 score - (double)Value;
		}

		/// <summary>Parse strings like "110%", "+1000", "-1000", "110" into handicap value and style.</summary>
		public static Handicap Parse(string s)
		{
			var h = new Handicap();

			if (!string.IsNullOrEmpty(s))
			{
				if (s[s.Length - 1] == '%')
				{
					h.Value = int.Parse(s.Substring(0, s.Length - 1), CultureInfo.InvariantCulture);
					h.Style = HandicapStyle.Percent;
				}
				else if (s[0] == '+')
				{
					h.Value = int.Parse(s.Substring(1), CultureInfo.InvariantCulture);
					h.Style = HandicapStyle.Plus;
				}
				else if (s[0] == '-')
				{
					h.Value = int.Parse(s.Substring(1), CultureInfo.InvariantCulture);
					h.Style = HandicapStyle.Minus;
				}
				else  // No style specified -- assume '%'.
				{
					h.Value = int.Parse(s, CultureInfo.InvariantCulture);
					h.Style = HandicapStyle.Percent;
				}
			}

			return h;
		}

		public override string ToString()
		{
			return Value == null ? "" :
				(Style == HandicapStyle.Percent) ? ((double)Value).ToString(CultureInfo.InvariantCulture) + '%' :
				Style.ToString() + ((double)Value).ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>True if the handicap is 100%, +0 or -0.</summary>
		public bool IsZero()
		{
			return Value == null || (Style == HandicapStyle.Percent && Value == 100) || (Style != HandicapStyle.Percent && Value == 0);
		}
	}

	public class LeaguePlayer
	{
		/// <summary>Friendly name e.g. "RONiN".</summary>
		public string Name { get; set; }
		/// <summary>Under-the-hood laser game system identifier e.g. "P11-JP9", "1-50-50", etc.</summary>
		public string Id { get; set; }  // 
		/// <summary>Represents player handicap +/-/%.</summary>
		public Handicap Handicap { get; set; }
		/// <summary>User-defined. Often used for player grade.</summary>
		public string Comment { get; set; }  // 
	
		Collection<GamePlayer> played;
		/// <summary>Games this player has played in. Set by LinkThings().</summary>
		public Collection<GamePlayer> Played { get { return played; } }

		public LeaguePlayer() 
		{
			played = new Collection<GamePlayer>();
		}

		public LeaguePlayer Clone()
		{
			LeaguePlayer clone = new LeaguePlayer();
			clone.Name = Name;
			clone.Id = Id;
			clone.Handicap = Handicap;
			clone.Comment = Comment;

			clone.played = new Collection<GamePlayer>(played);

			return clone;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>Stores data about each remembered league team</summary>
	public class LeagueTeam: IComparable
	{
		string name;
		public string Name 
		{
			get { 
				if (string.IsNullOrEmpty(name))
				{
					if (Players.Count == 0)
						name = "Team " + Id.ToString(CultureInfo.InvariantCulture);
					else if (Players.Count == 2)
						name = Players[0].Name + " and " + Players[1].Name;
					else if (Players.Count == 1)
						name = Players[0].Name;
					else
						name = Players[0].Name + "'s team";
				}
				return name;
			}
			
			set { name = value; } 
		}

		internal int Id  { get; set; }
		public Handicap Handicap { get; set; }
		public string Comment { get; set; }
		public List<LeaguePlayer> Players { get; private set; }
		public List<GameTeam> AllPlayed { get; private set; }

		public LeagueTeam()
		{
			Players = new List<LeaguePlayer>();
			AllPlayed = new List<GameTeam>();
		}

		public LeagueTeam Clone()
		{
			var clone = new LeagueTeam();
			clone.Name = Name;
			clone.Id = Id;
			clone.Handicap = Handicap;
			clone.Comment = Comment;
			
			clone.Players = new List<LeaguePlayer>(Players);
			//clone.AllPlayed = new List<GameTeam>(AllPlayed);
			
			return clone;
		}

		int IComparable.CompareTo(object obj)
		{
			return string.Compare(this.Name, ((LeagueTeam)obj).Name);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class GameTeam: IComparable
	{
		public int? TeamId { get; set; }
		Colour colour;
		public Colour Colour
		{
			get
			{
				if (colour == Colour.None)
				{
					var counts = new int[Enum.GetValues(typeof(Colour)).Cast<int>().Max() + 1];
					foreach (var player in players)
						if (0 <= (int)player.Colour && (int)player.Colour < counts.Length)
							counts[(int)player.Colour]++;

					colour = (Colour)counts.ToList().IndexOf(counts.Max());
				}
				return colour;
			}
			set { colour = value; }
		}
		public int Score { get; set; }
		public int Adjustment { get; set; }
		public double Points { get; set; }
		public double PointsAdjustment { get; set; }

		List<GamePlayer> players;
		public List<GamePlayer> Players { get { return players; } }

		public GameTeam()
		{
			players = new List<GamePlayer>();
		}

		public GameTeam Clone()
		{
			var clone = new GameTeam();
			clone.TeamId = TeamId;
			clone.Colour = colour;
			clone.Score = Score;
			clone.Adjustment = Adjustment;
			clone.Points = Points;
			clone.PointsAdjustment = PointsAdjustment;
			// Don't clone players as this will be done by LinkThings().

			return clone;
		}

		int IComparable.CompareTo(object obj)
		{
			GameTeam gt = (GameTeam)obj;
			if (this.Points == gt.Points)
				return gt.Score - this.Score;
			else
				return Math.Sign(gt.Points - this.Points);
		}

		public override string ToString()
		{
			return "GameTeam " + (TeamId ?? -1).ToString();
		}
	}

	public class GamePlayer: IComparable
	{
		public int GameTeamId { get; set; }
		/// <summary>Under-the-hood laser game system identifier e.g. "P11-JP9", "1-50-50", etc. Same as LeaguePlayer.Id.</summary>
		public string PlayerId { get; set; }
		public string Pack { get; set; }
		public int Score { get; set; }
		public uint Rank { get; set; }
		public Colour Colour { get; set; }
		public int HitsBy { get; set; }
		public int HitsOn { get; set; }
		public int BaseHits { get; set; }
		public int BaseDestroys { get; set; }
		public int BaseDenies { get; set; }
		public int BaseDenied { get; set; }
		public int YellowCards { get; set; }
		public int RedCards { get; set; }

		/// <summary>Used for accumulating data to be used for a team total, a game average, etc.</summary>
		public void Add(GamePlayer source)
		{
			Score += source.Score;
			Rank += source.Rank;
			HitsBy += source.HitsBy;
			HitsOn += source.HitsOn;
			BaseHits += source.BaseHits;
			BaseDestroys += source.BaseDestroys;
			BaseDenies += source.BaseDenies;
			BaseDenied += source.BaseDenied;
			YellowCards += source.YellowCards;
			RedCards += source.RedCards;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((GamePlayer)obj).Score - this.Score;
		}

		public GamePlayer CopyTo(GamePlayer target)
		{
			target.GameTeamId = GameTeamId;
			target.PlayerId = PlayerId;
			target.Pack = Pack;
			target.Score = Score;
			target.Rank = Rank;
			target.Colour = Colour;
			target.HitsBy = HitsBy;
			target.HitsOn = HitsOn;
			target.BaseHits = BaseHits;
			target.BaseDestroys = BaseDestroys;
			target.BaseDenies = BaseDenies;
			target.BaseDenied = BaseDenied;
			target.YellowCards = YellowCards;
			target.RedCards = RedCards;

			return target;
		}

		/// <summary>Used for averaging data for a team, a game, etc.</summary>
		public void DivideBy(int divisor)
		{
			Score /= divisor;
			Rank /= (uint)divisor;
			HitsBy /= divisor;
			HitsOn /= divisor;
			BaseHits /= divisor;
			BaseDestroys /= divisor;
			BaseDenies /= divisor;
			BaseDenied /= divisor;
			YellowCards /= divisor;
			RedCards /= divisor;
		}

		public GameTeam GameTeam(League league)
		{
			foreach (var game in league.AllGames)
				foreach (var gameTeam in game.Teams)
					if (gameTeam.Players.Contains(this))
						return gameTeam;
			
			return null;
		}

		public override string ToString()
		{
			return "GamePlayer " + PlayerId;
		}
	}

	public class Game: IComparable
	{
		public string Title { get; set; }
		public DateTime Time { get; set; }
		public bool Secret { get; set; }  // If true, don't serve this game from our internal webserver, or include it in any webserver reports.
		public List<GameTeam> Teams { get; private set; }
		public List<GamePlayer> Players { get; private set; }
		public ServerGame ServerGame {get; set; }

		int? hits = null;
		public int Hits { get 
			{
				if (hits == null)
					hits = Players.Sum(p => p.HitsOn);

				return (int)hits;
			} 
		}

		public Game()
		{
			Teams = new List<GameTeam>();
			Players = new List<GamePlayer>();
		}

		int IComparable.CompareTo(object obj)
		{
		   Game g = (Game)obj;
		   return DateTime.Compare(this.Time, g.Time);
		}

		/// <summary>True if any team in this game has victory points.</summary>
		public bool IsPoints()
		{
			foreach (GameTeam gameTeam in Teams)
				if (gameTeam.Points != 0)
					return true;
			
			return false;
		}

		public string LongTitle()
		{
			string timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Replace(":ss", "");

			return (string.IsNullOrEmpty(Title) ? "Game " : Title + " Game ") + Time.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + timeFormat);
		}

		public int Rank(string playerId)
		{
			return Players.FindIndex(x => x.PlayerId == playerId) + 1;
		}

		public override string ToString()
		{
			return Secret ? string.Join(", ", this.Teams.OrderBy(x => x.ToString()).Select(x => x.ToString())) :
				string.Join(", ", this.Teams.Select(x => x.ToString()));
		}

		int? totalScore = null;
		public int TotalScore()
		{
			if (totalScore == null)
				totalScore = Teams.Sum(t => t.Score);

			return (int)totalScore;
		}
	}

	public class Games: List<Game>
	{
		public DateTime? MostRecent()
		{
			return (this.Count == 0) ? (DateTime?)null : this.Select(x => x.Time).Max();
		}
	}

	public class GameTeamData
	{
		public GameTeam GameTeam { get; set; }
		public List<ServerPlayer> Players { get; set; }
	}

	/// <summary>Load and manage a .Torn league file, containing league teams and games.</summary>
	public class League
	{
		public string Title { get; set; }
		string file;

		public int GridHigh { get; set; }
		public int GridWide { get; set; }
		public int GridPlayers { get; set; }
		int sortMode, sortByRank, autoUpdate, updateTeams, elimMultiplier;
		public HandicapStyle HandicapStyle { get; set; }

		List<LeagueTeam> teams;
		public List<LeagueTeam> Teams { get { return teams; } }

		List<LeaguePlayer> players;
		public List<LeaguePlayer> Players { get { return players; } }

		public Games AllGames { get; private set; }

		Collection<double> victoryPoints;
		public Collection<double> VictoryPoints { get { return victoryPoints; } }
		
		public double VictoryPointsHighScore { get; set; }
		public double VictoryPointsProportional { get; set; }

		public League()
		{
			teams = new List<LeagueTeam>();
			players = new List<LeaguePlayer>();
			AllGames = new Games();
			victoryPoints = new Collection<double>();
			
			GridHigh = 3;
			GridWide = 1;
			GridPlayers = 6;
			HandicapStyle = HandicapStyle.Percent;
		}

		public League(string fileName): this()
		{
			Clear();
			file = fileName;
			Title = Path.GetFileNameWithoutExtension(fileName).Replace('_', ' ');
		}

		void Clear()
		{
			Title = "";
			teams.Clear();
			players.Clear();
			AllGames.Clear();
			victoryPoints.Clear();
			VictoryPointsHighScore = 0;
		}

		/// <summary>This is a deep clone of league teams, league players and victory points, but a shallow clone of game data: Game, GameTeam, GamePlayer.</summary>
		public League Clone()
		{
			League clone = new League();

			clone.Title = Title;
			clone.file = file;

			clone.GridHigh = GridHigh;
			clone.GridWide = GridWide;
			clone.GridPlayers = GridPlayers;

			clone.teams = Teams.Select(item => (LeagueTeam)item.Clone()).ToList();
			clone.players = Players.Select(item => (LeaguePlayer)item.Clone()).ToList();
			clone.AllGames.AddRange(AllGames);

			clone.victoryPoints = new Collection<double>(VictoryPoints.Select(item => item).ToList());
			clone.VictoryPointsHighScore = VictoryPointsHighScore;
			clone.VictoryPointsProportional = VictoryPointsProportional;

			clone.LinkThings();

			return clone;
		}

		void LinkTeamToGame(GameTeamData teamData, ServerGame serverGame)
		{
			if (teamData.GameTeam == null)
				teamData.GameTeam = new GameTeam();

			var gameTeam = teamData.GameTeam;
			Game game = serverGame.Game ?? Game(gameTeam);
			if (game == null)
			{
				game = new Game();
				game.Time = serverGame.Time;
				serverGame.Game = game;
				serverGame.Game.ServerGame = serverGame;
			}

			game.Teams.Add(gameTeam);

			LeagueTeam leagueTeam = LeagueTeam(teamData.GameTeam);

			if (leagueTeam == null)
				leagueTeam = GuessTeam(teamData.Players.Select(x => x.PlayerId).ToList());
			
			if (leagueTeam == null)
			{
				leagueTeam = new LeagueTeam();
				leagueTeam.Id = NextTeamID();
				Teams.Add(leagueTeam);
			}
			
			gameTeam.TeamId = leagueTeam.Id;

			leagueTeam.AllPlayed.RemoveAll(x => Game(x) != null && Game(x).Time == game.Time);
			leagueTeam.AllPlayed.Add(gameTeam);
			
			gameTeam.Players.Clear();
			gameTeam.Players.AddRange(teamData.Players);
		}

		void LinkPlayerToGame(GamePlayer gamePlayer, GameTeam gameTeam, ServerGame game)
		{
			gamePlayer.GameTeamId = (int)gameTeam.TeamId;

			if (!game.Game.Players.Contains(gamePlayer))
				game.Game.Players.Add(gamePlayer);

			var leaguePlayer = LeaguePlayer(gamePlayer);

			if (leaguePlayer == null)
				leaguePlayer = Players.Find(p => p.Id == gamePlayer.PlayerId);

			if (leaguePlayer == null)
			{
				leaguePlayer = new LeaguePlayer();
				if (gamePlayer is ServerPlayer)
					leaguePlayer.Name = ((ServerPlayer)gamePlayer).Alias;
				leaguePlayer.Id = gamePlayer.PlayerId;
				Players.Add(leaguePlayer);
			}

			if (!LeagueTeam(gameTeam).Players.Contains(leaguePlayer))
				LeagueTeam(gameTeam).Players.Add(leaguePlayer);

			if (!leaguePlayer.Played.Contains(gamePlayer))
				leaguePlayer.Played.Add(gamePlayer);
		}

		public void CommitGame(ServerGame serverGame, List<GameTeamData> teamDatas, GroupPlayersBy groupPlayersBy)
		{
			if (serverGame.Game == null)
			{
				serverGame.Game = new Game();
				serverGame.Game.Time = serverGame.Time;
				serverGame.Game.ServerGame = serverGame;
			}

			serverGame.Game.Teams.Clear();
			foreach (var teamData in teamDatas)
			{
				LinkTeamToGame(teamData, serverGame);

				foreach (var player in teamData.Players)
					LinkPlayerToGame(serverGame.Game.Players.Find(gp => gp.PlayerId == player.PlayerId) ?? player, //.CopyTo(new GamePlayer()),
					                 teamData.GameTeam, serverGame);

				teamData.GameTeam.Players.Sort();
				teamData.GameTeam.Score = (int)CalculateScore(teamData.GameTeam);
			}

			serverGame.Game.Players.Sort();
			for (int i = 0; i < serverGame.Game.Players.Count; i++)
				serverGame.Game.Players[i].Rank = (uint)i + 1;

			if (!AllGames.Contains(serverGame.Game))
			{
				AllGames.Add(serverGame.Game);
				AllGames.Sort();
			}

			serverGame.Game.Teams.Sort();
			serverGame.League = this;

			foreach (var teamData in teamDatas)
				teamData.GameTeam.Points = CalculatePoints(teamData.GameTeam, groupPlayersBy);

			Save();
		}

		public Games Games(bool includeSecret)
		{
			if (includeSecret) 
				return AllGames;

			var publicGames = new Games();
			publicGames.AddRange(AllGames.Where(g => !g.Secret).ToList());
			return publicGames;
		}
		
		public List<LeagueTeam> GuessTeams(ServerGame game)
		{
			List<LeagueTeam> result = new List<LeagueTeam>();

			if (game.Players == null)
				return result;

			var teams = game.Players.Select(x => x.PandCPlayerTeamId).Distinct();

			foreach (int teamId in teams)
				result.Add(GuessTeam(game.Players.FindAll(x => x.PandCPlayerTeamId == teamId).Select(y => y.PlayerId).ToList()));

			return result;
		}

		public LeagueTeam GuessTeam(List<string> ids)
		{
			if (teams.Count == 0 || ids.Count == 0)
				return null;

			LeagueTeam bestTeam = null;
			double bestScore = 0;

			foreach (LeagueTeam team in teams)
				if (team.Players.Count > 0)
				{
					double thisScore = 1.0 * team.Players.FindAll(p => ids.Contains(p.Id)).Count / team.Players.Count;

					if (bestScore < thisScore)
					{
						bestScore = thisScore;
						bestTeam = team;
					}
				}

			return bestTeam;
		}

		/// <summary>True if any game in this league has victory points.</summary>
		public bool IsPoints()
		{
			return IsPoints(AllGames);
		}

		/// <summary>True if any game in the list has victory points.</summary>
		public bool IsPoints(List<Game> games)
		{
			foreach (Game game in games)
				if (game.IsPoints())
					return true;

			return false;
		}

		int NextTeamID()
		{
			return teams.Count == 0 ? 0 : teams.Max(x => x.Id) + 1;
		}

		/// <summary>Load a .Torn file from disk.</summary>
		public void Load(string fileName)
		{
			Clear();

			var doc = new XmlDocument();
			doc.Load(fileName);

			var root = doc.DocumentElement;

			Title = root.GetString("Title");
			if (string.IsNullOrEmpty(Title))
				Title = Path.GetFileNameWithoutExtension(fileName).Replace('_', ' ');

			GridHigh = root.GetInt("GridHigh");
			GridWide = root.GetInt("GridWide");
			GridPlayers = root.GetInt("GridPlayers");
			sortMode = root.GetInt("SortMode");
			string s = root.GetString("HandicapStyle");
			HandicapStyle = s == "%" ? HandicapStyle.Percent : s == "+" ? HandicapStyle.Plus : HandicapStyle.Minus;
			sortByRank = root.GetInt("SortByRank");
			autoUpdate = root.GetInt("AutoUpdate");
			updateTeams = root.GetInt("UpdateTeams");
			elimMultiplier = root.GetInt("ElimMultiplier");

			XmlNodeList points = root.SelectNodes("Points");
			foreach (XmlNode point in points)
				victoryPoints.Add(double.Parse(point.InnerText, CultureInfo.InvariantCulture));

			VictoryPointsHighScore = root.GetDouble("High");
			VictoryPointsProportional = root.GetDouble("Proportional");

			XmlNodeList xteams = root.SelectSingleNode("leaguelist").SelectNodes("team");

			foreach (XmlNode xteam in xteams)
			{
				LeagueTeam leagueTeam = new LeagueTeam();
				
				leagueTeam.Name = xteam.GetString("teamname");
				leagueTeam.Id = xteam.GetInt("teamid");
				leagueTeam.Handicap = Handicap.Parse(xteam.GetString("handicap"));
				leagueTeam.Comment = xteam.GetString("comment");
				
				var teamPlayers = xteam.SelectSingleNode("players");
				if (teamPlayers != null)
				{
					XmlNodeList xplayers = teamPlayers.SelectNodes("player");
					
					foreach (XmlNode xplayer in xplayers)
					{
						LeaguePlayer leaguePlayer;
						string id = xplayer.GetString("buttonid");;
						
						leaguePlayer = players.Find(x => x.Id == id);
						if (leaguePlayer == null)
						{
							leaguePlayer = new LeaguePlayer();
							players.Add(leaguePlayer);
							leaguePlayer.Id = id;
						}

						leaguePlayer.Name = xplayer.GetString("name");
						leaguePlayer.Handicap = Handicap.Parse(xteam.GetString("handicap"));
						leaguePlayer.Comment = xplayer.GetString("comment");

						if (!leagueTeam.Players.Exists(x => x.Id == id))
							leagueTeam.Players.Add(leaguePlayer);
					}
				}

				teams.Add(leagueTeam);
			}

			XmlNodeList xgames = root.SelectSingleNode("games").SelectNodes("game");

			foreach (XmlNode xgame in xgames) 
			{
				Game game = new Game();

				game.Title = xgame.GetString("title");
				game.Secret = xgame.GetString("secret") == "true";

				var child = xgame.SelectSingleNode("ansigametime");
				if (child == null)
					game.Time = new DateTime(1899, 12, 30).AddDays(double.Parse(xgame.GetString("gametime"), CultureInfo.InvariantCulture));
				else
					game.Time = DateTime.Parse(child.InnerText, CultureInfo.InvariantCulture);
				//game.Hits = XmlInt(xgame, "hits");

				xteams = xgame.SelectSingleNode("teams").SelectNodes("team");

				foreach (XmlNode xteam in xteams)
				{
					GameTeam gameTeam = new GameTeam();

					gameTeam.TeamId = xteam.GetInt("teamid");
					gameTeam.Colour = ColourExtensions.ToColour(xteam.GetString("colour"));
					gameTeam.Score = xteam.GetInt("score");
					gameTeam.Points = xteam.GetInt("points");
					gameTeam.Adjustment = xteam.GetInt("adjustment");
					gameTeam.PointsAdjustment = xteam.GetInt("victorypointsadjustment");

					game.Teams.Add(gameTeam);
				}
				game.Teams.Sort();

				XmlNodeList xplayers = xgame.SelectSingleNode("players").SelectNodes("player");

				foreach (XmlNode xplayer in xplayers)
				{
					GamePlayer gamePlayer = new GamePlayer();

					gamePlayer.PlayerId = xplayer.GetString("playerid");
					gamePlayer.GameTeamId = xplayer.GetInt("teamid");
					gamePlayer.Pack = xplayer.GetString("pack");
					gamePlayer.Score = xplayer.GetInt("score");
					gamePlayer.Rank = (uint)xplayer.GetInt("rank");
					gamePlayer.HitsBy = xplayer.GetInt("hitsby");
					gamePlayer.HitsOn = xplayer.GetInt("hitson");
					gamePlayer.BaseHits = xplayer.GetInt("basehits");
					gamePlayer.BaseDestroys = xplayer.GetInt("basedestroys");
					gamePlayer.BaseDenies = xplayer.GetInt("basedenies");
					gamePlayer.BaseDenied = xplayer.GetInt("basedenied");
					gamePlayer.YellowCards = xplayer.GetInt("yellowcards");
					gamePlayer.RedCards = xplayer.GetInt("redcards");

					if (xplayer.SelectSingleNode("colour") != null)
						gamePlayer.Colour = (Colour)(xplayer.GetInt("colour"));

					game.Players.Add(gamePlayer);
				}
				game.Players.Sort();
				
				AllGames.Add(game);
			}

			AllGames.Sort();
			LinkThings();

			file = fileName;
		}

		/// <summary>
		/// If a league has just been loaded, it has a bunch of "links" present only as ID fields.
		/// LinkThings() matches these, and populates links and lists accordingly. 
		/// </summary>
		void LinkThings()
		{
			teams.Sort();
			foreach (var leagueTeam in teams)
			{
				leagueTeam.AllPlayed.Clear();
				
				foreach (var player in leagueTeam.Players)
					player.Played.Clear();
			}

			foreach (Game game in AllGames)
			{
				foreach (GameTeam gameTeam in game.Teams) 
				{
					// Connect each game team back to their league team.
					var leagueTeam = teams.Find(x => x.Id == gameTeam.TeamId);
					if (leagueTeam != null)
					{
						leagueTeam.AllPlayed.Add(gameTeam);

						// Connect each game player to their game team.
						gameTeam.Players.Clear();
						foreach (GamePlayer gamePlayer in game.Players)
							if (gamePlayer.GameTeamId == gameTeam.TeamId)
								gameTeam.Players.Add(gamePlayer);

						// Connect each game player to their league player.
						foreach (GamePlayer gamePlayer in gameTeam.Players)
							foreach (LeaguePlayer leaguePlayer in LeagueTeam(gameTeam).Players)
								if (gamePlayer.PlayerId == leaguePlayer.Id)
									leaguePlayer.Played.Add(gamePlayer);
					}
				}
			}
		}

		/// <summary>Save a .Torn file to disk.</summary>
		public void Save(string fileName = "")
		{
			if (!string.IsNullOrEmpty(fileName))
				file = fileName;

			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);

			XmlNode bodyNode = doc.CreateElement("body");
			doc.AppendChild(bodyNode);

			doc.AppendNode(bodyNode, "Title", Title);
			doc.AppendNode(bodyNode, "GridHigh", GridHigh);
			doc.AppendNode(bodyNode, "GridWide", GridWide);
			doc.AppendNode(bodyNode, "GridPlayers", GridPlayers);
			doc.AppendNode(bodyNode, "SortMode", sortMode);
			doc.AppendNode(bodyNode, "HandicapStyle", HandicapStyle.ToString());
			doc.AppendNonZero(bodyNode, "SortByRank", sortByRank);
			doc.AppendNode(bodyNode, "AutoUpdate", autoUpdate);
			doc.AppendNode(bodyNode, "UpdateTeams", updateTeams);
			doc.AppendNonZero(bodyNode, "ElimMultiplier", elimMultiplier);

			foreach (double point in victoryPoints)
				doc.AppendNode(bodyNode, "Points", point.ToString());

			if (VictoryPointsHighScore != 0) doc.AppendNode(bodyNode, "High", VictoryPointsHighScore.ToString());
			if (VictoryPointsProportional != 0) doc.AppendNode(bodyNode, "Proportional", VictoryPointsProportional.ToString());

			XmlNode leagueTeamsNode = doc.CreateElement("leaguelist");
			bodyNode.AppendChild(leagueTeamsNode);

			foreach (var team in teams)
			{
				XmlNode teamNode = doc.CreateElement("team");
				leagueTeamsNode.AppendChild(teamNode);

				doc.AppendNode(teamNode, "teamname", team.Name);
				doc.AppendNode(teamNode, "teamid", team.Id);
				if (team.Handicap != null && !team.Handicap.IsZero())
					doc.AppendNode(teamNode, "handicap", team.Handicap.ToString());
				if (!string.IsNullOrEmpty(team.Comment)) doc.AppendNode(teamNode, "comment", team.Comment);

				XmlNode playersNode = doc.CreateElement("players");
				teamNode.AppendChild(playersNode);

				foreach (var player in team.Players)
				{
					XmlNode playerNode = doc.CreateElement("player");
					playersNode.AppendChild(playerNode);

					doc.AppendNode(playerNode, "name", player.Name);
					doc.AppendNode(playerNode, "buttonid", player.Id);
					if (player.Handicap != null && !player.Handicap.IsZero())
						doc.AppendNode(playerNode, "handicap", player.Handicap.ToString());
					if (!string.IsNullOrEmpty(player.Comment)) doc.AppendNode(playerNode, "comment", player.Comment);
				}
			}

			XmlNode gamesNode = doc.CreateElement("games");
			bodyNode.AppendChild(gamesNode);

			foreach (var game in AllGames)
			{
				XmlNode gameNode = doc.CreateElement("game");
				gamesNode.AppendChild(gameNode);

				doc.AppendNode(gameNode, "title", game.Title);
				doc.AppendNode(gameNode, "ansigametime", game.Time.ToString("yyyy/MM/dd HH:mm:ss"));
				doc.AppendNode(gameNode, "hits", game.Hits);
				if (game.Secret)
					doc.AppendNode(gameNode, "secret", "y");

				XmlNode teamsNode = doc.CreateElement("teams");
				gameNode.AppendChild(teamsNode);

				foreach (var team in game.Teams)
				{
					XmlNode teamNode = doc.CreateElement("team");
					teamsNode.AppendChild(teamNode);

					doc.AppendNode(teamNode, "teamid", team.TeamId ?? -1);
					doc.AppendNode(teamNode, "colour", team.Colour.ToString());
					doc.AppendNode(teamNode, "score", team.Score);
					doc.AppendNonZero(teamNode, "points", team.Points);
					doc.AppendNonZero(teamNode, "adjustment", team.Adjustment);
					doc.AppendNonZero(teamNode, "victorypointsadjustment", team.PointsAdjustment);
				}

				XmlNode playersNode = doc.CreateElement("players");
				gameNode.AppendChild(playersNode);

				foreach (var player in game.Players)
				{
					XmlNode playerNode = doc.CreateElement("player");
					playersNode.AppendChild(playerNode);

					doc.AppendNode(playerNode, "teamid", player.GameTeamId);
					doc.AppendNode(playerNode, "playerid", player.PlayerId);
					doc.AppendNode(playerNode, "pack", player.Pack);
					doc.AppendNode(playerNode, "score", player.Score);
					doc.AppendNode(playerNode, "rank", (int)player.Rank);
					doc.AppendNonZero(playerNode, "hitsby", player.HitsBy);
					doc.AppendNonZero(playerNode, "hitson", player.HitsOn);
					doc.AppendNonZero(playerNode, "basehits", player.BaseHits);
					doc.AppendNonZero(playerNode, "basedestroys", player.BaseDestroys);
					doc.AppendNonZero(playerNode, "basedenies", player.BaseDenies);
					doc.AppendNonZero(playerNode, "basedenied", player.BaseDenied);
					doc.AppendNonZero(playerNode, "yellowcards", player.YellowCards);
					doc.AppendNonZero(playerNode, "redcards", player.RedCards);

					doc.AppendNode(playerNode, "colour", (int)player.Colour);
				}
			}
			if (File.Exists(file + "5Backup"))
				File.Delete(file + "5Backup");  // Delete old backup file, if any.
			if (File.Exists(file))
				File.Move(file, file + "5Backup");  // Rename the old league file before we save over it, by changing its extension to ".Torn5Backup".
			doc.Save(file);
		}

		public override string ToString()
		{
			return Title ?? "league with blank title";
		}

		public Game Game(GamePlayer gamePlayer)
		{
			return AllGames.Find(g => g.Players.Contains(gamePlayer));
		}

		public Game Game(GameTeam gameTeam)
		{
			return AllGames.Find(g => g.Teams.Contains(gameTeam));
		}

		public Game Game(ServerGame serverGame)
		{
			return AllGames.Find(x => x.Time == serverGame.Time);
		}

		public GameTeam GameTeam(GamePlayer gamePlayer)
		{
			foreach (var game in AllGames)
			{
				GameTeam gameTeam = game.Teams.Find(gt => gt.Players.Contains(gamePlayer));
				if (gameTeam != null)
					return gameTeam;
			}

			return null;
		}

		public LeaguePlayer LeaguePlayer(GamePlayer gamePlayer)
		{
			return players.Find(p => p.Id == gamePlayer.PlayerId);
		}

		public LeagueTeam LeagueTeam(GameTeam gameTeam)
		{
			return teams.Find(t => t.AllPlayed.Contains(gameTeam));
		}

		public LeagueTeam LeagueTeam(GamePlayer gamePlayer)
		{
			return LeagueTeam(GameTeam(gamePlayer));
		}

		public LeagueTeam LeagueTeam(LeaguePlayer leaguePlayer)
		{
			return teams.Find(t => t.Players.Contains(leaguePlayer));
		}

		class TeamPlayerCount
		{
			public LeagueTeam LeagueTeam;
			public LeaguePlayer LeaguePlayer;
			public int Count;

			public TeamPlayerCount(LeagueTeam leagueTeam, LeaguePlayer leaguePlayer, int count)
			{
				LeagueTeam = leagueTeam;
				LeaguePlayer = leaguePlayer;
				Count = count;
			}
		}

		/// <summary>A list of players and all the teams they play on, with the teams list sorted by number of games played by that player for that team.</summary>
		public List<KeyValuePair<LeaguePlayer, List<LeagueTeam>>> BuildPlayerTeamList()
		{
			var tpcList = new List<TeamPlayerCount>();
			foreach (var leagueTeam in teams)
				foreach (var leaguePlayer in leagueTeam.Players)
					tpcList.Add(new TeamPlayerCount(leagueTeam, leaguePlayer, leagueTeam.AllPlayed.Sum(gt => gt.Players.Count(gp => gp.PlayerId == leaguePlayer.Id))));

			tpcList.Sort((x, y) => y.Count - x.Count);

			var playerTeamList = new List<KeyValuePair<LeaguePlayer, List<LeagueTeam>>>();
			foreach (var tpc in tpcList)
			{
				KeyValuePair<LeaguePlayer, List<LeagueTeam>> entry;
				if (playerTeamList.Exists(pt => pt.Key == tpc.LeaguePlayer))
					entry = playerTeamList.Find(pt => pt.Key == tpc.LeaguePlayer);
				else
				{
					entry = new KeyValuePair<LeaguePlayer, List<LeagueTeam>>(tpc.LeaguePlayer, new List<LeagueTeam>());
					playerTeamList.Add(entry);
				}
				entry.Value.Add(tpc.LeagueTeam);
			}
			return playerTeamList.OrderBy(pt => pt.Value[0].Name).ThenBy(pt => pt.Key.Name).ToList();
		}

		int Plays(LeagueTeam leagueTeam, LeaguePlayer leaguePlayer)
		{
			return leaguePlayer.Played.Where(x => this.LeagueTeam(x.GameTeam(this)) == leagueTeam).Count();
		}

		public string GameString(Game game)
		{
			var teamNames = new List<string>();
			foreach (var gameTeam in game.Teams)
			{
				var leagueTeam = LeagueTeam(gameTeam);
				teamNames.Add(leagueTeam != null ? leagueTeam.Name : gameTeam.ToString());
			}
			if (game.Secret)
				teamNames.Sort();
			return string.Join(", ", teamNames);
		}

		public string GameString(ServerGame serverGame)
		{
			var leagueTeams = GuessTeams(serverGame);
			return leagueTeams.Count == 0 ? "?" : string.Join(", ", leagueTeams);
		}

		public double AverageScore(LeagueTeam leagueTeam, bool includeSecret)
		{
			return Played(leagueTeam, includeSecret).Average(x => x.Score);
		}

		public double AveragePoints(LeagueTeam leagueTeam, bool includeSecret)
		{
			return Played(leagueTeam, includeSecret).Average(x => x.Points);
		}

		public double CalculateScore(GameTeam gameTeam)
		{
			double score = 0;

			foreach (var player in gameTeam.Players)
				score += player.Score;

			score += gameTeam.Adjustment;

			LeagueTeam leagueTeam = LeagueTeam(gameTeam);
			return leagueTeam != null && leagueTeam.Handicap != null ? new Handicap(leagueTeam.Handicap.Value, HandicapStyle).Apply(score) : score;
		}

		public double CalculatePoints(GameTeam gameTeam, GroupPlayersBy groupPlayersBy)
		{
			var game = Game(gameTeam);
			if (game != null)
			{
				var relevantTeams = (groupPlayersBy == GroupPlayersBy.Lotr ?  // For Lord of the Ring we want just "teams" of this colour. For other modes, we want all teams. 
				                     game.Teams.Where(t => t.Colour == gameTeam.Colour) : game.Teams).OrderBy(x => -x.Score).ToList();

				var ties = relevantTeams.Where(t => t.Score == gameTeam.Score);  // If there are ties, this list will contain the tied teams. If not, it will contain just this team.

				double totalPoints = 0;
				foreach (var team in ties)
				{
					int index = relevantTeams.IndexOf(team);

					if (victoryPoints.Valid(index))
						totalPoints += victoryPoints[index];
				}
				return totalPoints / ties.Count() + gameTeam.PointsAdjustment;  // If there are ties, average the victory points for all teams involved in the tie.
			}

			return 0;
		}

		public List<GameTeam> Played(LeagueTeam leagueTeam, bool includeSecret)
		{
			return includeSecret ? leagueTeam.AllPlayed : leagueTeam.AllPlayed.FindAll(x => Game(x) != null && !Game(x).Secret);
		}
	}

	/// <summary>This is P&C/Helios/Nexus-specific, because at the moment that's the only server we get this detailed data for.</summary>
	public class Event
	{
		public DateTime Time;
		public int PandCPlayerId;  // ID of shooter
		public int PandCPlayerTeamId;  // team of shooter
		public int Event_Type;  // see below
		public int Score;  // points gained by shooter
		public int HitPlayer;  // ID of shootee
		public int HitTeam;  // team of shootee
		public int PointsLostByDeniee;  // if hit is Event_Type = 1402 (base denier) or 1404 (base denied), shows points the shootee lost.
		public int ShotsDenied;  // if hit is Event_Type = 1402 or 1404, number of shots shootee had on the base when denied.

// EventType (see ng_event_types):
//  0..6:   tagged foe (in various hit locations: laser, chest, left shoulder, right shoulder, other, other, back);
//  7..13:  tagged ally;
//  14..20: tagged by foe;
//  21..27: tagged by ally;
//  28: warning;
//  29: termination;
//  30: hit base;
//  31: destroyed base;
//  32: eliminated;
//  33: hit by base;
//  34: hit by mine;
//  35: trigger pressed;
//  36: game state (whatever that means);
//  37..46: player tagged target (whatever that means);
//  1401: score denial points, friendly;
//  1402: score denial points;
//  1403: lose points for being denied, friendly;
//  1404: lose points for being denied. Note that Score field is incorrect for this event type -- use Result_Data_3, PointsLostByDeniee instead.

		public override string ToString()
		{
			return "Event Type " + Event_Type.ToString();
		}

	}

	/// <summary>Represents a game as stored on the laser game server.</summary>
	public class ServerGame: IComparable<ServerGame>
	{
		public int GameId { get; set; }
		public string Description { get; set; }
		public DateTime Time { get; set; }
		public DateTime EndTime { get; set; }
		public League League { get; set; }
		public Game Game { get; set; }
		public List<ServerPlayer> Players { get; set; }
		public bool InProgress { get; set; }
		public bool OnServer { get; set; }
		public List<Event> Events { get; set; }

		public ServerGame()
		{
			Players = new List<ServerPlayer>();
			Events = new List<Event>();
		}

		public int CompareTo(ServerGame compareGame)
	    {
			return compareGame == null ? 1 : this.Time.CompareTo(compareGame.Time);
	    }
	}

	/// <summary>Represents a player as stored on the laser game server.</summary>
	public class ServerPlayer: GamePlayer
	{
		public int PandCPlayerId { get; set; }  // This is the under-the-hood PAndC table ID field.
		public int PandCPlayerTeamId { get; set; }  // Ditto. These two are only used by systems with in-game data available.
		public string Alias { get; set; }
		/// <summary>If this object is linked from a ListViewItem's Tag, list that ListViewItem here.</summary>
		public ListViewItem Item { get; set; }

		public override string ToString()
		{
			return "ServerPlayer " + Alias;
		}
	}
}
