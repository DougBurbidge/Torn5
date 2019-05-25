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

		public abstract DbDataReader GetPlayers(string mask);

		/// <summary>True if the GetPlayers query returns player names as well as aliases. (False for PAndC where heliosType >= 47.)</summary>
		public virtual bool HasNames() { return true; }
	}
}
