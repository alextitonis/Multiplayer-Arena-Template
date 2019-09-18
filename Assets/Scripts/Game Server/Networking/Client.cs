using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System;
using System.Net;
using UnityEngine;

namespace GameServer
{
    [RequireComponent(typeof(UnityClient)),
        RequireComponent(typeof(HandleData))]
    public class Client : MonoBehaviour
    {
        public static Client getInstance;
        public static ushort localID;

        [HideInInspector] public UnityClient client;
        public int Port = 15;
        public GameServer gameServer;
        public int gameServerID;
        public int clientID;

        void Start()
        {
            getInstance = this;

            try
            {
                Debug.Log(Application.systemLanguage);

                client = GetComponent<UnityClient>();
                if (client == null)
                    Destroy(gameObject);

                Port = Int32.Parse(Utils.GetArg("-port"));
                if (Port == -1)
                    Application.Quit();

                int spawnerPort = Int32.Parse(Utils.GetArg("-spawner"));
                int gameServerID = Int32.Parse(Utils.GetArg("-id"));
                this.gameServerID = gameServerID;

                client.Connect(IPAddress.Parse("127.0.0.1"), 4297, IPVersion.IPv4);

                client.MessageReceived += MessageReceived;
                client.Disconnected += ServerDisconnected;
                
                Server.getInstance._Start();
            }
            catch (Exception ex) { Debug.Log(ex.Message + " | " + ex.StackTrace); }
        }

        public void CloseConnection()
        {
            client.Disconnect();
        }

        private void ServerDisconnected(object sender, DarkRift.Client.DisconnectedEventArgs e)
        {
            Debug.Log("Disconnected");
            if (!e.LocalDisconnect)
            {
                Debug.Log("Server was disconnected");
                Application.Quit();
            }
        }
        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            HandleData.getInstance.HandleAsClient(sender, e);
        }

        public void Send(DarkRiftWriter writer, ushort packetID, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetID, writer))
                client.SendMessage(msg, sendMode);
        }

        public void SendWelcome()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(gameServerID);

                Send(writer, Packets.Welcome);
            }
        }
        public void GameIsEmpty()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(gameServerID);

                Send(writer, Packets.GameIsEmpty);
            }
        }
    }
}