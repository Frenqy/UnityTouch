using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

/// <summary>
/// 单个虚拟Marker上的功能
/// </summary>
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

        private MarkerSetting trueMkSetting = new MarkerSetting();
        public MarkerSetting mkSetting { get {
                trueMkSetting.MarkerID = int.Parse(mkID.text);
                trueMkSetting.buttonSetting = btnActions.Select(x => x.buttonSetting).ToList();
                return trueMkSetting;
            } }

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

        ////用于测试 按下S保存当前MK为vkxr
        //private void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.S))
        //    {
        //        SaveSetting();
        //    }
        //}

            /// <summary>
            /// 保存单个Marker上的信息
            /// 
            /// </summary>
        public void SaveSetting()
        {
            //MarkerSetting marker = new MarkerSetting();
            

            //Setting setting = new Setting();
            //setting.markers.Add(marker);

            //SettingManager.PackSetting(setting, null);

            //tip:在完成打包操作之后 必须清理已有的CardBase（未完成）
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

        private UIGradient gradient;
        private AnyButtonClick onExpandDelegate;
        private AnyButtonClick onCollapseDelegate;

        private ButtonSetting button = new ButtonSetting();
        public ButtonSetting buttonSetting 
        { 
            get 
            {
                //初始数据
                button.buttonID = btnIndex;
                button.previewPath = string.Empty;
                //遍历获取CardBase
                var cards = mediaPos.GetComponentsInChildren<CardBase>(true);
                button.mediaList = cards.Select(x => x.mediaSetting).ToList();

                return button;
            }
        }

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
