using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Torn
{
	/// <summary>
	/// This represents a Laserforce lasergame database server.
	/// You can ask it for scores for players, but not for individual events in game, nor time remaining.
	/// </summary>
	public class Laserforce: LaserGameServer
	{
		SqlConnection connection;
		public int GamesLimit { get; set; }
		public int PlayersLimit { get; set; }
		public string LogFolder { get; set; }

		public Laserforce(string server = "")
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
					ServerGame game = new ServerGame();
					game.GameId = reader.GetInt32(0);
					game.Time = reader.GetDateTime(1);
					game.EndTime = reader.GetDateTime(2);
					game.Description = reader.GetString(3);
					game.OnServer = true;
					games.Add(game);
				}
			}
			finally
			{
				reader.Close();
			}
			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!Connected)
				return;

			game.Players = new List<ServerPlayer>();

			string sql = "SELECT MAT.colourTeam AS [Colour], MP.score, U.[desc] AS [Pack_Name], " +
						 "cast(C.region as varchar) + '-' + cast(C.site as varchar) + '-' + cast(Member.id as varchar) as [Player_ID], " +
						 "member.codename AS [Alias], MAT.[desc] AS [Team] " +
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
					ServerPlayer player = new ServerPlayer();
					player.Colour = ColourExtensions.ToColour(reader.GetInt32(0));
					player.Score = reader.GetInt32(1);
					player.Pack = reader.GetString(2);
					if (!reader.IsDBNull(3))
						player.PlayerId = reader.GetString(3);
					if (!reader.IsDBNull(4))
						player.Alias = reader.GetString(4);
					player.ServerPlayerId = player.Alias;
					game.Players.Add(player);
				}
			}
			finally
			{
				reader.Close();
			}
			
			if (string.IsNullOrEmpty(LogFolder))
				return;

			var files = new DirectoryInfo(LogFolder).GetFiles(game.Time.ToString("HHmm") + " - *.txt");
			if (files.Length > 0)
			{
				var file = files[0].OpenText();
				game.Events.Clear();
				while (!file.EndOfStream)
				{
					// Line format: number of milliseconds elapsed in game [tab] event code [tab] teamcolour:playeralias [tab] event description [tab] teamcolour:playeralias
					
					var line = file.ReadLine().Split('\t');
	
					var oneEvent = new Event();

					oneEvent.Time = game.Time.AddMilliseconds(int.Parse(line[0]));

					string eventType = line[1];
					if (eventType.StartsWith("01")) continue;

					oneEvent.Event_Type = ParseEventType(line[1]);
					oneEvent.Score = EventToScore(oneEvent.Event_Type);

					SplitPlayer(line[2], out oneEvent.ServerTeamId, out oneEvent.ServerPlayerId);
					if (line.Length > 4)
						SplitPlayer(line[4], out oneEvent.OtherTeam, out oneEvent.OtherPlayer);

					if (eventType.StartsWith("0B"))
						oneEvent.ShotsDenied = 1;

					game.Events.Add(oneEvent);
					
					if (eventType.StartsWith("02"))  // The event is a player hitting another player. Let's create a complementary event to record that same hit from the other player's perspective.
					{
						var twoEvent = new Event();
						twoEvent.Time = oneEvent.Time;
						twoEvent.Event_Type = oneEvent.Event_Type + 14;
						twoEvent.Score = -40;

						twoEvent.ServerTeamId = oneEvent.OtherTeam;
						twoEvent.ServerPlayerId = oneEvent.OtherPlayer;
						twoEvent.OtherTeam = oneEvent.ServerTeamId;
						twoEvent.OtherPlayer = oneEvent.ServerPlayerId;

						game.Events.Add(twoEvent);
					}
				}
			}
		}

		void SplitPlayer(string input, out int team, out string player)
		{
			var arr = input.Split(':');
			team = int.Parse(arr[0]);
			player = arr[1];
		}

		int ParseEventType(string s)
		{
			switch (s) {
				case "0100": case "0101": return 36; // 0100: game start. 0101: game end.
				case "0203": return 30;  // player tags base.
				case "0204": return 31;  // player destroys base.
				case "0206": return 0;   // player tags foe.
				case "0208": return 7;   // player tags ally.
				case "0500": return 36;  // player is reloaded.
				case "0600": return 28;  // termination/penalty
				case "0B01":	         // 0B01: player denies player (number of hits worth of deny not specified). 0B01 always immediately follows a 0206 or 0208 with the same players. 
				case "0B02": return 1404; // 0B02: player denies player (number of hits worth of deny not specified). 0B02 occurs several seconds after the 0206 in which the player tags the shooter.
				default:     return 36;
			}
		}
		
		int EventToScore(int eventType)
		{
			switch (eventType) {
				case 0:  return 150;
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
	}
