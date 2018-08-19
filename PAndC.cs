using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
						heliosType = reader.GetInt32("Int_Data_1");
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
					game.GameId = reader.GetInt32("Game_ID");
					game.Description = reader.GetString("Description");
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

			game.Players = new List<ServerPlayer>();

			string sql = "SELECT Player_ID, Player_Team_ID, SUM(Score) AS Score, Pack_Name, QRCode AS Button_ID, M.Alias " +
                "FROM ng_player_event_log EL " +
                "LEFT JOIN ng_player_stats S ON EL.Game_ID = S.Game_ID AND EL.Player_ID = S.Pack_ID " +
                "LEFT JOIN members M ON S.Member_ID = M.Member_ID " +
				"WHERE EL.Game_ID = " + game.GameId.ToString() +
                " GROUP BY Player_ID " +
                "ORDER BY Score DESC";
			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerPlayer player = new ServerPlayer();
					player.PandCPlayerId = reader.GetInt32("Player_ID");
					player.PandCPlayerTeamId = reader.GetInt32("Player_Team_ID");
					player.Colour = (Colour)(player.PandCPlayerTeamId + 1);
					player.Score = reader.GetInt32("Score");
					player.PackName = reader.GetString("Pack_Name");
					player.PlayerId = reader.GetString("Button_ID");
					player.Alias = reader.GetString("Alias");
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
	}
}
