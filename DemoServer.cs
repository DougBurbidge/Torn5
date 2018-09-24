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

			ServerGame game = new ServerGame();
			game.GameId = 0;
			game.Description = "Demo Game";
			game.Time = DateTime.Now;
			game.OnServer = true;
			games.Add(game);

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			game.Players = new List<ServerPlayer>();

			ServerPlayer player = new ServerPlayer();
			player.Colour = Colour.Red;
			player.Score = 1;
			player.PackName = "Pack";
			player.PlayerId = "001";
			player.Alias = "Player";
			game.Players.Add(player);
		}

		public override DbDataReader GetPlayers(string mask)
		{
			return null;
		}
	}
}
