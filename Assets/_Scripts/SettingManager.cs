using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingManager : MonoBehaviour
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

    public static void InitTestSetting()
    {
        MediaSetting m1 = new MediaSetting() { mediaPath = "1", mediaType = MediaType.None };
        MediaSetting m2 = new MediaSetting() { mediaPath = "2", mediaType = MediaType.Text };
        MediaSetting m3 = new MediaSetting() { mediaPath = "3", mediaType = MediaType.Image };
        MediaSetting m4 = new MediaSetting() { mediaPath = "4", mediaType = MediaType.Video };

        MarkerSetting s1 = new MarkerSetting();
        MarkerSetting s2 = new MarkerSetting();
        s1.MarkerID = 0;
        s2.MarkerID = 1;
        s1.medias.Add(m1);
        s1.medias.Add(m2);
        s1.medias.Add(m3);
        s2.medias.Add(m4);

        Setting s = new Setting();
        s.markers.Add(s1);
        s.markers.Add(s2);

        setting = s;
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
public struct MediaSetting
{
    public MediaType mediaType;
    public string mediaPath;
}

public enum MediaType
{
    None, Text, Image, Video
}
