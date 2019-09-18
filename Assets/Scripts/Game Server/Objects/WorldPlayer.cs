using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameServer
{
    [RequireComponent(typeof(CharacterController))]
    public class WorldPlayer : MonoBehaviour
    {
        [SerializeField] float walkSpeed = 6.0f;
        [SerializeField] float runSpeed = 8.0f;
        [SerializeField] float jumpSpeed = 8.0f;
        [SerializeField] float gravity = 20.0f;

        CharacterController controller;

        public Player player { get; private set; }

        Vector3 moveDirection = Vector3.zero;
        Vector3 previousPosition = Vector3.zero;
        byte previusAnimationSpeed = 0;
        InputBuffer inputBuffer = new InputBuffer();

        public void SetUp(Player player)
        {
            this.player = player;

            controller = GetComponent<CharacterController>();

            previousPosition = transform.position;
        }

        void Update()
        {
            Move();
            
            if (transform.position != previousPosition)
            {
                Server.getInstance.MovePlayer(player, transform.position, transform.rotation);
                previousPosition = transform.position;
            }
        }

        void Move()
        {
            _Input i = new _Input(0f, 0f, false);
            if (inputBuffer.hasInput)
                i = inputBuffer.Get();

            float _speed = walkSpeed;
            if (i.running)
                _speed = runSpeed;

            if (controller.isGrounded)
            {
                moveDirection = new Vector3(i.horizontal, 0.0f, i.vertical);
                moveDirection *= _speed;
            }

            if (!controller.isGrounded)
				moveDirection.y -= gravity * Time.deltaTime;

            controller.Move(moveDirection * Time.deltaTime);

            byte s = 0;
            if (i.horizontal != 0.0f && i.vertical != 0.0f)
            {
                if (i.running)
                    s = 2;
                else
                    s = 1;
            }
            if (previusAnimationSpeed != s)
            {
                Server.getInstance.AnimatePlayer(player, s);
                previusAnimationSpeed = s;
            }
        }
        public void Move(float horizontal, float vertical, bool running)
        {
            inputBuffer.Add(horizontal, vertical, running);
        }
        public void Jump()
        {
            if (controller.isGrounded)
                moveDirection.y = jumpSpeed;

            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}