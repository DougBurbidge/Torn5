using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using Torn;

namespace Torn
{
	/// <summary>
	/// This queries a JSON server for lasergame data.
	/// </summary>
	public class JsonServer: LaserGameServer
	{
        readonly WebClient webClient;  // TODO: remove WebClient and replace with HttpClient. (A single HttpClient handles multiple requests in parallel.)
        readonly DemoServer demoServer;
        readonly string _server;

		public JsonServer(string server = "")
		{
			webClient = new WebClient();
			demoServer = new DemoServer();

			if (!server.Contains("://"))
				server = "http://" + server;
			if (!server.Substring(7).Contains(":"))
				server += ":8080";

			_server = server;
		}

		string DownloadString(ref bool reentrant, string query)
		{
			reentrant = true;
			try
			{
				connected = true;
				WebClient wc = webClient.IsBusy ? new WebClient() : webClient;
				return wc.DownloadString(_server + query);
			}
			catch (Exception e)
			{
				status = e.Message;
				connected = false;
				return null;
			}
			finally
			{
				reentrant = false;
			}
		}

		// These reentrant flags are to allow Torn5 to call itself for results: configure Torn5 to use JsonServer, and point it at localhost:8080.
		// When this happens, it will return results from DemoServer, rather than infinite looping. This is handy for testing, and for when users accidentally misconfigure.
		bool gameTimeElapsedReentrant = false;

		public override TimeSpan GameTimeElapsed()
		{
			if (gameTimeElapsedReentrant)
				return demoServer.GameTimeElapsed();

			try
			{
				string s = DownloadString(ref gameTimeElapsedReentrant, "/elapsed");
				return TimeSpan.FromSeconds(JsonSerializer.Deserialize<int>(s));
			}
			catch (Exception e)
			{
				status = e.Message;
				return TimeSpan.FromSeconds(-5);
			}
		}

		bool getGamesReentrant = false;

		public override List<ServerGame> GetGames()
		{
			if (getGamesReentrant)
				return demoServer.GetGames();

			try
			{
				string s = DownloadString(ref getGamesReentrant, "/games");
				return JsonSerializer.Deserialize<List<ServerGame>>(s);
			}
			catch (Exception e)
			{
				status = e.Message;
				return new List<ServerGame>();
			}
		}

		bool populateGameReentrant = false;

		public override void PopulateGame(ServerGame game)
		{
			if (populateGameReentrant)
			{
				demoServer.PopulateGame(game);
				return;
			}

			try
			{
				string s = DownloadString(ref populateGameReentrant, "/game" + game.Time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
				ServerGame game2 = JsonSerializer.Deserialize<ServerGame>(s);
				game.Description = s.IndexOf("{\"error\":") == 0 ? s.Substring(9) : game2.Description;
				game.EndTime = game2.EndTime;
				game.InProgress = game2.InProgress;
				game.OnServer = game2.OnServer;
				game.GameId = game2.GameId;
				game.Events = game2.Events;
				game.Players = game2.Players;
			}
			catch (Exception e)
			{
				status = e.Message;
			}
		}

		bool getPlayersReentrant = false;

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			if (getPlayersReentrant)
				return demoServer.GetPlayers(mask);

			try
			{
				string s = DownloadString(ref getPlayersReentrant, "/players");
				var players = JsonSerializer.Deserialize<List<LaserGamePlayer>>(s);
				return players;
			}
			catch (Exception e)
			{
				status = e.Message;
				return new List<LaserGamePlayer>();
			}
		}
		public override List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players)
		{
			return GetPlayers(mask);
		}
	}
}
