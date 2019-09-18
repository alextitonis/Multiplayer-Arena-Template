using DarkRift.Server;
using System.Collections.Generic;

namespace Server
{
    public class Holder
    {
        public static Dictionary<ushort, IClient> clients = new Dictionary<ushort, IClient>();
        public static Dictionary<IClient, Account> accounts = new Dictionary<IClient, Account>();
        public static Dictionary<string, IClient> spawners = new Dictionary<string, IClient>();

        public static void SetAccountState(IClient client, AccountState state)
        {
            if (accounts.ContainsKey(client))
                accounts[client].State = state;
        }
        public static void SetAccountServerID(IClient client, int ID)
        {
            if (accounts.ContainsKey(client))
                accounts[client].ServerID = ID;
        }
        public static Account getAccount(ushort ID)
        {
            foreach (var i in accounts)
                if (i.Key.ID == ID)
                    return i.Value;

            return null;
        }
        public static void RemoveSpawner(IClient client)
        {
            foreach (var i in spawners)
                if (i.Value == client)
                    spawners.Remove(i.Key);
        }
    }
}