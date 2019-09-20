using System.Collections;
using System.Collections.Generic;
using TouchScript;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于设置UI左上角的参数：参数影响三角形生成算法，进而影响操作UI时手指的干扰程度和距离
/// </summary>
public class ArgsSetting : MonoBehaviour
{
    public static float Distance = 5;
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
        DistanceText.text = $"{val:F2} CM";
        Distance = val * TouchManager.Instance.DotsPerCentimeter;
        Debug.Log($"Dis in Pixel : {Distance:F2}");
    }

    private void ToleranceChange(float val)
    {
        Tolerance = val;
        ToleranceText.text = val.ToString() +" 度";
    }
}
