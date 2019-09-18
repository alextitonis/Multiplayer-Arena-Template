using DarkRift;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class HandleData : MonoBehaviour
    {
        public static HandleData getInstance;
        void Awake()
        {
            getInstance = this;
        }

        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject gameClientPrefab;

        Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
        public Player getPlayer(ushort id)
        {
            if (players.ContainsKey(id))
                return players[id];

            return null;
        }
        public void DestroyPlayers()
        {
            foreach (var i in players)
                Destroy(i.Value.gameObject);

            players.Clear();
        }

        List<string> regions = new List<string>();
        public List<string> getRegions() { return regions; }
        public GameClient client { get; private set; }

        public void Handle(object sender, DarkRift.Client.MessageReceivedEventArgs e)
        {
            using (Message msg = e.GetMessage())
            {
                Debug.Log("Got new message with ID: " + msg.Tag);

                using (DarkRiftReader reader = msg.GetReader())
                {
                    if (msg.Tag == Packets.Welcome)
                    {
                        ushort clientID = reader.ReadUInt16();
                        Client.localID = clientID;

                        int count = reader.ReadInt32();
                        List<string> regions = new List<string>();
                        for (int i = 0; i < count; i++)
                            regions.Add(reader.ReadString());

                        this.regions = regions;

                        MenuManager.getInstance.Change(Screen.Login);
                    }
                    else if (msg.Tag == Packets.Login)
                    {
                        bool ok = reader.ReadBoolean();
                        string response = reader.ReadString();

                        LoginManager.getInstance.LoginResponse(ok, response);
                    }
                    else if (msg.Tag == Packets.Registration)
                    {
                        bool ok = reader.ReadBoolean();
                        string response = reader.ReadString();

                        RegistrationManager.getInstance.RegistrationResponse(ok, response);
                    }
                    else if (msg.Tag == Packets.OnlinePlayersCount)
                    {
                        uint count = reader.ReadUInt32();

                        LobbyManager.getInstance.SetOnlinePlayers(count);
                    }
                    else if (msg.Tag == Packets.Play)
                    {
                        bool canPlay = reader.ReadBoolean();
                        string reason = reader.ReadString();

                        if (canPlay)
                            LobbyManager.getInstance.StartCounter();
                        else
                            LobbyManager.getInstance.CantPlay(reason);
                    }
                    else if (msg.Tag == Packets.Spawn)
                    {
                        Debug.Log("Got spawn packet");

                        ushort clientID = reader.ReadUInt16();
                        string Name = reader.ReadString();

                        Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        Quaternion rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        Debug.Log("Player was already inside: " + players.ContainsKey(clientID));
                        if (!players.ContainsKey(clientID))
                        {
                            GameObject go = Instantiate(playerPrefab, position, rotation);
                            Player player = go.GetComponent<Player>();
                            if (player == null)
                            {
                                Destroy(go);
                                return;
                            }

                            player.SetUp(clientID, Name);
                            players.Add(clientID, player);
                        }

                        Debug.Log("Spawning player: " + Name);
                    }
                    else if (msg.Tag == Packets.Despawn)
                    {
                        ushort id = reader.ReadUInt16();

                        if (players.ContainsKey(id))
                        {
                            Destroy(players[id].gameObject);
                            players.Remove(id);
                        }
                    }
                    else if (msg.Tag == Packets.MatchmakingDone)
                    {
                        bool ok = reader.ReadBoolean();

                        Debug.Log("Matchmaking: " + ok);

                        if (ok)
                        {
                            LobbyManager.getInstance.StopCounter();
                            MenuManager.getInstance.Change(Screen.InGame);
                        }
                        else
                            Debug.Log("Matchmaking response: " + ok);
                    }
                    else if (msg.Tag == Packets.GameEnded)
                    {
                        int temp = reader.ReadInt32();

                        foreach (var i in players)
                            Destroy(i.Value.gameObject);

                        MenuManager.getInstance.Change(Screen.Lobby);

                        client.client.Disconnect();
                        Destroy(client.gameObject);
                    }
                    else if (msg.Tag == Packets.Move)
                    {
                        ushort playerID = reader.ReadUInt16();
                        Debug.Log("Moving player with ID:" + playerID);
                        Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        Quaternion rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        Player player = getPlayer(playerID);
                        if (player != null)
                        {
                            player.transform.position = pos;
                            player.transform.rotation = rot;
                        }
                    }
                    else if (msg.Tag == Packets.GameserverReady)
                    {
                        string _ip = reader.ReadString();
                        int _port = reader.ReadInt32();

                        GameObject go = Instantiate(gameClientPrefab, transform);
                        go.transform.position = Vector3.zero;
                        go.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

                        client = go.GetComponent<GameClient>();
                        client.Init(_ip, _port);
                    }
                    else if (msg.Tag == Packets.PlayerJoinedGameServer)
                    {
                        int temp = reader.ReadInt32();

                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(Client.getInstance.client.ID);
                            Debug.Log("Sending client id: " + Client.getInstance.client.ID);

                            client.Send(writer, Packets.PlayerJoinedGameServer);
                        }

                        MenuManager.getInstance.Change(Screen.InGame);
                    }
                    else if (msg.Tag == Packets.AnimatePlayer)
                    {
                        ushort playerId = reader.ReadUInt16();

                        byte speed = reader.ReadByte();

                        if (players.ContainsKey(playerId))
                            players[playerId].GetComponent<Player>().Animate(speed);
                    }
                }
            }
        }
    }
}