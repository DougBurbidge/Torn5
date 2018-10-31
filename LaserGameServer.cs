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
		//protected DbConnection connection;
		public bool Connected { get; protected set; }

		public LaserGameServer() {}

		public virtual void Dispose() {}

		public virtual TimeSpan GameTimeElapsed() { return TimeSpan.MinValue; }

		public abstract List<ServerGame> GetGames();

		public abstract void PopulateGame(ServerGame game);

		public abstract IEnumerable<Dto.Player> GetPlayers(string mask);
		
		/// <summary>True if the GetPlayers query returns player names as well as aliases. (False for PAndC where heliosType >= 47.)</summary>
		public virtual bool HasNames() { return true; }
	}
}

namespace Torn.Dto 
{
    public class Player 
    {
        public string UserId { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
    }    
}