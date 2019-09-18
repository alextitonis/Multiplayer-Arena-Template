using DarkRift.Server;

namespace Server
{
    [System.Serializable]
    public class Account
    {
        public IClient Client;
        public string Email;
        public string Name;
        public string Password;
        public Gender Gender;
        public AccountState State;
        public int ServerID;
        public string Region;

        public Account(IClient Client, string Email, string Name, string Password, Gender Gender, AccountState State, int ServerID, string Region)
        {
            this.Client = Client;
            this.Email = Email;
            this.Name = Name;
            this.Password = Password;
            this.Gender = Gender;
            this.State = State;
            this.ServerID = ServerID;
            this.Region = Region;
        }
    }
}