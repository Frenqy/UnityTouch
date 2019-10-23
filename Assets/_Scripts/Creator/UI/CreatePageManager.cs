using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript;

namespace VIC.Creator.UI
{
    
    /// <summary>
    /// 新建工程编辑界面管理器
    /// </summary>
    public class CreatePageManager : MonoBehaviour
    {
        private bool isActive = false;

        private void Start()
        {
            TouchManager.Instance.PointersUpdated += pointerPressedHandler;
        }

        public void pointerPressedHandler(object sender, PointerEventArgs e)
        {
            
        }

        public void SetManagerStateActive()
        {
            isActive = true;
        }

        public void SetManagerStateInactive()
        {
            isActive = false;
        }

        private void Update()
        {
            if (!isActive) return;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SearchMarker()
        {

        }
    }
}
