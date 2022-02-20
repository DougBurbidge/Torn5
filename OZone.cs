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

		private List<ServerGame> serverGames = new List<ServerGame>();
		private List<LaserGamePlayer> laserPlayers = new List<LaserGamePlayer>();

		protected OZone() { }

		public OZone(string server)
		{
			_server = server;
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
					serverGames.Add(game);
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

		public override void PopulateGame(ServerGame game)
		{
			if (!game.GameId.HasValue)
				return;

			if (game.Events.Count != 0)
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
							if (playerRoot.TryGetProperty("omid", out JsonElement playerId)) 
							{ 
								serverPlayer.PlayerId = playerId.GetInt32().ToString(); 
								serverPlayer.ServerPlayerId = playerId.GetInt32().ToString(); 
							};
							if (playerRoot.TryGetProperty("tid", out JsonElement teamId))
							{
								serverPlayer.TeamId = teamId.GetInt32();
								serverPlayer.ServerTeamId = teamId.GetInt32();
								if (0 <= serverPlayer.ServerTeamId && serverPlayer.ServerTeamId < 8)
									serverPlayer.Colour = (Colour)(serverPlayer.ServerTeamId + 1);
								else
									serverPlayer.Colour = Colour.None;
							}
							if(!serverPlayer.IsPopulated()) serverPlayer.Populate(game.Events);

							game.Players.Add(serverPlayer);

						}
					}


				}
				
			}

		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{

			Console.WriteLine("Get Players");
			GetGames();
			foreach (ServerGame game in serverGames)
			{
				string textToSend = "{\"gamenumber\": " + game.GameId + ", \"command\": \"all\"}";
				string result = QueryServer(textToSend);
				Console.WriteLine(result);

				using (JsonDocument document = JsonDocument.Parse(result))
				{
					JsonElement root = document.RootElement;

					if (root.TryGetProperty("players", out JsonElement players))
					{
						string playersStr = players.ToString();
						var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(playersStr);

						foreach (var player in playersDictionary)
						{
							string playerContent = player.Value.ToString();
							using (JsonDocument playerDocument = JsonDocument.Parse(playerContent))
							{
								JsonElement playerRoot = playerDocument.RootElement;
								LaserGamePlayer laserPlayer = new LaserGamePlayer();
								if (playerRoot.TryGetProperty("alias", out JsonElement alias))
								{
									laserPlayer.Alias = alias.GetString();
								}
								if (playerRoot.TryGetProperty("omid", out JsonElement playerId))
								{
									laserPlayer.Id = playerId.GetInt32().ToString();
								};
								if(laserPlayers.Find((p) => p.Id == laserPlayer.Id) == null)
                                {
									laserPlayers.Add(laserPlayer);
								}

							}
						}


					}

				}
			}

			
			return null;
		}
	}
}
