using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript;

namespace VIC.Creator.UI
{

    /// <summary>
    /// 编辑端下识别的步骤
    /// 1.检查三角形数量是否大于0
    /// 2.如果Marker数量大于1 则提示要排除干扰
    /// 3.如果Marker数量为1 则放置区闪动提示
    /// 4.如果进入放置区 则生成虚拟Marker并读取内容
    /// 5.
    /// </summary>
    public class CreatePageManager : TouchInput
    {
        [SerializeField]
        private Transform placeArea;
        [SerializeField]
        private GameObject mediaList;
        private bool isEditing = false;

        public override void OnEnable()
        {
            base.OnEnable();
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersUpdated += OnPressedHandler;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersUpdated -= OnPressedHandler;
            }

        }

        private void OnPressedHandler(object sender, PointerEventArgs e)
        {
            CheckCount();
        }

        /// <summary>
        /// 检查三角形数量
        /// </summary>
        private void CheckCount()
        {
            if (Triangels.Count > 0)
            {
                // 当前数量大于1
                if (Triangels.Count > 1)
                {
                    // TODO Marker放置数量大于1个，提示排除干扰
                    Debug.LogError("放置Marker数量过多");
                }
                else
                {
                    // TODO 检查Marker是否正确放置
                    CheckPlaceArea();
                    Debug.ClearDeveloperConsole();
                }
            }
            else
            {
                Debug.ClearDeveloperConsole();
            }
        }

        /// <summary>
        /// 检查是否正确放置Marker
        /// </summary>
        private void CheckPlaceArea()
        {
            if (IsPlaceCorrectly() == true)
            {
                // 设置虚拟Marker
                // 开启媒体列表
                SetupVirtualMarker(0);
                SetMediaList(true);
            }
            else
            {
                // 放置区域闪烁提示
                Debug.LogError("请将Marker放置到指定位置");
                SetMediaList(false);
            }
        }

        /// <summary>
        /// 是否放置在正确位置
        /// </summary>
        /// <returns></returns>
        private bool IsPlaceCorrectly()
        {
            if (Triangels.Count == 1)
            {
                //Debug.Log("Marker坐标 " + Triangels[0].Center);
                // Debug.Log("放置点坐标 " + placeArea.position);
                if (Triangels[0].Center.x < placeArea.position.x + 100 || Triangels[0].Center.x < placeArea.position.x - 100)
                {
                    if (Triangels[0].Center.y < placeArea.position.y + 100 || Triangels[0].Center.y < placeArea.position.y - 100)
                    {
                        Debug.LogError("Marker在范围内");
                        return true;
                    }
                    else
                    {
                        Debug.ClearDeveloperConsole();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 设置虚拟Maker
        /// </summary>
        private void SetupVirtualMarker(int index)
        {

        }

        /// <summary>
        /// 弹出媒体文件列表
        /// </summary>
        private void SetMediaList(bool visible)
        {
            mediaList.SetActive(visible);
        }
    }
}
