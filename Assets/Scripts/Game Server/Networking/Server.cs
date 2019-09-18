using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using UnityEngine;

namespace GameServer
{
    [RequireComponent(typeof(XmlUnityServer)),
        RequireComponent(typeof(HandleData))]
    public class Server : MonoBehaviour
    {
        public static Server getInstance;
        void Awake() { getInstance = this; }

        [HideInInspector] public XmlUnityServer server;
        [HideInInspector] public HandleData handleData;

        public bool gameStarted = false;
        public Clock clock;

        public void _Start()
        {
            Application.targetFrameRate = 60;

            server = GetComponent<XmlUnityServer>();
            handleData = GetComponent<HandleData>();
            clock = new Clock(60, true, 360, 1);

            server.Create(Client.getInstance.Port);

            server.Server.ClientManager.ClientConnected += ClientConnected;
            server.Server.ClientManager.ClientDisconnected += ClientDisconnected;
        }

        public void Log(string msg, LogType type)
        {
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

        bool gameEmptyIsSend = false;
        void Update()
        {
            if (gameStarted)
            {
                if (Holder.players.Count <= 0)
                {
                    if (!gameEmptyIsSend)
                    {
                        Client.getInstance.GameIsEmpty();
                        gameEmptyIsSend = true;
                    }
                }
            }
        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Debug.Log("A new player has joined!");

            e.Client.MessageReceived += HandleData;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(0);

                SendTo(e.Client, writer, Packets.PlayerJoinedGameServer);
            }
        }
        private void HandleData(object sender, MessageReceivedEventArgs e)
        {
            handleData.HandleAsServer(sender, e);
        }
        private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            DespawnPlayer(e.Client.ID);
            World.getInstance.DespawnPlayer(e.Client);
            Holder.players.Remove(e.Client);
        }

        void SendTo(IClient client, DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetId, writer))
                client.SendMessage(msg, sendMode);
        }
        void SendToAll(DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            foreach (var i in Holder.players)
                    SendTo(i.Key, writer, packetId, sendMode);
        }
        void SendToAllExcept(IClient client, DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            foreach (var i in Holder.players)
                if (i.Value.client != client)
                    SendTo(i.Key, writer, packetId, sendMode);
        }


        public void SpawnPlayer(Player player)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(player.ID);
                writer.Write(player.Name);

                writer.Write(player.Position.x);
                writer.Write(player.Position.y);
                writer.Write(player.Position.z);

                writer.Write(player.Rotation.x);
                writer.Write(player.Rotation.y);
                writer.Write(player.Rotation.z);
                writer.Write(player.Rotation.w);

                SendToAll(writer, Packets.Spawn);

                Debug.Log("Spawning player with name: " + player.Name);
            }
        }
        public void DespawnPlayer(ushort id)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(id);

                SendToAll(writer, Packets.Despawn);
            }
        }

        public void MovePlayer(Player _p, Vector3 p, Quaternion r)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(_p.ID);

                writer.Write(p.x);
                writer.Write(p.y);
                writer.Write(p.z);

                writer.Write(r.x);
                writer.Write(r.y);
                writer.Write(r.z);
                writer.Write(r.w);

                SendToAll(writer, Packets.Move, SendMode.Unreliable);
            }
        }
        public void AnimatePlayer(Player p,byte speed)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(p.ID);

                writer.Write(speed);

                SendToAll(writer, Packets.AnimatePlayer, SendMode.Unreliable);
            }
        }
    }
}