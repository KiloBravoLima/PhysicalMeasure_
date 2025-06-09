using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using PhysicalMeasure;

using TokenParser;
using PhysicalCalculator.Identifiers;

namespace PhysicalCalculator.Function
{
    /****
     
     * 
    //public delegate DateTime DateTimeFunction(...);
    public delegate DateTime ZeroParamDateTimeFunction();
    / **
    public delegate DateTime  UnaryDateTimeFunction_PQ(IQuantity pq);
    public delegate DateTime  BinaryDateTimeFunction_PQ_SB(IQuantity pq, SByte sb);
    public delegate DateTime  BinaryDateTimeFunction_PQ_PQ(IQuantity pq1, IQuantity pq2);
    ** /
    //public delegate DateTime  TernaryDateTimeFunction_PQ_PQ(IQuantity pq1, IQuantity pq2, IQuantity pq3);

    abstract class DateTimeFunction : NametableItem, IFunctionEvaluator<DateTime>
    {
        override public IdentifierKind Identifierkind => IdentifierKind.Function;

        override public String ToListString(String Name) => String.Format($"Func {Name}({ParameterlistStr()}) // BuildIn ");

        public CalculatorEnvironment StaticOuterContext { get; }
        public CalculatorEnvironment DynamicOuterContext { get; }

        public DateTimeFunction(CalculatorEnvironment staticOuterContext)
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

        public Boolean CheckParams(List<Quantity> actualParameterlist, ref String resultLine)
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

        abstract public Boolean Evaluate(CalculatorEnvironment localContext, out DateTime functionResult, ref String resultLine);
        virtual public Boolean Evaluate(CalculatorEnvironment localContext, List<Quantity> dummy_parameterlist, out DateTime functionResult, ref String resultLine)
        {
            return Evaluate(localContext, out functionResult, ref resultLine);
        }
    }

    class DateTimeZeroParamFunction : DateTimeFunction
    {
        Func<DateTime> F;

        public DateTimeZeroParamFunction(CalculatorEnvironment staticOuterContext, Func<DateTime> func)
            : base(staticOuterContext)
        {
            F = func;
        }

        override public Boolean Evaluate(CalculatorEnvironment localContext, out DateTime functionResult, ref String resultLine)
        {
            functionResult = F();
            return true;
        }

        / **
        override public Boolean Evaluate(CalculatorEnvironment localContext, List<Quantity> dummy_parameterlist, out DateTime functionResult, ref String resultLine)
        {
            functionResult = F();
            return true;
        }
        ** /

    }



    ****/

}
