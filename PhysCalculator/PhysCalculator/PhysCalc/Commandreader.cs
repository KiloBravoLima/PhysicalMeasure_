using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PhysicalCalculator
{
    [Flags]
    public enum Tracelevel : byte
    {
        none = 0x00,
        results = 0x01,
        fileenterleave = 0x02,
        functionenterleave = 0x04,
        debug = 0x08,
        low = results,
        normal = results | fileenterleave,
        high = results | fileenterleave | functionenterleave,
        all = 0xFF
    }

    interface ICommandAccessor
    {
        String Name { get; }
        Tracelevel OutputTracelevel { get; set; }
        Boolean IsEmpty { get; }
        String GetCommandLine(ref String ResultLine);
    }

    class CommandAccessorStack : List<ICommandAccessor>
    {
        public Tracelevel OutputTracelevel
        {
            get
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    return CA.OutputTracelevel;
                }
                // Error; just trace all
                return Tracelevel.all;
            }

            set
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    CA.OutputTracelevel = value;
                }
            }
        }

        public String GetName()
        {
            ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
            String Name = null;
            if (CA != null)
            {
                Name = CA.Name;
            }
            return Name;
        }

        public String GetCommandLine(ref String ResultLine)
        {
            ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
            String CommandLine = null;
            if (CA != null)
            {
                CommandLine = CA.GetCommandLine(ref ResultLine);
                if (CA.IsEmpty)
                {
                    this.RemoveAt(this.Count - 1);
                }
            }

            return CommandLine;
        }
    }

    class CommandList : List<String>
    {
        public CommandList(String[] ListOfCommands)
        {
            foreach (String Command in ListOfCommands)
            {
                this.Add(Command);
            }
        }

        public CommandList(String Commands)
            : this(Commands.Split(new char[] { '\n', '\r' }))
        {
        }

        public String GetFirst()
        {
            String CommandLine = null;
            if (Count > 0)
            {
                CommandLine = this[0];
                this.RemoveAt(0);
            }
            return CommandLine;
        }
    }

    abstract class CommandAccessor : ICommandAccessor
    {
        private String _name;
        private Tracelevel OutTracelevel = Tracelevel.normal; // Tracelevel.all; //Tracelevel.normal;
        public int LinesRead = 0;

        public String Name { get { return (_name != null ? _name : ""); } set { _name = value;  } }
        public Tracelevel OutputTracelevel { get { return OutTracelevel; } set { OutTracelevel = value; } }

        //virtual public Boolean IsEmpty { get { return true; } } 
        abstract public Boolean IsEmpty { get; } 
        abstract public string GetCommandLine(ref String ResultLine);
    }

    class CommandBlockAccessor : CommandAccessor
    {
        public CommandList CommandBlock;

        public CommandBlockAccessor(CommandList CommandBlock)
            : this(null, CommandBlock)
        {
        }

        public CommandBlockAccessor(String CommandBlockName, CommandList CommandBlock)
        {
            this.Name = CommandBlockName;
            this.CommandBlock = CommandBlock;
        }

        public String CommandBlockName { get { return Name; } set { Name = value; } }

        override public Boolean IsEmpty
        {
            get
            {
                return CommandBlock == null;
            }
        }

        override public string GetCommandLine(ref String ResultLine)
        {
            string S = null;
            if (CommandBlock == null)
            {
                S = "";
                if (!String.IsNullOrWhiteSpace(Name))
                {
                    ResultLine = "CommandBlock '" + Name + "' is empty";
                }
                else 
                {
                    ResultLine = "CommandBlock is empty";
                }
                CommandBlockName = null;
            }
            else if (LinesRead == 0)
            {
                if (OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
                {
                    ResultLine = "Reading from '" + Name + "'";
                }
            }

            if (CommandBlock != null)
            {
                S = CommandBlock.GetFirst();
                if (S == null)
                {
                    S = "";
                    if (OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
                    {
                        if (!String.IsNullOrEmpty(ResultLine))
                        {
                            ResultLine += "\n";
                        }
                        //ResultLine += "End of CommandBlock '" + Name + "'";
                        ResultLine += "End of '" + Name + "'";
                    }

                    CommandBlock = null;
                    CommandBlockName = null;
                }
                else
                {
                    LinesRead++;
                }
            }

            return S;
        }
    }

    class CommandFileAccessor : CommandAccessor
    {
        //public String FileNameStr;
        public StreamReader FileReader;

        public CommandFileAccessor()
            : this(null)
        {
        }

        public CommandFileAccessor(String FileNameStr)
        {
            this.FileNameStr = FileNameStr;
            this.FileReader = null;
        }

        public String FileNameStr { 
            get { return Name; } 
            set { Name = value; } 
        }

        override public Boolean IsEmpty
        {
            get
            {
                // Will not work because FileReader is null also before the file is read: return FileReader == null;
                return String.IsNullOrWhiteSpace(FileNameStr);
            }
        }

        override public string GetCommandLine(ref String ResultLine)
        {
            string S = null;
            if (FileReader == null && !String.IsNullOrWhiteSpace(FileNameStr))
            {
                if (!File.Exists(FileNameStr)
                    && !Path.HasExtension(FileNameStr))
                {
                    FileNameStr += ".cal";
                }
                try
                {
                    //FileReader = File.OpenText(FileNameStr);
                    //Encoding FileEncoding = Encoding.UTF8;
                    //Encoding FileEncoding = Encoding.ASCII;
                    //Encoding FileEncoding = Encoding.UTF7;
                    FileReader = new StreamReader(FileNameStr, Encoding.UTF7);
                    if (OutputTracelevel.HasFlag(Tracelevel.fileenterleave))
                    {
                        ResultLine = "Reading from file " + FileNameStr;
                    }

                }
                catch (FileNotFoundException e)
                {
                    S = "";
                    ResultLine = "File '" + e.FileName + "' not found";
                    FileNameStr = null;
                }
            }

            if (FileReader != null)
            {
                S = FileReader.ReadLine();
                if (S == null)
                {
                    S = "";
                    if (OutputTracelevel.HasFlag(Tracelevel.fileenterleave))
                    {
                        ResultLine = "End of File '" + FileNameStr + "'";
                    }

                    FileReader.Close();
                    FileReader = null;
                    FileNameStr = null;
                }
                else
                {
                    LinesRead++;
                }
            }

            return S;
        }
    }

    class ResultWriter
    {
        public String FileNameStr = null;
        public StreamWriter FileStreamWriter = null;
        public int LinesWritten = 0;

        public ResultWriter()
            : this(null)
        {
        }

        public ResultWriter(String FileNameStr)
        {
            this.FileNameStr = FileNameStr;
            this.FileStreamWriter = null;
        }

        public void CheckFile(ref string ResultText)
        {
            if (FileStreamWriter == null)
            {
                if (!String.IsNullOrWhiteSpace(FileNameStr))
                {
                    if (!Path.HasExtension(FileNameStr))
                    {
                        FileNameStr += ".cres";
                    }
                    try
                    {
                        FileStreamWriter = File.CreateText(FileNameStr);
                    }
                    catch (FileNotFoundException e)
                    {
                        ResultText = "File '" + e.FileName + "' not found";
                        FileNameStr = null;
                    }
                }
            }
        }

        public void WriteLine(String ResultLine)
        {
            this.CheckFile(ref ResultLine);

            if (FileStreamWriter != null)
            {
                FileStreamWriter.WriteLine(ResultLine);
            }
            else
            {
                Console.WriteLine(ResultLine);
            }
            LinesWritten++;
        }

        public void Write(String ResultText)
        {
            this.CheckFile(ref ResultText);

            if (FileStreamWriter != null)
            {
                FileStreamWriter.Write(ResultText);
            }
            else
            {
                Console.Write(ResultText);
            }
        }

        public void Close()
        {
            if (FileStreamWriter != null) {
                FileStreamWriter.Flush();
                FileStreamWriter.Close();
                FileNameStr = null;
            }
        }
    }

    class Commandreader
    {
        private CommandAccessorStack CommandAccessors = null;
        public ResultWriter ResultLineWriter = null;
        public Boolean ReadFromConsoleWhenEmpty = false;
        private Tracelevel GlobalOutputTracelevel = Tracelevel.normal; //Tracelevel.all; // Tracelevel.normal;

        public Commandreader(ResultWriter ResultLineWriter = null)
        {
            this.ResultLineWriter = ResultLineWriter;
        }

        public Commandreader(String[] args, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                AddFile(args[0]);
            }
            else if (args.Length >= 1)
            {
                AddCommandList("Command list", args);
            }
        }

        public Commandreader(String CommandLine, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (File.Exists(CommandLine))
            {
                AddFile(CommandLine);
            }
            else
            {
                AddCommandBlock("Command block", CommandLine);
            }
        }

        public Commandreader(String CommandName, String[] args, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                AddFile(args[0]);
            }
            else if (args.Length >= 1)
            {
                AddCommandList(CommandName, args);
            }
        }

        public Commandreader(String CommandName, String CommandLine, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (File.Exists(CommandLine))
            {
                AddFile(CommandLine);
            }
            else
            {
                AddCommandBlock(CommandName, CommandLine);
            }
        }

        public Tracelevel OutputTracelevel { 
            get {
                if (CommandAccessors != null)
                {
                    return CommandAccessors.OutputTracelevel;
                }
                else 
                {
                    return GlobalOutputTracelevel;
                }
            } 

            set {
                if (CommandAccessors != null)
                {
                    CommandAccessors.OutputTracelevel = value;
                }
                else
                {
                    GlobalOutputTracelevel = value;
                }
            } 
        }

        public void Write(string Line)
        {   // Echo Line to output
            if (ResultLineWriter != null) 
            {   // Echo Line to file output
                ResultLineWriter.Write(Line);
            }
            else
            {   // Echo Line to console output
                Console.Write(Line);
            }
        }

        public void WriteLine(string Line)
        {   // Echo Line to output
            if (ResultLineWriter != null)
            {   // Echo Line to file output
                ResultLineWriter.WriteLine(Line);
            }
            else
            {   // Echo Line to console output
                Console.WriteLine(Line);
            }
        }

        public void AddCommandList(String CommandListName, String[] ListOfCommands)
        {
            CommandList CL = new CommandList(ListOfCommands);

            if (CommandAccessors == null)
            {
                CommandAccessors = new CommandAccessorStack();
            }

            CommandAccessors.Add(new CommandBlockAccessor(CommandListName, CL));
        }

        public void AddCommandBlock(String CommandBlockName, String CommandBlock)
        {
            if (!String.IsNullOrWhiteSpace(CommandBlock))
            {
                if (CommandAccessors == null)
                {
                    CommandAccessors = new CommandAccessorStack();
                }

                CommandList CL = new CommandList(CommandBlock);

                CommandAccessors.Add(new CommandBlockAccessor(CommandBlockName, CL));
            }
        }
        
        public void AddFile(String filename)
        {
            if (CommandAccessors == null)
            {
                CommandAccessors = new CommandAccessorStack();
            }
            CommandAccessors.Add(new CommandFileAccessor(filename));
        }

        public Boolean HasAccessor()
        {
            return (CommandAccessors != null);
        }

        public String Accessor()
        {
            String AccessorName = null;
            if (CommandAccessors != null)
            {
                AccessorName = CommandAccessors.GetName();
            }
            return AccessorName;
        }

        public virtual Boolean ReadCommand(ref String ResultLine, out String CommandLine)
        {
            if (CommandAccessors != null)
            {
                CommandLine = CommandAccessors.GetCommandLine(ref ResultLine);
                if (CommandAccessors.Count <= 0)
                {
                    CommandAccessors = null;
                }

                if (!String.IsNullOrWhiteSpace(CommandLine))
                {   // Echo Command to output
                    if (!String.IsNullOrWhiteSpace(ResultLine))
                    {
                        WriteLine(ResultLine);
                        ResultLine = "";
                    }
                    Write("| ");
                    WriteLine(CommandLine);
                }
            }
            else if (ReadFromConsoleWhenEmpty)
            {
                // Show that we are ready to next command from user
                Write("|>");

                CommandLine = Console.ReadLine();
            }
            else
            {
                CommandLine = null;
            }

            Boolean CommandRead = CommandLine != null;
            return CommandRead;
        }
    }

}