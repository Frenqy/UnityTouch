using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneDetail : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject detail;
    [SerializeField] private Text detailText;

    [HideInInspector] public Triangel triangel;

    void Start()
    {
        
    }

    void Update()
    {
        if (detail.activeSelf && triangel != null)
        {
            detailText.text = $"顶角角度：{triangel.ApexAngle} \n识别角度：{Triangel.Degrees[triangel.ID]}";
        }
    }

    public void ShowDetail()
    {
        button.SetActive(false);
        detail.SetActive(true);
    }
}
