﻿using System.Collections;
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
        [SerializeField]
        private Animator readingAnimator;
        [SerializeField]
        private Animator exitConfirmAnimator;

        [Header("Marker配置信息")]
        public GameObject virtualMkPrefab;
        public Transform virtualPos;

        /// <summary>
        /// 是否触发Mk进入的事件
        /// </summary>
        private bool hasInvokeMkIn = false;

        /// <summary>
        /// 是否正在读取MK信息
        /// </summary>
        private bool isReading = false;

        /// <summary>
        /// 是否进入编辑状态
        /// </summary>
        private bool isEnterEdit = false;


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

        public void ExitEditMode()
        {
            if (isEnterEdit)
            {
                exitConfirmAnimator.Play("Modal Dialog In");

            }
            else
            {
                ResetSignal();
            }
        }

        public void ResetSignal()
        {
            readingAnimator.Play("Normal");
            ListTabsManager.Instance.PanelAnim(0);
            isEnterEdit = false;
            isReading = false;
            this.enabled = false;
        }

        /// <summary>
        /// 有接触时会每帧调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPressedHandler(object sender, PointerEventArgs e)
        {
            MarkerDetection();
        }

        /// <summary>
        /// 检查三角形数量
        /// 时刻检查
        /// </summary>
        private void MarkerDetection()
        {
            if (Triangels.Count > 0)  // 已经放置了Mk且不在编辑状态
            {
                if (Triangels.Count > 1)
                {
                    Debug.LogError("放置Marker数量过多");
                }
                else
                {
                    // MK数量只有一个且不在编辑状态 则检查是否进入指定区域
                    CheckPlaceArea();
                    Debug.ClearDeveloperConsole();
                }
            }
            else // 无Marker情况
            {
                Debug.ClearDeveloperConsole();
            }
        }

        /// <summary>
        /// 检查是否正确放置Marker
        /// </summary>
        private void CheckPlaceArea()
        {
            if (isEnterEdit) return;

            if (IsMkIn() == true && !isReading) // 
            {
                StartCoroutine(ReadMkInfo());
            }
            else if (IsMkIn() == false)
            {
                Debug.LogError("请将Marker放置到指定位置");
                StopAllCoroutines();
                isReading = false;
                readingAnimator.Play("Normal");
            }
        }

        /// <summary>
        /// 检查单个Mk状态下 是否放置在正确位置
        /// </summary>
        /// <returns></returns>
        private bool IsMkIn()
        {
            if (Triangels.Count == 1)
            {
                if (Triangels[0].Center.x < placeArea.position.x + 100 || Triangels[0].Center.x < placeArea.position.x - 100)
                {
                    if (Triangels[0].Center.y < placeArea.position.y + 100 || Triangels[0].Center.y < placeArea.position.y - 100)
                    {
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
        /// 读取MK信息 读取完毕进入编辑状态
        /// </summary>
        /// <returns></returns>
        IEnumerator ReadMkInfo()
        {
            isReading = true;
            readingAnimator.Play("Highlighted");
            yield return new WaitForSeconds(2.0f);
            isEnterEdit = true;
            SetupVirtualMarker(Triangels[0].ID);
            ShowMediaList(true);
        }

        GameObject virtualMk;

        /// <summary>
        /// 设置虚拟Maker
        /// </summary>
        private void SetupVirtualMarker(int index)
        {
            Debug.LogError("当前MK编号 " + index);
            virtualMk = Instantiate(virtualMkPrefab, virtualPos);
        }

        /// <summary>
        /// 弹出媒体文件列表
        /// </summary>
        private void ShowMediaList(bool visible)
        {
            mediaList.SetActive(visible);
        }
    }
}
