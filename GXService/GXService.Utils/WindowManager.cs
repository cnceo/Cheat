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

        public static string GetItemText(this IntPtr treeViewHwnd, IntPtr itemHwnd)
        {
            var result = new StringBuilder(1024);

            uint vProcessId;
            User32Api.GetWindowThreadProcessId(treeViewHwnd, out vProcessId);
            var vProcess = Kernel32Api.OpenProcess(
                WindowsMessageApi.PROCESS_VM_OPERATION |
                WindowsMessageApi.PROCESS_VM_READ |
                WindowsMessageApi.PROCESS_VM_WRITE,
                false,
                vProcessId
                );

            var pStrBufferMemory = Kernel32Api.VirtualAllocEx(vProcess,
                                                              IntPtr.Zero,
                                                              1024,
                                                              WindowsMessageApi.MEM_COMMIT,
                                                              WindowsMessageApi.PAGE_READWRITE);
            var remoteBuffer = Kernel32Api.VirtualAllocEx(vProcess,
                                                          IntPtr.Zero,
                                                          (uint) Marshal.SizeOf(typeof (User32Api.TVITEM)),
                                                          WindowsMessageApi.MEM_COMMIT,
                                                          WindowsMessageApi.PAGE_EXECUTE_READWRITE);

            try
            {
                var tvItem = new User32Api.TVITEM
                    {
                        mask = WindowsMessageApi.TVIF_TEXT,
                        hItem = itemHwnd,
                        pszText = pStrBufferMemory,
                        cchTextMax = 1024
                    };

                var localBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(tvItem));
                Marshal.StructureToPtr(tvItem, localBuffer, false);

                int vNumberOfBytesWrite;
                Kernel32Api.WriteProcessMemory(
                    vProcess,
                    remoteBuffer,
                    localBuffer,
                    Marshal.SizeOf(typeof (User32Api.TVITEM)),
                    out vNumberOfBytesWrite);

                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETITEM, 0, remoteBuffer.ToInt32());

                int vNumberOfBytesRead;
                Kernel32Api.ReadProcessMemory(
                    vProcess,
                    pStrBufferMemory,
                    result,
                    1024,
                    out vNumberOfBytesRead
                    );


            }
            finally
            {
                Kernel32Api.VirtualFreeEx(vProcess, pStrBufferMemory, 0, WindowsMessageApi.MEM_RELEASE);
                Kernel32Api.VirtualFreeEx(vProcess, remoteBuffer, 0, WindowsMessageApi.MEM_RELEASE);
                Kernel32Api.CloseHandle(vProcess);
            }

            return result.ToString();
        }

        public static void ItemClick(this IntPtr treeViewHwnd, IntPtr itemHwnd)
        {
            treeViewHwnd.ItemClick(itemHwnd, new Size(0, 0));
        }

        public static void ItemClick(this IntPtr treeViewHwnd, IntPtr itemHwnd, Size offSet)
        {
            uint vProcessId;
            User32Api.GetWindowThreadProcessId(treeViewHwnd, out vProcessId);
            var vProcess = Kernel32Api.OpenProcess(
                WindowsMessageApi.PROCESS_ALL_ACCESS,
                false,
                vProcessId
                );

            var remoteBuffer = Kernel32Api.VirtualAllocEx(vProcess,
                                                          IntPtr.Zero,
                                                          (uint)Marshal.SizeOf(typeof(User32Api.RectApi)),
                                                          WindowsMessageApi.MEM_COMMIT,
                                                          WindowsMessageApi.PAGE_EXECUTE_READWRITE);

            try
            {
                var rc = new User32Api.RectApi { left = itemHwnd.ToInt32() };
                var localBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
                Marshal.StructureToPtr(rc, localBuffer, false);

                int vNumberOfBytes;
                Kernel32Api.WriteProcessMemory(vProcess, remoteBuffer, localBuffer,
                                               Marshal.SizeOf(typeof(User32Api.RectApi)), out vNumberOfBytes);
                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_SELECTITEM, WindowsMessageApi.TVGN_CARET,
                                      itemHwnd.ToInt32());
                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_ENSUREVISIBLE, 0, itemHwnd.ToInt32());
                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETITEMRECT, 1, remoteBuffer.ToInt32());
                Kernel32Api.ReadProcessMemory(vProcess, remoteBuffer, localBuffer,
                                               Marshal.SizeOf(typeof(User32Api.RectApi)), out vNumberOfBytes);
                rc = (User32Api.RectApi)Marshal.PtrToStructure(localBuffer, typeof(User32Api.RectApi));
                var pt = rc.ToRectangle().Center() + offSet;

                User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.WM_LBUTTONDBLCLK, 0, (int)MAKELPARAM((uint)pt.X, (uint)pt.Y));
            }
            finally
            {
                Kernel32Api.VirtualFreeEx(vProcess, remoteBuffer, (uint) Marshal.SizeOf(typeof (User32Api.RectApi)),
                                          WindowsMessageApi.MEM_FREE);
                Kernel32Api.CloseHandle(vProcess);
            }
        }

        public static uint MAKELPARAM(uint wLow, uint wHigh)
        {
            return ((0xffff & wHigh) << 16) | (wLow & 0xffff);
        }

        public static void SearchChildItem(this IntPtr treeViewHwnd, IntPtr itemHwnd, string nodeName, ref List<IntPtr> searchedItem)
        {
            if (treeViewHwnd.GetItemText(itemHwnd) == nodeName)
            {
                searchedItem.Add(itemHwnd); 
            }

            var childItem = treeViewHwnd.GetChildItem(itemHwnd);
            if (childItem != IntPtr.Zero)
            {
                treeViewHwnd.SearchChildItem(childItem, nodeName, ref searchedItem);
            }

            var nextItem = treeViewHwnd.GetNextItem(itemHwnd);
            if (nextItem != IntPtr.Zero)
            {
                treeViewHwnd.SearchChildItem(nextItem, nodeName, ref searchedItem);
            }
        }

        //获取第一个子节点
        public static IntPtr GetChildItem(this IntPtr treeViewHwnd, IntPtr prevItemHwnd)
        {
            return User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETNEXTITEM,
                                         WindowsMessageApi.TVGN_CHILD, prevItemHwnd.ToInt32());
        }

        //获取下个子节点
        public static IntPtr GetNextItem(this IntPtr treeViewHwnd, IntPtr prevItemHwnd)
        {
            return User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETNEXTITEM,
                                         WindowsMessageApi.TVGN_NEXT, prevItemHwnd.ToInt32());
        }

        //获取根节点
        public static IntPtr GetRootItem(this IntPtr treeViewHwnd)
        {
            return User32Api.SendMessage(treeViewHwnd, WindowsMessageApi.TVM_GETNEXTITEM,
                                         WindowsMessageApi.TVGN_ROOT, 0);
        }

        #endregion
    }
}
