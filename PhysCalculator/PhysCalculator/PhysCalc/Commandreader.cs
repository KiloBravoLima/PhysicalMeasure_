using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PhysCalc
{

    class FileAccesser
    {
        public String FileNameStr;
        public StreamReader FileReader;
        public int LinesRead = 0;

        public FileAccesser()
            : this(null)
        {
        }

        public FileAccesser(String FileNameStr)
        {
            this.FileNameStr = FileNameStr;
            this.FileReader = null;
        }

        public string ReadFromFile(ref String ResultLine)
        {
            string S = null;
            if (FileReader == null)
            {
                if (!File.Exists(FileNameStr)
                    && !Path.HasExtension(FileNameStr))
                {
                    FileNameStr += ".cal";
                }
                try
                {
                    FileReader = File.OpenText(FileNameStr);
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
                    ResultLine = "End of File '" + FileNameStr + "'";

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

    class FileAccesserList : List<FileAccesser>
    {
        public string ReadFromFile(ref String ResultLine)
        {
            FileAccesser FA = Count > 0 ? this[Count - 1] : null;
            String CommandLine = null;
            if (FA != null)
            {
                CommandLine = FA.ReadFromFile(ref ResultLine);
                if (FA.FileNameStr == null)
                {
                    this.RemoveAt(this.Count - 1);
                }
            }

            return CommandLine;
        }
    }

    class CommandList : List<String>
    {
        public string GetFirst()
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
        private CommandList Commands = null;
        private FileAccesserList FileAccesserStack = null;
        private ResultWriter ResultLineWriter = null;

        public Commandreader(ResultWriter ResultLineWriter = null)
        {
            this.ResultLineWriter = ResultLineWriter;
        }

        public Commandreader(String[] args, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            foreach (String Command in args)
            {
                if (Commands == null)
                {
                    Commands = new CommandList();
                }
                Commands.Add(Command);
            }
        }

        String FileName { get { return GetFile(); } set { SetFile(value); } }

        public String GetFile()
        {
            String filename = null;

            if (FileAccesserStack != null)
            {
                FileAccesser FA = FileAccesserStack.Count > 0 ? FileAccesserStack[FileAccesserStack.Count - 1] : null;
                if (FA != null)
                {
                    filename = FA.FileNameStr;
                }
            }
            return filename;
        }

        public void SetFile(String filename)
        {
            if (FileAccesserStack == null)
            {
                FileAccesserStack = new FileAccesserList();
            }
            FileAccesserStack.Add(new FileAccesser(filename));
        }

        public Boolean HasFile()
        {
            return (FileAccesserStack != null);
        }

        public string ReadFromFile(ref String ResultLine)
        {
            string S = null;
            if (FileAccesserStack != null)
            {
                S = FileAccesserStack.ReadFromFile(ref ResultLine);
            }
            return S;
        }

        public virtual Boolean ReadCommand(ref String ResultLine, out String CommandLine)
        {
            Boolean CommandRead = false;
            Boolean CommandFromConsol = false;
            if (Commands != null)
            {
                CommandLine = Commands.GetFirst();
                if (Commands.Count <= 0)
                {
                    Commands = null;
                }
            }
            else if (FileAccesserStack != null)
            {
                CommandLine = FileAccesserStack.ReadFromFile(ref ResultLine);
                if (FileAccesserStack.Count <= 0)
                {
                    FileAccesserStack = null;
                }
            }
            else
            {
                CommandLine = Console.ReadLine();
                CommandFromConsol = true;
            }

            CommandRead = CommandLine != null;

            if (CommandRead && !CommandFromConsol)
            {
                if (!String.IsNullOrWhiteSpace(CommandLine))
                {   // Echo Command to output
                    if (ResultLineWriter != null) 
                    {   // Echo Command to file output
                        ResultLineWriter.WriteLine(CommandLine);
                    }
                    else
                    {   // Echo Command to console output
                        Console.WriteLine(CommandLine);
                    }
                }
            }
            return CommandRead;
        }
    }

}