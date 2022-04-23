using Bowling_Server.Classes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Bowling_Server
{
    internal class Program
    {
        static List<Player> players = new List<Player>();
        private static Socket socket;

        static void Main(string[] args)
        {
            string ip_address = Console.ReadLine();
            int port = int.Parse(Console.ReadLine());
            Listen(ip_address, port);
            Thread matchmakingThread = new Thread(() =>
            {
                while (true)
                {
                    if (players.Count % 2 == 0 && players.Count > 0)
                    {
                        List<Player> group = new List<Player>();
                        for (int i = 0; i < 2; i++)
                        {
                            group.Add(players[0]);
                            players.Remove(players[0]);
                        }
                        StartGame(group);
                    }
                    Thread.Sleep(1000);
                }
            });
            matchmakingThread.Start();

            while (true)
            {
                Socket client = socket.Accept();
                Thread thread = new Thread(() =>
                {
                    byte[] data = new byte[1024];
                    int recv = client.Receive(data);
                    string json = Encoding.ASCII.GetString(data, 0, recv);
                    Player player = new Player();
                    player.Deserialize(json);
                    player.socket = client;
                    players.Add(player);
                });
                thread.Start();
            }
        }

        static void Listen(string ip_address, int port)
        {
            IPAddress ip = IPAddress.Parse(ip_address);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipe);
            socket.Listen(100);
        }
        public static void Send(string message, Socket client)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data);
        }
        static void CastOpponents(List<Player> group)
        {
            foreach (Player player in group)
            {
                foreach (Player opponent in group)
                {
                    if (opponent != player)
                    {
                        Send(opponent.Serialize(), player.socket);
                    }
                }
            }
        }

        static void StartGame(List<Player> group)
        {
            
            Console.WriteLine("OK");
            CastOpponents(group);
            foreach (var player in group)
            {
                Thread thread = new Thread(() =>
                {
                    byte[] data = new byte[1024];
                    player.socket.ReceiveTimeout = 60000;
                    try
                    {
                        while (true)
                        {
                            int dataLength = player.socket.Receive(data);
                            string json = Encoding.ASCII.GetString(data, 0, dataLength);
                            if (json == "11")
                            {
                                //foreach (var player in group)
                                //{
                                    player.socket.Send(Encoding.ASCII.GetBytes("11"));
                                    player.Score = new List<int>();
                                    players.Add(player);
                                //}
                                break;
                            }
                            player.Deserialize(json);
                            CastOpponents(group);
                        }
                    }
                    catch
                    {
                        try
                        {
                            Console.WriteLine("Player disconnected");
                            if (group.IndexOf(player) == 1)
                            {
                                group[0].socket.Send(Encoding.ASCII.GetBytes("11"));
                                players.Add(group[0]);
                            }
                            else
                            {
                                group[1].socket.Send(Encoding.ASCII.GetBytes("11"));
                                players.Add(group[1]);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Players disconnected. Can't process disconnection. Game ended.");
                        }
                    }
                });
                thread.Start();
            }
        }
    }
}
