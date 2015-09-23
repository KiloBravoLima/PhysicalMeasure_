/*   http://physicalmeasure.codeplex.com                          */

using System;
using System.Collections.Generic;
using System.Linq;


namespace Extensions
{
    public static class DoubleExtensions
    {
        public static int EpsilonCompareTo(this double thisValue, double otherValue)
        {   /* Limited precision handling */
            double RelativeDiff = (thisValue - otherValue) / thisValue;
            if (RelativeDiff < -1e-15)
            {
                return -1;
            }
            if (RelativeDiff > 1e-15)
            {
                return 1;
            }
            return 0;
        }
    }

    public static class IEnumerableExtensions
    {
        public static T FirstOrNull<T>(this IEnumerable<T> sequence) where T : class
        {
            //return values.DefaultIfEmpty(null).FirstOrDefault();

            foreach (T item in sequence)
                return item;
            return null;
        }

        public static T FirstOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : class
        {
            foreach (T item in sequence.Where(predicate))
                return item;
            return null;
        }


        public static T? FirstStructOrNull<T>(this IEnumerable<T> sequence) where T : struct
        {
            foreach (T item in sequence)
                return item;
            return null;
        }

        public static T? FirstStructOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : struct
        {
            foreach (T item in sequence.Where(predicate))
                return item;
            return null;
        }
    }


    public static class ArrayExtensions
    {
        public static T[] Concat<T>(T[] a1, T[] a2)
        {
            if (a1 != null && a2 != null)
            {
                return a1.Concat(a2).ToArray();
            }
            else
            if (a2 != null)
            {
                return a2;
            }
            return a1;
        }


        public static T FirstOrNull<T>(this T[] values) where T : class
        {
            foreach (T item in values)
                return item;
            return null;
        }

        public static T FirstOrNull<T>(this T[] values, Func<T, bool> predicate) where T : class
        {
            foreach (T item in values.Where(predicate))
                return item;
            return null;
        }



        public static T? FirstStructOrNull<T>(this T[] values) where T : struct
        {
            foreach (T item in values)
                return item;
            return null;
        }

        public static T? FirstStructOrNull<T>(this T[] values, Func<T, bool> predicate) where T : struct
        {
            foreach (T item in values.Where(predicate))
                return item;
            return null;
        }
    }
}

