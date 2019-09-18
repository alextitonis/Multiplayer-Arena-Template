using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using UnityEngine;

namespace Spawner
{
    [RequireComponent(typeof(XmlUnityServer)),
        RequireComponent(typeof(HandleData))]
    public class Server : MonoBehaviour
    {
        public static Server getInstance;
        void Awake() { getInstance = this; }

        public string gameServerPath;

        [HideInInspector] public XmlUnityServer server;
        [HideInInspector] public HandleData handleData;

        void Start()
        {
            Application.targetFrameRate = -1;

            server = GetComponent<XmlUnityServer>();
            handleData = GetComponent<HandleData>();

            server.Server.ClientManager.ClientConnected += ClientConnected;
            server.Server.ClientManager.ClientDisconnected += ClientDisconnected;
        }

        public void Log(string msg, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    msg = "Error: " + msg;
                    break;
                case LogType.Info:
                    msg = "Info: " + msg;
                    break;
                case LogType.Warning:
                    msg = "Warning: " + msg;
                    break;
            }
            Debug.Log(msg);
        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Debug.Log("Client connected");

            e.Client.MessageReceived += HandleData;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);

                SendTo(e.Client, writer, Packets.WelcomeClient);
            }
        }
        private void HandleData(object sender, MessageReceivedEventArgs e)
        {
            handleData.HandleAsServer(sender, e);
        }
        private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

        }

        void SendTo(IClient client, DarkRiftWriter writer, ushort packetId, SendMode sendMode = SendMode.Reliable)
        {
            using (Message msg = Message.Create(packetId, writer))
                client.SendMessage(msg, sendMode);
        }
    }
}