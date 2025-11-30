﻿using System;

using TokenParser;

namespace CommandParser
{

    public class CommandHandler
    {
        public CommandHandler()
        {
        }

        public CommandHandler(String[] args)
        {
        }

        public virtual (Boolean CommandHandled, String ResultLine) Command(ref String CommandLine)
        {
            Boolean CommandHandled = false;

            String ResultLine = "Unknown command";

            return (CommandHandled, ResultLine);
        }

        public delegate ( Boolean CommandHandled, string CommandLine)  CommandDelegate(ref string CommandLine);

        // static 
        public (Boolean CommandFound, String ResultLine) CheckForCommand(String CommandKeyword, CommandDelegate CmdHandler, ref String CommandLine)
        {
            Boolean IsThisCommand = TryParseToken(CommandKeyword, ref CommandLine);
            String ResultLine = "";
            if (IsThisCommand)
            {
                (Boolean CommandHandled, ResultLine) = CmdHandler(ref CommandLine);
            }

            return (IsThisCommand, ResultLine);
        }

        // static 
        public Boolean StartsWithKeyword(String Keyword, String CommandLine) => CommandLine.StartsWithKeyword(Keyword);

        // static 
        public Boolean TryParseKeyword(String Keyword, ref String CommandLine) => TokenString.TryParseKeyword(Keyword, ref CommandLine);


        // static 
        public String SkipToken(String Token, String CommandLine) => CommandLine.SkipToken(Token);

        // static 
        public Boolean ParseChar(Char ch, ref String CommandLine, ref string ResultLine) => TokenString.ParseChar(ch, ref CommandLine, ref ResultLine);

        // static 
        public Boolean TryParseChar(Char ch, ref String CommandLine) => TokenString.TryParseChar(ch, ref CommandLine);

        // static 
        public Boolean ParseToken(String Token, ref String CommandLine, ref string ResultLine) => TokenString.ParseToken(Token, ref CommandLine, ref ResultLine);

        // static 
        public Boolean TryParseToken(String Token, ref String CommandLine) => TokenString.TryParseToken(Token, ref CommandLine);

        public Boolean TryParseKeywordPrefix(String Token, ref String CommandLine) => TokenString.TryParseKeywordPrefix(Token, ref CommandLine);


        // static 
        public String ReadToken(String CommandLine, out String Token) => CommandLine.ReadToken(out Token);
    }
}
