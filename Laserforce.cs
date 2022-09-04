using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Torn
{
	/// <summary>
	/// This represents a Laserforce lasergame database server.
	/// You can ask it for scores for players, but not for individual events in game, nor time remaining.
	/// </summary>
	public class Laserforce : LaserGameServer
	{
		SqlConnection connection;
		public int GamesLimit { get; set; }
		public int PlayersLimit { get; set; }
		public string LogFolder { get; set; }

		public Laserforce()
		{
			GamesLimit = 1000;
			PlayersLimit = 1000;
		}

		public void Connect(string server = "")
		{
			if (string.IsNullOrEmpty(server))
				server = "lf-main\\lf6";
			try
			{
				connection = new SqlConnection("Data Source=" + server + ";Database=Laserforce;Trusted_Connection=True");
				connection.Open();
				Connected = true;
			}
			catch
			{
				Connected = false;
				throw;
			}
		}

		public void Connect(string server, string sqlUserId, string sqlPassword)
		{
			try
			{
				connection = new SqlConnection("Data Source=" + server + ";Database=Laserforce;User Id=" + sqlUserId + ";Password=" + sqlPassword);
				Console.WriteLine("Data Source=" + server + ";Database=Laserforce;User Id=" + sqlUserId + ";Password=" + sqlPassword);
				connection.Open();
				Connected = true;
			}
			catch
			{
				Connected = false;
				throw;
			}
		}

		public override void Dispose()
		{
			if (Connected)
			{
				connection.Close();
				Connected = false;
			}
		}

		public override List<ServerGame> GetGames()
		{
			List<ServerGame> games = new List<ServerGame>();

			if (!Connected)
				return games;

			string sql = "SELECT TOP " + GamesLimit.ToString() +
						 " M.ref AS [Game_ID], M.start AS [Start_Time], M.[end] AS [Finish_Time], COALESCE(MT.desc1, MT.desc0, MG.[desc]) AS [Description] " +
						 "FROM Mission M " +
						 "LEFT JOIN MissionGroup MG ON MG.ref = M.[group] " +
						 "LEFT JOIN MissionType MT ON MT.ref = M.[type] " +
						 "ORDER BY M.start DESC";
			var cmd = new SqlCommand(sql, connection);
			SqlDataReader reader = cmd.ExecuteReader();

			try
			{
				while (reader.Read())
				{
					games.Add(new ServerGame
					{
						GameId = reader.GetInt32(0),
						Time = reader.GetDateTime(1),
						EndTime = reader.GetDateTime(2),
						Description = reader.GetString(3),
						OnServer = true
					});
				}
			}
			finally
			{
				reader.Close();
			}
			Console.WriteLine(games.Count);

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!Connected)
				return;

			game.Players = new List<ServerPlayer>();

			string sql = "SELECT MAT.colourTeam AS [Colour], MP.score, U.[desc] AS [Pack_Name], " +
						 "cast(C.region as varchar) + '-' + cast(C.site as varchar) + '-' + cast(Member.id as varchar) as [Player_ID], " +
						 "Member.codename AS [Alias], MAT.[desc] AS [Team] " +
						 "FROM MissionPlayer MP " +
						 "LEFT JOIN Unit U ON U.ref = MP.unit " +
						 "LEFT JOIN Member ON Member.ref = MP.member " +
						 "LEFT JOIN Centre C ON C.ref = Member.centre " +
						 "LEFT JOIN Mission ON Mission.ref = MP.mission " +
						 "LEFT JOIN MissionType MT ON MT.ref = Mission.type " +
						 "LEFT JOIN MissionAlignmentTeam MAT ON MAT.alignment = MT.alignment AND MAT.seq = MP.team " +
						 "WHERE Mission.ref = " + game.GameId.ToString() + " AND unitType = 0 " +
						 "ORDER BY score DESC";
			var cmd = new SqlCommand(sql, connection);
			SqlDataReader reader = cmd.ExecuteReader();

			try
			{
				while (reader.Read())
				{
					ServerPlayer player = new ServerPlayer
					{
						Colour = ColourExtensions.ToColour(reader.GetInt32(0)),
						Score = reader.GetInt32(1),
						Pack = reader.GetString(2)
					};
					if (!reader.IsDBNull(3))
						player.PlayerId = reader.GetString(3);
					if (!reader.IsDBNull(4))
						player.Alias = reader.GetString(4);
					player.ServerPlayerId = player.Pack;
					game.Players.Add(player);
				}
			}
			finally
			{
				reader.Close();
			}

			if (string.IsNullOrEmpty(LogFolder) || !Directory.Exists(LogFolder))
				return;
			Console.WriteLine(game.Time.ToString("*" + "yyyyMMddHHmm") + " - *.tdf");
			var files = new DirectoryInfo(LogFolder).GetFiles("*" + game.Time.ToString("yyyyMMddHHmm") + " - *.tdf");
			if (files.Length > 0)
			{
				Console.WriteLine("HERE");
				var file = files[0].OpenText();
				game.Events.Clear();
				List<string> lines = new List<string>();

				while (!file.EndOfStream)
				{
					lines.Add(file.ReadLine());
				}

				List<List<string>> entities = new List<List<string>>();

				foreach (string l in lines)
				{
					List<string> splitLine = l.Split('\t').ToList();
					// 3 time id type alias team level category pack
					if (splitLine[0] == "3")
                    {
						entities.Add(splitLine);

						if (splitLine[3] == "player") {
							int indexOfPlayer = game.Players.FindIndex(p => p.Pack == splitLine[8]);
							if (indexOfPlayer >= 0)
							{
								game.Players[indexOfPlayer].ServerPlayerId = splitLine[2];
								game.Players[indexOfPlayer].ServerTeamId = Int32.Parse(splitLine[5]);
							}
						}
					}
				}

					foreach (string l in lines)
                {
					List<string> detailEvent = l.Split('\t').ToList();

					// 4 event time type entity desc otherEntity
					if (detailEvent[0] == "4")
					{
						Event oneEvent = new Event
						{
							Time = game.Time.AddMilliseconds(int.Parse(detailEvent[1]))
						};

						int indexOfEvent = lines.IndexOf(l);

						oneEvent.Event_Type = ParseEventType(detailEvent[2]);

						// tag player
						if ( detailEvent[2] == "0206" || detailEvent[2] == "0208")
                        {
							Event otherEvent = new Event
							{
								Time = oneEvent.Time
                            };

							if (detailEvent[2] == "0206")
                            {
								oneEvent.Event_Name = "Tag Foe";
								otherEvent.Event_Type = 14;
								otherEvent.Event_Name = "Tagged by Foe";

							}
							else
                            {
								oneEvent.Event_Name = "Tag Ally";
								otherEvent.Event_Type = 21;
								otherEvent.Event_Name = "Tagged by Ally";
							}

							oneEvent.ServerPlayerId = detailEvent[3];
							oneEvent.OtherPlayer = detailEvent[5];

							otherEvent.OtherPlayer = detailEvent[3];
							otherEvent.ServerPlayerId = detailEvent[5];

							List<string> playerEntity = entities.Find(e => e[2] == oneEvent.ServerPlayerId);
							List<string> otherPlayerEntity = entities.Find(e => e[2] == oneEvent.OtherPlayer);

							List<string> playerScoreEvent = lines[indexOfEvent - 2].Split('\t').ToList();
							List<string> otherPlayerScoreEvent = lines[indexOfEvent - 1].Split('\t').ToList();

							int playerScore = Int32.Parse(playerScoreEvent[4]);
							int otherPlayerScore = Int32.Parse(otherPlayerScoreEvent[4]);

							ServerPlayer player = game.Players.Find(p => p.ServerPlayerId == oneEvent.ServerPlayerId);
							ServerPlayer otherPlayer = game.Players.Find(p => p.ServerPlayerId == oneEvent.OtherPlayer);

							oneEvent.Score = playerScore;
							oneEvent.ServerTeamId = player.ServerTeamId;
							oneEvent.OtherTeam = otherPlayer.ServerTeamId;

							otherEvent.Score = otherPlayerScore;
							otherEvent.ServerTeamId = otherPlayer.ServerTeamId;
							otherEvent.OtherTeam = player.ServerTeamId;

							game.Events.Add(oneEvent);
							game.Events.Add(otherEvent);

							player.HitsBy++;
							otherPlayer.HitsOn++;

						}

						// tags base
						if(detailEvent[2] == "0203" || detailEvent[2] == "0204")
                        {
							List<string> scoreEvent = lines[indexOfEvent - 1].Split('\t').ToList();

							oneEvent.ServerPlayerId = detailEvent[3];

							ServerPlayer player = game.Players.Find(p => p.ServerPlayerId == oneEvent.ServerPlayerId);
							List<string> baseEntity = entities.Find(e => e[2] == detailEvent[5]);

							oneEvent.OtherPlayer = baseEntity[4];
							oneEvent.ServerTeamId = player.ServerTeamId;
							oneEvent.OtherTeam = Int32.Parse(baseEntity[5]);
							oneEvent.Score = Int32.Parse(scoreEvent[4]);

							if (detailEvent[2] == "0203")
							{
								oneEvent.Event_Name = "Tag Base";
								player.BaseHits++;
							}
							else
							{
								oneEvent.Event_Name = "Destroy Base";
								player.BaseDestroys++;
							}

							game.Events.Add(oneEvent);
						}

					}
				}
			}
		}

		(int, string) SplitPlayer(string input)
		{
			var arr = input.Split(':');
			return (int.Parse(arr[0]), arr[1]);
		}

		int ParseEventType(string s)
		{
			switch (s)
			{
				case "0100": case "0101": return 36; // 0100: game start. 0101: game end.
				case "0203": return 30;  // player tags base.
				case "0204": return 31;  // player destroys base.
				case "0206": return 0;   // player tags foe.
				case "0208": return 7;   // player tags ally.
				case "0500": return 36;  // player is reloaded.
				case "0600": return 28;  // termination/penalty
				case "0B01":             // 0B01: player denies player (number of hits worth of deny not specified). 0B01 always immediately follows a 0206 or 0208 with the same players. 
				case "0B02": return 1404; // 0B02: player denies player (number of hits worth of deny not specified). 0B02 occurs several seconds after the 0206 in which the player tags the shooter.
				default: return 36;
			}
		}

		int EventToScore(int eventType)
		{
			switch (eventType)
			{
				case 0: return 150;
				case 14: return -150;
				case 28: return -1000;
				case 30: return -500;
				case 31: return 4001;
				case 1404: return 375;  // This should be 250 per hits worth of deny. But we don't know how many hits worth of deny we have here.
				default: return 0;
			}
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			string sql = "SELECT TOP " + PlayersLimit.ToString() +
						"M.codename AS [Alias], M.givenNames + ' ' + M.surname AS [Name], " +
						"cast(C.region as varchar) + ''-'' + cast(C.site as varchar) + ''-'' + cast(M.id as varchar) as [ID] " +
						"FROM Member M " +
						"LEFT JOIN Centre C ON C.ref = M.centre " +
						"WHERE M.surname LIKE @mask OR M.givenNames LIKE @mask OR M.codename LIKE '%' + @mask " +
						"ORDER BY M.codename, [ID]";
			using (SqlCommand cmd = new SqlCommand(sql, connection))
			{
				cmd.Parameters.AddWithValue("@mask", mask + "%");
				return ReadPlayers(cmd.ExecuteReader());
			}
		}

		public override List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players)
		{
			return GetPlayers(mask);
		}
	}
}