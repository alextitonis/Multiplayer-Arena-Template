using DarkRift;
using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Server
{
    public class MatchmakingManager : MonoBehaviour
    {
        public static MatchmakingManager getInstance;
        void Awake() { getInstance = this; }

        public void Play(Account account, GameMode Mode, string Region)
        {
            account.Region = Region;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(account.Client.ID);
                writer.Write(account.Name);
                writer.Write((int)account.Gender);
                writer.Write((int)Mode);

                Server.getInstance.SendToSpawner(Region, writer, Packets.Play);
            }
        }
    }
}