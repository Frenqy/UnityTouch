using UnityEngine;
using UnityEngine.SceneManagement;

namespace VIC.Creator.UI
{
    public class LoadScene : MonoBehaviour
    {
        public void ChangeToScene(string sceneName)
        {
            VIC.Creator.UI.LoadingScreen.LoadScene(sceneName);
        }
    }
}