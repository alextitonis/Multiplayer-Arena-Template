using DarkRift;
using DarkRift.Client;
using System.Net;
using UnityEngine;

namespace Spawner
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

                    Client.getInstance.Welcome();
                }
                else if (msg.Tag == Packets.Play)
                {
                    Account account = new Account(reader.ReadUInt16(), reader.ReadString(), (Gender)reader.ReadInt32(), -1);
                    GameMode Mode = (GameMode)reader.ReadInt32();
                    MatchmakingManager.getInstance.Play(account, Mode);
                }
            }
        }
        public void HandleAsServer(object sender,DarkRift.Server.MessageReceivedEventArgs e)
        {
            using (Message msg = e.GetMessage())
            using (DarkRiftReader reader = msg.GetReader())
            {
                if (msg.Tag == Packets.Welcome)
                {
                    int ServerID = reader.ReadInt32();

                    GameServer server = Holder.servers.Find(x => x.ID == ServerID);
                    if (server != null)
                    {
                        if (server.players.Count <= 0)
                            return;

                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(e.Client.ID);
                            writer.Write(server.ID);
                            writer.Write((int)server.Mode);

                            writer.Write(server.players.Count);
                            foreach (var i in server.players)
                            {
                                writer.Write(i.ID);
                                writer.Write(i.Name);
                                writer.Write((int)i.Gender);
                            }

                            using (Message _msg = Message.Create(Packets.Welcome, writer))
                                e.Client.SendMessage(_msg, SendMode.Reliable);
                        }
                    }
                    else
                        Debug.Log("An unknown game server connected, with ID: " + ServerID + "!");
                }
                else if (msg.Tag == Packets.GameserverReady)
                {
                    int serverID = reader.ReadInt32();

                    GameServer server = Holder.servers.Find(x => x.ID == serverID);
                    if (server == null)
                    {
                        Debug.Log("Server ID: " + serverID + " was not found in the game servers list!");
                        return;
                    }

                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(server.players.Count);
                        foreach (var i in server.players)
                            writer.Write(i.ID);

                        /* This wont work in local host
                        string externalip = new WebClient().DownloadString("http://icanhazip.com");
                        writer.Write(externalip);*/
                        writer.Write(server.ID);

                        writer.Write("127.0.0.1");
                        writer.Write(server.Port);

                        Client.getInstance.Send(writer, Packets.GameserverReady);
                    }
                }
                else if (msg.Tag == Packets.GameIsEmpty)
                {
                    int serverID = reader.ReadInt32();

                    GameServer server = Holder.servers.Find(x => x.ID == serverID);
                    if (server != null)
                    {
                        server.Process.Close();
                        Holder.servers.Remove(server);
                    }
                }
            }
        }
    }
}