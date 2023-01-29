using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OpenFileTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string fileExtName;
        fileExtName = "模型文件(*.fbx,*.obj,*.gltf,*.rvt)\0*.fbx;*.obj;*.gltf;*.rvt\0倾斜摄影(*.osgb)\0*.osgb\0ALL\0*.*\0";
        // fileExtName = "TIFF文件(*.tif)\0*.tif\0";
        string path;
        OpenFile (out path, fileExtName);
    }

    private void OpenFile (out string path, string fileExtName)
    {
        OpenFileSystem ofn = new OpenFileSystem ();
        ofn.structSize = Marshal.SizeOf (ofn);

        ofn.filter = fileExtName;
        ofn.file = new string (new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string (new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = @"C:\";
        ofn.title = $"打开: {fileExtName} 格式";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        path = null;
        if (OpenWindowsDll.GetOpenFileName (ofn))
        {
            path = ofn.file;
        }
    }
}



[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileSystem
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
    public String initialDir = @"C:\Users\";
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

[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}


public class OpenWindowsDll
{
    [DllImport ("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName ([In, Out] OpenFileSystem ofn);


    [DllImport ("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder ([In, Out] OpenDialogDir ofn);

    [DllImport ("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList ([In] IntPtr pidl, [In, Out] char[] fileName);
}
