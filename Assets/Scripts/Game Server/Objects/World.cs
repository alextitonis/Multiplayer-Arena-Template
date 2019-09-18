using DarkRift.Server;
using System.Collections.Generic;
using UnityEngine;

namespace GameServer
{
    public class World : MonoBehaviour
    {
        public static World getInstance;
        void Awake() { getInstance = this; }

        [SerializeField] GameObject mapParent;
        [SerializeField] GameObject playerPrefab;

        List<Collider> colliders = new List<Collider>();
        List<WorldPlayer> players = new List<WorldPlayer>();

        void Start()
        {
            LoadAllColliders();
        }

        void LoadAllColliders()
        {
            foreach (var i in mapParent.GetComponentsInChildren<Collider>())
                colliders.Add(i);

            Debug.Log("Loaded " + colliders.Count + " colliders!");
        }

        public void SpawnPlayer(ushort id, IClient client)
        {
            Player player = Client.getInstance.gameServer.players.Find(x => x.ID == id);
            if (player == null)
            {
                Debug.Log("Found invalid client id: " + id);
                return;
            }

            player.client = client;
            player.isOk = true;
            Holder.players.Add(client, player);
            Holder.PlayersOnline++;

            if (Holder.PlayersOnline == Client.getInstance.gameServer.players.Count)
            {
                foreach (var i in Holder.players)
                    Server.getInstance.SpawnPlayer(i.Value);

                Server.getInstance.gameStarted = true;
            }

            GameObject p = Instantiate(playerPrefab, player.Position, player.Rotation);
            WorldPlayer _p = p.GetComponent<WorldPlayer>();
            _p.SetUp(player);

            players.Add(_p);
            Holder.players[client].worldPlayer = _p;
        }
        public void DespawnPlayer(IClient client)
        {
            Destroy(getPlayer(client).gameObject);
            players.Remove(getPlayer(client));
        }
        public void DespawnPlayer(ushort id)
        {
            Destroy(getPlayer(id).gameObject);
            players.Remove(getPlayer(id));
        }
        
        public WorldPlayer getPlayer(ushort id) { return players.Find(x => x.player.ID == id); }
        public WorldPlayer getPlayer(IClient client) { return players.Find(x => x.player.client == client); }
    }
}