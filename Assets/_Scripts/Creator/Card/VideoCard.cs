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
    private RectTransform VideoDisplay; //主播放器
    private RectTransform UI;   //UI元素的父物体

    protected override void Start()
    {
        IntroImg = transform.Find("UI/IntroImg").gameObject;
        AddBtn = transform.Find("UI/AddBtn").gameObject;
        CloseBtn = transform.Find("UI/CloseBtn").gameObject;
        VideoDisplay = transform.Find("VideoDisplay").GetComponent<RectTransform>();
        UI = transform.Find("UI").GetComponent<RectTransform>();
    }

    public override void AddMedia()
    {
        base.AddMedia();

        //打开窗口获取文件
        mediaSetting.mediaType = MediaType.Video;
        mediaSetting.mediaContent = FileCommon.OpenFile("视频", new string[] { "mp4", "mkv", "avi", "wmv" });

        ShowVideo();
    }

    private void ShowVideo()
    {
        //背景颜色换回白色
        VideoDisplay.GetComponent<DisplayUGUI>().color = Color.white;

        //设置VCR脚本
        VCR vcr = GetComponentInChildren<VCR>();
        vcr.paths = new string[] { mediaSetting.mediaContent };
        vcr.OnOpenVideoFile();

        //因为VCR内切换播放器需要一点时间 所以延迟计算播放器的大小变化
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

        UI.sizeDelta *= scale;
    }
}