using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.IO.Compression;

public static class SettingManager
{
    public static Setting setting = null;

    public static void LoadSetting(string filepath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filepath, System.Text.Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                setting = JsonUtility.FromJson<Setting>(json);
            }

            string path, file;
            SplitFilePath(filepath, new string[] { "\\" }, out path, out file);

            //路径replace
            for (int i = 0; i < setting.markers.Count; i++)
            {
                for (int j = 0; j < setting.markers[i].buttonSetting.Count; j++)
                {
                    setting.markers[i].buttonSetting[j].previewPath = setting.markers[i].buttonSetting[j].previewPath.Replace("$PathPrefix$", path);
                    for (int k = 0; k < setting.markers[i].buttonSetting[j].mediaList.Count; k++)
                    {
                        setting.markers[i].buttonSetting[j].mediaList[k].mediaContent = setting.markers[i].buttonSetting[j].mediaList[k].mediaContent.Replace("$PathPrefix$", path);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public static void SaveSetting(Setting setting, string path)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                string json = JsonUtility.ToJson(setting, true);
                writer.Write(json);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 将完整的文件路径切割成 路径 + 文件名
    /// </summary>
    /// <param name="filepath">完整文件路径</param>
    /// <param name="seperator">路径分隔符（在json中通常为\\）</param>
    /// <param name="path">存放返回的路径</param>
    /// <param name="file">存放返回的文件名</param>
    public static void SplitFilePath(string filepath, string[] seperator, out string path, out string file)
    {
        //计算路径
        string[] splits = filepath.Split(seperator, System.StringSplitOptions.None);

        path = string.Empty;
        for (int i = 0; i < splits.Length - 1; i++)
        {
            path += splits[i];
            path += "\\";
        }

        file = splits[splits.Length - 1];

    }
}

/// <summary>
/// 管理完整的json配置文件
/// </summary>
[System.Serializable] public class Setting
{
    public List<MarkerSetting> markers = new List<MarkerSetting>();
}

/// <summary>
/// 保存一个marker的配置
/// </summary>
[System.Serializable] public class MarkerSetting
{
    public int MarkerID;
    public List<ButtonSetting> buttonSetting = new List<ButtonSetting>();
}

/// <summary>
/// 保存marker上某一个button的配置
/// </summary>
[System.Serializable] public class ButtonSetting
{
    public int buttonID;
    public string previewPath;
    public List<Media> mediaList = new List<Media>();
}

/// <summary>
/// 保存button上某一个Media的配置
/// </summary>
[System.Serializable] public class Media
{
    /// <summary>
    /// 媒体类型
    /// </summary>
    public MediaType mediaType;

    /// <summary>
    /// 媒体路径或内容（仅限文字）
    /// </summary>
    public string mediaContent;

    /// <summary>
    /// 默认摆放位置
    /// </summary>
    public Vector2 pos;

    /// <summary>
    /// 默认缩放
    /// </summary>
    public Vector2 scale;

    /// <summary>
    /// 默认旋转角度
    /// </summary>
    public Quaternion rotate;
}

public enum MediaType
{
    None,
    TextFile,   //从文件读取的文本
    TextContent,//直接编辑的文本
    Image,
    Video,
    Voice,
    ImageText,//图文
    ImageVoice,//图声
}
