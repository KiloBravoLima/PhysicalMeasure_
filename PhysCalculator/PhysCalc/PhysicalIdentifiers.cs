using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

using PhysicalCalculator.CommandBlock;
using PhysicalCalculator.Expression;
using PhysicalCalculator.Function;

using PhysicalMeasure;


namespace PhysicalCalculator.Identifiers
{
    public class PhysicalQuantityFunctionParam
    {
        public String Name;
        public Unit Unit;

        public PhysicalQuantityFunctionParam(String Name, Unit Unit)
        {
            this.Name = Name;
            this.Unit = Unit;
        }
    }

    public interface INametableItem
    {
        IdentifierKind Identifierkind { get; }

        String ToListString(String name);
    }

    public interface IEvaluator
    {
        //String ToListString(String name);

        Boolean Evaluate(CalculatorEnvironment localContext, out Quantity result, ref String resultLine);
    }

    public interface ICommands
    {
        List<String> Commands { get; /** */ set; /*  **/ }
    }

    public interface ICommandsEvaluator : ICommands, IEvaluator
    {
    }

    public interface IFunctionEvaluator : INametableItem
    {
        CalculatorEnvironment StaticOuterContext { get; }
        CalculatorEnvironment DynamicOuterContext { get; }

        List<PhysicalQuantityFunctionParam> Parameterlist { get; }

        String ParameterlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam parameter);

        Boolean Evaluate(CalculatorEnvironment localContext, List<Quantity> parameterlist, out Quantity functionResult, ref String resultLine);
    }


    public interface IFunctionCommandsEvaluator : IFunctionEvaluator, ICommands
    {
    }

    public abstract class NametableItem : INametableItem
    {
        public abstract IdentifierKind Identifierkind { get; }

        public virtual String ToListString(String name) => $"{Identifierkind.ToString()} {name}";
    }

    class NamedSystem : NametableItem 
    {
        public override IdentifierKind Identifierkind => IdentifierKind.UnitSystem;

        public IUnitSystem UnitSystem;

        public NamedSystem(String name)
        {
            UnitSystem = new UnitSystem(name, true);
        }

        public override String ToListString(String name) => $"system {name}" ;
    }

    class NamedUnit : NametableItem 
    {
        public override IdentifierKind Identifierkind => IdentifierKind.Unit;

        public Unit pu;

        public IEnvironment Environment = null;

        public NamedUnit(IUnitSystem unitSystem, String name, String unitSymbol, Unit physicalUnit, CalculatorEnvironment environment = null /* = null */)
        {
            this.Environment = environment;
            if (physicalUnit != null)
            {
                this.pu = physicalUnit;
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSymbol, unitSystem);
            }
        }

        public NamedUnit(IUnitSystem unitSystem, String name, String unitSymbol, Quantity physicalQuantity, CalculatorEnvironment environment /* = null */)
        {
            this.Environment = environment;
            if (physicalQuantity != null)
            {
                if ((unitSystem == null) && (physicalQuantity.Unit != null))
                {
                    unitSystem = physicalQuantity.Unit.ExponentsSystem;
                }

                if (physicalQuantity.Value != 0 && physicalQuantity.Value != 1)
                {
                    this.pu = MakeScaledUnit(name, unitSymbol, unitSystem, physicalQuantity.Unit, physicalQuantity.Value);
                }
                else
                {
                    this.pu = physicalQuantity.Unit;
                }
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSymbol, unitSystem);
            }
        }

        private static BaseUnit MakeBaseUnit(String name) => MakeBaseUnit(name, null, null);

        private static BaseUnit MakeBaseUnit(String name, String unitSymbol, IUnitSystem unitSystem)
        {
            if (unitSystem == null)
            {
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => new BaseUnit[] { new BaseUnit(unitsystem, 0, name, unitSymbol) } );
                return unitSystem.BaseUnits[0];
            }
            else
            {
                int NoOfBaseUnits = 0;
                if (unitSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = unitSystem.BaseUnits.Length;
                }
                BaseUnit[] baseunitarray = new BaseUnit[NoOfBaseUnits + 1];
                if (NoOfBaseUnits > 0)
                {
                    unitSystem.BaseUnits.CopyTo(baseunitarray, 0);
                }
                baseunitarray[NoOfBaseUnits] = new BaseUnit(unitSystem, (sbyte)(NoOfBaseUnits), name, unitSymbol);
                UnitSystem uso = unitSystem as UnitSystem;
                // bool test = uso.BaseUnits.IsFixedSize;
                uso.BaseUnits = baseunitarray;
                return baseunitarray[NoOfBaseUnits];
            }
        }

        private static ConvertibleUnit MakeScaledUnit(String name, String unitSymbol, IUnitSystem unitSystem, Unit primaryUnit, Double scaleFactor)
        {
            if (unitSystem == null)
            {
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[] { new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor)) };
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => null, (unitsystem) => null, (unitsystem) => convertibleunitarray);
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
                convertibleunitarray[NoOfConvertibleUnits] = new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor));
                UnitSystem uso = unitSystem as UnitSystem;
                uso.ConvertibleUnits = convertibleunitarray;
                return convertibleunitarray[NoOfConvertibleUnits];
            }
        }


        public override String ToListString(String name)
        {
            String Unitname = name;// String.Format("{0}", name);

            if ((pu == null) || (pu.Kind == UnitKind.BaseUnit))
            {
                return $"unit {Unitname}"; 
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
                        if (Environment != null)
                        {
                            cultureInfo = Environment.CurrentCultureInfo;
                        }

                        return $"unit {Unitname} = {val.ToString(cultureInfo)} {cu.PrimaryUnit.ToString()}";
                    }
                }

                return $"unit {Unitname} = {pu.ToString()}";
            } 
        }
    }

    class NamedVariable : Quantity, INametableItem
    {
        public virtual IdentifierKind Identifierkind => IdentifierKind.Variable;

        public IEnvironment Environment = null;
        public CultureInfo CultureInfo 
        {
            get
            {
                CultureInfo cultureInfo = null;
                if (Environment != null)
                {
                    cultureInfo = Environment.CurrentCultureInfo;
                }
                return cultureInfo;           
            }
        }

        public NamedVariable(Quantity somephysicalquantity, IEnvironment environment = null)
            : base(somephysicalquantity)
        {
            this.Environment = environment;
        }

        public virtual String ToListString(String name) => $"var {name} = {this.ToString(null, CultureInfo)}";

        public void WriteToTextFile(String name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(name));
        }
    }

    class NamedConstant : NamedVariable
    {
        public override IdentifierKind Identifierkind => IdentifierKind.Constant;

        public NamedConstant(Quantity somephysicalquantity, IEnvironment environment = null)
            : base(somephysicalquantity, environment)
        {
        }

        public override String ToListString(String name) => $"constant {name} = {this.ToString(null, CultureInfo)}";
    }


    public enum EnvironmentKind
    {
        Unknown = 0,
        NamespaceEnv = 1,
        FunctionEnv = 2
    }

    public enum IdentifierKind
    {
        Unknown = 0,
        Environment = 1,
        Constant = 2,
        Variable = 3,
        Function = 4,
        UnitSystem = 5,
        Unit = 6
    }

    public enum CommandParserState
    {
        Unknown = 0,
        ExecuteCommandLine = 1,

        ReadFunctionParameterList = 2,
        ReadFunctionParameters = 3,
        ReadFunctionParameter = 4,
        ReadFunctionParametersOptional = 5,
        ReadFunctionBlock = 6,
        ReadFunctionBody = 7,

        ReadCommandBlock = 8,  
        ReadCommands = 9  
    }
    
    public enum DeclarationEnvironmentKind
    {
        Unknown = 0,
        Global = 1,
        Outher = 2,
        Local = 3
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

        public Boolean RemoveItem(String itemName) => this.Remove(itemName);

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
                        ListStringBuilder.Append($"Item {Item.Key}");
                    }

                    count++;
                }
            }
            catch (Exception e)
            {
                String Message = $"{e.GetType().ToString()} Exception Source: {e.Source} - {e.ToString()}";
                ListStringBuilder.AppendLine();
                ListStringBuilder.Append(Message);
                ListStringBuilder.AppendLine();

            }

            return ListStringBuilder.ToString();
        }

    }

    public class CalculatorEnvironment : NametableItem, IEnvironment
    {
        public class CommandBlockParseInfo
        {
            public ICommandsEvaluator CommandBlock = null;
            public int InnerBlockCount = 0;

            public CommandBlockParseInfo()
            {
                this.CommandBlock = new PhysicalQuantityCommandsBlock();
            }

            //CurrentContext.ParseState = CommandParserState.ReadFunctionParameterList;
        }

        public class FunctionParseInfo
        {
            public IFunctionCommandsEvaluator Function = null;
            public String FunctionName = null;
            //public IEnvironment Environment = null;
            public INametableItem RedefineItem = null; 

            public FunctionParseInfo(CalculatorEnvironment staticOuterContext, string NewFunctionName)
            {
                this.FunctionName = NewFunctionName;
                this.Function = new PhysicalQuantityCommandsFunction(staticOuterContext);
            }

            // FunctionToParseInfo.functionName = functionName;
            //FunctionToParseInfo.Function = new PhysicalQuantityCommandsFunction();
            //CurrentContext.ParseState = CommandParserState.ReadFunctionParameterList;
        }

        public String Name = null;
        public EnvironmentKind environmentkind = EnvironmentKind.Unknown;
        public CalculatorEnvironment OuterContext = null;

        public CalculatorEnvironment()
        {
        }

        public CalculatorEnvironment(string name, EnvironmentKind environmentkind)
            : this(null, name, environmentkind)
        {
        }

        public CalculatorEnvironment(CalculatorEnvironment outerContext, string name, EnvironmentKind environmentkind)
        {
            this.OuterContext = outerContext;
            this.Name = name;
            this.environmentkind = environmentkind;
        }

        public DeclarationEnvironmentKind DefaultDeclarationEnvironment = DeclarationEnvironmentKind.Local;

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

        public CommandParserState ParseState = CommandParserState.ExecuteCommandLine;
        public FunctionParseInfo FunctionToParseInfo = null;
        public CommandBlockParseInfo CommandBlockToParseInfo = null;
        public int CommandBlockLevel = 0;

        private TraceLevels _OutputTracelevel = TraceLevels.Normal;
        public TraceLevels OutputTracelevel { get { return _OutputTracelevel; } set { _OutputTracelevel = value; } }

        private FormatProviderKind FormatProviderSrc = FormatProviderKind.DefaultFormatProvider; 
        public FormatProviderKind FormatProviderSource { get { return FormatProviderSrc; } set { FormatProviderSrc = value; } }

        #region INameTableItem interface implementation

        public override IdentifierKind Identifierkind => IdentifierKind.Environment;

        public override String ToListString(String name) => $"Namespace {name}";

        #endregion INameTableItem interface implementation

        public Boolean FindNameSpace(String nameSpaceName, out CalculatorEnvironment context)
        {
            // Check this namespace
            if (Name != null && Name.Equals(nameSpaceName, StringComparison.OrdinalIgnoreCase))
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
            FunctionToParseInfo = new FunctionParseInfo(this, functionName);
            ParseState = CommandParserState.ReadFunctionParameterList; 
        }

        public void EndParsingFunction()
        {
            FunctionToParseInfo = null;
            ParseState = CommandParserState.ExecuteCommandLine;
        }

        public void BeginParsingCommandBlock()
        {
            CommandBlockToParseInfo = new CommandBlockParseInfo();
            ParseState = CommandParserState.ReadCommandBlock;
        }

        public void EndParsingCommandBlock()
        {
            FunctionToParseInfo = null;
            ParseState = CommandParserState.ExecuteCommandLine;
        }

        #region Common Identifier access

        // Update or add local item
        public Boolean SetLocalIdentifier(String identifierName, INametableItem item) => NamedItems.SetItem(identifierName, item);

        // Check local items
        public Boolean FindLocalIdentifier(String identifierName, out INametableItem item) => NamedItems.GetItem(identifierName, out item);

        public Boolean FindIdentifier(String identifierName, out IEnvironment foundInContext, out INametableItem item)
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

        public String ListIdentifiers(Boolean forceListContextName = false, Boolean listNamedItems  = false, Boolean listSettings = false)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            Boolean HasItemsToShow = listNamedItems && (NamedItems.Count > 0);
            Boolean ListContextName = forceListContextName | HasItemsToShow | listSettings;
            String ListStr;
            if (OuterContext != null)
            {
                ListStr = OuterContext.ListIdentifiers(ListContextName, listNamedItems, listSettings);
                ListStringBuilder.Append(ListStr);
                ListStringBuilder.AppendLine();
            }
            if (ListContextName)
            {
                if (OuterContext != null)
                {
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.Append($"{Name}:");
                if (listSettings)
                {
                    ListStringBuilder.AppendLine();
                    ListStringBuilder.AppendLine("Settings");
                    
                    switch (OutputTracelevel)
                    {
                        case TraceLevels.Debug:
                            ListStr = "Debug";
                            break;
                        case TraceLevels.Low:
                            ListStr = "Low";
                            break;
                        case TraceLevels.Normal:
                            ListStr = "Normal";
                            break;
                        case TraceLevels.High:
                            ListStr = "High";
                            break;
                        default:
                            ListStr = OutputTracelevel.ToString();
                            break;
                    }

                    ListStringBuilder.Append($"Tracelevel = {ListStr}");
                    ListStringBuilder.AppendLine();

                    switch (FormatProviderSource)
                    {
                        case FormatProviderKind.InvariantFormatProvider:
                            ListStr = "Invariant";
                            break;
                        case FormatProviderKind.DefaultFormatProvider:
                            ListStr = "Default";
                            break;
                        case FormatProviderKind.InheritedFormatProvider:
                            ListStr = "Inherited";
                            break;
                        default:
                            ListStr = FormatProviderSource.ToString(); 
                            break;
                    }
                    ListStringBuilder.Append($"FormatProvider = {ListStr}");
                }
                if (HasItemsToShow)
                {
                    ListStringBuilder.AppendLine();

                    if (listSettings)
                    {
                        ListStringBuilder.AppendLine("Items");
                    }

                    ListStr = NamedItems.ListItems();
                    ListStringBuilder.Append(ListStr);
                }
            }

            return ListStringBuilder.ToString();
        }

        // Check local items
        public Boolean RemoveLocalIdentifier(String identifierName) => NamedItems.RemoveItem(identifierName);

        public Boolean ClearLocalIdentifiers()
        {
            NamedItems.ClearItems();

            return true;
        }

        #endregion Common Identifier access

        #region Function Identifier access

        public Boolean FunctionFind(String functionName, out IFunctionEvaluator functionEvaluator)
        {
            Boolean Found = FindIdentifier(functionName, out var context, out var Item) && Item.Identifierkind == IdentifierKind.Function;
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
            Boolean Found = FindIdentifier(systemName, out var context, out systemItem);

            if (Found && (context == this) && (systemItem.Identifierkind != IdentifierKind.UnitSystem))
            {   // Found locally but is not a system; Can't set as system
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            // Either identifier is a system in some context; set it to specified value
            // or identifier not found; No local identifier with that name, Declare local system
            systemItem = new NamedSystem(systemName);
            Physics.CurrentUnitSystems.Use((systemItem as NamedSystem).UnitSystem);  
            return context.SetLocalIdentifier(systemName, systemItem);
        }



        public Boolean UnitSet(IUnitSystem unitSystem, String unitName, Quantity unitValue, String unitSymbol, out INametableItem unitItem)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(unitName, out IEnvironment context, out unitItem);

            if (Found && (context == this) && (unitItem.Identifierkind != IdentifierKind.Unit))
            {   // Found locally, but is not a unit; Can't set as unit
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
                    unitSystem = unitValue.Unit.ExponentsSystem;
                }

                /**
                if (unitSystem == null)
                {   // Is first unit in a new system
                    if (SystemSet(unitName + "_system", out INametableItem SystemItem))
                    {
                        unitSystem = ((NamedSystem)SystemItem).UnitSystem;
                    }
                }
                **/
            }

            unitItem = new NamedUnit(unitSystem, unitName, unitSymbol, unitValue, this);
            return context.SetLocalIdentifier(unitName, unitItem);
        }

        public Boolean UnitGet(String unitName, out Unit unitValue, ref String resultLine)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(unitName, out var context, out var Item);
            if (Found && Item.Identifierkind == IdentifierKind.Unit)
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

        public Boolean VariableSetLocal(String variableName, Quantity variableValue)
        {
            // Find identifier 
            Boolean Found = FindLocalIdentifier(variableName, out var Item);
            if (Found && Item.Identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in local context; set it to specified value
                SetLocalIdentifier(variableName, new NamedVariable(variableValue, this));
            }
            else
            {
                if (Found && Item.Identifierkind != IdentifierKind.Variable)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local identifier with that name, Declare local variable
                    this.NamedItems.Add(variableName, new NamedVariable(variableValue, this));
                }
            }

            return true;
        }

        public Boolean VariableSet(String variableName, Quantity variableValue)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(variableName, out var context, out var Item);
            if (Found && Item.Identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in some context; set it to specified value
                context.SetLocalIdentifier(variableName, new NamedVariable(variableValue, context as CalculatorEnvironment));
            }
            else
            {
                if (Found && Item.Identifierkind != IdentifierKind.Variable && context == this)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local function with that name, Declare local variable
                    this.NamedItems.Add(variableName, new NamedVariable(variableValue, this));
                }
            }

            return true;
        }

        public Boolean VariableGet(String variableName, out Quantity variableValue, ref String resultLine)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(variableName, out var context, out var Item);
            if (Found && ((Item.Identifierkind == IdentifierKind.Variable) || (Item.Identifierkind == IdentifierKind.Constant)))
            {   // Identifier is a variable or constant in some context; Get it
                variableValue = Item as Quantity;
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
