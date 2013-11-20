using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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
}
