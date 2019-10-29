using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.Marker
{
    /// <summary>
    /// 编辑器端的MarkerManager的做法
    /// 1.放置MK，显示读取信息动画
    /// 2.读取信息动画完成，出现虚拟MK和媒体文件列表
    /// 3.拿走MK 不影响信息编辑
    /// </summary>
    public class CreatorMarkerManager : MonoBehaviour
    {
        public GameObject virtualMarkerPrefab;
        public Transform containerCanvas;

        private void OnEnable()
        {
            TouchInput.MarkerUpdated += PrepareMarker;
        }

        private void OnDisable()
        {
            TouchInput.MarkerUpdated -= PrepareMarker;
            isEditing = true;

        }

        /// <summary>
        /// 加载传入的三角形的素材信息
        /// </summary>
        private void LoadMarkerInfo(Triangel triangel)
        {

        }


        private bool isEditing = true;
        GameObject virtualMK;
        /// <summary>
        /// 在左下角展示通用型的Marker
        /// 在中心点位置展示该Marker下的内容
        /// 如果有内容则进行展示 无则显示空Marker
        /// </summary>
        /// <param name="triangels"></param>
        public void PrepareMarker(List<Triangel> triangels)
        {

            StartCoroutine(ReadingAnimation(2.0f));
            if (isEditing)
            {
                virtualMK = Instantiate<GameObject>(virtualMarkerPrefab, containerCanvas);
                isEditing = false;
            }

        }

        public void HideCreatorMarker(List<Triangel> triangels)
        {

        }

        IEnumerator ReadingAnimation(float duration)
        {
            yield return new WaitForSeconds(duration);
        }
    }
}
