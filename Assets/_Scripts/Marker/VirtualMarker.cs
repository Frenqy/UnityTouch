using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VIC.Creator.Marker
{

    /// <summary>
    /// 虚拟Marker
    /// </summary>
    public class VirtualMarker : MonoBehaviour
    {
        [SerializeField] private RawImage[] PreviewImg;
        [SerializeField] private ButtonActionGroup[] btnActions;
        [SerializeField] private Transform[] MediaParent;

        public Text mkID;

        public void SetMkInfo(int id)
        {
            mkID.text = id.ToString();
        }

        /// <summary>
        /// 点击了加号按钮
        /// </summary>
        public void OnClickAdd()
        {

        }

        /// <summary>
        /// 点击了减号按钮
        /// </summary>
        public void OnClickReduce()
        {

        }

    }

    [System.Serializable]
    public class ButtonActionGroup
    {
        public Button btn;
        public Image previewImg;

        private bool isAdd=false;
        private ButtonSetting buttonSetting;
        private int number;

        public void Init()
        {
            btn.onClick.AddListener(OnAdd);
        }

        private void OnAdd()
        {
            isAdd = true;
            Debug.LogError("虚拟MK点击了一次 " + number);
            number++;
        }

        public void SaveToSetting()
        {

        }
    }
}
