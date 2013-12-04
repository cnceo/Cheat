using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GXService.Utils
{
    public static class PointExtension
    {
        public static User32Api.PointApi ToPointApi(this Point pt)
        {
            return new User32Api.PointApi { X = pt.X, Y = pt.Y };
        }

        public static Point ToPoint(this User32Api.PointApi ptApi)
        {
            return new Point(ptApi.X, ptApi.Y);
        }
    }

    public static class MouseControlExtension
    {
        public static void MouseLClick(this Point pt)
        {
            var myMinput = new User32Api.MouseInputApi
                {
                    dx = pt.X * (65355 / Screen.PrimaryScreen.Bounds.Width),
                    dy = pt.Y * (65355 / Screen.PrimaryScreen.Bounds.Height),
                    Mousedata = 0,
                    dwFlag = User32Api.MouseEventAbsolute |
                             User32Api.MouseEventMove |
                             User32Api.MouseEventLeftDown |
                             User32Api.MouseEventLeftUp,
                    time = 0
                };

            var myInput = new[] { new User32Api.Input { type = 0, mi = myMinput } };

            if (User32Api.SendInput((uint)myInput.Length, myInput, Marshal.SizeOf(myInput[0].GetType())) == 0)
            {
                throw new Exception(string.Format("模拟鼠标点击失败!X:{0} Y: {1}", pt.X, pt.Y));
            }
        }

        public static void MouseLClick(this Point pt, IntPtr hWnd)
        {
            var ptApi = pt.ToPointApi();
            User32Api.ClientToScreen(hWnd, ref ptApi);
            ptApi.ToPoint().MouseLClick();
        }

        public static void MouseClick(this IntPtr hWnd, Point pt)
        {
            var ptApi = pt.ToPointApi();
            User32Api.ClientToScreen(hWnd, ref ptApi);
            ptApi.ToPoint().MouseLClick();
        }
    }
}
