﻿/*   http://physicalmeasure.codeplex.com                          */

namespace System
{
    using System.Collections.Generic;

    public static class BitTools
    {
        public static Byte RotateLeft(this Byte value, Byte NoOfBits) => (Byte)(value << NoOfBits | value >> (8 - NoOfBits));

        public static Byte RotateRight(this Byte value, Byte NoOfBits) => (Byte)(value >> NoOfBits | value << (8 - NoOfBits));

        /*
        public static  Byte operator <<( Byte value, Byte NoOfBits)
        {
            return (Byte)(value << NoOfBits | value >> (8 - NoOfBits));
        }

        public static Byte operator >>( Byte value, Byte NoOfBits)
        {
            return (Byte)(value >> NoOfBits | value << (8 - NoOfBits));
        }
        */

        public static Int16 RotateLeft(this Int16 value, Byte NoOfBits) => (Int16)(value << NoOfBits | value >> (16 - NoOfBits));

        public static Int16 RotateRight(this Int16 value, Byte NoOfBits) => (Int16)(value >> NoOfBits | value << (16 - NoOfBits));

        /*
        public static  Byte operator <<( Byte value, Byte NoOfBits)
        {
            return (Byte)(value << NoOfBits | value >> (8 - NoOfBits));
        }

        public static Byte operator >>( Byte value, Byte NoOfBits)
        {
            return (Byte)(value >> NoOfBits | value << (8 - NoOfBits));
        }
        */

        public static UInt16 RotateLeft(this UInt16 value, Byte NoOfBits) => (UInt16)(value << NoOfBits | value >> (16 - NoOfBits));

        public static UInt16 RotateRight(this UInt16 value, Byte NoOfBits) => (UInt16)(value >> NoOfBits | value << (16 - NoOfBits));

        /*
        public static  Byte operator <<( Byte value, Byte NoOfBits)
        {
            return (Byte)(value << NoOfBits | value >> (8 - NoOfBits));
        }

        public static Byte operator >>( Byte value, Byte NoOfBits)
        {
            return (Byte)(value >> NoOfBits | value << (8 - NoOfBits));
        }
        */
    }

    public static class ByteExtensions
    {
        public static SByte ToSByte(this Byte thisValue)
        {
            SByte res;
            if (thisValue < 128)
                res = (SByte)thisValue;
            else
                res = (SByte)(thisValue - 256);
            return res;
        }

        public static Char ToChar(this Byte thisValue)
        {
            Char res = (Char)thisValue;
            return res;
        }
    }

    public static class SByteExtensions
    {
        public static Byte ToByte(this SByte thisValue)
        {
            Byte res;
            if (thisValue >= 0)
                res = (Byte)thisValue;
            else
                res = (Byte)(thisValue + 256);
            return res;
        }
    }

    public static class ByteArrayExtensions
    {
        public static SByte[] ToSBytes(this Byte[] thisValue)
        {
            SByte[] res = new SByte[thisValue.Length];
            for (int i = 0; i < thisValue.Length; i++)
            {
                // res[i] = (SByte)thisValue[i];
                res[i] = thisValue[i].ToSByte();
            }
            return res;
        }

        public static Char[] ToCharArray(this Byte[] thisValue)
        {
            Char[] res = new Char[thisValue.Length];
            for (int i = 0; i < thisValue.Length; i++)
            {
                // res[i] = (Char)thisValue[i];
                res[i] = thisValue[i].ToChar();
            }
            return res;
        }

        public static Byte[] MaskAnd(this Byte[] thisValue, Byte[] otherArray)
        {
            int MinLen = Math.Min(thisValue.Length, otherArray.Length);
            int MaxLen = Math.Max(thisValue.Length, otherArray.Length);
            Byte[] res = new Byte[MaxLen];
            for (int i = 0; i < MinLen; i++)
            {
                res[i] = (Byte)(thisValue[i] & otherArray[i]);
            }
            for (int i = MinLen; i < thisValue.Length; i++)
            {
                res[i] = thisValue[i];
            }
            for (int i = MinLen; i < otherArray.Length; i++)
            {
                res[i] = otherArray[i];
            }
            return res;
        }

        public static Byte[] MaskXor(this Byte[] thisValue, Byte[] otherArray)
        {
            int MinLen = Math.Min(thisValue.Length, otherArray.Length);
            int MaxLen = Math.Max(thisValue.Length, otherArray.Length);
            Byte[] res = new Byte[MaxLen];
            for (int i = 0; i < MinLen; i++)
            {
                res[i] = (Byte)(thisValue[i] ^ otherArray[i]);
            }
            for (int i = MinLen; i < MaxLen; i++)
            {
                res[i] = 0xFF;
            }
            return res;
        }

        public static Byte[] MaskNot(this Byte[] thisValue)
        {
            Byte[] res = new Byte[thisValue.Length];
            for (int i = 0; i < thisValue.Length; i++)
            {
                res[i] = (Byte)(~thisValue[i] & 0xFF);
            }
            return res;
        }
    }

    public static class SByteArrayExtensions
    {
        public static Byte[] ToBytes(this SByte[] thisValue)
        {
            Byte[] res = new Byte[thisValue.Length];
            for (int i = 0; i < thisValue.Length; i++)
            {
                // res[i] = (Byte)thisValue[i];
                res[i] = thisValue[i].ToByte();
            }
            return res;
        }
    }

#if NETCORE
    public static class Math
    {
        // return Math.DivRem(a, b, out result);
        public static int DivRem(int a, int b, out int result)
        {
            result = a % b;
            return a / b;
        }
    }
#endif

    public static class DoubleExtensions
    {
        public static Int32 EpsilonCompareTo(this Double thisValue, Double otherValue)
        {   /* Limited precision handling */
            double RelativeDiff = (thisValue - otherValue) / thisValue;
            if (RelativeDiff < -1e-14)
            {
                return -1;
            }
            if (RelativeDiff > 1e-14)
            {
                return 1;
            }
            return 0;
        }

        public static Int32 EpsilonCompareTo(this Double thisValue, Double otherValue, Double relativeDiffPrecision)
        {   /* Limited precision handling */
            double RelativeDiff = (thisValue - otherValue) / thisValue;
            if (RelativeDiff < -relativeDiffPrecision)
            {
                return -1;
            }
            if (RelativeDiff > relativeDiffPrecision)
            {
                return 1;
            }
            return 0;
        }
    }

    public static class StringExtensions
    {

        public static String Tail(this String text, int Length = 1)
        {
            int FullLen = text.Length;
            return text.Substring(FullLen - Length, Length);
        }

        public static String Head(this String text, int Length = 1)
        {
            return text.Substring(0, Length);
        }


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

        public static String AppendSeparated(this String s, string text, string separator = " ")
        {
            if (s.Length > 0)
            {
                return s + separator + text;
            }

            return text;
        }
    }

    public static class PathExtensions
    {
        public static string ToValidFilePath(this String path)
        {

            return path.Replace(':', '_').Replace(" ", "__");
        }
    }

    public static class DateTimeExtensions
    {
        public static String ToSortString(this DateTime Me) => Me.ToString("yyyy-MM-dd HH:mm:ss");

        public static String ToSortShortDateString(this DateTime Me) => Me.ToString("yyyy-MM-dd");

        public static String ToSortString(this DateTimeOffset Me) => Me.ToString("yyyy-MM-dd HH:mm:ss");

        public static String ToSortShortDateString(this DateTimeOffset Me) => Me.ToString("yyyy-MM-dd");
    }
}

namespace System.Text
{

    public static class StringBuilderExtensions
    {
        public static void AppendSeparated(this StringBuilder sb, String text, String separator = " ")
        {
            if (text.Length > 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(text);
            }
        }
    }
}

namespace System.Collections.Generic
{
    using System.Linq;

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

        public static String ToStringList<Object>(this IEnumerable<Object> sequence, String separator = ", ")
        {
            return sequence.Select(item => item.ToString()).Aggregate((current, next) => current + separator + next);
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
}

namespace System.Reflection
{
    public static class DateTimeBuildNo
    {
        public static void ToBuildNo(this DateTime Me, out int buildNo, out int revisionNo)
        {
            TimeSpan TimeSince_2000_01_01 = Me.Date - new DateTime(2000, 1, 1);

            buildNo = TimeSince_2000_01_01.Days;
            revisionNo = (int)(Me.TimeOfDay.TotalSeconds / 2);
        }

        public static DateTime FromBuildNo(int buildNo, int revisionNo)
        {
            DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * buildNo +             // days since 1 January 2000
                                                                               TimeSpan.TicksPerSecond * 2 * revisionNo));  // seconds since midnight, (multiply by 2 to get original)
            return buildDateTime;
        }

        public static String ToBuildDateString(int buildNo, int revisionNo)
        {
            DateTime buildDateTime = FromBuildNo(buildNo, revisionNo);
            String buildNoString = buildDateTime.ToSortString();
            return buildNoString;
        }
    }

    public static class AssemblyExtensions
    {
        public static String VersionInfoStr(String AsemVersion, Int32 FileBuild, Int32 FileRevision)
        {
            if (FileBuild == 0)
            {
                return AsemVersion;
            }

            String buildNoString = DateTimeBuildNo.ToBuildDateString(FileBuild, FileRevision);
            return $"{AsemVersion} {buildNoString}";
        }

        public static String AssemblyVersionInfo(this System.Reflection.Assembly assembly)
        {
            System.Reflection.AssemblyName AsmName = assembly.GetName();

            Version AsemVersion = AsmName.Version;
            return VersionInfoStr(AsemVersion.ToString(), AsemVersion.Build, AsemVersion.Revision);
        }

        public static String AssemblyFileVersionInfo(this System.Reflection.Assembly assembly)
        {
            String InfoStr;

#if !NETCORE && !NETSTANDARD1_0
            // For Windows Desktop
            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            String AsemVersion = fileVersionInfo.FileVersion;

            InfoStr = VersionInfoStr(AsemVersion, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
#else
            // For NETCORE, NETSTANDARD1_0
            // AssemblyVersionAttribute ava =  assembly.GetCustomAttribute<AssemblyVersionAttribute>();
            AssemblyFileVersionAttribute afva = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            if (afva != null)
            {
                String AsemVersion = afva.Version;
                String[] parts = AsemVersion.Split('.');

                Int32 FileBuildPart = 0;
                Int32 FilePrivatePart = 0;

                if (parts.Length > 2)
                {
                    Int32.TryParse(parts[2], out FileBuildPart);
                }
                if (parts.Length > 3)
                {
                    Int32.TryParse(parts[3], out FilePrivatePart);
                }
                InfoStr = VersionInfoStr(AsemVersion, FileBuildPart, FilePrivatePart);
            }
            else
            {
                InfoStr = "null";
            }
#endif // UseWindowsDesktop
            return InfoStr;
        }

        public static String AssemblyInfo(this System.Reflection.Assembly assembly)
        {
            System.Reflection.AssemblyName AsmName = assembly.GetName();

            String assemblyVersionInfo = AssemblyVersionInfo(assembly);
            String assemblyFileVersionInfo = AssemblyFileVersionInfo(assembly);

            String InfoStr = String.Format("{0,-16} {1} {2}", AsmName.Name, assemblyVersionInfo, assemblyFileVersionInfo);
            return InfoStr;
        }
    }
}

