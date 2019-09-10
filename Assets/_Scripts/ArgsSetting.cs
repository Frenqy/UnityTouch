using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArgsSetting : MonoBehaviour
{
    public static float Distance = 220;
    public static float Tolerance = 10;

    public Slider DistanceSlider;
    public Slider ToleranceSlider;
    public Text DistanceText;
    public Text ToleranceText;

    private void OnEnable()
    {
        DistanceSlider.onValueChanged.AddListener(DistanceChange);
        ToleranceSlider.onValueChanged.AddListener(ToleranceChange);
    }

    private void OnDisable()
    {
        DistanceSlider.onValueChanged.RemoveListener(DistanceChange);
        ToleranceSlider.onValueChanged.RemoveListener(ToleranceChange);
    }

    private void Awake()
    {
        DistanceChange(DistanceSlider.value);
        ToleranceChange(ToleranceSlider.value);
    }

    private void DistanceChange(float val)
    {
        Distance = val;
        DistanceText.text = val.ToString();
    }

    private void ToleranceChange(float val)
    {
        Tolerance = val;
        ToleranceText.text = val.ToString();
    }
}
