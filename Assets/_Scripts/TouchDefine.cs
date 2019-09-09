using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class TouchPoint
{
    public Vector2 Pos;
    public List<int> Group = new List<int>();
    public bool Paired;
}

public class Triangel
{
    public Vector2[] Pos = new Vector2[3];
    public float Rotate;

    public Vector2 Center = new Vector2();
    public Vector2 Apex = new Vector2();

    public int ID;
    public float ApexAngle;
    public float Height;
    public float Width;

    /// <summary>
    /// 计算三角形中心
    /// </summary>
    /// <returns></returns>
    public Vector2 FindCenter() => (Pos[0] + Pos[1] + Pos[2]) / 3;

    /// <summary>
    /// 计算顶点
    /// </summary>
    /// <returns></returns>
    public int FindTop()
    {
        float[] dist = new float[3];
        dist[0] = Vector2.Distance(Pos[0], Pos[1]);
        dist[1] = Vector2.Distance(Pos[1], Pos[2]);
        dist[2] = Vector2.Distance(Pos[0], Pos[2]);

        float dist0to1 = dist[0];
        float dist1to2 = dist[1];
        float dist0to2 = dist[2];

        float diff01m02 = Mathf.Abs(dist0to1 - dist0to2);
        float diff02m12 = Mathf.Abs(dist0to2 - dist1to2);
        float diff01m12 = Mathf.Abs(dist0to1 - dist1to2);

        if (diff01m02 < diff02m12 && diff01m02 < diff01m12)
        {
            return 0;
        }
        else if (diff01m12 < diff01m02 && diff01m12 < diff02m12)
        {
            return 1;
        }
        else if (diff02m12 < diff01m02 && diff02m12 < diff01m12)
        {
            return 2;
        }
        else
        {
            throw new Exception();
        }
    }

    /// <summary>
    /// 计算顶角角度
    /// </summary>
    /// <param name="topPoint"></param>
    /// <returns></returns>
    public float FindAngleApex(int topPoint)
    {
        Vector2 a, b, c;
        a = new Vector2();
        b = new Vector2();
        c = new Vector2();

        b = Pos[topPoint];
        if (topPoint == 0)
        {
            a = Pos[1];
            c = Pos[2];
        }
        else if (topPoint == 1)
        {
            a = Pos[0];
            c = Pos[2];
        }
        else if (topPoint == 2)
        {
            a = Pos[0];
            c = Pos[1];
        }

        Vector2 ab = b - a;
        Vector2 cb = b - c;

        /*
         * 原算法
         * 
        float dot = (ab.x * cb.x + ab.y * cb.y); // dot product
        float cross = (ab.x * cb.y - ab.y * cb.x); // cross product

        float alpha = Mathf.Atan2(cross, dot);

        return Mathf.Abs(Mathf.Floor(alpha * 180 / Mathf.PI + 0.5f));
        
         */
        return Vector2.Angle(ab, cb);
    }

    /// <summary>
    /// 计算旋转角度（取中点指向顶点位置）（以坐标轴X轴正方向为0度？？）
    /// </summary>
    /// <param name="topPoint"></param>
    /// <returns></returns>
    public float FindRotate(int topPoint)
    {
        Vector2 a, b, c;
        a = new Vector2();
        b = new Vector2();
        c = new Vector2();

        b = Pos[topPoint];
        if (topPoint == 0)
        {
            a = Pos[1];
            c = Pos[2];
        }
        else if (topPoint == 1)
        {
            a = Pos[0];
            c = Pos[2];
        }
        else if (topPoint == 2)
        {
            a = Pos[0];
            c = Pos[1];
        }

        //底边中点
        Vector2 middlePt = (a + c) / 2;

        //底边中点指向顶点的向量
        Vector2 diff = b - middlePt;

        diff.Normalize();

        //old为原算法
        float rotate = Mathf.Rad2Deg * Mathf.Atan2(diff.x, diff.y) * -1;
        return rotate;
    }

    /// <summary>
    /// 底边长度
    /// </summary>
    /// <param name="topPoint"></param>
    /// <returns></returns>
    public float FindWidth(int topPoint)
    {
        Vector2 a, b, c;
        a = new Vector2();
        b = new Vector2();
        c = new Vector2();

        b = Pos[topPoint];
        if (topPoint == 0)
        {
            a = Pos[1];
            c = Pos[2];
        }
        else if (topPoint == 1)
        {
            a = Pos[0];
            c = Pos[2];
        }
        else if (topPoint == 2)
        {
            a = Pos[0];
            c = Pos[1];
        }

        return Vector2.Distance(a, c);
    }

    /// <summary>
    /// 三角形的高
    /// </summary>
    /// <param name="topPoint"></param>
    /// <returns></returns>
    public float FindHeight(int topPoint)
    {
        Vector2 a, b, c;
        a = new Vector2();
        b = new Vector2();
        c = new Vector2();

        b = Pos[topPoint];
        if (topPoint == 0)
        {
            a = Pos[1];
            c = Pos[2];
        }
        else if (topPoint == 1)
        {
            a = Pos[0];
            c = Pos[2];
        }
        else if (topPoint == 2)
        {
            a = Pos[0];
            c = Pos[1];
        }

        Vector2 middlePt = (a + c) / 2;

        return Vector2.Distance(middlePt, b);
    }

    /// <summary>
    /// 识别角度枚举
    /// </summary>
    public static int[] Degrees =
    {
        //Do not set tolerance higher than half of the smallest distance between your configured vertex angles.
        20,
        39,
        79,
        91,
        104,
        134,
        150,
        165
    };

}