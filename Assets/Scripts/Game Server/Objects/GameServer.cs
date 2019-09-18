using System.Collections.Generic;
using System.Diagnostics;

namespace Spawner
{
    public class GameServer
    {
        public int ID { get; private set; }
        public GameMode Mode { get; private set; }
        public int Port { get; private set; }
        public Process Process { get; private set; }
        public List<Account> players = new List<Account>();

        public GameServer(int ID, GameMode Mode, List<Account> players, int Port, Process Process)
        {
            if (players.Count <= 0)
                return;

            if (!PlayersAppropriateForMode(players.Count, Mode))
                return;//MatchmakingManager.Setup Failed!

            this.ID = ID;
            this.Mode = Mode;
            this.players = players;
            this.Port = Port;
            this.Process = Process;

            Process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public static bool PlayersAppropriateForMode(int count, GameMode Mode)
        {
            bool ok = false;

            switch (Mode)
            {
                case GameMode._3Players:
                    ok = count == 3;
                    break;
                case GameMode._5Players:
                    ok = count == 5;
                    break;
                case GameMode._test:
                    ok = true;
                    break;
                default:
                    ok = false;
                    break;
            }

            return ok;
        }
    }
}