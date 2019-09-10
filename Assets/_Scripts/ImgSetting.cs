using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript;
using TouchScript.Gestures;
using System;

public class ImgSetting : MonoBehaviour
{
    void Start()
    {
        var pressGesture = GetComponent<PressGesture>();
        if (pressGesture != null) pressGesture.Pressed += pressedHandler;
    }

    private void pressedHandler(object sender, EventArgs e)
    {
        transform.SetAsLastSibling();
    }
}
