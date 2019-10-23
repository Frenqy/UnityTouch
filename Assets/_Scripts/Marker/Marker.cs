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

    [HideInInspector] public List<ButtonSetting> buttonSettings;
    [HideInInspector] public GameObject TextPrefab;
    [HideInInspector] public GameObject ImagePrefab;
    [HideInInspector] public GameObject VideoPrefab;
    [HideInInspector] public bool isInit;


    private void Start()
    {
        isInit = false;
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        //遍历当前marker的Media列表
        for (int i = 0; i < buttonSettings.Count; i++)
        {
            //设置每个按钮的缩略图
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + buttonSettings[i].previewPath))
            {
                webRequest.downloadHandler = new DownloadHandlerTexture();
                yield return webRequest.SendWebRequest();
                PreviewImg[buttonSettings[i].buttonID].texture = DownloadHandlerTexture.GetContent(webRequest);
            }

            //设置按钮本身
            buttons[buttonSettings[i].buttonID].transform.parent.gameObject.SetActive(true);

            int temp = i;
            buttons[buttonSettings[i].buttonID].onClick.AddListener(() =>
            {
                MediaParent[temp].gameObject.SetActive(!MediaParent[temp].gameObject.activeSelf);
            });

            //设置当前按钮的媒体并按类型加载
            for (int j = 0; j < buttonSettings[i].mediaList.Count; j++)
            {
                switch (buttonSettings[i].mediaList[j].mediaType)
                {
                    case MediaType.TextFile:
                        yield return StartCoroutine(InitTextFile(i, j));
                        break;
                    case MediaType.Image:
                        yield return StartCoroutine(InitImage(i, j));
                        break;
                    case MediaType.Video:
                        InitVideo(i, j);
                        break;
                    default:
                        Debug.LogError("Unknown MediaType");
                        break;
                }
            }
        }

        //初始化完毕之后关闭本身
        gameObject.SetActive(false);
        isInit = true;
    }

    public IEnumerator InitTextFile(int buttonSettingIndex, int mediaIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent))
        {
            yield return webRequest.SendWebRequest();
            string content = webRequest.downloadHandler.text;

            GameObject text = Instantiate(TextPrefab, MediaParent[buttonSettingIndex]);
            text.GetComponent<Text>().text = content;
        }
    }

    public IEnumerator InitImage(int buttonSettingIndex, int mediaIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent))
        {
            webRequest.downloadHandler = new DownloadHandlerTexture();
            yield return webRequest.SendWebRequest();
            Texture2D content = DownloadHandlerTexture.GetContent(webRequest);

            GameObject go = Instantiate(ImagePrefab, MediaParent[buttonSettingIndex]);
            RawImage raw = go.GetComponent<RawImage>();
            raw.texture = content;
            raw.SetNativeSize();
        }
    }

    public void InitVideo(int buttonSettingIndex, int mediaIndex)
    {
        //为按钮添加视频
        GameObject go = Instantiate(VideoPrefab, MediaParent[buttonSettingIndex]);
        VCR vcr = go.GetComponentInChildren<VCR>();
        vcr.paths = new string[] { buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent }; 
    }
}
