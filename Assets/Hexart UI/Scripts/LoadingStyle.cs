using UnityEngine;

namespace VIC.Creator.UI
{
    public class LoadingStyle : MonoBehaviour
    {
        public void SetStyle(string prefabToLoad)
        {
            VIC.Creator.UI.LoadingScreen.prefabName = prefabToLoad;
        }
    }
}