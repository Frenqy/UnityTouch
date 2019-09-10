﻿using System.Collections;
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
    [SerializeField] private GameObject LineImage;

    private List<GameObject> Panels = new List<GameObject>();
    private TouchInput touch;
    private RectTransform rectTransform;

    private List<GameObject> Lines = new List<GameObject>();

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

            //绘制三角形的边
            //DrawTriangle(triangle);
        }
    }

    public void ClearTriangle()
    {
        //for (int i = Lines.Count - 1; i >= 0; i--)
        //{
        //    Destroy(Lines[i]);
        //    Lines.Remove(Lines[i]);
        //}

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

    public void DrawLine(Vector2 ScreenStart, Vector2 ScreenEnd)
    {
        Vector2 UIStart, UIEnd;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, ScreenStart, null, out UIStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, ScreenEnd, null, out UIEnd);

        GameObject line = Instantiate(LineImage, UIParent.transform);
        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchoredPosition = UIStart;
        rect.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector2(1, 0), ScreenEnd - ScreenStart));
        rect.localScale = new Vector3(Vector2.Distance(UIStart, UIEnd), 1, 1);

        Lines.Add(line);
    }

    public void DrawTriangle(Triangel t)
    {
        DrawLine(t.Pos[0], t.Pos[1]);
        DrawLine(t.Pos[0], t.Pos[2]);
        DrawLine(t.Pos[1], t.Pos[2]);
    }
}
