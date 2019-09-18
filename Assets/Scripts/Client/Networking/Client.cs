using DarkRift;
using DarkRift.Client.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(UnityClient)),
        RequireComponent(typeof(HandleData))]
    public class Client : MonoBehaviour
    {
        public static Client getInstance;
        public static ushort localID;

        [HideInInspector] public UnityClient client;

        [Serializable]
        public class ServerInfo
        {
            public ServerRegion region;
            public string IP = "127.0.0.1";
            public int Port = 4296;
            public IPVersion ipVersion = IPVersion.IPv4;
        }
        public enum ServerRegion
        {
            Europe = 0,
            USA = 1,
        }

        [SerializeField] List<ServerInfo> Servers = new List<ServerInfo>();
        public ServerInfo getServerInfo(ServerRegion region)
        {
            return Servers.Find(x => x.region == region);
        }

        void Awake()
        {
            getInstance = this;

            client = GetComponent<UnityClient>();
            if (client == null)
                Destroy(gameObject);

            client.MessageReceived += MessageReceived;
            client.Disconnected += ServerDisconnected;
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
                MenuManager.getInstance.Change(Screen.Disconnected);
            }
        }
        private void MessageReceived(object sender, DarkRift.Client.MessageReceivedEventArgs e)
        {
            HandleData.getInstance.Handle(sender, e);
        }

        void Send(DarkRiftWriter writer, ushort packetID, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetID, writer))
                client.SendMessage(msg, sendMode);
        }

        public void Login(string email, string password)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(email);
                writer.Write(password);

                Send(writer, Packets.Login);
            }
        }
        public void Register(string email, string password, string _name, Gender gender)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(email);
                writer.Write(password);
                writer.Write(_name);
                writer.Write((int)gender);

                Send(writer, Packets.Registration);
            }
        }

        public void Play(GameMode mode, string region)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write((int)mode);
                writer.Write(region);

                Send(writer, Packets.Play);
            }
        }
    }
}