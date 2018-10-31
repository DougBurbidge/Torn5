using System;
using System.Collections.Generic;

namespace Torn
{
    /// <summary>
    /// Description of Class1.
    /// </summary>
    public class DemoServer : LaserGameServer
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

        public override IEnumerable<Dto.Player> GetPlayers(string mask)
        {
            yield return new Dto.Player { UserId = "00001", Alias = "Player 1", Name = "Player 1" };
            yield return new Dto.Player { UserId = "00002", Alias = "Player 2", Name = "Player 2" };
            yield return new Dto.Player { UserId = "00003", Alias = "Player 3", Name = "Player 3" };
            yield return new Dto.Player { UserId = "00004", Alias = "Player 4", Name = "Player 4" };
            yield return new Dto.Player { UserId = "00005", Alias = "Player 5", Name = "Player 5" };
            yield return new Dto.Player { UserId = "00006", Alias = "Player 6", Name = "Player 6" };
        }
    }
}
