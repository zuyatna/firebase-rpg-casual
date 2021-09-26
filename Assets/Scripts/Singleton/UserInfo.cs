using Firebase.Auth;
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
        
        public FirebaseUser User;

        public FirebaseAuth Auth;

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
    }
}
