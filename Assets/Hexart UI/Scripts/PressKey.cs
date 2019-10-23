using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace VIC.Creator.UI
{
    /// <summary>
    /// Auto enter loading panel page
    /// </summary>
    public class PressKey : MonoBehaviour
    {
        private float time2Start = 3.0f;

        [Header("KEY ACTION")]  
        [SerializeField]
        public UnityEvent pressAction;

        private void Start()
        {
            StartCoroutine(AutoStart()); 
        }

        IEnumerator AutoStart()
        {
            yield return new WaitForSeconds(time2Start);
            pressAction.Invoke();
        }
    }
}