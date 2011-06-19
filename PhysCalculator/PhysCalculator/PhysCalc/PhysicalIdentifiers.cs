using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using TokenParser;
using CommandParser;

using PhysicalMeasure;
using PhysicalCalculator.Expression;

namespace PhysicalCalculator.Identifers
{
    public class PhysicalQuantityFunctionParam
    {
        public String Name;
        public IPhysicalUnit Unit;

        public PhysicalQuantityFunctionParam(String Name, IPhysicalUnit Unit)
        {
            this.Name = Name;
            this.Unit = Unit;
        }
    }

    public interface INametableItem
    {
        IdentifierKind identifierkind { get; }

        String ToListString(String Name);
        void WriteToTextFile(String Name, System.IO.StreamWriter file);
    }

    public interface IFunctionEvaluator : INametableItem
    {
        List<PhysicalQuantityFunctionParam> Paramlist { get; }

        List<String> Commands { get; set; }

        String ParamlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam param);

        Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine);
    }

    class NamedUnit : INametableItem
    {
        public virtual IdentifierKind identifierkind { get { return IdentifierKind.unit; } }

        public IPhysicalUnit pu;

        public NamedUnit(String Name, IPhysicalUnit pu)
        {
            if (pu != null)
            {
                this.pu = pu;
            }
            else
            {
                this.pu = MakeBaseUnit(Name);
            }

        }

        public NamedUnit(String Name, IPhysicalQuantity pq)
        {
            if (pq != null)
            {
                if (pq.Value != 0)
                {
                    this.pu = new ConvertibleUnit(Name, Name, pq.Unit, new ScaledValueConversion(1.0/pq.Value));
                }
                else
                {
                    this.pu = pq.Unit;
                }
            }
            else
            {
                this.pu = MakeBaseUnit(Name);
            }
        }

        private static BaseUnit MakeBaseUnit(String Name)
        {
            BaseUnit[] baseunitarray = new BaseUnit[1];
            baseunitarray[0] = new BaseUnit(0, Name, Name);

            IUnitSystem us = new UnitSystem(Name + "_System", null, baseunitarray);

            return baseunitarray[0];
        }

        public virtual String ToListString(String Name)
        {
            return String.Format("Unit {0} = {1}", Name, pu.ToPrintString());
        }

        public void WriteToTextFile(String Name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(Name));
        }
    }

    class NamedVariable : PhysicalQuantity, INametableItem
    {
        public virtual IdentifierKind identifierkind { get { return IdentifierKind.variable; } }

        public NamedVariable(IPhysicalQuantity somephysicalquantity)
            : base(somephysicalquantity.Value, somephysicalquantity.Unit)
        {
        }

        public virtual String ToListString(String Name)
        {
            return String.Format("var {0} = {1}", Name, this.ToString());
        }

        public void WriteToTextFile(String Name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(Name));
        }
    }

    public enum IdentifierKind
    {
        unknown = 0,
        enviroment = 1,
        variable = 2,
        function = 3,
        unit = 4
    }

    public enum CommandPaserState
    {
        unknown = 0,
        executecommandline = 1,
        readfunctionparamlist = 2,
        readfunctionparams = 3,
        readfunctionparam = 4,
        readfunctionparamsopt = 5,
        readfunctionblock = 6,
        readfunctionbody = 7  
    }
    
    public enum VariableDeclerationEnviroment
    {
        unknown = 0,
        global = 1,
        outher = 2,
        local = 3
    }

    public class NamedItemTable : Dictionary<String, INametableItem>
    {
        public NamedItemTable()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public Boolean AddItem(String ItemName, INametableItem ItemValue)
        {
            Debug.Assert(!this.ContainsKey(ItemName));

            this.Add(ItemName, ItemValue);

            return true;
        }

        public Boolean SetItem(String ItemName, INametableItem ItemValue)
        {
            if (this.ContainsKey(ItemName))
            {
                this[ItemName] = ItemValue;
            }
            else
            {
                this.Add(ItemName, ItemValue);
            }

            return true;
        }

        public Boolean RemoveItem(String ItemName)
        {
            return this.Remove(ItemName);
        }

        public Boolean ClearItems()
        {
            this.Clear();

            return true;
        }

        public Boolean GetItem(String ItemName, out INametableItem ItemValue)
        {
            Debug.Assert(ItemName != null);

            //return this.TryGetValue(ItemName, out ItemValue);
            Boolean Found = this.ContainsKey(ItemName);
            if (Found)
            {
                ItemValue = this[ItemName];
            }
            else
            {
                ItemValue = null;
            }
            return Found;
        }

        public String ListItems()
        {
            StringBuilder ListStringBuilder = new StringBuilder();
            try
            {
                int count = 0;
                foreach (KeyValuePair<String, INametableItem> Item in this)
                {
                    if (count > 0)
                    {
                        ListStringBuilder.AppendLine();
                    }

                    if (Item.Value != null)
                    {
                        ListStringBuilder.Append(Item.Value.ToListString(Item.Key));
                    }
                    else
                    {
                        ListStringBuilder.AppendFormat("Item {0}", Item.Key);
                    }

                    count++;
                }
            }
            catch (Exception e)
            {
                String Message = String.Format("{0} Exception Source: {1} - {2}", e.GetType().ToString(), e.Source, e.ToString());
                ListStringBuilder.AppendLine();
                ListStringBuilder.Append(Message);
                ListStringBuilder.AppendLine();

            }

            return ListStringBuilder.ToString();
        }

    }

    public class CalculatorEnviroment : IEnviroment
    {
        public String Name = null;
        public CalculatorEnviroment OuterContext = null;
        public VariableDeclerationEnviroment DefaultDeclerationEnviroment = VariableDeclerationEnviroment.local;

        public NamedItemTable NamedItems = new NamedItemTable();

        public CommandPaserState ParseState = CommandPaserState.executecommandline;
        public IFunctionEvaluator FunctionToParse = null;
        public String FunctionToParseName = null;

        public Tracelevel OutputTracelevel = Tracelevel.normal;   

        public Boolean FindNameSpace(String NameSpaceName, out CalculatorEnviroment context)
        {
            // Check this namespace
            if (Name != null && Name.Equals(NameSpaceName))
            {
                context = this;
                return true;
            }
            else
            // Check outher context
            if (OuterContext != null)
            {
                return OuterContext.FindNameSpace(NameSpaceName, out context);
            }

            context = null;
            return false;
        }

        #region Common Identifier access

        public Boolean FindLocalIdentifier(String IdentifierName, out INametableItem Item)
        {
            // Check local items
            return NamedItems.GetItem(IdentifierName, out Item);
        }

        public Boolean FindIdentifier(String IdentifierName, out CalculatorEnviroment context, out INametableItem Item)
        {
            // Check local items
            Boolean Found = FindLocalIdentifier(IdentifierName, out Item);
            if (Found)
            {
                context = this;
                return true;
            }
            else
            // Check outher context
            if (OuterContext != null)
            {
                return OuterContext.FindIdentifier(IdentifierName, out context, out Item);
            }

            context = null;
            return false;
        }

        public String ListIdentifiers()
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            String ListStr;
            if (OuterContext != null)
            {
                ListStr = OuterContext.ListIdentifiers();
                ListStringBuilder.Append(ListStr);
                ListStringBuilder.AppendLine();
            }

            ListStringBuilder.AppendFormat("{0}", Name);

            if (NamedItems.Count > 0)
            {
                ListStringBuilder.AppendLine();

                ListStr = NamedItems.ListItems();
                ListStringBuilder.Append(ListStr);
            }

            return ListStringBuilder.ToString();
        }

        public Boolean RemoveLocalIdentifier(String IdentifierName)
        {
            // Check local items
            return NamedItems.RemoveItem(IdentifierName);
        }

        public Boolean ClearLocalIdentifiers()
        {
            NamedItems.ClearItems();

            return true;
        }

        #endregion Common Identifier access

        #region Function Identifier access

        public Boolean FunctionFind(String FunctionName, out IFunctionEvaluator functionevaluator)
        {
            CalculatorEnviroment context;
            INametableItem Item;

            Boolean Found = FindIdentifier(FunctionName, out context, out Item) && Item.identifierkind == IdentifierKind.function;
            if (Found)
            {
                functionevaluator = Item as IFunctionEvaluator;
            }
            else
            {
                functionevaluator = null;
            }

            return Found;
        }

        #endregion Function Identifier access

        #region Unit Identifier access

        public Boolean UnitSet(String UnitName, IPhysicalQuantity UnitValue)
        {
            // Find identifier 
            CalculatorEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(UnitName, out context, out Item);
            if (Found && (Item.identifierkind == IdentifierKind.unit))
            {   // Identifier is a unit in some context; set it to specified value
                context.NamedItems.SetItem(UnitName, new NamedUnit(UnitName, UnitValue));
            }
            else
            {
                if (Found && context == this)
                {   // Found locally but not a unit; Can't set as unit
                    return false;
                }
                else
                {   // Variable not found; No local identifier with that name, Declare local variable
                    this.NamedItems.SetItem(UnitName, new NamedUnit(UnitName, UnitValue));
                }
            }

            return true;
        }

        public Boolean UnitGet(String UnitName, out IPhysicalUnit UnitValue, ref String ResultLine)
        {
            // Find identifier 
            CalculatorEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(UnitName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.unit)
            {   // Identifier is a unit in some context; Get it

                NamedUnit nu = Item as NamedUnit;

                UnitValue = nu.pu;
                return true;
            }
            else
            {
                UnitValue = null;
                ResultLine = "Unit '" + UnitName + "' not found";
                return false;
            }
        }

        #endregion Unit Identifier access

        #region Variable Identifier access

        public Boolean VariableSetLocal(String VariableName, IPhysicalQuantity VariableValue)
        {
            // Find identifier 
            INametableItem Item;
            Boolean Found = FindLocalIdentifier(VariableName, out Item);
            if (Found && Item.identifierkind == IdentifierKind.variable)
            {   // Identifier is a variable in local context; set it to specified value
                NamedVariable nv = Item as NamedVariable;
                nv.Value = VariableValue.Value;
                nv.Unit = VariableValue.Unit;
            }
            else
            {
                if (Found && Item.identifierkind != IdentifierKind.variable)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local identifier with that name, Declare local variable
                    this.NamedItems.Add(VariableName, new NamedVariable(VariableValue));
                }
            }

            return true;
        }

        public Boolean VariableSet(String VariableName, IPhysicalQuantity VariableValue)
        {
            // Find identifier 
            CalculatorEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(VariableName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.variable)
            {   // Identifier is a variable in some context; set it to specified value
                NamedVariable nv = Item as NamedVariable;
                nv.Value = VariableValue.Value;
                nv.Unit = VariableValue.Unit;
            }
            else
            {
                if (Found && Item.identifierkind != IdentifierKind.variable && context == this)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local function with that name, Declare local variable
                    this.NamedItems.Add(VariableName, new NamedVariable(VariableValue));
                }
            }

            return true;
        }

        public Boolean VariableGet(String VariableName, out IPhysicalQuantity VariableValue, ref String ResultLine)
        {
            // Find identifier 
            CalculatorEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(VariableName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.variable)
            {   // Identifier is a variable in some context; Get it
                VariableValue = Item as IPhysicalQuantity;
                return true;
            }
            else
            {
                VariableValue = null;
                ResultLine = "Variable '" + VariableName + "' not found";
                return false;
            }
        }

        #endregion Variable Identifier access
    }
}
