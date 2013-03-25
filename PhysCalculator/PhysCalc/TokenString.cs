﻿using System;
using System.Linq;

namespace TokenParser
{
    public static class TokenString
    {
        #region Static parse methodes

        public static Boolean ParseChar(Char ch, ref String CommandLine, ref string ResultLine)
        {
            Boolean IsExpectedChar = (!String.IsNullOrEmpty(CommandLine)) && CommandLine[0] == ch;

            if (IsExpectedChar)
            {
                CommandLine = CommandLine.Substring(1);
            }
            else
            {
                ResultLine = ResultLine + " Char '" + ch + "' expected";
            }
            return IsExpectedChar;
        }

        public static Boolean TryParseChar(Char ch, ref String CommandLine)
        {
            Boolean IsExpectedChar = (!String.IsNullOrEmpty(CommandLine)) && CommandLine[0] == ch;
            if (IsExpectedChar)
            {
                CommandLine = CommandLine.Substring(1);
            }
            return IsExpectedChar;
        }

        public static Boolean ParseToken(String Token, ref String CommandLine, ref string ResultLine)
        {
            Boolean IsExpectedToken = CommandLine.StartsWithKeyword(Token);
            if (IsExpectedToken)
            {
                CommandLine = CommandLine.SkipToken(Token);
            }
            else
            {
                ResultLine = ResultLine + " Token '" + Token + "' expected";
            }
            return IsExpectedToken;
        }

        public static Boolean TryParseToken(String Token, ref String CommandLine)
        {
            Boolean IsExpectedToken = CommandLine.StartsWithKeyword(Token);

            if (IsExpectedToken)
            {
                CommandLine = CommandLine.SkipToken(Token);
            }
            return IsExpectedToken;
        }

        #endregion Static parse methodes

        #region Class extension parse methodes

        public static Boolean IsHexDigit(this Char c)
        {
            //return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
            return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public static Boolean IsKeyword(this String CommandLine, String Keyword)
        {
            return CommandLine.Equals(Keyword, StringComparison.OrdinalIgnoreCase);
        }

        public static Boolean StartsWithKeyword(this String CommandLine, String Keyword)
        {
            return CommandLine.StartsWith(Keyword, StringComparison.OrdinalIgnoreCase);
        }

        public static String SkipToken(this String CommandLine, String Token)
        {
            return CommandLine.Substring(Token.Length).TrimStart();
        }

        public static String ReadToken(this String CommandLine, out String Token)
        {
            int i = PeekToken(CommandLine, out Token);
            //return CommandLine.Substring(i+1).TrimStart();
            return CommandLine.Substring(i).TrimStart();
        }

        public static int PeekToken(this String CommandLine, out String Token)
        {
            int i = CommandLine.IndexOf(" ");
            if (i < 0)
            {
                i = CommandLine.Length;
            }
            Token = CommandLine.Substring(0, i);
            return i;
        }

        public static Boolean IsIdentifier(this String CommandLine)
        {
            return (!String.IsNullOrEmpty(CommandLine)) && (Char.IsLetter(CommandLine[0]) || Char.Equals(CommandLine[0], '_'));
        }

        public static String ReadIdentifier(this String CommandLine, out String Identifier)
        {
            if (IsIdentifier(CommandLine))
            {
                int i = CommandLine.TakeWhile(c => Char.IsLetterOrDigit(c) || Char.Equals(c, '_')).Count();
                Identifier = CommandLine.Substring(0, i);

                return CommandLine.Substring(i).TrimStart();
            }
            else
            {
                Identifier = null;
                return CommandLine;
            }
        }

        #endregion Class extension parse methodes

    }
}
