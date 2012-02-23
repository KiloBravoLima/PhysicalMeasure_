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
using PhysicalCalculator.Identifers;
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
        override public IdentifierKind identifierkind { get { return IdentifierKind.function; } }

        override public String ToListString(String Name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendFormat("Func {0}({1}) // BuildIn ", Name, ParamlistStr());
            return ListStringBuilder.ToString();
        }

        protected List<PhysicalQuantityFunctionParam> formalparamlist;

        public List<PhysicalQuantityFunctionParam> Paramlist { get { return formalparamlist; } }

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

        public Boolean CheckParams(List<IPhysicalQuantity> parameterlist, ref String ResultLine)
        {
            int ParamCount = Paramlist.Count;
            if (parameterlist.Count < ParamCount)
            {
                ResultLine = "Missing parameter no " + (parameterlist.Count + 1).ToString() + " " + Paramlist[parameterlist.Count].Name;
                return false;
            }

            if (ParamCount < parameterlist.Count)
            {
                ResultLine = "Too many parameters specified in function call: " + parameterlist.Count + ". " + ParamCount + " parameters was expected";
                return false;
            }

            return true;
        }

        abstract public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine);
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

        override public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {

            if (!CheckParams(parameterlist, ref ResultLine))
            {
                FunctionResult = null;
                return false;
            }

            //FunctionResult = this.FF(parameterlist[0]);
            FunctionResult = this.FF(parameterlist);
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


        override public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            if (!CheckParams(parameterlist, ref ResultLine)) 
            {
                FunctionResult = null;
                return false;
            }

            FunctionResult = F(parameterlist[0], (SByte)parameterlist[1].Value);
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

        override public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            if (!CheckParams(parameterlist, ref ResultLine))
            {
                FunctionResult = null;
                return false;
            }

            FunctionResult = F(parameterlist[0], parameterlist[1]);
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

        public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            if (!CheckParams(parameterlist, ref ResultLine))
            {
                FunctionResult = null;
                return false;
            }

            FunctionResult = F(parameterlist[0], parameterlist[1], parameterlist[2]);
            return true;
        }
    }
    */

    class PhysicalQuantityCommandsFunction : PhysicalQuantityFunction, ICommandsEvaluator 
    {
        //override public IdentifierKind identifierkind { get { return IdentifierKind.function; } }

        override public String ToListString(String Name)
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


        private List<String> _commands;

        public List<String> Commands { get { return _commands; } set { _commands = value; } }

        override public Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
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


        public static IFunctionEvaluator ParseFunctionDeclaration(CalculatorEnviroment LocalContext, ref String CommandLine, ref String ResultLine)
        {
            // FUNC = FUNCNAME "(" PARAMLIST ")" "{" FUNCBODY "}" .         

            // FUNC = FUNCNAME #1 "(" #2 PARAMLIST #3  ")" #4 "{" FUNCBODY "}" .         


            Boolean OK = true;
                
            if (LocalContext.ParseState == CommandPaserState.readfunctionparamlist)    
            {
                if (CommandLine.StartsWith("//"))
                {   // #1 
                    CommandLine = null;
                    return null;
                }

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
                    if (CommandLine.StartsWith("//"))
                    {   // #2
                        CommandLine = null;
                        return null;
                    }

                    if (   (LocalContext.ParseState == CommandPaserState.readfunctionparams)
                        || (LocalContext.ParseState == CommandPaserState.readfunctionparam))
                    {
                        PhysicalQuantityFunctionParam param = ParseFunctionParam(ref CommandLine, ref ResultLine);

                        OK &= param != null;
                        LocalContext.FunctionToParseInfo.Function.ParamListAdd(param);
                        LocalContext.ParseState = CommandPaserState.readfunctionparamsopt;

                        if (CommandLine.StartsWith("//"))
                        {   // #3
                            CommandLine = null;
                            return null;
                        }
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
                            if (CommandLine.StartsWith("//"))
                            {   // #4
                                CommandLine = null;
                                return null;
                            }

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

                                int indexCommandEnd = CommandLine.IndexOf('}');
                                if (indexCommandEnd == -1)
                                {   // Line are not terminated by '}'
                                    // Whole CommandLine is part of Command Block 
                                    indexCommandEnd = CommandLine.Length;
                                }
                                else
                                {
                                    int indexStartComment = CommandLine.IndexOf("//");
                                    if (indexStartComment >= 0)
                                    {   // Command are terminated by "//"
                                        if (indexCommandEnd > indexStartComment)
                                        {
                                            // '}' is placed inside a comment
                                            // Whole CommandLine is part of Command Block 

                                            indexCommandEnd = CommandLine.Length;
                                        }
                                    }
                                    else
                                    {
                                        // Debug brak point
                                        int test = indexStartComment;
                                    }
                                }

                                if (indexCommandEnd > 0)
                                {
                                    if (LocalContext.FunctionToParseInfo.Function.Commands == null)
                                    {
                                        LocalContext.FunctionToParseInfo.Function.Commands = new List<String>();
                                    }
                                    LocalContext.FunctionToParseInfo.Function.Commands.Add(CommandLine.Substring(0, indexCommandEnd));
                                    CommandLine = CommandLine.Substring(indexCommandEnd);
                                }

                                if (!String.IsNullOrEmpty(CommandLine))
                                {
                                    OK = TokenString.ParseToken("}", ref CommandLine, ref ResultLine);
                                    if (OK)
                                    {   // Compleated function declaration parsing 
                                        LocalContext.ParseState = CommandPaserState.executecommandline;
                                        return LocalContext.FunctionToParseInfo.Function;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
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
