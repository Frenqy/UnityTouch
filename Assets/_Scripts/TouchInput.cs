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

    public Transform[] SimulatePoints;

    private Dictionary<Vector2, int> idMap = new Dictionary<Vector2, int>();

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersUpdated += pointersPressedHandler;
        }
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersUpdated -= pointersPressedHandler;
        }
    }

    private void pointersPressedHandler(object sender, PointerEventArgs e)
    {
        GetTriangle(sender, e, ArgsSetting.Distance, ArgsSetting.Tolerance);
        LightSet.Instance.ShowTriangle();
        LightSet.Instance.ClearTriangle();
    }

    /// <summary>
    /// 从触摸点构造三角形
    /// </summary>
    /// <param name="maxDistance">可连成边的两点之间的最大距离，单位为像素（屏幕越小，传入值应越大）</param>
    /// <param name="tolerance">判断物体ID时的角度误差范围，误差范围应不大于角度距离的一半</param>
    public void GetTriangle(object sender, PointerEventArgs e, float maxDistance, float tolerance)
    {
        TriangleUpdate(e);

        GetTouchPoints(sender, e);

        PairPoints(maxDistance);

        List<List<Vector2>> poly = Pair2Poly();

        List<List<Vector2>> polyNoDuplicate = DeleteDuplicatePoly(poly);

        Poly2Triangle(tolerance, polyNoDuplicate);
    }

    #region 详细的三角形构造算法

    /// <summary>
    /// 刷新三角形：删除失去跟踪的，更新保持跟踪的信息
    /// </summary>
    /// <param name="e"></param>
    private void TriangleUpdate(PointerEventArgs e)
    {
        //获取当前还存在的触摸点的id
        //List<int> ids = new List<int>();
        Dictionary<int, Vector2> ids = new Dictionary<int, Vector2>();
        

        for (int i = 0; i < e.Pointers.Count; i++)
        {
            ids.Add(e.Pointers[i].Id, e.Pointers[i].Position);
        }

        //遍历每一个三角形的id列表，检查当前id列表中是否还存在
        for (int i = Triangels.Count - 1; i >= 0; i--)
        {
            bool contain = true;
            for (int j = 0; j < Triangels[i].pointID.Count; j++)
            {
                contain = ids.Keys.Contains(Triangels[i].pointID[j]);
                if (!contain) break;
            }
            //有任一点id不存在，则删除三角形（ID在此自动释放）
            if (!contain) Triangels.Remove(Triangels[i]);
            //否则，刷新三角形基本信息
            else
            {
                Triangels[i].Pos[0] = ids[Triangels[i].pointID[0]];
                Triangels[i].Pos[1] = ids[Triangels[i].pointID[1]];
                Triangels[i].Pos[2] = ids[Triangels[i].pointID[2]];
                Triangels[i].Init();
            }
        }
    }

    /// <summary>
    /// 获取触摸点数据
    /// </summary>
    /// <param name="points"></param>
    /// <param name="pointCount"></param>
    private void GetTouchPoints(object sender, PointerEventArgs e)
    {
        points.Clear();
        idMap.Clear();

#if UNITY_EDITOR

        //编辑器下模拟三个点出来构造三角形 方便调试
        for (int i = 0; i < SimulatePoints.Length; i++)
        {
            TouchPoint t = new TouchPoint();
            t.Paired = false;
            t.Pos = SimulatePoints[i].position;
            points.Add(t);

            idMap.Add(SimulatePoints[i].position, i);
        }

#else

        //获取当前所有触摸点 初始化参数
        for (int i = 0; i < e.Pointers.Count; i++)
        {
            //筛查当前触摸点是否应该被排除
            bool skipPoint = false;
            for (int j = 0; j < Triangels.Count; j++)
            {
                //遍历每一个三角形的锁定点列表，检查当前点是否被三角形锁定
                skipPoint = Triangels[j].pointID.Contains(e.Pointers[i].Id);
                if (skipPoint) break;
            }

            if (!skipPoint)
            {
                //触摸点不被排除时，加入构造三角形的点列表
                TouchPoint t = new TouchPoint
                {
                    Paired = false,
                    Pos = e.Pointers[i].Position
                };

                idMap.Add(e.Pointers[i].Position, e.Pointers[i].Id);

                Debug.Log($"point's ID: {e.Pointers[i].Id}  pos: {e.Pointers[i].Position}");

                points.Add(t);
            }

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
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
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
            if (polyNoDuplicate[i].Count == 3)
            {
                Triangel t = new Triangel();
                t.Pos[0] = polyNoDuplicate[i][0];
                t.Pos[1] = polyNoDuplicate[i][1];
                t.Pos[2] = polyNoDuplicate[i][2];

                t.pointID.Add(idMap[t.Pos[0]]);
                t.pointID.Add(idMap[t.Pos[1]]);
                t.pointID.Add(idMap[t.Pos[2]]);

                t.Init();

                //获取ID
                bool getid = false;
                for (int j = 0; j < Triangel.Degrees.Length; j++)
                {
                    if (t.ApexAngle > Triangel.Degrees[j] - tolerance && t.ApexAngle < Triangel.Degrees[j] + tolerance)
                    {
                        t.ID = j;
                        getid = true;
                        break;
                    }
                }
                if (!getid) break;

                //Debug.Log($"tri ID: {t.ID} with pointID: {idMap[t.Pos[0]]} , {idMap[t.Pos[1]]} , {idMap[t.Pos[2]]}");

                Triangels.Add(t);
            }
        }
    }

    #endregion
}
