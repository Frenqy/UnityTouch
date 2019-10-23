using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SettingManager
{
    public static Setting setting = null;

    public static void LoadSetting(string path)
    {
        try
        {
            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                setting = JsonUtility.FromJson<Setting>(json);
            }

            //计算路径
            string[] splits = path.Split(new string[] { "\\" },System.StringSplitOptions.None);
            string prefix = string.Empty;
            for (int i = 0; i < splits.Length-1; i++)
            {
                prefix += splits[i];
                prefix += "\\";
            }

            //路径replace
            for (int i = 0; i < setting.markers.Count; i++)
            {
                for (int j = 0; j < setting.markers[i].buttonSetting.Count; j++)
                {
                    setting.markers[i].buttonSetting[j].previewPath = setting.markers[i].buttonSetting[j].previewPath.Replace("$PathPrefix$", prefix);
                    for (int k = 0; k < setting.markers[i].buttonSetting[j].mediaList.Count; k++)
                    {
                        setting.markers[i].buttonSetting[j].mediaList[k].mediaContent = setting.markers[i].buttonSetting[j].mediaList[k].mediaContent.Replace("$PathPrefix$", prefix);
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
