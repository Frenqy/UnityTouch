using RenderHeads.Media.AVProVideo.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Animations;

public class VideoCard : CardBase
{
    private RectTransform VideoDisplay;
    private RectTransform UI;


    protected override void Start()
    {
        IntroImg = transform.Find("UI/IntroImg").gameObject;
        AddBtn = transform.Find("UI/AddBtn").gameObject;
        CloseBtn = transform.Find("UI/CloseBtn").gameObject;
        VideoDisplay = transform.Find("VideoDisplay").GetComponent<RectTransform>();
        UI = transform.Find("UI").GetComponent<RectTransform>();

        //oldSize = VideoDisplay.sizeDelta;
    }

    public override void AddMedia()
    {
        base.AddMedia();

        mediaSetting.mediaType = MediaType.Video;
        mediaSetting.mediaContent = FileCommon.OpenFile("视频", new string[] { "mp4", "mkv", "avi", "wmv" });

        VideoDisplay.GetComponent<DisplayUGUI>().color = Color.white;

        ShowVideo();
    }

    private void ShowVideo()
    {
        VCR vcr = GetComponentInChildren<VCR>();
        vcr.paths = new string[] { mediaSetting.mediaContent };
        vcr.OnOpenVideoFile();

        StartCoroutine(DelaySetSize());
    }

    private IEnumerator DelaySetSize()
    {
        yield return new WaitForSeconds(1);
        Vector2 oldSize = VideoDisplay.sizeDelta;

        VideoDisplay.GetComponent<DisplayUGUI>().SetNativeSize();

        //计算大小变化
        Vector2 newSize = VideoDisplay.sizeDelta;
        Vector2 scale = newSize / oldSize;
        float scaleF = Mathf.Min(scale.x, scale.y);

        UI.sizeDelta *= scale;
        //UI.scaleOffset = Vector3.one * scale;
        //UI.constraintActive = true;
        //AddBtn.GetComponent<RectTransform>().sizeDelta *= scaleF;
        //CloseBtn.GetComponent<RectTransform>().sizeDelta *= scaleF;
    }
}