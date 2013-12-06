using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GXService.Utils
{
    public static class User32Api
    {
        #region 宏定义

        public const int MouseEventAbsolute = 0x8000;
        public const int MouserEventHwheel = 0x01000;
        public const int MouseEventMove = 0x0001;
        public const int MouseEventMoveNoCoalesce = 0x2000;
        public const int MouseEventLeftDown = 0x0002;
        public const int MouseEventLeftUp = 0x0004;
        public const int MouseEventMiddleDown = 0x0020;
        public const int MouseEventMiddleUp = 0x0040;
        public const int MouseEventRightDown = 0x0008;
        public const int MouseEventRightUp = 0x0010;
        public const int MouseEventWheel = 0x0800;
        public const int MousseEventXUp = 0x0100;
        public const int MousseEventXDown = 0x0080;

        #endregion

        #region 数据结构

        [StructLayout(LayoutKind.Explicit)]
        public struct Input
        {
            [FieldOffset(0)] public Int32 type;
            [FieldOffset(4)] public MouseInputApi mi;
            [FieldOffset(4)] public KeyBoardInputApi ki;
            [FieldOffset(4)] public HardwareInputApi hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInputApi
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 Mousedata;
            public Int32 dwFlag;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyBoardInputApi
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInputApi
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointApi
        {
            public Int32 X;
            public Int32 Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RectApi
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion

        #region API接口

        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref PointApi pt);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref PointApi pt);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RectApi rect);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
                                                 string lpszWindow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AnyPopup();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hWnd);

        #region 窗口枚举
        //窗口枚举回调函数定义
        public delegate bool CallBack(IntPtr hwnd, ref EnumCallBackModel lParam);

        //回调函数自定义数据类型，提供传出枚举到的窗口信息，可通过继承此类型，实现回调函数获取更多信息
        public class EnumCallBackModel
        {
            //枚举到的窗口
            public List<IntPtr> Hwnds { get; set; } 
        }

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int dwThreadId, CallBack lpfn, ref EnumCallBackModel enumedWndSet);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(CallBack lpfn, ref EnumCallBackModel enumedWndSet);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, CallBack lpfn, ref EnumCallBackModel enumedWndSet);
        #endregion

        //该函数获得指定窗口所属的类的类名。
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out uint ID);


        #region 树形控件
        ///   <summary>
        ///   TV_ITEM结构
        ///   </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
            public int HTreeItem;
        }
        #endregion

        #endregion
    }
}
