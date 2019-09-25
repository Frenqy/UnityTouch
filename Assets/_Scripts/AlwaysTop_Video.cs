using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

public class AlwaysTop_Video : MonoBehaviour
{
    private void Start()
    {
        PressGesture pressGesture = GetComponent<PressGesture>();
        if (pressGesture != null) pressGesture.Pressed += pressedHandler;
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
