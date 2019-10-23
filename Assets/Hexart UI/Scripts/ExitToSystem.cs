using UnityEngine;

namespace VIC.Creator.UI
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
            Debug.Log("It's working :)");
            Application.Quit();
        }
    }
}