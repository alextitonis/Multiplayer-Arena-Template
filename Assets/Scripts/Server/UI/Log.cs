using UnityEngine;
using UnityEngine.UI;

namespace Server
{
    public class Log : MonoBehaviour
    {
        [SerializeField] Text text;

        public void SetUp(string msg, Color color)
        {
            text.text = msg;
            text.color = color;
        }
    }
}