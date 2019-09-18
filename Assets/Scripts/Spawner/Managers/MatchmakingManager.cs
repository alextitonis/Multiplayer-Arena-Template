using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Spawner
{
    public class MatchmakingManager : MonoBehaviour
    {
        public static MatchmakingManager getInstance;
        void Awake() { getInstance = this; }

        Dictionary<int, GameServer> servers = new Dictionary<int, GameServer>();

        public class QueuedPlayer
        {
            public GameMode Mode;
            public Account Account;
        }
        List<QueuedPlayer> queue = new List<QueuedPlayer>();

        [System.Serializable]
        public class GameModeInfo
        {
            public GameMode Mode;
            public List<Vector3> SpawnLocations = new List<Vector3>();

            public Vector3 getRandomSpawnLocation()
            {
                int id = new System.Random().Next(0, SpawnLocations.Count);
                if (SpawnLocations[id] != null)
                {
                    Vector3 _spawn = SpawnLocations[id];
                    SpawnLocations.Remove(_spawn);

                    return _spawn;
                }

                return Vector3.zero;
            }

        }
        [SerializeField] List<GameModeInfo> gameModeInfo = new List<GameModeInfo>();

        System.Random rnd = new System.Random();
        readonly int gameModes = 3;

        public void Play(Account account, GameMode Mode)
        {
            UnityEngine.Debug.Log("Play: " + account.Name);

            QueuedPlayer player = new QueuedPlayer();
            player.Account = account;
            player.Mode = Mode;

            if (queue.Contains(player))
            {
                Server.getInstance.Log("Player with name: " + account.Name + " and with ID: " + account.ID + " tried to enter the matchmaking queue, while he was already inside!", LogType.Warning);
                Client.getInstance.PlayResponse(false, "Already in matchmaking!", account.ID);
                return;
            }

            queue.Add(player);
            Client.getInstance.PlayResponse(true, "", account.ID);
        }
        public void LeaveQueue(Account account)
        {
            queue.Remove(queue.Find(x => x.Account == account));
        }
        public void CreateServer(List<Account> players, GameMode Mode)
        {
            GameModeInfo info = gameModeInfo.Find(x => x.Mode == Mode);
            if (info == null)
            {
                Server.getInstance.Log("Couldn't find game mode info!", LogType.Warning);
                goto creationFailed;
            }

            int id = rnd.Next(1, int.MaxValue);
            int maxTries = int.MaxValue;
            int currentTry = 0;

            while (servers.ContainsKey(id))
            {
                id = rnd.Next(0, int.MaxValue);

                currentTry++;
                if (currentTry >= maxTries)
                {
                    Server.getInstance.Log("Max game servers reached!", LogType.Warning);
                    goto creationFailed;
                }
            }

            int port = Holder.GetPort();
            if (port == Holder.InvalidPort)
                goto creationFailed;

            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = Server.getInstance.gameServerPath;
            p.CreateNoWindow = false;
            p.WindowStyle = ProcessWindowStyle.Normal;
            p.Arguments = "-port " + port + " -spawner " + 4297 + " -id " + id;

            Process process = new Process();
            process.StartInfo = p;
            process.Start();

            GameServer server = new GameServer(id, Mode, players, port, process);
            Holder.servers.Add(server);

            foreach (var i in server.players)
                i.ServerID = id;

            return;

            creationFailed:
            {
                Server.getInstance.Log("Game server creation failed!", LogType.Warning);

                foreach (var i in players)
                    queue.Add(new QueuedPlayer { Mode = Mode, Account = i });

                return;

            }
        }

        public void GameEnded(GameServer server)
        {
            if (server == null)
                return;

            if (!Holder.servers.Contains(server))
                return;

            foreach (var i in server.players)
                Client.getInstance.GameEnded(i.ID);

            Holder.servers.Remove(server);

            /*   if (server == null)
                   return;

               if (!servers.ContainsKey(server.ID))
                   return;

               foreach (var i in server.getPlayers())
               {
                   Server.getInstance.GameEnded(i.client);
                   Holder.accounts[i.client].ServerID = -1;
                   Holder.accounts[i.client].State = AccountState.InLobby;
               }

               servers.Remove(server.ID);*/
        }

        void Update()
        {
            if (queue.Count > 0)
            {
                for (int i = 0; i < gameModes; i++)
                {
                    if (i == 0)
                    {
                        List<QueuedPlayer> _players = getQueuedPlayersForGameMode(GameMode._test);

                        if (_players.Count > 0)
                        {
                            List<Account> players = new List<Account>();
                            players.Add(_players.First().Account);
                            _players.Remove(_players.First());
                            CreateServer(players, GameMode._test);
                        }
                        else
                            foreach (var j in _players)
                                queue.Add(j);
                    }
                    else if (i == 1)
                    {
                        List<QueuedPlayer> _players = getQueuedPlayersForGameMode(GameMode._3Players);

                        if (_players.Count >= 3)
                        {
                            List<Account> players = new List<Account>();

                            for (int j = 0; j < 3; j++)
                            {
                                players.Add(_players.First().Account);
                                _players.Remove(_players.First());
                            }

                            CreateServer(players, GameMode._3Players);
                        }
                        else
                            foreach (var j in _players)
                                queue.Add(j);
                    }
                    else if (i == 2)
                    {
                        List<QueuedPlayer> _players = getQueuedPlayersForGameMode(GameMode._5Players);

                        if (_players.Count >= 5)
                        {
                            List<Account> players = new List<Account>();

                            for (int j = 0; j < 5; j++)
                            {
                                players.Add(_players.First().Account);
                                _players.Remove(_players.First());
                            }

                            CreateServer(players, GameMode._5Players);
                        }
                        else
                            foreach (var j in _players)
                                queue.Add(j);
                    }
                }
            }
        }

        List<QueuedPlayer> getQueuedPlayersForGameMode(GameMode mode)
        {
            List<QueuedPlayer> _queue = queue.FindAll(x => x.Mode == mode);

            if (_queue.Count > getPlayersCountForMode(mode))
                for (int i = getPlayersCountForMode(mode) -  1; i < _queue.Count; i++)
                    _queue.Remove(_queue[i]);

            foreach (var i in _queue)
                queue.Remove(i);

            return _queue;
        }
        public GameServer getServer(int ID)
        {
            if (servers.ContainsKey(ID))
                return servers[ID];

            return null;
        }
        void RemoveFromQueue(string Name)
        {
            foreach (var i in queue)
                if (i.Account.Name == Name)
                    queue.Remove(i);
        }
        public int getPlayersCountForMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode._test:
                    return 1;
                case GameMode._3Players:
                    return 3;
                case GameMode._5Players:
                    return 5;
                default:
                    return -1;
            }
        }
    }
}