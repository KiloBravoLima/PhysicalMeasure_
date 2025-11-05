using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using PhysicalCalculator.Expression;
using PhysicalCalculator.Identifiers;

using PhysicalMeasure;

using TokenParser;

namespace PhysicalCalculator.Function
{
    // Expression<Func<IQuantity, IQuantity, IQuantity>> lambda = (pq1, pq2) => pq1 * pq2;


    //public delegate IQuantity PhysicalQuantityFunction(...);
    public delegate IQuantity ZeroParamFunction();
    public delegate IQuantity UnaryFunction_PQ(IQuantity pq);
    public delegate IQuantity BinaryFunction_PQ_SB(IQuantity pq, SByte sb);
    public delegate IQuantity BinaryFunction_PQ_PQ(IQuantity pq1, IQuantity pq2);
    //public delegate IQuantity TernaryFunction_PQ_PQ(IQuantity pq1, IQuantity pq2, IQuantity pq3);

    abstract class PhysicalQuantityFunction : NametableItem, IFunctionEvaluator
    {
        override public IdentifierKind Identifierkind => IdentifierKind.Function;

        override public String ToListString(String Name) => String.Format($"Func {Name}({ParameterlistStr()}) // BuildIn ");

        public CalculatorEnvironment StaticOuterContext { get; }
        public CalculatorEnvironment DynamicOuterContext { get; }

        public Type ResultType { get; }

        public PhysicalQuantityFunction(CalculatorEnvironment staticOuterContext)
        {
            StaticOuterContext = staticOuterContext;
        }

        protected List<PhysicalQuantityFunctionParam> formalparamlist;

        public List<PhysicalQuantityFunctionParam> Parameterlist => formalparamlist;

        public void ParamListAdd(PhysicalQuantityFunctionParam param) 
        {
            if (formalparamlist == null)
            {
                formalparamlist = new List<PhysicalQuantityFunctionParam>();    
            }
            formalparamlist.Add(param);
        }

        public String ParameterlistStr()
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            if (formalparamlist != null)
            {
                int ParamCount = 0;
                foreach (PhysicalQuantityFunctionParam Param in Parameterlist)
                {
                    if (ParamCount > 0)
                    {
                        ListStringBuilder.Append(", ");
                    }
                    ListStringBuilder.AppendFormat("{0}", Param.Name );
                    if (Param.Unit != null)
                    {
                        ListStringBuilder.AppendFormat(" [{0}]", Param.Unit.ToString());
                    }
                    ParamCount++;
                }
                //ListStringBuilder.AppendLine();
            }
            return ListStringBuilder.ToString();
        }

        public Boolean CheckParams(List<OperandInfo> actualParameterlist, ref String resultLine)
        {
            int ParamCount = Parameterlist.Count;
            if (actualParameterlist.Count < ParamCount)
            {
                resultLine = "Missing parameter no " + (actualParameterlist.Count + 1).ToString() + " " + Parameterlist[actualParameterlist.Count].Name;
                return false;
            }

            if (ParamCount < actualParameterlist.Count)
            {
                resultLine = "Too many parameters specified in function call: " + actualParameterlist.Count + ". " + ParamCount + " parameters was expected";
                return false;
            }

            return true;
        }

        // abstract public Boolean Evaluate(CalculatorEnvironment localContext, List<Quantity> parameterlist, out T functionResult, ref String resultLine);
        abstract public Boolean Evaluate(CalculatorEnvironment localContext, List<OperandInfo> parameterlist, out OperandInfo functionResult, ref String resultLine);
        // abstract public Boolean Evaluate<T>(CalculatorEnvironment localContext, List<OperandInfo> parameterlist, out T functionResult, ref String resultLine);
    }


    class DateTimeZeroParamFunction : PhysicalQuantityFunction
    {
        Func<DateTime> F;

        public DateTimeZeroParamFunction(CalculatorEnvironment staticOuterContext, Func<DateTime> func)
            : base(staticOuterContext)
        {
            F = func;
        }

        virtual public Boolean Evaluate(CalculatorEnvironment localContext, out DateTime functionResult, ref String resultLine)
        {
            functionResult = F();
            return true;
        }

        public override bool Evaluate(CalculatorEnvironment localContext, List<OperandInfo> parameterList, out OperandInfo functionResult, ref string resultLine)
        {
            DateTime dateTimeObject;
            Boolean result = Evaluate(localContext, out dateTimeObject, ref resultLine);
            // functionResult = dateTimeObject;
            functionResult = new OperandInfo(dateTimeObject);
            return result;
        }
    

        /**
        override public Boolean Evaluate<T>(CalculatorEnvironment localContext, List<Quantity> dummy_parameterlist, out T functionResult, ref String resultLine)
        {
            DateTime dateTimeObject;
            Boolean result = Evaluate(localContext, out dateTimeObject, ref resultLine);
            functionResult = (T)(object)dateTimeObject;
            return result;
        }
        **/
    }

    class DateTimeParamFunction : PhysicalQuantityFunction
    {
        Func<String , DateTime> F;

        public DateTimeParamFunction(CalculatorEnvironment staticOuterContext, Func<String, DateTime> func)
            : base(staticOuterContext)
        {
            F = func;
        }

        virtual public Boolean Evaluate(CalculatorEnvironment localContext, in String dt, out DateTime functionResult, ref String resultLine)
        {
            functionResult = F(dt);
            return true;
        }

        public override bool Evaluate(CalculatorEnvironment localContext, List<OperandInfo> parameterList, out OperandInfo functionResult, ref string resultLine)
        {
            String dt = parameterList[0].AsString();  
            DateTime dateTimeObject;
            Boolean result = Evaluate(localContext, in dt, out dateTimeObject, ref resultLine);
            // functionResult = dateTimeObject;
            functionResult = new OperandInfo(dateTimeObject);
            return result;
        }


        /**
        override public Boolean Evaluate<T>(CalculatorEnvironment localContext, List<Quantity> dummy_parameterlist, out T functionResult, ref String resultLine)
        {
            DateTime dateTimeObject;
            Boolean result = Evaluate(localContext, out dateTimeObject, ref resultLine);
            functionResult = (T)(object)dateTimeObject;
            return result;
        }
        **/
    }

    class PhysicalQuantityZeroParameterFunction : PhysicalQuantityFunction
    {
        //UnaryFunction F;
        Func<Quantity> F;

        //public PhysicalQuantityUnaryFunction(UnaryFunction func)
        public PhysicalQuantityZeroParameterFunction(CalculatorEnvironment staticOuterContext, Func<Quantity> func)
            : base(staticOuterContext)
        {
            F = func;
        }

        virtual public Boolean Evaluate(CalculatorEnvironment localContext, out Quantity functionResult, ref String resultLine)
        {
            functionResult = F();
            return true;
        }


        public override bool Evaluate(CalculatorEnvironment localContext, List<OperandInfo> dummy_actualParameterlist, out OperandInfo functionResult, ref String resultLine)
        {
            Quantity quantityObject;
            Boolean result = Evaluate(localContext, out quantityObject, ref resultLine);
            functionResult = new OperandInfo(quantityObject);
            return result;
        }

        /**
        override public Boolean Evaluate<T>(CalculatorEnvironment localContext, List<Quantity> dummy_actualParameterlist, out T functionResult, ref String resultLine)
        {
            // return Evaluate(localContext, out functionResult, ref resultLine);
            Quantity quantityObject;
            Boolean result = Evaluate(localContext, out quantityObject, ref resultLine);
            functionResult = (T)(object)quantityObject;
            return result;
        }
        **/
    }

    class PhysicalQuantityFunction_PQ_SB : PhysicalQuantityFunction
    {
        //UnaryFunction F;
        Func<Quantity, SByte, Quantity> F;

        //public PhysicalQuantityUnaryFunction(UnaryFunction func)
        public PhysicalQuantityFunction_PQ_SB(CalculatorEnvironment staticOuterContext, Func<Quantity, SByte, Quantity> func)
            : base(staticOuterContext)
        {
            F = func;

            ParamListAdd(new PhysicalQuantityFunctionParam("PQ", null)); 
            ParamListAdd(new PhysicalQuantityFunctionParam("SB", null)); 
        }

        public PhysicalQuantityFunction_PQ_SB(CalculatorEnvironment staticOuterContext, Func<Quantity, SByte, Quantity> func, List<PhysicalQuantityFunctionParam> formalparams)
            : base(staticOuterContext)
        {
            F = func;
            formalparamlist = formalparams;
        }

        public override bool Evaluate(CalculatorEnvironment localContext, List<OperandInfo> actualParameterlist, out OperandInfo functionResult, ref string resultLine)
        {
            if (!CheckParams(actualParameterlist, ref resultLine))
            {
                functionResult = null;
                return false;
            }
            var param1 = actualParameterlist[0].OperandValue as Quantity;
            var param2 = (SByte)(actualParameterlist[1].OperandValue as Quantity).Value;
            functionResult = new OperandInfo(F(param1, param2));
            return true;
        }
    }


    class PhysicalQuantityBinaryFunction_PQ_PQ : PhysicalQuantityFunction
    {
        //BinaryFunction F;
        Func<Quantity, Quantity, Quantity> F;

        public PhysicalQuantityBinaryFunction_PQ_PQ(CalculatorEnvironment staticOuterContext, Func<Quantity, Quantity, Quantity> func)
            : base(staticOuterContext)
        {
            F = func;
        }

        public Boolean Evaluate(CalculatorEnvironment localContext, Quantity actualParameter1, Quantity actualParameter2, out Quantity functionResult, ref String resultLine)
        {
            functionResult = F(actualParameter1, actualParameter2);
            return true;
        }

        override public Boolean Evaluate(CalculatorEnvironment localContext, List<OperandInfo> actualParameterlist, out OperandInfo functionResult, ref String resultLine)
        {
            if (!CheckParams(actualParameterlist, ref resultLine))
            {
                functionResult = null;
                return false;
            }

            var param1 = actualParameterlist[0].OperandValue as Quantity;
            var param2 = actualParameterlist[1].OperandValue as Quantity;

            functionResult = new OperandInfo(F(param1, param2));
            return true;
        }
    }

    /*
    class PhysicalQuantityTernaryFunction : PhysicalQuantityFunction
    {
        TernaryFunction F;

        public PhysicalQuantityTernaryFunction(TernaryFunction func)
        {
            F = func;
        }

        public Boolean Evaluate(CalculatorEnvironment localContext, List<IQuantity> actualParameterlist, out IQuantity functionResult, ref String resultLine)
        {
            if (!CheckParams(actualParameterlist, ref resultLine))
            {
                functionResult = null;
                return false;
            }

            functionResult = F(actualParameterlist[0], actualParameterlist[1], actualParameterlist[2]);
            return true;
        }
    }
    */

    class PhysicalQuantityCommandsFunction : PhysicalQuantityFunction, IFunctionCommandsEvaluator 
    {
        //override public IdentifierKind Identifierkind { get { return IdentifierKind.Function; } }


        override public String ToListString(String name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendLine("//");
            if (Commands.Count <= 1)
            {
                // Single line func
                ListStringBuilder.AppendFormat("    Func {0}({1}) {{ {2} }}", name, ParameterlistStr(), Commands.Count > 0 ? Commands[0] : "");
            }
            else
            {
                // Multi line func
                ListStringBuilder.AppendFormat("    Func {0}({1})", name, ParameterlistStr());
                ListStringBuilder.AppendLine();
                ListStringBuilder.AppendLine("    {");
                foreach (String CommandLine in Commands)
                {
                    ListStringBuilder.AppendFormat("        {0}", CommandLine);
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.Append("    }");
                //ListStringBuilder.AppendLine();
            }
            return ListStringBuilder.ToString();
        }

        public PhysicalQuantityCommandsFunction(CalculatorEnvironment staticOuterContext)
            : base(staticOuterContext)
        {
        }

        private List<String> _commands = new List<String>(); 

        public List<String> Commands { get { return _commands; } set { _commands = value; } }

        /*
        public void AddCommandLine(String commandLine) 
        {
            if (_commands == null)
            {
                _commands = new List<string>();
            }
            _commands.Add(commandLine);
        }
        */


        override public Boolean Evaluate(CalculatorEnvironment localContext, List<OperandInfo> actualParameterlist, out OperandInfo functionResult, ref String resultLine)
        {
            if (PhysicalFunction.ExecuteCommandsCallback != null)
            {
                // Set params in local context
                int ParamIndex = 0;
                if (formalparamlist != null)
                {
                    foreach (PhysicalQuantityFunctionParam Param in formalparamlist)
                    {
                        if (actualParameterlist.Count <= ParamIndex)
                        {
                            resultLine = "Missing parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name;
                            functionResult = null;
                            return false;
                        }

                        OperandInfo paramValueOperand = actualParameterlist[ParamIndex];
                        Quantity paramValue = paramValueOperand.AsQuantity();
                        if (Param.Unit != null)
                        {
                            Quantity paramValueConverted = paramValue.ConvertTo(Param.Unit);
                            if (paramValueConverted == null)
                            {
                                resultLine = "Parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name + "  " + paramValue.ToString() + " has invalid unit.\nThe unit " + paramValue.Unit.ToPrintString() + " can't be converted to " + Param.Unit.ToPrintString();

                                functionResult = null;
                                return false;
                            }
                            else
                            {
                                paramValue = paramValueConverted;
                                paramValueOperand = new OperandInfo(paramValue);
                            }
                        }
                        // localContext.NamedItems.SetItem(Param.Name, new NamedVariable(paramValue));
                        localContext.NamedItems.SetItem(Param.Name, new NamedVariable(paramValueOperand));
                        ParamIndex++;
                    }
                }

                if (ParamIndex < actualParameterlist.Count)
                {
                    resultLine = "Too many parameters specified in function call: " + actualParameterlist.Count + ". " + ParamIndex + " parameters was expected";
                    functionResult = null;
                    return false;
                }
                // Run commands
                String FuncBodyResult = ""; // Dummy: Never used

                Boolean result = PhysicalFunction.ExecuteCommandsCallback(localContext, Commands, ref FuncBodyResult, out functionResult);
                return result;
            }
            else
            {
                if (Commands != null)
                {
                    resultLine = "Function call: PhysicalFunction.ExecuteCommandsCallback is null. Don't know how to evaluate function.";
                }
                functionResult = null;
                return false;
            }
        }
    }

    static class PhysicalFunction
    {
        #region Physical Function parser methods
        /**
            FUNC = FUNCNAME "(" PARAMLIST ")" "{" FUNCBODY "}" .         
            FUNCBODY = COMMANDS . 
            COMMANDS = COMMAND | COMMAND "\n" COMMANDS.
          
            FUNC = FUNCNAME "(" PARAMLIST ")" "{" FUNCBODY "}" .         
            FUNCBODY = COMMANDS . 
            COMMANDS = COMMAND COMMANDSopt . 
            COMMANDSopt = "\n" COMMANDS | e .
          
         **/

        public delegate Boolean ExecuteCommandsFunc(CalculatorEnvironment localContext, List<String> FuncBodyCommands, ref String funcBodyResult, out OperandInfo functionResult);

        public static ExecuteCommandsFunc ExecuteCommandsCallback;


        public static IFunctionEvaluator ParseFunctionDeclaration(CalculatorEnvironment localContext, ref String commandLine, ref String resultLine)
        {
            // FUNC = FUNCNAME "(" PARAMLIST ")" "{" FUNCBODY "}" .         

            // FUNC = FUNCNAME #1 "(" #2 PARAMLIST #3  ")" #4 "{" FUNCBODY "}" .         


            Boolean OK = true;
                
            if (localContext.ParseState == CommandParserState.ReadFunctionParameterList)    
            {
                if (commandLine.StartsWith("//"))
                {   // #1 
                    commandLine = null;
                    return null;
                }

                OK = TokenString.ParseChar('(', ref commandLine, ref resultLine);

                localContext.ParseState = CommandParserState.ReadFunctionParameters;
            }
            if (String.IsNullOrEmpty(commandLine))
            {
                return null;
            }

            if (   (   (   localContext.ParseState == CommandParserState.ReadFunctionParameters 
                        || localContext.ParseState == CommandParserState.ReadFunctionParametersOptional)
                    && !commandLine.StartsWith(")")) 
                || (localContext.ParseState == CommandParserState.ReadFunctionParameter))
            {
                Boolean MoreParamsToParse;
                do 
                {
                    if (commandLine.StartsWith("//"))
                    {   // #2
                        commandLine = null;
                        return null;
                    }

                    if (   (localContext.ParseState == CommandParserState.ReadFunctionParameters)
                        || (localContext.ParseState == CommandParserState.ReadFunctionParameter))
                    {
                        commandLine = commandLine.TrimStart();
                        PhysicalQuantityFunctionParam param = ParseFunctionParam(ref commandLine, ref resultLine);

                        OK &= param != null;
                        localContext.FunctionToParseInfo.Function.ParamListAdd(param);
                        localContext.ParseState = CommandParserState.ReadFunctionParametersOptional;

                        if (commandLine.StartsWith("//"))
                        {   // #3
                            commandLine = null;
                            return null;
                        }
                    }

                    MoreParamsToParse = TokenString.TryParseChar(',', ref commandLine);
                    if (MoreParamsToParse)
                    {
                        localContext.ParseState = CommandParserState.ReadFunctionParameter;
                    }

                } while (   OK 
                         && !String.IsNullOrEmpty(commandLine) 
                         && MoreParamsToParse);
            }

            if (OK && !String.IsNullOrEmpty(commandLine))
            {
                if (   (localContext.ParseState == CommandParserState.ReadFunctionParameters)
                    || (localContext.ParseState == CommandParserState.ReadFunctionParametersOptional))
                {
                    OK = TokenString.ParseChar(')', ref commandLine, ref resultLine);

                    if (OK)
                    {
                        localContext.ParseState = CommandParserState.ReadFunctionBlock;
                    }
                }
                if (OK)
                {
                    if (!String.IsNullOrEmpty(commandLine))
                    {
                        if (localContext.ParseState == CommandParserState.ReadFunctionBlock)
                        {
                            if (commandLine.StartsWith("//"))
                            {   // #4
                                commandLine = null;
                                return null;
                            }

                            OK = TokenString.ParseChar('{', ref commandLine, ref resultLine);
                            if (OK)
                            {
                                localContext.ParseState = CommandParserState.ReadFunctionBody;
                                localContext.CommandBlockLevel = 1;
                            }
                        }
                        if (localContext.ParseState == CommandParserState.ReadFunctionBody)
                        {
                            if (!String.IsNullOrEmpty(commandLine))
                            {

                                String TempcommandLine = commandLine;
                                int indexStartComment = commandLine.IndexOf("//");
                                if (indexStartComment >= 0)
                                {   // Command are terminated by "//"
                                    TempcommandLine = TempcommandLine.Substring(0, indexStartComment);
                                }

                                string NoCommentCommandLine = TempcommandLine;

                                int indexCommandBlockBegin = TempcommandLine.IndexOf('{');
                                int indexCommandBlockEnd = TempcommandLine.IndexOf('}');

                                while (localContext.CommandBlockLevel > 0 && (indexCommandBlockBegin >= 0 || indexCommandBlockEnd >= 0))
                                {
                                    Boolean beginIsBeforeEnd = (indexCommandBlockBegin >= 0 && (indexCommandBlockEnd < 0 || indexCommandBlockEnd > indexCommandBlockBegin));
                                    if (beginIsBeforeEnd)
                                    {
                                        localContext.CommandBlockLevel++;
                                        int skipLen = indexCommandBlockBegin + 1;
                                        TempcommandLine = TempcommandLine.Length > skipLen ? TempcommandLine.Substring(skipLen) : "";
                                        indexCommandBlockBegin = TempcommandLine.IndexOf('{');
                                        indexCommandBlockEnd -= skipLen;
                                    }
                                    else
                                    {
                                        localContext.CommandBlockLevel--;
                                        int skipLen = indexCommandBlockEnd;
                                        if (localContext.CommandBlockLevel > 0)
                                        {   // Include '}' if it is not the end of this function block
                                            skipLen++;
                                        }
                                        TempcommandLine = TempcommandLine.Length > skipLen ? TempcommandLine.Substring(skipLen) : "";
                                        indexCommandBlockBegin -= skipLen;
                                        indexCommandBlockEnd = TempcommandLine.IndexOf('}');
                                    }
                                };

                                int indexCommandEnd = -1;
                                if (localContext.CommandBlockLevel > 0)
                                {   // function block are not terminated by '}'
                                    // Whole commandLine is part of Command Block 
                                    indexCommandEnd = commandLine.Length;
                                }
                                else
                                {
                                    indexCommandEnd = NoCommentCommandLine.Length - TempcommandLine.Length;
                                }

                                if (indexCommandEnd > 0)
                                {
                                    if (localContext.FunctionToParseInfo.Function.Commands == null)
                                    {
                                        // Will not work; Commands are ReadOnly here: localContext.FunctionToParseInfo.Function.Commands = new List<String>();
                                    }
                                    Debug.Assert(localContext.FunctionToParseInfo.Function.Commands != null);

                                    localContext.FunctionToParseInfo.Function.Commands.Add(commandLine.Substring(0, indexCommandEnd));
                                    commandLine = commandLine.Substring(indexCommandEnd);
                                }

                                if (localContext.CommandBlockLevel == 0)
                                {
                                    OK = TokenString.ParseChar('}', ref commandLine, ref resultLine);
                                    if (OK)
                                    {   // Completed function declaration parsing 
                                        localContext.ParseState = CommandParserState.ExecuteCommandLine;
                                        return localContext.FunctionToParseInfo.Function;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static PhysicalQuantityFunctionParam ParseFunctionParam(ref String commandLine, ref String resultLine)
        {
            commandLine = commandLine.ReadIdentifier(out var ParamName);
            Debug.Assert(ParamName != null);

            Unit ParamUnit = PhysicalCalculator.Expression.PhysicalExpression.ParseOptionalConvertToUnit(ref commandLine, ref resultLine);

            PhysicalQuantityFunctionParam param = new PhysicalQuantityFunctionParam(ParamName, ParamUnit);

            return param;
        }
          
        #endregion Physical Expression parser methods
    }
}
