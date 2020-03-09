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
		protected int heliosType;  // This is the database schema version.
		protected string _server;

		protected PAndC() {}

		public PAndC(string server)
		{
			try
			{
				_server = server;
				Connect();

				var cmd = new MySqlCommand("SELECT Int_Data_1 FROM ng_registry WHERE Registry_ID = 0", connection);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
						heliosType = GetInt(reader, "Int_Data_1");
				}
			}
			catch
			{
				connected = false;
				throw;
			}
		}

		protected override bool GetConnected() { return connected && connection != null && connection.State == ConnectionState.Open; }

		public override void Dispose()
		{
			if (connected)
			{
				connection.Close();
				connected = false;
			}
		}

		public override TimeSpan GameTimeElapsed()
		{
			if (!EnsureConnected())
				return TimeSpan.Zero;
			try
			{
				string sql = "SELECT Event_Type, Time_Logged, CURRENT_TIMESTAMP FROM ng_game_log ORDER BY Time_Logged DESC LIMIT 1";
				MySqlCommand cmd = new MySqlCommand(sql, connection);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						if (reader.GetUInt32("Event_Type") == 0)  // 0 is 'Game Started'.
							return GetDateTime(reader, "CURRENT_TIMESTAMP") - GetDateTime(reader, "Time_Logged");
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
			GetMoreGames(games);
			return games;
		}

		public override void GetMoreGames(List<ServerGame> games)
		{
			string where = games.Count == 0 ? "" : "WHERE S.Start_Time > \"" + games.Last().EndTime.ToString("YYYY-MM-DD HH:mm:ss") + "\"";

			string sql = "SELECT S.Game_ID, S.Start_Time, S.Finish_Time, P.Profile_Description AS Description " +
			             "FROM ng_game_stats S " +
			             "JOIN ng_profiles P ON S.Profile_ID = P.Profile_ID " +
			             where +
			             " ORDER BY Start_Time";
			FillGames(sql, games);
		}

		void FillGames(string sql, List<ServerGame> games)
		{
			if (!EnsureConnected())
				return;

			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerGame game = new ServerGame();
					game.GameId = GetInt(reader, "Game_ID");
					game.Description = GetString(reader, "Description");
					game.Time = GetDateTime(reader, "Start_Time");
					game.EndTime = GetDateTime(reader, "Finish_Time");
					game.InProgress = game.EndTime == default(DateTime);
					game.OnServer = true;
					games.Add(game);
				}
			}
		}

		protected virtual string GameDetailSql(int? gameId)
		{
			return "SELECT Player_ID, Player_Team_ID, SUM(Score) AS Score, Pack_Name, QRCode AS Button_ID, M.Alias " +
			    "FROM ng_player_event_log EL " +
			    "LEFT JOIN ng_player_stats S ON EL.Game_ID = S.Game_ID AND EL.Player_ID = S.Pack_ID " +
			    "LEFT JOIN members M ON S.Member_ID = M.Member_ID " +
			    "WHERE EL.Game_ID = " + gameId.ToString() +
			    " GROUP BY Player_ID " +
			    "ORDER BY Score DESC";
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!EnsureConnected() || !game.GameId.HasValue)
				return;

			// Get game end time. Determine if game is in progress.
			string sql = "SELECT S.Finish_Time " +
                         "FROM ng_game_stats S " +
                         "WHERE S.Start_Time = \"" + game.Time.ToString("yyyy-MM-dd HH:mm:ss") + "\"";
			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					game.EndTime = GetDateTime(reader, "Finish_Time");
					game.InProgress = game.EndTime == default(DateTime);
					game.OnServer = true;
				}
			}

			sql = "SELECT Time_Logged, Player_ID, Player_Team_ID, Event_Type, Score, Result_Data_1, Result_Data_2, Result_Data_3, Result_Data_4 " +
				"FROM ng_player_event_log " +
				"WHERE Game_ID = " + game.GameId.ToString();
			cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.HasRows)
					game.Events.Clear();

				while (reader.Read())
				{
					var oneEvent = new Event();
					oneEvent.Time = GetDateTime(reader, "Time_Logged");
					oneEvent.ServerPlayerId = GetString(reader, "Player_ID");
					oneEvent.ServerTeamId = GetInt(reader, "Player_Team_ID");
					oneEvent.Event_Type = GetInt(reader, "Event_Type");
					oneEvent.Score = GetInt(reader, "Score");
					oneEvent.OtherPlayer = GetString(reader, "Result_Data_1");
					oneEvent.OtherTeam = oneEvent.Event_Type == 30 || oneEvent.Event_Type == 31 ? GetInt(reader, "Result_Data_1") : GetInt(reader, "Result_Data_2");
					oneEvent.PointsLostByDeniee = GetInt(reader, "Result_Data_3");
					oneEvent.ShotsDenied = GetInt(reader, "Result_Data_4");
					game.Events.Add(oneEvent);
				}
			}

			game.Players.Clear();

			cmd = new MySqlCommand(GameDetailSql(game.GameId), connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerPlayer player = new ServerPlayer();

					player.ServerPlayerId = GetString(reader, "Player_ID");
					player.ServerTeamId = GetInt(reader, "Player_Team_ID");

					if (0 <= player.ServerTeamId && player.ServerTeamId < 8)
						player.Colour = (Colour)(player.ServerTeamId + 1);
					else
						player.Colour = Colour.None;

					player.Score = GetInt(reader, "Score");
					player.Pack = GetString(reader, "Pack_Name");
					player.PlayerId = GetString(reader, "Button_ID");
					player.Alias = GetString(reader, "Alias");

					player.HitsBy = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && 
					                               (x.Event_Type <= 13 || x.Event_Type == 30 || x.Event_Type == 31 || x.Event_Type >= 37 && x.Event_Type <= 46));
					player.HitsOn = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && 
					                               (x.Event_Type >= 14 && x.Event_Type <= 27 || x.Event_Type == 33 || x.Event_Type == 34));
					player.BaseHits = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && x.Event_Type == 30);
					player.BaseDestroys = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && x.Event_Type == 31);
					player.BaseDenies = game.Events.FindAll(x => x.ServerPlayerId == player.ServerPlayerId && (x.Event_Type == 1401 || x.Event_Type == 1402)).Sum(x => x.ShotsDenied);
					player.BaseDenied = game.Events.FindAll(x => x.ServerPlayerId == player.ServerPlayerId && (x.Event_Type == 1404 || x.Event_Type == 1404)).Sum(x => x.ShotsDenied);
					player.YellowCards = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && x.Event_Type == 28);
					player.RedCards = game.Events.Count(x => x.ServerPlayerId == player.ServerPlayerId && x.Event_Type == 29);

					game.Players.Add(player);
				}
			}
		}

		protected virtual string PlayersSql()
		{
			return heliosType < 47 ? 
			// Less than 47 is the old schema, with QRCode in demographics.customer table.
				"SELECT M.Alias AS Alias, C.First_Name + ' ' + C.Last_Name AS Name, C.QRCode AS User_ID " +
				"FROM members M " +
				"LEFT JOIN demographics.customers C on C.Customer_ID = M.member_ID " +
				"WHERE SUBSTRING(C.QRCode, 1, 5) <> '00005' AND (M.Alias LIKE @mask OR C.First_Name LIKE @mask OR C.Last_Name LIKE @mask) " +
				"ORDER BY M.Alias" :
			// 47 or greater is the new schema, with QRCode in members table.
				"SELECT Alias AS Alias, '' AS Name, QRCode AS User_ID " +
				"FROM members M " +
				"WHERE SUBSTRING(M.QRCode, 1, 5) <> '00005' AND Alias LIKE @mask ORDER BY Alias LIMIT 1000";
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			if (!EnsureConnected())
				return null;

			using (var cmd = new MySqlCommand(PlayersSql(), connection))
			{
				cmd.Parameters.AddWithValue("@mask", "%" + mask + "%");
				return ReadPlayers(cmd.ExecuteReader());
			}
		}

		public override bool HasNames() 
		{
			return heliosType < 47;
		}

		protected void Connect()
		{
			if (connection != null)
				connection.Close();

			connection = new MySqlConnection("server=" + _server + ";user=root;database=ng_system;port=3306;password=password;Convert Zero Datetime=True");
			try
			{
				connection.Open();
				connected = true;
			}
			catch
			{
				connected = false;
			}
		}

		bool EnsureConnected()
		{
			if (!Connected)
				Connect();
			return Connected;
		}

		DateTime GetDateTime(MySqlDataReader reader, string column)
		{
			try {
				int i = reader.GetOrdinal(column);
				return reader.IsDBNull(i) ? default(DateTime) : reader.GetDateTime(i);
			} catch (Exception) {
				return default(DateTime);
			}
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

	public class PAndCNexusWithIButton: PAndC
	{
		public PAndCNexusWithIButton(string server)
		{
			try
			{
				_server = server;
				Connect();
				heliosType = -1;
			}
			catch
			{
				connected = false;
				throw;
			}
		}

		override protected string GameDetailSql(int? gameId)
		{
			return "SELECT Player_ID, Player_Team_ID, SUM(Score) AS Score, Pack_Name, Button_ID, Alias " +
				"FROM ng_player_event_log EL " +
				"JOIN ng_player_stats S ON EL.Game_ID = S.Game_ID AND EL.Player_ID = S.Pack_ID " +
				"LEFT JOIN members M ON S.Member_ID = M.Member_ID " +
				"WHERE EL.Game_ID = " + gameId.ToString() +
				" GROUP BY Player_ID " +
				"ORDER BY Score DESC";
		}

		override protected string PlayersSql()
		{
			return "SELECT TRIM(Alias) AS Alias, '' AS Name, Button_ID AS User_ID FROM members " +
				"WHERE TRIM(Alias) LIKE @mask ORDER BY Alias LIMIT 1000";
		}
	}
}
