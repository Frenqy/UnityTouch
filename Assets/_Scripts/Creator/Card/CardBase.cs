using UnityEngine;
using VIC.Core;

namespace VIC.Creator.Marker
{
    public abstract class CardBase : MonoBehaviour
    {
        protected Media media = new Media();
        public virtual Media mediaSetting
        {
            get
            {
                media.pos = transform.position;
                media.rotate = transform.rotation;
                media.scale = transform.localScale;
                return media;
            }
        }


        protected GameObject IntroImg;
        protected GameObject AddBtn;
        protected GameObject CloseBtn;

        protected virtual void Start()
        {
            IntroImg = transform.Find("IntroImg").gameObject;
            AddBtn = transform.Find("AddBtn").gameObject;
            CloseBtn = transform.Find("CloseBtn").gameObject;
        }

        public virtual void AddMedia()
        {
            IntroImg.SetActive(false);
            AddBtn.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}

