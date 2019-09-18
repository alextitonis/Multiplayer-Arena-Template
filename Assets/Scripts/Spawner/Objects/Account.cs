using DarkRift.Server;

namespace Spawner
{
    [System.Serializable]
    public class Account
    {
        public ushort ID;
        public string Name;
        public Gender Gender;
        public int ServerID;

        public Account(ushort ID, string Name, Gender Gender, int ServerID)
        {
            this.ID = ID;
            this.Name = Name;
            this.Gender = Gender;
            this.ServerID = ServerID;
        }
    }
}