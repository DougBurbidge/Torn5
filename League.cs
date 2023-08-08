using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Torn
{
	//                       0       1     2      3       4       5     6     7       8      9     10    11   12     13       14       15    16       17
	public enum Colour { None = 0, Red, Blue, Green, Yellow, Purple, Pink, Cyan, Orange, White, Black, Fire, Ice, Earth, Crystal, Rainbow, Cops, Referee };
	public static class ColourExtensions
	{
		public static Color ToColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0xA0, 0xA0), Color.FromArgb(0xA0, 0xD0, 0xFF),  // None, Red, Blue,
				Color.FromArgb(0xA0, 0xFF, 0xA0), Color.FromArgb(0xFF, 0xFF, 0x90), Color.FromArgb(0xC0, 0xA0, 0xFF), Color.FromArgb(0xFF, 0xA0, 0xF0),  // Green, Yellow, Purple, Pink,
				Color.FromArgb(0xA0, 0xFF, 0xFF), Color.FromArgb(0xFF, 0xD0, 0xA0), Color.FromArgb(0xFF, 0xFF, 0xFF), Color.FromArgb(0x90, 0x90, 0x90),  // Cyan, Orange, White, Black,
				Color.FromArgb(0xFF, 0xB0, 0x90), Color.FromArgb(0xE0, 0xE0, 0xFF), Color.FromArgb(0xD0, 0xD0, 0x80), Color.FromArgb(0xFF, 0xC0, 0xF0),  // Fire, Ice, Earth, Crystal,
				Color.FromArgb(0xE0, 0xE0, 0xE0), Color.FromArgb(0xB0, 0xC0, 0xFF), Color.FromArgb(0xE0, 0xE0, 0xE0)  // Rainbow, Cops, Referee
			};
			return Colors[(int)colour];
		}

		public static Color ToSaturatedColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0x50, 0x50), Color.FromArgb(0x60, 0x80, 0xFF),  // None, Red, Blue,
				Color.FromArgb(0x20, 0xFF, 0x20), Color.FromArgb(0xFF, 0xFF, 0x00), Color.FromArgb(0x80, 0x00, 0xFF), Color.FromArgb(0xFF, 0x10, 0xB0),  // Green, Yellow, Purple, Pink,
				Color.FromArgb(0x00, 0xFF, 0xFF), Color.FromArgb(0xFF, 0x80, 0x50), Color.FromArgb(0xEE, 0xEE, 0xEE), Color.FromArgb(0x70, 0x70, 0x70),  // Cyan, Orange, White, Black,
				Color.FromArgb(0xFF, 0x60, 0x40), Color.FromArgb(0x90, 0x90, 0xFF), Color.FromArgb(0xB0, 0xB0, 0x60), Color.FromArgb(0xFF, 0x40, 0xF0),  // Fire, Ice, Earth, Crystal,
				Color.FromArgb(0x90, 0x90, 0x90), Color.FromArgb(0x90, 0xA0, 0xFF), Color.FromArgb(0xD0, 0xD0, 0xD0)  // Rainbow, Cops, Referee
			};
			return Colors[(int)colour];
		}

		public static Color ToDarkColor(this Colour colour)
		{
			Color[] Colors = { Color.Empty, Color.FromArgb(0xFF, 0x40, 0x40), Color.FromArgb(0x40, 0x50, 0xFF),  // None, Red, Blue,
				Color.FromArgb(0x00, 0xA0, 0x00), Color.FromArgb(0xA0, 0xA0, 0x00), Color.FromArgb(0x80, 0x00, 0xFF), Color.FromArgb(0xFF, 0x10, 0xB0),  // Green, Yellow, Purple, Pink,
				Color.FromArgb(0x00, 0xC0, 0xC0), Color.FromArgb(0xFF, 0x60, 0x30), Color.FromArgb(0xDD, 0xDD, 0xDD), Color.FromArgb(0x40, 0x40, 0x40),  // Cyan, Orange, White, Black,
				Color.FromArgb(0xFF, 0x50, 0x30), Color.FromArgb(0x70, 0x70, 0xFF), Color.FromArgb(0xB0, 0xB0, 0x50), Color.FromArgb(0xFF, 0x00, 0xF0),  // Fire, Ice, Earth, Crystal,
				Color.FromArgb(0x70, 0x70, 0x70), Color.FromArgb(0x70, 0x80, 0xFF), Color.FromArgb(0xD0, 0xD0, 0xD0)  // Rainbow, Cops, Referee
			};
			return Colors[(int)colour];
		}

		public static Colour ToColour(string s)
		{
			if (string.IsNullOrEmpty(s)) 
				return Colour.None;

			var dict = new Dictionary<string, Colour> { 
				{ "red", Colour.Red }, { "blue", Colour.Blue }, { "blu", Colour.Blue }, { "green", Colour.Green }, { "grn", Colour.Green }, { "yellow", Colour.Yellow }, { "yel", Colour.Yellow },
				{ "purple", Colour.Purple }, { "pink", Colour.Pink }, { "cyan", Colour.Cyan }, { "orange", Colour.Orange }, { "white", Colour.White }, { "black", Colour.Black },
				{ "fire", Colour.Fire }, { "ice", Colour.Ice }, { "earth", Colour.Earth }, { "crystal", Colour.Crystal }, { "rainbow", Colour.Rainbow }, { "cops", Colour.Cops }, { "referee", Colour.Referee }
			};

			dict.TryGetValue(s.ToLower(CultureInfo.InvariantCulture), out Colour c);
			return c;
		}

		public static Colour ToColour(char ch)
		{
			var dict = new Dictionary<char, Colour> {
				{ 'r', Colour.Red }, { 'b', Colour.Blue }, { 'g', Colour.Green }, { 'y', Colour.Yellow },
				{ 'p', Colour.Purple }, { 'm', Colour.Pink }, { 'c', Colour.Cyan }, { 'o', Colour.Orange }, { 'w', Colour.White },
				{ '1', Colour.Red }, { '2', Colour.Blue }, { '3', Colour.Green }, { '4', Colour.Yellow },
				{ '5', Colour.Purple }, { '6', Colour.Pink }, { '7', Colour.Cyan }, { '8', Colour.Orange }, { '9', Colour.White }, { '!', Colour.Referee }
			};

			dict.TryGetValue(char.ToLower(ch, CultureInfo.InvariantCulture), out Colour c);
			return c;
		}

		public static char ToChar(this Colour c)
		{
			return "xRBGYPMCOWbfiecrcr"[(int)c];
		}

		public static Colour ToColour(int i) // Converts from a Laserforce colour index number.
		{
			switch (i)
			{
				case 1: return Colour.Red;
				case 2: return Colour.Green;
				case 3: return Colour.Yellow;
				case 4: return Colour.Blue;
				case 5: return Colour.Cyan;  // Laserforce calls cyan "Aqua".
				case 6: return Colour.Purple;
				case 7: return Colour.White;
				case 8: return Colour.Orange;
				case 9: return Colour.Pink;
				case 10: return Colour.Black;
				case 11: return Colour.Fire;
				case 12: return Colour.Ice;
				case 13: return Colour.Earth;
				case 14: return Colour.Crystal;
				case 15: return Colour.Rainbow;
				case 16: return Colour.Cops;
				case 17: return Colour.Referee;
				default: return Colour.None;
			}
		}
	}

	public enum HandicapStyle { Percent, Plus, Minus, None };
	public static class HandicapExtensions
	{
		public static HandicapStyle ToHandicapStyle(string s)
		{
		    var dict = new Dictionary<string, HandicapStyle> { 
				{ "%", HandicapStyle.Percent }, { "+", HandicapStyle.Plus }, { "-", HandicapStyle.Minus }, { ".", HandicapStyle.None }, { "None", HandicapStyle.None }
			};

			dict.TryGetValue(s.ToLower(CultureInfo.InvariantCulture), out HandicapStyle h);
			return h;
		}
		
		public static string ToString(this HandicapStyle handicapStyle)
		{
			switch (handicapStyle) {
				case HandicapStyle.Percent: return "%";
				case HandicapStyle.Plus: return "+";
				case HandicapStyle.Minus: return "-";
				case HandicapStyle.None: return ".";
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
				Style == HandicapStyle.Minus ?   score - (double)Value :
				                                 score;
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

		/// <summary>True if the handicap is 100%, +0 or -0, or handicap style is None.</summary>
		public bool IsZero()
		{
			return Value == null || (Style == HandicapStyle.Percent && Value == 100) || (Style != HandicapStyle.Percent && Value == 0) || (Style == HandicapStyle.None);
		}
	}

	/// <summary>Stores data about a player in a league. (This is different from GamePlayer.)</summary>
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

		public string Grade { get; set;  }
	
		public LeaguePlayer Clone()
		{
			return new LeaguePlayer
			{
				Name = Name,
				Id = Id,
				Handicap = Handicap,
				Comment = Comment,
				Grade = Grade
			};
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>Stores data about each remembered league team</summary>
	public class LeagueTeam: IComparable
	{
		internal int TeamId  { get; set; }

		public List<LeaguePlayer> Players { get; set; }

		string name;
		public string Name 
		{
			get { 
				if (string.IsNullOrEmpty(name))
				{
					if (Players.Count == 0)
						name = "Team " + TeamId.ToString(CultureInfo.InvariantCulture);
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

		public Handicap Handicap { get; set; }
		public string Comment { get; set; }

		public LeagueTeam()
		{
			Players = new List<LeaguePlayer>();
		}

		public LeagueTeam Clone(League clonedLeague)
		{
			var clone = new LeagueTeam
			{
				Name = Name,
				TeamId = TeamId,
				Handicap = Handicap,
				Comment = Comment,

				Players = new List<LeaguePlayer>()
			};
			foreach (var player in Players)
			{
				// By the time we get here, clonedLeague contains cloned players, but our newly created clone LeagueTeam does not.
				// Look up each existing player id in the old LeagueTeam, find its match in clonedLeague, and put that in our clone.
				var newPlayer = clonedLeague.LeaguePlayer(player.Id);
				if (newPlayer != null)
					clone.Players.Add(newPlayer);
			}

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
		public DateTime Time { get; set; }

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
		public double Score { get; set; }
		/// <summary>Game score adjustment</summary>
		public double Adjustment { get; set; }

		public List<TermRecord> TermRecords { get; set; }

		/// <summary>Victory points</summary>
		public double Points { get; set; }
		/// <summary>Victory points adjustment</summary>
		public double PointsAdjustment { get; set; }

		private List<GamePlayer> players;
		public List<GamePlayer> Players { get { return players; } set { players = value; } }

		public GameTeam()
		{
			players = new List<GamePlayer>();
		}

		public void AddTermRecord(TermRecord termRecord)
		{
			if (TermRecords == null)
			{
				TermRecords = new List<TermRecord>();
			}
			TermRecords.Add(termRecord);
		}

		public double GetZeroedScore()
        {
			double score = 0;
			foreach(GamePlayer player in players)
            {
				Console.WriteLine(player.PlayerId + " " + player.ZeroedScore + " " + player.Score);
				if(player.ZeroedScore != null && player.ZeroedScore != 0)
                {
					score += (double)player.ZeroedScore;
                } else
                {
					score += player.Score;
                }
				Console.WriteLine(score);
            }
			return score;
        }

		public GameTeam Clone()
		{
			return new GameTeam
			{
				Time = Time,
				TeamId = TeamId,
				Colour = colour,
				Score = Score,
				Adjustment = Adjustment,
				Points = Points,
				PointsAdjustment = PointsAdjustment,
				TermRecords = TermRecords
			};
			// Don't clone players as this will be done by LinkThings().
		}

		int IComparable.CompareTo(object obj)
		{
			GameTeam gt = (GameTeam)obj;
			if (this.Points == gt.Points)
				return gt.Score.CompareTo(this.Score);
			else
				return gt.Points.CompareTo(this.Points);
		}

		public static Comparison<GameTeam> CompareTime = (gameTeam1, gameTeam2) => gameTeam1.Time.CompareTo(gameTeam2.Time);

		public int GetHitsBy()
        {
			int hitsBy = 0;
			foreach(GamePlayer player in players)
            {
				hitsBy += player.HitsBy;
				hitsBy -= player.BaseDestroys;
            }
			return hitsBy;
        }

		public override string ToString()
		{
			return "GameTeam " + (TeamId ?? -1).ToString() + " " + Score.ToString();
		}
	}

	public enum TermType
	{
		Yellow,
		Red,
		Verbal,
		Other
	}

	public class TermRecord
    {
		public TermType Type { get; set; }
		public DateTime? Time { get; set; }
		public int Value { get; set; }
		public string Reason { get; set; }

		[JsonConstructor]
		public TermRecord(TermType type, DateTime? time, int value, string reason = "")
        {
			Type = type;
			Time = time;
			Value = value;
			Reason = reason;
        }

		public TermRecord(TermType type, int value, string reason = "")
		{
			Type = type;
			Value = value;
			Reason = reason;
			Time = null;
		}

		public override string ToString()
		{
			return "Term Type: " + Type + " Time: " + (Time == null  ? "N/A" : Time.ToString()) + " Value: " + Value + " Reason: " + Reason;
		}
	}

	/// <summary>Stores data about a player in a single game. (This is different from LeaguePlayer.)</summary>
	public class GamePlayer: IComparable
	{
		public int? TeamId { get; set; }
		/// <summary>Under-the-hood laser game system identifier e.g. "P11-JP9", "1-50-50", etc. Same as LeaguePlayer.Id.</summary>
		public string PlayerId { get; set; }
		public string QRCode { get; set; }
		public string Grade { get; set; }
		public string Pack { get; set; }
		public double Score { get; set; }
		public double? ZeroedScore { get; set; }
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
		public List<TermRecord> TermRecords { get; set; }

		public bool IsEliminated { get; set; }

		public double GetZeroedScore()
        {
			if(ZeroedScore != null && ZeroedScore != 0)
            {
				return (double)ZeroedScore;
            } 
			return Score;
        }

		public void SetIsEliminated(bool isElimed)
        {
			if(isElimed)
            {
				IsEliminated = true;
				ZeroedScore = ZeroedScore == null ? Score : ZeroedScore;
            } else
            {
				IsEliminated = false;
				ZeroedScore = null;
            }
        }

		public void AddTermRecord(TermRecord termRecord)
        {
			if(TermRecords == null)
            {
				TermRecords = new List<TermRecord>();
            }
			TermRecords.Add(termRecord);
        }

		/// <summary>Used for accumulating data to be used for a team total, a game average, etc.</summary>
		public void Add(GamePlayer source)
		{
			Score += source.Score;
			ZeroedScore += source.ZeroedScore;
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
			return ((GamePlayer)obj).Score.CompareTo(this.Score);
		}

		public GamePlayer CopyTo(GamePlayer target)
		{
			target.TeamId = TeamId;
			target.PlayerId = PlayerId;
			target.QRCode = QRCode;
			target.Grade = Grade;
			target.Pack = Pack;
			target.Score = Score;
			target.ZeroedScore = ZeroedScore;
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
			target.TermRecords = TermRecords;
			target.IsEliminated = IsEliminated;

			return target;
		}

		/// <summary>Used for averaging data for a team, a game, etc.</summary>
		public void DivideBy(int divisor)
		{
			if (divisor == 0) return;

			Score /= divisor;
			ZeroedScore /= divisor;
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
			return "GamePlayer " + PlayerId + " QRCode: " + QRCode;
		}
		public string GetFormattedAlias(string alias)
		{
			return (IsEliminated ? "💀 " : "") + (RedCards > 0 ? (RedCards + "R ") : "") + (YellowCards > 0 ? (YellowCards + "Y ") : "") + alias + (IsEliminated ? ("(" + ZeroedScore + ")") : "");
		}
	}

	public class Game: IComparable
	{
		public string Title { get; set; }
		public DateTime Time { get; set; }
		/// <summary>If true, don't serve this game from our internal webserver, or include it in any webserver reports.</summary>
		public bool Secret { get; set; }
		public List<GameTeam> Teams { get; private set; }
		/// <summary>Players not yet placed onto a GameTeam.</summary>
		public List<GamePlayer> UnallocatedPlayers { get; private set; }
		public ServerGame ServerGame { get; set; }
		/// <summary>Use Reported to decide whether to emit various report pages. Managed by caller.</summary>
		public bool Reported { get; set; }

		int? hits = null;
		public int Hits { get 
			{
				if (hits == null)
					RefreshHits();

				return (int)hits;
			} 
		}

		public Game()
		{
			Teams = new List<GameTeam>();
			UnallocatedPlayers = new List<GamePlayer>();
		}

		/// <summary>List of players who were assigned onto teams in this game.</summary>
		public List<GamePlayer> Players()
		{
			return Teams.SelectMany(t => t.Players).ToList();
		}

		/// <summary>All players whose packs started, even players not finally assigned onto a team.</summary>
		public List<GamePlayer> AllPlayers()
		{
			return Teams.SelectMany(t => t.Players).Union(UnallocatedPlayers).ToList();
		}

		public List<GamePlayer> SortedPlayers()
		{
			return Players().OrderByDescending(p => p.Score).ToList();
		}

		public List<GamePlayer> SortedAllPlayers()
		{
			return AllPlayers().OrderByDescending(p => p.Score).ToList();
		}

		int IComparable.CompareTo(object obj)
		{
		   Game g = (Game)obj;
		   return DateTime.Compare(this.Time, g.Time);
		}

		public DateTime EndTime()
		{
			return ServerGame == null || ServerGame.EndTime == default ? Time.AddMinutes(12) : ServerGame.EndTime;
		}

		/// <summary>True if any team in this game has victory points.</summary>
		public bool IsPoints()
		{
			foreach (GameTeam gameTeam in Teams)
				if (gameTeam.Points != 0)
					return true;
			
			return false;
		}

		public string ShortTitle()
		{
			string timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Replace(":ss", "");

			return (Title + " " + Time.ToString(timeFormat)).Trim();
		}

		public string LongTitle()
		{
			return (string.IsNullOrEmpty(Title) ? "Game " : Title + " Game ") + Utility.ShortDateTime(Time);
		}

		/// <summary>Pull Event data from ServerGames and put it into GamePlayers.</summary>
		public bool PopulateEvents()
		{
			bool any = false;
			foreach (var player in AllPlayers())
				if (player is ServerPlayer serverPlayer && !serverPlayer.IsPopulated())
				{
					serverPlayer.Populate(ServerGame.Events);
					any = true;
				}

			if (any)
				RefreshHits();

			return any;
		}

		void RefreshHits()
		{
			hits = Players().Sum(p => p.HitsOn);
		}

		public int Rank(string playerId)
		{
			return SortedPlayers().FindIndex(x => x.PlayerId == playerId) + 1;
		}

		public int Rank(GameTeam gt)
		{
			return Teams.IndexOf(gt) + 1;
		}

		public override string ToString()
		{
			return Time.ToString("yyyy/MM/dd HH:mm") + ": " + 
				(Secret ? string.Join(", ", this.Teams.OrderBy(x => x.ToString()).Select(x => x.ToString())) :
				 string.Join(", ", this.Teams.Select(x => x.ToString())));
		}

		double? totalScore = null;
		public double TotalScore()
		{
			if (totalScore == null && totalScore != 0)
				totalScore = Teams.Sum(t => t.Score);

			return (int)totalScore;
		}
	}

	public class Games: List<Game>
	{
		public DateTime? MostRecent()
		{
			return this.Any() ? this.Select(x => x.Time).Max() : (DateTime?)null;
		}
	}

	/// <summary>Used during game Commit, to hold a TeamBox's GameTeam and ServerPlayers.</summary>
	public class GameTeamData
	{
		public GameTeam GameTeam { get; set; }
		public List<ServerPlayer> Players { get; set; }

		public override string ToString()
		{
			return "GameTeamData " + (GameTeam == null ? "null GameTeam" : "GameTeam " + GameTeam.TeamId.ToString()) + ": " +
				(Players == null ? "null" : Players.Count.ToString()) + " Players";
		}
	}

	public class Grade
	{
		public string Name { get; set; }
		public decimal Points { get; set; }
		public bool HasPenalty { get; set; }
		public bool HasBonus { get; set; }

		public Grade(string name, decimal points, bool hasPenalty, bool hasBonus)
		{
			Name = name;
			Points = points;
			HasBonus = hasBonus;
			HasPenalty = hasPenalty;
		}

		public Grade(string name, decimal points)
		{
			Name = name;
			Points = points;
			HasBonus = false;
			HasPenalty = false;
		}
	}

	public class PointPercent
    {
		public decimal Points { get; set; }
		public decimal Percent { get; set; }

		public PointPercent(decimal points, decimal percent)
        {
			Points = points;
			Percent = percent;
        }
	}

	/// <summary>Load and manage a .Torn league file, containing league teams and games.</summary>
	public class League
	{
		public string Key { get; set; }
		public string Title { get; set; }

		/// <summary>Watch our .Torn file. If another process writes it, reload.</summary>
		FileSystemWatcher Watcher { get; set; }

		string fileName;
		public string FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				Watcher.Path = new FileInfo(fileName).Directory.FullName;
				Watcher.Filter = Path.GetFileName(fileName);
				Watcher.EnableRaisingEvents = true;
			}
		}

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

		public bool HitsTieBreak { get; set; }
		public bool ZeroedTieBreak { get; set; }
		public bool ZeroElimed { get; set; }
		public bool ZeroVps { get; set; }
		public bool HalfVps { get; set; }

		public double VictoryPointsHighScore { get; set; }
		public int SweepBonus { get; set; }
		public double VictoryPointsProportional { get; set; }

		public bool IsAutoHandicap { get; set; }

		public List<Grade> Grades { get; set; }

		public List<PointPercent> PointPercents { get; set; }

		public int ExpectedTeamSize { get; set; }
		public decimal MissingPlayerPenalty { get; set; }
		public decimal ExtraAPenalty { get; set; }
		public decimal ExtraGBonus { get; set; }

		public decimal VerbalTermValue { get; set; }
		public decimal YellowTermValue { get; set; }
		public decimal RedTermValue { get; set; }

		private readonly int DEFAULT_VERBAL_TERM = 0;
		private readonly int DEFAULT_YELLOW_TERM = -1000;
		private readonly int DEFAULT_RED_TERM = -2000;

		private readonly List<Grade> DEFAULT_GRADES = new List<Grade>
		{
			new Grade("AAA", 6, true, false),
			new Grade("A", 5, true, false),
			new Grade("BB", 5, false, false),
			new Grade("B", 4, false, false),
			new Grade("C", 3, false, false),
			new Grade("D", 2, false, false),
			new Grade("E", 1, false, false),
			new Grade("F", 0, false, false),
			new Grade("G", 0, false, true)
		};

		public readonly List<PointPercent> WA_LEAGUE_POINTS = new List<PointPercent>
		{
			new PointPercent(-4,260),
			new PointPercent(-3,240),
			new PointPercent(-2,220),
			new PointPercent(-1,200),
			new PointPercent(0,180),
			new PointPercent(1,170),
			new PointPercent(2,160),
			new PointPercent(3,155),
			new PointPercent(4,150),
			new PointPercent(5,145),
			new PointPercent(6,140),
			new PointPercent(7,135),
			new PointPercent(8,130),
			new PointPercent(9,127),
			new PointPercent(10,125),
			new PointPercent(11,123),
			new PointPercent(12,120),
			new PointPercent(13,117),
			new PointPercent(14,115),
			new PointPercent(15,113),
			new PointPercent(16,110),
			new PointPercent(17,107),
			new PointPercent(18,105),
			new PointPercent(19,103),
			new PointPercent(20,100),
			new PointPercent(21,100),
			new PointPercent(22,97),
			new PointPercent(23,95),
			new PointPercent(24,92),
			new PointPercent(25,90),
			new PointPercent(26,87),
			new PointPercent(27,85),
			new PointPercent(28,82),
			new PointPercent(29,80),
			new PointPercent(30,77),
			new PointPercent(31,75),
			new PointPercent(32,72),
			new PointPercent(33,70),
			new PointPercent(34,65),
		};

		public readonly List<PointPercent> WA_DOUBLES_POINTS = new List<PointPercent>
		{
			new PointPercent(-1,145),
			new PointPercent(0,140),
			new PointPercent(1,135),
			new PointPercent(2,130),
			new PointPercent(3,125),
			new PointPercent(4,120),
			new PointPercent(5,115),
			new PointPercent(6,110),
			new PointPercent(7,105),
			new PointPercent(8,100),
			new PointPercent(9,95),
			new PointPercent(10,90),
			new PointPercent(11,85),
			new PointPercent(12,80),
			new PointPercent(13,75),
		};

		public readonly List<PointPercent> WA_TRIPLES_POINTS = new List<PointPercent>
		{
			new PointPercent(-2,185),
			new PointPercent(-1,170),
			new PointPercent(0,160),
			new PointPercent(1,155),
			new PointPercent(2,150),
			new PointPercent(3,145),
			new PointPercent(4,140),
			new PointPercent(5,135),
			new PointPercent(6,130),
			new PointPercent(7,125),
			new PointPercent(8,120),
			new PointPercent(9,115),
			new PointPercent(10,110),
			new PointPercent(11,105),
			new PointPercent(12,100),
			new PointPercent(13,94),
			new PointPercent(14,87),
			new PointPercent(15,79),
			new PointPercent(16,70),
			new PointPercent(17,60),
			new PointPercent(18,49),
			new PointPercent(19,37),
			new PointPercent(20,24),
		};

		public int GetAutoHandicap(int points)
        {
			PointPercent pointPercent = PointPercents.Find((pp) => pp.Points == points);
			if (pointPercent == null && PointPercents.Count() > 1)
            {
				List<PointPercent> sortedPointPercents = PointPercents.OrderBy(p => p.Points).ToList();
				PointPercent minPointPercent = sortedPointPercents[0];
				PointPercent maxPointPercent = sortedPointPercents[sortedPointPercents.Count - 1];

				if (points < minPointPercent.Points)
                {
					PointPercent secondSmallestPointPercent = sortedPointPercents[1];
					decimal diffPoints = Math.Abs(points - minPointPercent.Points);
					decimal diff = Math.Abs(secondSmallestPointPercent.Percent - minPointPercent.Percent) * diffPoints;
					int result = Convert.ToInt32(minPointPercent.Percent + diff);
					return result;
				}
				if (points > maxPointPercent.Points)
				{
					PointPercent secondLargestPointPercent = sortedPointPercents[sortedPointPercents.Count - 2];
					decimal diffPoints = Math.Abs(points - maxPointPercent.Points);
					decimal diff = Math.Abs(secondLargestPointPercent.Percent - maxPointPercent.Percent) * diffPoints;
					int result = Math.Max(Convert.ToInt32(maxPointPercent.Percent - diff), 1);
					return result;
				}
			}
			return Convert.ToInt32(pointPercent.Percent);
        }

		public decimal GetGradePoints(string playerGrade)
        {
			Grade grade = Grades.Find(g => g.Name == playerGrade);
			return grade?.Points ?? 0;
        }

		public decimal GetGradePenalty(string playerGrade)
		{
			Grade grade = Grades.Find(g => g.Name == playerGrade);
			if (grade!= null && grade.HasPenalty)
				return ExtraAPenalty;
			else
				return 0;
		}

		public decimal GetGradeBonus(string playerGrade)
		{
			Grade grade = Grades.Find(g => g.Name == playerGrade);
			if (grade != null && grade.HasBonus)
				return ExtraGBonus;
			else
				return 0;
		}

		public League()
		{
			teams = new List<LeagueTeam>();
			players = new List<LeaguePlayer>();
			AllGames = new Games();
			victoryPoints = new Collection<double>();

			Watcher = new FileSystemWatcher
			{
				NotifyFilter = NotifyFilters.LastWrite
			};
			Watcher.Changed += new FileSystemEventHandler(OnFileChanged);

			GridHigh = 3;
			GridWide = 1;
			GridPlayers = 6;
			HandicapStyle = HandicapStyle.Percent;
			Grades = DEFAULT_GRADES;
			PointPercents = WA_LEAGUE_POINTS;
			ExpectedTeamSize = 5;
			MissingPlayerPenalty = 2;
			ExtraAPenalty = 1;
			ExtraGBonus = -1;
			VerbalTermValue = DEFAULT_VERBAL_TERM;
			YellowTermValue = DEFAULT_YELLOW_TERM;
			RedTermValue = DEFAULT_RED_TERM;
		}

		public League(string fileName): this()
		{
			Clear();
			FileName = fileName;
			Title = Path.GetFileNameWithoutExtension(fileName).Replace('_', ' ');
		}

		public void AddTeam(LeagueTeam leagueTeam)
		{
			if (leagueTeam.TeamId <= 0)
				leagueTeam.TeamId = NextTeamId();

			Teams.Add(leagueTeam);
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
			League clone = new League
			{
				Title = Title,
				fileName = fileName,

				GridHigh = GridHigh,
				GridWide = GridWide,
				GridPlayers = GridPlayers,

				players = Players.Select(item => (LeaguePlayer)item.Clone()).ToList(),

				victoryPoints = new Collection<double>(VictoryPoints.Select(item => item).ToList()),
				VictoryPointsHighScore = VictoryPointsHighScore,
				VictoryPointsProportional = VictoryPointsProportional,
				Grades = Grades,
				ExpectedTeamSize = ExpectedTeamSize,
				MissingPlayerPenalty = MissingPlayerPenalty,
				ExtraAPenalty = ExtraAPenalty,
				ExtraGBonus = ExtraGBonus
			};

			clone.teams = Teams.Select(item => (LeagueTeam)item.Clone(clone)).ToList();
			clone.AllGames.AddRange(AllGames);
			return clone;
		}

		string LinkTeamToGame(GameTeamData teamData, ServerGame serverGame)
		{
			if (teamData.GameTeam == null)
				teamData.GameTeam = new GameTeam();

			var gameTeam = teamData.GameTeam;
			Game game = serverGame.Game ?? Game(gameTeam);
			if (game == null)
			{
				game = new Game
				{
					Time = serverGame.Time
				};
				serverGame.Game = game;
				serverGame.Game.ServerGame = serverGame;
			}

			gameTeam.Time = game.Time;
			game.Teams.Add(gameTeam);

			LeagueTeam leagueTeam = LeagueTeam(gameTeam);

			if (leagueTeam == null)
				leagueTeam = GuessTeam(teamData.Players.Select(x => x.PlayerId).ToList());
			
			if (leagueTeam == null)
			{
				List<LeaguePlayer> leaguePlayers = new List<LeaguePlayer>();
				foreach(ServerPlayer player in teamData.Players)
                {
					LeaguePlayer leaguePlayer = new LeaguePlayer
					{
						Name = player.Alias,
						Id = player.PlayerId
					};
					leaguePlayers.Add(leaguePlayer);
                }
				leagueTeam = new LeagueTeam
				{
					TeamId = NextTeamId(),
					Players = leaguePlayers
				};
				Teams.Add(leagueTeam);
			}
			
			gameTeam.TeamId = leagueTeam.TeamId;

			gameTeam.Players.Clear();
			gameTeam.Players.AddRange(teamData.Players);

			if (IsAutoHandicap)
			{
				int cap = CalulateTeamCap(gameTeam);
				leagueTeam.Handicap = new Handicap(cap, HandicapStyle.Percent);
			}

			return leagueTeam.Name;
		}

		string LinkPlayerToGame(GamePlayer gamePlayer, GameTeam gameTeam, ServerGame game)
		{
			gamePlayer.TeamId = (int)gameTeam.TeamId;

			if (!game.Game.AllPlayers().Contains(gamePlayer))
				game.Game.UnallocatedPlayers.Add(gamePlayer);

			var leaguePlayer = LeaguePlayer(gamePlayer);

			if (leaguePlayer == null)
			{
				leaguePlayer = new LeaguePlayer();
				if (gamePlayer is ServerPlayer serverPlayer)
					leaguePlayer.Name = serverPlayer.Alias;
				leaguePlayer.Id = gamePlayer.PlayerId;
				Players.Add(leaguePlayer);
			}

			if (!LeagueTeam(gameTeam).Players.Contains(leaguePlayer))
				LeagueTeam(gameTeam).Players.Add(leaguePlayer);

			return leaguePlayer == null ? "null" : leaguePlayer.Name;
		}

		public string CommitGame(ServerGame serverGame, List<GameTeamData> teamDatas, GroupPlayersBy groupPlayersBy)
		{
			Load(FileName);
			var debug = new StringBuilder();
			if (serverGame.Game == null)
			{
				serverGame.Game = new Game
				{
					Time = serverGame.Time,
					ServerGame = serverGame
				};
				debug.Append("Created new serverGame for ");
				debug.Append(serverGame.Time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture));
				debug.Append(".\n");
			}
			Game game = serverGame.Game;

			game.Teams.Clear();
			foreach (var teamData in teamDatas)
			{
				debug.Append("ID'ed league team ");
				debug.Append(LinkTeamToGame(teamData, serverGame));
				debug.Append(" for players ");

				foreach (var player in teamData.Players)
				{
					debug.Append(LinkPlayerToGame(game.AllPlayers().Find(gp => gp.PlayerId == player.PlayerId) ?? player, //.CopyTo(new GamePlayer()),
					                              teamData.GameTeam, serverGame));
					debug.Append(", ");
				}

				debug.Length -= 2; debug.Append(".\n");

				teamData.GameTeam.Players.ForEach((player) => {
					LeaguePlayer leaguePlayer = Players.Find((p) => p.Id == player.PlayerId);
					player.Grade = leaguePlayer.Grade;
				});

				teamData.GameTeam.Players.Sort();
				teamData.GameTeam.Score = (int)CalculateScore(teamData.GameTeam);
			}

			var players = game.SortedAllPlayers();
			for (int i = 0; i < players.Count; i++)
				players[i].Rank = (uint)i + 1;

			if (!AllGames.Contains(game))
			{
				AllGames.RemoveAll(g => g.Time == game.Time);
				AllGames.Add(game);
				AllGames.Sort();
			}

			game.Teams.Sort();
			game.Reported = false;
			serverGame.League = this;

			foreach (var teamData in teamDatas)
				teamData.GameTeam.Points = CalculatePoints(teamData.GameTeam, groupPlayersBy);

			Save();
			return debug.ToString();
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

			var teams = game.Players.Select(x => x.ServerTeamId).Distinct();

			foreach (int teamId in teams)
				result.Add(GuessTeam(game.Players.FindAll(x => x.ServerTeamId == teamId).Select(y => y.PlayerId).ToList()));

			return result.Where(x => x != null).ToList();
		}

		public LeagueTeam GuessTeam(List<string> ids)
		{
			if (!teams.Any() || !ids.Any())
				return null;

			LeagueTeam bestTeam = null;
			double bestScore = 0;

			foreach (LeagueTeam team in teams)
				if (team.Players.Any())
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

		int NextTeamId()
		{
			return teams.Any() ? teams.Max(x => x.TeamId) + 1 : 1;
		}

		void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			System.Threading.Thread.Sleep(1000);
			Load(fileName);
		}

		/// <summary>Load a .Torn file from disk.</summary>
		public void Load(string fileName)
		{
			Clear();

			var doc = new XmlDocument();
			doc.Load(fileName);

			var root = doc.DocumentElement;

			Title = root.GetString("Title");
			Key = root.GetString("Key");
			if (string.IsNullOrEmpty(Title))
				Title = Path.GetFileNameWithoutExtension(fileName).Replace('_', ' ');

			GridHigh = root.GetInt("GridHigh");
			GridWide = root.GetInt("GridWide");
			GridPlayers = root.GetInt("GridPlayers");
			sortMode = root.GetInt("SortMode");
			string s = root.GetString("HandicapStyle");
			HandicapStyle = s == "%" || s == "Percent" ? HandicapStyle.Percent : s == "+" || s == "Plus" ? HandicapStyle.Plus : HandicapStyle.Minus;
			sortByRank = root.GetInt("SortByRank");
			autoUpdate = root.GetInt("AutoUpdate");
			updateTeams = root.GetInt("UpdateTeams");
			elimMultiplier = root.GetInt("ElimMultiplier");
			int teamSize = root.GetInt("ExpectedTeamSize");
			MissingPlayerPenalty = root.GetDecimal("MissingPlayerPenalty");
			ExtraAPenalty = root.GetDecimal("ExtraAPenalty");
			ExtraGBonus = root.GetDecimal("ExtraGBonus");
			VerbalTermValue = root.GetString("VerbalTermValue") == null ? DEFAULT_VERBAL_TERM : root.GetDecimal("VerbalTermValue");
			YellowTermValue = root.GetString("YellowTermValue") == null ? DEFAULT_YELLOW_TERM : root.GetDecimal("YellowTermValue");
			RedTermValue = root.GetString("RedTermValue") == null ? DEFAULT_RED_TERM : root.GetDecimal("RedTermValue");
			IsAutoHandicap = root.GetInt("AutoHandicap") > 0;
			HitsTieBreak = root.GetInt("HitsTieBreak") > 0;
			ZeroedTieBreak = root.GetInt("ZeroedTieBreak") > 0;
			ZeroElimed = root.GetInt("ZeroElimed") > 0;
			ZeroVps = root.GetInt("ZeroVps") > 0;
			HalfVps = root.GetInt("HalfVps") > 0;
			SweepBonus = root.GetInt("SweepBonus");

			ExpectedTeamSize = teamSize == 0 ? 5 : teamSize;

			XmlNodeList points = root.SelectNodes("Points");
			foreach (XmlNode point in points)
				victoryPoints.Add(double.Parse(point.InnerText, CultureInfo.InvariantCulture));

			VictoryPointsHighScore = root.GetDouble("High");
			VictoryPointsProportional = root.GetDouble("Proportional");

			XmlNode gradesNode = root.SelectSingleNode("grades");

			if (gradesNode != null)
			{

				XmlNodeList xgrades = gradesNode.SelectNodes("grade");

				List<Grade> grades = new List<Grade>();

				foreach (XmlNode xgrade in xgrades)
				{
					Grade grade = new Grade(xgrade.GetString("name"), xgrade.GetDecimal("points"), xgrade.GetInt("hasPenalty") > 0, xgrade.GetInt("hasBonus") > 0);
					grades.Add(grade);
				}

				Grades = grades;
			} else
            {
				Grades = DEFAULT_GRADES;
            }

			XmlNode capsNode = root.SelectSingleNode("caps");

			if(capsNode != null)
            {
				XmlNodeList xcaps = capsNode.SelectNodes("cap");

				List<PointPercent> pointPercents = new List<PointPercent>();

				foreach (XmlNode xcap in xcaps)
				{
					PointPercent pointPercent = new PointPercent(xcap.GetDecimal("points"), xcap.GetDecimal("percent"));
					pointPercents.Add(pointPercent);
				}
				PointPercents = pointPercents;
			} else
            {
				PointPercents = WA_LEAGUE_POINTS;

			}

			XmlNodeList xteams = root.SelectSingleNode("leaguelist").SelectNodes("team");

			foreach (XmlNode xteam in xteams)
			{
				LeagueTeam leagueTeam = new LeagueTeam
				{
					Name = xteam.GetString("teamname"),
					TeamId = xteam.GetInt("teamid"),
					Handicap = Handicap.Parse(xteam.GetString("handicap")),
					Comment = xteam.GetString("comment")
				};

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
						leaguePlayer.Grade = xplayer.GetString("grade");

						if (!leagueTeam.Players.Exists(x => x.Id == id))
							leagueTeam.Players.Add(leaguePlayer);
					}
				}

				teams.Add(leagueTeam);
			}

			XmlNodeList xgames = root.SelectSingleNode("games").SelectNodes("game");

			foreach (XmlNode xgame in xgames) 
			{
				Game game = new Game
				{
					Title = xgame.GetString("title"),
					Secret = xgame.GetString("secret") == "y",
					Reported = xgame.GetString("reported") == "y"
				};

				var child = xgame.SelectSingleNode("ansigametime");
				if (child == null)
					game.Time = new DateTime(1899, 12, 30).AddDays(double.Parse(xgame.GetString("gametime"), CultureInfo.InvariantCulture));
				else
					game.Time = DateTime.Parse(child.InnerText, CultureInfo.InvariantCulture);
				//game.Hits = XmlInt(xgame, "hits");

				xteams = xgame.SelectSingleNode("teams").SelectNodes("team");

				foreach (XmlNode xteam in xteams)
				{
					GameTeam gameTeam = new GameTeam
					{
						Time = game.Time,
						TeamId = xteam.GetInt("teamid"),
						Colour = ColourExtensions.ToColour(xteam.GetString("colour")),
						Score = xteam.GetDouble("score"),
						Points = xteam.GetDouble("points"),
						Adjustment = xteam.GetDouble("adjustment"),
						PointsAdjustment = xteam.GetDouble("victorypointsadjustment")
					};

					if (xteam.SelectSingleNode("terms") != null)
					{
						XmlNodeList xterms = xteam.SelectSingleNode("terms").SelectNodes("term");

						foreach (XmlNode xterm in xterms)
						{
							TermType termType;
							TermType.TryParse(xterm.GetString("type"), out termType);
							string reason = xterm.SelectSingleNode("reason") != null ? xterm.GetString("reason") : "";
							string time = xterm.SelectSingleNode("time") != null ? xterm.GetString("time") : "";
							TermRecord termRecord = time == "" ?
								termRecord = new TermRecord(termType, xterm.GetInt("value"), reason) :
								termRecord = new TermRecord(termType, DateTime.Parse(time), xterm.GetInt("value"), reason);

							gameTeam.AddTermRecord(termRecord);
						}
					}

					game.Teams.Add(gameTeam);
				}
				game.Teams.Sort();

				XmlNodeList xplayers = xgame.SelectSingleNode("players").SelectNodes("player");

				foreach (XmlNode xplayer in xplayers)
				{
					GamePlayer gamePlayer = new GamePlayer
					{
						PlayerId = xplayer.GetString("playerid"),
						QRCode = xplayer.GetString("qrcode"),
						TeamId = xplayer.GetInt("teamid"),
						Pack = xplayer.GetString("pack"),
						Grade = xplayer.GetString("grade"),
						Score = xplayer.GetInt("score"),
						ZeroedScore = xplayer.GetInt("zeroedScore"),
						Rank = (uint)xplayer.GetInt("rank"),
						HitsBy = xplayer.GetInt("hitsby"),
						HitsOn = xplayer.GetInt("hitson"),
						BaseHits = xplayer.GetInt("basehits"),
						BaseDestroys = xplayer.GetInt("basedestroys"),
						BaseDenies = xplayer.GetInt("basedenies"),
						BaseDenied = xplayer.GetInt("basedenied"),
						YellowCards = xplayer.GetInt("yellowcards"),
						RedCards = xplayer.GetInt("redcards"),
						IsEliminated = xplayer.GetInt("elim") > 0
					};

					if (xplayer.SelectSingleNode("colour") != null)
						gamePlayer.Colour = (Colour)(xplayer.GetInt("colour") + 1);

					if (xplayer.SelectSingleNode("terms") != null)
					{
						XmlNodeList xterms = xplayer.SelectSingleNode("terms").SelectNodes("term");

						foreach (XmlNode xterm in xterms)
						{
							TermType termType;
							TermType.TryParse(xterm.GetString("type"), out termType);
							string reason = xterm.SelectSingleNode("reason") != null ? xterm.GetString("reason") : "";
							string time = xterm.SelectSingleNode("time") != null ? xterm.GetString("time") : "";
							TermRecord termRecord = time == "" ?
								termRecord = new TermRecord(termType, xterm.GetInt("value"), reason) :
								termRecord = new TermRecord(termType, DateTime.Parse(time), xterm.GetInt("value"), reason);

							gamePlayer.AddTermRecord(termRecord);
						}
					}

					game.UnallocatedPlayers.Add(gamePlayer);
				}
				
				AllGames.Add(game);
			}

			AllGames.Sort();
			LinkThings();

			FileName = fileName;
		}

		/// <summary>
		/// If a league has just been loaded, it has a bunch of "links" present only as ID fields.
		/// LinkThings() matches these, and populates links and lists accordingly. 
		/// </summary>
		void LinkThings()
		{
			teams.Sort();

			for(int i = 0; i < AllGames.Count; i++)
            {
				var game = AllGames[i];
				foreach (GameTeam gameTeam in game.Teams) 
				{
					// Connect each game team back to their league team.
					var leagueTeam = teams.Find(x => x.TeamId == gameTeam.TeamId);
					if (leagueTeam != null)
					{
						// Connect each game player to their game team.
						gameTeam.Players.Clear();
						var players = game.UnallocatedPlayers.ToList();  // Make a copy of UnallocatedPlayers. We can't remove from the collection we're foreach'ing over.
						foreach (GamePlayer gamePlayer in players)
							if (gamePlayer.TeamId == gameTeam.TeamId)
							{
								gameTeam.Players.Add(gamePlayer);
								game.UnallocatedPlayers.Remove(gamePlayer);
							}
					}
				}
			}
		}

		/// <summary>Save a .Torn file to disk.</summary>
		public void Save(string fileName = "")
		{
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
			doc.AppendNode(bodyNode, "AutoHandicap", IsAutoHandicap ? 1 : 0);
			doc.AppendNonZero(bodyNode, "ExpectedTeamSize", ExpectedTeamSize);
			doc.AppendNode(bodyNode, "MissingPlayerPenalty", MissingPlayerPenalty.ToString());
			doc.AppendNode(bodyNode, "ExtraAPenalty", ExtraAPenalty.ToString());
			doc.AppendNode(bodyNode, "ExtraGBonus", ExtraGBonus.ToString());
			doc.AppendNode(bodyNode, "HitsTieBreak", HitsTieBreak ? 1 : 0);
			doc.AppendNode(bodyNode, "ZeroedTieBreak", ZeroedTieBreak ? 1 : 0);
			doc.AppendNode(bodyNode, "Key", Key);
			doc.AppendNode(bodyNode, "VerbalTermValue", VerbalTermValue.ToString());
			doc.AppendNode(bodyNode, "YellowTermValue", YellowTermValue.ToString());
			doc.AppendNode(bodyNode, "RedTermValue", RedTermValue.ToString());
			doc.AppendNode(bodyNode, "ZeroElimed", ZeroElimed ? 1 : 0);
			doc.AppendNode(bodyNode, "ZeroVps", ZeroVps ? 1 : 0);
			doc.AppendNode(bodyNode, "HalfVps", HalfVps ? 1 : 0);
			doc.AppendNode(bodyNode, "SweepBonus", SweepBonus);


			XmlNode gradesNode = doc.CreateElement("grades");
			bodyNode.AppendChild(gradesNode);

			foreach (Grade grade in Grades)
            {
				XmlNode gradeNode = doc.CreateElement("grade");
				gradesNode.AppendChild(gradeNode);

				doc.AppendNode(gradeNode, "name", grade.Name);
				doc.AppendNode(gradeNode, "points", grade.Points.ToString());
				if (grade.HasPenalty)
					doc.AppendNode(gradeNode, "hasPenalty", 1);
				if (grade.HasBonus)
				doc.AppendNode(gradeNode, "hasBonus", 1);
			}

			XmlNode capsNode = doc.CreateElement("caps");
			bodyNode.AppendChild(capsNode);

			foreach (PointPercent pointPercent in PointPercents)
			{
				XmlNode capNode = doc.CreateElement("cap");
				capsNode.AppendChild(capNode);

				doc.AppendNode(capNode, "percent", pointPercent.Percent.ToString());
				doc.AppendNode(capNode, "points", pointPercent.Points.ToString());
			}

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
				doc.AppendNode(teamNode, "teamid", team.TeamId);
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
					if (!string.IsNullOrEmpty(player.Grade)) doc.AppendNode(playerNode, "grade", player.Grade);
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
				if (game.Reported)
					doc.AppendNode(gameNode, "reported", "y");

				XmlNode teamsNode = doc.CreateElement("teams");
				gameNode.AppendChild(teamsNode);

				foreach (var team in game.Teams)
				{
					XmlNode teamNode = doc.CreateElement("team");
					teamsNode.AppendChild(teamNode);

					doc.AppendNode(teamNode, "teamid", team.TeamId ?? -1);
					doc.AppendNode(teamNode, "colour", team.Colour.ToString());
					doc.AppendNonZero(teamNode, "score", team.Score);
					doc.AppendNonZero(teamNode, "points", team.Points);
					doc.AppendNonZero(teamNode, "adjustment", team.Adjustment);
					doc.AppendNonZero(teamNode, "victorypointsadjustment", team.PointsAdjustment);

					if (team.TermRecords != null)
					{
						XmlNode termsNode = doc.CreateElement("terms");
						teamNode.AppendChild(termsNode);

						foreach (TermRecord termRecord in team.TermRecords)
						{
							XmlNode termNode = doc.CreateElement("term");
							termsNode.AppendChild(termNode);

							doc.AppendNode(termNode, "type", termRecord.Type.ToString());
							doc.AppendNode(termNode, "time", termRecord.Time.ToString());
							doc.AppendNode(termNode, "value", termRecord.Value);
							doc.AppendNode(termNode, "reason", termRecord.Reason);
						}
					}

				}

				XmlNode playersNode = doc.CreateElement("players");
				gameNode.AppendChild(playersNode);

				foreach (var player in game.Players())
				{
					XmlNode playerNode = doc.CreateElement("player");
					playersNode.AppendChild(playerNode);

					doc.AppendNode(playerNode, "teamid", player.TeamId ?? -1);
					doc.AppendNode(playerNode, "playerid", player.PlayerId);
					doc.AppendNode(playerNode, "qrcode", player.QRCode);
					doc.AppendNode(playerNode, "grade", player.Grade);
					doc.AppendNode(playerNode, "pack", player.Pack);
					doc.AppendNode(playerNode, "score", (int)(ZeroElimed && player.IsEliminated && player.Score > 0 ? 0 : player.Score));
					if (ZeroElimed && player.IsEliminated)
					{
						doc.AppendNode(playerNode, "zeroedScore", (int)player.ZeroedScore);
					}
					doc.AppendNode(playerNode, "rank", (int)player.Rank);
					doc.AppendNonZero(playerNode, "hitsby", player.HitsBy);
					doc.AppendNonZero(playerNode, "hitson", player.HitsOn);
					doc.AppendNonZero(playerNode, "basehits", player.BaseHits);
					doc.AppendNonZero(playerNode, "basedestroys", player.BaseDestroys);
					doc.AppendNonZero(playerNode, "basedenies", player.BaseDenies);
					doc.AppendNonZero(playerNode, "basedenied", player.BaseDenied);
					doc.AppendNonZero(playerNode, "yellowcards", player.YellowCards);
					doc.AppendNonZero(playerNode, "redcards", player.RedCards);
					doc.AppendNonZero(playerNode, "elim", player.IsEliminated ? 1 : 0);

					doc.AppendNode(playerNode, "colour", ((int)player.Colour) - 1);

					if (player.TermRecords != null)
					{
						XmlNode termsNode = doc.CreateElement("terms");
						playerNode.AppendChild(termsNode);

						foreach (TermRecord termRecord in player.TermRecords)
						{
							XmlNode termNode = doc.CreateElement("term");
							termsNode.AppendChild(termNode);

							doc.AppendNode(termNode, "type", termRecord.Type.ToString());
							doc.AppendNode(termNode, "time", termRecord.Time.ToString());
							doc.AppendNode(termNode, "value", termRecord.Value);
							doc.AppendNode(termNode, "reason", termRecord.Reason);
						}
					}
				}
			}

			if (!string.IsNullOrEmpty(fileName))
				FileName = fileName;

			Watcher.EnableRaisingEvents = false;
			try
			{
				if (File.Exists(this.fileName + "5Backup"))
					File.Delete(this.fileName + "5Backup");  // Delete old backup file, if any.
				if (File.Exists(this.fileName))
					File.Move(this.fileName, this.fileName + "5Backup");  // Rename the old league file before we save over it, by changing its extension to ".Torn5Backup".
				doc.Save(this.fileName);
			}
			finally
			{
				if (!string.IsNullOrEmpty(Watcher.Path))
					Watcher.EnableRaisingEvents = true;
			}
		}

		public override string ToString()
		{
			return Title ?? "league with blank title";
		}

		public Game Game(GamePlayer gamePlayer)
		{
			return AllGames.Find(g => g.AllPlayers().Contains(gamePlayer));
		}

		public Game Game(GameTeam gameTeam)
		{
			return AllGames.Find(g => g.Time == gameTeam.Time);
		}

		public Game Game(ServerGame serverGame)
		{
			return AllGames.Find(g => g.Time == serverGame.Time);
		}

		public GameTeam GameTeamFromPlayer(GamePlayer gamePlayer)
		{
			foreach (var game in AllGames)
			{
				GameTeam gameTeam = game.Teams.Find(gt => gt.Players.Contains(gamePlayer));
				if (gameTeam != null)
					return gameTeam;
			}

			return null;
		}

		public List<GameTeam> Played(LeagueTeam leagueTeam, bool includeSecret = true)
		{
			var played = new List<GameTeam>();
			foreach (var game in AllGames)
			{
				GameTeam gameTeam = includeSecret || !game.Secret ? game.Teams.Find(gt => gt.TeamId == leagueTeam.TeamId) : null;
				if (gameTeam != null)
					played.Add(gameTeam);
			}

			return played;
		}

		public List<GameTeam> Played(List<Game> games, LeagueTeam leagueTeam)
		{
			var played = new List<GameTeam>();
			foreach (var game in games)
			{
				GameTeam gameTeam = game.Teams.Find(gt => gt.TeamId == leagueTeam.TeamId);
				if (gameTeam != null)
					played.Add(gameTeam);
			}

			return played;
		}

		public List<GamePlayer> Played(LeaguePlayer leaguePlayer, bool includeSecret = true)
		{
			return Played(AllGames, leaguePlayer, includeSecret);
		}

		public static List<GamePlayer> Played(IEnumerable<Game> games, LeaguePlayer leaguePlayer, bool includeSecret)
		{
			var played = new List<GamePlayer>();

			if (leaguePlayer == null)
				return played;

			foreach (var game in games)
			{
				GamePlayer gamePlayer = includeSecret || !game.Secret ? game.Players().Find(gp => gp.PlayerId == leaguePlayer.Id) : null;
				if (gamePlayer != null)
					played.Add(gamePlayer);
			}

			return played;
		}

		public LeaguePlayer LeaguePlayer(GamePlayer gamePlayer)
		{
			return players.Find(p => p.Id == gamePlayer.PlayerId);
		}

		public LeaguePlayer LeaguePlayer(string id)
		{
			return players.Find(p => p.Id == id);
		}

		/// <summary>Return a player's Name.</summary>
		public string Alias(GamePlayer gamePlayer)
		{
			var leaguePlayer = LeaguePlayer(gamePlayer);
			return leaguePlayer == null ? (string)null : leaguePlayer.Name;
		}

		public LeagueTeam LeagueTeam(GameTeam gameTeam)
		{
			var result = gameTeam == null ? null : teams.Find(team => team.TeamId == gameTeam.TeamId);
			if (result != null)
				return result;

			foreach (var team in teams)
				if (team.TeamId == gameTeam.TeamId)
					return team;

			return null;
		}

		public LeagueTeam LeagueTeam(GamePlayer gamePlayer)
		{
			return LeagueTeam(GameTeamFromPlayer(gamePlayer));
		}

		public LeagueTeam LeagueTeam(LeaguePlayer leaguePlayer)
		{
			return teams.Find(t => t.Players.Contains(leaguePlayer));
		}

		public LeagueTeam LeagueTeam(int teamId)
		{
			return teams.Find(t => t.TeamId == teamId);
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
					tpcList.Add(new TeamPlayerCount(leagueTeam, leaguePlayer, Played(leagueTeam).Sum(gt => gt.Players.Count(gp => gp.PlayerId == leaguePlayer.Id))));

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
			return playerTeamList.Distinct().OrderBy(pt => pt.Value[0].Name).ThenBy(pt => pt.Key.Name).ToList();
		}

		int Plays(LeagueTeam leagueTeam, LeaguePlayer leaguePlayer)
		{
			return Played(leaguePlayer).Where(x => this.LeagueTeam(x.GameTeam(this)) == leagueTeam).Count();
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
			return leagueTeams.Any() ? string.Join(", ", leagueTeams) : "?";
		}

		public double AverageScore(LeagueTeam leagueTeam, bool includeSecret)
		{
			var p = Played(leagueTeam, includeSecret);
			return p.Any() ? p.Average(x => x.Score) : 0;
		}

		public double AveragePoints(LeagueTeam leagueTeam, bool includeSecret)
		{
			var p = Played(leagueTeam, includeSecret);
			return p.Any() ? p.Average(x => x.Points) : 0;
		}

		public double TotalPoints(LeagueTeam leagueTeam, bool includeSecret)
		{
			var p = Played(leagueTeam, includeSecret);
			return p.Any() ? p.Sum(x => x.Points) : 0;
		}

		public List<LeagueTeam> GetTeamLadder()
        {
			return teams.OrderByDescending(x => TotalPoints(x, false)).ThenByDescending(x => AverageScore(x,false)).ToList();
		}

		public List<LeagueTeam> GetTeamLadderScaled()
		{
			return teams.OrderByDescending(x => AveragePoints(x, false)).ThenByDescending(x => AverageScore(x, false)).ToList();
		}

		public double CalculateScore(GameTeam gameTeam)
		{
			if(IsAutoHandicap)
            {
				return CalculateAutoCappedScore(gameTeam);
            } else
            {
				double score = 0;
				foreach (var player in gameTeam.Players)
					score += (ZeroElimed && (player?.IsEliminated ?? false) && player.Score > 0) ? 0 : player.Score;

				score += gameTeam.Adjustment;

				if (gameTeam?.TermRecords != null)
				{
					foreach (TermRecord term in gameTeam?.TermRecords)
					{
						score += term.Value;
					}
				}


				LeagueTeam leagueTeam = LeagueTeam(gameTeam);
				return leagueTeam != null && leagueTeam.Handicap != null ? new Handicap(leagueTeam.Handicap.Value, HandicapStyle).Apply(score) : score;
			}			
		}

		public int CalulateTeamCap(GameTeam gameTeam)
        {

			decimal totalPoints = 0;
			int bonusCount = 0;
			decimal bonusTotal = 0;
			int penaltyCount = 0;
			decimal penaltyTotal = 0;
			foreach (var player in gameTeam.Players)
			{
				totalPoints += GetGradePoints(player.Grade);
				decimal bonus = GetGradeBonus(player.Grade);
				decimal penalty = GetGradePenalty(player.Grade);

				if (bonus < 0)
				{
					bonusCount++;
					if (bonusCount > 1)
						bonusTotal += bonus;
				}

				if (penalty > 0)
				{
					penaltyCount++;
					if (penaltyCount > 1)
						penaltyTotal += penalty;
				}
			}

			int missingPlayers = ExpectedTeamSize - gameTeam.Players.Count();

			decimal missingPenalty = missingPlayers > 0 ? MissingPlayerPenalty * missingPlayers : 0;

			totalPoints += bonusTotal + penaltyTotal + missingPenalty;

			return GetAutoHandicap((int)totalPoints);
		}

		public double CalculateAutoCappedScore(GameTeam gameTeam)
        {
			double score = 0;

			foreach (var player in gameTeam.Players)
				score += (ZeroElimed && (player?.IsEliminated ?? false) && player.Score > 0) ? 0 : player.Score;

			if (gameTeam?.TermRecords != null)
			{
				foreach (TermRecord term in gameTeam?.TermRecords)
				{
					score += term.Value;
				}
			}

			int cap = CalulateTeamCap(gameTeam);

			return new Handicap(cap, HandicapStyle.Percent).Apply(score);
		}

		public double CalculatePoints(GameTeam gameTeam, GroupPlayersBy groupPlayersBy)
		{
			Game game = Game(gameTeam);
			if (game != null)
			{
				List<GameTeam> teams = (groupPlayersBy == GroupPlayersBy.Lotr ?  // For Lord of the Ring we want just "teams" of this colour. For other modes, we want all teams. 
				                     game.Teams.Where(t => t.Colour == gameTeam.Colour) : game.Teams).OrderBy(x => -x.Score).ToList();

				List<GameTeam> nonElimedTeams = teams.Where(team =>
				{
					bool isSwept = true;
					foreach (var player in team.Players)
					{
						if (!player.IsEliminated)
						{
							isSwept = false;
						}
					}
					return !isSwept;

				}).ToList();

				List<GameTeam> relevantTeams = ZeroVps ? nonElimedTeams : teams;

				bool hasSwept = nonElimedTeams.Count() == 1 && nonElimedTeams[0].TeamId == gameTeam.TeamId;

				Console.WriteLine("Tie Break " + ZeroedTieBreak);

				if (HitsTieBreak)
                {
					relevantTeams = relevantTeams.OrderBy(x => -x.Score).ThenBy(x => -x.GetHitsBy()).ToList();
				} else if (ZeroedTieBreak)
                {
					relevantTeams = relevantTeams.OrderBy(x => -x.Score).ThenBy(x => -x.GetZeroedScore()).ToList();
				}
				var ties = relevantTeams.Where(t => (t.Score == gameTeam.Score) && ((HitsTieBreak && t.GetHitsBy() == gameTeam.GetHitsBy()) || !HitsTieBreak) && ((ZeroedTieBreak && t.GetZeroedScore() == gameTeam.GetZeroedScore()) || !ZeroedTieBreak));  // If there are ties, this list will contain the tied teams. If not, it will contain just this team.

				if (ties.Count() == 0) { return 0; }

				double totalPoints = 0;
				foreach (var team in ties)
				{
					int index = relevantTeams.IndexOf(team);

					bool isSwept = !nonElimedTeams.Contains(team);
					bool halfPoints = isSwept && HalfVps;

					if (victoryPoints.Valid(index))
						totalPoints += Math.Floor(halfPoints ? victoryPoints[index] / 2: victoryPoints[index]);
				}
				return totalPoints / ties.Count() + gameTeam.PointsAdjustment + (hasSwept ? SweepBonus : 0);  // If there are ties, average the victory points for all teams involved in the tie.
			}

			return 0;
		}
	}

	/// <summary>Encapsulate a single event from a LaserGameServer. A player-on-player hit may be two events, one from the perspective of each player.</summary>
	public class Event
	{
		public DateTime Time { get; set; }
		public string QRCode { get; set; }  // QR code from O-Zone of player that this record is about
		[JsonPropertyName("eventType")]
		public int Event_Type { get; set; }  // see below
		public string Event_Name { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Score { get; set; }  // points gained by shooter

		public string ServerPlayerId { get; set; }  // ID of player that this record is about
		public int ServerTeamId { get; set; }  // team of that player
		public string OtherPlayer { get; set; }  // ID of other involved player: for 0..13, this is the shootee; for 14..27 this is the shooter.
		public int OtherTeam { get; set; }  // team of other involved player

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int PointsLostByDeniee { get; set; }  // if hit is Event_Type = 1402 (base denier) or 1404 (base denied), shows points the shootee lost.

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int ShotsDenied { get; set; }  // if hit is Event_Type = 1402 or 1404, number of shots shootee had on the base when denied.

		// EventType (see P&C ng_event_types):
		//  0..6:   tagged foe (in various hit locations: laser, chest, left shoulder, right shoulder, left back shoulder (never used), right back shoulder (never used), back);
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
		//  1404: lose points for being denied. Note that Score field is incorrect for this event type -- use Result_Data_3, PointsLostByDeniee instead.

		public override string ToString()
		{
			return "Event Type " + Event_Type.ToString();
		}
	}

	/// <summary>Represents a game as stored on the laser game server.</summary>
	public class ServerGame: IComparable<ServerGame>
	{
		[JsonIgnore]
		public int? GameId { get; set; }
		public string Description { get; set; }
		[JsonPropertyName("Time")]
		public DateTime Time { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public DateTime EndTime { get; set; }
		[JsonIgnore]
		public League League { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		public virtual List<ServerPlayer> Players { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public bool InProgress { get; set; }
		[JsonIgnore]
		public bool OnServer { get; set; }
		[JsonPropertyName("Events")]
		public List<Event> Events { get; set; }
		[JsonIgnore]
		public virtual IEnumerable<Event> FilteredEvents { get => Events.Where(e => e.Event_Type != 35 && e.Event_Type != 1409); }

		public ServerGame()
		{
			Players = new List<ServerPlayer>();
			Events = new List<Event>();
		}

		public int CompareTo(ServerGame compareGame)
	    {
			return compareGame == null ? 1 : this.Time.CompareTo(compareGame.Time);
	    }

		public override string ToString()
		{
			return Time.ToString("yyyy/MM/dd HH:mm") + ": " + (OnServer ? "on server " : "") + Events.Count.ToString();
		}
	}
	/// <summary>Used to publish a ServerGame without its players or events.</summary>
	public class JsonGame: ServerGame
	{
		// Dummy properties, never used; just here to change JSON visibility:
		[JsonIgnore]
		public override List<ServerPlayer> Players { get; set; }
		[JsonIgnore]
		public override IEnumerable<Event> FilteredEvents { get; }
		public static JsonGame ShallowClone(ServerGame serverGame)
		{
			return new JsonGame
			{
				Description = serverGame.Description,
				Time = serverGame.Time,
				EndTime = serverGame.EndTime,
				InProgress = serverGame.InProgress
			};
		}
	}

	/// <summary>Represents a player as stored on the laser game server.</summary>
	public class ServerPlayer : GamePlayer
	{
		public string ServerPlayerId { get; set; }  // This is the under-the-hood PAndC table ID field.
		public int ServerTeamId { get; set; }   // Ditto. These two are only used by systems with in-game data available.
		public string Alias { get; set; }
		/// <summary>If this object is linked from a ListViewItem's Tag, list that ListViewItem here.</summary>
		public ListViewItem Item { get; set; }

		public bool IsPopulated()
		{
			return HitsBy > 0 || HitsOn > 0 || BaseHits > 0 || BaseDestroys > 0 || BaseDenies > 0 || BaseDenied > 0 || YellowCards > 0 || RedCards > 0;
		}

		public void Populate(List<Event> events)
		{		
			HitsBy = events.Count(x => x.ServerPlayerId == ServerPlayerId &&
								  (x.Event_Type <= 13 || x.Event_Type == 30 || x.Event_Type == 31 || x.Event_Type >= 37 && x.Event_Type <= 46));
			HitsOn = events.Count(x => x.ServerPlayerId == ServerPlayerId &&
								  (x.Event_Type >= 14 && x.Event_Type <= 27 || x.Event_Type == 33 || x.Event_Type == 34));
			BaseHits = events.Count(x => x.ServerPlayerId == ServerPlayerId && x.Event_Type == 30);
			BaseDestroys = events.Count(x => x.ServerPlayerId == ServerPlayerId && x.Event_Type == 31);
			BaseDenies = events.FindAll(x => x.ServerPlayerId == ServerPlayerId && (x.Event_Type == 1401 || x.Event_Type == 1402)).Sum(x => x.ShotsDenied);
			BaseDenied = events.FindAll(x => x.ServerPlayerId == ServerPlayerId && (x.Event_Type == 1404 || x.Event_Type == 1404)).Sum(x => x.ShotsDenied);
			PopulateTerms(events);
		}

		public void PopulateTerms(List<Event> events)
        {
			YellowCards = 0;
			RedCards = 0;
			TermRecords = null;

			foreach (Event e in events)
			{
				if (e.ServerPlayerId == ServerPlayerId)
				{
					if (e.Event_Type == 28)
					{
						YellowCards++;
						TermRecord termRecord = new TermRecord(TermType.Yellow, e.Time, e.Score);
						AddTermRecord(termRecord);
					}
					if (e.Event_Type == 29)
					{
						RedCards++;
						TermRecord termRecord = new TermRecord(TermType.Red, e.Time, e.Score);
						AddTermRecord(termRecord);
					}
				}
			}
		}

		public override string ToString()
		{
			return "ServerPlayer " + Alias + "; " + PlayerId;
		}

		public string GetFormattedAlias()
		{
			return (IsEliminated ? "💀  " : "") + (RedCards > 0 ? (RedCards + "R ") : "") + (YellowCards > 0 ? (YellowCards + "Y ") : "") + Alias;
		}
	}
}
