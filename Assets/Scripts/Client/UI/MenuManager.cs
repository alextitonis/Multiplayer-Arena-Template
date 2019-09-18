using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager getInstance;
        void Awake() { getInstance = this; }

        [System.Serializable]
        public class ScreenObject
        {
            public Screen screen;
            public GameObject[] objects;
        }

        [SerializeField] Screen startingScreen;
        [SerializeField] List<ScreenObject> Screens = new List<ScreenObject>();

        Screen currentScreen;

        void Start()
        {
            Change(startingScreen);
        }

        public void Change(Screen screen)
        {
            if (currentScreen == screen)
                return;

            ScreenObject obj = Screens.Find(x => x.screen == screen);

            if (obj == null)
                return;

            foreach (var i in Screens)
                foreach (var j in i.objects)
                    j.SetActive(false);

            foreach (var i in obj.objects)
                i.SetActive(true);

            currentScreen = screen;

            if (currentScreen != Screen.InGame)
                HandleData.getInstance.DestroyPlayers();

            if (currentScreen == Screen.Lobby)
                LobbyManager.getInstance.Init();
        }
    }
}