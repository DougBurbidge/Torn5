using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Torn
{
	/// <summary>
	/// This represents a P&C Micro's lasergame database server. 
	/// You can ask it how much time is remaining in the current game.
	/// </summary>
	public class PAndC: LaserGameServer
	{
		MySqlConnection connection;
		int heliosType;  // This is the database schema version.

		public PAndC(string server)
		{
			try
			{
				connection = new MySqlConnection("server=" + server + ";user=root;database=ng_system;port=3306;password=password");
				connection.Open();
				Connected = true;

				var cmd = new MySqlCommand("SELECT Int_Data_1 FROM ng_registry WHERE Registry_ID = 0", connection);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
						heliosType = GetInt(reader, "Int_Data_1");
				}
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

		public override TimeSpan GameTimeElapsed()
		{
			if (!Connected)
				return TimeSpan.Zero;
			try
			{
				string sql = "SELECT Event_Type, Time_Logged, CURRENT_TIMESTAMP FROM ng_game_log ORDER BY Time_Logged DESC";
				MySqlCommand cmd = new MySqlCommand(sql, connection);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						if (reader.GetUInt32("Event_Type") == 0)  // 0 is 'Game Started'.
							return reader.GetDateTime("CURRENT_TIMESTAMP") - reader.GetDateTime("Time_Logged");
						else
							return TimeSpan.Zero;
					}
					else
						return TimeSpan.Zero;
				}
			}
			catch
			{
				return TimeSpan.Zero; // If we throw here it's probably because the server went away (e.g. due to server power-saving, network breakage, etc). MySql.Data.MySqlClient.MySqlException
			}
		}

		public override List<ServerGame> GetGames()
		{
			List<ServerGame> games = new List<ServerGame>();

			if (!Connected)
				return games;

			string sql = "SELECT S.Game_ID, S.Start_Time, P.Profile_Description AS Description " +
                         "FROM ng_game_stats S " +
                         "JOIN ng_profiles P ON S.Profile_ID = P.Profile_ID " +
                         "ORDER BY Start_Time";
			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerGame game = new ServerGame();
					game.GameId = GetInt(reader, "Game_ID");
					game.Description = GetString(reader, "Description");
					game.Time = reader.GetDateTime("Start_Time");
					game.OnServer = true;
					games.Add(game);
				}
			}

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!Connected)
				return;

			game.Events.Clear();
			string sql = "SELECT Time_Logged, Player_ID, Player_Team_ID, Event_Type, Score, Result_Data_1, Result_Data_2, Result_Data_3, Result_Data_4 " +
				"FROM ng_player_event_log " +
				"WHERE Game_ID = " + game.GameId.ToString();
			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var oneEvent = new Event();
					oneEvent.Time = GetDateTime(reader, "Time_Logged");
					oneEvent.PandCPlayerId = GetInt(reader, "Player_ID");
					oneEvent.PandCPlayerTeamId = GetInt(reader, "Player_Team_ID");
					oneEvent.Event_Type = GetInt(reader, "Event_Type");
					oneEvent.Score = GetInt(reader, "Score");
					oneEvent.HitPlayer = GetInt(reader, "Result_Data_1");
					oneEvent.HitTeam = GetInt(reader, "Result_Data_2");
					oneEvent.PointsLostByDeniee = GetInt(reader, "Result_Data_3");
					oneEvent.ShotsDenied = GetInt(reader, "Result_Data_4");
					game.Events.Add(oneEvent);
				}
			}

			game.Players = new List<ServerPlayer>();

			sql = "SELECT Player_ID, Player_Team_ID, SUM(Score) AS Score, Pack_Name, QRCode AS Button_ID, M.Alias " +
                "FROM ng_player_event_log EL " +
                "LEFT JOIN ng_player_stats S ON EL.Game_ID = S.Game_ID AND EL.Player_ID = S.Pack_ID " +
                "LEFT JOIN members M ON S.Member_ID = M.Member_ID " +
				"WHERE EL.Game_ID = " + game.GameId.ToString() +
                " GROUP BY Player_ID " +
                "ORDER BY Score DESC";
			cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerPlayer player = new ServerPlayer();

					player.PandCPlayerId = GetInt(reader, "Player_ID");
					player.PandCPlayerTeamId = GetInt(reader, "Player_Team_ID");
					player.Colour = (Colour)(player.PandCPlayerTeamId + 1);
					player.Score = GetInt(reader, "Score");
					player.PackName = GetString(reader, "Pack_Name");
					player.PlayerId = GetString(reader, "Button_ID");
					player.Alias = GetString(reader, "Alias");

					player.HitsBy = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && 
					                               (x.Event_Type <= 13 || x.Event_Type == 30 || x.Event_Type == 31 || x.Event_Type >= 37 && x.Event_Type <= 46));
					player.HitsOn = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && 
					                               (x.Event_Type >= 14 && x.Event_Type <= 27 || x.Event_Type == 33 || x.Event_Type == 34));
					player.BaseHits = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 30);
					player.BaseDestroys = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 31);
					player.BaseDenies = game.Events.FindAll(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 1402).Sum(x => x.ShotsDenied);
					player.BaseDenied = game.Events.FindAll(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 1404).Sum(x => x.ShotsDenied);
					player.YellowCards = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 28);
					player.RedCards = game.Events.Count(x => x.PandCPlayerId == player.PandCPlayerId && x.Event_Type == 29);

					game.Players.Add(player);
				}
			}
		}

		public override DbDataReader GetPlayers(string mask)
		{
//			Acacia: "SELECT Player_Alias AS Alias, First_Name + ' ' + Last_Name AS Name, User_ID FROM MEMBERS WHERE User_ID <> ''"
//			Nexus: "SELECT Alias AS Alias, '' AS Name, Button_ID AS User_ID FROM members"

			// Less than 47 is the old schema, with QRCode in demographics.customer table.
			string sql = heliosType < 47 ? 
				"SELECT M.Alias AS Alias, C.First_Name + ' ' + C.Last_Name AS Name, C.QRCode AS User_ID " +
				"FROM members M " +
				"LEFT JOIN demographics.customers C on C.Customer_ID = M.member_ID " +
				"WHERE SUBSTRING(C.QRCode, 1, 5) <> '00005' AND (M.Alias LIKE @mask OR C.First_Name LIKE @mask OR C.Last_Name LIKE @mask) " +
				"ORDER BY M.Alias" :
			// 47 or greater is the new schema, with QRCode in members table.
				"SELECT Alias AS Alias, '' AS Name, QRCode AS User_ID " +
				"FROM members M " +
				"WHERE SUBSTRING(M.QRCode, 1, 5) <> '00005' AND Alias LIKE @mask ORDER BY Alias";

			using (var cmd = new MySqlCommand(sql, connection))
			{
				cmd.Parameters.AddWithValue("@mask", "%" + mask + "%");
			    return cmd.ExecuteReader();
			}
		}

		public override bool HasNames() 
		{
			return heliosType < 47;
		}

		DateTime GetDateTime(MySqlDataReader reader, string column)
		{
			int i = reader.GetOrdinal(column);
			return reader.IsDBNull(i) ? default(DateTime) : reader.GetDateTime(i);
		}

		int GetInt(MySqlDataReader reader, string column)
		{
			int i = reader.GetOrdinal(column);
			return reader.IsDBNull(i) ? -1 : reader.GetInt32(i);
		}

		string GetString(MySqlDataReader reader, string column)
		{
			int i = reader.GetOrdinal(column);
			return reader.IsDBNull(i) ? null : reader.GetString(i);
		}
	}
}
