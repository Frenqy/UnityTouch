using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo.Demos;

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

            //大坑：
            //此处必须使用 temp 变量储存 i 再传入
            //不能直接将循环变量 i 传入
            //否则该方法内创建的循环变量 i 将一直留在内存无法释放
            //导致->调用 onClick 时，执行 onClick 内部语句时会按 i 最后（循环结束后）的值来执行，而不是 AddListener 时刻的 i 的值来执行
            //如果使用 temp 储存一次 i 的值
            //则可以在触发 onClick 时，按 AddListener 时刻的 temp 的值来执行内部语句
            int temp = i;
            buttons[medias[i].buttonID].onClick.AddListener(() =>
            {
                Debug.Log(temp);
                MediaParent[temp].gameObject.SetActive(!MediaParent[temp].gameObject.activeSelf);
            });

            //根据媒体类型将媒体加载进来
            switch (medias[i].mediaType)
            {
                case MediaType.None:
                    break;
                case MediaType.Text:
                    yield return StartCoroutine(InitText(i));
                    break;
                case MediaType.Image:
                    yield return StartCoroutine(InitImage(i));
                    break;
                case MediaType.Video:
                    InitVideo(i);
                    break;
                default:
                    break;
            }
        }

        //初始化完毕之后关闭本身
        gameObject.SetActive(false);
    }

    private void SetMediaActive(int index)
    {
        MediaParent[index].gameObject.SetActive(!MediaParent[index].gameObject.activeSelf);
    }

    public IEnumerator InitText(int mediaIndex)
    {
        //为按钮添加文字（UTF8)
        for (int j = 0; j < medias[mediaIndex].mediaPath.Length; j++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[mediaIndex].mediaPath[j]))
            {
                yield return webRequest.SendWebRequest();
                string content = webRequest.downloadHandler.text;

                GameObject text = Instantiate(TextPrefab, MediaParent[mediaIndex]);
                text.GetComponent<Text>().text = content;
            }
        }
    }

    public IEnumerator InitImage(int mediaIndex)
    {
        //为按钮添加图片
        for (int j = 0; j < medias[mediaIndex].mediaPath.Length; j++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + medias[mediaIndex].mediaPath[j]))
            {
                webRequest.downloadHandler = new DownloadHandlerTexture();
                yield return webRequest.SendWebRequest();
                Texture2D content = DownloadHandlerTexture.GetContent(webRequest);

                GameObject go = Instantiate(ImagePrefab, MediaParent[mediaIndex]);
                RawImage raw = go.GetComponent<RawImage>();
                raw.texture = content;
                raw.SetNativeSize();
            }
        }
    }

    public void InitVideo(int mediaIndex)
    {
        //为按钮添加视频
        GameObject go = Instantiate(VideoPrefab, MediaParent[mediaIndex]);
        VCR vcr = go.GetComponentInChildren<VCR>();
        vcr.paths = medias[mediaIndex].mediaPath;
    }
}
