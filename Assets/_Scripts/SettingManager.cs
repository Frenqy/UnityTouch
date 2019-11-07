using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace VIC.Core
{
    public static class SettingManager
    {
        /// <summary>
        /// 存储从json读出来的Setting
        /// </summary>
        private static Setting setting = null;

        /// <summary>
        /// Setting内的详细marker信息
        /// </summary>
        public static List<MarkerSetting> markers => setting?.markers;

        public static string TempPath { get; private set; }

        private static readonly byte[] KEY = new byte[] { 0, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 0 };

        /// <summary>
        /// 将Setting以及对应的资源文件打包
        /// </summary>
        /// <param name="packSetting"></param>
        /// <returns>保存模板是否成功</returns>
        public static IEnumerator PackSetting(Setting packSetting, UnityAction<float> progressCallback)
        {
            //获取保存路径
            string zipFilePath = FileCommon.SaveFile("vkxr");
            //临时存放要打包的文件的路径
            List<string> fileList = new List<string>();

            //修改Setting，统计需要打包的文件
            string outpath, outfile;
            for (int i = 0; i < packSetting.markers.Count; i++)
            {
                for (int j = 0; j < packSetting.markers[i].buttonSetting.Count; j++)
                {
                    //循环：设置缩略图路径
                    //判空：判断有没有设置缩略图
                    if (!string.IsNullOrEmpty(packSetting.markers[i].buttonSetting[j].previewPath))
                    {
                        fileList.Add(packSetting.markers[i].buttonSetting[j].previewPath);
                        FileCommon.SplitFilePath(packSetting.markers[i].buttonSetting[j].previewPath, new string[] { "\\" }, out outpath, out outfile);
                        packSetting.markers[i].buttonSetting[j].previewPath = "$PathPrefix$" + outfile;
                    }

                    //内部循环：设置媒体文件路径
                    for (int k = 0; k < packSetting.markers[i].buttonSetting[j].mediaList.Count; k++)
                    {
                        fileList.Add(packSetting.markers[i].buttonSetting[j].mediaList[k].mediaContent);
                        int length = FileCommon.SplitFilePath(packSetting.markers[i].buttonSetting[j].mediaList[k].mediaContent, new string[] { "\\" }, out outpath, out outfile);
                        if (length != 1) //判断mediaContent内存放的是路径还是文本内容
                        {
                            packSetting.markers[i].buttonSetting[j].mediaList[k].mediaContent = "$PathPrefix$" + outfile;
                        }
                    }
                }
            }
            FileCommon.SplitFilePath(zipFilePath, new string[] { "\\" }, out outpath, out outfile);

            //生成json并添加进打包列表
            string jsonPath = outpath + "setting.json";
            SaveSettingToJson(packSetting, jsonPath);
            fileList.Add(jsonPath);

            //将需要打包的文件列表转换成数组
            string[] files = fileList.Distinct().ToArray();

            //打包成zip
            bool packResult = ZipUtility.Zip(files, zipFilePath + ".tmp");
            //对zip进行加密
            if (packResult)
            {
                yield return FileCommon.EncryptFile(zipFilePath + ".tmp", zipFilePath, KEY, progressCallback);
            }
            //清理临时文件
            File.Delete(jsonPath);
            File.Delete(zipFilePath + ".tmp");

            //return packResult;
        }

        /// <summary>
        /// 加载vkxr资源包
        /// </summary>
        /// <returns>加载结果 需要返回true再继续</returns>
        public static IEnumerator LoadSettingPack(UnityAction<float> progressCallback)
        {
            //获取模板文件
            string packPath = FileCommon.OpenFile("vkxr");

#if UNITY_EDITOR
            //编辑器模式下不操作StreamingAssets文件夹
            //因为会触发资源导入
            TempPath = @"D:\\tmp";
#else
        TempPath = Application.streamingAssetsPath.Replace('/', '\\') + "\\tmp";
#endif

            // 创建文件目录
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            //解密并尝试解压
            yield return FileCommon.DecryptFile(packPath, TempPath + "\\pack.tmp", KEY, progressCallback);
            bool unpackResult = ZipUtility.UnzipFile(TempPath + "\\pack.tmp", TempPath);
            //清理临时文件
            File.Delete(TempPath + "\\pack.tmp");
            //读取json
            if (unpackResult)
            {
                LoadSettingFromJson(TempPath + "\\setting.json");
            }

            //return unpackResult;
        }

        private static void LoadSettingFromJson(string filepath)
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
                for (int i = 0; i < markers.Count; i++)
                {
                    for (int j = 0; j < markers[i].buttonSetting.Count; j++)
                    {
                        markers[i].buttonSetting[j].previewPath = markers[i].buttonSetting[j].previewPath.Replace("$PathPrefix$", path);
                        for (int k = 0; k < markers[i].buttonSetting[j].mediaList.Count; k++)
                        {
                            markers[i].buttonSetting[j].mediaList[k].mediaContent = markers[i].buttonSetting[j].mediaList[k].mediaContent.Replace("$PathPrefix$", path);
                        }
                    }
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static void SaveSettingToJson(Setting setting, string path)
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
    [System.Serializable]
    public class Setting
    {
        public List<MarkerSetting> markers = new List<MarkerSetting>();
    }

    /// <summary>
    /// 保存一个marker的配置
    /// </summary>
    [System.Serializable]
    public class MarkerSetting
    {
        public int MarkerID;
        public List<ButtonSetting> buttonSetting = new List<ButtonSetting>();
    }

    /// <summary>
    /// 保存marker上某一个button的配置
    /// </summary>
    [System.Serializable]
    public class ButtonSetting
    {
        public int buttonID;
        public string previewPath;
        public List<Media> mediaList = new List<Media>();
    }

    /// <summary>
    /// 保存button上某一个Media的配置
    /// </summary>
    [System.Serializable]
    public class Media
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

    [System.Serializable]
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
}

