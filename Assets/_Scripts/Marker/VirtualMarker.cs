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
        [SerializeField] private Button[] buttons;
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
}
