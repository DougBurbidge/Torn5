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
        readonly WebClient webClient;
        readonly DemoServer demoServer;
        readonly string _server;

		public JsonServer(string server = "")
		{
			webClient = new WebClient();
			demoServer = new DemoServer();
			_server = server;
		}

		bool gameTimeElapsedReentrant = false;
		public override TimeSpan GameTimeElapsed()
		{
			if (gameTimeElapsedReentrant)
				return TimeSpan.FromSeconds(-4);

			gameTimeElapsedReentrant = true;
			try
			{
				try
				{
					return TimeSpan.FromSeconds(JsonSerializer.Deserialize<int>(webClient.DownloadString(_server + "/elapsed")));
				}
				catch
				{
					return TimeSpan.FromSeconds(-5);
				}
			}
			finally
			{
				gameTimeElapsedReentrant = false;
			}
		}

		// These reentrant flags are to allow Torn5 to call itself for results: configure Torn5 to use JsonServer, and point it at localhost:8080.
		// When this happens, it will return results from DemoServer, rather than infinite looping. This is handy for testing, and for when users accidentally misconfigure.
		bool getGamesReentrant = false;

		public override List<ServerGame> GetGames()
		{
			if (getGamesReentrant)
				return demoServer.GetGames();

			getGamesReentrant = true;
            try
            {
				try
				{
					return JsonSerializer.Deserialize<List<ServerGame>>(webClient.DownloadString(_server + "/games"));
				}
				catch
				{
					return null;
				}
			}
			finally
			{
				getGamesReentrant = false;
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

			populateGameReentrant = true;
			try
            {
				ServerGame game2 = JsonSerializer.Deserialize<ServerGame>(webClient.DownloadString(_server + "/game" + game.Time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)));
				game.Description = game2.Description;
			}
			finally
            {
				populateGameReentrant = false;
			}
		}

		bool getPlayersReentrant = false;

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			if (getPlayersReentrant)
				return demoServer.GetPlayers(mask);

			getPlayersReentrant = true;
			try
			{
				var players = new List<LaserGamePlayer>();

				return players;
			}
			finally
			{
				getPlayersReentrant = false;
			}
		}
	}
}
