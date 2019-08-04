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
		string[] adjectives;
		string[] nouns;

		public DemoServer()
		{
			adjectives = new string[] { "Actual ", "Cyber ", "Dark ", "Laser ", "Mega ", "Phasor ", "Super ", "Ultra ", "Vector ", "Zone " };
			nouns = new string[] { "Ace", "Blazer", "Chaser", "Dueller", "Rogue", "Runner", "Shark", "Star", "Stunner", "Trekker" };
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
				player.Alias = adjectives[x] + nouns[y];
				game.Players.Add(player);
			}
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			var players = new List<LaserGamePlayer>();
			for (int x = 0; x < 10; x++)
				for (int y = 0; y < 10; y++)
				{
					players.Add(new LaserGamePlayer
				            {
				            	Alias = adjectives[x] + nouns[y],
				            	Name = adjectives[x] + nouns[y],
				            	Id = "demo" + (x * 10).ToString() + y.ToString()
				            }
				           );
				}
			return players;
		}
	}
}
