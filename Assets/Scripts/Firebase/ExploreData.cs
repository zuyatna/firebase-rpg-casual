using System.Collections;
using Firebase.Database;
using Singleton;
using UnityEngine;

namespace Firebase
{
    public class ExploreData : MonoBehaviour
    {
        [Header("Firebase")] 
        private DependencyStatus m_DependencyStatus;
        private DatabaseReference m_DatabaseReference;

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
                UserInfo.Instance.level = 0;
                UserInfo.Instance.xp = 0;
                UserInfo.Instance.diamond = 0;
                UserInfo.Instance.gold = 0;
            }
            else
            {
                DataSnapshot snapshot = dbTask.Result;

                UserInfo.Instance.level = (int)snapshot.Child("level").Value;
                UserInfo.Instance.xp = (int)snapshot.Child("xp").Value;
                UserInfo.Instance.diamond = (int)snapshot.Child("diamond").Value;
                UserInfo.Instance.gold = (int)snapshot.Child("gold").Value;
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
        
        public void SaveExplore()
        {
            UserInfo.Instance.diamond += 1;
            UserInfo.Instance.gold += 150;

            if (UserInfo.Instance.xp > 100)
            {
                UserInfo.Instance.level += 1;
                UserInfo.Instance.xp = 0;
                
                StartCoroutine(UpdateLevel(UserInfo.Instance.level));
            }
            else
            {
                UserInfo.Instance.xp += 10;
            }

            StartCoroutine(UpdateXp(UserInfo.Instance.xp));
            StartCoroutine(UpdateDiamond(UserInfo.Instance.diamond));
            StartCoroutine(UpdateGold(UserInfo.Instance.gold));
            
            SceneManagement.ChangeScene("Home");
        }
    }
}
