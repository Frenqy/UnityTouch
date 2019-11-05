using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
        [SerializeField] private Sprite expandIcon;
        [SerializeField] private Sprite collaspeIcon;

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
                MediaParent[i].gameObject.SetActive(false);
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
            btnActions[index].previewImg.sprite = collaspeIcon;
            MediaParent[index].gameObject.SetActive(true);
            SetAllButtonsActivation(false);
            VirtualMarkerManager.Instance.SetAddState(true);
        }

        public void CollapseAnyItem(int index)
        {
            Debug.LogError("折叠 " + index + "层级");

            btnActions[index].previewImg.sprite = expandIcon;
            MediaParent[index].gameObject.SetActive(false);
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

            GetSettingInfo();
        }

        public void GetSettingInfo()
        {
            //初始数据
            buttonSetting.buttonID = btnIndex;
            buttonSetting.previewPath = null;
            //遍历获取CardBase
            var cards = mediaPos.GetComponentsInChildren<CardBase>(true);
            buttonSetting.mediaList = cards.Select(x => x.mediaSetting).ToList();

            Debug.Log(buttonSetting.mediaList.Count);
        }
    }

    public delegate void AnyButtonClick(int index);

}
