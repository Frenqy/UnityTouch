using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO.Compression;

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

    public static string OpenFile(string type)
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

}
