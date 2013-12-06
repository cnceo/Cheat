using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GXService.Utils
{
    public static class Kernel32Api
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint desiredAccess, bool isInheritHandle, uint processId);

        /// <summary>
        /// 从指定内存中读取字节集数据
        /// </summary>
        /// <param name="handle">进程句柄</param>
        /// <param name="address">内存地址</param>
        /// <param name="data">数据存储变量</param>
        /// <param name="size">长度</param>
        /// <param name="read">读取长度</param>
        [DllImport("Kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr handle, IntPtr address, out byte[] data, int size, out int read);

        /// <summary>
        /// 此函数能写入某一进程的内存区域
        /// </summary>
        /// <param name="handle">进程句柄</param>
        /// <param name="address">要写的内存首地址</param>
        /// <param name="data">指向要写的数据的指针</param>
        /// <param name="size">要写入的字节数</param>
        /// <param name="write">读取长度</param>
        [DllImport("Kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr handle, IntPtr address, IntPtr data, int size, out int write);

        /// <summary>
        /// VirtualAllocEx 函数的作用是在指定进程的虚拟空间保留或提交内存区域，除非指定MEM_RESET参数，否则将该内存区域置0。
        /// </summary>
        /// <param name="hProcess">申请内存所在的进程句柄。</param>
        /// <param name="address">保留页面的内存地址；一般用NULL自动分配 </param>
        /// <param name="size">欲分配的内存大小，字节单位；注意实际分 配的内存大小是页内存大小的整数倍</param>
        /// <param name="allocationType">
        /// MEM_COMMIT：为特定的页面区域分配内存中或磁盘的页面文件中的物理存储
        /// MEM_PHYSICAL ：分配物理内存（仅用于地址窗口扩展内存）
        /// MEM_RESERVE：保留进程的虚拟地址空间，而不分配任何物理存储。保留页面可通过继续调用VirtualAlloc（）而被占用
        /// MEM_RESET ：指明在内存中由参数lpAddress和dwSize指定的数据无效
        /// MEM_TOP_DOWN：在尽可能高的地址上分配内存（Windows 98忽略此标志）
        /// MEM_WRITE_WATCH：必须与MEM_RESERVE一起指定，使系统跟踪那些被写入分配区域的页面（仅针对Windows 98）
        /// </param>
        /// <param name="protect">
        /// PAGE_READONLY： 该区域为只读。如果应用程序试图访问区域中的页的时候，将会被拒绝访
        /// PAGE_READWRITE 区域可被应用程序读写
        /// PAGE_EXECUTE： 区域包含可被系统执行的代码。试图读写该区域的操作将被拒绝。
        /// PAGE_EXECUTE_READ ：区域包含可执行代码，应用程序可以读该区域。
        /// PAGE_EXECUTE_READWRITE： 区域包含可执行代码，应用程序可以读写该区域。
        /// PAGE_GUARD： 区域第一次被访问时进入一个STATUS_GUARD_PAGE异常，这个标志要和其他保护标志合并使用，表明区域被第一次访问的权限
        /// PAGE_NOACCESS： 任何访问该区域的操作将被拒绝
        /// PAGE_NOCACHE： RAM中的页映射到该区域时将不会被微处理器缓存（cached)
        /// 注:
        /// PAGE_GUARD和PAGE_NOCHACHE标志可以和其他标志合并使用以进一步指定页的特征。
        /// PAGE_GUARD标志指定了一个防护页（guard page），即当一个页被提交时会因第一次被访问而产生一个one-shot异常，
        /// 接着取得指定的访问权限。PAGE_NOCACHE防止当它映射到虚拟页的时候被微处理器缓存。这个标志方便设备驱动使用
        /// 直接内存访问方式（DMA）来共享内存块。</param>
        /// <returns>执行成功就返回分配内存的首地址，不成功就是NULL。</returns>
        [DllImport("Kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr address, uint size, uint allocationType,
                                                   uint protect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr process, IntPtr address, uint size, uint freeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
    }
}
