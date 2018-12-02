using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Torn
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class DemoServer: LaserGameServer
	{
		public DemoServer()
		{
		}

		public override List<ServerGame> GetGames()
		{
			List<ServerGame> games = new List<ServerGame>();

			for (int i = 0; i < 10; i++)
			{
				ServerGame game = new ServerGame();
				game.GameId = i;
				game.Description = "Demo Game";
				game.Time = DateTime.Now.AddMinutes(i - 10);
				game.OnServer = true;
				games.Add(game);
			}

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			var adjectives = new string[] { "Actual ", "Cyber ", "Dark ", "Laser ", "Mega ", "Phasor ", "Super ", "Ultra ", "Vector ", "Zone " };
			var nouns = new string[] { "Ace", "Blazer", "Chaser", "Dueller", "Rogue", "Runner", "Shark", "Star", "Stunner", "Trekker" };

		    game.Players = new List<ServerPlayer>();
			Random r = new Random();

			for (int i = 0; i < 10; i++)
			{
				ServerPlayer player = new ServerPlayer();
				player.Colour = (Colour)r.Next(1, 8);
				player.Score = r.Next(-100, 1000) * 10 + r.Next(0,2);
				player.PackName = "Pack" + r.Next(1, 30).ToString();
				var x = r.Next(0, 9);
				var y = r.Next(0, 9);
				player.PlayerId = "demo" + (x * 10).ToString() + y.ToString();
				player.Alias = adjectives[x] + nouns[y];
				game.Players.Add(player);
			}
		}

		public override DbDataReader GetPlayers(string mask)
		{
			return null;
		}
	}
}
