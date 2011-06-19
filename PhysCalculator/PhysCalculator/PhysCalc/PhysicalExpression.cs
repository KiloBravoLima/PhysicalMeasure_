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
        Boolean UnitGet(String UnitName, out IPhysicalUnit UnitValue, ref String ResultLine);
        Boolean VariableGet(String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine);
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
            T = F Topt .
            Topt = "*" F Topt | "/" F Topt | e .
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
        public delegate Boolean IdentifierContextLookupFunc(String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind);
        public delegate Boolean QualifiedIdentifierContextLookupFunc(IEnviroment LookInContext, String IdentifierName, out IEnviroment FoundInContext, out IdentifierKind identifierkind);

        public delegate Boolean VariableValueLookupFunc(IEnviroment LookInContext, String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine);
        public delegate Boolean FunctionLookupFunc(IEnviroment LookInContext, String FunctionName, out IFunctionEvaluator functionevaluator);
        public delegate Boolean FunctionEvaluateFunc(String FunctionName, IFunctionEvaluator functionevaluator, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine);
        public delegate Boolean FunctionEvaluateFileReadFunc(String FunctionName, out IPhysicalQuantity FunctionResult, ref String ResultLine);


        // Delegert static globals
        public static IdentifierContextLookupFunc IdentifierContextLookupCallback;
        public static QualifiedIdentifierContextLookupFunc QualifiedIdentifierContextLookupCallback;

        public static VariableValueLookupFunc VariableValueGetCallback;
        public static FunctionLookupFunc FunctionLookupCallback;
        public static FunctionEvaluateFunc FunctionEvaluateCallback;
        public static FunctionEvaluateFileReadFunc FunctionEvaluateFileReadCallback;

        // static access functions
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

        public static IPhysicalQuantity ParseExpression(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseTerm(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalExpression(pq, ref CommandLine, ref ResultLine);
            }

            return pq;
        }

        public static IPhysicalQuantity ParseOptionalExpression(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                if (TokenString.TryParseToken("+", ref CommandLine))
                {
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Add(pq2);
                }
                else if (TokenString.TryParseToken("-", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Subtract(pq2);
                }
            }

            return pqRes;
        }

        public static IPhysicalQuantity ParseTerm(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParseFactor(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalTerm(pq, ref CommandLine, ref ResultLine);
            }
            return pq;
        }

        public static IPhysicalQuantity ParseOptionalTerm(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                CommandLine = CommandLine.TrimStart();

                if (   TokenString.TryParseChar('*', ref CommandLine) 
                    || TokenString.TryParseChar('·', ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    if (pq2 != null)
                    {
                        pqRes = pq.Multiply(pq2);
                    }
                    else
                    {
                        pqRes = null;
                    }
                }
                else if (TokenString.TryParseToken("/", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    if (pq2 != null)
                    {
                        pqRes = pq.Divide(pq2);
                    }
                    else
                    {
                        pqRes = null;
                    }
                }
            }

            return pqRes;
        }

        public static IPhysicalQuantity ParseFactor(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq;

            pq = ParsePrimaryExpression(ref CommandLine, ref ResultLine);
            if (pq != null)
            {
                pq = ParseOptionalPostUnaryOperator(pq, ref CommandLine, ref ResultLine);
            }
            return pq;
        }

        public static IPhysicalQuantity ParseOptionalPostUnaryOperator(IPhysicalQuantity pq, ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pqRes = pq;
            if (!String.IsNullOrEmpty(CommandLine))
            {
                CommandLine = CommandLine.TrimStart();

                if (TokenString.TryParseChar('^', ref CommandLine)) 
                {
                    CommandLine = CommandLine.TrimStart();

                    Double RealExponent;
                    Boolean OK = ParseDouble(ref CommandLine, ref ResultLine, out RealExponent);
                    if (OK)
                    {
                        sbyte IntExponent;

                        if (RealExponent >= 1.0)
                        {
                            IntExponent = (SByte)Math.Round(RealExponent);
                            pqRes = pq.Pow(IntExponent);
                        }
                        else
                        {
                            IntExponent = (SByte)Math.Round(1.0/RealExponent);
                            pqRes = pq.Rot(IntExponent);
                        }
                    }
                    else
                    {
                        ResultLine = "Signed number for exponent expected";
                    }
                }
            }

            return pqRes;
        }

        public static IPhysicalQuantity ParsePrimaryExpression(ref String CommandLine, ref String ResultLine)
        {
            IPhysicalQuantity pq = null;
            CommandLine = CommandLine.TrimStart();

            if (String.IsNullOrEmpty(CommandLine))
            {
                // ResultLine = "Factor not found";
            }
            else if ( (CommandLine[0] == '+') || (CommandLine[0] == '-'))
            {
                Boolean Negate = CommandLine[0] == '-';

                CommandLine = CommandLine.Substring(1);
                CommandLine = CommandLine.TrimStart();

                pq = ParseFactor(ref CommandLine, ref ResultLine);
                if (Negate)
                {
                    pq.Value = -pq.Value;
                }
            }
            else if (Char.IsLetter(CommandLine[0]) || Char.Equals(CommandLine[0], '_'))
            {
                String IdentifierName;

                Boolean IdentifierFound = ParseQualifiedIdentifier(ref CommandLine, ref ResultLine, out IdentifierName, out pq);
                if (!IdentifierFound)
                {
                    Boolean IsFileFunction = false;
                    if (!String.IsNullOrEmpty(IdentifierName) && !String.IsNullOrEmpty(CommandLine) && CommandLine[0] == '(')
                    {
                        string line2 = CommandLine.Substring(1).TrimStart();
                        if (!String.IsNullOrEmpty(line2) && line2[0] == ')')
                        {   // Undefined function without parameters? Maybe it is a .cal file name? 
                            IsFileFunction = File.Exists(IdentifierName + ".cal");
                            if (IsFileFunction)
                            {
                                TokenString.ParseChar('(', ref CommandLine, ref ResultLine);
                                CommandLine = CommandLine.TrimStart();
                                TokenString.ParseChar(')', ref CommandLine, ref ResultLine);

                                FileFunctionGet(IdentifierName, out pq, ref ResultLine);
                            }
                        }
                    }

                    if (!IsFileFunction)
                    {
                        ResultLine = "Unknown identifier: '" + IdentifierName + "'";
                    }
                }
            }
            else if (CommandLine[0] == '(')
            {   // paranteses
                CommandLine = CommandLine.Substring(1).TrimStart(); // Skip start parantes '('

                pq = ParseExpression(ref CommandLine, ref ResultLine);

                CommandLine = CommandLine.TrimStart();
                TokenString.ParseChar(')', ref CommandLine, ref ResultLine);
                CommandLine = CommandLine.TrimStart();
            }
            else
            {
                Double D;
                Boolean OK = ParseDouble(ref CommandLine, ref ResultLine, out D);
                if (OK)
                {
                    if (!String.IsNullOrWhiteSpace(CommandLine))
                    {   // Parse optional unit
                        CommandLine = CommandLine.TrimStart();

                        IPhysicalUnit pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                        if (pu != null)
                        {
                            pq = new PhysicalQuantity(D, pu);
                        }
                    }

                    if (pq == null)
                    {
                        pq = new PhysicalQuantity(D, Physics.dimensionless);
                    }
                }
            }
            return pq;
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
                    OK = Double.TryParse(CommandLine.Substring(0, numlen), System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo, out D); // styles, provider
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
            String IdentifierName;
            IEnviroment Context;
            IdentifierKind identifierkind;

            String CommandLineRest = CommandLine.ReadIdentifier(out IdentifierName);

            if (IdentifierName != null)
            {
                // Check for custom defined unit
                Debug.Assert(IdentifierName != null);
                Boolean UnitIdentifierFound = IdentifierContextLookup(IdentifierName, out Context, out identifierkind, ref ResultLine);
                if (UnitIdentifierFound && identifierkind == IdentifierKind.unit)
                {
                    CommandLine = CommandLineRest;
                    Boolean OK = Context.UnitGet(IdentifierName, out pu, ref ResultLine);
                }
                else
                {   // Standard units
                    pu = PhysicalUnit.ParseUnit(ref CommandLine);
                }
            }
            return pu;
        }

        #endregion Physical Expression parser methods
    }
}
