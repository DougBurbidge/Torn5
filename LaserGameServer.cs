using System;
using System.Collections.Generic;

namespace Torn
{
	/// <summary>
	/// Represents a proprietary laser game system, which exposes some sort of interface that we can get game data from.
	/// </summary>
	public abstract class LaserGameServer: IDisposable
	{
		public bool Connected { get; protected set; }

		public LaserGameServer() {}

		public virtual void Dispose() {}

		public virtual TimeSpan GameTimeElapsed() { return TimeSpan.MinValue; }

		public virtual List<ServerGame> GetGames() { return null; }

		public virtual void PopulateGame(ServerGame game) {}
	}
}
