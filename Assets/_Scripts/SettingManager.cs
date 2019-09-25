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

[System.Serializable]
public class Setting
{
    public List<MarkerSetting> markers = new List<MarkerSetting>();
}

[System.Serializable]
public class MarkerSetting
{
    public int MarkerID;
    public List<MediaSetting> medias = new List<MediaSetting>();
}

[System.Serializable]
public class MediaSetting
{
    public int buttonID;
    public MediaType mediaType;
    public string previewPath;
    public string[] mediaPath;
}

public enum MediaType
{
    None, Text, Image, Video
}
