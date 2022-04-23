using System;
using System.Net;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Dns.GetHostAddresses("mootfrost.ru")[0]);
        }
    }
}
