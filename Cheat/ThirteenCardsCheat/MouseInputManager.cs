using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThirteenCardsCheat
{

    public class MouseInputManager
    {
        protected const int MouseEventAbsolute = 0x8000;
        protected const int MouserEventHwheel = 0x01000;
        protected const int MouseEventMove = 0x0001;
        protected const int MouseEventMoveNoCoalesce = 0x2000;
        protected const int MouseEventLeftDown = 0x0002;
        protected const int MouseEventLeftUp = 0x0004;
        protected const int MouseEventMiddleDown = 0x0020;
        protected const int MouseEventMiddleUp = 0x0040;
        protected const int MouseEventRightDown = 0x0008;
        protected const int MouseEventRightUp = 0x0010;
        protected const int MouseEventWheel = 0x0800;
        protected const int MousseEventXUp = 0x0100;
        protected const int MousseEventXDown = 0x0080;

        [DllImport("user32.dll")]
        protected static extern UInt32 SendInput(UInt32 nInputs, Input[] pInputs, int cbSize);

        public static void Click(Int32 x, Int32 y)
        {
            var myMinput = new MouseInput
                {
                    dx = x * (65355 / Screen.PrimaryScreen.WorkingArea.Width),
                    dy = y * (65355 / Screen.PrimaryScreen.WorkingArea.Height),
                    Mousedata = 0,
                    dwFlag = MouseEventAbsolute | MouseEventMove | MouseEventLeftDown | MouseEventLeftUp,
                    time = 0
                };

            var myInput = new[]{new Input {type = 0, mi = myMinput}};

            if (SendInput((uint)myInput.Length, myInput, Marshal.SizeOf(myInput[0].GetType())) == 0)
            {
                throw new Exception(string.Format("模拟鼠标点击失败!X:{0} Y: {1}", x, y));
            }
        }

        public static void DoubleClick(Int32 x, Int32 y)
        {
            Click(x, y);
            Click(x, y);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Input
    {
        [FieldOffset(0)]
        public Int32 type;
        [FieldOffset(4)]
        public MouseInput mi;
        [FieldOffset(4)]
        public tagKEYBDINPUT ki;
        [FieldOffset(4)]
        public tagHARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public Int32 dx;
        public Int32 dy;
        public Int32 Mousedata;
        public Int32 dwFlag;
        public Int32 time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagKEYBDINPUT
    {
        Int16 wVk;
        Int16 wScan;
        Int32 dwFlags;
        Int32 time;
        IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagHARDWAREINPUT
    {
        Int32 uMsg;
        Int16 wParamL;
        Int16 wParamH;
    }
}
