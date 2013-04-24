using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using System.Text;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using TokenParser;

using CommandParser;

using PhysicalMeasure;
using PhysicalCalculator.Identifiers;
using PhysicalCalculator.Expression;

namespace PhysicalCalculator.Function
{
    // Expression<Func<IPhysicalQuantity, IPhysicalQuantity, IPhysicalQuantity>> lambda = (pq1, pq2) => pq1 * pq2;


    //public delegate IPhysicalQuantity PhysicalQuantityFunction(...);
    public delegate IPhysicalQuantity ZeroParamFunction();
    public delegate IPhysicalQuantity UnaryFunction_PQ(IPhysicalQuantity pq);
    public delegate IPhysicalQuantity BinaryFunction_PQ_SB(IPhysicalQuantity pq, SByte sb);
    public delegate IPhysicalQuantity BinaryFunction_PQ_PQ(IPhysicalQuantity pq1, IPhysicalQuantity pq2);
    //public delegate IPhysicalQuantity TernaryFunction_PQ_PQ(IPhysicalQuantity pq1, IPhysicalQuantity pq2, IPhysicalQuantity pq3);

    abstract class PhysicalQuantityFunction : NametableItem, IFunctionEvaluator
    {
        override public IdentifierKind Identifierkind { get { return IdentifierKind.Function; } }

        override public String ToListString(String Name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendFormat("Func {0}({1}) // BuildIn ", Name, ParameterlistStr());
            return ListStringBuilder.ToString();
        }

        protected List<PhysicalQuantityFunctionParam> formalparamlist;

        public List<PhysicalQuantityFunctionParam> Parameterlist { get { return formalparamlist; } }

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

        public Boolean CheckParams(List<IPhysicalQuantity> actualParameterlist, ref String resultLine)
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

        abstract public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity functionResult, ref String resultLine);
    }


    /**
    //class PhysicalQuantityFunction<TFunc> : PhysicalQuantityFunction where TFunc : LambdaExpression
    //class PhysicalQuantityFunction<TFunc> : PhysicalQuantityFunction where TFunc : Expression<Func<PhysicalQuantity>>
    //class PhysicalQuantityFunction<TFunc> : PhysicalQuantityFunction where TFunc : Func<PhysicalQuantity>
    class PhysicalQuantityFunction<TFunc> : PhysicalQuantityFunction where TFunc : IInvocable
    {
        private TFunc FF;

        //public BuildInPhysicalQuantityFunction<TFunc>(TFunc func)
        public PhysicalQuantityFunction(TFunc func)
        {
            this.FF = func;
        }

        override public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> actualParameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
        {

            if (!CheckParams(actualParameterlist, ref resultLine))
            {
                functionResult = null;
                return false;
            }

            //functionResult = this.FF(parameterlist[0]);
            functionResult = this.FF(actualParameterlist);
            return true;
        }
    }
    **/

    class PhysicalQuantityFunction_PQ_SB : PhysicalQuantityFunction
    {
        //UnaryFunction F;
        Func<IPhysicalQuantity, SByte, IPhysicalQuantity> F;

        //public PhysicalQuantityUnaryFunction(UnaryFunction func)
        public PhysicalQuantityFunction_PQ_SB(Func<IPhysicalQuantity, SByte, IPhysicalQuantity> func)
        {
            F = func;

            ParamListAdd(new PhysicalQuantityFunctionParam("PQ", null)); 
            ParamListAdd(new PhysicalQuantityFunctionParam("SB", null)); 
        }

        public PhysicalQuantityFunction_PQ_SB(Func<IPhysicalQuantity, SByte, IPhysicalQuantity> func, List<PhysicalQuantityFunctionParam> formalparams)
        {
            F = func;
            formalparamlist = formalparams;
        }


        override public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> actualParameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
        {
            if (!CheckParams(actualParameterlist, ref resultLine)) 
            {
                functionResult = null;
                return false;
            }

            functionResult = F(actualParameterlist[0], (SByte)actualParameterlist[1].Value);
            return true;
        }
    }


    class PhysicalQuantityBinaryFunction_PQ_PQ : PhysicalQuantityFunction
    {
        //BinaryFunction F;
        Func<IPhysicalQuantity, IPhysicalQuantity, IPhysicalQuantity> F;

        public PhysicalQuantityBinaryFunction_PQ_PQ(Func<IPhysicalQuantity, IPhysicalQuantity, IPhysicalQuantity> func)
        {
            F = func;
        }

        override public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> actualParameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
        {
            if (!CheckParams(actualParameterlist, ref resultLine))
            {
                functionResult = null;
                return false;
            }

            functionResult = F(actualParameterlist[0], actualParameterlist[1]);
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

        public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> actualParameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
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

    class PhysicalQuantityCommandsFunction : PhysicalQuantityFunction, ICommandsEvaluator 
    {
        //override public IdentifierKind Identifierkind { get { return IdentifierKind.Function; } }

        override public String ToListString(String name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendLine("//");
            if (Commands.Count <= 1)
            {
                // Single line func
                ListStringBuilder.AppendFormat("Func {0}({1}) {{ {2} }}", name, ParameterlistStr(), Commands.Count > 0 ? Commands[0] : "");
            }
            else
            {
                // Multi line func
                ListStringBuilder.AppendFormat("Func {0}({1})", name, ParameterlistStr());
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


        private List<String> _commands;

        public List<String> Commands { get { return _commands; } set { _commands = value; } }

        override public Boolean Evaluate(CalculatorEnvironment localContext, List<IPhysicalQuantity> actualParameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
        {
            if (PhysicalFunction.ExecuteCommandsCallback != null)
            {
                // Set params in local context
                int ParamIndex = 0;
                foreach (PhysicalQuantityFunctionParam Param in formalparamlist)
                {
                    if (actualParameterlist.Count <= ParamIndex)
                    {
                        resultLine = "Missing parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name;
                        functionResult = null;
                        return false;
                    }

                    IPhysicalQuantity paramValue = actualParameterlist[ParamIndex];
                    if (Param.Unit != null)
                    {
                        IPhysicalQuantity paramValueConverted = paramValue.ConvertTo(Param.Unit);
                        if (paramValueConverted == null)
                        {
                            resultLine = "Parameter no " + (ParamIndex + 1).ToString() + " " + Param.Name + "  " + paramValue.ToString() + " has invalid unit.\nThe unit " + paramValue.Unit.ToPrintString() + " can't be converted to " + Param.Unit.ToPrintString();

                            functionResult = null;
                            return false;
                        }
                        else
                        {
                            paramValue = paramValueConverted;
                        }
                    }
                    localContext.NamedItems.SetItem(Param.Name, new NamedVariable(paramValue));
                    ParamIndex++;
                }

                if (ParamIndex < actualParameterlist.Count)
                {
                    resultLine = "Too many parameters specified in function call: " + actualParameterlist.Count + ". " + ParamIndex + " parameters was expected";
                    functionResult = null;
                    return false;
                }
                // Run commands
                String FuncBodyResult = ""; // Dummy: Never used
                return PhysicalFunction.ExecuteCommandsCallback(localContext, Commands, ref FuncBodyResult, out functionResult);
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

        public delegate Boolean ExecuteCommandsFunc(CalculatorEnvironment localContext, List<String> FuncBodyCommands, ref String funcBodyResult, out IPhysicalQuantity functionResult);

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

                OK = TokenString.ParseToken("(", ref commandLine, ref resultLine);

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

                    MoreParamsToParse = TokenString.TryParseToken(",", ref commandLine);
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
                    OK = TokenString.ParseToken(")", ref commandLine, ref resultLine);

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

                            OK = TokenString.ParseToken("{", ref commandLine, ref resultLine);
                            if (OK)
                            {
                                localContext.ParseState = CommandParserState.ReadFunctionBody;
                            }
                        }
                        if (localContext.ParseState == CommandParserState.ReadFunctionBody)
                        {
                            if (!String.IsNullOrEmpty(commandLine))
                            {
                                int indexCommandEnd = commandLine.IndexOf('}');
                                if (indexCommandEnd == -1)
                                {   // Line are not terminated by '}'
                                    // Whole commandLine is part of Command Block 
                                    indexCommandEnd = commandLine.Length;
                                }
                                else
                                {
                                    int indexStartComment = commandLine.IndexOf("//");
                                    if (indexStartComment >= 0)
                                    {   // Command are terminated by "//"
                                        if (indexCommandEnd > indexStartComment)
                                        {
                                            // '}' is placed inside a comment
                                            // Whole commandLine is part of Command Block 

                                            indexCommandEnd = commandLine.Length;
                                        }
                                    }
                                }

                                if (indexCommandEnd > 0)
                                {
                                    if (localContext.FunctionToParseInfo.Function.Commands == null)
                                    {
                                        localContext.FunctionToParseInfo.Function.Commands = new List<String>();
                                    }
                                    localContext.FunctionToParseInfo.Function.Commands.Add(commandLine.Substring(0, indexCommandEnd));
                                    commandLine = commandLine.Substring(indexCommandEnd);
                                }

                                if (!String.IsNullOrEmpty(commandLine))
                                {
                                    OK = TokenString.ParseToken("}", ref commandLine, ref resultLine);
                                    if (OK)
                                    {   // Compleated function declaration parsing 
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
            String ParamName;
            commandLine = commandLine.ReadIdentifier(out ParamName);
            Debug.Assert(ParamName != null);

            IPhysicalUnit ParamUnit = PhysicalCalculator.Expression.PhysicalExpression.ParseOptionalConvertToUnit(ref commandLine, ref resultLine);

            PhysicalQuantityFunctionParam param = new PhysicalQuantityFunctionParam(ParamName, ParamUnit);

            return param;
        }
          
        #endregion Physical Expression parser methods
    }
}
