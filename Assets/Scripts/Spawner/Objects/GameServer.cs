using System.Collections.Generic;
using System.Diagnostics;

namespace GameServer
{
    public class GameServer
    {
        public int ID { get; private set; }
        public GameMode Mode { get; private set; }
        public Clock clock { get; private set; }

        public List<Player> players = new List<Player>();

        public GameServer(int ID, GameMode Mode, List<Player> players)
        {
            if (players.Count <= 0)
                return;

            this.ID = ID;
            this.Mode = Mode;
            this.players = players;

            clock = new Clock(30, false, 0, ID);
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