using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Torn;

namespace Torn5
{
    class TornTcpServer: LaserGameServer
    {
        Int32 port;
        string server;
        TcpClient client;
        int gameLimit;
        string gameFilter;
        bool hasGameFilter;

        public TornTcpServer(int limit, string filter, bool hasFilter, string _server = "", string _port = "12081")
        {
            port = Int32.Parse(_port);
            server = _server;
            gameLimit = limit;
            gameFilter = hasFilter ? filter : "";
        }

        string FetchFromTorn(string message)
        {
            client = new TcpClient(server, port);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 100;

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            data = new Byte[256];

            String responseData = String.Empty;

            while (true)
            {
                try
                {
                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    string response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    responseData += response;
                }
                catch
                {
                    break;
                }
            }

            Console.WriteLine("Received: {0}", responseData);

            // Close everything.
            stream.Close();
            client.Close();


            return responseData;
        }

        public override List<ServerGame> GetGames()
        {
            try
            {
                string message = "listGames#" + gameLimit + "#" + gameFilter;
                Console.WriteLine(message);
                string responseData = FetchFromTorn(message);
                Connected = true;

                return JsonSerializer.Deserialize<List<ServerGame>>(responseData);

            }
            catch (Exception e)
            {
                status = e.Message;
                Console.WriteLine("Exception: {0}", e.Message);

                return new List<ServerGame>();
            }

        }
        public override void PopulateGame(ServerGame game)
        {
            try
            {
                string time = game.Time.ToString("yyyy-MM-ddTHH:mm:ss");
                string message = "getGame#" + time;
                string responseData = FetchFromTorn(message);
                ServerGame populatedGame = JsonSerializer.Deserialize<ServerGame>(responseData);
                game.Description = populatedGame.Description;
                game.EndTime = populatedGame.EndTime;
                game.InProgress = populatedGame.InProgress;
                game.OnServer = populatedGame.OnServer;
                game.GameId = populatedGame.GameId;
                game.Events = populatedGame.Events;
                game.Players = populatedGame.Players;
            }
            catch (Exception e)
            {
                status = e.Message;
                Console.WriteLine("Exception: {0}", e.Message);;
            }
        }

        public override List<LaserGamePlayer> GetPlayers(string mask) 
        {
            try
            {
                string message = "listPlayers#" + mask;
                string responseData = FetchFromTorn(message);

                return JsonSerializer.Deserialize<List<LaserGamePlayer>>(responseData);
            } catch(Exception e)
            {
                status = e.Message;
                Console.WriteLine("Exception: {0}", e.Message);

                return new List<LaserGamePlayer>();
            }
        }
        public override List<LaserGamePlayer> GetPlayers(string mask, List<LeaguePlayer> players)
        {
            return GetPlayers(mask);
        }

        public override TimeSpan GameTimeElapsed()
        {
            try
            {
                string responseData = FetchFromTorn("gameTimeElapsed");

                return JsonSerializer.Deserialize<TimeSpan>(responseData);
            }
            catch (Exception e)
            {
                status = e.Message;
                Console.WriteLine("Exception: {0}", e.Message);

                return new TimeSpan();
            }
        }

    }
}
