using System;
using System.Linq;

using TokenParser;

namespace PhysCalc
{

    class Commandhandler
    {
        public Commandhandler()
        {
        }

        public Commandhandler(String[] args)
        {
        }

        public virtual Boolean Command(String CommandLine, out String ResultLine)
        {
            Boolean CommandHandled = false;

            ResultLine = "Unknown command";

            return CommandHandled;
        }

        public delegate Boolean CommandDelegate(ref string CommandLine, ref string ResultLine);

        public Boolean CheckForCommand(String CommandKeyword, CommandDelegate CmdHandler, String CommandLine, ref String ResultLine, ref Boolean CommandHandled)
        {
            Boolean IsThisCommand = TryParseToken(CommandKeyword, ref CommandLine);

            if (IsThisCommand)
            {
                ResultLine = "";
                CommandHandled = CmdHandler(ref CommandLine, ref ResultLine);
            }

            return IsThisCommand;
        }

        public Boolean StartsWithKeyword(String Keyword, String CommandLine)
        {
            return CommandLine.StartsWithKeyword(Keyword);
        }

        public String SkipToken(String Token, String CommandLine)
        {
            return CommandLine.SkipToken(Token);
        }

        public Boolean ParseChar(Char ch, ref String CommandLine, ref string ResultLine)
        {
            return TokenString.ParseChar(ch, ref CommandLine, ref ResultLine);
        }

        public Boolean TryParseChar(Char ch, ref String CommandLine)
        {
            return TokenString.TryParseChar(ch, ref CommandLine);
        }

        public Boolean ParseToken(String Token, ref String CommandLine, ref string ResultLine)
        {
            return TokenString.ParseToken(Token, ref CommandLine, ref ResultLine);
        }

        public Boolean TryParseToken(String Token, ref String CommandLine)
        {
            return TokenString.TryParseToken(Token, ref CommandLine);
        }

        public String ReadToken(String CommandLine, out String Token)
        {
            return CommandLine.ReadToken(out Token);
        }
    }

    /**
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
            return CommandLine.Substring(Token.Count()).TrimStart();
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
     **/ 
}
