﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using TokenParser;

using PhysicalMeasure;

using PhysicalCalculator.Identifers;
using PhysicalCalculator.Function;

namespace PhysicalCalculator.Expression
{

    public interface IEnviroment
    {
        Tracelevel OutputTracelevel { get; set; } 

        Boolean SetLocalIdentifier(String IdentifierName, INametableItem Item);
        Boolean RemoveLocalIdentifier(String IdentifierName);

        Boolean FindIdentifier(String IdentifierName, out IEnviroment FoundInContext, out INametableItem Item);

        Boolean SystemSet(String SystemName, out INametableItem SystemItem);

        Boolean UnitSet(IUnitSystem UnitSystem, String UnitName, IPhysicalQuantity UnitValue, out INametableItem UnitItem);

        Boolean VariableGet(String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine);
        Boolean VariableSet(String VariableName, IPhysicalQuantity VariableValue);

        Boolean FunctionFind(String FunctionName, out IFunctionEvaluator functionevaluator);
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
        public delegate Boolean IdentifierItemLookupFunc(String IdentifierName, out IEnviroment FoundInContext, out INametableItem Item);
        public delegate Boolean IdentifierContextLookupFunc(String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind);
        public delegate Boolean QualifiedIdentifierContextLookupFunc(IEnviroment LookInContext, String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind);

        public delegate Boolean VariableValueLookupFunc(IEnviroment LookInContext, String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine);
        public delegate Boolean FunctionLookupFunc(IEnviroment LookInContext, String FunctionName, out IFunctionEvaluator functionevaluator);
        public delegate Boolean FunctionEvaluateFunc(String FunctionName, IFunctionEvaluator functionevaluator, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine);
        public delegate Boolean FunctionEvaluateFileReadFunc(String FunctionName, out IPhysicalQuantity FunctionResult, ref String ResultLine);


        // Delegert static globals
        public static IdentifierItemLookupFunc IdentifierItemLookupCallback;
        public static IdentifierContextLookupFunc IdentifierContextLookupCallback;
        public static QualifiedIdentifierContextLookupFunc QualifiedIdentifierContextLookupCallback;

        public static VariableValueLookupFunc VariableValueGetCallback;
        public static FunctionLookupFunc FunctionLookupCallback;
        public static FunctionEvaluateFunc FunctionEvaluateCallback;
        public static FunctionEvaluateFileReadFunc FunctionEvaluateFileReadCallback;

        // static access functions

        public static Boolean IdentifierItemLookup(String IdentifierName, out IEnviroment FoundInContext, out INametableItem Item, ref String ResultLine)
        {
            Item = null;
            FoundInContext = null;
            if (IdentifierItemLookupCallback != null)
            {
                return IdentifierItemLookupCallback(IdentifierName, out FoundInContext, out Item);
            }
            return false;
        }

        public static Boolean IdentifierContextLookup(String VariableName, out IEnviroment FoundInContext, out IdentifierKind identifierkind, ref String ResultLine)
        {
            identifierkind = IdentifierKind.unknown;
            FoundInContext = null;
            if (IdentifierContextLookupCallback != null)
            {
                return IdentifierContextLookupCallback(VariableName, out FoundInContext, out identifierkind);
            }
            return false;
        }

        public static Boolean QualifiedIdentifierContextLookup(IEnviroment LookInContext, String VariableName, out IEnviroment FoundInContext, out IdentifierKind identifierkind, ref String ResultLine)
        {
            identifierkind = IdentifierKind.unknown;
            FoundInContext = null;
            if (QualifiedIdentifierContextLookupCallback != null)
            {
                return QualifiedIdentifierContextLookupCallback(LookInContext, VariableName, out FoundInContext, out identifierkind);
            }
            return false;
        }

        public static Boolean VariableGet(IEnviroment LookInContext, String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine)
        {
            VariableValue = null;
            if (VariableValueGetCallback != null)
            {
                return VariableValueGetCallback(LookInContext, VariableName, out VariableValue, ref ResultLine);
            }
            return false;
        }

        public static Boolean FunctionGet(IEnviroment LookInContext, String FunctionName, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            FunctionResult = null;
            IFunctionEvaluator functionevaluator;
            if (FunctionLookupCallback(LookInContext, FunctionName, out functionevaluator))
            {
                if (FunctionEvaluateCallback != null)
                {
                    return FunctionEvaluateCallback(FunctionName, functionevaluator, parameterlist, out FunctionResult, ref ResultLine);
                }
                else
                {
                    ResultLine = "Internal error: No FunctionEvaluateCallback handler specified";
                }
            }
            else
            {
                ResultLine = "Internal error: FunctionLookupCallback failed";
            }

            return false;
        }

        public static Boolean FileFunctionGet(String FunctionName, out IPhysicalQuantity FunctionResult, ref String ResultLine)
        {
            FunctionResult = null;
            if (FunctionEvaluateFileReadCallback != null)
            {
                return FunctionEvaluateFileReadCallback(FunctionName, out FunctionResult, ref ResultLine);
            }
            else
            {
                ResultLine = "Internal error: No FunctionEvaluateFileReadCallback handler specified";
            }
            return false;
        }

        public static List<IPhysicalQuantity> ParseExpressionList(ref String CommandLine, ref String ResultLine)
        {
            List<IPhysicalQuantity> pqList = new List<IPhysicalQuantity>();
            Boolean MoreToParse = false;
            Boolean OK = true;
            do
            {
                IPhysicalQuantity pq = null;
                pq = ParseConvertedExpression(ref CommandLine, ref ResultLine);
                OK = pq != null;
                if (OK)
                {
                    pqList.Add(pq);
                    MoreToParse = TokenString.TryParseToken(",", ref CommandLine);
                }
                else
                {
                    return null;
                }
            } while (OK && MoreToParse);
            return pqList;
        }

        public static IPhysicalQuantity ParseConvertedExpression(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseExpression(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalConvertedExpression(pq, ref CommandLine, ref ResultLine);
            }

            return pq;
        }

        public static IPhysicalQuantity ParseOptionalConvertedExpression(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                IPhysicalUnit pu = ParseOptionalConvertToUnit(ref CommandLine, ref ResultLine);
                if (pu != null)
                {
                    pqRes = pq.ConvertTo(pu);
                    if (pqRes == null)
                    {
                        ResultLine = "The unit " + pq.Unit.ToPrintString() + " can't be converted to " + pu.ToPrintString();
                    }
                }
            }

            return pqRes;
        }

        public static IPhysicalUnit ParseOptionalConvertToUnit(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalUnit pu = null;
            if (TokenString.TryParseToken("[", ref CommandLine))
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

                    /*** 
                    int SpacePos = CommandLine.IndexOf(' ');
                    if (SpacePos == 1)
                    {
                        Char NumberBaseChar = Char.ToLower(CommandLine[0]);
                        if ((NumberBaseChar == 'h')
                            || (NumberBaseChar == 'x'))
                        {
                            UnitString = UnitString.Substring(2);
                        }
                    }
                    ***/

                    UnitString = UnitString.TrimEnd();
                    String UnitStringAll = UnitString;
                    UnitStrLen = UnitString.Length;

                    pu = ParsePhysicalUnit(ref UnitString, ref ResultLine);

                    CommandLine = CommandLine.Substring(UnitStrLen - UnitString.Length);
                    CommandLine = CommandLine.TrimStart();
                    TokenString.ParseToken("]", ref CommandLine, ref ResultLine);

                    if (pu == null)
                    {
                        ResultLine = "'" + UnitStringAll + "' is not a valid unit.";
                    }
                }
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

        /*****
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
            add = 3,
            sub = 4,

            // Precediens == 5
            mult = 5,
            div = 6,

            // Precediens == 7
            pow = 7,
            root = 8,

            // Precediens == 9
            unaryplus = 9,
            unaryminus = 10
        }

        public static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            switch (operatoren)
            {
                case OperatorKind.parenbegin: // "("
                case OperatorKind.parenend: // ")":
                    return OperatorKind.parenbegin; // 1;
                case OperatorKind.add: // "+"
                case OperatorKind.sub: // "-":
                     return OperatorKind.add; // 3;
                case OperatorKind.mult: // "*":
                case OperatorKind.div: // "/":
                     return OperatorKind.mult; // 5;
                case OperatorKind.pow: // "^":
                case OperatorKind.root: // "!":
                     return OperatorKind.pow; // 7;
                case OperatorKind.unaryplus: // unaryplus:
                case OperatorKind.unaryminus: // UnaryMinus:
                     return OperatorKind.unaryplus; // 9;
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
                //      return OperatorKind.root; // 8;
                / *
                case '+': // unaryplus:
                     return OperatorKind.unaryplus; // 9;
                case '-': // UnaryMinus:
                     return OperatorKind.unaryminus; // 10;
                 * /
            }

            return OperatorKind.none;
        }
        
        public static OperatorKind Precedence(this OperatorKind operatoren)
        {
            return OperatorPrecedence(operatoren);
        }
 
        *****/

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
            public Boolean InputRecognaized = true;
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
                    //LastReadToken = tokenkind.Operator;
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

                while (InputString.Length > Pos && InputRecognaized)
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
                            // End of recognatized input; Stop reading and return operator tokens from stack.
                            InputRecognaized = false;
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
                            // End of recognatized input; Stop reading and return operator tokens from stack.
                            InputRecognaized = false;
                        }
                    }
                    else
                    {
                        OperatorKind NewOperator = OperatorKindExtensions.OperatorKindFromChar(c);

                        if (NewOperator != OperatorKind.none)
                        {
                            if (PushNewOperator(NewOperator))
                            {
                                Pos++; // Shift to next char
                            }
                            else
                            {
                                //return null;
                                // End of recognatized input; Stop reading and return operator tokens from stack.
                                InputRecognaized = false;
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
                                    // End of recognatized input; Stop reading and return operator tokens from stack.
                                    InputRecognaized = false;
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
                                    IPhysicalQuantity pq =  new PhysicalQuantity(D, dimensionless);
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Parse optional unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0])))
                                        {
                                            ResultLine = "";
                                            IPhysicalUnit pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            Pos += OldLen - CommandLine.Length;
                                            if (pu != null)
                                            {
                                                pq.Unit = pu;
                                            }
                                        }
                                    }

                                    LastReadToken = TokenKind.Operand;
                                    return new token(pq);
                                }

                                if (ThrowExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid or missing operand after '" + c + "' at pos " + Pos.ToString());
                                }
                                else
                                {
                                    //return null;
                                    // End of recognatized input; Stop reading and return operator tokens from stack.
                                    InputRecognaized = false;
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
                                    ResultLine = "Unknown identifier: '" + IdentifierName + "'";
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
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid or missing operand at '" + c + "' at pos " + Pos.ToString());
                            }
                            else
                            {
                                //return null;
                                // End of recognatized input; Stop reading and return operator tokens from stack.
                                InputRecognaized = false;
                            }

                        }
                        else
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Invalid input '" + InputString.Substring(Pos) + "' at pos " + Pos.ToString());
                            }
                            else
                            {
                                //return null;
                                // End of recognatized input; Stop reading and return operator tokens from stack.
                                InputRecognaized = false;
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
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Operand expected '" + InputString.Substring(Pos) + "' at pos " + Pos.ToString());
                    }
                    else
                    {
                        //return null;
                        // End of recognatized input; Stop reading and return operator tokens from stack.
                        InputRecognaized = false;                    
                    }
                }
                // Check for balanced parentheses
                if (ParenCount > 0)
                {
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical expression format. Closing parenthesis expected '" + InputString.Substring(Pos) + "' at pos " + Pos.ToString());
                    } 
                    else
                    {
                        //return null;
                        // End of recognatized input; Stop reading and return operator tokens from stack.
                        InputRecognaized = false;
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

        public static IPhysicalQuantity ParseExpression(ref String CommandLine, ref String ResultLine)
        {
            //public static readonly 
            PhysicalUnit dimensionless = new CombinedUnit();

            expressiontokenizer Tokenizer = new expressiontokenizer(dimensionless, CommandLine);

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
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                }

                Token = Tokenizer.GetToken();
            }

            CommandLine = Tokenizer.GetRemainingInput(); // Remaining of input string
            ResultLine = Tokenizer.ResultString;

            Debug.Assert(Operands.Count <= 1);  // 0 or 1

            return (Operands.Count > 0) ? Operands.Pop() : null;
        }


        public static Boolean ParseQualifiedIdentifier(ref String CommandLine, ref String ResultLine, out String IdentifierName, out IPhysicalQuantity pq)
        {
            pq = null;

            String QualifiedIdentifierName;
            IEnviroment PrimaryContext;
            IEnviroment QualifiedIdentifierContext;
            IdentifierKind identifierkind;

            CommandLine = CommandLine.ReadIdentifier(out IdentifierName);
            Debug.Assert(IdentifierName != null);

            Boolean PrimaryIdentifierFound = IdentifierContextLookup(IdentifierName, out PrimaryContext, out identifierkind, ref ResultLine);

            Boolean IdentifierFound = PrimaryIdentifierFound;

            QualifiedIdentifierContext = PrimaryContext;
            QualifiedIdentifierName = IdentifierName;

            while (IdentifierFound && !String.IsNullOrEmpty(CommandLine) && CommandLine[0] == '.')
            {
                TokenString.ParseChar('.', ref CommandLine, ref ResultLine);
                CommandLine = CommandLine.TrimStart();

                CommandLine = CommandLine.ReadIdentifier(out IdentifierName);
                Debug.Assert(IdentifierName != null);
                CommandLine = CommandLine.TrimStart();

                IEnviroment FoundInContext;
                IdentifierKind FoundIdentifierkind;

                IdentifierFound = QualifiedIdentifierContextLookup(QualifiedIdentifierContext, IdentifierName, out FoundInContext, out FoundIdentifierkind, ref ResultLine);
                if (IdentifierFound)
                {
                    QualifiedIdentifierContext = FoundInContext;
                    identifierkind = FoundIdentifierkind;

                    QualifiedIdentifierName += "." + IdentifierName;
                }
                else
                {
                    ResultLine = QualifiedIdentifierName + " don't have a field named '" + IdentifierName + "'";
                }
            }

            if (IdentifierFound)
            {
                if (identifierkind == IdentifierKind.variable)
                {
                    VariableGet(QualifiedIdentifierContext, IdentifierName, out pq, ref ResultLine);
                }
                else if (identifierkind == IdentifierKind.function)
                {
                    TokenString.ParseChar('(', ref CommandLine, ref ResultLine);
                    CommandLine = CommandLine.TrimStart();
                    List<IPhysicalQuantity> parameterlist = ParseExpressionList(ref CommandLine, ref ResultLine);
                    Boolean OK = parameterlist != null;
                    if (OK)
                    {
                        TokenString.ParseChar(')', ref CommandLine, ref ResultLine);

                        FunctionGet(QualifiedIdentifierContext, IdentifierName, parameterlist, out pq, ref ResultLine);

                        CommandLine = CommandLine.TrimStart();
                    }
                    else
                    {
                        // Error in result line
                        Debug.Assert(!String.IsNullOrEmpty(ResultLine));
                    }
                }
            }

            return PrimaryIdentifierFound;
        }

        public static Boolean ParseDouble(ref String CommandLine, ref String ResultLine, out Double D)
        {
            Boolean OK = false;
            D = 0.0;
            CommandLine = CommandLine.TrimStart();

            if (String.IsNullOrEmpty(CommandLine))
            {
                // ResultLine = "Double not found";
            }
            else
            {
                //NumberStyles styles = NumberStyles.Float;
                //IFormatProvider provider = NumberFormatInfo.InvariantInfo;
                // 0x010203.040506 + 0x102030.405060

                // Scan number
                int numlen = 0;
                int maxlen = CommandLine.Length; // Max length of sign and digits to look for
                int numberBase = 10; // Decimal number expected
                int exponentNumberBase = 10; // Decimal exponentnumber expected

                int numberSignPos = -1; // No number sign found
                int hexNumberPos = -1; // No hex number prefix found
                int DecimalCharPos = -1; // No decimal char found
                int exponentCharPos = -1;  // No exponent char found
                int exponentNumberSignPos = -1; // No exponent number sign found
                int exponentHexNumberPos = -1; // No exponent hex number prefix found

                if ((CommandLine[0] == '-') || (CommandLine[0] == '+'))
                {
                    numberSignPos = numlen;
                    numlen = 1;
                }

                while (numlen < maxlen && Char.IsDigit(CommandLine[numlen]))
                {
                    numlen++;
                }

                if (   (numlen < maxlen)
                    && (CommandLine[numlen] == 'x')
                    && (numlen > 0)
                    && (CommandLine[numlen-1] == '0')
                    && (   (numlen < 2)
                        || (!Char.IsDigit(CommandLine[numlen-2]))))
                {
                    numlen++;
                    hexNumberPos = numlen;
                    numberBase = 0x10; // Hexadecimal number expected
                }

                while ((numlen < maxlen)
                       && (Char.IsDigit(CommandLine[numlen])
                           || ((numberBase == 0x10)
                               && Char.IsLetter(CommandLine[numlen])
                    //&& Char.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
                               && TokenString.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
                {
                    numlen++;
                }

                if ((numlen < maxlen)
                    && ((CommandLine[numlen] == '.')
                        || (CommandLine[numlen] == ',')))
                {
                    DecimalCharPos = numlen;
                    numlen++;
                }
                while (   (numlen < maxlen)
                       && (   Char.IsDigit(CommandLine[numlen]) 
                           || (   (numberBase == 0x10) 
                               && Char.IsLetter(CommandLine[numlen])
                               //&& Char.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
                               && TokenString.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
                {
                    numlen++;
                }

                if ((numlen < maxlen)
                    && ((CommandLine[numlen] == 'E')
                        || (CommandLine[numlen] == 'e')
                        || (CommandLine[numlen] == 'H')
                        || (CommandLine[numlen] == 'h')))
                {
                    exponentCharPos = numlen;

                    numlen++;
                    if ((numlen < maxlen)
                        && ((CommandLine[numlen] == '-')
                            || (CommandLine[numlen] == '+')))
                    {
                        exponentNumberSignPos = numlen;
                        numlen++;
                    }

                    while (numlen < maxlen && Char.IsDigit(CommandLine[numlen]))
                    {
                        numlen++;
                    }

                    if ((numlen < maxlen)
                        && (CommandLine[numlen] == 'x')
                        && (numlen > 0)
                        && (CommandLine[numlen - 1] == '0')
                        && ((numlen < 2)
                            || (!Char.IsDigit(CommandLine[numlen - 2]))))
                    {
                        numlen++;
                        exponentHexNumberPos = numlen;
                        exponentNumberBase = 0x10; // Hexadecimal number expected
                    }

                    while ((numlen < maxlen)
                           && (Char.IsDigit(CommandLine[numlen])
                               || ((exponentNumberBase == 0x10)
                                   && Char.IsLetter(CommandLine[numlen])
                        //&& Char.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
                                   && TokenString.IsHexDigit(Char.ToLower(CommandLine[numlen])))))
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
                        //OK = Double.TryParse(CommandLine.Substring(0, numlen), numberstyle, NumberFormatInfo.InvariantInfo, out D); 

                        if (numberBase == 10)
                        {
                            System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                            OK = Double.TryParse(CommandLine.Substring(0, baseNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out D);
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
                            OK = long.TryParse(CommandLine.Substring(hexNumberPos, baseIntegralNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                            D = baseNumberL;

                            if (DecimalCharPos > 0)
                            {
                                int NoOfChars = baseNumberLen - (DecimalCharPos + 1);
                                OK = long.TryParse(CommandLine.Substring(DecimalCharPos + 1, NoOfChars), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                                D = D + (baseNumberL / Math.Pow(16, NoOfChars)) ;
                                
                            }
                            
                            if (numberSignPos > 0 && CommandLine[numberSignPos] == '-')
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
                                OK = Double.TryParse(CommandLine.Substring(baseNumberLen + 1, numlen - (baseNumberLen + 1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumberD);
                            }
                            else
                            {
                                long exponentNumber = 0;
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                                OK = long.TryParse(CommandLine.Substring(exponentHexNumberPos, numlen - (exponentHexNumberPos-1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumber);
                                exponentNumberD = exponentNumber;

                                if (exponentNumberSignPos > 0 && CommandLine[exponentNumberSignPos] == '-')
                                {
                                    exponentNumberD = -exponentNumberD;
                                }
                            }

                            if (OK)
                            {
                                Double Exponent;
                                if ((CommandLine[exponentCharPos] == 'H') || (CommandLine[exponentCharPos] == 'h'))
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
                        OK = Double.TryParse(CommandLine.Substring(0, numlen), numberstyle, NumberFormatInfo.InvariantInfo, out D); // styles, provider
                    }
                    if (OK)
                    {
                        CommandLine = CommandLine.Substring(numlen);
                    }
                    else
                    {
                        ResultLine = CommandLine.Substring(0, numlen) + " is not a valid number";
                    }
                }
            }
            return OK;
        }

        public static IPhysicalUnit ParsePhysicalUnit(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalUnit pu = null;
            Boolean UnitIdentifierFound = false;
            String IdentifierName;
            IEnviroment Context;

            String CommandLineRest = CommandLine.ReadIdentifier(out IdentifierName);

            if (IdentifierName != null)
            {
                // Check for custom defined unit
                INametableItem Item;
                UnitIdentifierFound = IdentifierItemLookup(IdentifierName, out Context, out Item, ref ResultLine);
                if (UnitIdentifierFound)
                {
                    if (Item.identifierkind == IdentifierKind.unit)
                    {
                        CommandLine = CommandLineRest;
                        pu = ((NamedUnit)Item).pu;
                    }
                    else
                    {
                        ResultLine = IdentifierName + " is a " + Item.identifierkind.ToString() + ". Expected an unit";
                    }
                }
            }
            //if (!UnitIdentifierFound)
            if (pu == null)
            {   // Standard units
                pu = PhysicalUnit.ParseUnit(ref CommandLine);
                if (pu != null && UnitIdentifierFound)
                {   
                    ResultLine = "";
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
            add = 3,
            sub = 4,

            // Precediens == 5
            mult = 5,
            div = 6,

            // Precediens == 7
            pow = 7,
            root = 8,

            // Precediens == 9
            unaryplus = 9,
            unaryminus = 10
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
                case OperatorKind.add: // "+"
                case OperatorKind.sub: // "-":
                     return OperatorKind.add; // 3;
                case OperatorKind.mult: // "*":
                case OperatorKind.div: // "/":
                     return OperatorKind.mult; // 5;
                case OperatorKind.pow: // "^":
                case OperatorKind.root: // "!":
                     return OperatorKind.pow; // 7;
                case OperatorKind.unaryplus: // unaryplus:
                case OperatorKind.unaryminus: // UnaryMinus:
                     return OperatorKind.unaryplus; // 9;
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
                //      return OperatorKind.root; // 8;
                /*
                case '+': // unaryplus:
                     return OperatorKind.unaryplus; // 9;
                case '-': // UnaryMinus:
                     return OperatorKind.unaryminus; // 10;
                 */
            }

            return OperatorKind.none;
        }

        public static OperatorKind Precedence(this OperatorKind operatoren)
        {
            return OperatorPrecedence(operatoren);
        }
    }
}
