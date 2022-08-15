using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Torn
{
	/// <summary>
	/// This represents a P&C Micro's O-Zone lasergame database server. 
	/// It connects to O-Zone as if we were an O-Zone print server.
	/// </summary>
	public class OZone : LaserGameServer
	{
		protected string server;
		protected string port;

		private List<ServerGame> serverGames = new List<ServerGame>();
		private List<LaserGamePlayer> laserPlayers = new List<LaserGamePlayer>();

		private TcpClient client;
		private NetworkStream nwStream;

		protected OZone() { }

		public OZone(string _server, string _port)
		{
			server = _server;
			port = _port;
			Connect();
		}

		private bool Connect()
        {
			if (connected) {
				client.Close();
			};
			client = new TcpClient(server, Int32.Parse(port));
			nwStream = client.GetStream();
			connected = true;
			nwStream.ReadTimeout = 500;

			while (true)
			{

				string data = ReadFromOzone(client, nwStream);
				if (data == "")
				{
					break;
				}
			}
			return connected;
		}

		public override List<ServerGame> GetGames()
		{
			string textToSend = "{\"command\": \"list\"}";
			string result = QueryServer(textToSend);

			Console.WriteLine(result);


			List<ServerGame> games = new List<ServerGame>();

			string cleanedResult = result.Remove(0, 5);

			JObject root = JObject.Parse(cleanedResult);

			JToken gameList = root.SelectToken("$.gamelist");

			foreach (JObject jgame in gameList.Children())
			{
				var game = new ServerGame();
				if (jgame["gamenum"] != null)   game.GameId = Int32.Parse(jgame["gamenum"].ToString());
				if (jgame["gamename"] != null)   game.Description = jgame["gamename"].ToString();
				if (jgame["starttime"] != null)
				{
					string dateTimeStr = jgame["starttime"].ToString();
					game.Time = DateTime.Parse(dateTimeStr,
						System.Globalization.CultureInfo.InvariantCulture);
				}
				if (jgame["endtime"] != null)
				{
					try
					{
						string dateTimeStr = jgame["endtime"].ToString();
						game.EndTime = DateTime.Parse(dateTimeStr,
							System.Globalization.CultureInfo.InvariantCulture);
					} catch
                    {
						string dateTimeStr = jgame["starttime"].ToString();
						game.EndTime = DateTime.Parse(dateTimeStr,
							System.Globalization.CultureInfo.InvariantCulture);
					}
				}
				if (jgame["valid"] != null)
				{
					int isValid = Int16.Parse(jgame["valid"].ToString());
					if (isValid > 0)
					{
						game.OnServer = true;
						games.Add(game);
					}
					else game.OnServer = false;

					serverGames.Add(game);
				}
			}
			
			return games;
		}

		string ReadFromOzone(TcpClient client, NetworkStream nwStream)
        {
			try
			{
				string str = "";
				bool reading = true;
				int BYTE_LIMIT = 1024;


				while (reading)
				{
					byte[] bytesToRead = new byte[BYTE_LIMIT];
					int bytesRead = nwStream.Read(bytesToRead, 0, BYTE_LIMIT);
					string current = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

					str += current;

					if (bytesRead < BYTE_LIMIT) reading = false;
				}

				return str;
			}
			catch
			{
				return "";
			}
		}

		private string QueryServer(string query)
		{
			//---create a TCPClient object at the IP and port no.---
			byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes("(" + query);

			nwStream.ReadTimeout = 2000;

			int[] header = new int[] { query.Length, 0, 0, 0 };

			byte[] bytesToSend = new byte[header.Length + messageBytes.Length];
			System.Buffer.BlockCopy(header, 0, bytesToSend, 0, header.Length);
			System.Buffer.BlockCopy(messageBytes, 0, bytesToSend, header.Length, messageBytes.Length);

			//---send the text---
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);

			//---read back the text---
			string result = "";
			while (true)
			{

				string data = ReadFromOzone(client, nwStream);
				result += data;
				if (data == "")
				{
					break;
				}
			}

			return result;
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!game.GameId.HasValue)
				return;

			if (game.Events.Count != 0)
				return;

			string textToSend = "{\"gamenumber\": " + game.GameId + ", \"command\": \"all\"}";
			string result = QueryServer(textToSend);

			string cleanedResult = result.Remove(0, 5);

			JObject root = JObject.Parse(cleanedResult);

			IDictionary<int, string> eventNames = new Dictionary<int, string>();
			eventNames.Add(0, "Tag Foe - Phasor");
			eventNames.Add(1, "Tag Foe - Chest");
			eventNames.Add(2, "Tag Foe - Left Front Shoulder");
			eventNames.Add(3, "Tag Foe - Right Front Shoulder");
			eventNames.Add(4, "Tag Foe - Left Back Shoulder");
			eventNames.Add(5, "Tag Foe - Right Back Shoulder");
			eventNames.Add(6, "Tag Foe - Back");

			eventNames.Add(7, "Tag Ally - Phasor");
			eventNames.Add(8, "Tag Ally - Chest");
			eventNames.Add(9, "Tag Ally - Left Front Shoulder");
			eventNames.Add(10, "Tag Ally - Right Front Shoulder");
			eventNames.Add(11, "Tag Ally - Left Back Shoulder");
			eventNames.Add(12, "Tag Ally - Right Back Shoulder");
			eventNames.Add(13, "Tag Ally - Back");

			eventNames.Add(14, "Tagged by Foe - Phasor");
			eventNames.Add(15, "Tagged by Foe - Chest");
			eventNames.Add(16, "Tagged by Foe - Left Front Shoulder");
			eventNames.Add(17, "Tagged by Foe - Right Front Shoulder");
			eventNames.Add(18, "Tagged by Foe - Left Back Shoulder");
			eventNames.Add(19, "Tagged by Foe - Right Back Shoulder");
			eventNames.Add(20, "Tagged by Foe - Back");

			eventNames.Add(21, "Tagged by Ally - Phasor");
			eventNames.Add(22, "Tagged by Ally - Chest");
			eventNames.Add(23, "Tagged by Ally - Left Front Shoulder");
			eventNames.Add(24, "Tagged by Ally - Right Front Shoulder");
			eventNames.Add(25, "Tagged by Ally - Left Back Shoulder");
			eventNames.Add(26, "Tagged by Ally - Right Back Shoulder");
			eventNames.Add(27, "Tagged by Ally - Back");

			eventNames.Add(28, "Level 1 Termination");
			eventNames.Add(29, "Level 2 Termination");

			eventNames.Add(30, "Tag Base");
			eventNames.Add(31, "Destroy Base");

			eventNames.Add(32, "Eliminated");

			eventNames.Add(33, "Tagged by Base");
			eventNames.Add(34, "Tagged by Mine");

			eventNames.Add(61, "Denied Player");
			eventNames.Add(63, "Denied");

			if (root["events"] != null)
			{
				string eventsStr = root["events"].ToString();
				var eventsDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventsStr);

				foreach (var evnt in eventsDictionary)
				{
					string eventContent = evnt.Value.ToString();
					JObject eventRoot = JObject.Parse(eventContent);

					int eventTime = 0;
					string eventPlayerId = "";
					int eventPlayerTeamId = -1;
					int eventType = -1;
					int score = 0;
					string eventOtherPlayerId = "";
					int eventOtherPlayerTeamId = -1;
					if (eventRoot["time"] != null) eventTime = Int32.Parse(eventRoot["time"].ToString());
					if (eventRoot["idf"] != null) eventPlayerId = eventRoot["idf"].ToString();
					if (eventRoot["tidf"] != null) eventPlayerTeamId = Int32.Parse(eventRoot["tidf"].ToString());
					if (eventRoot["evtyp"] != null) eventType = Int32.Parse(eventRoot["evtyp"].ToString());
					if (eventRoot["score"] != null) score = Int32.Parse(eventRoot["score"].ToString());
					if (eventRoot["ida"] != null) eventOtherPlayerId = eventRoot["ida"].ToString();
					if (eventRoot["tida"] != null) eventOtherPlayerTeamId = Int32.Parse(eventRoot["tida"].ToString());

					var gameEvent = new Event
					{
						Time = game.Time.AddSeconds(eventTime),
						ServerPlayerId = eventPlayerId,
						ServerTeamId = eventPlayerTeamId,
						Event_Type = eventType,
						Score = score,
						OtherPlayer = eventOtherPlayerId,
						OtherTeam = eventOtherPlayerTeamId,
						Event_Name = eventNames.ContainsKey(eventType) ? eventNames[eventType] : "Unknown",
					};
					game.Events.Add(gameEvent);
				}


			}

			if (root["players"] != null)
			{
				string playersStr = root["players"].ToString();
				var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(playersStr);

				foreach(var player in playersDictionary)
                {
					string id = player.Key.ToString();
					string playerContent = player.Value.ToString();
					JObject playerRoot = JObject.Parse(playerContent);

					ServerPlayer serverPlayer = new ServerPlayer();
					if (playerRoot["alias"] != null) serverPlayer.Alias = playerRoot["alias"].ToString();
					if (playerRoot["score"] != null) serverPlayer.Score = Int32.Parse(playerRoot["score"].ToString());
					if (playerRoot["omid"] != null) 
					{
						string omid = playerRoot["omid"].ToString();
						// If pack was not logged in use alias as identifier
						if(omid == "-1")
                        {
							omid = playerRoot["alias"].ToString();

						}
						serverPlayer.PlayerId = omid; 
						serverPlayer.ServerPlayerId = id; 
					};
					if (playerRoot["tid"] != null)
					{
						serverPlayer.TeamId = Int32.Parse(playerRoot["tid"].ToString());
						serverPlayer.ServerTeamId = Int32.Parse(playerRoot["tid"].ToString());
						if (0 <= serverPlayer.ServerTeamId && serverPlayer.ServerTeamId < 8)
							serverPlayer.Colour = (Colour)(serverPlayer.ServerTeamId + 1);
						else
							serverPlayer.Colour = Colour.None;
					}

					if (playerRoot["qrcode"] != null) serverPlayer.QRCode = playerRoot["qrcode"].ToString();

					if (!serverPlayer.IsPopulated()) serverPlayer.Populate(game.Events);

					game.Players.Add(serverPlayer);

				}


			}


		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			return new List<LaserGamePlayer>();
		}

		public override List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players)
		{
			foreach (LeaguePlayer player in players)
			{

				LaserGamePlayer laserPlayer = new LaserGamePlayer();
				laserPlayer.Alias = player.Name;
				laserPlayer.Id = player.Id;
				if(laserPlayers.Find((p) => p.Id == laserPlayer.Id) == null) laserPlayers.Add(laserPlayer);

			}
			
			return laserPlayers;
		}
	}
}
