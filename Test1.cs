using System;
using NUnit.Framework;
using Torn;

namespace TornWeb
{
	[TestFixture]
	public class Test1
	{
		[Test]
		public void TestLeagueSaveLoad()
		{
			var league = new League();

			var team = new LeagueTeam() { Name = "Team A" };
			league.Teams.Add(team);

			team.Players.Add(new LeaguePlayer() { Name = "One", Id = "001", Comment = "one" } );
			team.Players.Add(new LeaguePlayer() { Name = "Two", Id = "002", Comment = "two" } );
			team.Players.Add(new LeaguePlayer() { Name = "Three", Id = "003" } );

			team = new LeagueTeam() { Name = "Team B" };
			league.Teams.Add(team);

			team.Players.Add(new LeaguePlayer() { Name = "Four", Id = "004", Comment = "one" } );
			team.Players.Add(new LeaguePlayer() { Name = "One", Id = "001" } );

			team = new LeagueTeam() { Name = "Team C" };
			league.Teams.Add(team);

			league.Save("testleague.Torn");

			var league2 = new League();
			league2.Load("testleague5.Torn");
			
			Assert.AreEqual(3, league2.Teams.Count, "Number of teams");
			Assert.AreEqual("Team A", league2.Teams[0].Name, "name of team 1");
			Assert.AreEqual(3, league2.Teams[0].Players.Count, "Number of players on team A");
			Assert.AreEqual(2, league2.Teams[1].Players.Count, "Number of players on team B");
			Assert.AreEqual(0, league2.Teams[2].Players.Count, "Number of players on team C");
		}
	}
}
