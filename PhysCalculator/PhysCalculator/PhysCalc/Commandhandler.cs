﻿﻿using System;

using TokenParser;

namespace CommandParser
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


}