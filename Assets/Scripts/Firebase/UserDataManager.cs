using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase
{
    public class UserDataManager : MonoBehaviour
    {
        [Header("User Data")] 
        public Text usernameText;
        public Text levelText;
        public Text xpText;
        public Text diamondText;
        public Text goldText;

        private void Awake()
        {
            usernameText.text = UserInfo.Instance.username;
            levelText.text = "Level: " +UserInfo.Instance.level;
            xpText.text = "Xp: " +UserInfo.Instance.xp;
            diamondText.text = "Diamond: " +UserInfo.Instance.diamond;
            goldText.text = "Gold: " +UserInfo.Instance.gold;
        }

        public void Logout()
        {
            UserInfo.Instance.Logout();
            
            SceneManagement.ChangeScene("Login");
        }
    }
}
