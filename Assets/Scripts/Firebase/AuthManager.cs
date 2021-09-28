using System.Collections;
using Firebase.Auth;
using Firebase.Database;
using Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase
{
    public class AuthManager : MonoBehaviour
    {
        [Header("Firebase")] 
        private DependencyStatus m_DependencyStatus;
        private DatabaseReference m_DatabaseReference;
        private FirebaseAuth m_Auth;
        private FirebaseUser m_User;

        [Header("Login")] 
        public InputField emailLoginField;
        public InputField passwordLoginField;

        [Header("Register")] 
        public InputField usernameRegisterField;
        public InputField emailRegisterField;
        public InputField passwordRegisterField;
        public InputField passwordVerifyRegisterField;

        [Header("Message")] public Text messageText;

        public GameObject splashPanel;
        
        private void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                m_DependencyStatus = task.Result;

                if (m_DependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError($"Could not resolve all firebase dependencies: {m_DependencyStatus}");
                }
            });
        }

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth");
            
            m_Auth = FirebaseAuth.DefaultInstance;
            m_DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private IEnumerator Login(string email, string password)
        {
            var loginTask = m_Auth.SignInWithEmailAndPasswordAsync(email, password);

            messageText.text = "Loading...";
            
            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null)
            {
                Debug.LogError($"Failed to login task with: {loginTask.Exception}");

                if (loginTask.Exception.GetBaseException() is FirebaseException firebaseException)
                {
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string message = "Login failed!";

                    switch (authError)
                    {
                        case AuthError.MissingEmail:
                            message = "Missing email!";
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing password!";
                            break;
                        case AuthError.WrongPassword:
                            message = "Wrong password!";
                            break;
                        case AuthError.InvalidEmail:
                            message = "Invalid email";
                            break;
                        case AuthError.UserNotFound:
                            message = "User not found!";
                            break;
                    }

                    messageText.text = message;
                }
            }
            else
            {
                m_User = loginTask.Result;
                
                UserInfo.Instance.Auth = m_Auth;
                UserInfo.Instance.Reference = m_DatabaseReference;
                UserInfo.Instance.User = m_User;

                Debug.LogWarning($"User login successfully: {m_User.DisplayName}, {m_User.Email}");
            }
        }

        private IEnumerator Register(string email, string password, string username)
        {
            if (username == "")
            {
                messageText.text = "Missing username";
            }
            else if (passwordRegisterField.text != passwordVerifyRegisterField.text)
            {
                messageText.text = "Password doesn't match";
            }
            else
            {
                var registerTask = m_Auth.CreateUserWithEmailAndPasswordAsync(email, password);

                yield return new WaitUntil(() => registerTask.IsCompleted);

                if (registerTask.Exception != null)
                {
                    Debug.LogWarning($"Failed to register task with: {registerTask.Exception}");

                    if (registerTask.Exception.GetBaseException() is FirebaseException firebaseException)
                    {
                        AuthError authError = (AuthError)firebaseException.ErrorCode;

                        string message = "Register failed";

                        switch (authError)
                        {
                            case AuthError.MissingEmail:
                                message = "Missing email";
                                break;
                            case AuthError.MissingPassword:
                                message = "Missing password";
                                break;
                            case AuthError.WeakPassword:
                                message = "Weak password";
                                break;
                            case AuthError.EmailAlreadyInUse:
                                message = "Email already used";
                                break;
                        }

                        messageText.text = message;
                    }
                }
                else
                {
                    m_User = registerTask.Result;

                    if (m_User != null)
                    {
                        UserProfile profile = new UserProfile { DisplayName = username };

                        var profileTask = m_User.UpdateUserProfileAsync(profile);

                        messageText.text = "Loading...";
                        
                        yield return new WaitUntil(() => profileTask.IsCompleted);

                        if (profileTask.Exception != null)
                        {
                            Debug.LogWarning($"Failed to update profile task with: {profileTask.Exception}");
                            
                            messageText.text = "Username set failed!";
                        }
                        else
                        {
                            UserInfo.Instance.User = m_User;

                            StartCoroutine(UserInfo.Instance.UpdateUsernameDatabase(username));
                            StartCoroutine(UserInfo.Instance.UpdateLevel(0));
                            StartCoroutine(UserInfo.Instance.UpdateXp(0));
                            StartCoroutine(UserInfo.Instance.UpdateDiamond(0));
                            StartCoroutine(UserInfo.Instance.UpdateGold(0));

                            // clear message
                            Debug.LogWarning($"User register successfully: {m_User.DisplayName}, {m_User.Email}");

                            messageText.text = $"User register successfully: {m_User.DisplayName}, {m_User.Email}";
                        }
                    }
                }
            }
        }

        private IEnumerator FetchDataUser()
        {
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
            
            yield return new WaitForSeconds(2);
            
            StartCoroutine(UserInfo.Instance.LoadUserData());

            yield return new WaitForSeconds(2);
            
            SceneManagement.ChangeScene("home");
        }

        public void Login()
        {
            StartCoroutine(FetchDataUser());
        }

        public void Register()
        {
            StartCoroutine(Register(emailRegisterField.text, passwordVerifyRegisterField.text, usernameRegisterField.text));
        }
    }
}
