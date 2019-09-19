using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript;
using System;

/// <summary>
/// 用于绘制三角形对应的UI
/// </summary>
public class LightSet : MonoBehaviour
{
    public static LightSet Instance;

    [SerializeField] private GameObject TestPanel;
    [SerializeField] private GameObject UIParent;

    private List<GameObject> Panels = new List<GameObject>();
    private TouchInput touch;
    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        touch = GetComponent<TouchInput>();
        rectTransform = UIParent.GetComponent<RectTransform>();

        for (int i = 0; i < Triangel.Degrees.Length; i++)
        {
            GameObject go = Instantiate(TestPanel, UIParent.transform);
            go.SetActive(false);
            Panels.Add(go);
        }

    }

    public void ShowTriangle()
    {
        foreach (var triangle in touch.Triangels)
        {
            Panels[triangle.ID].SetActive(true);
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, triangle.Center, null, out pos);
            Panels[triangle.ID].GetComponent<RectTransform>().anchoredPosition = pos;
            Panels[triangle.ID].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, triangle.Rotate);
        }
    }

    public void ClearTriangle()
    {
        for (int i = 0; i < Panels.Count; i++)
        {
            if (Panels[i].activeSelf)
            {
                bool open = false;
                for (int j = 0; j < touch.Triangels.Count; j++)
                {
                    if (i == touch.Triangels[j].ID) open = true;
                }
                Panels[i].SetActive(open);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
