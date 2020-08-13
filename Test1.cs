using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Torn;
using Torn.Report;
using Zoom;

//using System.Runtime.Serialization.Json;

//  Json in System.Web.Helpers, JsonQueryStringConverter in System.ServiceModel.Web, JavascriptSerializer in System.Web.Script.Serialization,
//	DataContractJsonSerializer in System.Runtime.Serialization.Json
//  Json.NET, System.Json, JsonValue in Windows.Data.Json

namespace TornWeb
{
	[TestFixture]
	public class Test1
	{
		League CreateLeague()
		{
			var league = new League(Path.Combine(Path.GetTempPath(), "TestLeague1.Torn"));

			var team = new LeagueTeam() { Name = "Team A" };
			league.AddTeam(team);

			league.Players.Add(new LeaguePlayer() { Name = "One", Id = "001", Comment = "one" } );
			team.Players.Add(league.Players[0]);
			league.Players.Add(new LeaguePlayer() { Name = "<&>' \"", Id = "002", Comment = "two" } );
			team.Players.Add(league.Players[1]);
			league.Players.Add(new LeaguePlayer() { Name = "Three", Id = "003" } );
			team.Players.Add(league.Players[2]);

			team = new LeagueTeam() { Name = "Team B" };
			league.AddTeam(team);

			league.Players.Add(new LeaguePlayer() { Name = "Four", Id = "004", Comment = "one" } );
			team.Players.Add(league.Players[3]);
			league.Players.Add(new LeaguePlayer() { Name = "One", Id = "001" } );
			team.Players.Add(league.Players[4]);

			team = new LeagueTeam() { Name = "Team C" };
			league.AddTeam(team);

			return league;			
		}

		void AddGame(League league)
		{
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
			
			league.CommitGame(serverGame, teamDatas, GroupPlayersBy.Alias);
			league.AllGames[0].Teams[0].Colour = Colour.Red;
			league.AllGames[0].Teams[1].Colour = Colour.Green;
		}

		void AddTeam(Game game)
		{
			var gameTeam = new GameTeam();
			gameTeam.Time = new DateTime(2018, 1, 1, 12, 0, 0);
			gameTeam.Players.Add(new GamePlayer() { PlayerId = "005", Score = 1000, Colour = Colour.Blue } );
			gameTeam.Score = 1000;
			game.Teams.Add(gameTeam);
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
			AddGame(league);

			Assert.AreEqual(1, league.AllGames.Count, "game count");
//			Assert.AreEqual(1, league.Teams[0].AllPlayed.Count, "team 0 game count");
//			Assert.AreEqual(1, league.Teams[1].AllPlayed.Count, "team 1 game count");
//			Assert.AreEqual(0, league.Teams[2].AllPlayed.Count, "team 2 game count");
			Assert.AreEqual(1, league.Played(league.Teams[0]).Count, "team 0 game count");
			Assert.AreEqual(1, league.Played(league.Teams[1]).Count, "team 1 game count");
			Assert.AreEqual(0, league.Played(league.Teams[2]).Count, "team 2 game count");
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

		[Test]
		public void TestFixtureParse()
		{
			var league = CreateLeague();
			var fixture = new Fixture();
			fixture.Teams.Parse(league);

			Assert.AreEqual(3, fixture.Teams.Count, "fixture teams = 3");
			Assert.AreEqual("Team B", fixture.Teams[1].Name, "fixture Team B");

			AddGame(league);
			fixture.Games.Parse(league, fixture.Teams);

			Assert.AreEqual(1, fixture.Games.Count, "fixture games = 1");
			Assert.AreEqual(new DateTime(2018, 1, 1, 12, 0, 0), fixture.Games[0].Time, "fixture game time");
			Assert.AreEqual(2, fixture.Games[0].Teams.Count, "fixture game team count");
			Assert.AreEqual(Colour.Green, fixture.Games[0].Teams[fixture.Teams[0]], "fixture game team colour");
			Assert.AreEqual(Colour.Red, fixture.Games[0].Teams[fixture.Teams[1]], "fixture game team colour");
			
			fixture.Games.Clear();
			Assert.IsNull(fixture.BestMatch(league.AllGames[0]), "match game null");
			
			var lines = new string[] { "row", "b..", "y.p" };
			fixture.Games.Parse(lines, fixture.Teams, null, null);

			Assert.AreEqual(3, fixture.Games.Count, "fixture games = 3");
			Assert.AreEqual(Colour.Red, fixture.Games[0].Teams[fixture.Teams[0]], "fixture game team colour");
			Assert.AreEqual(Colour.Blue, fixture.Games[0].Teams[fixture.Teams[1]], "fixture game team colour");
			Assert.AreEqual(Colour.Yellow, fixture.Games[0].Teams[fixture.Teams[2]], "fixture game team colour");
			Assert.AreEqual(Colour.Orange, fixture.Games[1].Teams[fixture.Teams[0]], "fixture game team colour");
			Assert.AreEqual(Colour.White, fixture.Games[2].Teams[fixture.Teams[0]], "fixture game team colour");
			Assert.AreEqual(Colour.Purple, fixture.Games[2].Teams[fixture.Teams[2]], "fixture game team colour");
			
			var grid = fixture.Games.ToGrid(fixture.Teams);
			Assert.AreEqual(3, grid.Length);
			Assert.AreEqual("ROW", grid[0]);
			Assert.AreEqual("B..", grid[1]);
			Assert.AreEqual("Y.P", grid[2]);
			
			AddGame(league);
			Assert.AreSame(fixture.Games[0], fixture.BestMatch(league.AllGames[0]), "match game 1");
			fixture.Games.Parse("8:00\t2\t1\t3", fixture.Teams, '\t');
			Assert.AreSame(fixture.Games[3], fixture.BestMatch(league.AllGames[0]), "match game 4");
			league.AllGames[0].Teams[0].Colour = Colour.Green;
			league.AllGames[0].Teams[1].Colour = Colour.Red;
			Assert.AreSame(fixture.Games[0], fixture.BestMatch(league.AllGames[0]), "match game 1 again");
		}

		[Test]
		public void TestVictoryPoints()
		{
			var league = CreateLeague();
			league.VictoryPoints.Add(6.0);
			league.VictoryPoints.Add(4.0);
			league.VictoryPoints.Add(2.0);

			AddGame(league);
			AddTeam(league.AllGames[0]);
			AddTeam(league.AllGames[0]);
			AddTeam(league.AllGames[0]);

			league.AllGames[0].Teams[0].Score = 2000;
			
			Assert.AreEqual(6.0, league.CalculatePoints(league.AllGames[0].Teams[0], GroupPlayersBy.Alias), "2000 - 1st");
			Assert.AreEqual(0.0, league.CalculatePoints(league.AllGames[0].Teams[1], GroupPlayersBy.Alias), "0 - 5th");
			Assert.AreEqual(2.0, league.CalculatePoints(league.AllGames[0].Teams[2], GroupPlayersBy.Alias), "1000 - tied 2nd/3rd/4th A");
			Assert.AreEqual(2.0, league.CalculatePoints(league.AllGames[0].Teams[3], GroupPlayersBy.Alias), "1000 - tied 2nd/3rd/4th B");
			Assert.AreEqual(2.0, league.CalculatePoints(league.AllGames[0].Teams[4], GroupPlayersBy.Alias), "1000 - tied 2nd/3rd/4th C");

			Assert.AreEqual(6.0, league.CalculatePoints(league.AllGames[0].Teams[0], GroupPlayersBy.Lotr), "2000 - 1st");
			Assert.AreEqual(6.0, league.CalculatePoints(league.AllGames[0].Teams[1], GroupPlayersBy.Lotr), "0 - 1st");
			Assert.AreEqual(4.0, league.CalculatePoints(league.AllGames[0].Teams[2], GroupPlayersBy.Lotr), "1000 - tied 1st/2nd/3rd A");
			Assert.AreEqual(4.0, league.CalculatePoints(league.AllGames[0].Teams[3], GroupPlayersBy.Lotr), "1000 - tied 1st/2nd/3rd B");
			Assert.AreEqual(4.0, league.CalculatePoints(league.AllGames[0].Teams[4], GroupPlayersBy.Lotr), "1000 - tied 1st/2nd/3rd C");
		}

		LaserGameServer stubServer;
		LaserGameServer jsonServer;

		[Test]
		public void TestJsonServer()
		{
			var webOutput = new WebOutput(8080);
			stubServer = new StubServer();

			webOutput.Games = stubServer.GetGames;
			webOutput.PopulateGame = stubServer.PopulateGame;
			webOutput.Players = stubServer.GetPlayers;
			webOutput.Leagues = new Holders();
			webOutput.Elapsed = Elapsed;

			jsonServer = new JsonServer();
			Assert.AreEqual(new TimeSpan(0, 0, 42), jsonServer.GameTimeElapsed(), "jsonServer time");

			var games = jsonServer.GetGames();
			Assert.AreEqual(3, games.Count);
			
			webOutput.Dispose();
		}

		int Elapsed()
		{
			if (stubServer == null)
				return -3;
			else if (!stubServer.Connected)
				return -2;
			
			var timeElapsed = stubServer.GameTimeElapsed();

			if (timeElapsed == TimeSpan.Zero)
				return -1;
			else
				return (int)timeElapsed.TotalSeconds;
		}
	}

	public class StubServer: LaserGameServer
	{
		public StubServer() {}
		
		protected override bool GetConnected()
		{
			return true;
		}

		public override TimeSpan GameTimeElapsed()
		{
			return new TimeSpan(0, 0, 42);
		}

		public override List<ServerGame> GetGames()
		{
			var games = new List<ServerGame>();

			games.Add(NewGame(11, new DateTime(2018, 1, 1, 12, 0, 0), new DateTime(2018, 1, 1, 12, 12, 0), "League"));
			games.Add(NewGame(12, new DateTime(2018, 1, 1, 12, 15, 0), new DateTime(2018, 1, 1, 12, 27, 0), "League"));
			games.Add(NewGame(13, new DateTime(2018, 1, 1, 12, 30, 0), new DateTime(2018, 1, 1, 12, 42, 0), "League"));
			
			return games;
		}

		ServerGame NewGame(int gameId, DateTime startTime, DateTime endTime, string description)
		{
			ServerGame game = new ServerGame();
			game.GameId = 11;
			game.Time = new DateTime(2018, 1, 1, 12, 0, 0);
			game.EndTime = new DateTime(2018, 1, 1, 12, 12, 0);
			game.Description = "League";
			game.OnServer = true;

			return game;
		}

		public override void PopulateGame(ServerGame game)
		{
			game.Players.Add(NewServerPlayer(Colour.Red, "RONiN 441", "1-50-50", "pack1", 1001));
			game.Players.Add(NewServerPlayer(Colour.Green, "", "", "pack2", 2001));
			game.Players.Add(NewServerPlayer(Colour.Red, "C", "1-50-3", "pack3", 3001));
		}

		ServerPlayer NewServerPlayer(Colour colour, string alias, string id, string pack, int score)
		{
			var player = new ServerPlayer();
			player.Colour = colour;
			player.Score = score;
			player.Pack = pack;
			if (!string.IsNullOrEmpty(id))
				player.PlayerId = id;
			if (!string.IsNullOrEmpty(alias))
				player.Alias = alias;
			player.ServerPlayerId = player.Alias;
			return player;
		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{
			var players = new List<LaserGamePlayer>();
			players.Add(NewGamePlayer("RONiN 441", "1-50-50"));
			players.Add(NewGamePlayer("B", "1-50-2"));
			players.Add(NewGamePlayer("C", "1-50-3"));
			return players;
		}
		
		LaserGamePlayer NewGamePlayer(string alias, string id)
		{
			var player = new LaserGamePlayer();
			player.Alias = alias;
			player.Id = id;
			return player;
		}
    }
}
