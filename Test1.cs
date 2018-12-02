using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Torn;
using Torn.Report;
using Zoom;

namespace TornWeb
{
	[TestFixture]
	public class Test1
	{
		League CreateLeague()
		{
			var league = new League(Path.Combine(Path.GetTempPath(), "TestLeague1.Torn"));

			var team = new LeagueTeam() { Name = "Team A" };
			league.Teams.Add(team);

			league.Players.Add(new LeaguePlayer() { Name = "One", Id = "001", Comment = "one" } );
			team.Players.Add(league.Players[0]);
			league.Players.Add(new LeaguePlayer() { Name = "<&>' \"", Id = "002", Comment = "two" } );
			team.Players.Add(league.Players[1]);
			league.Players.Add(new LeaguePlayer() { Name = "Three", Id = "003" } );
			team.Players.Add(league.Players[2]);

			team = new LeagueTeam() { Name = "Team B" };
			league.Teams.Add(team);

			league.Players.Add(new LeaguePlayer() { Name = "Four", Id = "004", Comment = "one" } );
			team.Players.Add(league.Players[3]);
			league.Players.Add(new LeaguePlayer() { Name = "One", Id = "001" } );
			team.Players.Add(league.Players[4]);

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
			league2.Load("testleague.Torn");

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

		[Test]
		public void TestLeagueCommit()
		{
			var league = CreateLeague();
			
			var serverGame = new ServerGame();
			serverGame.League = league;
			serverGame.Time = new DateTime(2018, 1, 1, 12, 0, 0);
			
			var teamDatas = new System.Collections.Generic.List<GameTeamData>();

			var teamData = new GameTeamData();
			teamData.GameTeam = new GameTeam();
			teamData.Players = new System.Collections.Generic.List<ServerPlayer>();
			teamData.Players.Add(new ServerPlayer() { PlayerId = "001" } );
			teamData.Players.Add(new ServerPlayer() { PlayerId = "002" } );
			teamData.Players.Add(new ServerPlayer() { PlayerId = "003" } );
			teamDatas.Add(teamData);
			
			teamData = new GameTeamData();
			teamData.GameTeam = new GameTeam();
			teamData.Players = new System.Collections.Generic.List<ServerPlayer>();
			teamData.Players.Add(new ServerPlayer() { PlayerId = "004" } );
			teamData.Players.Add(new ServerPlayer() { PlayerId = "nonexistent" } );
			teamDatas.Add(teamData);
			
			league.CommitGame(serverGame, teamDatas);
			
			Assert.AreEqual(1, league.AllGames.Count, "game count");
			Assert.AreEqual(1, league.Teams[0].AllPlayed.Count, "team 0 game count");
			Assert.AreEqual(1, league.Teams[1].AllPlayed.Count, "team 1 game count");
			Assert.AreEqual(0, league.Teams[2].AllPlayed.Count, "team 2 game count");
		}
		
		[Test]
		public void TestReportTemplates()
		{
			var reportTemplates = new ReportTemplates();
			reportTemplates.Parse("TeamLadder, Description, ShowColours, ChartType=bar with rug, OrderBy=score, to 2018-09-11 23:59&SoloLadder, ChartType=bar with rug, OrderBy=score, , Drop worst 10% best 10% ");
			Assert.AreEqual(2, reportTemplates.Count, "Number of report templates");
			Assert.That(reportTemplates[0].ReportType == ReportType.TeamLadder, "team ladder");
			Assert.That(reportTemplates[0].Settings.Contains("ShowColours"), "ShowColours");
			Assert.That(reportTemplates[0].To == new DateTime(2018, 09, 11, 23,59, 00), "to 2018-09-11 23:59");
			Assert.AreEqual("bar with rug", reportTemplates[0].Setting("ChartType"), "ChartType");
			Assert.That(ChartTypeExtensions.ToChartType(reportTemplates[0].Setting("ChartType")) == (ChartType.Bar | ChartType.Rug), "parse ChartType");

			Assert.That(reportTemplates[1].ReportType == ReportType.SoloLadder, "solo ladder");
			Assert.That(reportTemplates[1].Drops != null, "drops");
			Assert.That(reportTemplates[1].Drops.PercentWorst == 10.0, "worst 10%");
			Assert.That(reportTemplates[1].Drops.PercentBest == 10.0, "best 10%");
			Assert.That(reportTemplates[1].Drops.CountWorst == 0, "worst 0");
			
			var doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);
			XmlNode bodyNode = doc.CreateElement("body");
			doc.AppendChild(bodyNode);

			var root = doc.DocumentElement;
			reportTemplates.ToXml(doc, bodyNode);
			
			var reportTemplates2 = new ReportTemplates();
			reportTemplates2.FromXml(doc, bodyNode.FirstChild);
			
			Assert.AreEqual(2, reportTemplates2.Count, "XML Number of report templates");
			Assert.That(reportTemplates2[0].ReportType == ReportType.TeamLadder, "XML team ladder");
			Assert.That(reportTemplates2[0].Settings.Contains("ShowColours"), "XML ShowColours");
			Assert.That(reportTemplates2[0].To == new DateTime(2018, 09, 11, 23,59, 00), "XML to 2018-09-11 23:59");
			Assert.AreEqual("bar with rug", reportTemplates2[0].Setting("ChartType"), "XML ChartType");
			Assert.That(ChartTypeExtensions.ToChartType(reportTemplates2[0].Setting("ChartType")) == (ChartType.Bar | ChartType.Rug), "XML parse ChartType");

			Assert.That(reportTemplates2[1].ReportType == ReportType.SoloLadder, "XML solo ladder");
			Assert.That(reportTemplates2[1].Drops != null, "XML drops");
			Assert.That(reportTemplates2[1].Drops.PercentWorst == 10.0, "XML worst 10%");
			Assert.That(reportTemplates2[1].Drops.PercentBest == 10.0, "XML best 10%");
			Assert.That(reportTemplates2[1].Drops.CountWorst == 0, "XML worst 0");
		}
		
		[Test]
		public void TestHandicap()
		{
			var h = Handicap.Parse("100%");
			Assert.AreEqual(100, h.Value, "handicap is 100");
			Assert.That(h.IsZero(), "handicap is 100%");

			h = Handicap.Parse("+0");
			Assert.AreEqual(0, h.Value, "handicap is 0");
			Assert.That(h.IsZero(), "handicap is +0");

			h = Handicap.Parse("-0");
			Assert.That(h.IsZero(), "handicap is -0");

			h = Handicap.Parse("0");
			Assert.AreEqual(0, h.Value, "handicap is still 0");
		}
	}
}
