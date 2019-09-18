using DarkRift;
using DarkRift.Server;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    [RequireComponent(typeof(Server))]
    public class HandleData : MonoBehaviour
    {
        Server server;

        void Awake()
        {
            server = GetComponent<Server>();
        }

        public void Handle(object sender, MessageReceivedEventArgs e)
        {
            using (Message msg = e.GetMessage())
            using (DarkRiftReader reader = msg.GetReader())
            {
                if (msg.Tag == Packets.Welcome)
                {
                    string region = reader.ReadString();
                    Debug.Log("Region Connected: " + region);

                    if (Holder.spawners.ContainsKey(region))
                        Debug.Log("Region: " + region + " was already inside the list!");
                    else
                        Holder.spawners.Add(region, e.Client);

                    if (Holder.clients.ContainsKey(e.Client.ID))
                        Holder.clients.Remove(e.Client.ID);
                }
                else if (msg.Tag == Packets.Login)
                {
                    string email = reader.ReadString();
                    string password = reader.ReadString();

                    Debug.Log("Got Login: email: " + email + " password: " + password);

                    Database.getInstance.Login(e.Client, email, password);
                }
                else if (msg.Tag == Packets.Registration)
                {
                    string email = reader.ReadString();
                    string password = reader.ReadString();
                    string _name = reader.ReadString();
                    Gender gender = (Gender)reader.ReadInt32();

                    Database.getInstance.Register(e.Client, email, password, _name, gender);
                }
                else if (msg.Tag == Packets.Play)
                {
                    GameMode mode = (GameMode)reader.ReadInt32();
                    string region = reader.ReadString();

                    MatchmakingManager.getInstance.Play(Holder.accounts[e.Client], mode, region);
                }
                else if (msg.Tag == Packets.PlayResponse)
                {
                    bool ok = reader.ReadBoolean();
                    string response = reader.ReadString();
                    ushort id = reader.ReadUInt16();

                    IClient client = Holder.clients[id];
                    if (client != null)
                        Server.getInstance.Play(client, ok, response);
                }
                else if (msg.Tag == Packets.GameserverReady)
                {
                    int count = reader.ReadInt32();

                    if (count <= 0)
                        return;

                    List<ushort> _players = new List<ushort>();
                    for (int i = 0; i < count; i++)
                        _players.Add(reader.ReadUInt16());

                    int serverID = reader.ReadInt32();

                    string _ip = reader.ReadString();
                    int _port = reader.ReadInt32();

                    List<Account> players = new List<Account>();
                    foreach (var i in _players)
                    {
                        players.Add(Holder.getAccount(i));
                        Holder.getAccount(i).ServerID = serverID;
                    }

                    foreach (var i in players)
                        Server.getInstance.SendConnectionPacket(i.Client, _ip, _port);
                }
                else if (msg.Tag == Packets.GameEnded)
                {
                    ushort id = reader.ReadUInt16();

                    IClient client = Holder.clients[id];
                    if (client != null)
                    {
                        Server.getInstance.GameEnded(client);
                        Holder.accounts[client].ServerID = -1;
                    }
                }
            }
        }
    }
}