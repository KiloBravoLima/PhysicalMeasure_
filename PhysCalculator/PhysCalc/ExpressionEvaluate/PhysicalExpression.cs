using System;
using System.Collections.Generic;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using PhysicalMeasure;

using TokenParser;

using PhysicalCalculator.Identifiers;
using PhysicalCalculator.Function;


namespace PhysicalCalculator.Expression
{
    public record class OperandInfo
    {
        public Object OperandValue;
        public Type OperandType;


        public OperandInfo(Object someValue)
        {
            Debug.Assert(someValue != null);

            this.OperandValue = someValue;
            this.OperandType = someValue.GetType();
        }

        public OperandInfo(Object someValue, Type someType)
        {
            this.OperandValue = someValue;
            this.OperandType = someType;
        }

        public OperandInfo(Boolean someValue)
        {
            Debug.Assert(someValue != null);
            this.OperandValue = someValue;
            this.OperandType = typeof(Boolean);
        }

        public OperandInfo(DateTime someValue)
        {
            Debug.Assert(someValue != null);
            this.OperandValue = someValue;
            this.OperandType = typeof(DateTime);
        }
        public DateTime? AsDateTime()
        {
            return this.OperandValue as DateTime?;
        }
        public OperandInfo(String someValue)
        {
            Debug.Assert(someValue != null);
            this.OperandValue = someValue;
            this.OperandType = typeof(String);
        }
        public String AsString()
        {
            return this.OperandValue as String;
        }
        public OperandInfo(Quantity someValue)
        {
            Debug.Assert(someValue != null);
            this.OperandValue = someValue;
            this.OperandType = typeof(Quantity);
        }
        public Quantity AsQuantity()
        {
            return this.OperandValue as Quantity;
        }


        public OperandInfo(Unit someValue)
        {
            Debug.Assert(someValue != null);
            this.OperandValue = someValue;
            this.OperandType = typeof(Unit);
        }
        public Unit AsUnit()
        {
            return this.OperandValue as Unit;
        }




        public String ToString(String format, IFormatProvider formatProvider)
        {
            switch (OperandValue)
            {
                case DateTime dateTimeValue:
                    return dateTimeValue.ToString(format, formatProvider);
                case String stringValue:
                    return stringValue.ToString(formatProvider);
                case Quantity quantityValue:
                    return quantityValue.ToString(format, formatProvider);
                case Unit unitValue:
                    return unitValue.ToString(format);
                case Boolean boolValue:
                    return boolValue.ToString(formatProvider);
                case Object objectValue:
                    return objectValue.ToString();
                default:
                    return OperandValue?.ToString();
            }
        }

        public String ToString(String format)
        {
            switch (OperandValue)
            {
                case DateTime dateTimeValue:
                    return dateTimeValue.ToString(format);
                case String stringValue:
                    return stringValue;
                case Quantity quantityValue:
                    return quantityValue.ToString(format);
                case Unit unitValue:
                    return unitValue.ToString(format);
                case Boolean boolValue:
                    return boolValue.ToString();
                case Object objectValue:
                    return objectValue.ToString();
                default:
                    return OperandValue.ToString();
            }
        }

        public override String ToString()
        {
            return OperandValue.ToString();
        }

        public static implicit operator OperandInfo(DateTime v)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEnvironment
    {
        TraceLevels OutputTracelevel { get; set; }
        FormatProviderKind FormatProviderSource { get; set; }
        bool AutoDefineUnits { get; set; }

        CultureInfo CurrentCultureInfo { get; }

        Boolean SetLocalIdentifier(String identifierName, INametableItem item);
        Boolean RemoveLocalIdentifier(String identifierName);

        Boolean FindLocalIdentifier(String identifierName, out INametableItem item);
        Boolean FindIdentifier(String identifierName, out IEnvironment foundInContext, out INametableItem item);

        Boolean SystemSet(String systemName, bool setAsDefaultSystem, out INametableItem systemItem);

        Boolean UnitGet(String unitName, out Unit unitValue, ref String resultLine);
        Boolean UnitSet(IUnitSystem unitSystem, String unitName, OperandInfo unitValue, String unitSymbol, BaseUnitDimension? specifiedDimension, out INametableItem unitItem, out String errorMessage);

        Boolean VariableGet(String variableName, out OperandInfo variableValue, ref String resultLine);
        Boolean VariableSet(String variableName, OperandInfo variableValue);


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

        // Delegate types
        // VariableLookup callback
        public delegate Boolean IdentifierItemLookupFunc(String identifierName, out IEnvironment foundInContext, out INametableItem item);
        public delegate Boolean QualifiedIdentifierItemLookupFunc(IEnvironment lookInContext, String identifierName, out INametableItem item);
        public delegate Boolean IdentifierContextLookupFunc(String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);
        public delegate Boolean QualifiedIdentifierContextLookupFunc(IEnvironment lookInContext, String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);

        public delegate Boolean VariableValueLookupFunc(IEnvironment lookInContext, String variableName, out OperandInfo variableValue, ref String resultLine);
        public delegate Boolean UnitLookupFunc(IEnvironment lookInContext, String variableName, out Unit unitValue, ref String resultLine);
        public delegate Boolean FunctionLookupFunc(IEnvironment lookInContext, String functionName, out IFunctionEvaluator functionEvaluator);
        public delegate Boolean FunctionEvaluateFunc(String functionName, IFunctionEvaluator functionevaluator, List<OperandInfo> parameterlist, out OperandInfo functionResult, ref String resultLine);
        public delegate Boolean FunctionEvaluateFileReadFunc(String functionName, out OperandInfo functionResult, ref String resultLine);

        // Delegate static globals
        public static IdentifierItemLookupFunc IdentifierItemLookupCallback;
        public static QualifiedIdentifierItemLookupFunc QualifiedIdentifierItemLookupCallback;

        public static IdentifierContextLookupFunc IdentifierContextLookupCallback;
        public static QualifiedIdentifierContextLookupFunc QualifiedIdentifierContextLookupCallback;

        public static VariableValueLookupFunc VariableValueGetCallback;
        public static UnitLookupFunc UnitGetCallback;
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

        public static Boolean QualifiedIdentifierItemLookup(IEnvironment lookInContext, String identifierName, out INametableItem item, ref String resultLine)
        {
            item = null;
            if (QualifiedIdentifierItemLookupCallback != null)
            {
                return QualifiedIdentifierItemLookupCallback(lookInContext, identifierName, out item);
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

        public static Boolean VariableGet(IEnvironment lookInContext, String variableName, out OperandInfo variableValue, ref String resultLine)
        {
            variableValue = null;
            if (VariableValueGetCallback != null)
            {
                return VariableValueGetCallback(lookInContext, variableName, out variableValue, ref resultLine);
            }
            return false;
        }

        public static Boolean UnitGet(IEnvironment lookInContext, String unitName, out Unit unitValue, ref String resultLine)
        {
            unitValue = null;
            if (UnitGetCallback != null)
            {
                return UnitGetCallback(lookInContext, unitName, out unitValue, ref resultLine);
            }
            return false;
        }

        public static Boolean FunctionGet(IEnvironment lookInContext, String functionName, List<OperandInfo> parameterlist, out OperandInfo functionResult, ref String resultLine)
        {
            functionResult = null;
            if (FunctionLookupCallback(lookInContext, functionName, out var functionEvaluator))
            {
                if (FunctionEvaluateCallback != null)
                {
                    return FunctionEvaluateCallback(functionName, functionEvaluator, parameterlist, out functionResult, ref resultLine);
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


        public static Boolean FileFunctionGet(String functionName, out OperandInfo functionResult, ref String resultLine)
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


        public static String ParseStringLitteralParam(ref String commandLine, ref String resultLine, List<String> ExpectedFollow, Boolean AllowEmptyList = false)
        {
            String stringLitteral = null;
            int followPos = 0;
            foreach (String follow in ExpectedFollow)
            {
                int pos = commandLine.IndexOf(follow);
                if (pos > 0 && (followPos == 0 || pos < followPos))
                {
                    followPos = pos;
                }
            }
            if (followPos == 0)
            {
                stringLitteral = commandLine;
                commandLine = String.Empty;
            }
            else
            {
                stringLitteral = commandLine.Substring(0, followPos);
                commandLine = commandLine.Substring(followPos);
            }
            return stringLitteral;
        }

        public static List<OperandInfo> ParseExpressionList(ref String commandLine, ref String resultLine, List<String> ExpectedFollow, Boolean AllowEmptyList = false )
        {
            List<OperandInfo> pqList = new List<OperandInfo>();
            Boolean MoreToParse = false;
            Boolean OK = true;
            List<String> TempExpectedFollow = ExpectedFollow;
            if (!TempExpectedFollow.Contains(","))
            {
                TempExpectedFollow = new List<string>(ExpectedFollow) { "," };
            }
            do
            {
                OperandInfo pq = null;
                pq = ParseConvertedExpression(ref commandLine, ref resultLine, TempExpectedFollow);
                OK = pq != null;
                if (OK)
                {
                    pqList.Add(pq);
                    MoreToParse = TokenString.TryParseChar(',', ref commandLine);
                }
                else
                {
                    if (!AllowEmptyList)
                    {
                        return null;
                    }
                }
            } while (OK && MoreToParse);
            return pqList;
        }


        public static Nullable<Boolean> ParseBooleanExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            OperandInfo pq = ParseExpression(ref commandLine, ref resultLine, ExpectedFollow);
            if (pq != null)
            {
                return !pq.Equals(PQ_False);
            }
            return null;
        }

        public static OperandInfo ParseConvertedExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            OperandInfo pq;

            List<String> TempExpectedFollow = ExpectedFollow;
            if (!TempExpectedFollow.Contains("["))
            {
                TempExpectedFollow = new List<string>(ExpectedFollow) { "[" };
            }
            pq = ParseExpression(ref commandLine, ref resultLine, TempExpectedFollow);
            if (pq != null && pq.OperandType == typeof(Quantity))
            {
                Quantity tempQuantity = pq.OperandValue as Quantity;
                Quantity tempQuantityConverted = ParseOptionalConvertedExpression(tempQuantity, ref commandLine, ref resultLine);
                pq = new OperandInfo(tempQuantityConverted);
            }

            return pq;
        }

        private static readonly NamedDerivedUnit ConvertToBaseUnits = new NamedDerivedUnit(null, "BaseUnitDimensions", "Dims", new SByte[] { -127, -127, -127, -127, -127, -127, -127 });

        public static Quantity ParseOptionalConvertedExpression(Quantity pq, ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            if (!String.IsNullOrEmpty(commandLine))
            {
                pu = ParseOptionalConvertToUnit(ref commandLine, ref resultLine);
                if (pu != null && Object.ReferenceEquals(pu, ConvertToBaseUnits))
                {
                    pu = new CombinedUnit(pq.Unit.AsPrefixedUnitExponentList());
                }
            }

            Quantity pqRes;
            if (pu != null)
            {
                pqRes = pq.ConvertTo(pu);
                if (pqRes == null)
                {
                    resultLine = "The unit " + pq.Unit.ToPrintString() + " can't be converted to " + pu.ToPrintString() + "\n";
                    CombinedUnit newRelativeUnit = new CombinedUnit(pu).CombineMultiply(pq.Unit.Divide(pu));
                    pqRes = pq.ConvertTo(newRelativeUnit);
                }
            }
            else
            {
                // No unit specified to convert to; Check if pq can be shown as a NamedDerivedUnit
                pqRes = CheckForNamedDerivedUnit(pq);
            }
            return pqRes;
        }

        public static Quantity CheckForNamedDerivedUnit(Quantity pq)
        {
            Quantity pqRes = pq;
            if (pqRes != null)
            {
                Unit pqUnit = pqRes.Unit;
                if (pqUnit != null && !pqUnit.IsDimensionless)
                {
                    Unit namedDerivedUnit = pqUnit.AsNamedUnit;
                    if (namedDerivedUnit != null && !ReferenceEquals(namedDerivedUnit, pqUnit))
                    {
                        pqRes = new Quantity(pqRes.Value, namedDerivedUnit);
                    }
                    else
                    {   // Try shorten converted units
                        Unit shortnedConvertedUnit = pqUnit.AsShortnedUnit;
                        if (shortnedConvertedUnit != null && !ReferenceEquals(shortnedConvertedUnit, pqUnit))
                        {
                            pqRes = new Quantity(pqRes.Value, shortnedConvertedUnit);
                        }
                    }
                }
            }

            return pqRes;
        }

        public static Unit ParseOptionalConvertToUnit(ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            if (TokenString.TryParseChar('[', ref commandLine))
            { // "Convert to unit" square parentheses

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

                    UnitString.Trim().ReadToken(out var DimToken);
                    if (   DimToken.Equals("base", StringComparison.InvariantCultureIgnoreCase) 
                        || DimToken.Equals("dim", StringComparison.InvariantCultureIgnoreCase))
                    {
                        pu = ConvertToBaseUnits;
                    }
                    else
                    {
                        pu = ParsePhysicalUnit(ref UnitString, ref resultLine);
                    }
                }
                
                commandLine = commandLine.Substring(UnitStringLen);
                commandLine = commandLine.TrimStart();

                TokenString.ParseChar(']', ref commandLine, ref resultLine);
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

        class Token
        {
            public readonly TokenKind TokenKind;

            // public readonly Quantity QuantityOperand;
            public readonly OperandInfo Operand;
            public readonly OperatorKind Operator;


            public Token(TokenKind tokenKind, OperandInfo Operand)
            {
                this.TokenKind = tokenKind;
                this.Operand = Operand;
            }

            public Token(Boolean Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = new OperandInfo(Operand);
            }

            public Token(DateTime Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = new OperandInfo(Operand);
            }

            public Token(Object Operand, Type OperandType)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = new OperandInfo(Operand, OperandType);
            }

            public Token(Quantity Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = new OperandInfo(Operand);
            }

            public Token(OperandInfo Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = Operand;
            }


            public Token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }

            public override string ToString() => TokenKind.ToString() + (TokenKind == TokenKind.None ? "" : " " + (TokenKind == TokenKind.Operand ? Operand.ToString() : Operator.ToString()));
        }

        class ExpressionTokenizer
        {
            public String InputString;
            public String ResultString;

            public int Pos = 0;

            public Unit dimensionless = Global.dimensionless;
            public List<String> ExpectedFollow = new List<String>(); // The list of words and symbols which will terminate parsing the InputString without signaling an error.

            public Boolean ThrowExceptionOnInvalidInput = false;

            private Boolean inputRecognized = true;
            private Boolean errorReported = false;
            private Boolean InvalidPhysicalExpressionFormatErrorReported = false;


            private Stack<OperatorKind> Operators = new Stack<OperatorKind>();
            private List<Token> Tokens = new List<Token>();

            TokenKind LastReadToken = TokenKind.None;
            int ParenCount = 0;

            public ExpressionTokenizer(String InputString)
            {
                this.InputString = InputString;
            }

            public ExpressionTokenizer(Unit dimensionless, String InputString)
            {
                this.dimensionless = dimensionless;
                this.InputString = InputString;
            }

            public string RemainingInput => InputString.Substring(Pos);

            public Boolean InputRecognized => inputRecognized;

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
                            OperatorKind NextOperatorsPrecedence = Operators.Peek().Precedence();
                            KeepPoping = ((NextOperatorsPrecedence > NewOperatorPrecedence)
                                          || ((NextOperatorsPrecedence == NewOperatorPrecedence)
                                              && (NewOperatorPrecedence != OperatorKind.unaryplus)));
                            if (KeepPoping)
                            {
                                Tokens.Add(new Token(Operators.Pop()));
                            }
                        }
                    }
                    Operators.Push(newOperator);
                    LastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand at pos " + Pos.ToString());

                    return false;
                }
            }

            private Boolean PushNewParenbegin()
            {
                if (LastReadToken == TokenKind.Operand)
                {
                    // Cannot follow operand
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operator at pos " + Pos.ToString());
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
                if (ParenCount == 0)
                {
                    if (!ExpectedFollow.Contains(")"))
                    {
                        // Must have matching opening parenthesis
                        ReportInvalidPhysicalExpressionFormatError("Unmatched closing parenthesis at pos " + Pos.ToString());
                    }

                    return false;
                }
                else if (LastReadToken != TokenKind.Operand)
                {
                    // Must follow operand
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand at pos " + Pos.ToString());

                    return false;
                }
                else
                {
                    // Pop all operators until matching opening parenthesis found
                    OperatorKind temp = Operators.Pop();
                    while (temp != OperatorKind.parenbegin)
                    {
                        Tokens.Add(new Token(temp));
                        temp = Operators.Pop();
                    }

                    // Track number of opening parenthesis
                    ParenCount--;

                    return true;
                }
            }


            private Token RemoveFirstToken()
            {   // return first operator from post fix operators
                Token Token = Tokens[0];
                Tokens.RemoveAt(0);

                return Token;
            }

            public void ReportError(String errorMessage)
            {
                // End of recognized input; Stop reading and return operator tokens from stack.
                inputRecognized = false;
                if (!errorReported)
                {
                    if (!String.IsNullOrEmpty(ResultString))
                    {
                        ResultString += ". ";
                    }
                    ResultString += errorMessage;
                    errorReported = true;
                }
                if (ThrowExceptionOnInvalidInput)
                {
                    throw new PhysicalUnitFormatException(ResultString);
                }
            }

            public void ReportInvalidPhysicalExpressionFormatError(String errorMessage)
            {
                
                if (!InvalidPhysicalExpressionFormatErrorReported)
                {
                    errorMessage = "The string argument is not in a valid physical expression format. " + errorMessage;
                }
                ReportError(errorMessage);
                InvalidPhysicalExpressionFormatErrorReported = true;
            }

            public Token GetToken()
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
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;
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
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;
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
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                // Error signaling already done in PushNewOperator
                            }
                        }
                        else if (Char.IsDigit(c))
                        {
                            if (LastReadToken == TokenKind.Operand)
                            {
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                ReportInvalidPhysicalExpressionFormatError("An operator must follow a operand. Invalid operand at '" + c + "' at pos " + Pos.ToString());
                            }
                            else
                            {
                                String CommandLine = RemainingInput;
                                int OldLen = CommandLine.Length;
                                String ResultLine = "";
                                Boolean OK = ParseDouble(ref CommandLine, ref ResultLine, out var D);
                                Pos += OldLen - CommandLine.Length;
                                if (OK)
                                {
                                    Unit pu = null;
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Parse optional unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0]) || Char.Equals(CommandLine[0], '°')))     // Need '°' to read "°C" as unit name
                                        {
                                            ResultLine = "";
                                            pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            Pos += OldLen - CommandLine.Length;
                                        }
                                    }
                                    if (String.IsNullOrWhiteSpace(ResultLine))
                                    {
                                        if (pu == null)
                                        {
                                            pu = dimensionless;
                                        }

                                        Quantity pq = new Quantity(D, pu);

                                        LastReadToken = TokenKind.Operand;
                                        return new Token(pq);
                                    }
                                    else
                                    {
                                        // End of recognized input; Stop reading and return operator tokens from stack.
                                        ReportInvalidPhysicalExpressionFormatError($"Invalid or missing operand after '{D}' at position {Pos}. {ResultLine}");
                                    }
                                }
                                else
                                {
                                    // End of recognized input; Stop reading and return operator tokens from stack.
                                    ReportInvalidPhysicalExpressionFormatError($"Invalid or missing operand after '{c}' at position {Pos}. {ResultLine}");
                                }
                            }
                        }
                        else if (Char.IsLetter(c) || Char.Equals(c, '_'))
                        {
                            String CommandLine = RemainingInput;
                            int OldLen = CommandLine.Length;
                            String ResultLine = "";
                            Boolean PrimaryIdentifierFound = ParseQualifiedIdentifier(ref CommandLine, ref ResultLine, out var IdentifierName, out var pq);
                            // 2014-09-09 Moved to only be done when PrimaryIdentifierFound or call of .cal file as function : Pos += OldLen - CommandLine.Length;
                            int newPos = Pos + OldLen - CommandLine.Length;

                            if (!String.IsNullOrEmpty(ResultLine))
                            {
                                if (!String.IsNullOrEmpty(ResultString))
                                {
                                    ResultString += ". ";
                                }
                                ResultString += ResultLine;
                            }

                            if (PrimaryIdentifierFound)
                            {
                                // Increment read pos; mark IdentifierName as read
                                Pos = newPos;

                                // Check if any inner identifier was found
                                Boolean InnerIdentifierFound = (pq != null);
                                if (!InnerIdentifierFound)
                                {
                                    LastReadToken = TokenKind.Operand;
                                    return new Token(TokenKind.Operand, null);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(IdentifierName) && !String.IsNullOrEmpty(CommandLine) && CommandLine[0] == '(')
                                {
                                    OldLen = CommandLine.Length;

                                    string line2 = CommandLine.Substring(1).TrimStart();
                                    if (!String.IsNullOrEmpty(line2) && line2[0] == ')')
                                    {   // Undefined function without parameters? Maybe it is a .cal file name? 
                                        PrimaryIdentifierFound = File.Exists(IdentifierName + ".cal");
                                        if (PrimaryIdentifierFound)
                                        {
                                            TokenString.ParseChar('(', ref CommandLine, ref ResultLine);
                                            CommandLine = CommandLine.TrimStart();
                                            TokenString.ParseChar(')', ref CommandLine, ref ResultLine);

                                            FileFunctionGet(IdentifierName, out pq, ref ResultLine);
                                            Pos = newPos + OldLen - CommandLine.Length;
                                        }
                                    }
                                }

                                if (!PrimaryIdentifierFound)
                                {
                                    Unit pu = null;
                                    CommandLine = RemainingInput;
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Try parse an unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0])))
                                        {
                                            ResultLine = "";
                                            pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            if (pu != null)
                                            {
                                                Pos += OldLen - CommandLine.Length;
                                                pq = new OperandInfo(new Quantity(1, pu), typeof(Quantity));
                                            }
                                        }
                                    }
                                }

                                /**
                                if (!PrimaryIdentifierFound)
                                {
                                    resultLine = "Unknown identifier: '" + IdentifierName + "'";
                                }
                                **/

                            }

                            if (pq != null)
                            {
                                LastReadToken = TokenKind.Operand;
                                return new Token(pq);
                            }

                            // End of recognized input; Stop reading and return operator tokens from stack.
                            String errorMessage = (!String.IsNullOrEmpty(IdentifierName)) ? "Unknown identifier: '" + IdentifierName + "'"
                                                                                          : "Invalid or missing operand at '" + InputString.Substring(Pos) + "' at position " + Pos.ToString();
                            ReportInvalidPhysicalExpressionFormatError(errorMessage);
                        }
                        else
                        {
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;

                            if (!ExpectedFollow.Contains(new String(c,1)))
                            {
                                ReportInvalidPhysicalExpressionFormatError("Invalid input '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
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
                    // End of recognized input; Stop reading and return operator tokens from stack.
                    ReportInvalidPhysicalExpressionFormatError("Operand expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                }
                // Check for balanced parentheses
                if (ParenCount > 0)
                {
                    // End of recognized input; Stop reading and return operator tokens from stack.
                    ReportInvalidPhysicalExpressionFormatError("Closing parenthesis expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                }
                // Retrieve remaining operators from stack
                while (Operators.Count > 0) 
                {
                    Tokens.Add(new Token(Operators.Pop()));
                }

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                return null;
            }
        }

        public static readonly Quantity PQ_False = new Quantity(0);
        public static readonly Quantity PQ_True = new Quantity(1);
        public static readonly Quantity PQ_Pi = new Quantity(Math.PI);


        public static readonly OperandInfo OI_False = new OperandInfo(PQ_False, typeof(Quantity));
        public static readonly OperandInfo OI_True = new OperandInfo(PQ_True, typeof(Quantity));

        public static OperandInfo ParseExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow ) // = null)
        {
            //public static readonly 
            Unit dimensionless = new CombinedUnit();

            ExpressionTokenizer Tokenizer = new ExpressionTokenizer(dimensionless, commandLine)
            {
                ExpectedFollow = ExpectedFollow,
                ThrowExceptionOnInvalidInput = false
            };
            Stack<OperandInfo> Operands = new Stack<OperandInfo>();

            Token Token = Tokenizer.GetToken();
            while (Token != null)
            {
                // OperatorKind.parenbegin indicates error in Tokenizer.GetToken();
                Debug.Assert(Token.TokenKind != TokenKind.Operand || Token.Operator != OperatorKind.parenbegin);

                if (Token.TokenKind == TokenKind.Operand)
                {
                    // Stack Quantity operand
                    if (Token.Operand != null)
                    {
                        Operands.Push(Token.Operand);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Tokenizer.ResultString))
                        {
                            resultLine += "Internal error. Missing operand value";
                            resultLine += '\n';
                        }
                    }
                }
                else if (Token.TokenKind == TokenKind.Operator)
                {

                    if (Token.Operator == OperatorKind.unaryplus)
                    {
                        // Nothing to do
                    }
                    else if (Token.Operator == OperatorKind.unaryminus)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        OperandInfo pqTopOperand = Operands.Pop();
                        Quantity pqTop = pqTopOperand.AsQuantity();
                        // Invert sign of pq
                        Quantity pqResult = pqTop.Multiply(-1);
                        OperandInfo pqResultOperand = new OperandInfo(pqResult);
                        Debug.Assert(pqResult != null);
                        Debug.Assert(pqResultOperand != null);
                        Operands.Push(pqResultOperand);
                    }
                    else if (Token.Operator == OperatorKind.not)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        OperandInfo pqTopOperand = Operands.Pop();
                        Quantity pqTop = pqTopOperand.AsQuantity();
                        // Invert pqTop as boolean
                        if (!pqTop.Equals(PQ_False))
                        {
                            pqTop = PQ_False;
                            pqTopOperand = OI_False;
                        }
                        else
                        {
                            pqTop = PQ_True;
                            pqTopOperand = OI_True;
                        }
                        Operands.Push(pqTopOperand);
                    }
                    else if (Operands.Count >= 2)
                    {
                        Debug.Assert(Operands.Count >= 2);

                        OperandInfo secondOperand = Operands.Pop();
                        OperandInfo firstOperand = Operands.Pop();

                        if (firstOperand.OperandType == typeof(Quantity) && secondOperand.OperandType == typeof(Quantity))
                        {
                            Quantity pqSecond = secondOperand.AsQuantity();
                            Quantity pqFirst = firstOperand.AsQuantity();

                            Debug.Assert(pqSecond != null);
                            Debug.Assert(pqFirst != null);

                            if (Token.Operator == OperatorKind.add)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1+pq2   
                                Quantity pqResult = pqFirst.Add(pqSecond);
                                OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                Debug.Assert(pqResult != null);
                                Debug.Assert(pqResultOperand != null);
                                Operands.Push(pqResultOperand);
                            }
                            else if (Token.Operator == OperatorKind.sub)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1-pq2
                                Quantity pqResult = pqFirst.Subtract(pqSecond);
                                OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                Debug.Assert(pqResult != null);
                                Debug.Assert(pqResultOperand != null);
                                Operands.Push(pqResultOperand);
                            }
                            else if (Token.Operator == OperatorKind.mult)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1*pq2   
                                Quantity pqResult = pqFirst.Multiply(pqSecond);
                                OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                Debug.Assert(pqResult != null);
                                Debug.Assert(pqResultOperand != null);
                                Operands.Push(pqResultOperand);
                            }
                            else if (Token.Operator == OperatorKind.div)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1/pq2
                                Quantity pqResult = pqFirst.Divide(pqSecond);
                                OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                Debug.Assert(pqResult != null);
                                Debug.Assert(pqResultOperand != null);
                                Operands.Push(pqResultOperand);
                            }
                            else if ((Token.Operator == OperatorKind.pow)
                                     || (Token.Operator == OperatorKind.root))
                            {
                                SByte Exponent;
                                if (pqSecond.Value >= 1)
                                {   // Use operator and Exponent
                                    Exponent = (SByte)pqSecond.Value;
                                }
                                else
                                {   // Invert operator and Exponent
                                    Exponent = (SByte)(1 / pqSecond.Value);

                                    if (Token.Operator == OperatorKind.pow)
                                    {
                                        Token = new Token(OperatorKind.root);
                                    }
                                    else
                                    {
                                        Token = new Token(OperatorKind.pow);
                                    }
                                }

                                if (Token.Operator == OperatorKind.pow)
                                {
                                    // Combine pq and exponent to the new Quantity pq^expo
                                    Quantity pqResult = pqFirst.Pow(Exponent);
                                    OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                    Debug.Assert(pqResult != null);
                                    Debug.Assert(pqResultOperand != null);
                                    Operands.Push(pqResultOperand);
                                }
                                else
                                {
                                    // Combine pq and exponent to the new Quantity pq^(1/expo)
                                    Quantity pqResult = pqFirst.Rot(Exponent);
                                    OperandInfo pqResultOperand = new OperandInfo(pqResult);
                                    Debug.Assert(pqResult != null);
                                    Debug.Assert(pqResultOperand != null);
                                    Operands.Push(pqResultOperand);
                                }
                            }
                            else if (Token.Operator == OperatorKind.equals)
                            {
                                // Save pqFirst == pqSecond
                                Operands.Push(pqFirst.Equals(pqSecond) ? OI_True : OI_False);
                            }
                            else if (Token.Operator == OperatorKind.differs)
                            {
                                // Save pqFirst != pqSecond
                                Operands.Push(pqFirst.Equals(pqSecond) ? OI_False : OI_True);
                            }
                            else if (Token.Operator == OperatorKind.lessthan
                                     || Token.Operator == OperatorKind.lessorequals
                                     || Token.Operator == OperatorKind.largerthan
                                     || Token.Operator == OperatorKind.largerorequals
                                    )
                            {
                                int res = pqFirst.CompareTo(pqSecond);

                                if (((Token.Operator == OperatorKind.lessthan) && (res < 0))
                                    || ((Token.Operator == OperatorKind.lessorequals) && (res <= 0))
                                    || ((Token.Operator == OperatorKind.largerthan) && (res > 0))
                                    || ((Token.Operator == OperatorKind.largerorequals) && (res >= 0))
                                    )
                                {
                                    Operands.Push(OI_True);
                                }
                                else
                                {
                                    Operands.Push(OI_False);
                                }
                            }
                            else
                            {
                                resultLine += $"Can't {firstOperand.OperandType} {Token.Operator} {secondOperand.OperandType}";
                                Debug.Assert(false);
                            }
                        }
                        else if (firstOperand.OperandType == typeof(DateTime) && secondOperand.OperandType == typeof(Quantity))
                        {
                            Quantity pqSecond = secondOperand.AsQuantity();
                            DateTime? dtFirst = firstOperand.AsDateTime();

                            Debug.Assert(pqSecond != null);
                            Debug.Assert(dtFirst != null);

                            if (Token.Operator == OperatorKind.add)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1+pq2   
                                DateTime dtResult;
                                if (pqSecond.Unit.Equals(SI.s))
                                {
                                    dtResult = dtFirst.Value.AddSeconds(pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.min))
                                {
                                    dtResult = dtFirst.Value.AddMinutes(pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.h))
                                {
                                    dtResult = dtFirst.Value.AddHours(pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.d))
                                {
                                    dtResult = dtFirst.Value.AddDays(pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.y))
                                {
                                    dtResult = dtFirst.Value.AddYears((int)pqSecond.Value);
                                }
                                else
                                {
                                    Quantity pq = pqSecond.ConvertTo(SI.s);
                                    if (pq != null)
                                    {
                                        dtResult = dtFirst.Value.AddSeconds(pq.Value);
                                    }
                                    else
                                    {
                                        dtResult = dtFirst.Value;
                                        resultLine += $" Can't convert {pqSecond} to timespan to add it to DateTime";
                                        Debug.Assert(false);
                                    }
                                }
                                OperandInfo dtResultOperand = new OperandInfo(dtResult);
                                Debug.Assert(dtResult != null);
                                Debug.Assert(dtResultOperand != null);
                                Operands.Push(dtResultOperand);
                            }
                            else if (Token.Operator == OperatorKind.sub)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1-pq2
                                DateTime dtResult;
                                if (pqSecond.Unit.Equals(SI.s))
                                {
                                    dtResult = dtFirst.Value.AddSeconds(-1 * pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.min))
                                {
                                    dtResult = dtFirst.Value.AddMinutes(-1 * pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.h))
                                {
                                    dtResult = dtFirst.Value.AddHours(-1 * pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.d))
                                {
                                    dtResult = dtFirst.Value.AddDays(-1 * pqSecond.Value);
                                }
                                else
                                if (pqSecond.Unit.Equals(SI.y))
                                {
                                    dtResult = dtFirst.Value.AddYears(-1 * (int)pqSecond.Value);
                                }
                                else
                                {
                                    Quantity pq = pqSecond.ConvertTo(SI.s);
                                    if (pq != null)
                                    {
                                        dtResult = dtFirst.Value.AddSeconds(pq.Value);
                                    }
                                    else
                                    {
                                        dtResult = dtFirst.Value;
                                        resultLine += $" Can't convert {pqSecond} to timespan to subtract it from DateTime";
                                        Debug.Assert(false);
                                    }
                                }
                                OperandInfo dtResultOperand = new OperandInfo(dtResult);
                                Debug.Assert(dtResult != null);
                                Debug.Assert(dtResultOperand != null);
                                Operands.Push(dtResultOperand);
                            }
                            else
                            {
                                resultLine += $"Can't {firstOperand.OperandType} {Token.Operator} {secondOperand.OperandType}";
                                Debug.Assert(false);
                            }
                        }
                        else if (firstOperand.OperandType == typeof(Quantity) && secondOperand.OperandType == typeof(DateTime))
                        {
                            DateTime? dtSecond = secondOperand.AsDateTime();
                            Quantity pqFirst = firstOperand.AsQuantity();

                            Debug.Assert(dtSecond != null);
                            Debug.Assert(pqFirst != null);

                            if (Token.Operator == OperatorKind.add)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1+pq2   
                                DateTime dtResult;
                                if (pqFirst.Unit.Equals(SI.s))
                                {
                                    dtResult = dtSecond.Value.AddSeconds(pqFirst.Value);
                                }
                                else
                                if (pqFirst.Unit.Equals(SI.min))
                                {
                                    dtResult = dtSecond.Value.AddMinutes(pqFirst.Value);
                                }
                                else
                                if (pqFirst.Unit.Equals(SI.h))
                                {
                                    dtResult = dtSecond.Value.AddHours(pqFirst.Value);
                                }
                                else
                                if (pqFirst.Unit.Equals(SI.d))
                                {
                                    dtResult = dtSecond.Value.AddDays(pqFirst.Value);
                                }
                                else
                                if (pqFirst.Unit.Equals(SI.y))
                                {
                                    dtResult = dtSecond.Value.AddYears((int)pqFirst.Value);
                                }
                                else
                                {
                                    Quantity pq = pqFirst.ConvertTo(SI.s);
                                    if (pq != null)
                                    {
                                        dtResult = dtSecond.Value.AddSeconds(pq.Value);
                                    }
                                    else
                                    {
                                        dtResult = dtSecond.Value;
                                        resultLine += $" Can't convert {pqFirst} to timespan to add it to DateTime";
                                        Debug.Assert(false);
                                    }
                                }
                                OperandInfo dtResultOperand = new OperandInfo(dtResult);
                                Debug.Assert(dtResult != null);
                                Debug.Assert(dtResultOperand != null);
                                Operands.Push(dtResultOperand);
                            }
                            else if (Token.Operator == OperatorKind.sub)
                            {
                                // Combine pq1 and pq2 to the new Quantity pq1-pq2
                                resultLine += $"Can't {firstOperand.OperandType} {Token.Operator} {secondOperand.OperandType}";
                                Debug.Assert(false);
                            }
                            else
                            {
                                resultLine += $"Can't {firstOperand.OperandType} {Token.Operator} {secondOperand.OperandType}";
                                Debug.Assert(false);
                            }
                        }
                        else
                        {
                            resultLine += $"Can't use a {firstOperand.OperandType} and a {secondOperand.OperandType}";
                            Debug.Assert(false);
                        }

                    }
                    else
                    if (Tokenizer.InputRecognized)
                    {   // Error: Unexpected token or missing operands (Operands.Count < 2).
                        Debug.Assert(Token.TokenKind == TokenKind.Operand);
                        // OperatorKind.parenbegin indicates error in Tokenizer.GetToken();
                        Debug.Assert(Token.Operator  != OperatorKind.parenbegin);

                        Debug.Assert(Operands.Count >= 2);
                        Debug.Assert(false);
                    }
                }

                Token = Tokenizer.GetToken();
            }

            commandLine = Tokenizer.RemainingInput; // Remaining of input string
            if (!String.IsNullOrEmpty(Tokenizer.ResultString))
            {
                resultLine += Tokenizer.ResultString;
                resultLine += '\n';
            }

            /*
            if (!Tokenizer.InputRecognized)
            {
                if (!String.IsNullOrEmpty(resultLine))
                {
                    resultLine += ". "; 
                }
                resultLine += Tokenizer.ErrorMessage + '\n'; // 
            }
            */ 
            /*
            if (!String.IsNullOrEmpty(commandLine)) 
            {
                // resultLine += "Physical quantity expected"  + '\n'; // ;
            }
            */

            Debug.Assert(Operands.Count <= 1);  // 0 or 1

            return (Operands.Count > 0) ? Operands.Pop() : null;
        }

        public static Boolean ParseQualifiedIdentifier(ref String commandLine, ref String resultLine, out String identifierName, out OperandInfo identifierValue)
        {
            identifierValue = null;
            commandLine = commandLine.ReadIdentifier(out identifierName);
            Debug.Assert(identifierName != null);
            Boolean PrimaryIdentifierFound = IdentifierItemLookup(identifierName, out var PrimaryContext, out var PrimaryItem, ref resultLine);

            if (PrimaryIdentifierFound)
            {
                Boolean IdentifierFound = PrimaryIdentifierFound;
                String QualifiedIdentifierName = identifierName;
                IEnvironment QualifiedIdentifierContext = PrimaryContext;
                INametableItem IdentifierItem = PrimaryItem;

                while (IdentifierFound && !String.IsNullOrEmpty(commandLine) && commandLine[0] == '.')
                {
                    TokenString.ParseChar('.', ref commandLine, ref resultLine);
                    commandLine = commandLine.TrimStart();

                    commandLine = commandLine.ReadIdentifier(out identifierName);
                    Debug.Assert(identifierName != null);
                    commandLine = commandLine.TrimStart();
                    // IdentifierItem.InnerContext;
                    if (IdentifierItem is IEnvironment QualifiedIdentifierInnerContext)
                    {
                        QualifiedIdentifierContext = QualifiedIdentifierInnerContext;
                    }

                    IdentifierFound = QualifiedIdentifierItemLookup(QualifiedIdentifierContext, identifierName, out IdentifierItem, ref resultLine);
                    if (IdentifierFound)
                    {
                        QualifiedIdentifierName += "." + identifierName;
                    }
                    else
                    {
                        resultLine = QualifiedIdentifierName + " don't have a field named '" + identifierName + "'";
                    }
                }

                if (IdentifierFound)
                {
                    IdentifierKind identifierkind = IdentifierItem.Identifierkind;
                    switch (identifierkind)
                    {
                        case IdentifierKind.Constant:
                        case IdentifierKind.Variable:
                            VariableGet(QualifiedIdentifierContext, identifierName, out identifierValue, ref resultLine);
                            break;
                        case IdentifierKind.Function:
                            if (TokenString.ParseChar('(', ref commandLine, ref resultLine))
                            {
                                commandLine = commandLine.TrimStart();

                                // FunktionInfo funk = IdentifierItem as Funktion;
                                List<string> ExpectedFollow = new List<string> { ")" };
                                List<OperandInfo> operandInfoParameterlist = null;
                                if (   IdentifierItem is PhysicalQuantityFunction_PQ_SB 
                                    || IdentifierItem is PhysicalQuantityFunction_PQ
                                    || IdentifierItem is PhysicalQuantityCommandsFunction)
                                {
                                    operandInfoParameterlist = ParseExpressionList(ref commandLine, ref resultLine, ExpectedFollow, true);
                                }
                                else
                                if (IdentifierItem is DateTimeParamFunction)
                                {
                                    String litteralStringParameter = ParseStringLitteralParam(ref commandLine, ref resultLine, ExpectedFollow, true);
                                    operandInfoParameterlist = new List<OperandInfo> { new OperandInfo(litteralStringParameter) };
                                }
                                else
                                {
                                    Debug.Assert(false);
                                }
                                Boolean OK = operandInfoParameterlist != null;
                                if (OK)
                                {
                                    TokenString.ParseChar(')', ref commandLine, ref resultLine);

                                    FunctionGet(QualifiedIdentifierContext, identifierName, operandInfoParameterlist, out identifierValue, ref resultLine);

                                    commandLine = commandLine.TrimStart();
                                }
                                else
                                {
                                    // Error in result line
                                    Debug.Assert(!String.IsNullOrEmpty(resultLine));
                                }
                            }
                            else
                            {
                                resultLine = "Missing parameters for funtion " + identifierName; //  + " " + IdentifierItem.;
                            }
                            break;
                        case IdentifierKind.Unit:
                            Unit foundUnit;
                            UnitGet(QualifiedIdentifierContext, identifierName, out foundUnit, ref resultLine);
                            identifierValue = new OperandInfo(foundUnit, typeof(Unit));
                            // ref resultLine
                            break;
                        case IdentifierKind.UnitSystem:
                        case IdentifierKind.Unknown:
                        case IdentifierKind.Environment:
                        default:
                            // Unexpeted identifier kind
                            Debug.Assert((identifierkind == IdentifierKind.Variable) || (identifierkind == IdentifierKind.Constant) || (identifierkind == IdentifierKind.Function) || (identifierkind == IdentifierKind.Unit));
                            break;
                    }
                }

            }
            return PrimaryIdentifierFound;  // Indicate if PrimaryIdentifier was found; even if inner identifier was not found
        }

        public static Boolean IsValidDigit(Char ch, int numberBase)
        {
            return    Char.IsDigit(ch)
                   || (numberBase == 0x10) && TokenString.IsHexDigitLetter(ch);
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
                int numLen = 0;
                int maxLen = commandLine.Length; // Max length of sign and digits to look for
                int numberBase = 10; // Decimal number expected
                int exponentNumberBase = 10; // Decimal exponent number expected

                int numberSignPos = -1; // No number sign found
                int hexNumberPos = -1; // No hex number prefix found
                int DecimalCharPos = -1; // No decimal char found
                int exponentCharPos = -1;  // No exponent char found
                int exponentNumberSignPos = -1; // No exponent number sign found
                int exponentHexNumberPos = -1; // No exponent hex number prefix found
                Boolean canParseMore = true;

                if ((commandLine[numLen] == '-') || (commandLine[numLen] == '+'))
                {
                    numberSignPos = numLen;
                    numLen++;
                }

                while (numLen < maxLen && Char.IsDigit(commandLine[numLen]))
                {
                    numLen++;
                }

                if (   (numLen < maxLen)
                    && (commandLine[numLen] == 'x')
                    && (numLen > 0)
                    && (commandLine[numLen-1] == '0')
                    && (   (numLen < 2)
                        || (!Char.IsDigit(commandLine[numLen-2]))))
                {
                    numLen++;
                    hexNumberPos = numLen;
                    numberBase = 0x10; // Hexadecimal number expected
                }

                while ((numLen < maxLen) && IsValidDigit(commandLine[numLen], numberBase))
                {
                    numLen++;
                }

                if (   (numLen < maxLen)
                    && (   (commandLine[numLen] == '.')
                        || (commandLine[numLen] == ','))
                    )
                {
                    canParseMore = (numLen+1 < maxLen) && IsValidDigit(commandLine[numLen + 1], numberBase);
                    if (canParseMore)
                    {
                        DecimalCharPos = numLen;
                        numLen++;
                    }
                }
                while (canParseMore && (numLen < maxLen) && IsValidDigit(commandLine[numLen], numberBase))
                {
                    numLen++;
                }

                if (   canParseMore 
                    && (numLen < maxLen)
                    && (   (commandLine[numLen] == 'E')
                        || (commandLine[numLen] == 'e')
                        || (commandLine[numLen] == 'H')
                        || (commandLine[numLen] == 'h')))
                {
                    exponentCharPos = numLen;

                    numLen++;
                    if ((numLen < maxLen)
                        && ((commandLine[numLen] == '-')
                            || (commandLine[numLen] == '+')))
                    {
                        exponentNumberSignPos = numLen;
                        numLen++;
                    }

                    while (numLen < maxLen && Char.IsDigit(commandLine[numLen]))
                    {
                        numLen++;
                    }

                    if ((numLen < maxLen)
                        && (commandLine[numLen] == 'x')
                        && (numLen > 0)
                        && (commandLine[numLen - 1] == '0')
                        && ((numLen < 2)
                            || (!Char.IsDigit(commandLine[numLen - 2]))))
                    {
                        numLen++;
                        exponentHexNumberPos = numLen;
                        exponentNumberBase = 0x10; // Hexadecimal number expected
                    }

                    while ((numLen < maxLen) && IsValidDigit(commandLine[numLen], numberBase))
                    {
                        numLen++;
                    }

                }

                if (numLen > 0)
                {
                    //System.Globalization.NumberStyles numberStyle = System.Globalization.NumberStyles.Float;
                     
                    if (numberBase == 0x10 || exponentNumberBase == 0x10)
                    {   // Hex number
                        //Double baseNumberD = 0;
                        int baseNumberLen = numLen;
                        if (exponentCharPos > 0)
                        {
                            baseNumberLen = exponentCharPos -1;
                        }
                        //OK = Double.TryParse(commandLine.Substring(0, numLen), numberStyle, NumberFormatInfo.InvariantInfo, out D); 

                        if (numberBase == 10)
                        {
                            System.Globalization.NumberStyles numberStyle = System.Globalization.NumberStyles.Float;
                            OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberStyle, null, out D);
                            if (!OK)
                            {
                                OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberStyle, NumberFormatInfo.InvariantInfo, out D);
                            }
                        }
                        else
                        {
                            int baseIntegralNumberLen = baseNumberLen - hexNumberPos;
                            if (DecimalCharPos > 0)
                            {
                                baseIntegralNumberLen = DecimalCharPos - hexNumberPos;
                            }
                            
                            System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                            OK = long.TryParse(commandLine.Substring(hexNumberPos, baseIntegralNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out long baseNumberL);
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
                                OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numLen - (baseNumberLen + 1)), numberstyle, null, out exponentNumberD);
                                if (!OK)
                                {
                                    OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numLen - (baseNumberLen + 1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumberD);
                                }
                            }
                            else
                            {
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                                OK = long.TryParse(commandLine.Substring(exponentHexNumberPos, numLen - (exponentHexNumberPos-1)), numberstyle, NumberFormatInfo.InvariantInfo, out long exponentNumber);
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
                        OK = Double.TryParse(commandLine.Substring(0, numLen), numberstyle, null, out D); // styles, provider
                        if (!OK)
                        {
                            OK = Double.TryParse(commandLine.Substring(0, numLen), numberstyle, NumberFormatInfo.InvariantInfo, out D); // styles, provider
                        }
                    }
                    if (OK)
                    {
                        commandLine = commandLine.Substring(numLen);
                    }
                    else
                    {
                        resultLine = commandLine.Substring(0, numLen) + " is not a valid number";
                    }
                }
            }
            return OK;
        }

        public static Unit ParsePhysicalUnit(ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            Boolean UnitIdentifierFound = false;
            String CommandLineRest = commandLine.ReadIdentifier(out var IdentifierName);

            if (IdentifierName != null)
            {
                // Check for custom defined unit
                UnitIdentifierFound = IdentifierItemLookup(IdentifierName, out var Context, out var Item, ref resultLine);
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

            if (pu == null)
            {   // Standard physical unit expressions

                // Parse unit
                commandLine = commandLine.TrimStart();
                if (!String.IsNullOrEmpty(commandLine) && (Char.IsLetter(commandLine[0]) || Char.Equals(commandLine[0], '°')))        // Need '°' to read "°" and "°C" as unit names
                {
                    int UnitStringLen = commandLine.IndexOfAny(new Char[] { ' ' });  // ' '
                    if (UnitStringLen < 0 )
                    {
                        UnitStringLen = commandLine.Length;
                    }
                    String UnitStr = commandLine.Substring(0, UnitStringLen);
                    String tempResultLine = null;
                    pu = Unit.ParseMixedUnit(ref UnitStr, ref tempResultLine, false);
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
        // Precedence == 0
        none = 0,

        //Precedence == 1
        parenbegin = 1,
        parenend = 2,

        //Precedence == 3
        equals = 3,
        differs = 4,

        //Precedence == 5
        lessthan = 5,
        largerthan = 6,
        lessorequals = 7,
        largerorequals = 8,

        //Precedence == 9
        not = 9,

        //Precedence == 11
        add = 11,
        sub = 12,

        //Precedence == 13
        mult = 13,
        div = 14,

        //Precedence == 15
        pow = 15,
        root = 16,

        //Precedence == 17
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
                    return OperatorKind.parenbegin; //   1;
                case OperatorKind.equals: // "=="
                case OperatorKind.differs: // "!=":
                    return OperatorKind.equals; // 3;
                case OperatorKind.lessthan: // "<"
                case OperatorKind.largerthan: // ">":
                case OperatorKind.lessorequals: // "<="
                case OperatorKind.largerorequals: // ">=":
                    return OperatorKind.lessthan; //   5;
                case OperatorKind.not: // "!"
                    return OperatorKind.not; //   9;
                case OperatorKind.add: // "+"
                case OperatorKind.sub: // "-":
                     return OperatorKind.add; //   11;
                case OperatorKind.mult: // "*":
                case OperatorKind.div: // "/":
                     return OperatorKind.mult; //   13;
                case OperatorKind.pow: // "^":
                case OperatorKind.root: // "!":
                     return OperatorKind.pow; //   15;
                case OperatorKind.unaryplus: // UnaryPlus:
                case OperatorKind.unaryminus: // UnaryMinus:
                     return OperatorKind.unaryplus; //   17;
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar(Char c)
        {
            switch (c)
            {
                case '(':
                    return OperatorKind.parenbegin; //   1;
                case ')':
                    return OperatorKind.parenend; //   2;


                case '!':
                    return OperatorKind.not; // 9;
                case '<':
                    return OperatorKind.lessthan; // 5;
                case '>':
                    return OperatorKind.largerthan; // 1;

                case '+':
                    return OperatorKind.add; // 3;
                case '-':
                    return OperatorKind.sub; // 4;
                case '*':
                case '·':  // center dot  '\0x0B7' (Char)183 U+00B7
                    return OperatorKind.mult; // 5;
                case '/':
                    return OperatorKind.div; // 6;
                case '^':
                    return OperatorKind.pow; // 7;
                // case '!':
                //      return OperatorKind.Root; // 8;
                /*
                case '+': // UnaryPlus:
                     return OperatorKind.unaryplus; // 9;
                case '-': // UnaryMinus:
                     return OperatorKind.unaryminus; // 10;
                 */
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar1Equal(Char c1)
        {
            switch (c1)
            {
                case '=':   // ==
                    return OperatorKind.equals;
                case '!':   // !=
                    return OperatorKind.differs;
                case '<':   // <=
                    return OperatorKind.lessorequals;
                case '>':   // >=
                    return OperatorKind.largerorequals;
            }

            return OperatorKind.none;
        }

        public static OperatorKind Precedence(this OperatorKind operatoren) => OperatorPrecedence(operatoren);
    }
}
