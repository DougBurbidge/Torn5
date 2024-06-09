using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Torn;

namespace Torn5
{
    class TornTcpListener
    {
        TcpListener server = null;
        Thread tcpListenerThread = null;
        LaserGameServer laserGameServer;
        List<ServerGame> serverGames = new List<ServerGame>();
        Int32 port;

        public TornTcpListener(LaserGameServer gameServer, string remoteTornPort)
        {
            laserGameServer = gameServer;
            port = Int32.Parse(remoteTornPort);
        }

        public void Connect()
        {
            try
            {

                server = new TcpListener(IPAddress.Any, port);

                server.Start();

                tcpListenerThread = new Thread(() =>
                {
                    Byte[] bytes = new byte[256];

                    String data = null;

                    while (true)
                    {
                        if (server.Pending())
                        {
                            TcpClient client = server.AcceptTcpClient();
                            Console.WriteLine("Connected!");

                            data = null;

                            NetworkStream stream = client.GetStream();

                            int i;

                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                // Translate data bytes to a ASCII string.
                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                                Console.WriteLine("Received: {0}", data);

                                // Process the data sent by the client.
                                String response = ProcessCommand(data);

                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);

                                // Send back a response.
                                stream.Write(msg, 0, msg.Length);
                                Console.WriteLine("Sent: {0}", response);
                            }

                            client.Close();
                        }
                    }
                });

                tcpListenerThread.Start();

                
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        private String ProcessCommand(String data)
        {
            try
            {
                if (data == "listGames")
                {
                    serverGames = laserGameServer.GetGames();
                    string gamesJson = JsonSerializer.Serialize<List<ServerGame>>(serverGames);
                    return gamesJson;
                }
                if (data.StartsWith("getGame"))
                {
                    string gameTime = data.Split('#')[1];
                    ServerGame serverGame = serverGames.Find((game) => game.Time.ToString("yyyy-MM-ddTHH:mm:ss") == gameTime);
                    if (serverGame != null)
                    {
                        laserGameServer.PopulateGame(serverGame);
                        string gameJson = JsonSerializer.Serialize<ServerGame>(serverGame);
                        return gameJson;
                    } else
                    {
                        return "{ error: 'No Game Found'}";
                    }
                }
                if (data.StartsWith("listPlayers"))
                {
                    string mask = data.Split('#')[1];
                    List<LaserGamePlayer> serverPlayers = laserGameServer.GetPlayers(mask);
                    string playersJson = JsonSerializer.Serialize<List<LaserGamePlayer>>(serverPlayers);
                    return playersJson;
                }
                if(data == "gameTimeElapsed")
                {
                    TimeSpan elapsed = laserGameServer.GameTimeElapsed();
                    return JsonSerializer.Serialize<TimeSpan>(elapsed);
                }
                return "Message Recieved";
            } catch
            {
                return "{ error: 'Could not process message'}";
            }
        }

        public void Close()
        {
            tcpListenerThread?.Abort();
            server?.Stop();
        }
    }
}
