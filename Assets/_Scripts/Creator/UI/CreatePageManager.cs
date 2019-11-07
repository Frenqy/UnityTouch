using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TouchScript;
using UnityEngine;
using VIC.Core;
using VIC.Creator.Marker;

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

        #region VirtualMarker相关配置

        private VirtualMarker virtualMk;
        private Dictionary<int, VirtualMarker> VMkDict = new Dictionary<int, VirtualMarker>();
        private Setting setting = new Setting();
        #endregion

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

        /// <summary>
        /// 退出编辑模式
        /// </summary>
        public void ExitEditMode()
        {
            if (isEnterEdit)
            {
                exitConfirmAnimator.Play("Modal Dialog In");
            }
            else
            {
                ResetSignal();
                ListTabsManager.Instance.PanelAnim(0);
                enabled = false;
            }
        }

        public void SaveProject()
        {
            ResetSignal();

            setting.markers = VMkDict.Values.Select(x => x.mkSetting).ToList();
            StartCoroutine(SettingManager.PackSetting(setting, null));

        }

        /// <summary>
        /// 编辑界面的标志位和动画效果重置
        /// </summary>
        public void ResetSignal()
        {
            readingAnimator.Play("Normal");
            isEnterEdit = false;
            isReading = false;
            virtualMk.gameObject.SetActive(false);
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
                }
                else
                {
                    // MK数量只有一个且不在编辑状态 则检查是否进入指定区域
                    CheckPlaceArea();
                }
            }
        }

        /// <summary>
        /// 检查是否正确放置Marker
        /// </summary>
        private void CheckPlaceArea()
        {
            if (isEnterEdit)
            {
                return;
            }

            if (IsMkIn() == true && !isReading) // 
            {
                StartCoroutine(ReadMkInfo());
            }
            else if (IsMkIn() == false)
            {
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
                }
            }

            return false;
        }

        /// <summary>
        /// 读取MK信息 读取完毕进入编辑状态
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadMkInfo()
        {
            isReading = true;
            readingAnimator.Play("Highlighted");
            yield return new WaitForSeconds(2.0f);
            isEnterEdit = true;
            SetupVirtualMarker(Triangels[0].ID);    // 读取完成之后
            ShowMediaList(true);
        }

        /// <summary>
        /// 设置虚拟Maker
        /// </summary>
        private void SetupVirtualMarker(int index)
        {
            if (VMkDict.ContainsKey(index))
            {
                virtualMk = VMkDict[index];
                virtualMk.gameObject.SetActive(true);
            }
            else
            {
                virtualMk = Instantiate(virtualMkPrefab, virtualPos).GetComponent<VirtualMarker>();
                virtualMk.SetMkInfo(index);
                VMkDict.Add(index, virtualMk);
            }

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
