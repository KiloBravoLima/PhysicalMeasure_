using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using PhysicalMeasure;
using System.Diagnostics;
using System.IO;

using TokenParser;

namespace PhysicalMeasure.Expression
{

    static class PhysicalExpression 
    {
        #region Physical Expression parser methods
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

        // VariableLookup callback

        public delegate Boolean VariableLookupFunc(String VariableName, out IPhysicalQuantity VariableValue);

        public static VariableLookupFunc VariableGetCallback;

        public static Boolean VariableGet(String VariableName, out IPhysicalQuantity VariableValue)
        {
            VariableValue = null;
            if (VariableGetCallback != null)
            {
                return VariableGetCallback(VariableName, out VariableValue);
            }
            return false;
        }


        public static List<IPhysicalQuantity> ParseExpressionList(ref String CommandLine, ref String ResultLine)
        {
            List<IPhysicalQuantity> pqList = new List<IPhysicalQuantity>();
            Boolean MoreToParse = false;
            do
            {
                IPhysicalQuantity pq = null;
                pq = ParseConvertedExpression(ref CommandLine, ref ResultLine);
                pqList.Add(pq);

                MoreToParse = TokenString.TryParseToken(",", ref CommandLine);
            } while (MoreToParse);
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

                        IPhysicalUnit cu = new PhysicalMeasure.CombinedUnit();
                        //IPhysicalQuantity one = new PhysicalQuantity(1, cu);
                        IPhysicalUnit pu = PhysicalUnit.ParseUnit(ref UnitString);

                        CommandLine = CommandLine.Substring(UnitStrLen - UnitString.Length);
                        CommandLine = CommandLine.TrimStart();
                        TokenString.ParseToken("]", ref CommandLine, ref ResultLine);
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

        public static IPhysicalQuantity ParseExpression(ref String CommandLine, ref String ResultLine)
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
            pq = ParseOptionalTerm(pq, ref CommandLine, ref ResultLine);

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
                    pqRes = pq.Multiply(pq2);
                }
                else if (TokenString.TryParseToken("/", ref CommandLine))
                {
                    CommandLine = CommandLine.TrimStart();
                    IPhysicalQuantity pq2 = ParseExpression(ref CommandLine, ref ResultLine);
                    pqRes = pq.Divide(pq2);
                }
            }

            return pqRes;
        }

        public static IPhysicalQuantity ParseFactor(ref String CommandLine, ref String ResultLine)
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
                /*
                Boolean Found = VariableGet(VariableName, out pq);
                if (!Found)
                {
                    ResultLine = "Variable " + VariableName + " not found";
                }
                */
                VariableGet(VariableName, out pq);
            }
            else if (CommandLine[0] == '(')
            { // paranteses
                CommandLine = CommandLine.Substring(1).TrimStart(); // Skip start parantes '('

                pq = ParseExpression(ref CommandLine, ref ResultLine);

                CommandLine = CommandLine.TrimStart();
                TokenString.ParseChar(')', ref CommandLine, ref ResultLine);
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

        #endregion Physical Expression parser methods
    }


}
