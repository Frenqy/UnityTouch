using RenderHeads.Media.AVProVideo.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VIC.Core;

namespace VIC.Player.Marker
{
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
                //初始化按钮本身
                yield return StartCoroutine(InitButton(i));

                //设置当前按钮的媒体并按类型加载
                for (int j = 0; j < buttonSettings[i].mediaList.Count; j++)
                {
                    switch (buttonSettings[i].mediaList[j].mediaType)
                    {
                        case MediaType.TextFile:
                            yield return StartCoroutine(InitTextFile(i, j));
                            break;
                        case MediaType.TextContent:
                            InitTextContent(i, j);
                            break;
                        case MediaType.Image:
                            yield return StartCoroutine(InitImage(i, j));
                            break;
                        case MediaType.Video:
                            InitVideo(i, j);
                            break;
                        case MediaType.Voice:
                        case MediaType.ImageText:
                        case MediaType.ImageVoice:
                            Debug.LogWarning("功能未完成");
                            break;
                        default:
                            Debug.LogError("Unknown MediaType");
                            break;
                    }
                }
            }

            //初始化完毕之后关闭marker本身
            gameObject.SetActive(false);
            isInit = true;
        }

        private IEnumerator InitButton(int buttonSettingIndex)
        {
            //设置每个按钮的缩略图
            if (!string.IsNullOrEmpty(buttonSettings[buttonSettingIndex].previewPath))
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + buttonSettings[buttonSettingIndex].previewPath))
                {
                    webRequest.downloadHandler = new DownloadHandlerTexture();
                    yield return webRequest.SendWebRequest();
                    PreviewImg[buttonSettings[buttonSettingIndex].buttonID].texture = DownloadHandlerTexture.GetContent(webRequest);
                }
            }

            //设置按钮本身的点击功能
            //打开有功能的按钮
            buttons[buttonSettings[buttonSettingIndex].buttonID].transform.parent.gameObject.SetActive(true);

            //按钮功能：打开对应的media
            buttons[buttonSettings[buttonSettingIndex].buttonID].onClick.AddListener(() =>
            {
                MediaParent[buttonSettingIndex].gameObject.SetActive(!MediaParent[buttonSettingIndex].gameObject.activeSelf);
            });
        }

        private IEnumerator InitTextFile(int buttonSettingIndex, int mediaIndex)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(@"file://" + buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent))
            {
                yield return webRequest.SendWebRequest();
                string content = webRequest.downloadHandler.text;

                GameObject text = Instantiate(TextPrefab, MediaParent[buttonSettingIndex]);
                text.GetComponent<Text>().text = content;
            }
        }

        private void InitTextContent(int buttonSettingIndex, int mediaIndex)
        {
            string content = buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent;
            GameObject text = Instantiate(TextPrefab, MediaParent[buttonSettingIndex]);
            text.GetComponent<Text>().text = content;
        }

        private IEnumerator InitImage(int buttonSettingIndex, int mediaIndex)
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

        private void InitVideo(int buttonSettingIndex, int mediaIndex)
        {
            //为按钮添加视频
            GameObject go = Instantiate(VideoPrefab, MediaParent[buttonSettingIndex]);
            VCR vcr = go.GetComponentInChildren<VCR>();
            vcr.paths = new string[] { buttonSettings[buttonSettingIndex].mediaList[mediaIndex].mediaContent };
        }
    }
}
