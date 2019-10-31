using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace VIC.Creator.Marker
{
    /// <summary>
    /// 虚拟Marker功能
    /// </summary>
    public class VirtualMarker : MonoBehaviour
    {
        [SerializeField] private RawImage[] PreviewImg;
        [SerializeField] private ButtonActionGroup[] btnActions;
        [SerializeField] private Transform[] MediaParent;

        public AnyButtonClick onAnyBtnExpand;
        public AnyButtonClick onAnyBtnCollapse;
        public Text mkID;

        private void OnEnable()
        {
            onAnyBtnExpand += ExpandAnyItem;
            onAnyBtnCollapse += CollapseAnyItem;
        }

        private void OnDisable()
        {
            onAnyBtnExpand -= ExpandAnyItem;
            onAnyBtnCollapse -= CollapseAnyItem;
        }

        private void Start()
        {
            for(int i=0; i<btnActions.Length; i++)
            {
                btnActions[i].Init(onAnyBtnExpand, onAnyBtnCollapse);
                btnActions[i].mediaPos = MediaParent[i];
            }
        }

        public void SetMkInfo(int id)
        {
            mkID.text = id.ToString();
        }

        public void ExpandAnyItem(int index)
        {
            Debug.LogError("展开 " + index + "层级");
            VirtualMarkerManager.Instance.SetMediaPos(MediaParent[index]);
            SetAllButtonsActivation(false);
            VirtualMarkerManager.Instance.SetAddState(true);
        }

        public void CollapseAnyItem(int index)
        {
            Debug.LogError("折叠 " + index + "层级");

            SetAllButtonsActivation(true);
            VirtualMarkerManager.Instance.SetAddState(false);
        }

        public void SetAllButtonsActivation(bool isActive)
        {
            for (int i = 0; i < btnActions.Length; i++)
            {
                btnActions[i].btn.interactable = isActive;
            }
        }

    }

    [System.Serializable]
    public class ButtonActionGroup
    {
        // 当前添加Button设置
        public Button btn;
        public int btnIndex;
        public Image previewImg;
        public Transform mediaPos;

        private bool isExpanded = false;

        private ButtonSetting buttonSetting;
        private UIGradient gradient;
        private AnyButtonClick onExpandDelegate;
        private AnyButtonClick onCollapseDelegate;

        public void Init(AnyButtonClick onExpand, AnyButtonClick onCollapse)
        {
            btn.onClick.AddListener(OnExpand);
            gradient = previewImg.gameObject.GetComponent<UIGradient>();
            gradient.enabled = false;

            this.onExpandDelegate = onExpand;
            this.onCollapseDelegate = onCollapse;
        }

        private void OnExpand()
        {
            isExpanded = true;
            gradient.enabled = true;
            onExpandDelegate(btnIndex);

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnCollapse);

            btn.interactable = true;
        }

        public void OnCollapse()
        {
            if(isExpanded)
            {
                isExpanded = false;
                gradient.enabled = false;
                onCollapseDelegate(btnIndex);

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnExpand);
            }
        }

        public void GetSettingInfo()
        {

        }
    }

    public delegate void AnyButtonClick(int index);

}
