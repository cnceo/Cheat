using System;
using System.Drawing;

namespace GXService.Utils
{
    public static class WindowManager
    {
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
    }
}
