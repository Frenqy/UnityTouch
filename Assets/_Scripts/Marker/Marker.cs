using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    [SerializeField] private RawImage[] PreviewImg;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Transform[] MediaParent;

    [HideInInspector] public List<MediaSetting> medias;

    public IEnumerator Init()
    {
        foreach (var media in medias)
        {
            for (int i = 0; i < media.mediaPath.Length; i++)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + media.previewPath)) 
                {
                    webRequest.downloadHandler = new DownloadHandlerTexture();
                    yield return webRequest.SendWebRequest();
                    PreviewImg[i].texture = DownloadHandlerTexture.GetContent(webRequest);
                }
                using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + media.mediaPath[i]))
                {

                }
            }
        }
    }
}
