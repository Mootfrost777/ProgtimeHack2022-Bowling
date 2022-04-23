using System;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace NetLib
{
    public static class NetLib
    {
        public static string IP;
        public static int port;
        private static Socket socket;


        public static void Connect()
        {
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
        }

        public static void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.Send(data);
        }

        public static string Receive()
        {
            byte[] data = new byte[1024];
            int recv = socket.Receive(data);
            string message = Encoding.ASCII.GetString(data, 0, recv);
            return message;
        }
    }
}
