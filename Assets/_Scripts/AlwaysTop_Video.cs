using System;
using TouchScript.Gestures;
using UnityEngine;

namespace VIC.Player.Marker
{
    public class AlwaysTop_Video : MonoBehaviour
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
            transform.parent.SetAsLastSibling();
            transform.parent.parent.SetAsLastSibling();
        }

        private void pressedHandler(object sender, EventArgs e)
        {
            transform.parent.SetAsLastSibling();
            transform.parent.parent.SetAsLastSibling();
        }
    }
}
