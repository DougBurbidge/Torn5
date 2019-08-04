using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Torn
{
	/// <summary>
	/// Represents a proprietary laser game system, which exposes some sort of interface that we can get game data from.
	/// </summary>
	public abstract class LaserGameServer: IDisposable
	{
		protected bool connected;
		protected virtual bool GetConnected() { return connected; }
		protected virtual void SetConnected(bool value) { connected = value; }
		public bool Connected { get { return GetConnected(); } protected set { SetConnected(value); } }

		public LaserGameServer() {}

		public virtual void Dispose() {}

		public virtual TimeSpan GameTimeElapsed() { return TimeSpan.MinValue; }

		public abstract List<ServerGame> GetGames();
		
		public virtual void GetMoreGames(List<ServerGame> games) {}

		public abstract void PopulateGame(ServerGame game);

		public abstract List<LaserGamePlayer> GetPlayers(string mask);

		public virtual List<LaserGamePlayer> ReadPlayers(DbDataReader reader)
		{
			var players = new List<LaserGamePlayer>();
			try
			{
				while (reader.Read())
					players.Add(new LaserGamePlayer
					            {
					            	Alias = reader.GetString(0),
					            	Name = reader.GetString(1),
					            	Id = reader.GetString(2)
					            }
					           );
			}
			finally
			{
				reader.Close();
			}
			return players;
		}

		/// <summary>True if the GetPlayers query returns player names as well as aliases. (False for PAndC where heliosType >= 47.)</summary>
		public virtual bool HasNames() { return true; }
	}

	/// <summary>Represents a player's identity.</summary>
	public class LaserGamePlayer
	{
		public string Alias { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
	}

	/// <summary>This is a fake stub lasergame server for test purposes.</summary>
	public class StubServer: LaserGameServer
	{
		public StubServer(string server = "")
		{
			Connected = true;
		}

		public override List<ServerGame> GetGames()
		{
			List<ServerGame> games = new List<ServerGame>();

			var game = new ServerGame();
			game.GameId = 1;
			game.Description = "test game 1";
			game.Time = new DateTime(2018, 1, 1, 12, 00, 00);
			game.EndTime = new DateTime(2018, 1, 1, 12, 12, 00);
			game.OnServer = true;
			games.Add(game);

			game = new ServerGame();
			game.GameId = 2;
			game.Description = "test game 2";
			game.Time = new DateTime(2018, 1, 1, 12, 15, 00);
			game.EndTime = new DateTime(2018, 1, 1, 12, 27, 00);
			game.OnServer = true;
			games.Add(game);

			game = new ServerGame();
			game.GameId = 3;
			game.Description = "test game 3";
			game.Time = new DateTime(2018, 1, 1, 12, 30, 00);
			game.InProgress = true;
			game.OnServer = true;
			games.Add(game);

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			game.Players = new List<ServerPlayer>();

			var player = new ServerPlayer();
			player.Colour = Colour.Red;
			player.Score = 1000;
			player.Pack = "Pack 1";
			player.PlayerId = "ABC-001";
			player.Alias = "Alias 1";
			game.Players.Add(player);

			player = new ServerPlayer();
			player.Colour = Colour.Red;
			player.Score = 2000;
			player.Pack = "Pack 2";
			player.PlayerId = "ABC-002";
			player.Alias = "Alias 2";
			game.Players.Add(player);
			
			player = new ServerPlayer();
			player.Colour = Colour.Blue;
			player.Score = 3000;
			player.Pack = "Pack 3";
			player.PlayerId = "ABC-003";
			player.Alias = "Alias 3";
			game.Players.Add(player);
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			var players = new List<LaserGamePlayer>();
			players.Add(new LaserGamePlayer { Alias = "Alias 1", Name = "Name 1", Id = "ABC-001" });
			players.Add(new LaserGamePlayer { Alias = "Alias 2", Name = "Name 2", Id = "ABC-002" });
			players.Add(new LaserGamePlayer { Alias = "Alias 3", Name = "Name 3", Id = "ABC-003" });
			players.Add(new LaserGamePlayer { Alias = "Alias 4", Name = "Name 4", Id = "ABC-004" });
			return players;
		}
	}
}
