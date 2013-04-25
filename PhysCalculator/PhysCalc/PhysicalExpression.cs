﻿﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using TokenParser;

using PhysicalMeasure;

using PhysicalCalculator.Identifiers;
using PhysicalCalculator.Function;

namespace PhysicalCalculator.Expression
{

    public interface IEnvironment
    {
        TraceLevels OutputTracelevel { get; set; }
        FormatProviderKind FormatProviderSource { get; set; }

        CultureInfo CurrentCultureInfo { get; }

        Boolean SetLocalIdentifier(String identifierName, INametableItem item);
        Boolean RemoveLocalIdentifier(String identifierName);

        Boolean FindIdentifier(String identifierName, out IEnvironment foundInContext, out INametableItem item);

        Boolean SystemSet(String systemName, out INametableItem systemItem);

        Boolean UnitSet(IUnitSystem unitSystem, String unitName, IPhysicalQuantity unitValue, out INametableItem unitItem);

        Boolean VariableGet(String variableName, out IPhysicalQuantity variableValue, ref String resultLine);
        Boolean VariableSet(String variableName, IPhysicalQuantity variableValue);

        Boolean FunctionFind(String functionName, out IFunctionEvaluator functionEvaluator);
    }

    static class PhysicalExpression 
    {
        #region Physical Expression parser methods
        /**
            CE = E | E [ SYS ] .
            E = E "+" T | E "-" T | T .
            T = T "*" F | T "/" F | F .
            F = PE | PE ^ SN .         
            PE = PQ | UE | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .         
            PQ = num SU . 
            SU = sU | U .
            SYS = sys | sys "." SU | SU .
            UE = + F | - F .
            SN = + num | - num | num .
            EXPLIST = E | E "," EXPLIST .
          
         
            CE = E CEopt .
            Eopt = [ SYS ] | e .
            E = T Eopt .
            Eopt = "+" T Eopt | "-" T Eopt | e .
          
     *      T = F Topt .
     *      Topt = "*" F Topt | "/" F Topt | e .

     **     T1 = T2 T1opt .
     **     T1opt = "*" T2 T1opt | e .
     **     T2 = F T2opt .
     **     T2opt = "/" F T2opt | e .
         
            F = PE Fopt .
            PE = PQ | UE | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .
            Fopt = ^ SN | e .
            PQ = num SU . 
            SU = sU | U | e .
            SYS = SYST | SU .
            SYST = system SYSTopt .
            SN = Sopt num .
            Sopt = + | - | e .
            SYSTopt = "." SU | e .
            UE =  + F | - F .
            EXPLIST = E EXPLISTopt . 
            EXPLISTopt = "," EXPLIST | e .
          
         **/

        // Delegert types
        // VariableLookup callback
        public delegate Boolean IdentifierItemLookupFunc(String identifierName, out IEnvironment foundInContext, out INametableItem item);
        public delegate Boolean IdentifierContextLookupFunc(String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);
        public delegate Boolean QualifiedIdentifierContextLookupFunc(IEnvironment lookInContext, String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);

        public delegate Boolean VariableValueLookupFunc(IEnvironment lookInContext, String variableName, out IPhysicalQuantity variableValue, ref String resultLine);
        public delegate Boolean FunctionLookupFunc(IEnvironment lookInContext, String functionName, out IFunctionEvaluator functionEvaluator);
        public delegate Boolean FunctionEvaluateFunc(String functionName, IFunctionEvaluator functionevaluator, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity functionResult, ref String resultLine);
        public delegate Boolean FunctionEvaluateFileReadFunc(String functionName, out IPhysicalQuantity functionResult, ref String resultLine);
        
        // Delegert static globals
        public static IdentifierItemLookupFunc IdentifierItemLookupCallback;
        public static IdentifierContextLookupFunc IdentifierContextLookupCallback;
        public static QualifiedIdentifierContextLookupFunc QualifiedIdentifierContextLookupCallback;

        public static VariableValueLookupFunc VariableValueGetCallback;
        public static FunctionLookupFunc FunctionLookupCallback;
        public static FunctionEvaluateFunc FunctionEvaluateCallback;
        public static FunctionEvaluateFileReadFunc FunctionEvaluateFileReadCallback;

        // static access functions

        public static Boolean IdentifierItemLookup(String identifierName, out IEnvironment foundInContext, out INametableItem item, ref String resultLine)
        {
            item = null;
            foundInContext = null;
            if (IdentifierItemLookupCallback != null)
            {
                return IdentifierItemLookupCallback(identifierName, out foundInContext, out item);
            }
            return false;
        }

        public static Boolean IdentifierContextLookup(String variableName, out IEnvironment foundInContext, out IdentifierKind identifierKind, ref String resultLine)
        {
            identifierKind = IdentifierKind.Unknown;
            foundInContext = null;
            if (IdentifierContextLookupCallback != null)
            {
                return IdentifierContextLookupCallback(variableName, out foundInContext, out identifierKind);
            }
            return false;
        }

        public static Boolean QualifiedIdentifierContextLookup(IEnvironment lookInContext, String variableName, out IEnvironment foundInContext, out IdentifierKind identifierKind, ref String resultLine)
        {
            identifierKind = IdentifierKind.Unknown;
            foundInContext = null;
            if (QualifiedIdentifierContextLookupCallback != null)
            {
                return QualifiedIdentifierContextLookupCallback(lookInContext, variableName, out foundInContext, out identifierKind);
            }
            return false;
        }

        public static Boolean VariableGet(IEnvironment lookInContext, String variableName, out IPhysicalQuantity variableValue, ref String resultLine)
        {
            variableValue = null;
            if (VariableValueGetCallback != null)
            {
                return VariableValueGetCallback(lookInContext, variableName, out variableValue, ref resultLine);
            }
            return false;
        }

        public static Boolean FunctionGet(IEnvironment lookInContext, String functionName, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity functionResult, ref String resultLine)
        {
            functionResult = null;
            IFunctionEvaluator functionevaluator;
            if (FunctionLookupCallback(lookInContext, functionName, out functionevaluator))
            {
                if (FunctionEvaluateCallback != null)
                {
                    return FunctionEvaluateCallback(functionName, functionevaluator, parameterlist, out functionResult, ref resultLine);
                }
                else
                {
                    resultLine = "Internal error: No FunctionEvaluateCallback handler specified";
                }
            }
            else
            {
                resultLine = "Internal error: FunctionLookupCallback failed";
            }

            return false;
        }

        public static Boolean FileFunctionGet(String functionName, out IPhysicalQuantity functionResult, ref String resultLine)
        {
            functionResult = null;
            if (FunctionEvaluateFileReadCallback != null)
            {
                return FunctionEvaluateFileReadCallback(functionName, out functionResult, ref resultLine);
            }
            else
            {
                resultLine = "Internal error: No FunctionEvaluateFileReadCallback handler specified";
            }
            return false;
        }

        public static List<IPhysicalQuantity> ParseExpressionList(ref String commandLine, ref String resultLine)
        {
            List<IPhysicalQuantity> pqList = new List<IPhysicalQuantity>();
            Boolean MoreToParse = false;
            Boolean OK = true;
            do
            {
                IPhysicalQuantity pq = null;
                pq = ParseConvertedExpression(ref commandLine, ref resultLine);
                OK = pq != null;
                if (OK)
                {
                    pqList.Add(pq);
                    MoreToParse = TokenString.TryParseToken(",", ref commandLine);
                }
                else
                {
                    return null;
                }
            } while (OK && MoreToParse);
            return pqList;
        }


        public static Nullable<Boolean> ParseBooleanExpression(ref String commandLine, ref String resultLine)
        {
            IPhysicalQuantity pq = ParseExpression(ref commandLine, ref resultLine);
            if (pq != null)
            {
                return !pq.Equals(PQ_False);
            }
            return null;
        }

        public static IPhysicalQuantity ParseConvertedExpression(ref String commandLine, ref String resultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseExpression(ref commandLine, ref resultLine);
            if (pq != null)
            {
                pq = ParseOptionalConvertedExpression(pq, ref commandLine, ref resultLine);
            }

            return pq;
        }

        public static IPhysicalQuantity ParseOptionalConvertedExpression(IPhysicalQuantity pq, ref String commandLine, ref String resultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(commandLine))
            {
                IPhysicalUnit pu = ParseOptionalConvertToUnit(ref commandLine, ref resultLine);
                if (pu != null)
                {
                    pqRes = pq.ConvertTo(pu);
                    if (pqRes == null)
                    {
                        resultLine = "The unit " + pq.Unit.ToPrintString() + " can't be converted to " + pu.ToPrintString() + "\n";
                        //  pqRes = pq.ConvertTo(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponentList { new PrefixedUnitExponent(pu), new PrefixedUnitExponent(pq.Unit) }, new PrefixedUnitExponentList { new PrefixedUnitExponent(pu) }));
                        pqRes = pq.ConvertTo(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponentList { new PrefixedUnitExponent(pu), new PrefixedUnitExponent(pq.Unit.Divide(pu).Unit) }, null));
                    }
                    return pqRes;
                }
            }

            pqRes = CheckForNamedDerivedUnit(pqRes);

            return pqRes;
        }

        public static IPhysicalQuantity CheckForNamedDerivedUnit(IPhysicalQuantity pq)
        {
            IPhysicalQuantity pqRes = pq;
            if (pqRes != null)
            {   
                IUnitSystem unitSystem = pqRes.Unit.System;
                if (unitSystem != null)
                {
                    IPhysicalUnit namedDerivedUnit = unitSystem.NamedDerivedUnitFromUnit(pqRes.Unit);
                    if (namedDerivedUnit != null)
                    {
                        pqRes = new PhysicalQuantity(pqRes.Value, namedDerivedUnit);
                    }
                }
            }

            return pqRes;
        }

        public static IPhysicalUnit ParseOptionalConvertToUnit(ref String commandLine, ref String resultLine)
        {
            IPhysicalUnit pu = null;
            if (TokenString.TryParseToken("[", ref commandLine))
            { // "Convert to unit" square paranteses

                int UnitStringLen = commandLine.IndexOf(']');
                if (UnitStringLen == 0)
                {
                    resultLine = "Missing unit to convert to";
                }
                else
                {
                    String UnitString;
                    if (UnitStringLen == -1)
                    {   // Not terminated by ']', but handle that later
                        // Try to parse rest of line as an unit 
                        UnitString = commandLine;
                        UnitStringLen = commandLine.Length;
                    }
                    else
                    {   // Parse only the valid unit formatted string
                        UnitString = commandLine.Substring(0, UnitStringLen);
                    }

                    pu = ParsePhysicalUnit(ref UnitString, ref resultLine);
                }

                commandLine = commandLine.Substring(UnitStringLen);
                commandLine = commandLine.TrimStart();

                TokenString.ParseToken("]", ref commandLine, ref resultLine);
            }

            return pu;
        }

        // Token kinds
        public enum TokenKind
        {
            None = 0,
            Operand = 1,
            Operator = 2 /*,
            UnaryOperator = 3 */
        }

        class token
        {
            public readonly TokenKind TokenKind;

            public readonly IPhysicalQuantity Operand;
            public readonly OperatorKind Operator;

            public token(IPhysicalQuantity Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = Operand;
            }

            public token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }
        }

        class expressiontokenizer
        {
            public String InputString;
            public String ResultString;
            public int Pos = 0;
            public Boolean InputRecognized = true;
            public IPhysicalUnit dimensionless = Physics.dimensionless;
            public Boolean ThrowExceptionOnInvalidInput = false;

            private Stack<OperatorKind> Operators = new Stack<OperatorKind>();
            private List<token> Tokens = new List<token>();

            TokenKind LastReadToken = TokenKind.None;
            int ParenCount = 0;

            public expressiontokenizer(String InputString)
            {
                this.InputString = InputString;
            }

            public expressiontokenizer(IPhysicalUnit dimensionless, String InputString)
            {
                this.dimensionless = dimensionless;
                this.InputString = InputString;
            }

            public string GetRemainingInput()
            {
                return InputString.Substring(Pos);
            }

            private Boolean PushNewOperator(OperatorKind newOperator)
            {
                Boolean NewOperatorValid = (LastReadToken != TokenKind.Operator);

                if (!NewOperatorValid)
                {
                    if (newOperator == OperatorKind.add)
                    {
                        newOperator = OperatorKind.unaryplus;
                        NewOperatorValid = true;
                    }
                    else
                        if (newOperator == OperatorKind.sub)
                        {
                            newOperator = OperatorKind.unaryminus;
                            NewOperatorValid = true;
                        }
                }

                if (NewOperatorValid)
                {
                    if (Operators.Count > 0)
                    {
                        // Pop operators with precedence higher than new operator
                        OperatorKind NewOperatorPrecedence = newOperator.Precedence();
                        Boolean KeepPoping = true;
                        while ((Operators.Count > 0) && KeepPoping)
                        {
                            OperatorKind  NextOperatorsPrecedence = Operators.Peek().Precedence();
                            KeepPoping = (   (NextOperatorsPrecedence > NewOperatorPrecedence)
                                          || (   (NextOperatorsPrecedence == NewOperatorPrecedence)
                                              && (NewOperatorPrecedence != OperatorKind.unaryplus)));
                            if (KeepPoping) 
                            {
                                Tokens.Add(new token(Operators.Pop()));
                            }
                        }
                    }
                    Operators.Push(newOperator);
                    LastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    ResultString = "The string argument is not in a valid physical expression format. Invalid or missing operand at pos " + Pos.ToString();
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException(ResultString);
                    }

                    return false;
                }
            }

            private Boolean PushNewParenbegin()
            {
                if (LastReadToken == TokenKind.Operand)
                {
                    // Cannot follow operand
                    ResultString = "The string argument is not in a valid physical expression format. Invalid or missing operator at pos " + Pos.ToString();
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException(ResultString);
                    }

                    return false;
                } 
                else 
                {
                    // Push opening parenthesis onto stack
                    Operators.Push(OperatorKind.parenbegin);
                    //LastReadToken = TokenKind.Operator;
                    // Track number of parentheses
                    ParenCount++;

                    return true;
                }
            }

            private Boolean PopUntilParenbegin()
            {
                if (LastReadToken != TokenKind.Operand)
                {
                    // Must follow operand
                    ResultString = "The string argument is not in a valid physical expression format. Invalid or missing operand at pos " + Pos.ToString();
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException(ResultString);
                    }

                    return false;
                }
                else if (ParenCount == 0)
                {
                    // Must have matching opening parenthesis
                    ResultString = "The string argument is not in a valid physical expression format. Unmatched closing parenthesis at pos " + Pos.ToString();
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException(ResultString);
                    }

                    return false;
                }
                else
                {
                    // Pop all operators until matching opening parenthesis found
                    OperatorKind temp = Operators.Pop();
                    while (temp != OperatorKind.parenbegin)
                    {
                        Tokens.Add(new token(temp));
                        temp = Operators.Pop();
                    }

                    // Track number of opening parenthesis
                    ParenCount--;

                    return true;
                }
            }


            private token RemoveFirstToken()
            {   // return first operator from post fix operators
                token Token = Tokens[0];
                Tokens.RemoveAt(0);

                return Token;
            }

            public token GetToken()
            {
                Debug.Assert(InputString != null);

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                while (InputString.Length > Pos && InputRecognized)
                {
                    Char c = InputString[Pos];

                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        Pos++; // Shift to next char
                    }
                    else if (c == '(') 
                    {
                        // Push opening parenthesis onto operator stack
                        if (PushNewParenbegin())
                        {
                            Pos++; // Shift to next char
                        }
                        else
                        {
                            //return null;
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            InputRecognized = false;
                        }
                    }
                    else if (c == ')')
                    {
                        // Pop all operators until matching opening parenthesis found
                        if (PopUntilParenbegin())
                        {
                            Pos++; // Shift to next char
                        }
                        else
                        {
                            //return null;
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            InputRecognized = false;
                        }
                    }
                    else
                    {
                        OperatorKind NewOperator = OperatorKind.none;

                        if (Pos + 1 < InputString.Length && InputString[Pos + 1] == '=')
                        {
                            NewOperator = OperatorKindExtensions.OperatorKindFromChar1Equal(c);
                            if (NewOperator != OperatorKind.none)
                            {
                                Pos++; // Shift to next char
                            }
                        }

                        if (NewOperator == OperatorKind.none)
                        {
                            NewOperator = OperatorKindExtensions.OperatorKindFromChar(c);
                        }

                        if (NewOperator != OperatorKind.none)
                        {
                            if (PushNewOperator(NewOperator))
                            {
                                Pos++; // Shift to next char
                            }
                            else
                            {
                                //return null;
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                InputRecognized = false;
                            }
                        }
                        else if (Char.IsDigit(c))
                        {
                            if (LastReadToken == TokenKind.Operand)
                            {
                                if (ThrowExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An operator must follow a operand. Invalid operand at '" + c + "' at pos " + Pos.ToString());
                                }
                                else
                                {
                                    //return null;
                                    // End of recognized input; Stop reading and return operator tokens from stack.
                                    InputRecognized = false;
                                }
                            }
                            else
                            {
                                Double D;

                                String CommandLine = GetRemainingInput();
                                int OldLen = CommandLine.Length;
                                String ResultLine = "";
                                Boolean OK = ParseDouble(ref CommandLine, ref ResultLine, out D);
                                Pos += OldLen - CommandLine.Length;
                                if (OK)
                                {
                                    IPhysicalUnit pu = null;
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Parse optional unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0])))
                                        {
                                            ResultLine = "";
                                            pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            Pos += OldLen - CommandLine.Length;
                                        }
                                    }
                                    if (pu == null)
                                    {
                                        pu = dimensionless;
                                    }

                                    IPhysicalQuantity pq = new PhysicalQuantity(D, pu);

                                    LastReadToken = TokenKind.Operand;
                                    return new token(pq);
                                }

                                if (ThrowExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid or missing operand after '" + c + "' at position " + Pos.ToString());
                                }
                                else
                                {
                                    //return null;
                                    // End of recognized input; Stop reading and return operator tokens from stack.
                                    InputRecognized = false;
                                }
                            }
                        }
                        else if (Char.IsLetter(c) || Char.Equals(c, '_'))
                        {
                            String IdentifierName;

                            String CommandLine = GetRemainingInput();
                            int OldLen = CommandLine.Length;
                            String ResultLine = "";

                            IPhysicalQuantity pq;

                            Boolean IdentifierFound = ParseQualifiedIdentifier(ref CommandLine, ref ResultLine, out IdentifierName, out pq);
                            Pos += OldLen - CommandLine.Length;
                            if (!IdentifierFound)
                            {
                                if (!String.IsNullOrEmpty(IdentifierName) && !String.IsNullOrEmpty(CommandLine) && CommandLine[0] == '(')
                                {
                                    OldLen = CommandLine.Length;
                                            
                                    string line2 = CommandLine.Substring(1).TrimStart();
                                    if (!String.IsNullOrEmpty(line2) && line2[0] == ')')
                                    {   // Undefined function without parameters? Maybe it is a .cal file name? 
                                        IdentifierFound = File.Exists(IdentifierName + ".cal");
                                        if (IdentifierFound)
                                        {
                                            TokenString.ParseChar('(', ref CommandLine, ref ResultLine);
                                            CommandLine = CommandLine.TrimStart();
                                            TokenString.ParseChar(')', ref CommandLine, ref ResultLine);

                                            FileFunctionGet(IdentifierName, out pq, ref ResultLine);
                                            Pos += OldLen - CommandLine.Length;
                                        }
                                    }
                                }
                                /**
                                if (!IdentifierFound)
                                {
                                    resultLine = "Unknown identifier: '" + IdentifierName + "'";
                                }
                                **/

                            }

                            if (pq != null)
                            {
                                LastReadToken = TokenKind.Operand;
                                return new token(pq);
                            }

                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid or missing operand at '" + c + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                //return null;
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                InputRecognized = false;
                            }

                        }
                        else
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid input '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                //return null;
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                InputRecognized = false;
                            }
                        }
                    }

                    if (Tokens.Count > 0)
                    {   // return first operator from post fix operators
                        return RemoveFirstToken();
                    }

                };

                // Expression cannot end with operator
                if (LastReadToken == TokenKind.Operator)
                {
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Operand expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                    }
                    else
                    {
                        //return null;
                        // End of recognized input; Stop reading and return operator tokens from stack.
                        InputRecognized = false;                    
                    }
                }
                // Check for balanced parentheses
                if (ParenCount > 0)
                {
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Closing parenthesis expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                    } 
                    else
                    {
                        //return null;
                        // End of recognized input; Stop reading and return operator tokens from stack.
                        InputRecognized = false;
                    }
                }
                // Retrieve remaining operators from stack
                while (Operators.Count > 0) 
                {
                    Tokens.Add(new token(Operators.Pop()));
                }

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                return null;
            }
        }
        public static readonly IPhysicalQuantity PQ_False = new PhysicalMeasure.PhysicalQuantity(0);
        public static readonly IPhysicalQuantity PQ_True = new PhysicalMeasure.PhysicalQuantity(1);

        public static IPhysicalQuantity ParseExpression(ref String commandLine, ref String resultLine)
        {
            //public static readonly 
            PhysicalUnit dimensionless = new CombinedUnit();

            expressiontokenizer Tokenizer = new expressiontokenizer(dimensionless, commandLine);

            Tokenizer.ThrowExceptionOnInvalidInput = false;
            Stack<IPhysicalQuantity> Operands = new Stack<IPhysicalQuantity>();

            token Token = Tokenizer.GetToken();
            while (Token != null)
            {
                if (Token.TokenKind == TokenKind.Operand)
                {
                    // Stack PhysicalQuantity operand
                    Operands.Push(Token.Operand);
                }
                else if (Token.TokenKind == TokenKind.Operator)
                {

                    if (Token.Operator == OperatorKind.unaryplus)
                    {
                        // Notthing to do
                    }
                    else if (Token.Operator == OperatorKind.unaryminus)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        IPhysicalQuantity pqTop = Operands.Pop();
                        // Invert sign of pq
                        Operands.Push(pqTop.Multiply(-1));
                    }
                    else if (Token.Operator == OperatorKind.not)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        IPhysicalQuantity pqTop = Operands.Pop();
                        // Invert pqTop as boolean
                        if (!pqTop.Equals(PQ_False))
                        {
                            pqTop = PQ_False;
                        }
                        else
                        {
                            pqTop = PQ_True;
                        }
                        Operands.Push(pqTop);
                    }
                    else
                    {
                        Debug.Assert(Operands.Count >= 2);

                        IPhysicalQuantity pqSecond = Operands.Pop();
                        IPhysicalQuantity pqFirst = Operands.Pop();

                        if (Token.Operator == OperatorKind.add)
                        {
                            // Combine pq1 and pq2 to the new PhysicalQuantity pq1*pq2   
                            Operands.Push(pqFirst.Add(pqSecond));
                        }
                        else if (Token.Operator == OperatorKind.sub)
                        {
                            // Combine pq1 and pq2 to the new PhysicalQuantity pq1/pq2
                            Operands.Push(pqFirst.Subtract(pqSecond));
                        }
                        else if (Token.Operator == OperatorKind.mult)
                        {
                            // Combine pq1 and pq2 to the new PhysicalQuantity pq1*pq2   
                            Operands.Push(pqFirst.Multiply(pqSecond));
                        }
                        else if (Token.Operator == OperatorKind.div)
                        {
                            // Combine pq1 and pq2 to the new PhysicalQuantity pq1/pq2
                            Operands.Push(pqFirst.Divide(pqSecond));
                        }
                        else if (   (Token.Operator == OperatorKind.pow) 
                                 || (Token.Operator == OperatorKind.root))
                        {
                            SByte Exponent;
                            if (pqSecond.Value >= 1)
                            {   // Use operator and Exponent
                                Exponent = (SByte)pqSecond.Value;
                            }
                            else
                            {   // Invert operator and Exponent
                                Exponent = (SByte)(1/pqSecond.Value);

                                if (Token.Operator == OperatorKind.pow)
                                {
                                    Token = new token(OperatorKind.root);
                                }
                                else
                                {
                                    Token = new token(OperatorKind.pow);
                                }
                            }

                            if (Token.Operator == OperatorKind.pow)
                            {
                                // Combine pq and exponent to the new PhysicalQuantity pq^expo
                                Operands.Push(pqFirst.Pow(Exponent));
                            }
                            else
                            {
                                // Combine pq and exponent to the new PhysicalQuantity pq^(1/expo)
                                Operands.Push(pqFirst.Rot(Exponent));
                            }
                        }
                        else if (Token.Operator == OperatorKind.equals)
                        {
                            // Save pqFirst == pqSecond
                            //Operands.Push(pqFirst.Equals(pqSecond));

                            if (pqFirst.Equals(pqSecond))
                            {
                                Operands.Push(PQ_True);
                            }
                            else
                            {
                                Operands.Push(PQ_False);
                            }
                        }
                        else if (Token.Operator == OperatorKind.differs)
                        {
                            // Save pqFirst != pqSecond
                            //Operands.Push(!pqFirst.Equals(pqSecond));
                            if (!pqFirst.Equals(pqSecond))
                            {
                                Operands.Push(PQ_True);
                            }
                            else
                            {
                                Operands.Push(PQ_False);
                            }

                        }
                        else if (   Token.Operator == OperatorKind.lessthan
                                 || Token.Operator == OperatorKind.lessorequals
                                 || Token.Operator == OperatorKind.largerthan
                                 || Token.Operator == OperatorKind.largerorequals
                                )
                        {
                            int res = pqFirst.CompareTo(pqSecond);

                            if (   ((Token.Operator == OperatorKind.lessthan)       && (res <  0))
                                || ((Token.Operator == OperatorKind.lessorequals)   && (res <= 0))
                                || ((Token.Operator == OperatorKind.largerthan)     && (res >  0))                               
                                || ((Token.Operator == OperatorKind.largerorequals) && (res >= 0))                               
                                )
                            {
                                Operands.Push(PQ_True);
                            }
                            else
                            {
                                Operands.Push(PQ_False);
                            }
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                }

                Token = Tokenizer.GetToken();
            }

            commandLine = Tokenizer.GetRemainingInput(); // Remaining of input string
            resultLine = Tokenizer.ResultString;

            Debug.Assert(Operands.Count <= 1);  // 0 or 1

            return (Operands.Count > 0) ? Operands.Pop() : null;
        }


        public static Boolean ParseQualifiedIdentifier(ref String commandLine, ref String resultLine, out String identifierName, out IPhysicalQuantity identifierValue)
        {
            identifierValue = null;

            String QualifiedIdentifierName;
            IEnvironment PrimaryContext;
            IEnvironment QualifiedIdentifierContext;
            IdentifierKind identifierkind;

            commandLine = commandLine.ReadIdentifier(out identifierName);
            Debug.Assert(identifierName != null);

            Boolean PrimaryIdentifierFound = IdentifierContextLookup(identifierName, out PrimaryContext, out identifierkind, ref resultLine);

            Boolean IdentifierFound = PrimaryIdentifierFound;

            QualifiedIdentifierContext = PrimaryContext;
            QualifiedIdentifierName = identifierName;

            while (IdentifierFound && !String.IsNullOrEmpty(commandLine) && commandLine[0] == '.')
            {
                TokenString.ParseChar('.', ref commandLine, ref resultLine);
                commandLine = commandLine.TrimStart();

                commandLine = commandLine.ReadIdentifier(out identifierName);
                Debug.Assert(identifierName != null);
                commandLine = commandLine.TrimStart();

                IEnvironment FoundInContext;
                IdentifierKind FoundIdentifierkind;

                IdentifierFound = QualifiedIdentifierContextLookup(QualifiedIdentifierContext, identifierName, out FoundInContext, out FoundIdentifierkind, ref resultLine);
                if (IdentifierFound)
                {
                    QualifiedIdentifierContext = FoundInContext;
                    identifierkind = FoundIdentifierkind;

                    QualifiedIdentifierName += "." + identifierName;
                }
                else
                {
                    resultLine = QualifiedIdentifierName + " don't have a field named '" + identifierName + "'";
                }
            }

            if (IdentifierFound)
            {
                if ((identifierkind == IdentifierKind.Variable) || (identifierkind == IdentifierKind.Constant))
                {
                    VariableGet(QualifiedIdentifierContext, identifierName, out identifierValue, ref resultLine);
                }
                else if (identifierkind == IdentifierKind.Function)
                {
                    TokenString.ParseChar('(', ref commandLine, ref resultLine);
                    commandLine = commandLine.TrimStart();
                    List<IPhysicalQuantity> parameterlist = ParseExpressionList(ref commandLine, ref resultLine);
                    Boolean OK = parameterlist != null;
                    if (OK)
                    {
                        TokenString.ParseChar(')', ref commandLine, ref resultLine);

                        FunctionGet(QualifiedIdentifierContext, identifierName, parameterlist, out identifierValue, ref resultLine);

                        commandLine = commandLine.TrimStart();
                    }
                    else
                    {
                        // Error in result line
                        Debug.Assert(!String.IsNullOrEmpty(resultLine));
                    }
                }
            }

            return PrimaryIdentifierFound;
        }

        public static Boolean ParseDouble(ref String commandLine, ref String resultLine, out Double D)
        {
            Boolean OK = false;
            D = 0.0;
            commandLine = commandLine.TrimStart();

            if (String.IsNullOrEmpty(commandLine))
            {
                // resultLine = "Double not found";
            }
            else
            {
                //NumberStyles styles = NumberStyles.Float;
                //IFormatProvider provider = NumberFormatInfo.InvariantInfo;
                // 0x010203.040506 + 0x102030.405060

                // Scan number
                int numlen = 0;
                int maxlen = commandLine.Length; // Max length of sign and digits to look for
                int numberBase = 10; // Decimal number expected
                int exponentNumberBase = 10; // Decimal exponentnumber expected

                int numberSignPos = -1; // No number sign found
                int hexNumberPos = -1; // No hex number prefix found
                int DecimalCharPos = -1; // No decimal char found
                int exponentCharPos = -1;  // No exponent char found
                int exponentNumberSignPos = -1; // No exponent number sign found
                int exponentHexNumberPos = -1; // No exponent hex number prefix found

                if ((commandLine[0] == '-') || (commandLine[0] == '+'))
                {
                    numberSignPos = numlen;
                    numlen = 1;
                }

                while (numlen < maxlen && Char.IsDigit(commandLine[numlen]))
                {
                    numlen++;
                }

                if (   (numlen < maxlen)
                    && (commandLine[numlen] == 'x')
                    && (numlen > 0)
                    && (commandLine[numlen-1] == '0')
                    && (   (numlen < 2)
                        || (!Char.IsDigit(commandLine[numlen-2]))))
                {
                    numlen++;
                    hexNumberPos = numlen;
                    numberBase = 0x10; // Hexadecimal number expected
                }

                while ((numlen < maxlen)
                       && (Char.IsDigit(commandLine[numlen])
                           || ((numberBase == 0x10)
                               && Char.IsLetter(commandLine[numlen])
                    //&& Char.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                               && TokenString.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                {
                    numlen++;
                }

                if ((numlen < maxlen)
                    && ((commandLine[numlen] == '.')
                        || (commandLine[numlen] == ',')))
                {
                    DecimalCharPos = numlen;
                    numlen++;
                }
                while (   (numlen < maxlen)
                       && (   Char.IsDigit(commandLine[numlen]) 
                           || (   (numberBase == 0x10) 
                               && Char.IsLetter(commandLine[numlen])
                               //&& Char.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                               && TokenString.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                {
                    numlen++;
                }

                if ((numlen < maxlen)
                    && ((commandLine[numlen] == 'E')
                        || (commandLine[numlen] == 'e')
                        || (commandLine[numlen] == 'H')
                        || (commandLine[numlen] == 'h')))
                {
                    exponentCharPos = numlen;

                    numlen++;
                    if ((numlen < maxlen)
                        && ((commandLine[numlen] == '-')
                            || (commandLine[numlen] == '+')))
                    {
                        exponentNumberSignPos = numlen;
                        numlen++;
                    }

                    while (numlen < maxlen && Char.IsDigit(commandLine[numlen]))
                    {
                        numlen++;
                    }

                    if ((numlen < maxlen)
                        && (commandLine[numlen] == 'x')
                        && (numlen > 0)
                        && (commandLine[numlen - 1] == '0')
                        && ((numlen < 2)
                            || (!Char.IsDigit(commandLine[numlen - 2]))))
                    {
                        numlen++;
                        exponentHexNumberPos = numlen;
                        exponentNumberBase = 0x10; // Hexadecimal number expected
                    }

                    while ((numlen < maxlen)
                           && (Char.IsDigit(commandLine[numlen])
                               || ((exponentNumberBase == 0x10)
                                   && Char.IsLetter(commandLine[numlen])
                        //&& Char.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                                   && TokenString.IsHexDigit(Char.ToLower(commandLine[numlen])))))
                    {
                        numlen++;
                    }

                }

                if (numlen > 0)
                {
                    //System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                     
                    if (numberBase == 0x10 || exponentNumberBase == 0x10)
                    {   // Hex number
                        //Double baseNumberD = 0;
                        int baseNumberLen = numlen;
                        if (exponentCharPos > 0)
                        {
                            baseNumberLen = exponentCharPos -1;
                        }
                        //OK = Double.TryParse(commandLine.Substring(0, numlen), numberstyle, NumberFormatInfo.InvariantInfo, out D); 

                        if (numberBase == 10)
                        {
                            System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                            OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberstyle, null, out D);
                            if (!OK)
                            {
                                OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out D);
                            }
                        }
                        else
                        {
                            long baseNumberL = 0;
                            int baseIntegralNumberLen = baseNumberLen - hexNumberPos;
                            if (DecimalCharPos > 0)
                            {
                                baseIntegralNumberLen = DecimalCharPos - hexNumberPos;
                            }
                            
                            System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                            OK = long.TryParse(commandLine.Substring(hexNumberPos, baseIntegralNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                            D = baseNumberL;

                            if (DecimalCharPos > 0)
                            {
                                int NoOfChars = baseNumberLen - (DecimalCharPos + 1);
                                OK = long.TryParse(commandLine.Substring(DecimalCharPos + 1, NoOfChars), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                                D = D + (baseNumberL / Math.Pow(16, NoOfChars)) ;
                                
                            }
                            
                            if (numberSignPos > 0 && commandLine[numberSignPos] == '-')
                            {
                                D = -D;
                            }
                        }
                        if (OK && exponentCharPos > 0)
                        {
                            Double exponentNumberD = 0;
                            if (numberBase == 10)
                            {
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                                OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numlen - (baseNumberLen + 1)), numberstyle, null, out exponentNumberD);
                                if (!OK)
                                {
                                    OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numlen - (baseNumberLen + 1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumberD);
                                }
                            }
                            else
                            {
                                long exponentNumber = 0;
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                                OK = long.TryParse(commandLine.Substring(exponentHexNumberPos, numlen - (exponentHexNumberPos-1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumber);
                                exponentNumberD = exponentNumber;

                                if (exponentNumberSignPos > 0 && commandLine[exponentNumberSignPos] == '-')
                                {
                                    exponentNumberD = -exponentNumberD;
                                }
                            }

                            if (OK)
                            {
                                Double Exponent;
                                if ((commandLine[exponentCharPos] == 'H') || (commandLine[exponentCharPos] == 'h'))
                                {
                                    Exponent = 0x10;
                                }
                                else
                                {
                                    Exponent = 10;
                                }

                                D = D * Math.Pow(Exponent, exponentNumberD);
                            }
                        }
                    }
                    else
                    {
                        System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                        OK = Double.TryParse(commandLine.Substring(0, numlen), numberstyle, null, out D); // styles, provider
                        if (!OK)
                        {
                            OK = Double.TryParse(commandLine.Substring(0, numlen), numberstyle, NumberFormatInfo.InvariantInfo, out D); // styles, provider
                        }

                    }
                    if (OK)
                    {
                        commandLine = commandLine.Substring(numlen);
                    }
                    else
                    {
                        resultLine = commandLine.Substring(0, numlen) + " is not a valid number";
                    }
                }
            }
            return OK;
        }

        public static IPhysicalUnit ParsePhysicalUnit(ref String commandLine, ref String resultLine)
        {
            IPhysicalUnit pu = null;
            Boolean UnitIdentifierFound = false;
            String IdentifierName;
            IEnvironment Context;

            String CommandLineRest = commandLine.ReadIdentifier(out IdentifierName);

            if (IdentifierName != null)
            {
                // Check for custom defined unit
                INametableItem Item;
                UnitIdentifierFound = IdentifierItemLookup(IdentifierName, out Context, out Item, ref resultLine);
                if (UnitIdentifierFound)
                {
                    if (Item.Identifierkind == IdentifierKind.Unit)
                    {
                        commandLine = CommandLineRest;
                        pu = ((NamedUnit)Item).pu;
                    }
                    else
                    {
                        resultLine = IdentifierName + " is a " + Item.Identifierkind.ToString() + ". Expected an unit";
                    }
                }
            }
            //if (!UnitIdentifierFound)
            if (pu == null)
            {   // Standard physical unit expressions

                // Parse unit
                commandLine = commandLine.TrimStart();
                if (!String.IsNullOrEmpty(commandLine) && (Char.IsLetter(commandLine[0])))
                {
                    int UnitStringLen = commandLine.IndexOfAny(new char[] { ' ' });  // ' '
                    if (UnitStringLen < 0 )
                    {
                        UnitStringLen = commandLine.Length;
                    }
                    String UnitStr = commandLine.Substring(0, UnitStringLen);
                    String tempResultLine = null;
                    pu = PhysicalUnit.ParseUnit(ref UnitStr, ref tempResultLine, false);
                    if (pu != null || String.IsNullOrEmpty(resultLine))
                    {
                        resultLine = tempResultLine;
                    }

                    int Pos = UnitStringLen - UnitStr.Length;
                    commandLine = commandLine.Substring(Pos);
                }

            }
            return pu;
        }

        #endregion Physical Expression parser methods
    }


       // Operator kinds
        // Precedence for a group of operators is same as first (lowest) enum in the group
        public enum OperatorKind
        {
            // Precediens == 0
            none = 0,

            // Precediens == 1
            parenbegin = 1,
            parenend = 2,

            // Precediens == 3
            equals = 3,
            differs = 4,

            // Precediens == 5
            lessthan = 5,
            largerthan = 6,
            lessorequals = 7,
            largerorequals = 8,

            // Precediens == 9
            not = 9,

            // Precediens == 11
            add = 11,
            sub = 12,

            // Precediens == 13
            mult = 13,
            div = 14,

            // Precediens == 15
            pow = 15,
            root = 16,

            // Precediens == 17
            unaryplus = 17,
            unaryminus = 18
        }

    public static class OperatorKindExtensions
    {

        public static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            switch (operatoren)
            {
                case OperatorKind.parenbegin: // "("
                case OperatorKind.parenend: // ")":
                    return OperatorKind.parenbegin; // 1;
                case OperatorKind.equals: // "=="
                case OperatorKind.differs: // "!=":
                    return OperatorKind.equals; // 3;
                case OperatorKind.lessthan: // "<"
                case OperatorKind.largerthan: // ">":
                case OperatorKind.lessorequals: // "<="
                case OperatorKind.largerorequals: // ">=":
                    return OperatorKind.lessthan; // 3;
                case OperatorKind.not: // "!"
                    return OperatorKind.not; // 5;
                case OperatorKind.add: // "+"
                case OperatorKind.sub: // "-":
                     return OperatorKind.add; // 7;
                case OperatorKind.mult: // "*":
                case OperatorKind.div: // "/":
                     return OperatorKind.mult; // 9;
                case OperatorKind.pow: // "^":
                case OperatorKind.root: // "!":
                     return OperatorKind.pow; // 11;
                case OperatorKind.unaryplus: // unaryplus:
                case OperatorKind.unaryminus: // UnaryMinus:
                     return OperatorKind.unaryplus; // 13;
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar(Char c)
        {
            switch (c)
            {
                case '(':
                    return OperatorKind.parenbegin; // 1;
                case ')':
                    return OperatorKind.parenend; // 2;
                //case '!':
                //      return OperatorKind.not; // 5;

                case '!':
                    return OperatorKind.not; // 1;
                case '<':
                    return OperatorKind.lessthan; // 1;
                case '>':
                    return OperatorKind.largerthan; // 1;

                case '+':
                    return OperatorKind.add; // 3;
                case '-':
                    return OperatorKind.sub; // 4;
                case '*':
                case '·':  // centre dot  '\0x0B7' (char)183 U+00B7
                    return OperatorKind.mult; // 5;
                case '/':
                    return OperatorKind.div; // 6;
                case '^':
                    return OperatorKind.pow; // 7;
                // case '!':
                //      return OperatorKind.Root; // 8;
                /*
                case '+': // unaryplus:
                     return OperatorKind.unaryplus; // 9;
                case '-': // UnaryMinus:
                     return OperatorKind.unaryminus; // 10;
                 */
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar1Equal(Char c)
        {
            switch (c)
            {
                case '=':
                    return OperatorKind.equals; // 1;
                case '!':
                    return OperatorKind.differs; // 1;
                case '<':
                    return OperatorKind.lessorequals; // 1;
                case '>':
                    return OperatorKind.largerorequals; // 1;
            }

            return OperatorKind.none;
        }

        public static OperatorKind Precedence(this OperatorKind operatoren)
        {
            return OperatorPrecedence(operatoren);
        }
    }
}
