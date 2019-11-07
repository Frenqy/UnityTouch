using System;
using TouchScript.Gestures;
using UnityEngine;

namespace VIC.Player.Marker
{
    /// <summary>
    /// 为手势操作的物体设置层级结构->置于其它图片上方
    /// </summary>
    public class AlwaysTop : MonoBehaviour
    {
        private void Start()
        {
            PressGesture pressGesture = GetComponent<PressGesture>();
            if (pressGesture != null)
            {
                pressGesture.Pressed += pressedHandler;
            }
        }

        private void OnEnable()
        {
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
        }

        private void pressedHandler(object sender, EventArgs e)
        {
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
        }
    }
}
