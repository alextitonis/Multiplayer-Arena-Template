using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using UnityEngine;

namespace Server
{
    [RequireComponent(typeof(XmlUnityServer)),
        RequireComponent(typeof(HandleData))]
    public class Server : MonoBehaviour
    {
        public static Server getInstance;
        void Awake() { getInstance = this; }

        [HideInInspector] public XmlUnityServer server;
        [HideInInspector] public HandleData handleData;

        void Start()
        {
            Application.targetFrameRate = -1;

            server = GetComponent<XmlUnityServer>();
            handleData = GetComponent<HandleData>();

            server.Server.ClientManager.ClientConnected += ClientConnected;
            server.Server.ClientManager.ClientDisconnected += ClientDisconnected;
        }

        public void Log(string msg, LogType type)
        {
            LogManager.getInstance.Log(msg, type);

            switch (type)
            {
                case LogType.Error:
                    msg = "Error: " + msg;
                    break;
                case LogType.Info:
                    msg = "Info: " + msg;
                    break;
                case LogType.Warning:
                    msg = "Warning: " + msg;
                    break;
            }
            Debug.Log(msg);
        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (!Holder.clients.ContainsKey(e.Client.ID))
            {
                Log("A new client has connected with ID: " + e.Client.ID, LogType.Info);

                Holder.clients.Add(e.Client.ID, e.Client);
                e.Client.MessageReceived += HandleData;
                Welcome(e.Client);
            }
            else
                Log("A client with id: " + e.Client + " was already connected and is trying to reconnect!", LogType.Warning);

            foreach (var i in Holder.clients.Values)
                if (i != e.Client)
                    SendPlayerCount(i);
        }
        private void HandleData(object sender, MessageReceivedEventArgs e)
        {
            handleData.Handle(sender, e);
        }
        private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            if (Holder.clients.ContainsKey(e.Client.ID))
            {
                Holder.clients.Remove(e.Client.ID);

                if (Holder.accounts.ContainsKey(e.Client))
                {
                    //  if (Holder.accounts[e.Client].ServerID != -1)
                    //    MatchmakingManager.getInstance.getServer(Holder.accounts[e.Client].ServerID).RemovePlayer(e.Client.ID);

                    Holder.accounts.Remove(e.Client);
                }

                foreach (var i in Holder.accounts.Keys)
                    SendPlayerCount(i);
            }
            else Holder.RemoveSpawner(e.Client);
        }

        public IClient getClient(ushort id) { return server.Server.ClientManager.GetClient(id); }
        public IClient[] getAllClients() { return server.Server.ClientManager.GetAllClients(); }

        public void SendTo(IClient client, DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetId, writer))
                client.SendMessage(msg, sendMode);
        }
        public void SendToEveryone(DarkRiftWriter writer, ushort packetId, AccountState accountState, int serverID = -1, SendMode sendMode = SendMode.Reliable)
        {
            if (accountState != AccountState.InGame)
            {
                foreach (var i in Holder.clients.Values)
                {
                    if (accountState != AccountState.All)
                    {
                        if (Holder.accounts[i].State == accountState)
                            SendTo(i, writer, packetId, sendMode);
                    }
                    else
                    {
                        SendTo(i, writer, packetId, sendMode);
                    }
                }
            }

            //  if (serverID >= 0)
            //     foreach (var i in MatchmakingManager.getInstance.getServer(serverID).getPlayers())
            //            SendTo(i.client, writer, packetId, sendMode);

            if (serverID < 0)
                Log("Found invalid server id: " + serverID, LogType.Warning);
        }
        public void SendToEveryoneExcept(IClient client, DarkRiftWriter writer, ushort packetId, AccountState accountState, int serverID = -1, SendMode sendMode = SendMode.Reliable)
        {
            if (accountState != AccountState.InGame)
            {
                foreach (var i in Holder.clients.Values)
                {
                    if (accountState != AccountState.All)
                    {
                        if (Holder.accounts[i].State == accountState)
                            if (i != client)
                                SendTo(i, writer, packetId, sendMode);
                    }
                    else
                    {
                        if (i != client)
                            SendTo(i, writer, packetId, sendMode);
                    }
                }
            }

            //   if (serverID >= 0)
            //      foreach (var i in MatchmakingManager.getInstance.getServer(serverID).getPlayers())
            //         if (i.client != client)
            //            SendTo(i.client, writer, packetId, sendMode);

            if (serverID < 0)
                Log("Found invalid server id: " + serverID, LogType.Warning);
        }
        public void SendToSpawner(string region, DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            IClient client = Holder.spawners[region];

            if (client != null)
                SendTo(client, writer, packetId, sendMode);
            else
                Debug.Log("Spawner: " + region + " not found!");
        }

        public void Welcome(IClient client)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(client.ID);

                writer.Write(Holder.spawners.Count);
                if (Holder.spawners.Count > 0)
                    foreach (var i in Holder.spawners)
                        writer.Write(i.Key);

                SendTo(client, writer, Packets.Welcome);
            }
        }

        public void LoginResponse(bool ok, string response, IClient client)
        {
            if (ok)
                response = "ok";

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(ok);
                writer.Write(response);

                Debug.Log("login response: " + response);

                SendTo(client, writer, Packets.Login);
            }

            if (ok)
                SendPlayerCount(client);
        }
        public void RegistrationResponse(bool ok, string response, IClient client)
        {
            if (ok)
                response = "ok";

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(ok);
                writer.Write(response);

                SendTo(client, writer, Packets.Registration);
            }
        }

        public void SendPlayerCount(IClient client)
        {
            if (Holder.accounts.ContainsKey(client))
            {
                if (Holder.accounts[client].State == AccountState.InLobby)
                {
                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(Holder.clients.Count);

                        SendTo(client, writer, Packets.OnlinePlayersCount);
                    }
                }
            }
        }
        public void Play(IClient client, bool canPlay, string respone)
        {
            if (canPlay)
                respone = "Can Play!";

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(canPlay);
                writer.Write(respone);

                SendTo(client, writer, Packets.Play);
            }
        }
        public void MatchmakingDone(IClient client, bool ok)
        {
            Debug.Log("matchmaking done: " + ok);
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(ok);

                SendTo(client, writer, Packets.MatchmakingDone);
            }
        }
        public void GameEnded(IClient client)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(0);

                SendTo(client, writer, Packets.GameEnded);
            }
        }
        public void SendConnectionPacket(IClient client, string _ip, int _port)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(_ip);
                writer.Write(_port);

                SendTo(client, writer, Packets.GameserverReady);
            }
        }
    }
}