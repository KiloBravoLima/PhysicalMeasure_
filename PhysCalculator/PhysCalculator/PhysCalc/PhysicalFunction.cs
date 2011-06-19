using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using TokenParser;

using CommandParser;

using PhysicalMeasure;
using PhysicalCalculator.Identifers;
using PhysicalCalculator.Expression;

namespace PhysicalCalculator.Function
{
    class PhysicalQuantityFunction : IFunctionEvaluator
    {
        public IdentifierKind identifierkind { get { return IdentifierKind.function; } }

        public String ToListString(String Name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendLine("//");
            if (Commands.Count <= 1)
            {
                // Single line func
                ListStringBuilder.AppendFormat("Func {0}({1}) {{ {2} }}", Name, ParamlistStr(), Commands.Count > 0 ? Commands[0] : "");
            }
            else
            {
                // Multi line func
                ListStringBuilder.AppendFormat("Func {0}({1})", Name, ParamlistStr());
                ListStringBuilder.AppendLine();
                ListStringBuilder.AppendLine("{");
                foreach (String CommandLine in Commands)
                {
                    ListStringBuilder.AppendFormat("\t{0}", CommandLine);
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.Append("}");
                //ListStringBuilder.AppendLine();
            }
            return ListStringBuilder.ToString();
        }

        public void WriteToTextFile(String Name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(Name));
        }


        private List<PhysicalQuantityFunctionParam> formalparamlist;

        private List<String> _commands;

        public List<PhysicalQuantityFunctionParam> Paramlist { get { return formalparamlist; } }
        public List<String> Commands { get { return _commands; } set { _commands = value; } }

        public void ParamListAdd(PhysicalQuantityFunctionParam param) 
        {
            if (formalparamlist == null)
            {
                formalparamlist = new List<PhysicalQuantityFunctionParam>();    
            }
            formalparamlist.Add(param);
        }

        public String ParamlistStr()
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            if (formalparamlist != null)
            {
                int ParamCount = 0;
                foreach (PhysicalQuantityFunctionParam Param in Paramlist)
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

        public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            if (PhysicalFunction.ExecuteCommandsCallback != null)
            {
                // Set params in local context
                int ParamIndex = 0;
                foreach (PhysicalQuantityFunctionParam Param in Paramlist)
                {
                    if (parameterlist.Count <= ParamIndex)
                    {
                        ResultLine = "Missing parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name;
                        FunctionResult = null;
                        return false;
                    }

                    IPhysicalQuantity paramValue = parameterlist[ParamIndex];
                    if (Param.Unit != null)
                    {
                        IPhysicalQuantity paramValueConverted = paramValue.ConvertTo(Param.Unit);
                        if (paramValueConverted == null)
                        {
                            ResultLine = "Parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name + "  " + paramValue.ToString() + " has invalid unit.\nThe unit " + paramValue.Unit.ToPrintString() + " can't be converted to " + Param.Unit.ToPrintString();

                            FunctionResult = null;
                            return false;
                        }
                        else
                        {
                            paramValue = paramValueConverted;
                        }
                    }
                    LocalContext.NamedItems.SetItem(Param.Name, new NamedVariable(paramValue));
                    ParamIndex++;
                }

                if (ParamIndex < parameterlist.Count)
                {
                    ResultLine = "Too many parameters specified in function call: " + parameterlist.Count + ". " + ParamIndex + " parameters was expected";
                    FunctionResult = null;
                    return false;
                }
                // Run commands
                String FuncBodyResult = ""; // Dummy: Never used
                return PhysicalFunction.ExecuteCommandsCallback(LocalContext, Commands, ref FuncBodyResult, out FunctionResult);
            }
            else
            {
                if (Commands != null)
                {
                    ResultLine = "Function call: PhysicalFunction.ExecuteCommandsCallback is null. Don't know how to evaluate function.";
                }
                FunctionResult = null;
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

        public delegate Boolean ExecuteCommandsFunc(CalculatorEnviroment LocalContext, List<String> FuncBodyCommands, ref String FuncBodyResult, out IPhysicalQuantity FunctionResult);

        public static ExecuteCommandsFunc ExecuteCommandsCallback;


        public static IFunctionEvaluator ParseFunctionDecleration(CalculatorEnviroment LocalContext, ref String CommandLine, ref String ResultLine)
        {
            // FUNC = FUNCNAME "(" PARAMLIST ")" "{" FUNCBODY "}" .         

            Boolean ParseFunctionCompleated = false;

            Boolean OK = true;
                
            if (LocalContext.ParseState == CommandPaserState.readfunctionparamlist)    
            {
                OK = TokenString.ParseToken("(", ref CommandLine, ref ResultLine);

                LocalContext.ParseState = CommandPaserState.readfunctionparams;
            }
            if (String.IsNullOrEmpty(CommandLine))
            {
                return null;
            }

            if (   (   (   LocalContext.ParseState == CommandPaserState.readfunctionparams 
                        || LocalContext.ParseState == CommandPaserState.readfunctionparamsopt)
                    && !CommandLine.StartsWith(")")) 
                || (LocalContext.ParseState == CommandPaserState.readfunctionparam))
            {
                Boolean MoreParamsToParse;
                do 
                {
                    if (   (LocalContext.ParseState == CommandPaserState.readfunctionparams)
                        || (LocalContext.ParseState == CommandPaserState.readfunctionparam))
                    {

                        PhysicalQuantityFunctionParam param = ParseFunctionParam(ref CommandLine, ref ResultLine);

                        OK &= param != null;
                        LocalContext.FunctionToParse.ParamListAdd(param);
                        LocalContext.ParseState = CommandPaserState.readfunctionparamsopt;
                    }

                    MoreParamsToParse = TokenString.TryParseToken(",", ref CommandLine);
                    if (MoreParamsToParse)
                    {
                        LocalContext.ParseState = CommandPaserState.readfunctionparam;
                    }

                } while (   OK 
                         && !String.IsNullOrEmpty(CommandLine) 
                         && MoreParamsToParse);
            }

            if (OK && !String.IsNullOrEmpty(CommandLine))
            {
                if (   (LocalContext.ParseState == CommandPaserState.readfunctionparams)
                    || (LocalContext.ParseState == CommandPaserState.readfunctionparamsopt))
                {
                    OK = TokenString.ParseToken(")", ref CommandLine, ref ResultLine);

                    if (OK)
                    {
                        LocalContext.ParseState = CommandPaserState.readfunctionblock;
                    }
                }
                if (OK)
                {
                    if (!String.IsNullOrEmpty(CommandLine))
                    {
                        if (LocalContext.ParseState == CommandPaserState.readfunctionblock)
                        {
                            OK = TokenString.ParseToken("{", ref CommandLine, ref ResultLine);
                            if (OK)
                            {
                                LocalContext.ParseState = CommandPaserState.readfunctionbody;
                            }
                        }
                        if (LocalContext.ParseState == CommandPaserState.readfunctionbody)
                        {
                            if (!String.IsNullOrEmpty(CommandLine))
                            {

                                int index = CommandLine.IndexOf('}');
                                if (index == -1)
                                {   // Not terminated by '}', but handle that later
                                    // Use rest of CommandLine as a Command Block 
                                    index = CommandLine.Length;
                                }

                                if (index > 0)
                                {
                                    if (LocalContext.FunctionToParse.Commands == null)
                                    {
                                        LocalContext.FunctionToParse.Commands = new List<String>();
                                    }
                                    LocalContext.FunctionToParse.Commands.Add(CommandLine.Substring(0, index));
                                    CommandLine = CommandLine.Substring(index);
                                }

                                if (!String.IsNullOrEmpty(CommandLine))
                                {
                                    OK = TokenString.ParseToken("}", ref CommandLine, ref ResultLine);
                                    if (OK)
                                    {   // Compleated function declaration parsing 
                                        ParseFunctionCompleated = true;
                                        LocalContext.ParseState = CommandPaserState.executecommandline;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!ParseFunctionCompleated)
            {
                return null;
            }
            return LocalContext.FunctionToParse;
        }

        public static PhysicalQuantityFunctionParam ParseFunctionParam(ref String CommandLine, ref String ResultLine)
        {
            String ParamName;
            CommandLine = CommandLine.ReadIdentifier(out ParamName);
            Debug.Assert(ParamName != null);

            IPhysicalUnit ParamUnit = PhysicalCalculator.Expression.PhysicalExpression.ParseOptionalConvertToUnit(ref CommandLine, ref ResultLine);

            PhysicalQuantityFunctionParam param = new PhysicalQuantityFunctionParam(ParamName, ParamUnit);

            return param;
        }
          
        #endregion Physical Expression parser methods
    }
}
