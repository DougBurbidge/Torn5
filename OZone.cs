using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Torn
{
	/// <summary>
	/// This represents a P&C Micro's O-Zone lasergame database server. 
	/// It connects to O-Zone as if we were an O-Zone print server.
	/// </summary>
	public class OZone : LaserGameServer
	{
		protected string _server;

		private const int PORT_NO = 12121;

		protected OZone() { }

		public OZone(string server)
		{
			_server = server;
			Connect();
		}

		public override List<ServerGame> GetGames()
		{
			string textToSend = "{\"command\": \"list\"}";
			string result = QueryServer(textToSend);
			Console.WriteLine(result);

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
					if (jgame.TryGetProperty("starttime", out JsonElement startTime)) 
					{ 
						string dateTimeStr = startTime.GetString();
						game.Time = DateTime.Parse(dateTimeStr,
						  System.Globalization.CultureInfo.InvariantCulture);
					}
					if (jgame.TryGetProperty("endtime",   out JsonElement endTime))
					{
						try
						{
							string dateTimeStr = endTime.GetString();
							game.EndTime = DateTime.Parse(dateTimeStr,
							  System.Globalization.CultureInfo.InvariantCulture);
						} catch
                        {
							string dateTimeStr = startTime.GetString();
							game.EndTime = DateTime.Parse(dateTimeStr,
							  System.Globalization.CultureInfo.InvariantCulture);
						}
					}
					if (jgame.TryGetProperty("valid", out JsonElement valid))
					{
						int isValid = valid.GetInt16();
						if (isValid > 0) game.OnServer = true;
						else game.OnServer = false;
					}
					games.Add(game);
				}
			}
			System.Console.WriteLine(games);
			return games;
		}

		string ReadFromOzone(TcpClient client, NetworkStream nwStream)
        {
			byte[] bytesToRead = new byte[client.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
			return Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
		}

		private string QueryServer(string query)
		{
			//---create a TCPClient object at the IP and port no.---
			TcpClient client = new TcpClient(_server, PORT_NO);
			NetworkStream nwStream = client.GetStream();
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(query);
			Thread.Sleep(1);

			ReadFromOzone(client, nwStream);

			//---send the text---
			Console.WriteLine("Sending : " + query);
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);

			//---read back the text---
			var result = ReadFromOzone(client, nwStream);

			client.Close();

			return result;
		}

		void FillGames(string sql, List<ServerGame> games)
		{
			Console.WriteLine("FillGames");
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
			if (!game.GameId.HasValue)
				return;

			Console.WriteLine("PopulateGame");
			string textToSend = "{\"gamenumber\": " + game.GameId + ", \"command\": \"all\"}";
			string result = QueryServer(textToSend);
			Console.WriteLine(result);




			using (JsonDocument document = JsonDocument.Parse(result))
			{
				JsonElement root = document.RootElement;

				if (root.TryGetProperty("events", out JsonElement events))
				{
					string eventsStr = events.ToString();
					var eventsDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventsStr);

					foreach (var evnt in eventsDictionary)
					{
						string eventContent = evnt.Value.ToString();
						using (JsonDocument eventDocument = JsonDocument.Parse(eventContent))
						{
							JsonElement eventRoot = eventDocument.RootElement;
							int eventTime = 0;
							string eventPlayerId = "";
							int eventPlayerTeamId = -1;
							int eventType = -1;
							int score = 0;
							string eventOtherPlayerId = "";
							int eventOtherPlayerTeamId = -1;
							if (eventRoot.TryGetProperty("time", out JsonElement time)) eventTime = time.GetInt32();
							if (eventRoot.TryGetProperty("idf", out JsonElement playerId)) eventPlayerId = playerId.GetInt32().ToString();
							if (eventRoot.TryGetProperty("tidf", out JsonElement playerTeamId)) eventPlayerTeamId = playerTeamId.GetInt32();
							if (eventRoot.TryGetProperty("evtyp", out JsonElement evType)) eventType = evType.GetInt32();
							if (eventRoot.TryGetProperty("score", out JsonElement scr)) score = scr.GetInt32();
							if (eventRoot.TryGetProperty("ida", out JsonElement otherPlayerId)) eventOtherPlayerId = otherPlayerId.GetInt32().ToString();
							if (eventRoot.TryGetProperty("tida", out JsonElement otherplayerTeamId)) eventOtherPlayerTeamId = otherplayerTeamId.GetInt32();


							var gameEvent = new Event
							{
								Time = game.Time.AddSeconds(eventTime),
								ServerPlayerId = eventPlayerId,
								ServerTeamId = eventPlayerTeamId,
								Event_Type = eventType,
								Score = score,
								OtherPlayer = eventOtherPlayerId,
								OtherTeam = eventOtherPlayerTeamId,
							};
							game.Events.Add(gameEvent);
						}
					}


				}

				if (root.TryGetProperty("players", out JsonElement players))
				{
					string playersStr = players.ToString();
					var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(playersStr);

					foreach(var player in playersDictionary)
                    {
						string playerContent = player.Value.ToString();
						using (JsonDocument playerDocument = JsonDocument.Parse(playerContent))
                        {
							JsonElement playerRoot = playerDocument.RootElement;
							ServerPlayer serverPlayer = new ServerPlayer();
							if (playerRoot.TryGetProperty("alias", out JsonElement alias)) serverPlayer.Alias = alias.GetString();
							if (playerRoot.TryGetProperty("score", out JsonElement score)) serverPlayer.Score = score.GetInt32();
							if (playerRoot.TryGetProperty("omid", out JsonElement playerId)) serverPlayer.PlayerId = playerId.GetInt32().ToString();
							serverPlayer.Populate(game.Events);

							game.Players.Add(serverPlayer);

						}
					}


				}
				
			}

			/*			using (var reader = cmd.ExecuteReader())
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
						}*/

		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			Console.WriteLine("Get Players");
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
