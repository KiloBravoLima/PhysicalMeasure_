// Copyright Alex Shvedov
//             Modified by MercuryP with color specifications
// 2014-09-23  Modified by KiloBravo  Placed in class 
// Use this code in any way you want

using System;
using System.Diagnostics;                // for Debug
using System.Drawing;                    // for Color (add reference to  System.Drawing.assembly)
using System.Runtime.InteropServices;    // for StructLayout

namespace ConsolAnyColor
{
    static class ConsolNativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COLORREF
        {
            internal uint ColorDWORD;

            internal COLORREF(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            internal COLORREF(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }

            internal Color GetColor() => Color.FromArgb((int)(0x000000FFU & ColorDWORD), (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);

            internal void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal COLORREF[] colors;

            /* 
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;    
            */
        }


        internal const int STD_OUTPUT_HANDLE = -11;                               // per WinBase.h
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(Int32 nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        // private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);
        internal static extern Boolean GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        // private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);
        internal static extern Boolean SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe);
    }

    static class ConsoleAnsiColors
    {
        /**
        // Console.Write("\x1b[31mThis is red via ANSI\x1b[0m\n");
        // const string ansi = $"\x1b[38;2;{r};{g};{b}m";
        public const string ForegroundDarkGreen = "\x1b[38;2;0;100;0m";
        public const string ForegroundBlue = "\x1b[38;2;0;0;255m";
        public const string ForegroundRed = "\x1b[31m";
        public const string ForegroundOrange = "\x1b[38;2;255;100;0m";
        public const string ForegroundYellow = "\x1b[38;2;255;255;0m";
        public const string ForegroundDarkGreen = "\x1b[38;2;0;100;0m";
        public const string ForegroundWhite = "\x1b[38;2;255;255;255m";
        **/

        public const string ColorsReset = "\x1b[0m";

        // Dark Foreground colors: \u001b[{30+ i}m i= 0..7
        public const string ForegroundBlack = "\x1b[30m";
        public const string ForegroundDarkBlue = "\x1b[34m";
        public const string ForegroundDarkGreen = "\x1b[32m";
        public const string ForegroundDarkCyan = "\x1b[36m";
        public const string ForegroundDarkRed = "\x1b[31m";
        public const string ForegroundDarkMagenta = "\x1b[35m";
        public const string ForegroundDarkYellow = "\x1b[33m";
        public const string ForegroundGray = "\x1b[37m";

        // Light Foreground colors: \u001b[{90+ i}m i= 0..7
        public const string ForegroundDarkGray = "\x1b[90m";
        public const string ForegroundBlue = "\x1b[94m";
        public const string ForegroundGreen = "\x1b[92m";
        public const string ForegroundCyan = "\x1b[96m";
        public const string ForegroundRed = "\x1b[91m";
        public const string ForegroundMagenta = "\x1b[95m";
        public const string ForegroundYellow = "\x1b[93m";
        public const string ForegroundWhite = "\x1b[97m";

        // Dark Background colors: \u001b[{40+ i}m
        public const string BackgroundBlack = "\x1b[40m";
        public const string BackgroundDarkBlue = "\x1b[44m";
        public const string BackgroundDarkGreen = "\x1b[42m";
        public const string BackgroundDarkCyan = "\x1b[46m";
        public const string BackgroundDarkRed = "\x1b[41m";
        public const string BackgroundDarkMagenta = "\x1b[45m";
        public const string BackgroundDarkYellow = "\x1b[43m";
        public const string BackgroundGray = "\x1b[47m";

        // Light Background colors: \u001b[{100+ i}m
        public const string BackgroundDarkGray = "\x1b[100m";
        public const string BackgroundBlue = "\x1b[104m";
        public const string BackgroundGreen = "\x1b[102m";
        public const string BackgroundCyan = "\x1b[106";
        public const string BackgroundRed = "\x1b[101m";
        public const string BackgroundMagenta = "\x1b[105m";
        public const string BackgroundYellow = "\x1b[103m";
        public const string BackgroundWhite = "\x1b[107m";

    }

    class ConsolAnyColorClass
    {
        // using ConsolNativeMethods;

        // Set a specific console color to an RGB color
        // The default console colors used are gray (foreground) and black (background)
        public static int SetColor(ConsoleColor colorToSet, Color targetColor) => SetColor(colorToSet, new ConsolNativeMethods.COLORREF(targetColor));

        public static int SetColor(ConsoleColor colorToSet, uint r, uint g, uint b) => SetColor(colorToSet, new ConsolNativeMethods.COLORREF(r, g, b));

        public static int SetColor(ConsoleColor colorToSet, ConsolNativeMethods.COLORREF targetColor)
        {
            // CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            ConsolNativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe = new ConsolNativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY();
            csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60


            IntPtr hConsoleOutput = ConsolNativeMethods.GetStdHandle(ConsolNativeMethods.STD_OUTPUT_HANDLE);    // 7
            if (hConsoleOutput == ConsolNativeMethods.INVALID_HANDLE_VALUE)
            {
                return Marshal.GetLastWin32Error();
            }
            bool brc = ConsolNativeMethods.GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }

            csbe.colors[(int)colorToSet] = targetColor;

            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            brc = ConsolNativeMethods.SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }

        public static int SetScreenColors(Color foregroundColor, Color backgroundColor)
        {
            int irc;
            irc = SetColor(ConsoleColor.Gray, foregroundColor);
            if (irc != 0) return irc;
            irc = SetColor(ConsoleColor.Black, backgroundColor);
            if (irc != 0) return irc;

            return 0;
        }

    }
}