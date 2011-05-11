using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using PhysicalMeasure;
using System.Diagnostics;
using System.IO;

using TokenParser;
using PhysicalMeasure.Expression;

namespace PhysCalc
{

    class PhysCalculator : Commandhandler
    {
        Commandreader CommandLineReader = null;
        ResultWriter ResultLineWriter = null;

        const string AccumulatorName = "Accumulator";
        IPhysicalQuantity Accumulator = null;
        private Dictionary<String, IPhysicalQuantity> Variables = new Dictionary<string,IPhysicalQuantity>();

        public PhysCalculator()
        {
            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new Commandreader(this.ResultLineWriter);
        }

        public PhysCalculator(Commandreader CommandLineReader)
        {
            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = CommandLineReader;
        }

        public PhysCalculator(String[] PhysCalculatorConfig_args)
        {
            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new Commandreader(PhysCalculatorConfig_args, this.ResultLineWriter);
        }

        public PhysCalculator(Commandreader CommandLineReader, String[] PhysCalculatorConfig_args)
        {
            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = CommandLineReader;
        }

        public PhysCalculator(Commandreader CommandLineReader, ResultWriter ResultLineWriter)
        {
            this.ResultLineWriter = ResultLineWriter;
            this.CommandLineReader = CommandLineReader;
        }

        public void Run()
        {
            Boolean CommandLineFromFile = false;
            Boolean CommandLineEmpty;
            Boolean ResultLineEmpty;
            Boolean LoopExit;

            // Setup VariableLookup callback 
            PhysicalExpression.VariableGetCallback = VariableGet;

            // ResultWriter.WriteLineToFile("PhysCalculator ready");
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
                    ResultLineWriter.Write(">");
                    CommandLineFromFile = CommandLineReader.HasFile();
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
                            LoopExit = !Command(CommandLine, out ResultLine);

                            if (!String.IsNullOrWhiteSpace(ResultLine))
                            {
                                ResultLineWriter.WriteLine(ResultLine);
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
            } while ((CommandLineFromFile || !CommandLineEmpty || !ResultLineEmpty) && !LoopExit);
        }

        public override Boolean Command(String CommandLine, out String ResultLine)
        {
            Boolean CommandHandled = false;
            ResultLine = "Unknown command";
            Boolean CommandFound =    CheckForCommand("//", CommandComment, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Read", CommandReadFromFile, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Save", CommandSaveToFile, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Set", CommandSet, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Print", CommandPrint, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("List", CommandList, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Store", CommandStore, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Remove", CommandRemove, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Clear", CommandClear, CommandLine, ref ResultLine, ref CommandHandled)
                                   || CheckForCommand("Help", CommandHelp, CommandLine, ref ResultLine, ref CommandHandled)
                                   || (CommandHandled = CommandPrint(ref CommandLine, ref ResultLine)) // Assume a print command
                                   || (CommandHandled = base.Command(CommandLine, out ResultLine));

            return CommandHandled;
        }

        #region Command methods

        public Boolean CommandHelp(ref String CommandLine, ref String ResultLine)
        {

            ResultLine = "Commands:\n"
                        + "    Read <filename>                          Reads commands from file\n"
                        + "    Save <filename>                          Save variables to file\n"
                        + "    Set <varname> = <expression>             Assign variable\n"
                        + "    Print <expression> [, <expression> ]*    Show variable values\n"
                        + "    List                                     Show all variable values\n"
                        + "    Store <varname>                          Save last calculation's result to variable\n"
                        + "    Remove <varname>                         Remove variable\n"
                        + "    Clear                                    Remove all variables";

            CommandLine = "";
            return true;
        }
        

        public Boolean CommandComment(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = CommandLine;
            CommandLine = "";
            return true;
        }
        
        public Boolean CommandReadFromFile(ref String CommandLine, ref String ResultLine)
        {
            String FilePathStr;
            ResultLine = "";

            FilePathStr = CommandLine;
            CommandLine = "";

            if (CommandLineReader != null)
            {
                CommandLineReader.SetFile(FilePathStr);
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
                foreach (KeyValuePair<String, IPhysicalQuantity> Variable in Variables)
                {
                    file.WriteLine("set {0} = {1}", Variable.Key, Variable.Value.ToString());
                }

                if (Accumulator != null)
                {
                    file.WriteLine("set {0} = {1}", AccumulatorName, Accumulator.Value.ToString());
                }
            }

            //ResultLine = ListStringBuilder.ToString();

            return true;
        }

        public Boolean CommandSet(ref String CommandLine, ref String ResultLine)
        {
            String VariableName;
            ResultLine = "";
            CommandLine = CommandLine.ReadIdentifier(out VariableName);

            if (VariableName == null)
            {
                ResultLine = "Variable name expected";
            }
            else
            {
                TryParseToken("=", ref CommandLine);

                IPhysicalQuantity pq = GetPhysicalQuantity(ref CommandLine, ref ResultLine);

                if (pq != null)
                {
                    VariableSet(VariableName, pq);

                    ResultLine = pq.ToString();
                }
            }

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
                    OK = VariableSet(VariableName, pq);

                    ResultLine = pq.ToString();
                }
            }

            return OK;
        }

        public Boolean CommandRemove(ref String CommandLine, ref String ResultLine)
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
                OK = VariableRemove(VariableName);
                if (!OK)
                {
                    ResultLine = "Variable '" + VariableName + "' not know";
                }
            }
            return OK;
        }

        public Boolean CommandClear(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = "";

            Accumulator = null;
            return VariablesClear();
        }


        public Boolean CommandList(ref String CommandLine, ref String ResultLine)
        {
            ResultLine = "";
            StringBuilder ListStringBuilder = new StringBuilder();

            foreach (KeyValuePair<String, IPhysicalQuantity> Variable in Variables)
            {
                ListStringBuilder.AppendFormat("{0} : {1}", Variable.Key, Variable.Value.ToString());
                ListStringBuilder.AppendLine();
            }

            ResultLine = ListStringBuilder.ToString();

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

        #endregion Command methods

        #region Command helpers

        public IPhysicalQuantity GetPhysicalQuantity(ref String CommandLine, ref String ResultLine)
        {
            // IPhysicalQuantity pq = ParseConvertedExpression(ref CommandLine, ref ResultLine);
            IPhysicalQuantity pq = PhysicalMeasure.Expression.PhysicalExpression. ParseConvertedExpression(ref CommandLine, ref ResultLine);
            return pq;
        }
        #endregion Command helpers

        #region Variables access
        public Boolean VariableSet(String VariableName, IPhysicalQuantity VariableValue)
        {
            if (VariableName == AccumulatorName)
            {
                Accumulator = VariableValue;
            }
            else if (Variables.ContainsKey(VariableName))
            {
                Variables[VariableName] = VariableValue;
            }
            else  
            {
                Variables.Add(VariableName, VariableValue);
            }

            return true;
        }

        public Boolean VariableRemove(String VariableName)
        {
            Boolean Found = Variables.ContainsKey(VariableName);
            if (Found)
            {
                Variables.Remove(VariableName);
            }

            return Found;
        }

        public Boolean VariablesClear()
        {
            Variables.Clear();
            
            return true;
        }

        public Boolean VariableGet(String VariableName, out IPhysicalQuantity VariableValue)
        {
            Boolean Found = Variables.ContainsKey(VariableName);
            if (Found)
            {
                VariableValue = Variables[VariableName];
            }
            else
            {
                VariableValue = null;
            }

            return Found;
        }
        #endregion  Variables access

        #region Expression parser methods
        /**
            CE = E | E [ SYS ] .
            E = E "+" T | E "-" T | T .
            T = T "*" F | T "/" F | F .
            F = PQ | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .         
            PQ = num SU . 
            SU = sU | U .
            SYS = sys | sys "." SU | SU .
            EXPLIST = E | E "," EXPLIST .
          
         
            CE = E CEopt .
            Eopt = [ SYS ] | e .
            E = T Eopt .
            Eopt = "+" T Eopt | "-" T Eopt | e .
            T = F Topt .
            Topt = "*" F Topt | "/" F Topt | e .
            F = PQ | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .
            PQ = num SU . 
            SU = sU | U | e .
            SYS = SYST | SU .
            SYST = system SYSTopt .
            SYSTopt = "." SU | e .
            EXPLIST = E EXPLISTopt . 
            EXPLISTopt = "," EXPLIST | e .
          
         **/
        /***
        public List<IPhysicalQuantity> ParseExpressionList(ref String CommandLine, ref String ResultLine)
        {
            List<IPhysicalQuantity> pqList = new List<IPhysicalQuantity>();
            Boolean MoreToParse = false;
            do
            {
                IPhysicalQuantity pq = null;
                pq = ParseConvertedExpression(ref CommandLine, ref ResultLine);
                pqList.Add(pq);

                MoreToParse = TryParseToken(",", ref CommandLine);
            } while (MoreToParse);
            return pqList;
        }

        public IPhysicalQuantity ParseConvertedExpression(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseExpression(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalConvertedExpression(pq, ref CommandLine, ref ResultLine);
            }

            return pq;
        }

        public IPhysicalQuantity ParseOptionalConvertedExpression(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                if (TryParseToken("[", ref CommandLine))
                { // "Convert to unit" square paranteses

                    int UnitStrLen = CommandLine.IndexOf(']');
                    if (UnitStrLen == 0)
                    {
                        ResultLine = "Missing unit to convert to";
                    }
                    else
                    {
                        String UnitString;
                        if (UnitStrLen == -1)
                        {   // Not terminated by ']', but handle that later
                            // Try to parse rest of line as an unit 
                            UnitString = CommandLine;
                            UnitStrLen = CommandLine.Length;
                        }
                        else
                        {   // Parse only the valid unit formatted string
                            UnitString = CommandLine.Substring(0, UnitStrLen);
                        }

                        UnitString = UnitString.TrimEnd();
                        String UnitStringAll = UnitString;
                        UnitStrLen = UnitString.Length;

                        IPhysicalUnit cu = new PhysicalMeasure.CombinedUnit();
                        IPhysicalQuantity one = new PhysicalQuantity(1, cu);
                        IPhysicalUnit pu = PhysicalUnit.ParseUnit(ref UnitString);

                        CommandLine = CommandLine.Substring(UnitStrLen - UnitString.Length);
                        CommandLine = CommandLine.TrimStart();
                        ParseToken("]", ref CommandLine, ref ResultLine);
                        if (pu != null)
                        {
                            pqRes = pq.ConvertTo(pu);
                            if (pqRes == null)
                            {
                                ResultLine = "The unit " + pq.Unit.ToPrintString() + " can't be converted to " + pu.ToPrintString();
                            }
                        }
                        else
                        {
                            ResultLine = "'" + UnitStringAll + "' is not a valid unit.";
                        }
                    }
                }
            }

            return pqRes;
        }

        public IPhysicalQuantity ParseExpression(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseTerm(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalExpression(pq, ref CommandLine, ref ResultLine);
            }
            else
            {
                ResultLine = "Physical quantity expected";
            }

            return pq;
        }

        public IPhysicalQuantity ParseOptionalExpression(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                if (TryParseToken("+", ref CommandLine))
                {
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Add(pq2);
                }
                else if (TryParseToken("-", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Subtract(pq2);
                }
            }

            return pqRes;
        }

        public IPhysicalQuantity ParseTerm(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseFactor(ref CommandLine, ref ResultLine);
            pq = ParseOptionalTerm(pq, ref CommandLine, ref ResultLine);

            return pq;
        }

        public IPhysicalQuantity ParseOptionalTerm(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                CommandLine = CommandLine.TrimStart();

                if (TryParseToken("*", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Multiply(pq2);
                }
                else if (TryParseToken("/", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Divide(pq2);
                }
            }

            return pqRes;
        }

        public IPhysicalQuantity ParseFactor(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq = null;
            CommandLine = CommandLine.TrimStart();

            if (String.IsNullOrEmpty(CommandLine))
            {
                // ResultLine = "Factor not found";
            }
            else if (Char.IsLetter(CommandLine[0]))
            {
                String VariableName;
                CommandLine = CommandLine.ReadIdentifier(out VariableName);
                Debug.Assert(VariableName != null);
                Boolean Found = VariableGet(VariableName, out pq);
                if (!Found)
                {
                    ResultLine = "Variable " + VariableName + " not found";
                }
            }
            else if (CommandLine[0] == '(')
            { // paranteses
                CommandLine = CommandLine.Substring(1).TrimStart(); // Skip start parantes '('

                pq = ParseExpression(ref CommandLine, ref ResultLine);

                CommandLine = CommandLine.TrimStart();
                ParseChar(')', ref CommandLine, ref ResultLine);
                CommandLine = CommandLine.TrimStart();
            }
            else
            {
                //NumberStyles styles = NumberStyles.Float;
                //IFormatProvider provider = NumberFormatInfo.InvariantInfo;

                // Scan number
                int numlen = 0;

                if ((CommandLine[0] == '-') || (CommandLine[0] == '+'))
                {
                    numlen = 1;
                }

                int maxlen = CommandLine.Length; // Max length of sign and digits to look for
                while (numlen < maxlen && Char.IsDigit(CommandLine[numlen]))
                {
                    numlen++;
                }
                if ((numlen < maxlen)
                    && ((CommandLine[numlen] == '.')
                        || (CommandLine[numlen] == ',')))
                {
                    numlen++;
                }
                while (numlen < maxlen && Char.IsDigit(CommandLine[numlen]))
                {
                    numlen++;
                }
                if ((numlen < maxlen)
                    && ((CommandLine[numlen] == 'E')
                        || (CommandLine[numlen] == 'e')))
                {
                    numlen++;
                    if ((numlen < maxlen)
                        && ((CommandLine[numlen] == '-')
                            || (CommandLine[numlen] == '+')))
                    {
                        numlen++;
                    }
                    while (numlen < maxlen && Char.IsDigit(CommandLine[numlen]))
                    {
                        numlen++;
                    }
                }

                if (numlen > 0)
                {
                    pq = new PhysicalQuantity(1, Physics.dimensionless);

                    pq.Value *= Double.Parse(CommandLine.Substring(0, numlen), System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo); // styles, provider
                    CommandLine = CommandLine.Substring(numlen);

                    if (!String.IsNullOrWhiteSpace(CommandLine))
                    {   // Parse optional unit
                        CommandLine = CommandLine.TrimStart();
                        IPhysicalUnit pu = PhysicalUnit.ParseUnit(ref CommandLine);
                        if (pu != null)
                        {
                            pq = pq.Multiply(pu);
                        }
                    }
                }

            }
            return pq;
        }
        ***/

        #endregion Expression parser methods
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
}

