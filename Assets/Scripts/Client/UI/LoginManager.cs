using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class LoginManager : MonoBehaviour
    {
        public static LoginManager getInstance;
        void Awake() { getInstance = this; }

        [SerializeField] InputField emailInput, passwordInput;
        [SerializeField] Text log;

		//toggle save password
		//on login save in playerspref is the toggle is true
		//on change screen to this, load the password and username from playerspref if exist
        public void Login()
        {
            string email = emailInput.text;
            string password = passwordInput.text;

            Clean();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                log.text = "Email or password was empty!";
                return;
            }

            if (!email.Contains("@"))
            {
                log.text = "Invalid email!";
                return;
            }

            if (email.Length < 2 || password.Length < 2)
            {
                log.text = "Email or password is too small!";
                return;
            }

            Client.getInstance.Login(email, password);
        }

        public void LoginResponse(bool ok, string response)
        {
            Debug.Log("Login Response: " + response);

            if (ok)
                MenuManager.getInstance.Change(Screen.Lobby);
            else
                log.text = response;
        }

        void OnEnable()
        {
            Clean();
        }

        public void Clean()
        {
            emailInput.text = passwordInput.text = log.text = string.Empty;
        }

        public void Register()
        {
            MenuManager.getInstance.Change(Screen.Registration);
        }
    }
}