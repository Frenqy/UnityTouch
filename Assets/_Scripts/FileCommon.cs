using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO.Compression;
using System.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenFileDlg
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class FileCommon
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileDlg ofd);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileDlg ofn); 


    public static bool GetOFN([In, Out] OpenFileDlg ofn)
    {
        return GetOpenFileName(ofn);//执行打开文件的操作
    }

    public static bool SaveOFN([In, Out] OpenFileDlg ofn)
    {
        return GetSaveFileName(ofn);
    }

    /// <summary>
    /// 打开文件窗口选择指定类型文件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string OpenFile(string type)
    {
        return OpenFile(type, new string[] { type });
    }

    /// <summary>
    /// 打开文件窗口选择多种类型文件
    /// </summary>
    /// <param name="formatName">文件类型的描述，如：图片</param>
    /// <param name="type">文件类型数组，如： { "jpg", "png", "bmp", "gif" }</param>
    /// <returns></returns>
    public static string OpenFile(string formatName, string[] type)
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = $"{type}文件(*." + type + ")\0*." + type;
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.streamingAssetsPath.Replace('/', '\\');
        pth.title = "选择文件";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        pth.filter = $"{formatName}文件(";
        for (int i = 0; i < type.Length; i++)
        {
            pth.filter += $"*.{type[i]}";
        }
        pth.filter += ")\0";
        for (int i = 0; i < type.Length - 1; i++)
        {
            pth.filter += $"*.{type[i]};";
        }
        pth.filter += $"*.{type[type.Length - 1]};";

        if (GetOpenFileName(pth))
        {
            string filepath = pth.file;
            return filepath;
        }
        else return null;
    }


    public static string SaveFile(string type)
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = $"{type}文件(*." + type + ")\0*." + type;
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.streamingAssetsPath.Replace('/', '\\');
        pth.title = "选择文件";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (GetSaveFileName(pth))
        {
            return pth.file + "." + type;
        }
        else return null;
    }

    /// <summary>
    /// 将完整的文件路径切割成 路径 + 文件名
    /// </summary>
    /// <param name="filepath">完整文件路径</param>
    /// <param name="seperator">路径分隔符（在json中通常为\\）</param>
    /// <param name="path">存放返回的路径</param>
    /// <param name="file">存放返回的文件名</param>
    public static int SplitFilePath(string filepath, string[] seperator, out string path, out string file)
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

        return splits.Length;
    }

    public static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
        {
            throw new ArgumentException("please input path");
        }
        else
        {
            StringBuilder builder = new StringBuilder();
            string spliter = "\\";
            string firstPath = paths[0];

            if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
            {
                spliter = "/";
            }

            if (!firstPath.EndsWith(spliter))
            {
                firstPath = firstPath + spliter;
            }
            builder.Append(firstPath);

            for (int i = 1; i < paths.Length; i++)
            {
                string nextPath = paths[i];
                if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                {
                    nextPath = nextPath.Substring(1);
                }

                if (i != paths.Length - 1)//not the last one
                {
                    if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                    {
                        nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                    }
                    else
                    {
                        nextPath = nextPath + spliter;
                    }
                }

                builder.Append(nextPath);
            }

            return builder.ToString();
        }
    }

    public static IEnumerator Encrypt(string inFilePath, string outFilePath, byte[] key, UnityAction<float> progressCallback)
    {
        byte[] src;

        //读取文件
        using (FileStream fs = File.OpenRead(inFilePath))
        {
            src = new byte[fs.Length];
            fs.Read(src, 0, src.Length);
        }

        List<byte[]> srcs = SplitArray(src, 1048576);

        //写入文件
        using (FileStream fs = File.Create(outFilePath))
        {
            using (RijndaelManaged rDel = new RijndaelManaged())
            {
                //加密操作
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = rDel.CreateEncryptor())
                {
                    for (int i = 0; i < srcs.Count; i++)
                    {
                        byte[] resultArray = cTransform.TransformFinalBlock(srcs[i], 0, srcs[i].Length);
                        fs.Write(resultArray, 0, resultArray.Length);

                        float progress = i / (float)srcs.Count;
                        progressCallback?.Invoke(progress);
                        yield return 0;
                    }
                }
            }
        }
    }

    public static IEnumerator Decrypt(string inFilePath, string outFilePath, byte[] key, UnityAction<float> progressCallback)
    {
        byte[] src;

        //读取文件
        using (FileStream fs = File.OpenRead(inFilePath))
        {
            src = new byte[fs.Length];
            fs.Read(src, 0, src.Length);
        }

        List<byte[]> srcs = SplitArray(src, 1048592);

        //写入文件
        using (FileStream fs = File.Create(outFilePath))
        {
            using (RijndaelManaged rDel = new RijndaelManaged())
            {
                //解密文件
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = rDel.CreateDecryptor())
                {
                    for (int i = 0; i < srcs.Count; i++)
                    {
                        byte[] resultArray = cTransform.TransformFinalBlock(srcs[i], 0, srcs[i].Length);
                        fs.Write(resultArray, 0, resultArray.Length);

                        float progress = i / (float)srcs.Count;
                        progressCallback?.Invoke(progress);
                        yield return 0;
                    }
                }
            }
        }
    }

    public static List<byte[]> SplitArray(byte[] ary, int subSize)
    {
        int count = ary.Length % subSize == 0 ? ary.Length / subSize : ary.Length / subSize + 1;
        List<byte[]> subAryList = new List<byte[]>();
        for (int i = 0; i < count; i++)
        {
            int index = i * subSize;
            byte[] subary = ary.Skip(index).Take(subSize).ToArray();
            subAryList.Add(subary);
        }
        return subAryList;
    }
}
