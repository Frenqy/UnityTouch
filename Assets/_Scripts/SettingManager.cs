using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SettingManager
{
    public static Setting setting = null;

    public static string tempPath;

    private static void LoadSetting(string filepath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filepath, System.Text.Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                setting = JsonUtility.FromJson<Setting>(json);
            }

            string path, file;
            FileCommon.SplitFilePath(filepath, new string[] { "\\" }, out path, out file);

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

    /// <summary>
    /// 将Setting转换成Json文件
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="path"></param>
    private static void SaveSetting(Setting setting, string path)
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
    /// 将Setting以及对应的资源文件打包
    /// </summary>
    /// <param name="setting"></param>
    public static void PackSetting(Setting setting)
    {
        string path = FileCommon.SaveFile("vkxr");
        List<string> fileList = new List<string>();

        //修改Setting并生成json
        string outpath, outfile;
        for (int i = 0; i < setting.markers.Count; i++)
        {
            for (int j = 0; j < setting.markers[i].buttonSetting.Count; j++)
            {
                fileList.Add(setting.markers[i].buttonSetting[j].previewPath);
                FileCommon.SplitFilePath(setting.markers[i].buttonSetting[j].previewPath, new string[] { "\\" }, out outpath, out outfile);
                setting.markers[i].buttonSetting[j].previewPath = "$PathPrefix$"+ outfile;

                for (int k = 0; k < setting.markers[i].buttonSetting[j].mediaList.Count; k++)
                {
                    fileList.Add(setting.markers[i].buttonSetting[j].mediaList[k].mediaContent);
                    int length = FileCommon.SplitFilePath(setting.markers[i].buttonSetting[j].mediaList[k].mediaContent, new string[] { "\\" }, out outpath, out outfile);
                    if (length != 1) //判断mediaContent内存放的是路径还是文本内容
                    {
                        setting.markers[i].buttonSetting[j].mediaList[k].mediaContent = "$PathPrefix$" + outfile;
                    }
                }
            }
        }
        FileCommon.SplitFilePath(path, new string[] { "\\" }, out outpath, out outfile);
        string jsonPath = outpath + "setting.json";
        SaveSetting(setting, jsonPath);
        fileList.Add(jsonPath);

        string[] files = fileList.Distinct().ToArray();

        ZipUtility.Zip(files, path);
        File.Delete(jsonPath);
    }

    public static void LoadSettingPack()
    {
        string packPath = FileCommon.OpenFile("vkxr");
        tempPath = Application.streamingAssetsPath.Replace('/', '\\') + "\\tmp";
        ZipUtility.UnzipFile(packPath, tempPath);

        LoadSetting(tempPath + "\\setting.json");
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
