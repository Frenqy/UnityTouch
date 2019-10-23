using System.Collections.Generic;
using UnityEngine;
//using System.Windows.Forms;

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
    }

    /// <summary>
    /// 从windows资源管理器中选择json文件
    /// </summary>
    private void GetJson()
    {

        string path = OpenFileDialog.OpenFile("json");
        Debug.Log(path);
        Init(path);
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
            markers[id].buttonSettings = SettingManager.setting.markers[i].buttonSetting;

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
            //保护marker的初始化状态，确保资源正确加载进入Unity
            if (!markers[i].isInit) continue;

            //根据识别出的三角形id表，设置marker的活动状态
            markers[i].gameObject.SetActive(ids.Contains(i));
        }

    }
}
