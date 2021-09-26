using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static void ChangeScene(string nameScene) => SceneManager.LoadScene(nameScene);
}
