using DarkRift;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using static Client.Client;

namespace Client {
    public class StartingScreenManager : MonoBehaviour
    {
        [SerializeField] Dropdown serverSelectionDropDown;

        public void Connect()
        {
            if (getInstance.client.ConnectionState == ConnectionState.Connected)
                return;

            string ip = "127.0.0.1";
            int port = 4296;
            IPVersion ipVersion = IPVersion.IPv4;

            int serverID = serverSelectionDropDown.value;
            ServerRegion region;

            if (serverID == 0)
                region = ServerRegion.Europe;
            else
                region = ServerRegion.USA;

            ServerInfo server = getInstance.getServerInfo(region);
            if (server != null)
            {
                ip = server.IP;
                port = server.Port;
                ipVersion = server.ipVersion;
            }

            getInstance.client.Connect(IPAddress.Parse(ip), port, ipVersion);
        }
    }
}