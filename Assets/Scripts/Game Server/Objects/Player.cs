using DarkRift.Server;
using UnityEngine;

namespace GameServer
{
    [System.Serializable]
    public class Player
    {
        public IClient client;
        public ushort ID;
        public string Name;
        public Gender Gender;
        public int ServerID;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool isOk = false;
        public WorldPlayer worldPlayer;

        public Player(IClient client)
        {
            this.client = client;
        }
        public Player()
        {
        }

        void Update()
        {
            Position = worldPlayer.transform.position;
            Rotation = worldPlayer.transform.rotation;
        }

        public void Move(float horizontal, float vertical, bool running)
        {
            worldPlayer.Move(horizontal, vertical, running);
        }
        public void Jump()
        {
            worldPlayer.Jump();
        }
    }
}