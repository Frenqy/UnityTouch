using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace VIC.Core
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileDlg
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public string filter = null;
        public string customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public string file = null;
        public int maxFile = 0;
        public string fileTitle = null;
        public int maxFileTitle = 0;
        public string initialDir = null;
        public string title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public string defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public string templateName = null;
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
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 打开文件窗口保存文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
            else
            {
                return null;
            }
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

        /// <summary>
        /// 加密磁盘中的文件
        /// </summary>
        /// <param name="inFilePath">要加密的文件的路径</param>
        /// <param name="outFilePath">加密后文件的存放位置</param>
        /// <param name="key">密钥</param>
        /// <param name="progressCallback">进度回调，范围0-1</param>
        /// <returns></returns>
        public static IEnumerator EncryptFile(string inFilePath, string outFilePath, byte[] key, UnityAction<float> progressCallback)
        {
            const int E_BLOCKSIZE = 1048576;
            int readCount = 0;
            byte[] buffer;

            using (FileStream readFs = File.OpenRead(inFilePath))
            {
                using (FileStream writeFs = File.Create(outFilePath))
                {
                    buffer = new byte[E_BLOCKSIZE];
                    while ((readCount = readFs.Read(buffer, 0, E_BLOCKSIZE)) > 0)
                    {
                        if (readCount == E_BLOCKSIZE)
                        {
                            byte[] result = Encrypt(buffer, key);
                            writeFs.Write(result, 0, result.Length);
                        }
                        else
                        {
                            byte[] temp = new byte[readCount];
                            Buffer.BlockCopy(buffer, 0, temp, 0, readCount);
                            byte[] result = Encrypt(temp, key);
                            writeFs.Write(result, 0, result.Length);
                        }
                        float progress = (float)readFs.Position / readFs.Length;
                        progressCallback?.Invoke(progress);
                        yield return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 加密字节数组
        /// </summary>
        /// <param name="src">要加密的字节数组</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] src, byte[] key)
        {
            using (RijndaelManaged rDel = new RijndaelManaged())
            {
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = rDel.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(src, 0, src.Length);
                    return resultArray;
                }
            }
        }

        /// <summary>
        /// 解密磁盘中的文件
        /// </summary>
        /// <param name="inFilePath">要解密的文件的路径</param>
        /// <param name="outFilePath">解密后的文件的路径</param>
        /// <param name="key">密钥</param>
        /// <param name="progressCallback">进度回调，范围0-1</param>
        /// <returns></returns>
        public static IEnumerator DecryptFile(string inFilePath, string outFilePath, byte[] key, UnityAction<float> progressCallback)
        {
            const int D_BLOCKSIZE = 1048592;
            byte[] buffer;
            int readCount = 0;

            //读取文件
            using (FileStream readFs = File.OpenRead(inFilePath))
            {
                using (FileStream writeFs = File.Create(outFilePath))
                {
                    buffer = new byte[D_BLOCKSIZE];
                    while ((readCount = readFs.Read(buffer, 0, D_BLOCKSIZE)) > 0)
                    {
                        if (readCount == D_BLOCKSIZE)
                        {
                            byte[] result = Decrypt(buffer, key);
                            writeFs.Write(result, 0, result.Length);
                        }
                        else
                        {
                            byte[] temp = new byte[readCount];
                            Buffer.BlockCopy(buffer, 0, temp, 0, readCount);
                            byte[] result = Decrypt(temp, key);
                            writeFs.Write(result, 0, result.Length);
                        }
                        float progress = (float)readFs.Position / readFs.Length;
                        progressCallback?.Invoke(progress);
                        yield return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 解密字节数组
        /// </summary>
        /// <param name="src">要解密的字节数组</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] src, byte[] key)
        {
            using (RijndaelManaged rDel = new RijndaelManaged())
            {
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform cTransform = rDel.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(src, 0, src.Length);
                    return resultArray;
                }
            }
        }

        /// <summary>
        /// 将字节数组按指定大小划分
        /// </summary>
        /// <param name="ary"></param>
        /// <param name="subSize">要切割的大小</param>
        /// <returns></returns>
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
}
