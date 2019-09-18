using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System;
using UnityEngine;

namespace Spawner
{
    [RequireComponent(typeof(UnityClient)),
        RequireComponent(typeof(HandleData))]
    public class Client : MonoBehaviour
    {
        public static Client getInstance;
        public static ushort localID;

        [HideInInspector] public UnityClient client;
        [SerializeField] string region;

        void Awake()
        {
            getInstance = this;

            client = GetComponent<UnityClient>();
            if (client == null)
                Destroy(gameObject);

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
                Application.Quit();
            }
        }
        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            HandleData.getInstance.HandleAsClient(sender, e);
        }

        public void Send(DarkRiftWriter writer, ushort packetID, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetID, writer))
                client.SendMessage(msg, sendMode);
        }

        public void Welcome()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(region);

                Send(writer, Packets.Welcome);
            }
        }

        public void PlayResponse(bool ok, string response, ushort accountID)
        {
            if (ok)
                response = "ok";

            if (string.IsNullOrEmpty(response))
                response = ok.ToString();

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(ok);
                writer.Write(response);
                writer.Write(accountID);

                Send(writer, Packets.PlayResponse);
            }
        }

        public void GameEnded(ushort id)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(id);

                Send(writer, Packets.GameEnded);
            }
        }
    }
}