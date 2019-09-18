using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Spawner
{
    public class Holder
    {
        public const int MinPort= 0 ;
        public const int MaxPort= 100;
        public const int InvalidPort = -1;

        public static List<GameServer> servers = new List<GameServer>();
        static Random rnd = new Random();

        static bool PortIsUsed(int port)
        {
            if (servers.Count <= 0)
                return false;

            foreach (var i in servers)
                if (i.Port == port)
                    return true;

            return false;
        }
        static bool PortIsInUse(int port)
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(ipAddress, 666);
                tcpListener.Start();
            }
            catch (SocketException ex)
            {
                return true;
            }

            tcpListener.Stop();
            return false;
        }
        public static int GetPort()
        {
            if (servers.Count == MaxPort - MinPort)
                return InvalidPort;

            int port = rnd.Next(MinPort, MaxPort);
            int currentTry = 0;
            int maxTries = MaxPort - MinPort;

            while (PortIsUsed(port) && PortIsInUse(port))
            {
                port = rnd.Next(MinPort, MaxPort);

                currentTry++;

                if (currentTry >= maxTries)
                    return InvalidPort;
            }

            return port;
        }
    }
}