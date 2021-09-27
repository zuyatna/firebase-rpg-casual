using System.Collections;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

namespace Singleton
{
    public class UserInfo: MonoBehaviour
    {
        private static UserInfo _instance;

        public static UserInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UserInfo>();

                    if (_instance == null)
                    {
                        GameObject go = new GameObject
                        {
                            name = "UserInfo"
                        };
                        
                        _instance = go.AddComponent<UserInfo>();
                        
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }
        
        [Header("Firebase")]
        public FirebaseUser User;
        public FirebaseAuth Auth;
        public DatabaseReference Reference;

        [Header("Player Data")] 
        public string username = "null";
        public int level;
        public int xp;
        public int diamond;
        public int gold;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void ClearData()
        {
            username = "null";
            level = 0;
            xp = 0;
            diamond = 0;
            gold = 0;
        }
        
        public IEnumerator UpdateUsernameAuth(string usernameUser)
        {
            UserProfile profile = new UserProfile { DisplayName = usernameUser };

            var profileTask = User.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(() => profileTask.IsCompleted);

            Debug.LogWarning(profileTask.Exception != null
                ? $"Failed to update profile task with: {profileTask.Exception}"
                : $"User register successfully: {User.DisplayName}, {User.Email}");
        }
        
        public IEnumerator UpdateUsernameDatabase(string usernameUser)
        {
            var dbTask = Reference.Child("users").Child(User.UserId).Child("username").SetValueAsync(usernameUser);

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
        
        public IEnumerator LoadUserData()
        {
            var dbTask = Reference.Child("users").Child(User.UserId).GetValueAsync();

            yield return new WaitUntil(() => dbTask.IsCompleted);

            if (dbTask.Exception != null)
            {
                Debug.LogWarning($"Failed to load user data task with: {dbTask.Exception}");
            }
            else if (dbTask.Result.Value == null)
            {
                ClearData();
            }
            else
            {
                DataSnapshot snapshot = dbTask.Result;

                username = snapshot.Child("username").Value.ToString();
                level = int.Parse(snapshot.Child("level").Value.ToString());
                xp = int.Parse(snapshot.Child("xp").Value.ToString());
                diamond = int.Parse(snapshot.Child("diamond").Value.ToString());
                gold = int.Parse(snapshot.Child("gold").Value.ToString());
                
                Debug.Log("Load user data successfully..");
            }
        }
        
        public IEnumerator UpdateLevel(int addLevel)
        {
            level += addLevel;
            
            var dbTask = Reference.Child("users").Child(User.UserId).Child("level").SetValueAsync(level);

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
        
        public IEnumerator UpdateXp(int addXp)
        {
            xp += addXp;
            
            var dbTask = Reference.Child("users").Child(User.UserId).Child("xp").SetValueAsync(xp);

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
        
        public IEnumerator UpdateDiamond(int addDiamond)
        {
            diamond += addDiamond;
            
            var dbTask = Reference.Child("users").Child(User.UserId).Child("diamond").SetValueAsync(diamond);

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
        
        public IEnumerator UpdateGold(int addGold)
        {
            gold += addGold;
            
            var dbTask = Reference.Child("users").Child(User.UserId).Child("gold").SetValueAsync(gold);

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

        public void GetLevelUp()
        {
            int levelUp = (level + 2) * 10;

            if (xp > levelUp)
            {
                int addLevel = 1;
                
                xp = 0;

                StartCoroutine(UpdateLevel(addLevel));
                StartCoroutine(UpdateXp(xp));
            }
        }
        
        public void Logout()
        {
            Auth.SignOut();

            ClearData();
            
            Debug.LogWarning($"User {User.DisplayName} was sign out");
        }
    }
}
