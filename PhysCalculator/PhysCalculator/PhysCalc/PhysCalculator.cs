﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.IO;

using PhysicalMeasure;

using TokenParser;
using CommandParser;

using PhysicalCalculator.Identifers;
using PhysicalCalculator.Function;
using PhysicalCalculator.Expression;

namespace PhysicalCalculator
{

    class PhysCalculator : Commandhandler
    {
        Commandreader CommandLineReader = null;
        ResultWriter ResultLineWriter = null;

        const string AccumulatorName = "Accumulator";
        IPhysicalQuantity Accumulator = null;
        CalculatorEnviroment GlobalContext;
        public CalculatorEnviroment CurrentContext;

        public PhysCalculator()
        {
            GlobalContext = new CalculatorEnviroment();
            GlobalContext.Name = " Global namespace";
            CurrentContext = GlobalContext;

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new Commandreader("Calculator Prompt", this.ResultLineWriter);
        }

        public PhysCalculator(Commandreader CommandLineReader)
        {
            GlobalContext = new CalculatorEnviroment();
            CurrentContext = GlobalContext;

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = CommandLineReader;
        }

        public PhysCalculator(String[] PhysCalculatorConfig_args)
        {
            GlobalContext = new CalculatorEnviroment();
            CurrentContext = GlobalContext;

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new Commandreader("Calculator Prompt", PhysCalculatorConfig_args, this.ResultLineWriter);
        }

        public PhysCalculator(Commandreader CommandLineReader, String[] PhysCalculatorConfig_args)
        {
            GlobalContext = new CalculatorEnviroment();
            CurrentContext = GlobalContext;

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = CommandLineReader;
        }

        public PhysCalculator(Commandreader CommandLineReader, ResultWriter ResultLineWriter)
        {
            GlobalContext = new CalculatorEnviroment();
            CurrentContext = GlobalContext;

            this.ResultLineWriter = ResultLineWriter;
            this.CommandLineReader = CommandLineReader;
        }

        
        public void Run()
        {
            // Setup Lookup callback delegert static globals

            // Delegert static globals
            PhysicalExpression.IdentifierContextLookupCallback = IdentifierContextLookup;
            PhysicalExpression.QualifiedIdentifierContextLookupCallback = QualifiedIdentifierIEnviromentLookup;

            PhysicalExpression.VariableValueGetCallback = VariableGet;
            PhysicalExpression.FunctionLookupCallback = FunctionLookup;
            PhysicalExpression.FunctionEvaluateCallback = FunctionEvaluate;
            PhysicalExpression.FunctionEvaluateFileReadCallback = FunctionEvaluateFileRead;

            PhysicalFunction.ExecuteCommandsCallback = ExecuteCommandsCallback;

            CurrentContext.ParseState = CommandPaserState.executecommandline;

            ExecuteCommands(CurrentContext, CommandLineReader, ResultLineWriter);
        }

        public void ExecuteCommands(CalculatorEnviroment LocalContext, Commandreader CommandLineReader, ResultWriter ResultLineWriter)
        {
            Boolean CommandLineFromAccessor = false;

            Boolean CommandLineEmpty;
            Boolean ResultLineEmpty; 
            Boolean LoopExit;

            do
            {
                String CommandLine;
                String ResultLine;

                CommandLineEmpty = true;
                ResultLineEmpty = true;
                LoopExit = false;
                try
                {
                    ResultLine = "";

                    CommandLineFromAccessor = CommandLineReader.HasAccessor();
                    CommandLineReader.ReadCommand(ref ResultLine, out CommandLine);
                    CommandLineEmpty = String.IsNullOrWhiteSpace(CommandLine);
                    ResultLineEmpty = String.IsNullOrWhiteSpace(ResultLine);
                    if (!ResultLineEmpty)
                    {
                        ResultLineWriter.WriteLine(ResultLine);
                        ResultLine = "";
                        LoopExit = false;   // Show error 
                    }

                    if (!CommandLineEmpty)
                    {
                        CommandLine = CommandLine.Trim();
                        LoopExit = CommandLine.Equals("Exit", StringComparison.OrdinalIgnoreCase);
                        if (!LoopExit)
                        {
                            if (LocalContext.FunctionToParse != null)
                            {
                                LoopExit = !FunctionDecleration(CommandLine, out ResultLine);
                            }
                            else
                            {
                                LoopExit = !Command(CommandLine, out ResultLine);
                            }

                            if (!String.IsNullOrWhiteSpace(ResultLine))
                            {
                                ResultLineWriter.WriteLine(ResultLine);
                            }
                            else
                            {
                                Boolean ShowEmptyResultLine = (LocalContext.FunctionToParse == null)
                                                            && !CommandLineFromAccessor;

                                if (ShowEmptyResultLine)
                                {
                                    ResultLineWriter.WriteLine("?");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /**
                        Possible exceptions:
                            InvalidCastException("The 'obj' argument is not a IPhysicalUnit object.");
                            ArgumentException("object's physical unit " + temp.Unit.ToString() + " is not convertible to a " + ConvToUnitName);
                            ArgumentException("object is not a IPhysicalQuantity");
                            ArgumentException("Physical quantity is not a pure unit; but has a value = " + pq.Value.ToString());
                            InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object."); 
                            ArgumentException("object's physical unit " + pq2.Unit.ToString()+ " is not convertible to a " + pq1.Unit.ToString());
                      
                    **/
                    String Message = String.Format("{0} Exception Source: {1} - {2}", e.GetType().ToString(), e.Source, e.ToString());
                    ResultLineWriter.WriteLine(Message);
                    LoopExit = false;
                }
            } while ((CommandLineFromAccessor || !CommandLineEmpty || !ResultLineEmpty) && !LoopExit);
        }

        public Boolean ExecuteCommandsCallback(CalculatorEnviroment LocalContext, List<String> FuncBodyCommands, ref String FuncBodyResult, out IPhysicalQuantity FunctionResult)
        {
            // Dummy: Never used    FuncBodyResult
            Commandreader functionCommandLineReader = new Commandreader(LocalContext.Name, FuncBodyCommands.ToArray(), CommandLineReader.ResultLineWriter);
            functionCommandLineReader.ReadFromConsoleWhenEmpty = false; // Return from ExecuteCommands() function when FuncBodyCommands are done

            if (LocalContext.OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
            {
                ResultLineWriter.WriteLine("Enter " + LocalContext.Name);
            }
            ExecuteCommands(LocalContext, functionCommandLineReader, ResultLineWriter);
            FunctionResult = Accumulator;
            if (LocalContext.OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
            {
                ResultLineWriter.WriteLine("Leave " + LocalContext.Name);
            }
            return true;
        }

        public override Boolean Command(String CommandLine, out String ResultLine)
        {
            Boolean CommandHandled = false;
            ResultLine = "Unknown command";
            Boolean CommandFound =    CheckForCommand("//", CommandComment, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Read", CommandReadFromFile, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Include", CommandReadFromFile, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Save", CommandSaveToFile, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Files", CommandListFiles, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Var", CommandVar, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Set", CommandSet, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Unit", CommandUnit, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Print", CommandPrint, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("List", CommandList, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Store", CommandStore, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Remove", CommandRemove, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Clear", CommandClear, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Func", CommandFunc, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Help", CommandHelp, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Version", CommandVersion, CommandLine, ref ResultLine, ref CommandHandled)
                                   || (CommandHandled = IdentifierAssumed(ref CommandLine, ref ResultLine)) // Assume a print or set command
                                   || (CommandHandled = CommandPrint(ref CommandLine, ref ResultLine)) // Assume a print command
                                   || (CommandHandled = base.Command(CommandLine, out ResultLine));

            return CommandHandled;
        }

        public Boolean FunctionDecleration(String CommandLine, out String ResultLine)
        {
            ResultLine = "";
            Boolean DeclerationDone = ParseFunctionDecleration(ref CommandLine, ref ResultLine);

            return true; // CommandHandled; Don't exit
        }

        #region Command methods

        enum CommandHelpPart
        {
            none = 0,
            expression = 1,
            parameter = 2,
            command = 3,
            all = 0xF
        }

        public Boolean CommandHelp(ref String CommandLine, ref String ResultLine)
        {

            CommandHelpPart HelpPart = CommandHelpPart.command;
            if (CommandLine.StartsWithKeyword("Exp"))
                HelpPart = CommandHelpPart.expression;
            else if (CommandLine.StartsWithKeyword("Par"))
                HelpPart = CommandHelpPart.parameter;
            else if (CommandLine.StartsWithKeyword("All"))
                HelpPart = CommandHelpPart.all;

            ResultLine = "";
            if (HelpPart == CommandHelpPart.command || HelpPart == CommandHelpPart.all)
            {
                ResultLine += "Commands:\n"
                            + "    Include <filename>                                                           Reads commands from file\n"
                            + "    Save <filename>                                                              Save variables and functions declarations to file\n"
                            + "    Files [-sort=<filepropertyname>] [ [-path=] <folderpath> ]                   List files in folder\n"
                            + "    Var [ <contextname> . ] <varname> [ '=' <expression> ] [',' <var> ]*         Declare new variable (local or in specified context)\n"
                            + "    Set <varname> [ '=' ] <expression> [',' <varname> [ '=' ] <expression> ]*    Assign variable (declare it locally if not already declared)\n"
                            + "    Unit <unitname> [ [ '=' ] <expression> ]                                     Define new unit. Without an expression it becomes a base unit,\n"
                            + "                                                                                     else a converted (scaled) unit\n"
                            + "    [ Print ] <expression> [',' <expression> ]*                                  Evaluate expressions and show values\n"
                            + "    List                                                                         Show all variable values and functions declarations\n"
                            + "    Store <varname>                                                              Save last calculation's result to variable\n"
                            + "    Remove <varname> [',' <varname> ]*                                           Remove variable\n"
                            + "    Clear                                                                        Remove all variables\n"
                            + "    Func <functionname> '(' <paramlist> ')'  '{' <commands> '}'                  Declare a function\n"
                            + "    Version                                                                      Shows application version info";
            }
            if (HelpPart == CommandHelpPart.expression || HelpPart == CommandHelpPart.all)
            {
                if (HelpPart == CommandHelpPart.all)
                {
                    ResultLine += "\n\n";
                }
                ResultLine += "Expression:\n"
                         // + "    <expression> = <CE> .                                                        Expression\n"
                            + "    <CE> = <E> | <E> '[' <SYS> ']' .                                             Converted Expression\n"
                            + "    <E> = <E> '+' <T> | <E> '-' <T> | <T> .                                      Expression (simple/unconverted)\n"
                            + "    <T> = <T> '*' <F> | <T> '/' <F> | <F> .                                      Term\n"
                            + "    <F> = <PE> | <PE> '^' num .                                                  Factor\n"
                            + "    <PE> = <PQ> | <UE> | <varname>                                               Primary expression\n"
                            + "         | <functionname> '(' <explist> ')' | '(' <E> ')' .                      \n"
                            + "    <PQ> = num <SU> .                                                            Physical Measure\n"
                            + "    <SU> = s<U> | <U> .                                                          Scaled Unit\n"
                            + "    <SYS> = sys | sys '.' <SU> | <SU> .                                          System unit\n"
                            + "    <UE> = + <F> | - <F> .                                                       Unary operator\n"
                            + "    <explist> = <CE> | <CE> ',' <explist> .                                      Expression List";
            }
            if (HelpPart == CommandHelpPart.parameter || HelpPart == CommandHelpPart.all)
            {
                if (HelpPart == CommandHelpPart.all)
                {
                    ResultLine += "\n\n";
                }
                ResultLine += "Parameter list:\n"
                            + "    <paramlist> = <param> | <param> ',' <paramlist> .                            Parameter list\n"
                            + "    <param> = <paramname> | <paramname> '[' <SYS> ']' .                          Parameter";
            }
            CommandLine = "";
            return true;
        }

        public Boolean CommandVersion(ref String CommandLine, ref String ResultLine)
        {
            //PhysCalc
            System.Reflection.Assembly PhysCaclAsm = System.Reflection.Assembly.GetExecutingAssembly();

            //PhysicalMeasure
            System.Reflection.Assembly PhysicalMeasureAsm = typeof(PhysicalMeasure.PhysicalQuantity).Assembly;

            ResultLine = PhysCaclAsm.AssemblyInfo() + "\n" + PhysicalMeasureAsm.AssemblyInfo();

            CommandLine = "";
            return true;
        }
        
        public Boolean CommandComment(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = ""; // = CommandLine;
            CommandLine = "";
            return true;
        }
        
        public Boolean CommandReadFromFile(ref String CommandLine, ref String ResultLine)
        {
            String FilePathStr;
            ResultLine = "";

            FilePathStr = CommandLine;
            CommandLine = "";

            if ((CommandLineReader != null) && (!string.IsNullOrWhiteSpace(FilePathStr)))
            {
                CommandLineReader.AddFile(FilePathStr);
                ResultLine = "Reading from '" + CommandLineReader.Accessor() + "' ";
            }
            return true;
        }

        public Boolean CommandSaveToFile(ref String CommandLine, ref String ResultLine)
        {
            String FileNameStr;
            FileNameStr = CommandLine;
            CommandLine = "";
            ResultLine = "";

            if (!Path.HasExtension(FileNameStr))
            {
                FileNameStr += ".cal";
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileNameStr))
            {
                file.WriteLine("// Saved {0} to {1}", DateTime.Now.ToSortString(), FileNameStr);

                SaveContextToFile(CurrentContext, file);

                if (Accumulator != null)
                {
                    file.WriteLine("set {0} = {1}", AccumulatorName, Accumulator.ToString());
                }
            }

            return true;
        }

        public void SaveContextToFile(CalculatorEnviroment Context, System.IO.StreamWriter file)
        {
            if (Context.OuterContext != null)
            {
                SaveContextToFile(Context.OuterContext, file);
            }

            file.WriteLine("// {0}", Context.Name);

            foreach (KeyValuePair<String, INametableItem> Item in Context.NamedItems)
            {
                Item.Value.WriteToTextFile(Item.Key, file);
            }
        }

        public Boolean CommandListFiles(ref String CommandLine, ref String ResultLine)
        {
            String FilePathStr = "."; // Look in current (local) dir
            String SortStr = "name"; // Sort by file name
            Func<FileInfo, String> KeySelector;
            ResultLine = "";

            if (!string.IsNullOrWhiteSpace(CommandLine))
            {
                while (TryParseChar('-', ref CommandLine))
                {
                    if (TryParseToken("sort", ref CommandLine))
                    {
                        ParseChar('=', ref CommandLine, ref ResultLine);
                        CommandLine = CommandLine.ReadToken(out SortStr);
                    }
                    else if (TryParseToken("path", ref CommandLine))
                    {
                        ParseChar('=', ref CommandLine, ref ResultLine);
                        //CommandLine.ReadToken(out FilePathStr);
                        FilePathStr = CommandLine;
                    }
                }

                // Last token is FilePathStr
                if (!string.IsNullOrWhiteSpace(CommandLine))
                {
                    FilePathStr = CommandLine;
                }
            }

            if (SortStr.StartsWithKeyword("create"))
            {
                KeySelector = (e => e.CreationTime.ToSortString());
            }
            else if (SortStr.StartsWithKeyword("write"))
            {
                KeySelector = (e => e.LastWriteTime.ToSortString());
            }
            else if (SortStr.StartsWithKeyword("access"))
            {
                KeySelector = (e => e.LastAccessTime.ToSortString());
            }
            else
            // if (SortStr.StartsWithKeyword("name"))
            {
                KeySelector = (e => e.Name);
            }

            StringBuilder ListStringBuilder = new StringBuilder();

            System.IO.DirectoryInfo dir;
            IEnumerable<FileInfo> CalFiles;
            try
            {
                dir = new System.IO.DirectoryInfo(FilePathStr);

                if (dir.Exists)
                {
                    // Get all of the .cal files from the directory and order them by file name in descending order
                    //IEnumerable<FileInfo> files = dir.GetFiles("*.cal").OrderByDescending(e => e.Name); 
                    CalFiles = dir.GetFiles("*.cal").OrderBy(KeySelector);
                    int fileCount = 0;
                    //foreach (FileInfo fi in dir.EnumerateFiles("*.cal"))
                    foreach (FileInfo fi in CalFiles)
                    {
                        if (fileCount == 0)
                        {
                            ListStringBuilder.AppendFormat("{0,16}   {1,8}   {2,16}   {3,16}   {4,16}\n\r", "File name", "size/bytes", "last access", "last write", "created");
                        }
                        fileCount++;

                        ListStringBuilder.AppendFormat("{0,16}   {1,8}   {2,16}   {3,16}   {4,16}\n\r", fi.Name, fi.Length, fi.LastAccessTime, fi.LastWriteTime, fi.CreationTime);
                    }

                    ListStringBuilder.AppendFormat("Found {0} .cal files in '{1}'", fileCount, dir.FullName);
                }
                else
                {
                    ListStringBuilder.AppendFormat("Folder not found '{0}'", dir.FullName);
                }
            }
            catch (Exception e)
            {
                String Message = String.Format("{0} Exception Source: {1} - {2}", e.GetType().ToString(), e.Source, e.ToString());

                ListStringBuilder.AppendFormat("Folder not found '{0}'. {1}", FilePathStr, Message);
            }

            ResultLine = ListStringBuilder.ToString();
            return true;
        }

        public Boolean CommandVar(ref String CommandLine, ref String ResultLine)
        {
            String VariableName;
            ResultLine = "";
            Boolean OK;
            do
            {
                OK = false;

                CommandLine = CommandLine.ReadIdentifier(out VariableName);
                if (VariableName == null)
                {
                    ResultLine = "Variable name expected";
                }
                else
                {
                    Debug.Assert(VariableName != null);

                    CalculatorEnviroment VariableContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(VariableName, out VariableContext, out Item);

                    CalculatorEnviroment FoundVariableDeclartionNamespace = VariableContext as CalculatorEnviroment;
                    CalculatorEnviroment NewVariableDeclartionNamespace;

                    if (CurrentContext.DefaultDeclerationEnviroment == VariableDeclerationEnviroment.global)
                    {
                        NewVariableDeclartionNamespace = GlobalContext;
                    }
                    else
                    {
                        NewVariableDeclartionNamespace = CurrentContext;
                    }

                    Boolean ALocalIdentifier = IdentifierFound && FoundVariableDeclartionNamespace == NewVariableDeclartionNamespace;
                    OK = !ALocalIdentifier || Item.identifierkind == IdentifierKind.variable;

                    if (OK)
                    {
                        TryParseToken("=", ref CommandLine);

                        IPhysicalQuantity pq = GetPhysicalQuantity(ref CommandLine, ref ResultLine);

                        if (!ALocalIdentifier || pq != null)
                        {
                            // Declare new local var or set new value
                            OK = VariableSet(NewVariableDeclartionNamespace, VariableName, pq);
                            if (pq != null)
                            {
                                ResultLine = VariableName + " = " + pq.ToString();
                            }
                            else
                            {
                                ResultLine = "Variable '" + VariableName + "' declared";
                            }
                        }
                        else
                        {
                            ResultLine = "Variable '" + VariableName + "' was already declared";
                        }
                    }
                    else 
                    {
                        if (ALocalIdentifier && Item.identifierkind != IdentifierKind.variable)
                        {
                            ResultLine = "Identifier '" + VariableName + "' was already declared as " + Item.identifierkind.ToString();
                        }
                    }
                }
            } while (OK && TryParseToken(",", ref CommandLine));
            return true;
        }

        public Boolean CommandSet(ref String CommandLine, ref String ResultLine)
        {
            CalculatorEnviroment IdentifierContext;
            String VariableName;
            ResultLine = "";
            Boolean OK;
            do
            {
                OK = false;
                Boolean IdentifierFound = ParseQualifiedIdentifier(ref CommandLine, ref ResultLine, out IdentifierContext, out VariableName);

                if (VariableName == null)
                {
                    ResultLine = "Variable name expected";
                }
                else
                {
                    TryParseToken("=", ref CommandLine);

                    Boolean IsCalculatorSetting = CheckForCalculatorSetting(IdentifierContext, VariableName, ref CommandLine, ref ResultLine);
                    if (!IsCalculatorSetting)
                    {
                        IPhysicalQuantity pq = GetPhysicalQuantity(ref CommandLine, ref ResultLine);

                        if (pq != null)
                        {
                            OK = VariableSet(IdentifierContext, VariableName, pq);

                            ResultLine = VariableName + " = " + pq.ToString();
                            Accumulator = pq;
                        }
                    }
                }
            } while (OK && TryParseToken(",", ref CommandLine));

            return true;
        }

        public Boolean CommandUnit(ref String CommandLine, ref String ResultLine)
        {
            String UnitName;
            ResultLine = "";
            Boolean OK;
            do
            {
                OK = false;

                CommandLine = CommandLine.ReadIdentifier(out UnitName);
                if (UnitName == null)
                {
                    ResultLine = "Unit name expected";
                }
                else
                {
                    Debug.Assert(UnitName != null);

                    CalculatorEnviroment UnitContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(UnitName, out UnitContext, out Item);

                    CalculatorEnviroment FoundUnitDeclartionNamespace = UnitContext as CalculatorEnviroment;
                    CalculatorEnviroment NewUnitDeclartionNamespace;

                    if (CurrentContext.DefaultDeclerationEnviroment == VariableDeclerationEnviroment.global)
                    {
                        NewUnitDeclartionNamespace = GlobalContext;
                    }
                    else
                    {
                        NewUnitDeclartionNamespace = CurrentContext;
                    }

                    Boolean ALocalIdentifier = IdentifierFound && FoundUnitDeclartionNamespace == NewUnitDeclartionNamespace;
                    OK = !ALocalIdentifier || Item.identifierkind == IdentifierKind.unit;

                    if (OK)
                    {
                        TryParseToken("=", ref CommandLine);

                        IPhysicalQuantity pq = GetPhysicalQuantity(ref CommandLine, ref ResultLine);
                        OK = UnitSet(NewUnitDeclartionNamespace, UnitName, pq);
                        if (OK)
                        {
                            if (pq != null)
                            {
                                // Defined new local unit with value
                                ResultLine = " Unit " + UnitName + " = " + pq.ToString();
                            }
                            else
                            {
                                // Defined new local base unit 
                                ResultLine = "Unit '" + UnitName + "' was declared.";
                            }
                        }
                        else
                        {
                            ResultLine = "Unit '" + UnitName + "' was not declared.\r\n" + ResultLine;
                        }
                    }
                    else
                    {
                        ResultLine = "Identifier '" + UnitName + "' was already declared as " + Item.identifierkind.ToString();
                    }
                }
            } while (OK && TryParseToken(",", ref CommandLine));
            return true;
        }

        public Boolean CommandStore(ref String CommandLine, ref String ResultLine)
        {
            Boolean OK = false;
            String VariableName;
            ResultLine = "";
            CommandLine = CommandLine.ReadIdentifier(out VariableName);

            if (VariableName == null)
            {
                ResultLine = "Variable name expected";
            }
            else
            {
                IPhysicalQuantity pq = Accumulator;

                if (pq != null)
                {
                    OK = VariableSet(CurrentContext, VariableName, pq);

                    ResultLine = pq.ToString();
                }
            }

            return OK;
        }

        public Boolean CommandRemove(ref String CommandLine, ref String ResultLine)
        {
            Boolean OK = false;
            String ItemName;
            ResultLine = "";
            do
            {
                CommandLine = CommandLine.ReadIdentifier(out ItemName);
                if (ItemName == null)
                {
                    ResultLine = "Variable name or unit name or function name expected";
                }
                else
                {
                    CalculatorEnviroment IdentifierContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(ItemName, out IdentifierContext, out Item);

                    if (IdentifierFound)
                    {
                        OK = IdentifierContext.RemoveLocalIdentifier(ItemName);
                    }
                    else
                    {
                        ResultLine = "'" + ItemName + "' not known";
                    }
                }
            } while (OK && TryParseToken(",", ref CommandLine));
            return OK;
        }

        public Boolean CommandClear(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = "";

            Accumulator = null;
            return IdentifiersClear();
        }


        public Boolean CommandList(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = CurrentContext.ListIdentifiers();
            return true;
        }

        public Boolean CommandPrint(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = "";
            do
            {
                IPhysicalQuantity pq = GetPhysicalQuantity(ref CommandLine, ref ResultLine);

                if (pq != null)
                {
                    if (!String.IsNullOrWhiteSpace(ResultLine))
                    {
                        ResultLine += ", ";
                    }
                    ResultLine += pq.ToString();

                    Accumulator = pq;
                }
            } while (!String.IsNullOrWhiteSpace(CommandLine) && TryParseToken(",", ref CommandLine));

            return true;
        }

        public Boolean IdentifierAssumed(ref String CommandLine, ref String ResultLine)
        {
            string token;
            int len = CommandLine.PeekToken(out token);

            if (token.IsIdentifier() )
            {
                string token2;
                int len2 = CommandLine.Substring(len).TrimStart().PeekToken(out token2);

                if (token2 == "=")
                {
                    // Assume a set command
                    return CommandSet(ref CommandLine, ref ResultLine);
                }
                else
                {
                    // Assume a print command
                    return CommandPrint(ref CommandLine, ref ResultLine);
                }
            }

            return false;
        }

        public Boolean CommandFunc(ref String CommandLine, ref String ResultLine)
        {
            String FunctionName;
            ResultLine = "";
            CommandLine = CommandLine.ReadIdentifier(out FunctionName);
            if (FunctionName == null)
            {
                ResultLine = "Function name expected";
            }
            else
            {
                CalculatorEnviroment IdentifierContext;
                INametableItem Item;

                Boolean IdentifierFound = CurrentContext.FindIdentifier(FunctionName, out IdentifierContext, out Item);

                CalculatorEnviroment FoundFunctionDeclartionNamespace = IdentifierContext as CalculatorEnviroment;
                CalculatorEnviroment NewFunctionDeclartionNamespace;

                if (CurrentContext.DefaultDeclerationEnviroment == VariableDeclerationEnviroment.global)
                {
                    NewFunctionDeclartionNamespace = GlobalContext;
                }
                else
                {
                    NewFunctionDeclartionNamespace = CurrentContext;
                }

                Boolean ALocalIdentifier = IdentifierFound && FoundFunctionDeclartionNamespace == NewFunctionDeclartionNamespace;
                Boolean OK = !ALocalIdentifier || Item.identifierkind == IdentifierKind.function;
                if (OK)
                {
                    if (ALocalIdentifier)
                    {
                        ResultLine = "Function '" + FunctionName + "' already declared";
                    }
                    else
                    {
                        CurrentContext.FunctionToParseName = FunctionName;
                        CurrentContext.FunctionToParse = new PhysicalQuantityFunction();
                        CurrentContext.ParseState = CommandPaserState.readfunctionparamlist;

                        ParseFunctionDecleration(ref CommandLine, ref ResultLine);
                    }
                }
                else
                {
                    ResultLine = "'" + FunctionName + "' already definded as " + Item.identifierkind.ToString();
                }
            }
            return true;
        }


        #endregion Command methods

        #region Command helpers

        public IPhysicalQuantity GetPhysicalQuantity(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq = PhysicalCalculator.Expression.PhysicalExpression.ParseConvertedExpression(ref CommandLine, ref ResultLine);

            if (pq == null)
            {
                if (!String.IsNullOrEmpty(ResultLine)) 
                {
                    ResultLine += ". ";
                }
                ResultLine += "Physical quantity expected";
            }

            return pq;
        }

        public Boolean ParseQualifiedIdentifier(ref String CommandLine, ref String ResultLine, out CalculatorEnviroment QualifiedIdentifierContext, out String IdentifierName)
        {
            String QualifiedIdentifierName;
            CalculatorEnviroment PrimaryContext;
            INametableItem Item;
            IdentifierKind identifierkind;

            CommandLine = CommandLine.ReadIdentifier(out IdentifierName);
            Debug.Assert(IdentifierName != null);

            Boolean IdentifierFound = CurrentContext.FindIdentifier(IdentifierName, out PrimaryContext, out Item);

            if (IdentifierFound)
            {
                identifierkind = Item.identifierkind;
            }
            else
            {   // Look for Global system settings and predefined symbols
                IdentifierFound = IdentifierName.Equals("Global", StringComparison.OrdinalIgnoreCase);
                if (IdentifierFound)
                {
                    PrimaryContext = GlobalContext.OuterContext;
                    identifierkind = IdentifierKind.enviroment;
                }
                else
                {
                    IdentifierFound = IdentifierName.Equals("Outer", StringComparison.OrdinalIgnoreCase);
                    if (IdentifierFound)
                    {
                        PrimaryContext = CurrentContext.OuterContext;
                        identifierkind = IdentifierKind.enviroment;
                    }
                    else
                    {
                        IdentifierFound = IdentifierName.Equals("Local", StringComparison.OrdinalIgnoreCase);
                        if (IdentifierFound)
                        {
                            PrimaryContext = CurrentContext;
                            identifierkind = IdentifierKind.enviroment;
                        }
                        else
                        {
                            identifierkind = IdentifierKind.unknown;
                        }
                    }
                }
            }

            QualifiedIdentifierContext = PrimaryContext;
            QualifiedIdentifierName = IdentifierName;

            while (IdentifierFound 
                && identifierkind == IdentifierKind.enviroment 
                && !String.IsNullOrEmpty(CommandLine) 
                && CommandLine[0] == '.')
            {
                TokenString.ParseChar('.', ref CommandLine, ref ResultLine);
                CommandLine = CommandLine.TrimStart();

                CommandLine = CommandLine.ReadIdentifier(out IdentifierName);
                Debug.Assert(IdentifierName != null);
                CommandLine = CommandLine.TrimStart();

                CalculatorEnviroment FoundInContext;
                IdentifierKind FoundIdentifierkind;

                IdentifierFound = QualifiedIdentifierCalculatorEnviromentLookup(QualifiedIdentifierContext, IdentifierName, out FoundInContext, out FoundIdentifierkind);
                if (IdentifierFound)
                {
                    QualifiedIdentifierContext = FoundInContext;
                    identifierkind = FoundIdentifierkind;

                    QualifiedIdentifierName += "." + IdentifierName;
                }
            }

            return IdentifierFound;
        }


        public Boolean ParseFunctionDecleration(ref String CommandLine, ref String ResultLine)
        {
            IFunctionEvaluator FuncEval = null;
            FuncEval = PhysicalCalculator.Function.PhysicalFunction.ParseFunctionDecleration(CurrentContext, ref CommandLine, ref ResultLine);
            if (FuncEval != null)
            {
                CurrentContext.NamedItems.Add(CurrentContext.FunctionToParseName, FuncEval);

                ResultLine = "Function '" + CurrentContext.FunctionToParseName + "' declared";

                CurrentContext.FunctionToParse = null;
                CurrentContext.FunctionToParseName = null;

                CurrentContext.ParseState = CommandPaserState.executecommandline;
            }

            return FuncEval != null;
        }


        public Boolean CheckForCalculatorSetting(CalculatorEnviroment IdentifierContext, String VariableName, ref String CommandLine, ref String ResultLine)
        {
            Boolean SettingFound = false;
            if (VariableName.IsKeyword("Tracelevel"))
            {
                SettingFound = true;
                String tracelevelvaluestr;
                Tracelevel tl = Tracelevel.all ;
                CommandLine = CommandLine.ReadToken(out tracelevelvaluestr);
                if (tracelevelvaluestr.IsKeyword("Normal"))
                {
                    tl = Tracelevel.normal;
                }
                else if (tracelevelvaluestr.IsKeyword("On"))
                {
                    tl = Tracelevel.high;
                }
                else if (tracelevelvaluestr.IsKeyword("Off"))
                {
                    tl = Tracelevel.low;
                }
                else if (tracelevelvaluestr.IsKeyword("Debug"))
                {
                    tl = Tracelevel.all;
                }

                if (IdentifierContext != null)
                {
                    IdentifierContext.OutputTracelevel = tl;
                }
                else
                {
                    CommandLineReader.OutputTracelevel = tl;
                    CurrentContext.OutputTracelevel = tl;
                }
            }

            return SettingFound;
        }

        #endregion Command helpers

        #region Variables access

        public Boolean VariableSet(CalculatorEnviroment context, String VariableName, IPhysicalQuantity VariableValue)
        {
            if (VariableName == AccumulatorName)
            {
                Accumulator = VariableValue;
                //return true;
                return false;
            }
            else 
            {
                if (context == null)
                {
                    context = CurrentContext;
                }
                return context.VariableSet(VariableName, VariableValue);
            }
        }

        public Boolean VariableSetLocal(String VariableName, IPhysicalQuantity VariableValue)
        {
            return VariableSet(CurrentContext, VariableName, VariableValue);
        }

        public Boolean VariableSetGlobal(String VariableName, IPhysicalQuantity VariableValue)
        {
            return VariableSet(GlobalContext, VariableName, VariableValue);
        }

        public Boolean VariableSet(String VariableName, IPhysicalQuantity VariableValue)
        {
            if (VariableName == AccumulatorName)
            {
                Accumulator = VariableValue;
                return true;
            }
            else 
            {
                return CurrentContext.VariableSet(VariableName, VariableValue);
            }
        }

        public Boolean VariableGet(IEnviroment Context, String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine)
        {
            return Context.VariableGet(VariableName, out VariableValue, ref ResultLine);
        }

        #endregion  Variables access

        #region  Costum Unit access

        public Boolean UnitSet(CalculatorEnviroment context, String UnitName, IPhysicalQuantity UnitValue)
        {
            return context.UnitSet(UnitName, UnitValue);
        }

        #endregion  Costum Unit  access

        #region  Function access

        public Boolean FunctionLookup(IEnviroment Context, String FunctionName, out IFunctionEvaluator functionevaluator)
        {
            return Context.FunctionFind(FunctionName, out functionevaluator);
        }

        public Boolean FunctionEvaluate(String FunctionName, IFunctionEvaluator functionevaluator, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            Boolean OK = false;
            FunctionResult = null;

            if (functionevaluator != null)
            {
                CalculatorEnviroment LocalContext = new CalculatorEnviroment();
                LocalContext.Name = "Function "+ FunctionName;
                LocalContext.OuterContext = CurrentContext;
                CurrentContext = LocalContext;

                OK = functionevaluator.Evaluate(LocalContext, parameterlist, out FunctionResult, ref ResultLine);
                CurrentContext = LocalContext.OuterContext;
                LocalContext.OuterContext = null;
            }

           return OK;
        }

        public Boolean FunctionEvaluateFileRead(String FunctionName, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            Boolean OK = false;
            FunctionResult = null;

            if (CommandLineReader != null)
            {
                CalculatorEnviroment LocalContext = new CalculatorEnviroment();
                LocalContext.Name = "File Function "+ FunctionName;
                LocalContext.OuterContext = CurrentContext;
                CurrentContext = LocalContext;

                Commandreader functionCommandLineReader = new Commandreader(FunctionName + ".cal", CommandLineReader.ResultLineWriter);

                functionCommandLineReader.ReadFromConsoleWhenEmpty = false; // Return from ExecuteCommands() function when file commands are done

                if (LocalContext.OuterContext.OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
                {
                    functionCommandLineReader.ResultLineWriter.WriteLine("Enter " + LocalContext.Name);
                }
                ExecuteCommands(LocalContext, functionCommandLineReader, ResultLineWriter);
                FunctionResult = Accumulator;
                if (LocalContext.OuterContext.OutputTracelevel.HasFlag(Tracelevel.functionenterleave))
                {
                    functionCommandLineReader.ResultLineWriter.WriteLine("Leave " + LocalContext.Name);
                }
                OK = true;

                CurrentContext = LocalContext.OuterContext;
                LocalContext.OuterContext = null;
            }

           return OK;
        }

        #endregion  Function access

        #region  Identifier access

        public Boolean IdentifierContextLookup(String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind)
        {
            CalculatorEnviroment context;
            INametableItem Item;

            Boolean IdentifierFound = CurrentContext.FindIdentifier(IdentifierName, out context, out Item);
            if (IdentifierFound)
            {
                identifierkind = Item.identifierkind;
            }
            else
            {   // Look for Global system settings and predefined symbols
                IdentifierFound = IdentifierName.Equals("Global", StringComparison.OrdinalIgnoreCase);
                if (IdentifierFound)
                {
                    context = GlobalContext.OuterContext;
                    identifierkind = IdentifierKind.enviroment;
                }
                else
                {
                    IdentifierFound = IdentifierName.Equals("Outer", StringComparison.OrdinalIgnoreCase);
                    if (IdentifierFound)
                    {
                        context = CurrentContext.OuterContext;
                        identifierkind = IdentifierKind.enviroment;
                    }
                    else
                    {
                        IdentifierFound = IdentifierName.Equals("Local", StringComparison.OrdinalIgnoreCase);
                        if (IdentifierFound)
                        {
                            context = CurrentContext;
                            identifierkind = IdentifierKind.enviroment;
                        }
                        else
                        {
                            identifierkind = IdentifierKind.unknown;
                        }
                    }
                }
            }

            FoundInContext = context;
            return IdentifierFound;
        }

        public Boolean QualifiedIdentifierCalculatorEnviromentLookup(IEnviroment LookInContext, String IdentifierName, out CalculatorEnviroment FoundInContext, out IdentifierKind identifierkind)
        {
            CalculatorEnviroment context;
            INametableItem Item;

            Boolean Found = CurrentContext.FindIdentifier(IdentifierName, out context, out Item);
            FoundInContext = context;

            if (Found)
            {
                identifierkind = Item.identifierkind;
            }
            else
            {
                identifierkind = IdentifierKind.unknown;
            }
            return Found;
        }

        public Boolean QualifiedIdentifierIEnviromentLookup(IEnviroment LookInContext, String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind)
        {
            CalculatorEnviroment context;
            Boolean Res = QualifiedIdentifierCalculatorEnviromentLookup(LookInContext, IdentifierName, out context, out identifierkind);
            FoundInContext = context;
            return Res;
        }

        public Boolean IdentifiersClear()
        {
            return CurrentContext.ClearLocalIdentifiers();
        }

        #endregion  Identifier access

    }

    static class DateTimeSortString
    {
        public static string ToSortString(this DateTime Me)
        {
            return Me.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToSortShortDateString(this DateTime Me)
        {
            return Me.ToString("yyyy-MM-dd");
        }
    }

    static class ExtensionList
    {
        public static String AssemblyInfo(this System.Reflection.Assembly Asm)
        {
            System.Reflection.AssemblyName AsmName = Asm.GetName();

            FileInfo AsmFileInfo = new FileInfo(Asm.Location);
            Version AsemVersion = AsmName.Version;
            DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * AsemVersion.Build +             // days since 1 January 2000
                                                                          TimeSpan.TicksPerSecond * 2 * AsemVersion.Revision));  // seconds since midnight, (multiply by 2 to get original              
            String InfoStr = String.Format("{0,-16} {1} {2}", AsmName.Name, AsemVersion.ToString(), buildDateTime.ToSortString());
            return InfoStr;
        }
    }
}


