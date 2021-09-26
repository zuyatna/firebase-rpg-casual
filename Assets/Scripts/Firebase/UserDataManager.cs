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

        private int m_Level;
        private int m_Xp;
        private int m_Diamond;
        private int m_Gold;
        
        private void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                m_DependencyStatus = task.Result;

                if (m_DependencyStatus == DependencyStatus.Available)
                {
                    m_DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                    StartCoroutine(LoadUserData());
                }
                else
                {
                    Debug.LogWarning($"Could not resolve all Firebase dependencies: {m_DependencyStatus}");
                }
            });
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
                m_Level = 0;
                m_Xp = 0;
                m_Diamond = 0;
                m_Gold = 0;
            }
            else
            {
                DataSnapshot snapshot = dbTask.Result;

                m_Level = (int)snapshot.Child("level").Value;
                m_Xp = (int)snapshot.Child("xp").Value;
                m_Diamond = (int)snapshot.Child("diamond").Value;
                m_Gold = (int)snapshot.Child("gold").Value;

                levelText.text = $"Level: {m_Level}";
                xpText.text = $"Xp: {m_Xp}";
                diamondText.text = $"Diamond: {m_Diamond}";
                goldText.text = $"Gold: {m_Gold}";

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
        
        private IEnumerator UpdateLevel(int level)
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).Child("level").SetValueAsync(level);

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to add level task with: {dbTask.Exception}");
            }
            else
            {
                Debug.Log("Success to add level user");
            }
        }
        
        private IEnumerator UpdateXp(int xp)
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).Child("xp").SetValueAsync(xp);

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to add xp task with: {dbTask.Exception}");
            }
            else
            {
                Debug.Log("Success to add xp user");
            }
        }
        
        private IEnumerator UpdateDiamond(int diamond)
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).Child("diamond").SetValueAsync(diamond);

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to add diamond task with: {dbTask.Exception}");
            }
            else
            {
                Debug.Log("Success to add diamond user");
            }
        }
        
        private IEnumerator UpdateGold(int gold)
        {
            var dbTask = m_DatabaseReference.Child("users").Child(UserInfo.Instance.User.UserId).Child("gold").SetValueAsync(gold);

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to add gold task with: {dbTask.Exception}");
            }
            else
            {
                Debug.Log("Success to add gold user");
            }
        }

        public void Logout()
        {
            UserInfo.Instance.Auth.SignOut();
        }

        public void SaveExplore()
        {
            m_Xp += 10;
            m_Diamond += 1;
            m_Gold += 150;

            if (m_Xp > m_Level * 100)
            {
                m_Level += 1;
                m_Xp = 0;
                
                StartCoroutine(UpdateLevel(m_Level));
            }

            StartCoroutine(UpdateXp(m_Xp));
            StartCoroutine(UpdateDiamond(m_Diamond));
            StartCoroutine(UpdateGold(m_Gold));
        }
    }
}
