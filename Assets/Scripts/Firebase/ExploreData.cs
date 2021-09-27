using Singleton;
using UnityEngine;

namespace Firebase
{
    public class ExploreData : MonoBehaviour
    {
        [SerializeField] private int xp;
        [SerializeField] private int diamond;
        [SerializeField] private int gold;
        
        public void SaveExplore()
        {
            StartCoroutine(UserInfo.Instance.UpdateXp(xp));
            StartCoroutine(UserInfo.Instance.UpdateDiamond(diamond));
            StartCoroutine(UserInfo.Instance.UpdateGold(gold));
            
            UserInfo.Instance.GetLevelUp();
            
            SceneManagement.ChangeScene("Home");
        }
    }
}
