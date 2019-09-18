using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager getInstance;
        void Awake() { getInstance = this; }

        [SerializeField] Text onlinePlayersText, timerText;
        [SerializeField] Dropdown gameModeDropDown, regionDropDown;
        [SerializeField] Button playButton;

        bool countTime = false;
        float time = 0;

        public void Init()
        {
            UpdateButton(true);

            foreach (var i in HandleData.getInstance.getRegions())
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = i;
                regionDropDown.options.Add(data);
            }
        }

        public void Play()
        {
            GameMode mode = (GameMode)gameModeDropDown.value;
            Client.getInstance.Play(mode, regionDropDown.options[regionDropDown.value].text);
        }

        public void SetOnlinePlayers(uint count)
        {
            onlinePlayersText.text = "Online: " + count + ", last update: " + DateTime.Now;
        }

        public void StartCounter()
        {
            UpdateButton(false);
            time = 0;
            timerText.text = "";
            countTime = true;

            Debug.Log("Joined the queue!");
        }
        public void StopCounter()
        {
            countTime = false;
            timerText.text = "";
            time = 0f;
        }
        public void CantPlay(string reason)
        {
            Debug.Log(reason);
        }

        public void UpdateButton(bool enable)
        {
            playButton.gameObject.SetActive(enable);
        }

        void Update()
        {
            if (countTime)
            {
                time = Time.time;
                timerText.text = "Searching for game: " + time;
            }
        }
    }
}