//#define Test
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TouchScript;
using System;

public class TouchInput : MonoBehaviour
{
    public List<Triangel> Triangels = new List<Triangel>();
    private List<TouchPoint> points = new List<TouchPoint>();
    private int pointCount;

    public Transform[] SimulatePoints;

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersUpdated += pointersPressedHandler;
            //TouchManager.Instance.PointersReleased += pointersReleasedHandler;
        }
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersUpdated -= pointersPressedHandler;
            //TouchManager.Instance.PointersReleased -= pointersReleasedHandler;
        }
    }

    private void pointersPressedHandler(object sender, PointerEventArgs e)
    {
        GetTriangle(sender, e, 220, 5);
        LightSet.Instance.ShowTriangle();
        LightSet.Instance.ClearTriangle();
    }

    private void pointersReleasedHandler(object sender, PointerEventArgs e)
    {
        GetTriangle(sender, e, 220, 5);
    }

    /// <summary>
    /// 从触摸点构造三角形
    /// </summary>
    /// <param name="maxDistance">可连成边的两点之间的最大距离</param>
    /// <param name="tolerance">通过顶角角度判断物体ID时的角度误差范围</param>
    /// Do not set tolerance higher than half of the smallest distance between your configured vertex angles.
    /// 考虑增加：两点之间最小距离限制
    /// Distance单位为pixel，则屏幕小的时候需要传入更高的距离
    public void GetTriangle(object sender, PointerEventArgs e, float maxDistance = 220, float tolerance = 5)
    {
        GetTouchPoints(sender, e);

        PairPoints(maxDistance);

        List<List<Vector2>> poly = Pair2Poly();

        List<List<Vector2>> polyNoDuplicate = DeleteDuplicatePoly(poly);

        //清理已有的三角形
        Triangels.Clear();

        Poly2Triangle(tolerance, polyNoDuplicate);
    }

    //-------------------以下是详细的三角形构造算法

    /// <summary>
    /// 获取触摸点数据
    /// </summary>
    /// <param name="points"></param>
    /// <param name="pointCount"></param>
    private void GetTouchPoints(object sender, PointerEventArgs e)
    {
        points.Clear();
#if UNITY_EDITOR

        for (int i = 0; i < SimulatePoints.Length; i++)
        {
            TouchPoint t = new TouchPoint();
            t.Paired = false;
            t.Pos = SimulatePoints[i].position;
            points.Add(t);
        }
        pointCount = SimulatePoints.Length;

#else

        pointCount = e.Pointers.Count;

        //获取当前所有触摸点 初始化参数
        for (int i = 0; i < pointCount; i++)
        {
            TouchPoint t = new TouchPoint();
            t.Paired = false;
            t.Pos = e.Pointers[i].Position;

            points.Add(t);
        }

#endif
    }

    /// <summary>
    /// 对所有点进行配对-连线
    /// </summary>
    /// <param name="maxDistance"></param>
    /// <param name="points"></param>
    /// <param name="pointCount"></param>
    private void PairPoints(float maxDistance)
    {
        int pairNum = 0;
        for (int i = 0; i < pointCount; i++)
        {
            for (int j = 0; j < pointCount; j++)
            {
                //判断两点之间距离 且不能是同一个点
                if (i != j && Vector2.Distance(points[i].Pos, points[j].Pos) < maxDistance)
                {
                    //两个点中至少有一个点要未配对
                    if (!points[i].Paired || !points[j].Paired)
                    {
                        points[i].Group.Add(pairNum);
                        points[j].Group.Add(pairNum);
                        points[i].Paired = true;
                        points[j].Paired = true;
                        pairNum++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 配对连线之后，用相连的边构造图形
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private List<List<Vector2>> Pair2Poly()
    {
        List<List<Vector2>> poly = new List<List<Vector2>>();
        for (int i = 0; i < points.Count; i++)
        {
            //0为该点无边 1为只有一条边 2条边3个点才能构造一个图形
            if (points[i].Group.Count > 1)
            {
                List<Vector2> p = new List<Vector2>();
                for (int i2 = 0; i2 < points[i].Group.Count; i2++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        for (int j2 = 0; j2 < points[j].Group.Count; j2++)
                        {
                            if (points[i].Group[i2] == points[j].Group[j2])
                            {
                                p.Add(points[j].Pos);
                            }
                        }
                    }
                }
                if (p.Count > 0)
                {
                    p.Add(points[i].Pos);
                    poly.Add(p);
                }
            }
        }

        return poly;
    }

    /// <summary>
    /// 排除重复构造的图形
    /// </summary>
    /// <param name="poly"></param>
    /// <returns></returns>
    private List<List<Vector2>> DeleteDuplicatePoly(List<List<Vector2>> poly)
    {
        List<List<Vector2>> polyNoDuplicate = new List<List<Vector2>>();
        for (int i = 0; i < poly.Count; i++)
        {
            List<Vector2> p = new List<Vector2>();
            if (poly[i].Count > 0)
            {
                p.Add(poly[i][0]);
                for (int j = 0; j < poly[i].Count; j++)
                {
                    bool exist = false;
                    for (int k = 0; k < p.Count; k++)
                    {
                        if (p[k] == poly[i][j])
                        {
                            exist = true;
                        }
                        if (j == 0)
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        p.Add(poly[i][j]);
                    }
                }
            }
            polyNoDuplicate.Add(p);
        }

        return polyNoDuplicate;
    }

    /// <summary>
    /// 将构造的图形中的三角形筛选出来，并计算基本数据
    /// </summary>
    /// <param name="tolerance"></param>
    /// <param name="polyNoDuplicate"></param>
    private void Poly2Triangle(float tolerance, List<List<Vector2>> polyNoDuplicate)
    {
        for (int i = 0; i < polyNoDuplicate.Count; i++)
        {
            List<Vector2> triangle = new List<Vector2>();
            if (polyNoDuplicate[i].Count == 3)
            {
                Triangel t = new Triangel();
                t.Pos[0] = polyNoDuplicate[i][0];
                t.Pos[1] = polyNoDuplicate[i][1];
                t.Pos[2] = polyNoDuplicate[i][2];

                //顶点的下标
                int topPoint = t.FindTop();

                t.Center = t.FindCenter(); ;
                t.ApexAngle = t.FindAngleApex(topPoint);
                t.Rotate = t.FindRotate(topPoint);
                t.Apex = t.Pos[topPoint];
                t.Width = t.FindWidth(topPoint);
                t.Height = t.FindHeight(topPoint);

                //获取ID
                for (int j = 0; j < Triangel.Degrees.Length; j++)
                {
                    if (t.ApexAngle > Triangel.Degrees[j] - tolerance && t.ApexAngle < Triangel.Degrees[j] + tolerance)
                    {
                        t.ID = j;
                    }
                }

                Triangels.Add(t);
            }
        }
    }

    //---------------------
}
