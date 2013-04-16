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
using System.Runtime.Serialization;

namespace PhysicalCalculator.Identifiers
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
        List<PhysicalQuantityFunctionParam> Parameterlist { get; }

        String ParameterlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam parameter);

        Boolean Evaluate(CalculatorEnviroment localContext, List<IPhysicalQuantity> parameterlist, out IPhysicalQuantity functionResult, ref String resultLine);
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

        public virtual String ToListString(String name)
        {
            return String.Format("{0} {1}", identifierkind.ToString(), name);
        }

        public void WriteToTextFile(String name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(name));
        }
    }

    class NamedSystem : NametableItem 
    {
        public override IdentifierKind identifierkind { get { return IdentifierKind.UnisSystem; } }

        public IUnitSystem UnitSystem;

        public NamedSystem(String name)
        {
            UnitSystem NewUnitSystem = new UnitSystem(name, null);

            UnitSystem = NewUnitSystem;
            Physics.Default_UnitSystem_Stack.Push(NewUnitSystem);
        }

        public override String ToListString(String name)
        {
            return String.Format("system {0}", name);
        }
    }

    class NamedUnit : NametableItem 
    {
        public override IdentifierKind identifierkind { get { return IdentifierKind.Unit; } }

        public IPhysicalUnit pu;

        public CalculatorEnviroment Enviroment = null;

        public NamedUnit(IUnitSystem unitSystem, String name, IPhysicalUnit physicalUnit, CalculatorEnviroment enviroment = null /* = null */)
        {
            this.Enviroment = enviroment;
            if (physicalUnit != null)
            {
                this.pu = physicalUnit;
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSystem);
            }
        }

        public NamedUnit(IUnitSystem unitSystem, String name, IPhysicalQuantity physicalQuantity, CalculatorEnviroment enviroment /* = null */)
        {
            this.Enviroment = enviroment;
            if (physicalQuantity != null)
            {
                if ((unitSystem == null) && (physicalQuantity.Unit != null))
                {
                    unitSystem = physicalQuantity.Unit.System;
                }

                if (physicalQuantity.Value != 0 && physicalQuantity.Value != 1)
                {
                    this.pu = MakeScaledUnit(name, unitSystem, physicalQuantity.Unit, physicalQuantity.Value);
                }
                else
                {
                    this.pu = physicalQuantity.Unit;
                }
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSystem);
            }
        }

        private static IBaseUnit MakeBaseUnit(String name, IUnitSystem unitSystem)
        {
            if (unitSystem == null)
            {
                unitSystem = new UnitSystem(name + "_system", null, null, null, null);
            }
            
            if (unitSystem != null)
            {
                int NoOfBaseUnits = 0;
                if (unitSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = unitSystem.BaseUnits.Length;
                }
                IBaseUnit[] baseunitarray = new BaseUnit[NoOfBaseUnits + 1];
                if (NoOfBaseUnits > 0)
                {
                    unitSystem.BaseUnits.CopyTo(baseunitarray, 0);
                }
                baseunitarray[NoOfBaseUnits] = new BaseUnit(0, name, name);
                UnitSystem uso = unitSystem as UnitSystem;
                uso.BaseUnits = baseunitarray;
                return baseunitarray[NoOfBaseUnits];
            }

            return null;
        }

        private static IConvertibleUnit MakeScaledUnit(String name, IUnitSystem unitSystem, IPhysicalUnit primaryUnit, Double scaleFactor)
        {
            if (unitSystem == null)
            {
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[1];
                convertibleunitarray[0] = new ConvertibleUnit(name, name, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor));
                unitSystem = new UnitSystem(name + "_system", null, null, null, convertibleunitarray);
                return convertibleunitarray[0];
            }
            else
            {
                int NoOfConvertibleUnits = 0;
                if (unitSystem.ConvertibleUnits != null)
                {
                    NoOfConvertibleUnits = unitSystem.ConvertibleUnits.Length;
                }
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[NoOfConvertibleUnits + 1];
                if (NoOfConvertibleUnits > 0)
                {
                    unitSystem.ConvertibleUnits.CopyTo(convertibleunitarray, 0);
                }
                convertibleunitarray[NoOfConvertibleUnits] = new ConvertibleUnit(name, name, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor));
                UnitSystem uso = unitSystem as UnitSystem;
                uso.ConvertibleUnits = convertibleunitarray;
                return convertibleunitarray[NoOfConvertibleUnits];
            }
        }

        private static IBaseUnit MakeBaseUnit(String name)
        {
            return MakeBaseUnit(name, null);
        }

        public override String ToListString(String name)
        {
            String Unitname = String.Format("{0}", name);

            if ((pu == null) || (pu.Kind == UnitKind.BaseUnit))
            {
                return String.Format("unit {0}", Unitname); 
            }
            else
            {
                if (pu.Kind == UnitKind.ConvertibleUnit)
                {
                    ConvertibleUnit cu = (ConvertibleUnit)pu;
                    Double val = cu.Conversion.ConvertToPrimaryUnit(1);

                    if (val != 1.0 && name.Equals(cu.Symbol))
                    {   // User defined scaled unit
                        CultureInfo cultureInfo = null;
                        if (Enviroment != null)
                        {
                            cultureInfo = Enviroment.CurrentCultureInfo;
                        }

                        return String.Format("unit {0} = {1} {2}", Unitname, val.ToString(cultureInfo), cu.PrimaryUnit.ToString());
                    }
                }

                return String.Format("unit {0} = {1}", Unitname, pu.ToString());
            } 
        }
    }

    class NamedVariable : PhysicalQuantity, INametableItem
    {
        public virtual IdentifierKind identifierkind { get { return IdentifierKind.Variable; } }

        public NamedVariable(IPhysicalQuantity somephysicalquantity)
            : base(somephysicalquantity)
        {
        }

        public virtual String ToListString(String name)
        {
            return String.Format("var {0} = {1}", name, this.ToString());
        }

        public void WriteToTextFile(String name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(name));
        }
    }

    public enum EnviromentKind
    {
        Unknown = 0,
        NamespaceEnv = 1,
        FunctionEnv = 2
    }

    public enum IdentifierKind
    {
        Unknown = 0,
        Enviroment = 1,
        Variable = 2,
        Function = 3,
        UnisSystem = 4,
        Unit = 5
    }

    public enum CommandPaserState
    {
        Unknown = 0,
        ExecuteCommandLine = 1,
        ReadFunctionParameterList = 2,
        ReadFunctionParameters = 3,
        ReadFunctionParameter = 4,
        ReadFunctionParametersOptional = 5,
        ReadFunctionBlock = 6,
        ReadFunctionBody = 7  
    }
    
    public enum VariableDeclarationEnviroment
    {
        Unknown = 0,
        Global = 1,
        Outher = 2,
        Local = 3
    }

    public enum FormatProviderKind
    {
        InvariantFormatProvider = 0,
        DefaultFormatProvider = 1,
        InheritedFormatProvider = 2
    }

    [Serializable]
    public class NamedItemTable : Dictionary<String, INametableItem>
    {
        public NamedItemTable()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        protected NamedItemTable(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
        }

        public Boolean AddItem(String itemName, INametableItem itemValue)
        {
            Debug.Assert(!this.ContainsKey(itemName));

            this.Add(itemName, itemValue);

            return true;
        }

        public Boolean SetItem(String itemName, INametableItem itemValue)
        {
            if (this.ContainsKey(itemName))
            {
                this[itemName] = itemValue;
            }
            else
            {
                this.Add(itemName, itemValue);
            }

            return true;
        }

        public Boolean RemoveItem(String itemName)
        {
            return this.Remove(itemName);
        }

        public Boolean ClearItems()
        {
            this.Clear();

            return true;
        }

        public Boolean GetItem(String itemName, out INametableItem itemValue)
        {
            Debug.Assert(itemName != null);

            //return this.TryGetValue(itemName, out itemValue);
            Boolean Found = this.ContainsKey(itemName);
            if (Found)
            {
                itemValue = this[itemName];
            }
            else
            {
                itemValue = null;
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

            // FunctionToParseInfo.functionName = functionName;
            //FunctionToParseInfo.Function = new PhysicalQuantityCommandsFunction();
            //CurrentContext.ParseState = CommandPaserState.ReadFunctionParameterList;

        }

        public String Name = null;
        public EnviromentKind enviromentkind = EnviromentKind.Unknown;
        public CalculatorEnviroment OuterContext = null;

        public FormatProviderKind FormatProviderSource = FormatProviderKind.DefaultFormatProvider;

        public CalculatorEnviroment()
        {
        }

        public CalculatorEnviroment(string name, EnviromentKind enviromentkind)
            : this(null, name, enviromentkind)
        {
        }

        public CalculatorEnviroment(CalculatorEnviroment outerContext, string name, EnviromentKind enviromentkind)
        {
            this.OuterContext = outerContext;
            this.Name = name;
            this.enviromentkind = enviromentkind;
        }

        public VariableDeclarationEnviroment DefaultDeclarationEnviroment = VariableDeclarationEnviroment.Local;

        public CultureInfo CurrentCultureInfo
        {
            get
            {
                if (FormatProviderSource == FormatProviderKind.InvariantFormatProvider)
                {
                    return CultureInfo.InvariantCulture;
                }
                else if (FormatProviderSource == FormatProviderKind.InheritedFormatProvider)
                {
                    if (this.OuterContext != null)
                    {
                        return this.OuterContext.CurrentCultureInfo;
                    }
                }

                return null;
            }
        }
        public NamedItemTable NamedItems = new NamedItemTable();

        public CommandPaserState ParseState = CommandPaserState.ExecuteCommandLine;
        public FunctionParseInfo FunctionToParseInfo = null;  

        public TraceLevels _OutputTracelevel = TraceLevels.Normal;

        public TraceLevels OutputTracelevel { get { return _OutputTracelevel; } set { _OutputTracelevel = value; } }

        #region INameTableItem interface implementation

        public override IdentifierKind identifierkind { get { return IdentifierKind.Enviroment; } }

        public override String ToListString(String name)
        {
            return String.Format("Namespace {0}", name);
        }

        #endregion INameTableItem interface implementation

        public Boolean FindNameSpace(String nameSpaceName, out CalculatorEnviroment context)
        {
            // Check this namespace
            if (Name != null && Name.Equals(nameSpaceName))
            {
                context = this;
                return true;
            }
            else
            // Check outher context
            if (OuterContext != null)
            {
                return OuterContext.FindNameSpace(nameSpaceName, out context);
            }

            context = null;
            return false;
        }

        public void BeginParsingFunction(string functionName)
        {
            FunctionToParseInfo = new FunctionParseInfo(functionName);
            ParseState = CommandPaserState.ReadFunctionParameterList; 
        }

        public void EndParsingFunction()
        {
            FunctionToParseInfo = null;
            ParseState = CommandPaserState.ExecuteCommandLine;
        }

        #region Common Identifier access

        public Boolean SetLocalIdentifier(String identifierName, INametableItem item)
        {
            // Update or add local item
            return NamedItems.SetItem(identifierName, item);
        }

        public Boolean FindLocalIdentifier(String identifierName, out INametableItem item)
        {
            // Check local items
            return NamedItems.GetItem(identifierName, out item);
        }

        public Boolean FindIdentifier(String identifierName, out IEnviroment foundInContext, out INametableItem item)
        {
            // Check local items
            Boolean Found = FindLocalIdentifier(identifierName, out item);
            if (Found)
            {
                foundInContext = this;
                return true;
            }
            else
            // Check outher context
            if (OuterContext != null)
            {
                return OuterContext.FindIdentifier(identifierName, out foundInContext, out item);
            }

            foundInContext = null;
            return false;
        }

        public String ListIdentifiers(Boolean forceListContextName = false)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            Boolean HasItemsToShow = (NamedItems.Count > 0);
            Boolean ListContextName = forceListContextName | HasItemsToShow;
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

        public Boolean RemoveLocalIdentifier(String identifierName)
        {
            // Check local items
            return NamedItems.RemoveItem(identifierName);
        }

        public Boolean ClearLocalIdentifiers()
        {
            NamedItems.ClearItems();

            return true;
        }

        #endregion Common Identifier access

        #region Function Identifier access

        public Boolean FunctionFind(String functionName, out IFunctionEvaluator functionEvaluator)
        {
            IEnviroment context;
            INametableItem Item;

            Boolean Found = FindIdentifier(functionName, out context, out Item) && Item.identifierkind == IdentifierKind.Function;
            if (Found)
            {
                functionEvaluator = Item as IFunctionEvaluator;
            }
            else
            {
                functionEvaluator = null;
            }

            return Found;
        }

        #endregion Function Identifier access

        #region Unit Identifier access

        public Boolean SystemSet(String systemName, out INametableItem systemItem)
        {
            // Find identifier 
            IEnviroment context;
            Boolean Found = FindIdentifier(systemName, out context, out systemItem);

            if (Found && (context == this) && (systemItem.identifierkind != IdentifierKind.UnisSystem))
            {   // Found locally but is not a system; Can't set as system
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            // Either is identifier a system in some context; set it to specified value
            // or identifier not found; No local identifier with that name, Declare local system
            systemItem = new NamedSystem(systemName);
            return context.SetLocalIdentifier(systemName, systemItem);
        }

        public Boolean UnitSet(IUnitSystem unitSystem, String unitName, IPhysicalQuantity unitValue, out INametableItem unitItem)
        {
            // Find identifier 
            IEnviroment context;
            Boolean Found = FindIdentifier(unitName, out context, out unitItem);

            if (Found && (context == this) && (unitItem.identifierkind != IdentifierKind.Unit))
            {   // Found locally but is not a unit; Can't set as unit
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            // Either is identifier an unit in some context; set it to specified value
            // or identifier not found; No local identifier with that name, Declare local unit
            if (unitSystem == null)
            {
                if (unitValue != null && unitValue.Unit != null)
                {   // Is same system as values unit
                    unitSystem = unitValue.Unit.System;
                }

                if (unitSystem == null)
                {   // Is first unit in a new system
                    INametableItem SystemItem;
                    if (SystemSet(unitName + "_system", out SystemItem))
                    {
                        unitSystem = ((NamedSystem)SystemItem).UnitSystem;
                    }
                }
            }

            unitItem = new NamedUnit(unitSystem, unitName, unitValue, this);
            return context.SetLocalIdentifier(unitName, unitItem);
        }

        public Boolean UnitGet(String unitName, out IPhysicalUnit unitValue, ref String resultLine)
        {
            // Find identifier 
            IEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(unitName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.Unit)
            {   // Identifier is a unit in some context; Get it
                NamedUnit nu = Item as NamedUnit;

                unitValue = nu.pu;
                return true;
            }
            else
            {
                unitValue = null;
                resultLine = "Unit '" + unitName + "' not found";
                return false;
            }
        }

        #endregion Unit Identifier access

        #region Variable Identifier access

        public Boolean VariableSetLocal(String variableName, IPhysicalQuantity variableValue)
        {
            // Find identifier 
            INametableItem Item;
            Boolean Found = FindLocalIdentifier(variableName, out Item);
            if (Found && Item.identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in local context; set it to specified value
                SetLocalIdentifier(variableName, new NamedVariable(variableValue));
            }
            else
            {
                if (Found && Item.identifierkind != IdentifierKind.Variable)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local identifier with that name, Declare local variable
                    this.NamedItems.Add(variableName, new NamedVariable(variableValue));
                }
            }

            return true;
        }

        public Boolean VariableSet(String variableName, IPhysicalQuantity variableValue)
        {
            // Find identifier 
            IEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(variableName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in some context; set it to specified value
                context.SetLocalIdentifier(variableName, new NamedVariable(variableValue));
            }
            else
            {
                if (Found && Item.identifierkind != IdentifierKind.Variable && context == this)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local function with that name, Declare local variable
                    this.NamedItems.Add(variableName, new NamedVariable(variableValue));
                }
            }

            return true;
        }

        public Boolean VariableGet(String variableName, out IPhysicalQuantity variableValue, ref String resultLine)
        {
            // Find identifier 
            IEnviroment context;
            INametableItem Item;
            Boolean Found = FindIdentifier(variableName, out context, out Item);
            if (Found && Item.identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in some context; Get it
                variableValue = Item as IPhysicalQuantity;
                return true;
            }
            else
            {
                variableValue = null;
                resultLine = "Variable '" + variableName + "' not found";
                return false;
            }
        }

        #endregion Variable Identifier access

    }
}
