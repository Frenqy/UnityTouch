using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.Marker
{

    public class VirtualMarkerManager : MonoBehaviour
    {
        public GameObject imageCardPrefab;
        public GameObject videoCardPrefab;
        public static VirtualMarkerManager Instance;

        //private List<List<CardBase>> cardContainer = new List<List<CardBase>>();
        //private List<CardBase> cardList = new List<CardBase>();
        private Transform mediaPos { get; set; }
        private bool canAdd = false;
        private Setting setting = new Setting();

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="pos"></param>
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

        public void AddIntoMarker(int type)
        {
            if(canAdd)
            {
                switch (type)
                {
                    case (int)MediaType.Image:
                        GameObject.Instantiate(imageCardPrefab, mediaPos);
                        //GameObject.Instantiate(imageCardPrefab, Vector3.zero, Quaternion.identity, mediaPos);
                        break;

                    case (int)MediaType.Video:
                        GameObject.Instantiate(videoCardPrefab, mediaPos);
                        //GameObject.Instantiate(videoCardPrefab, Vector3.zero, Quaternion.identity, mediaPos);
                        break;
                }
            }
        }

    }

}
