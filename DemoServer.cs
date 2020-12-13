using System;
using System.Collections.Generic;

namespace Torn
{
    /// <summary>
	/// DemoServer returns a list of 10 games, each containing pseudorandom players.
    /// </summary>
    public class DemoServer : LaserGameServer
    {
        readonly string[] adjectives;
        readonly string[] nouns;

        public DemoServer()
        {
			adjectives = new string[] { "Actual ", "Battle ", "Cyber ", "Dark ", "Delta ", "Elite ", "Inter ", "Laser ", "Mega ", "Phasor ", "Super ", "Ultra ", "Vector ", "Zone " };
			nouns = new string[] { "Ace", "Blaster", "Blazer", "Chaser", "Crystal", "Dueller", "Max", "Rogue", "Runner", "Shark", "Star", "Stunner", "Trekker", "Warrior" };
			connected = true;
		}

		public override TimeSpan GameTimeElapsed()
		{
			DateTime now = DateTime.Now;
			DateTime start = now.TruncDateTime(new TimeSpan(0, 15, 0));
			TimeSpan elapsed = now.Subtract(start);
			return elapsed > new TimeSpan(0, 12, 0) ? TimeSpan.Zero : elapsed;
        }

        public override List<ServerGame> GetGames()
        {
            List<ServerGame> games = new List<ServerGame>();

			DateTime now = DateTime.Now.TruncDateTime(new TimeSpan(0, 15, 0));

			for (int i = 0; i < 10; i++)
			{
                ServerGame game = new ServerGame
                {
                    GameId = i,
                    Description = "Demo Game",
                    Time = now.AddMinutes(i * 15 - 150),
                    OnServer = true
                };
                game.EndTime = game.Time.AddMinutes(12);
				games.Add(game);
			}

            return games;
        }

        public override void PopulateGame(ServerGame game)
        {
			if (!game.GameId.HasValue)
				return;

		    game.Players = new List<ServerPlayer>();
		    Random r = new Random((int)game.GameId);

			for (int i = 0; i < 10; i++)
			{
				var x = r.Next(0, adjectives.Length);
				var y = r.Next(0, nouns.Length);
                ServerPlayer player = new ServerPlayer
                {
                    Colour = (Colour)r.Next(1, 9),
                    Score = r.Next(-100, 1000) * 10 + r.Next(0, 3) * 2001,
                    Pack = "Pack" + r.Next(1, 30).ToString("D2"),
                    PlayerId = string.Format("demo{0:D2}{1:D2}", x, y),
                    Alias = adjectives[x] + nouns[y]
                };
                game.Players.Add(player);
			}
		}

        public override IEnumerable<Dto.Player> GetPlayers(string mask)
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
