using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class Holder
    {
        public static Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();

        public static int PlayersOnline = 0;
    }
}