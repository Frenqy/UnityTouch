using System;
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
    [HideInInspector] public GameObject TextPrefab;
    [HideInInspector] public GameObject ImagePrefab;
    [HideInInspector] public GameObject VideoPrefab;


    private void Start()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
       
        //遍历当前marker的Media列表
        for (int i = 0; i < medias.Count; i++)
        {
            //设置每个按钮的缩略图
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[i].previewPath))
            {
                webRequest.downloadHandler = new DownloadHandlerTexture();
                yield return webRequest.SendWebRequest();
                PreviewImg[medias[i].buttonID].texture = DownloadHandlerTexture.GetContent(webRequest);
            }

            //设置按钮本身
            buttons[medias[i].buttonID].transform.parent.gameObject.SetActive(true);
            buttons[medias[i].buttonID].onClick.AddListener(() => MediaParent[i].gameObject.SetActive(MediaParent[i].gameObject.activeSelf));

            //根据媒体类型将媒体加载进来
            switch (medias[i].mediaType)
            {
                case MediaType.None:
                    break;
                case MediaType.Text:
                    yield return StartCoroutine(InitText(i));
                    break;
                case MediaType.Image:
                    //yield return StartCoroutine(InitImage(i));
                    break;
                case MediaType.Video:
                    //yield return StartCoroutine(InitVideo(i));
                    break;
                default:
                    break;
            }
        }

        //初始化完毕之后关闭本身
        gameObject.SetActive(false);
    }

    public IEnumerator InitText(int mediaIndex)
    {
        //为按钮添加媒体
        for (int j = 0; j < medias[mediaIndex].mediaPath.Length; j++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[mediaIndex].mediaPath[j]))
            {
                yield return webRequest.SendWebRequest();
                //UTF-8
                string content = webRequest.downloadHandler.text;
                GameObject text = Instantiate(TextPrefab, MediaParent[mediaIndex]);
                text.GetComponent<Text>().text = content;
            }
        }
    }

    public IEnumerator InitImage(int mediaIndex)
    {
        //为按钮添加媒体
        for (int j = 0; j < medias[mediaIndex].mediaPath.Length; j++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[mediaIndex].mediaPath[j]))
            {
                webRequest.downloadHandler = new DownloadHandlerTexture();
                yield return webRequest.SendWebRequest();
                Texture2D content = DownloadHandlerTexture.GetContent(webRequest);
            }
        }
    }

    public IEnumerator InitVideo(int mediaIndex)
    {
        //为按钮添加媒体
        for (int j = 0; j < medias[mediaIndex].mediaPath.Length; j++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[mediaIndex].mediaPath[j]))
            {
                yield return 0;
            }
        }
    }
}
