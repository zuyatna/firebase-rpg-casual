using System;
using System.Collections;
using Firebase.Auth;
using Firebase.Database;
using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase
{
    public class UserDataManager : MonoBehaviour
    {
        [Header("Firebase")] 
        private DependencyStatus m_DependencyStatus;
        private DatabaseReference m_DatabaseReference;

        [Header("User Data")] 
        public Text usernameText;
        public Text levelText;
        public Text xpText;
        public Text diamondText;
        public Text goldText;

        private void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                m_DependencyStatus = task.Result;

                if (m_DependencyStatus == DependencyStatus.Available)
                {
                    m_DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                }
                else
                {
                    Debug.LogWarning($"Could not resolve all Firebase dependencies: {m_DependencyStatus}");
                }
            });
        }

        private void Start()
        {
            StartCoroutine(LoadUserData());
        }

        private IEnumerator LoadUserData()
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).GetValueAsync();

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to load user data task with: {dbTask.Exception}");
            }
            else if (dbTask.Result.Value == null)
            {
                levelText.text = "Level: 0";
                xpText.text = "Xp: 0";
                diamondText.text = "Diamond: 0";
                goldText.text = "Gold: 0";
            }
            else
            {
                DataSnapshot snapshot = dbTask.Result;

                UserInfo.Instance.level = int.Parse(snapshot.Child("level").Value.ToString());
                UserInfo.Instance.xp = int.Parse(snapshot.Child("xp").Value.ToString());
                UserInfo.Instance.diamond = int.Parse(snapshot.Child("diamond").Value.ToString());
                UserInfo.Instance.gold = int.Parse(snapshot.Child("gold").Value.ToString());

                levelText.text = $"Level: {snapshot.Child("level").Value}";
                xpText.text = $"Xp: {snapshot.Child("xp").Value}";
                diamondText.text = $"Diamond: {snapshot.Child("diamond").Value}";
                goldText.text = $"Gold: {snapshot.Child("gold").Value}";

                usernameText.text = snapshot.Child("username").Value.ToString();
            }
        }

        private IEnumerator UpdateUsernameAuth(string username)
        {
            UserProfile profile = new UserProfile { DisplayName = username };

            var profileTask = UserInfo.Instance.User.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(() => profileTask.IsCompleted);

            Debug.LogWarning(profileTask.Exception != null
                ? $"Failed to update profile task with: {profileTask.Exception}"
                : $"User register successfully: {UserInfo.Instance.User.DisplayName}, {UserInfo.Instance.User.Email}");
        }
        
        private IEnumerator UpdateUsernameDatabase(string username)
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).Child("username").SetValueAsync(username);

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to add username task with: {dbTask.Exception}");
            }
            else
            {
                Debug.Log("Success to add username user");
            }
        }

        public void Logout()
        {
            UserInfo.Instance.Auth.SignOut();
            
            SceneManagement.ChangeScene("Login");
        }
    }
}
