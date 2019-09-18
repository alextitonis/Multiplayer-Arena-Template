using UnityEngine;

namespace Client
{
    public class Player : MonoBehaviour
    {
        public ushort clientID { get; private set; }
        public string Name { get; private set; }

        Animator anim;

        public bool isLocal { get { return clientID == Client.localID; } }

        public void SetUp(ushort clientID, string Name)
        {
            this.clientID = clientID;
            this.Name = Name;

            anim = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            if (!isLocal)
                return;

            Move();
            Jump();
        }

        void Move()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool running = Input.GetKeyDown(KeyCode.LeftShift);

            if (horizontal != 0f || vertical != 0f)
                HandleData.getInstance.client.Move(horizontal, vertical, running);
        }
        void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                HandleData.getInstance.client.Jump();
        }

        public void Animate(byte speed)
        {
            anim.SetFloat("Speed", speed);
        }
    }
}