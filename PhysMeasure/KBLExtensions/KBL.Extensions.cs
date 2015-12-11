/*   http://physicalmeasure.codeplex.com                          */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace KBL.Extensions
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


    public static class StringExtensions
    {
        public static List<String> Chop(this String text, int maxLength = 80)
        {
            List<String> textParts = new List<String>();
            int textLen = text.Length;
            int choppedLen = 0;

            while (choppedLen < textLen)
            {
                int len = textLen - choppedLen; 
                if (len > maxLength) 
                {
                    len = maxLength; // Make extra line shift

                    // Wrap whole word to next line; find start of word to wrap
                    while (len > 0 && text[choppedLen + len - 1] != ' ' && text[choppedLen + len] != ' ')
                    {
                        len--;
                    }

                    if (len == 0)
                    {   // One large word on this line; Can't wrap whole word to next line. Must output something to this line. 
                        len = maxLength;
                    }
                }
                String strtext;
                try
                {
                    // strtext =  "| " + choppedLen.ToString() + " " + len.ToString() + " |"   + text.Substring(choppedLen, len);
                    strtext = text.Substring(choppedLen, len);
                }
                catch
                {
                    strtext = "| Exception: " + choppedLen.ToString() + " " + len.ToString() + " |";
                }

                textParts.Add(strtext);
                choppedLen += len;
            }

            return textParts;
        }
    }


    public static class DateTimeSortString
    {
        public static string ToSortString(this DateTime Me) => Me.ToString("yyyy-MM-dd HH:mm:ss");

        public static string ToSortShortDateString(this DateTime Me) => Me.ToString("yyyy-MM-dd");

        public static void ToBuildNo(this DateTime Me, out int buildNo, out int revisionNo)
        {
            TimeSpan TimeSince_2000_01_01 = Me.Date - new DateTime(2000, 1, 1);

            buildNo = TimeSince_2000_01_01.Days;
            revisionNo = (int)(Me.TimeOfDay.TotalSeconds / 2);
        }
    }


    public static class AssemblyExtensions
    {
        public static String AssemblyVersionInfo(this System.Reflection.Assembly assembly)
        {
            System.Reflection.AssemblyName AsmName = assembly.GetName();

            Version AsemVersion = AsmName.Version;
            String InfoStr;

            if (AsemVersion.Build != 0)
            {
                DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * AsemVersion.Build +             // days since 1 January 2000
                                                                                    TimeSpan.TicksPerSecond * 2 * AsemVersion.Revision));  // seconds since midnight, (multiply by 2 to get original)
                InfoStr = String.Format("{0} {1}", AsemVersion.ToString(), buildDateTime.ToSortString());
            }
            else
            {
                InfoStr = AsemVersion.ToString();
            }
            return InfoStr;
        }

        public static String AssemblyFileVersionInfo(this System.Reflection.Assembly assembly)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            //String AsemVersion = String.Format("{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
            String AsemVersion = fileVersionInfo.FileVersion;
            String InfoStr;

            if (fileVersionInfo.FileBuildPart != 0)
            {
                DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * fileVersionInfo.FileBuildPart +             // days since 1 January 2000
                                                                                    TimeSpan.TicksPerSecond * 2 * fileVersionInfo.FilePrivatePart));  // seconds since midnight, (multiply by 2 to get original)
                InfoStr = String.Format("{0} {1}", AsemVersion, buildDateTime.ToSortString());
            }
            else
            {
                InfoStr = AsemVersion;
            }
            return InfoStr;
        }

        public static String AssemblyInfo(this System.Reflection.Assembly assembly)
        {
            // System.Reflection.Assembly assembly = typeof(Quantity).Assembly;
            System.Reflection.AssemblyName AsmName = assembly.GetName();

            //FileInfo AsmFileInfo = new FileInfo(Asm.Location);
            // Version AsemVersion = AsmName.Version;
            String assemblyVersionInfo = AssemblyVersionInfo(assembly);
            String assemblyFileVersionInfo = AssemblyFileVersionInfo(assembly);

            String InfoStr = String.Format("{0,-16} {1} {2}", AsmName.Name, assemblyVersionInfo, assemblyFileVersionInfo);
            return InfoStr;
        }
    }
}

