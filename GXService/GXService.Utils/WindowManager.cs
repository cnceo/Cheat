using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GXService.Utils
{
    public static class WindowManager
    {
        /// <summary>
        /// 进程回调处理函数
        /// </summary>
        /// <param name="enumedWnd"></param>
        /// <param name="enumedWndSet"></param>
        /// <returns></returns>
        public static bool ThreadWindowEnumCallBack(IntPtr enumedWnd, ref User32Api.EnumCallBackModel enumedWndSet)
        {
            //将枚举到的窗口添加到集合中传出
            enumedWndSet.Hwnds.Add(enumedWnd);
            return true;
        }
        /// <summary>
        /// 窗口回调处理函数
        /// </summary>
        /// <param name="enumedWnd">顶层窗体（顶层窗体就是不具有WS_CHILD风格的窗体，可以在屏幕上任意拖动）</param>
        /// <param name="enumedWndSet"></param>
        /// <returns></returns>
        public static bool WindowEnumCallBack(IntPtr enumedWnd, ref User32Api.EnumCallBackModel enumedWndSet)
        {
            //将枚举到的窗口添加到集合中传出
            enumedWndSet.Hwnds.Add(enumedWnd);
            return true;
        }

        /// <summary>
        /// 子窗口回调处理函数
        /// </summary>
        /// <param name="enumedWnd"></param>
        /// <param name="enumedWndSet"></param>
        /// <returns></returns>
        public static bool ChildWindowEnumCallBack(IntPtr enumedWnd, ref User32Api.EnumCallBackModel enumedWndSet)
        {
            //将枚举到的窗口添加到集合中传出
            enumedWndSet.Hwnds.Add(enumedWnd);
            return true;
        }

        public static List<IntPtr> GetChildWindows(this IntPtr hWnd)
        {
            var result = new User32Api.EnumCallBackModel
                {
                    Hwnds = new List<IntPtr>()
                };

            User32Api.EnumChildWindows(hWnd, ChildWindowEnumCallBack, ref result);

            return result.Hwnds;
        }

        public static Bitmap Capture(this Rectangle rect, IntPtr hWnd)
        {
            var bmp = Capture(hWnd);
            bmp =
                bmp.Clone(new Rectangle(rect.X < 0 ? 0 : rect.X,
                                        rect.Y < 0 ? 0 : rect.Y,
                                        bmp.Width < rect.Width ? bmp.Width : rect.Width,
                                        bmp.Height < rect.Height ? bmp.Height : rect.Height),
                          bmp.PixelFormat);

            return bmp;
        }

        public static Bitmap Capture(this Rectangle rect)
        {
            //创建图象，保存将来截取的图象
            var image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Width, rect.Height));
            return image;
        }

        public static Bitmap Capture(this IntPtr hWnd, Rectangle rect)
        {
            var bmp = Capture(hWnd);
            bmp = bmp.Clone(rect, bmp.PixelFormat);

            return bmp;
        }

        public static Bitmap Capture(this IntPtr hWnd)
        {
            var hscrdc = User32Api.GetWindowDC(hWnd);
            var windowRect = new User32Api.RectApi();
            User32Api.GetWindowRect(hWnd, ref windowRect);
            var width = windowRect.right - windowRect.left;
            var height = windowRect.bottom - windowRect.top;

            var hbitmap = Gdi32Api.CreateCompatibleBitmap(hscrdc, width, height);
            var hmemdc = Gdi32Api.CreateCompatibleDC(hscrdc);
            Gdi32Api.SelectObject(hmemdc, hbitmap);
            User32Api.PrintWindow(hWnd, hmemdc, 0);
            var bmp = Image.FromHbitmap(hbitmap);
            Gdi32Api.DeleteDC(hscrdc);//删除用过的对象
            Gdi32Api.DeleteDC(hmemdc);//删除用过的对象

            GC.Collect();

            return bmp;
        }

        public static IntPtr FindWindow(this string titleWnd)
        {
            return User32Api.FindWindow(null, titleWnd);
        }

        public static IntPtr FindWindow(this string titleWnd, string classWnd)
        {
            return User32Api.FindWindow(classWnd, titleWnd);
        }

        public static Rectangle GetWindowRect(this IntPtr hWnd)
        {
            var rect = new User32Api.RectApi();
            User32Api.GetWindowRect(hWnd, ref rect);
            return rect.ToRectangle();
        }

        public static bool SetForeground(this IntPtr hWnd)
        {
            return User32Api.SetForegroundWindow(hWnd);
        }

        public static string GetClassName(this IntPtr hWnd)
        {
            var sb = new StringBuilder(1024);
            User32Api.GetClassName(hWnd, sb, sb.Capacity);

            return sb.ToString();
        }

        public static string GetWindowText(this IntPtr hWnd)
        {
            var sb = new StringBuilder(hWnd.GetWindowTextLength());
            User32Api.GetWindowText(hWnd, sb, sb.Capacity);

            return sb.ToString();
        }

        public static int GetWindowTextLength(this IntPtr hWnd)
        {
            return User32Api.GetWindowTextLength(hWnd);
        }

        #region 树形控件
        public static string GetItemText(IntPtr treeViewHwnd, IntPtr itemHwnd)
        {
            string output;

            uint vProcessId;
            User32Api.GetWindowThreadProcessId(treeViewHwnd, out vProcessId);
            var vProcess = Kernel32Api.OpenProcess(
                WindowsMessageApi.PROCESS_VM_OPERATION |
                WindowsMessageApi.PROCESS_VM_READ |
                WindowsMessageApi.PROCESS_VM_WRITE,
                false,
                vProcessId
                );
            var vPointer = Kernel32Api.VirtualAllocEx(vProcess,
                                                      IntPtr.Zero,
                                                      4096,
                                                      WindowsMessageApi.MEM_RESERVE | WindowsMessageApi.MEM_COMMIT,
                                                      WindowsMessageApi.PAGE_READWRITE);

            try
            {
                var vBuffer = new byte[1024];
                var vItem = new User32Api.TVITEM[1];
                vItem[0] = new User32Api.TVITEM
                    {
                        mask = WindowsMessageApi.TVIF_TEXT,
                        hItem = itemHwnd,
                        pszText = (IntPtr) ((int) vPointer + Marshal.SizeOf(typeof (User32Api.TVITEM))),
                        cchTextMax = vBuffer.Length
                    };
                int vNumberOfBytesRead;

                Kernel32Api.WriteProcessMemory(
                    vProcess,
                    vPointer,
                    Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                    Marshal.SizeOf(typeof(User32Api.TVITEM)),
                    out vNumberOfBytesRead
                    );

                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETITEM, 0, vPointer);

                Kernel32Api.ReadProcessMemory(
                    vProcess,
                    new IntPtr(vPointer.ToInt32() + Marshal.SizeOf(typeof(User32Api.TVITEM))),
                    out vBuffer,
                    vBuffer.Length,
                    out vNumberOfBytesRead
                    );

                output = Marshal.PtrToStringAuto(Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0));
            }
            finally
            {
                Kernel32Api.VirtualFreeEx(vProcess, vPointer, 0, WindowsMessageApi.MEM_RELEASE);
                Kernel32Api.CloseHandle(vProcess);
            }

            return output;
        }
        #endregion
    }
}
