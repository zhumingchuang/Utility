using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class OpenMultipleFileName : MonoBehaviour
{
    private void Start ()
    {
        var files = BrowseMultiFile ("全部文件\0*.*\0\0");
        foreach (var item in files)
        {
            Debug.Log (item);
        }

    }

    static List<string> BrowseMultiFile (string filter)
    {
        int size = 1024;
        List<string> list = new List<string> ();
        //多选文件是传出一个指针，这里需要提前分配空间
        //如果是单选文件，使用已经分配大小的StringBuilder或string
        IntPtr filePtr = Marshal.AllocHGlobal (size);

        //清空分配的内存区域
        for (int i = 0; i < size; i++)
        {
            Marshal.WriteByte (filePtr, i, 0);
        }

        OpenFileName openFileName = new OpenFileName ();
        openFileName.lStructSize = Marshal.SizeOf (openFileName);
        openFileName.lpstrFilter = filter;
        openFileName.filePtr = filePtr;
        openFileName.nMaxFile = size;
        openFileName.lpstrFileTitle = new string (new char[64]);
        openFileName.nMaxFileTitle = 64;
        openFileName.lpstrInitialDir = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
        openFileName.lpstrFileTitle = "浏览文件";
        openFileName.lpstrDefExt = "*.*";
        openFileName.Flags = WinAPI.OFN_EXPLORER | WinAPI.OFN_FILEMUSTEXIST | WinAPI.OFN_PATHMUSTEXIST | WinAPI.OFN_ALLOWMULTISELECT | WinAPI.OFN_NOCHANGEDIR;
        if (WinAPI.GetOpenFileName (openFileName))
        {
            var file = Marshal.PtrToStringAuto (openFileName.filePtr);
            while (!string.IsNullOrEmpty (file))
            {
                list.Add (file);
                //转换为地址
                long filePointer = (long)openFileName.filePtr;
                //偏移
                filePointer += file.Length * Marshal.SystemDefaultCharSize + Marshal.SystemDefaultCharSize;
                openFileName.filePtr = (IntPtr)filePointer;
                file = Marshal.PtrToStringAuto (openFileName.filePtr);
            }
        }

        //第一条字符串为文件夹路径，需要再拼成完整的文件路径
        if (list.Count > 1)
        {
            for (int i = 1; i < list.Count; i++)
            {
                list[i] = System.IO.Path.Combine (list[0], list[i]);
            }

            list = list.Skip (1).ToList ();
        }

        Marshal.FreeHGlobal (filePtr);
        return list;
    }

}


[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct OpenFileName
{
    public int lStructSize;
    public IntPtr hwndOwner;
    public IntPtr hInstance;
    public string lpstrFilter;
    public string lpstrCustomFilter;
    public int nMaxCustFilter;
    public int nFilterIndex;
    public IntPtr filePtr;  //多选文件时不能用string或StringBuilder
    public int nMaxFile;
    public string lpstrFileTitle;
    public int nMaxFileTitle;
    public string lpstrInitialDir;
    public string lpstrTitle;
    public int Flags;
    public short nFileOffset;
    public short nFileExtension;
    public string lpstrDefExt;
    public IntPtr lCustData;
    public IntPtr lpfnHook;
    public string lpTemplateName;
    public IntPtr pvReserved;
    public int dwReserved;
    public int flagsEx;
}

public class WinAPI
{

    public const int OFN_READONLY = 0x1;
    public const int OFN_OVERWRITEPROMPT = 0x2;
    public const int OFN_HIDEREADONLY = 0x4;
    public const int OFN_NOCHANGEDIR = 0x8;
    public const int OFN_SHOWHELP = 0x10;
    public const int OFN_ENABLEHOOK = 0x20;
    public const int OFN_ENABLETEMPLATE = 0x40;
    public const int OFN_ENABLETEMPLATEHANDLE = 0x80;
    public const int OFN_NOVALIDATE = 0x100;
    public const int OFN_ALLOWMULTISELECT = 0x200;
    public const int OFN_EXTENSIONDIFFERENT = 0x400;
    public const int OFN_PATHMUSTEXIST = 0x800;
    public const int OFN_FILEMUSTEXIST = 0x1000;
    public const int OFN_CREATEPROMPT = 0x2000;
    public const int OFN_SHAREAWARE = 0x4000;
    public const int OFN_NOREADONLYRETURN = 0x8000;
    public const int OFN_NOTESTFILECREATE = 0x10000;
    public const int OFN_NONETWORKBUTTON = 0x20000;
    public const int OFN_NOLONGNAMES = 0x40000;
    public const int OFN_EXPLORER = 0x80000;
    public const int OFN_NODEREFERENCELINKS = 0x100000;
    public const int OFN_LONGNAMES = 0x200000;
    public const int OFN_ENABLEINCLUDENOTIFY = 0x400000;
    public const int OFN_ENABLESIZING = 0x800000;
    public const int OFN_DONTADDTORECENT = 0x2000000;
    public const int OFN_FORCESHOWHIDDEN = 0x10000000;
    public const int OFN_EX_NOPLACESBAR = 0x1;
    public const int OFN_SHAREFALLTHROUGH = 2;
    public const int OFN_SHARENOWARN = 1;
    public const int OFN_SHAREWARN = 0;

    [DllImport ("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName ([In, Out] OpenFileName ofn);
}