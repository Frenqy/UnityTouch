using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VIC.Core;

namespace VIC.Creator.Marker
{
    public class ImageCard : CardBase
    {
        private RawImage raw;

        protected override void Start()
        {
            base.Start();

            raw = GetComponent<RawImage>();
        }

        public override void AddMedia()
        {
            base.AddMedia();

            raw.color = Color.white;

            mediaSetting.mediaType = MediaType.Image;
            mediaSetting.mediaContent = FileCommon.OpenFile("图片", new string[] { "jpg", "png", "bmp", "gif" });

            StartCoroutine(ShowImg());
        }

        private IEnumerator ShowImg()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + mediaSetting.mediaContent))
            {
                webRequest.downloadHandler = new DownloadHandlerTexture();
                yield return webRequest.SendWebRequest();
                Texture2D content = DownloadHandlerTexture.GetContent(webRequest);

                raw.texture = content;
            }

            //计算大小变化 维持两个按钮的大小
            Vector2 oldSize = raw.rectTransform.sizeDelta;
            raw.SetNativeSize();
            Vector2 newSize = raw.rectTransform.sizeDelta;

            Vector2 scale = newSize / oldSize;
            float scaleF = Mathf.Min(scale.x, scale.y);
            AddBtn.GetComponent<RectTransform>().sizeDelta *= scaleF;
            CloseBtn.GetComponent<RectTransform>().sizeDelta *= scaleF;
        }
    }
}
