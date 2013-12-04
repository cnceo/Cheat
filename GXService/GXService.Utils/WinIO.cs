using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GXService.Utils
{
    public static class WinIoApi
    {
        public const int KbcKeyCmd = 0x64;
        public const int KbcKeyData = 0x60;

        [DllImport("winio.dll")]
        public static extern bool InitializeWinIo();

        [DllImport("winio.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);

        [DllImport("winio.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);

        [DllImport("winio.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);

        [DllImport("winio.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);

        [DllImport("winio.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);

        [DllImport("winio.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);

        [DllImport("winio.dll")]
        public static extern void ShutdownWinIo();
    }

    public class WinIoLab
    {
        public static readonly WinIoLab Singleton = new WinIoLab();

        private WinIoLab()
        {
            Initialize();
        }

        /// <summary>
        /// 等待键盘缓冲区为空
        /// </summary>
        private void KbcWait4Ibe()
        {
            int dwVal;
            do
            {
                WinIoApi.GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }

        private void Initialize()
        {
            if (WinIoApi.InitializeWinIo())
            {
                KbcWait4Ibe();
            }
        }

        public void Shutdown()
        {
            WinIoApi.ShutdownWinIo();
        }

        /// <summary>
        /// 模拟键盘标按下
        /// </summary>
        /// <param name="vKeyCoad"></param>
        public void KeyDown(Keys vKeyCoad)
        {
            var btScancode = User32Api.MapVirtualKey((uint)vKeyCoad, 0);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyCmd, (IntPtr)0xD2, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyData, (IntPtr)0x60, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyCmd, (IntPtr)0xD2, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyData, (IntPtr)btScancode, 1);
        }

        /// <summary>
        /// 模拟键盘弹出
        /// </summary>
        /// <param name="vKeyCoad"></param>
        public void KeyUp(Keys vKeyCoad)
        {
            var btScancode = User32Api.MapVirtualKey((uint)vKeyCoad, 0);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyCmd, (IntPtr)0xD2, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyData, (IntPtr)0x60, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyCmd, (IntPtr)0xD2, 1);
            KbcWait4Ibe();
            WinIoApi.SetPortVal(WinIoApi.KbcKeyData, (IntPtr)(btScancode | 0x80), 1);
        }

        public void KeyPress(Keys vKeyCode)
        {
            KeyDown(vKeyCode);
            Thread.Sleep(200);
            KeyUp(vKeyCode);
            Thread.Sleep(200);
        }

        public void KeyPress(List<Keys> keyses)
        {
            keyses.ForEach(KeyPress);
        }
    }
}
