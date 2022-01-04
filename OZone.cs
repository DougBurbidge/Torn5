using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Torn
{
	/// <summary>
	/// This represents a P&C Micro's O-Zone lasergame database server. 
	/// It connects to O-Zone as if we were an O-Zone print server.
	/// </summary>
	public class OZone : LaserGameServer
	{
		protected string _server;

		protected OZone() { }

		public OZone(string server)
		{
			_server = server;
			Connect();
		}

		public override List<ServerGame> GetGames()
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + _server);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			new StreamWriter(httpWebRequest.GetRequestStream()).Write("{ \"command\": \"list\" }");

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			var result = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();

			List<ServerGame> games = new List<ServerGame>();

			using (JsonDocument document = JsonDocument.Parse(result))
			{
				JsonElement root = document.RootElement;
				JsonElement gameList = root.GetProperty("gamelist");

				foreach (JsonElement jgame in gameList.EnumerateArray())
				{
					var game = new ServerGame();
					if (jgame.TryGetProperty("gamenum",   out JsonElement gameNum))   game.GameId = gameNum.GetInt32();
					if (jgame.TryGetProperty("gamename",  out JsonElement gameName))  game.Description = gameName.GetString();
					if (jgame.TryGetProperty("starttime", out JsonElement startTime)) game.Time = startTime.GetDateTime();
					if (jgame.TryGetProperty("endtime",   out JsonElement endTime))   game.EndTime = endTime.GetDateTime();
					if (jgame.TryGetProperty("valid",     out JsonElement valid))     game.OnServer = valid.GetBoolean();
				}
			}
			return games;
		}

		void FillGames(string sql, List<ServerGame> games)
		{
			if (!EnsureConnected())
				return;
/*
			MySqlCommand cmd = new MySqlCommand(sql, connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerGame game = new ServerGame
					{
						GameId = GetInt(reader, "Game_ID"),
						Description = GetString(reader, "Description"),
						Time = GetDateTime(reader, "Start_Time"),
						EndTime = GetDateTime(reader, "Finish_Time"),
						OnServer = true
					};
					game.InProgress = game.EndTime == default;
					games.Add(game);
				}
			}
*/
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!EnsureConnected() || !game.GameId.HasValue)
				return;

/*			" { \"gamenumber\": " + game.GameId + ", \"command\": \"all\" }";

			using (var reader = cmd.ExecuteReader())
			{
				if (reader.HasRows)
					game.Events.Clear();

				while (reader.Read())
				{
					var oneEvent = new Event
					{
						Time = GetDateTime(reader, "Time_Logged"),
						ServerPlayerId = GetString(reader, "Player_ID"),
						ServerTeamId = GetInt(reader, "Player_Team_ID"),
						Event_Type = GetInt(reader, "Event_Type"),
						Score = GetInt(reader, "Score"),
						OtherPlayer = GetString(reader, "Result_Data_1"),
						PointsLostByDeniee = GetInt(reader, "Result_Data_3"),
						ShotsDenied = GetInt(reader, "Result_Data_4")
					};
					oneEvent.OtherTeam = oneEvent.Event_Type == 30 || oneEvent.Event_Type == 31 ? GetInt(reader, "Result_Data_1") : GetInt(reader, "Result_Data_2");
					game.Events.Add(oneEvent);
				}
			}

			game.Players.Clear();

			cmd = new MySqlCommand(GameDetailSql(game.GameId), connection);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					ServerPlayer player = new ServerPlayer
					{
						ServerPlayerId = GetString(reader, "Player_ID"),
						ServerTeamId = GetInt(reader, "Player_Team_ID")
					};

					if (0 <= player.ServerTeamId && player.ServerTeamId < 8)
						player.Colour = (Colour)(player.ServerTeamId + 1);
					else
						player.Colour = Colour.None;

					player.Score = GetInt(reader, "Score");
					player.Pack = GetString(reader, "Pack_Name");
					player.PlayerId = GetString(reader, "Button_ID");
					player.Alias = GetString(reader, "Alias");
					player.Populate(game.Events);

					game.Players.Add(player);
				}
			}
*/
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			if (!EnsureConnected())
				return null;
			/*
						using (var cmd = new MySqlCommand(PlayersSql(), connection))
						{
							cmd.Parameters.AddWithValue("@mask", "%" + mask + "%");
							return ReadPlayers(cmd.ExecuteReader());
						}
			*/
			return null;
		}

		protected void Connect()
		{
/*
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
*/
		}

		bool EnsureConnected()
		{
			if (!Connected)
				Connect();
			return Connected;
		}
	}
}
