using System;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class LogManager : MonoBehaviour
    {
        public static LogManager getInstance;
        void Awake() { getInstance = this; }

        [SerializeField] int maxMsgs = 25;
        [SerializeField] GameObject textPrefab;
        [SerializeField] Transform spawnLocation;

        List<GameObject> textMessages = new List<GameObject>();

        public void Log(string msg, LogType type)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            Color c = Color.white;

            switch (type)
            {
                case LogType.Info:
                    c = Color.white;
                    break;
                case LogType.Warning:
                    c = Color.yellow;
                    break;
                case LogType.Error:
                    c = Color.red;
                    break;
            }

            if (textMessages.Count >= maxMsgs)
            {
                Destroy(textMessages[0]);
                textMessages.Remove(textMessages[0]);
            }

            msg = "[" + DateTime.Now + "] " + msg;

            GameObject go = Instantiate(textPrefab, spawnLocation);
            go.GetComponent<Log>().SetUp(msg, c);

            textMessages.Add(go);
        }
    }
}