using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RewriteMe.Business.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Func<Task>> WhenTaskDone(this IEnumerable<Func<Task>> list, Action action)
        {
            return list.Select<Func<Task>, Func<Task>>(x => async () =>
            {
                await x().ConfigureAwait(false);
                action();
            });
        }

        public static int GetCount(this IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            var num = 0;
            while (enumerator.MoveNext())
            {
                ++num;
            }

            return num;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
