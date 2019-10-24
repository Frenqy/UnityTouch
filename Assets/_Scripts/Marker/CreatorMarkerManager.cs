using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.Marker
{
    /// <summary>
    /// 编辑器端的MarkerManager的做法
    /// 1.显示三角形列表里的第一个三角形
    /// 2.
    /// </summary>
    public class CreatorMarkerManager : MonoBehaviour
    {
        public GameObject virtualMarkerPrefab;
        public Transform containerCanvas;

        private void OnEnable()
        {
            TouchInput.MarkerUpdated += ShowCreatorMarker;
        }

        private void OnDisable()
        {
            TouchInput.MarkerUpdated -= ShowCreatorMarker;
            isEditing = true;

        }


        private bool isEditing = true;
        GameObject virtualMK;
        /// <summary>
        /// 在左下角展示通用型的Marker
        /// 在中心点位置展示该Marker下的内容
        /// 如果有内容则进行展示 无则显示空Marker
        /// </summary>
        /// <param name="triangels"></param>
        public void ShowCreatorMarker(List<Triangel> triangels)
        {
            if (isEditing)
            {
                virtualMK = Instantiate<GameObject>(virtualMarkerPrefab, containerCanvas);
                isEditing = false;
            }

        }
    }
}
