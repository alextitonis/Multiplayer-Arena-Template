using DarkRift;
using DarkRift.Client.Unity;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(UnityClient))]
    public class GameClient : MonoBehaviour
    {
        public static Client getInstance;
        public static ushort localID;

        [HideInInspector] public UnityClient client;

        public void Init(string _ip, int _port)
        {
            client = GetComponent<UnityClient>();
            if (client == null)
                Destroy(gameObject);

            client.Connect(IPAddress.Parse(_ip), _port, IPVersion.IPv4);

            client.MessageReceived += MessageReceived;
            client.Disconnected += ServerDisconnected;
        }

        public void CloseConnection()
        {
            client.Disconnect();
        }

        private void ServerDisconnected(object sender, DarkRift.Client.DisconnectedEventArgs e)
        {
            Debug.Log("Disconnected");
            if (!e.LocalDisconnect)
            {
                Debug.Log("Server was disconnected");
                MenuManager.getInstance.Change(Screen.Lobby);
            }
        }
        private void MessageReceived(object sender, DarkRift.Client.MessageReceivedEventArgs e)
        {
            HandleData.getInstance.Handle(sender, e);
        }

        public void Send(DarkRiftWriter writer, ushort packetID, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetID, writer))
                client.SendMessage(msg, sendMode);
        }

        public void Move(float horizontal, float vertical, bool running)
        {
            byte _running = running ? (byte)0 : (byte)1;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(horizontal);
                writer.Write(vertical);
                writer.Write(_running);

                Send(writer, Packets.Move);
            }
        }
        public void Jump()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(0);

                Send(writer, Packets.Jump);
            }
        }
    }
}