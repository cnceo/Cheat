using System;
using System.Runtime.InteropServices;

namespace GXService.Utils
{
    public static class Gdi32Api
    {
        #region 宏定义
        public const int SrcCopy = 0x00CC0020;
        #endregion

        #region API接口
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                                         int nWidth, int nHeight, IntPtr hObjectSource,
                                         int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
                                        int nWidth, int nHeight, IntPtr hdcSrc,
                                        int nXSrc, int nYSrc, UInt32 dwRop);
        #endregion
    }
}
