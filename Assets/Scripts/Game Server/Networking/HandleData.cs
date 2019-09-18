using DarkRift;
using DarkRift.Client;
using System.Collections.Generic;
using UnityEngine;

namespace GameServer
{
    [RequireComponent(typeof(Server))]
    public class HandleData : MonoBehaviour
    {
        public static HandleData getInstance;

        Server server;

        void Awake()
        {
            getInstance = this;
            server = GetComponent<Server>();
        }

        public void HandleAsClient(object sender, MessageReceivedEventArgs e)
        {
            using (Message msg = e.GetMessage())
            using (DarkRiftReader reader = msg.GetReader())
            {
                if (msg.Tag == Packets.Welcome)
                {
                    ushort clientID = reader.ReadUInt16();
                    Client.localID = clientID;

                    int serverID = reader.ReadInt32();
                    GameMode mode = (GameMode)reader.ReadInt32();

                    int count = reader.ReadInt32();
                    List<Player> players = new List<Player>();
                    for (int i = 0; i < count; i++)
                    {
                        Player player = new Player();
                        player.ServerID = serverID;
                        player.Position = Vector3.zero;
                        player.Rotation = new Quaternion(0f, 0f, 0f, 0f);
                        player.ID = reader.ReadUInt16();
                        player.Name = reader.ReadString();
                        player.Gender = (Gender)reader.ReadInt32();

                        players.Add(player);
                    }

                    Client.getInstance.gameServer = new GameServer(serverID, mode, players);

                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(serverID);

                        Client.getInstance.Send(writer, Packets.GameserverReady);
                    }
                }
                else if (msg.Tag == Packets.WelcomeClient)
                {
                    ushort clientID = reader.ReadUInt16();
                    Client.getInstance.clientID = clientID;

                    Client.getInstance.SendWelcome();
                }
            }
        }
        public void HandleAsServer(object sender,DarkRift.Server.MessageReceivedEventArgs e)
        {
            using (Message msg = e.GetMessage())
            using (DarkRiftReader reader = msg.GetReader())
            {
                if (msg.Tag == Packets.PlayerJoinedGameServer)
                {
                    ushort id = reader.ReadUInt16();

                    World.getInstance.SpawnPlayer(id, e.Client);
                }
                else if (msg.Tag == Packets.Move)
                {
                    float horizotanl = reader.ReadSingle();
                    float vertical = reader.ReadSingle();
                    bool running = reader.ReadByte() != 0;

                    Player p = Holder.players[e.Client];
                    if (p != null)
                        p.Move(horizotanl, vertical, running);
                }
                else if (msg.Tag == Packets.Jump)
                {
                    int temp = reader.ReadInt32();

                    Player p = Holder.players[e.Client];
                    if (p != null)
                        p.Jump();
                }
            }
        }
    }
}