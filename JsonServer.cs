using System;
using System.Collections.Generic;
using System.Net;
using Torn;

namespace Torn5
{
	/// <summary>
	/// This queries a JSON server for lasergame data.
	/// </summary>
	public class JsonServer: LaserGameServer
	{
		WebClient webClient;

		public JsonServer()
		{
			webClient = new WebClient();
		}

		public override List<ServerGame> GetGames()
		{
			string g;
			try
			{
				g = webClient.DownloadString("http://localhost:8080/games");
			}
			catch
			{
				return null;
			}

			List<ServerGame> games = new List<ServerGame>();

			int pos = g.IndexOf('[') + 1;

			while (pos > 0 && pos < g.Length)
			{
				int posEnd = g.IndexOf('}', pos);
				if (posEnd > -1 && posEnd < g.Length)
					games.Add(new ServerGame(g.Substring(pos, posEnd - pos)));
				pos = posEnd + 1;
			}

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
		    game.Players = new List<ServerPlayer>();
			Random r = new Random();

			for (int i = 0; i < 10; i++)
			{
				ServerPlayer player = new ServerPlayer();
				player.Colour = (Colour)r.Next(1, 9);
				player.Score = r.Next(-100, 1000) * 10 + r.Next(0, 3) * 2001;
				player.Pack = "Pack" + r.Next(1, 30).ToString("D2");
				var x = r.Next(0, 10);
				var y = r.Next(0, 10);
				player.PlayerId = "demo" + (x * 10).ToString() + y.ToString();
				game.Players.Add(player);
			}
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			var players = new List<LaserGamePlayer>();

			return players;
		}
	}
}
