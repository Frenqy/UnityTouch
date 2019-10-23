using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VIC.Creator.UI
{
    /// <summary>
    /// Auto enter main panel page
    /// </summary>
    public class TimedAction : MonoBehaviour
    {
        [Header("TIMING (SECONDS)")]
        public Text text;
        public float time2ChangeText2 = 2;
        public float time2Enter = 3;

        [Header("END ACTION")]
        public UnityEvent timerAction;

        private string text1 = "正在载入资源...";
        private string text2 = "正在初始化服务...";

        void Start()
        {
            StartCoroutine(TimedEvent());
        }

        IEnumerator TimedEvent()
        {
            text.text = text1;
            yield return new WaitForSeconds(time2ChangeText2);
            text.text = text2;
            yield return new WaitForSeconds(time2Enter);
            timerAction.Invoke();
        }
    }
}
