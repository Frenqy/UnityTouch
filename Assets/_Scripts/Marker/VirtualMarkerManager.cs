using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.Marker
{

    public class VirtualMarkerManager : MonoBehaviour
    {
        public static VirtualMarkerManager Instance;

        private Transform mediaPos { get; set; }
        private bool canAdd = false;

        private void Awake()
        {
            Instance = this;
        }

        public void SetMediaPos(Transform pos)
        {
            mediaPos = pos;
        }

        public void SetAddState(bool anyExpand)
        {
            canAdd = anyExpand;
        }

        public Transform GetMediaPos()
        {
            return mediaPos;
        }

        public void AddIntoMarker()
        {
            if(canAdd)
            {
                Debug.LogError("添加了一个卡片");
            }
        }
    }
}
