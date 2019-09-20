using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    public GameObject MarkerPrefab;
    public GameObject TextPrefab;
    public GameObject ImagePrefab;
    public GameObject VideoPrefab;

    public Transform UIParent;

    private List<Marker> markers = new List<Marker>();

    public void Init(string jsonPath)
    {
        //读取设置
        SettingManager.LoadSetting(jsonPath);

        //提前生成marker（按Degrees数量而不是marker中配置数量生成，这样在管理状态的时候会更方便）
        for (int i = 0; i < Triangel.Degrees.Length; i++)
        {
            GameObject go = Instantiate(MarkerPrefab, UIParent);
            Marker m = go.GetComponent<Marker>();
            markers.Add(m);
        }

        //将详细的媒体配置信息传到marker的脚本内
        for (int i = 0; i < SettingManager.setting.markers.Count; i++)
        {
            int id = SettingManager.setting.markers[i].MarkerID;
            markers[id].medias = SettingManager.setting.markers[i].medias;

            markers[id].TextPrefab = TextPrefab;
            markers[id].ImagePrefab = ImagePrefab;
            markers[id].VideoPrefab = VideoPrefab;
        }
    }
}
