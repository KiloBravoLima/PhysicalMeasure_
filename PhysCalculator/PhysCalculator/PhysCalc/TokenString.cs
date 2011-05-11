using System;
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
            int i = CommandLine.IndexOf(" ");
            if (i < 0)
            {
                i = CommandLine.Length;
            }
            Token = CommandLine.Substring(0, i);
            return CommandLine.Substring(i+1).TrimStart();
        }

        public static String ReadIdentifier(this String CommandLine, out String Identifier)
        {
            //int i = CommandLine.IndexOfAny([' ', '*', '+'] ");
            if ((!String.IsNullOrEmpty(CommandLine)) && Char.IsLetter(CommandLine[0]))
            {
                int i = CommandLine.TakeWhile(c => Char.IsLetterOrDigit(c)).Count();
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

    static class ExtensionList
    {
        /*
        public static T LastOne<T>(this List<T> Me)
        {
            return Me.Count > 0 ? Me[Me.Count - 1] : null; 
        }

        public static void Push<T>(this List<T> Me, T element)
        {
            Me.Add(element);
        }

        public static T Pop<T>(this List<T> Me)
        {
            T element = Me.LastOne();
            if (element != null)
            {
                Me.RemoveAt(Me.Count - 1);
            }
            return element;
        }
        */
    }

}
