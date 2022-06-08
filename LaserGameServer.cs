using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.Json.Serialization;

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

		protected string status;
		protected virtual string GetStatus() { return status; }
		protected virtual void SetStatus(string value) { status = value; }
		public string Status { get { return GetStatus(); } protected set { SetStatus(value); } }

		public LaserGameServer() {}

		public virtual void Dispose() {}

		public virtual TimeSpan GameTimeElapsed() { return TimeSpan.MinValue; }

		public abstract List<ServerGame> GetGames();
		
		public virtual void GetMoreGames(List<ServerGame> games) {}

		public abstract void PopulateGame(ServerGame game);

		public abstract List<LaserGamePlayer> GetPlayers(string mask);
		public abstract List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players);

		public virtual List<LaserGamePlayer> ReadPlayers(DbDataReader reader)
		{
			var players = new List<LaserGamePlayer>();
			string name;
			try
			{
				while (reader.Read())
					players.Add(new LaserGamePlayer
					            {
					            	Alias = reader.GetString(0),
					            	Name = (name = reader.GetString(1)) == "" ? null : name,
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
		[JsonPropertyName("alias")]
		public string Alias { get; set; }

		[JsonPropertyName("name")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
		public string Name { get; set; }

		[JsonPropertyName("id")]
		public string Id { get; set; }
	}

	/// <summary>This is a fake stub lasergame server for test purposes.</summary>
	public class StubServer: LaserGameServer
	{
		public StubServer()
		{
			Connected = true;
		}

		public override List<ServerGame> GetGames()
		{
			List<ServerGame> games = new List<ServerGame>
			{
				new ServerGame
				{
					GameId = 1,
					Description = "test game 1",
					Time = new DateTime(2018, 1, 1, 12, 00, 00),
					EndTime = new DateTime(2018, 1, 1, 12, 12, 00),
					OnServer = true
				},

				new ServerGame
				{
					GameId = 2,
					Description = "test game 2",
					Time = new DateTime(2018, 1, 1, 12, 15, 00),
					EndTime = new DateTime(2018, 1, 1, 12, 27, 00),
					OnServer = true
				},

				new ServerGame
				{
					GameId = 3,
					Description = "test game 3",
					Time = new DateTime(2018, 1, 1, 12, 30, 00),
					InProgress = true,
					OnServer = true
				}
			};

			return games;
		}

		public override void PopulateGame(ServerGame game)
		{
			game.Players = new List<ServerPlayer>
			{
				new ServerPlayer
				{
					Colour = Colour.Red,
					Score = 1000,
					Pack = "Pack 1",
					PlayerId = "ABC-001",
					Alias = "Alias 1"
				},

				new ServerPlayer
				{
					Colour = Colour.Red,
					Score = 2000,
					Pack = "Pack 2",
					PlayerId = "ABC-002",
					Alias = "Alias 2"
				},

				new ServerPlayer
				{
					Colour = Colour.Blue,
					Score = 3000,
					Pack = "Pack 3",
					PlayerId = "ABC-003",
					Alias = "Alias 3"
				}
			};
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			return new List<LaserGamePlayer>
			{
				new LaserGamePlayer { Alias = "Alias 1", Name = "Name 1", Id = "ABC-001" },
				new LaserGamePlayer { Alias = "Alias 2", Name = "Name 2", Id = "ABC-002" },
				new LaserGamePlayer { Alias = "Alias 3", Name = "Name 3", Id = "ABC-003" },
				new LaserGamePlayer { Alias = "Alias 4", Name = "Name 4", Id = "ABC-004" }
			};
		}

		public override List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players)
		{
			return GetPlayers(mask);
		}
	}
}
