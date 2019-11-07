using System;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Core
{
    /// <summary>
    /// 记录触摸点数据
    /// </summary>
    [Serializable]
    public class TouchPoint
    {
        public Vector2 Pos;
        public List<int> Group = new List<int>();
        public bool Paired;
    }

    /// <summary>
    /// 记录并计算三角形数据
    /// </summary>
    public class Triangel
    {
        #region Public properties

        /// <summary>
        /// 识别ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 顶点坐标
        /// </summary>
        public Vector2[] Pos = new Vector2[3];

        /// <summary>
        /// 识别角度枚举
        /// </summary>
        public static int[] Degrees =
        {
        //7个识别角度，需要180/(7+1)计算得出角度距离
        //Do not set tolerance higher than half of the smallest distance between your configured vertex angles.
        22,
        44,//22
        67,//23
        90,//23
        112,//22
        134,//22
        157//23
    };

        /// <summary>
        /// 旋转角度
        /// </summary>
        public float Rotate { get; private set; }

        /// <summary>
        /// 中心坐标
        /// </summary>
        public Vector2 Center { get; private set; } = new Vector2();

        /// <summary>
        /// 顶点坐标
        /// </summary>
        public Vector2 Apex { get; private set; } = new Vector2();

        /// <summary>
        /// 顶角角度
        /// </summary>
        public float ApexAngle { get; private set; }

        /// <summary>
        /// 三角形的高的长度
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// 三角形底边长度
        /// </summary>
        public float Width { get; private set; }

        #endregion

        public List<int> pointID = new List<int>();

        /// <summary>
        /// 根据三个点的位置计算三角形的基本数据
        /// </summary>
        public void Init()
        {
            int top = FindTop();

            Center = FindCenter();
            ApexAngle = FindAngleApex(top);
            Rotate = FindRotate(top);
            Apex = Pos[top];
            Width = FindWidth(top);
            Height = FindHeight(top);

        }

        #region Private Methon

        /// <summary>
        /// 计算三角形中心
        /// </summary>
        /// <returns></returns>
        private Vector2 FindCenter()
        {
            return (Pos[0] + Pos[1] + Pos[2]) / 3;
        }

        /// <summary>
        /// 计算顶点
        /// </summary>
        /// <returns></returns>
        private int FindTop()
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
        private float FindAngleApex(int topPoint)
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
        private float FindRotate(int topPoint)
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
        private float FindWidth(int topPoint)
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
        private float FindHeight(int topPoint)
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

        #endregion
    }
}

