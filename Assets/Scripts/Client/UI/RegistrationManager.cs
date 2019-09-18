using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class RegistrationManager : MonoBehaviour
    {
        public static RegistrationManager getInstance;
        void Awake() { getInstance = this; }

        [SerializeField] InputField emailInput, passwordInput, passwordRepeatInput, nameInput;
        [SerializeField] Slider genderSlider;
        [SerializeField] Text genderText, log;

        void Start()
        {
            genderSlider.onValueChanged.AddListener(delegate { genderText.text = ((Gender)((int)genderSlider.value)).ToString(); });
        }
        public void Register()
        {
            string email = emailInput.text;
            string password = passwordInput.text;
            string passwordRepeat = passwordRepeatInput.text;
            string name = nameInput.text;
            Gender gender = (Gender)((int)genderSlider.value);

            Clean();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRepeat) || string.IsNullOrEmpty(name))
            {
                log.text = "Invalid email or password or name!";
                return;
            }

            if (email.Length < 2 || password.Length < 2 || passwordRepeat.Length < 2 || name.Length < 2)
            {
                log.text = "Email or password or name is too small!";
                return;
            }

            if (password != passwordRepeat)
            {
                log.text = "Passwords must match!";
                return;
            }

            Client.getInstance.Register(email, password, name, gender);
        }

        public void RegistrationResponse(bool ok, string response)
        {
            if (ok)
                MenuManager.getInstance.Change(Screen.Login);
            else
                log.text = response;
        }

        void OnEnable()
        {
            Clean();
        }

        public void Clean()
        {
            emailInput.text = passwordInput.text = passwordRepeatInput.text = nameInput.text = log.text = string.Empty;
        }

        public void Back()
        {
            MenuManager.getInstance.Change(Screen.Login);
        }
    }
}