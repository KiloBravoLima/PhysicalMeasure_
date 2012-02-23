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
using PhysicalCalculator.Function;

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

        String ParamlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam param);

        Boolean Evaluate(CalculatorEnviroment LocalContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity FunctionResult, ref String ResultLine);
    }

    public interface ICommandsEvaluator : IFunctionEvaluator
    {
        List<String> Commands { get; set; }
    }

    public abstract class NametableItem : INametableItem
    {
        public abstract IdentifierKind identifierkind { get; }

        /*
        public NametableItem()
        {
        }
        */

        public virtual String ToListString(String Name)
        {
            return String.Format("{0} {1}", identifierkind.ToString(), Name);
        }

        public void WriteToTextFile(String Name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(Name));
        }
    }

    class NamedSystem : NametableItem, INametableItem
    {
        public override IdentifierKind identifierkind { get { return IdentifierKind.system; } }

        public IUnitSystem UnitSystem;

        public NamedSystem(String Name)
        {
            UnitSystem NewUnitSystem = new UnitSystem(Name, null);

            UnitSystem = NewUnitSystem;
            Physics.Default_UnitSystem_Stack.Push(NewUnitSystem);
        }

        public override String ToListString(String Name)
        {
            return String.Format("system {0}", Name);
        }
    }

    class NamedUnit : NametableItem, INametableItem
    {
        public override IdentifierKind identifierkind { get { return IdentifierKind.unit; } }

        public IPhysicalUnit pu;

        public NamedUnit(IUnitSystem UnitSystem, String Name, IPhysicalUnit pu)
        {
            if (pu != null)
            {
                this.pu = pu;
            }
            else
            {
                this.pu = MakeBaseUnit(Name, UnitSystem);
            }
        }

        public NamedUnit(IUnitSystem UnitSystem, String Name, IPhysicalQuantity pq)
        {
            if (pq != null)
            {
                if ((UnitSystem == null) && (pq.Unit != null))
                {
                    UnitSystem = pq.Unit.System;
                }

                if (pq.Value != 0 && pq.Value != 1)
                {
                    this.pu = MakeScaledUnit(Name, UnitSystem, pq.Unit, pq.Value);
                }
                else
                {
                    this.pu = pq.Unit;
                }
            }
            else
            {
                this.pu = MakeBaseUnit(Name, UnitSystem);
            }
        }

        private static IBaseUnit MakeBaseUnit(String Name, IUnitSystem us)
        {
            if (us == null)
            {
                us = new UnitSystem(Name + "_system", null, null, null, null);
            }
            
            if (us != null)
            {
                int NoOfBaseUnits = 0;
                if (us.BaseUnits != null)
                {
                    NoOfBaseUnits = us.BaseUnits.Length;
                }
                IBaseUnit[] baseunitarray = new BaseUnit[NoOfBaseUnits + 1];
                if (NoOfBaseUnits > 0)
                {
                    us.BaseUnits.CopyTo(baseunitarray, 0);
                }
                baseunitarray[NoOfBaseUnits] = new BaseUnit(0, Name, Name);
                UnitSystem uso = us as UnitSystem;
                uso.BaseUnits = baseunitarray;
                return baseunitarray[NoOfBaseUnits];
            }

            return null;
        }

        private static IConvertibleUnit MakeScaledUnit(String Name, IUnitSystem us, IPhysicalUnit PrimaryUnit, Double ScaleFactor)
        {
            if (us == null)
            {
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[1];
                convertibleunitarray[0] = new ConvertibleUnit(Name, Name, PrimaryUnit, new ScaledValueConversion(1.0 / ScaleFactor));
                us = new UnitSystem(Name + "_system", null, null, null, convertibleunitarray);
                return convertibleunitarray[0];
            }
            else
            {
                int NoOfConvertibleUnits = 0;
                if (us.ConvertibleUnits != null)
                {
                    NoOfConvertibleUnits = us.ConvertibleUnits.Length;
                }
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[NoOfConvertibleUnits + 1];
                if (NoOfConvertibleUnits > 0)
                {
                    us.ConvertibleUnits.CopyTo(convertibleunitarray, 0);
                }
                convertibleunitarray[NoOfConvertibleUnits] = new ConvertibleUnit(Name, Name, PrimaryUnit, new ScaledValueConversion(1.0 / ScaleFactor));
                UnitSystem uso = us as UnitSystem;
                uso.ConvertibleUnits = convertibleunitarray;
                return convertibleunitarray[NoOfConvertibleUnits];
            }
        }

        private static IBaseUnit MakeBaseUnit(String Name)
        {
            return MakeBaseUnit(Name, null);
        }

        public override String ToListString(String Name)
        {
            String Unitname = String.Format("{0}", Name);

            if ((pu == null) || (pu.Kind == UnitKind.ukBaseUnit))
            {
                return String.Format("unit {0}", Unitname); 
            }
            else
            {
                if (pu.Kind == UnitKind.ukConvertibleUnit)
                {
                    ConvertibleUnit cu = (ConvertibleUnit)pu;
                    Double val = cu.Conversion.ConvertToPrimaryUnit(1);

                    if (val != 1.0 && Name.Equals(cu.Symbol))
                    {   // User defined scaled unit
                        return String.Format("unit {0} = {1} {2}", Unitname, val.ToString(CultureInfo.InvariantCulture), cu.PrimaryUnit.ToString());
                    }
                }

                return String.Format("unit {0} = {1}", Unitname, pu.ToString());
            } 
        }
    }

    class NamedVariable : PhysicalQuantity, INametableItem
    {
        public virtual IdentifierKind identifierkind { get { return IdentifierKind.variable; } }

        public NamedVariable(IPhysicalQuantity somephysicalquantity)
            : base(somephysicalquantity)
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

    public enum EnviromentKind
    {
        unknown = 0,
        namespaceenv = 1,
        functionenv = 2
    }

    public enum IdentifierKind
    {
        unknown = 0,
        enviroment = 1,
        variable = 2,
        function = 3,
        system = 4,
        unit = 5
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
    
    public enum VariableDeclarationEnviroment
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

    public class CalculatorEnviroment : NametableItem, IEnviroment
    {
        public class FunctionParseInfo
        {
            public ICommandsEvaluator Function = null;
            public String FunctionName = null;
            //public IEnviroment Enviroment = null;
            public INametableItem RedefineItem = null; 

            public FunctionParseInfo(string NewFunctionName)
            {
                this.FunctionName = NewFunctionName;
                this.Function = new PhysicalQuantityCommandsFunction();
            }

            // FunctionToParseInfo.FunctionName = FunctionName;
            //FunctionToParseInfo.Function = new PhysicalQuantityCommandsFunction();
            //CurrentContext.ParseState = CommandPaserState.readfunctionparamlist;

        }

        public String Name = null;
        public EnviromentKind enviromentkind = EnviromentKind.unknown;
        public CalculatorEnviroment OuterContext = null;

        public CalculatorEnviroment()
        {
        }

        public CalculatorEnviroment(string name, EnviromentKind enviromentkind)
            : this(null, name, enviromentkind)
        {
        }

        public CalculatorEnviroment(CalculatorEnviroment outercontext, string name, EnviromentKind enviromentkind)
        {
            this.OuterContext = outercontext;
            this.Name = name;
            this.enviromentkind = enviromentkind;
        }

        public VariableDeclarationEnviroment DefaultDeclarationEnviroment = VariableDeclarationEnviroment.local;

        public NamedItemTable NamedItems = new NamedItemTable();

        public CommandPaserState ParseState = CommandPaserState.executecommandline;
        public FunctionParseInfo FunctionToParseInfo = null;  

        public Tracelevel _OutputTracelevel = Tracelevel.normal;

        public Tracelevel OutputTracelevel { get { return _OutputTracelevel; } set { _OutputTracelevel = value; } }

        #region INameTableItem interface implementation

        public override IdentifierKind identifierkind { get { return IdentifierKind.enviroment; } }

        public override String ToListString(String Name)
        {
            return String.Format("Namespace {0}", Name);
        }

        #endregion INameTableItem interface implementation

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

        public void BeginParsingFunction(string FunctionName)
        {
            FunctionToParseInfo = new FunctionParseInfo(FunctionName);
            ParseState = CommandPaserState.readfunctionparamlist; 
        }

        public void EndParsingFunction()
        {
            FunctionToParseInfo = null;
            ParseState = CommandPaserState.executecommandline;
        }

        #region Common Identifier access

        public Boolean SetLocalIdentifier(String IdentifierName, INametableItem Item)
        {
            // Update or add local item
            return NamedItems.SetItem(IdentifierName, Item);
        }

        public Boolean FindLocalIdentifier(String IdentifierName, out INametableItem Item)
        {
            // Check local items
            return NamedItems.GetItem(IdentifierName, out Item);
        }

        public Boolean FindIdentifier(String IdentifierName, out IEnviroment FoundInContext, out INametableItem Item)
        {
            // Check local items
            Boolean Found = FindLocalIdentifier(IdentifierName, out Item);
            if (Found)
            {
                FoundInContext = this;
                return true;
            }
            else
            // Check outher context
            if (OuterContext != null)
            {
                return OuterContext.FindIdentifier(IdentifierName, out FoundInContext, out Item);
            }

            FoundInContext = null;
            return false;
        }

        public String ListIdentifiers(Boolean ForceListContextName = false)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            Boolean HasItemsToShow = (NamedItems.Count > 0);
            Boolean ListContextName = ForceListContextName | HasItemsToShow;
            String ListStr;
            if (OuterContext != null)
            {
                ListStr = OuterContext.ListIdentifiers(ListContextName);
                ListStringBuilder.Append(ListStr);
                // ?? 2012-01-15
                ListStringBuilder.AppendLine();
            }
            if (ListContextName)
            {
                if (OuterContext != null)
                {
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.AppendFormat("{0}:", Name);
                if (HasItemsToShow)
                {
                    ListStringBuilder.AppendLine();

                    ListStr = NamedItems.ListItems();
                    ListStringBuilder.Append(ListStr);
                }
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
            IEnviroment context;
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

        public Boolean SystemSet(String SystemName, out INametableItem SystemItem)
        {
            // Find identifier 
            IEnviroment context;
            Boolean Found = FindIdentifier(SystemName, out context, out SystemItem);

            if (Found && (context == this) && (SystemItem.identifierkind != IdentifierKind.system))
            {   // Found locally but is not a system; Can't set as system
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            // Either is identifier a system in some context; set it to specified value
            // or identifier not found; No local identifier with that name, Declare local system
            SystemItem = new NamedSystem(SystemName);
            return context.SetLocalIdentifier(SystemName, SystemItem);
        }

        public Boolean UnitSet(IUnitSystem UnitSystem, String UnitName, IPhysicalQuantity UnitValue, out INametableItem UnitItem)
        {
            // Find identifier 
            IEnviroment context;
            Boolean Found = FindIdentifier(UnitName, out context, out UnitItem);

            if (Found && (context == this) && (UnitItem.identifierkind != IdentifierKind.unit))
            {   // Found locally but is not a unit; Can't set as unit
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            // Either is identifier an unit in some context; set it to specified value
            // or identifier not found; No local identifier with that name, Declare local unit
            if (UnitSystem == null)
            {
                if (UnitValue != null && UnitValue.Unit != null)
                {   // Is same system as values unit
                    UnitSystem = UnitValue.Unit.System;
                }

                if (UnitSystem == null)
                {   // Is first unit in a new system
                    INametableItem SystemItem;
                    if (SystemSet(UnitName + "_system", out SystemItem))
                    {
                        UnitSystem = ((NamedSystem)SystemItem).UnitSystem;
                    }
                }
            }

            UnitItem = new NamedUnit(UnitSystem, UnitName, UnitValue);
            return context.SetLocalIdentifier(UnitName, UnitItem);
        }

        public Boolean UnitGet(String UnitName, out IPhysicalUnit UnitValue, ref String ResultLine)
        {
            // Find identifier 
            IEnviroment context;
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
            IEnviroment context;
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
            IEnviroment context;
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
