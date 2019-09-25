using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Windows.Forms;

public class MarkerManager : MonoBehaviour
{
    public GameObject MarkerPrefab;
    public GameObject TextPrefab;
    public GameObject ImagePrefab;
    public GameObject VideoPrefab;

    public RectTransform UIParent;

    private List<Marker> markers = new List<Marker>();

    private void OnEnable()
    {
        TouchInput.MarkerUpdated += ShowMarker;
    }

    private void OnDisable()
    {
        TouchInput.MarkerUpdated -= ShowMarker;
    }

    private void Start()
    {
        GetJson();

        //Init(@"E:\openFramework\SettingTest\marker.json");

        //Setting s = SettingManager.setting;
        //string[] paths = { @"E:\openFramework\SettingTest\1.txt", "" };
        //s.markers[0].medias[0].mediaPath = paths;
        //SettingManager.SaveSetting(s, @"E:\openFramework\SettingTest\new.json");
    }

    /// <summary>
    /// 从windows资源管理器中选择json文件
    /// </summary>
    private void GetJson()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.InitialDirectory = UnityEngine.Application.dataPath;
        dialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
        if (dialog.ShowDialog()==DialogResult.OK)
        {
            //Debug.Log(dialog.FileName);
            Init(dialog.FileName);
        }
    }

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

        //将详细的媒体配置信息传到marker的脚本内，并打开一次marker，初始化信息后再关闭
        for (int i = 0; i < SettingManager.setting.markers.Count; i++)
        {
            int id = SettingManager.setting.markers[i].MarkerID;
            markers[id].medias = SettingManager.setting.markers[i].medias;

            markers[id].TextPrefab = TextPrefab;
            markers[id].ImagePrefab = ImagePrefab;
            markers[id].VideoPrefab = VideoPrefab;

            markers[id].gameObject.SetActive(true);
        }
    }

    public void ShowMarker(List<Triangel> triangels)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < triangels.Count; i++)
        {
            ids.Add(triangels[i].ID);

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIParent, triangels[i].Center, null, out pos);
            markers[triangels[i].ID].GetComponent<RectTransform>().anchoredPosition = pos;
            markers[triangels[i].ID].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, triangels[i].Rotate);
        }

        for (int i = 0; i < markers.Count; i++)
        {
            if (!markers[i].isInit) continue;

            markers[i].gameObject.SetActive(ids.Contains(i));
            //if (ids.Contains(i))
            //{
            //    markers[i].gameObject.SetActive(true);
            //}
        }

    }
}
