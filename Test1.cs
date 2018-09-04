using System;
using NUnit.Framework;
using Torn;

namespace TornWeb
{
	[TestFixture]
	public class Test1
	{
		League CreateLeague()
		{
			var league = new League();

			var team = new LeagueTeam() { Name = "Team A" };
			league.Teams.Add(team);

			team.Players.Add(new LeaguePlayer() { Name = "One", Id = "001", Comment = "one" } );
			team.Players.Add(new LeaguePlayer() { Name = "<&>' \"", Id = "002", Comment = "two" } );
			team.Players.Add(new LeaguePlayer() { Name = "Three", Id = "003" } );

			team = new LeagueTeam() { Name = "Team B" };
			league.Teams.Add(team);

			team.Players.Add(new LeaguePlayer() { Name = "Four", Id = "004", Comment = "one" } );
			team.Players.Add(new LeaguePlayer() { Name = "One", Id = "001" } );

			team = new LeagueTeam() { Name = "Team C" };
			league.Teams.Add(team);

			return league;			
		}

		[Test]
		public void TestLeagueSaveLoad()
		{
			var league = CreateLeague();

			league.Save("testleague.Torn");

			var league2 = new League();
			league2.Load("testleague5.Torn");

			Assert.AreEqual(3, league2.Teams.Count, "Number of teams");
			Assert.AreEqual("Team A", league2.Teams[0].Name, "name of team 1");
			Assert.AreEqual(3, league2.Teams[0].Players.Count, "Number of players on team A");
			Assert.AreEqual(2, league2.Teams[1].Players.Count, "Number of players on team B");
			Assert.AreEqual(0, league2.Teams[2].Players.Count, "Number of players on team C");
			Assert.AreEqual("<&>' \"", league2.Teams[0].Players[1].Name, "Special character in player name");
		}

		[Test]
		public void TestLeagueClone()
		{
			var league = CreateLeague();
			var clone = league.Clone();
			
			clone.Teams[0].Name = "Team A changed";

			Assert.AreEqual(3, clone.Teams.Count, "Number of teams");
			Assert.AreEqual("Team A", league.Teams[0].Name, "name of team 1");
			Assert.AreEqual("Team A changed", clone.Teams[0].Name, "name of team 1");

			league.Teams[0].Name = "Team A also changed";

			Assert.AreEqual("Team A also changed", league.Teams[0].Name, "name of team 1");
			Assert.AreEqual("Team A changed", clone.Teams[0].Name, "name of team 1");

			clone.Teams[0].Players.Add(new LeaguePlayer() { Name = "Four", Id = "004" } );
			clone.Teams[0].Players.Add(new LeaguePlayer() { Name = "Five", Id = "005" } );

			Assert.AreEqual(3, league.Teams[0].Players.Count, "Number of players on team A");
			Assert.AreEqual(5, clone.Teams[0].Players.Count, "Number of players on team A");
		}
	}
}
