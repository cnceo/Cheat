using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace GXService.Utils
{
    public class Util
    {
        public static List<T> DeserializeList<T>(byte[] objsData)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            ms.Write(objsData, 0, objsData.Length);
            ms.Seek(0, SeekOrigin.Begin);

            try
            {
                return bf.Deserialize(ms) as List<T>;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] Serialize<T>(T obj)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        public static object Deserialize(byte[] objsData)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            ms.Write(objsData, 0, objsData.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            try
            {
                return bf.Deserialize(ms);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public static class PermutationCombinationExtension
    {
        public static IEnumerable<IEnumerable<T>> Combination<T>(this IEnumerable<T> a, int choose)
        {
            var combination = a as IList<T> ?? a.ToList();
            var count = combination.Count();
            if (choose >= count)
            {
                yield return combination;
            }
            else if (choose <= 1)
            {
                foreach (var n in (from m in combination select m))
                {
                    yield return Enumerable.Repeat(n, 1);
                }
            }
            else
            {
                for (int i = 0; i + choose <= count; ++i)
                {
                    foreach (var m in combination.Skip(i + 1).Combination(choose - 1))
                    {
                        yield return combination.Skip(i).Take(1).Union(m);
                    }
                }
            }
        }

        public static IEnumerable<T> Combination<T>(this IEnumerable<T> a, int choose, Func<T, T, T> aggregate)
        {
            return (from e in a.Combination(choose)
                    select e.Aggregate(aggregate));
        }

        private static IEnumerable<IEnumerable<T>> FullPermutation<T>(this IEnumerable<T> a)
        {
            int count = a.Count();
            if (count <= 1)
            {
                yield return a;
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    var m = a.Skip(i).Take(1);
                    foreach (var n in a.Except(m).FullPermutation())
                    {
                        yield return m.Union(n);
                    }
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Permutation<T>(this IEnumerable<T> a, int choose)
        {
            if (choose >= a.Count()) return a.FullPermutation();
            return (from e in a.Combination(choose) select e.FullPermutation()).Aggregate((e1, e2) => e1.Union(e2));
        }

        public static IEnumerable<T> Permutation<T>(this IEnumerable<T> a, int choose, Func<T, T, T> aggregate)
        {
            return (from e in a.Permutation(choose)
                    select e.Aggregate(aggregate));
        }
    }
    
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

            var myInput = new[] { new Input { type = 0, mi = myMinput } };

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

        #region 数据结构
        [StructLayout(LayoutKind.Explicit)]
        protected struct Input
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
        protected struct MouseInput
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 Mousedata;
            public Int32 dwFlag;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct tagKEYBDINPUT
        {
            Int16 wVk;
            Int16 wScan;
            Int32 dwFlags;
            Int32 time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct tagHARDWAREINPUT
        {
            Int32 uMsg;
            Int16 wParamL;
            Int16 wParamH;
        }
        #endregion
    }
}
