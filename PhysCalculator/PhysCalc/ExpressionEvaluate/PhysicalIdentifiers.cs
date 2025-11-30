using ConsolAnyColor;

using PhysicalCalculator.CommandBlock;
using PhysicalCalculator.Expression;
using PhysicalCalculator.Function;

using PhysicalMeasure;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using TokenParser;

using static PhysicalCalculator.Identifiers.CalculatorEnvironment;

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

        Boolean Evaluate(CalculatorEnvironment localContext, out OperandInfo result, ref String resultLine);
    }

    public interface ICommands
    {
        List<String> Commands { get; /* set; */ }
        /**
        void AddCommandLine();
        **/
    }

    public interface ICommandsEvaluator : ICommands, IEvaluator
    {
    }

    /****
    public interface IFunctionEvaluator : INametableItem
    {
        CalculatorEnvironment StaticOuterContext { get; }
        CalculatorEnvironment DynamicOuterContext { get; }

        List<PhysicalQuantityFunctionParam> Parameterlist { get; }

        String ParameterlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam parameter);

        Boolean Evaluate(CalculatorEnvironment localContext, List<Quantity> parameterlist, out Quantity functionResult, ref String resultLine);
    }
    ****/

    public interface IFunctionEvaluator : INametableItem
    {
        CalculatorEnvironment StaticOuterContext { get; }
        CalculatorEnvironment DynamicOuterContext { get; }


        List<PhysicalQuantityFunctionParam> Parameterlist { get; }

        String ParameterlistStr();

        void ParamListAdd(PhysicalQuantityFunctionParam parameter);

        Type ResultType { get; }

        Boolean Evaluate(CalculatorEnvironment localContext, List<OperandInfo> parameterlist, out OperandInfo functionResult, ref String resultLine);

       //  Boolean Evaluate<T>(CalculatorEnvironment localContext, List<Quantity> parameterlist, out T functionResult, ref String resultLine);
    }

    /**
    public interface IQuantityFunctionEvaluator : IFunctionEvaluator<Quantity>;

    public interface IDateTimeFunctionEvaluator : IFunctionEvaluator<DateTime>;

    // public interface IFunctionEvaluator2 : IFunctionEvaluator<DateTime>;

    **/


    public interface IFunctionCommandsEvaluator : IFunctionEvaluator, ICommands
    {
}

    public abstract class NametableItem : INametableItem
    {
        public abstract IdentifierKind Identifierkind { get; }

        public virtual String ToListString(String name) => $"{Identifierkind} {name}";
    }

    class NamedSystem : NametableItem 
    {
        public override IdentifierKind Identifierkind => IdentifierKind.UnitSystem;

        public IUnitSystem UnitSystem;

        public NamedSystem(String name)
        {
            UnitSystem = new UnitSystem(name, someIsIsolated: true, isModifiableUnitSystem: true);
        }

        public override String ToListString(String name) => $"system {name}";
    }

    class NamedUnit : NametableItem 
    {
        public override IdentifierKind Identifierkind => IdentifierKind.Unit;

        public Unit pu;

        public IEnvironment Environment = null;

        public NamedUnit(IUnitSystem unitSystem, String name, String unitSymbol, Unit physicalUnit, BaseUnitDimension? dimension, CalculatorEnvironment environment = null /* = null */)
        {
            this.Environment = environment;
            if (physicalUnit != null)
            {
                this.pu = physicalUnit;
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSymbol, unitSystem, dimension);
            }
        }

        public NamedUnit(IUnitSystem unitSystem, String name, String unitSymbol, Quantity unitValue, BaseUnitDimension? dimension, CalculatorEnvironment environment /* = null */)
        {
            this.Environment = environment;
            if (unitValue != null)
            {
                if ((unitSystem == null) && (unitValue.Unit != null))
                {
                    unitSystem = unitValue.Unit.ExponentsSystem;
                }

                if (unitValue.Value != 0 && unitValue.Value != 1)
                {
                    this.pu = MakeScaledUnit(name, unitSymbol, unitSystem, unitValue.Unit, unitValue.Value);
                }
                else
                {
                    this.pu = unitValue.Unit;
                }
            }
            else
            {
                this.pu = MakeBaseUnit(name, unitSymbol, unitSystem, dimension);
            }
        }

        public (bool, String) UpdateUnit(String unitName, Quantity unitValue)
        {

            bool wasBaseUnit = this.pu.Kind == UnitKind.BaseUnit;
            bool isBaseUnit = (unitValue == null || (unitValue.Unit?.Kind == UnitKind.BaseUnit && (unitValue.Value == 0 || unitValue.Value == 1)));

            if (wasBaseUnit && isBaseUnit)
            {
                // OK: Nothing to update
                return (true, "");
            }

            if (wasBaseUnit || isBaseUnit)
            {
                // Error:  Can't update to or from BaseUnit
                return (false, "Can't update to or from BaseUnit");
            }

            bool wasScaledUnit = this.pu.Kind == UnitKind.ConvertibleUnit;
            bool isScaledUnit = (unitValue != null && (unitValue.Value != 0 && unitValue.Value != 1));

            if (!wasScaledUnit && !isScaledUnit)
            {
                // OK:  Update pu to new unit
                this.pu = unitValue.Unit;
                return (true, "");
            }

            // Update of ConvertibleUnit 
            ConvertibleUnit cu = (ConvertibleUnit)this.pu;
            bool isSameCoversion = (unitValue.Unit == cu.PrimaryUnit) && (unitValue.Value == 1.0/cu.Conversion.LinearScale);

            if (isSameCoversion)
            {
                // OK: Nothing to update
                return (true, "");
            }

            // Error:  Can't update to or from ScaledUnit
            return (false, "Can't update to or from ScaledUnit");


            /***
            if (!wasScaledUnit || !isScaledUnit)
            {
                // Error:  Can't update to or from ScaledUnit
                return (false, "Can't update to or from ScaledUnit");
            }

            // OK:  Update pu with new PrimaryUnit and LinearScale
            ConvertibleUnit cu = (ConvertibleUnit)this.pu;
            cu.PrimaryUnit = unitValue.Unit;               Readonly!!
            cu.Conversion.LinearScale = unitValue.Value;   Readonly!!

            return (true, "");
            ***/
        }

        private static BaseUnit MakeBaseUnit(String name) => MakeBaseUnit(name, null, null);

        private static BaseUnit MakeBaseUnit(String name, String unitSymbol, IUnitSystem unitSystem, BaseUnitDimension? someDimension = null)
        {
            BaseUnitDimension dimension = (someDimension != null) ? someDimension.Value : BaseUnitDimension.Unknown;

            if (unitSystem == null)
            {
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => new BaseUnit[] { new BaseUnit(unitsystem, 0, name, unitSymbol, dimension) }, isModifiableUnitSystem: true);
                return unitSystem.BaseUnits[0];
            }
            else
            if (unitSystem.IsModifiableUnitSystem)
            {
                int NoOfBaseUnits = 0;
                sbyte indexForExistingBaseUnit = -1;
                if (unitSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = unitSystem.BaseUnits.Length;
                    indexForExistingBaseUnit = unitSystem.BaseUnits.Single(bu => bu.Name == name && bu.Symbol == unitSymbol).BaseUnitNumber;
                }
                if (indexForExistingBaseUnit >= 0)
                {
                    // Base unit already exists
                    unitSystem.BaseUnits[indexForExistingBaseUnit] = new BaseUnit(unitSystem, indexForExistingBaseUnit, name, unitSymbol, dimension); 
                    return unitSystem.BaseUnits[indexForExistingBaseUnit];
                }
                BaseUnit[] baseunitarray = new BaseUnit[NoOfBaseUnits + 1];
                if (NoOfBaseUnits > 0)
                {
                    unitSystem.BaseUnits.CopyTo(baseunitarray, 0);
                }
                baseunitarray[NoOfBaseUnits] = new BaseUnit(unitSystem, (sbyte)(NoOfBaseUnits), name, unitSymbol, dimension);
                UnitSystem uso = unitSystem as UnitSystem;
                uso.BaseUnits = baseunitarray;
                return baseunitarray[NoOfBaseUnits];
            }
            else
            {
                // try use a new unit system

                /**
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => new BaseUnit[] { new BaseUnit(unitsystem, 0, name, unitSymbol) }, isModifiableUnitSystem: true);
                return unitSystem.BaseUnits[0];
                **/

                unitSystem = new ExtentedUnitSystem(unitSystem as UnitSystem, $"{unitSystem.Name}_With_{name}_system", someExtraBaseUnitsBuilder: (newUnitsystem) => new BaseUnit[] { new BaseUnit(newUnitsystem, (sbyte)unitSystem.BaseUnits.Length, name, unitSymbol, dimension) }, someExtraNamedDerivedUnitsBuilder: null, someExtraConvertibleUnitsBuilder: null, isModifiableUnitSystem: true); 
                return unitSystem.BaseUnits[unitSystem.BaseUnits.Length-1];
            }
        }

        private static ConvertibleUnit MakeScaledUnit(String name, String unitSymbol, IUnitSystem unitSystem, Unit primaryUnit, Double scaleFactor)
        {
            if (unitSystem == null)
            {
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[] { new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor)) };
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => null, (unitsystem) => null, (unitsystem) => convertibleunitarray, isModifiableUnitSystem: true);
                return convertibleunitarray[0];
            }
            else
            if (unitSystem.IsModifiableUnitSystem)
            {
                int NoOfConvertibleUnits = 0;
                int indexForExistingConvertibleUnit = -1;
                if (unitSystem.ConvertibleUnits != null)
                {
                    NoOfConvertibleUnits = unitSystem.ConvertibleUnits.Length;
                    var existingConvertibleUnit = unitSystem.ConvertibleUnits.FirstOrDefault(cu => cu.Name == name && cu.Symbol == unitSymbol);
                    if (existingConvertibleUnit != null)
                    {
                        indexForExistingConvertibleUnit = Array.IndexOf(unitSystem.ConvertibleUnits, existingConvertibleUnit);
                    }
                }
                if (indexForExistingConvertibleUnit >= 0)
                {
                    // Convertible unit already exists
                    unitSystem.ConvertibleUnits[indexForExistingConvertibleUnit] = new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor));
                    return unitSystem.ConvertibleUnits[indexForExistingConvertibleUnit];
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
            else
            {
                // try use a new unit system

                /**
                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[] { new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor)) };
                unitSystem = new UnitSystem(name + "_system", null, (unitsystem) => null, (unitsystem) => null, (unitsystem) => convertibleunitarray, isModifiableUnitSystem: true);
                return convertibleunitarray[0];
                **/

                ConvertibleUnit[] convertibleunitarray = new ConvertibleUnit[] { new ConvertibleUnit(name, unitSymbol, primaryUnit, new ScaledValueConversion(1.0 / scaleFactor)) };
                unitSystem = new ExtentedUnitSystem(unitSystem as UnitSystem, $"{unitSystem.Name}_With_{name}_system", someExtraBaseUnitsBuilder: null, someExtraNamedDerivedUnitsBuilder: null, (unitsystem) => convertibleunitarray, isModifiableUnitSystem: true);
                return convertibleunitarray[0];
            }
        }


        public override String ToListString(String name)
        {
            String Unitname = name;

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

                        return $"unit {Unitname} = {val.ToString(cultureInfo)} {cu.PrimaryUnit}";
                    }
                }

                return $"unit {Unitname} = {pu}";
            } 
        }
    }

    // class NamedVariable : Quantity, INametableItem
    record class NamedVariable : OperandInfo, INametableItem
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

        public NamedVariable(OperandInfo somephysicalquantity, IEnvironment environment = null)
            : base(somephysicalquantity)
        {
            this.Environment = environment;
        }

        public T ValueAs<T>() where T : class
        {
                if (OperandType == typeof(T))
                {
                    return OperandValue as T;
                }
                else
                {
                    return null;
                }
        }

        public Quantity AsQuantity
        {
            get
            {
                return ValueAs<Quantity>();
            }
        }

        public virtual String ToListString(String name)
        {
            string str = null;
            if (this.OperandType == typeof(Quantity))
            {
                str = $"var {name} = {this.ToString(" UL", CultureInfo)}";
            }
            else
            if (this.OperandType == typeof(DateTime))
            {
                str = $"var {name} = {this.AsDateTime().Value.ToString(CultureInfo)}";
            }
            else
            if (this.OperandType == typeof(String))
            {
                str = $"var {name} = {this.AsString().ToString(CultureInfo)}";
            }
            else
            {
                str = $"var {name} = {this.ToString()}";
            }

            return str;
        }

        public void WriteToTextFile(String name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(name));
        }
    }

    record class NamedConstant : NamedVariable
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
        ReadCommands = 9,

        ReadComment = 10,
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

                    String itemStr = (Item.Value != null) ? Item.Value.ToListString(Item.Key)
                                                          : $"Item {Item.Key}";
                    ListStringBuilder.Append("    " + itemStr);

                    count++;
                }
            }
            catch (Exception e)
            {
                String Message = $"{e.GetType()} Exception Source: {e.Source} - {e}";
                ListStringBuilder.AppendLine();
                ListStringBuilder.Append(Message);
                ListStringBuilder.AppendLine();
            }

            return ListStringBuilder.ToString();
        }
    }

    public class CalculatorEnvironment : NametableItem, IEnvironment
    {
        public class CommentParseInfo
        {
            public CommentParseInfo outerCommentToParseInfo = null;
            public CommandParserState PrevParseState;

            public String CommentStartStr = null;
            public String CommentEndStr = null;
            public String CommentText = null;

            public void AddCommentText(String commentText)
            {
                this.CommentText += commentText;
                if (outerCommentToParseInfo != null)
                {
                    outerCommentToParseInfo.AddCommentText(commentText);
                }   
            }

            public CommentParseInfo(string commentStartStr, CommentParseInfo outerCommentToParseInf, CommandParserState prevParseState)
            {
                this.outerCommentToParseInfo = outerCommentToParseInf;
                this.PrevParseState = prevParseState;

                this.CommentStartStr = commentStartStr;
                var reversed = commentStartStr.Reverse().ToArray();
                String reversedStr = "";
                foreach (char chr in reversed)
                {
                    reversedStr += chr;
                }
                this.CommentEndStr = reversedStr;

                this.CommentText = "";
            }
        }

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
            public INametableItem RedefineItem = null; 

            public FunctionParseInfo(CalculatorEnvironment staticOuterContext, string NewFunctionName)
            {
                this.FunctionName = NewFunctionName;
                this.Function = new PhysicalQuantityCommandsFunction(staticOuterContext);
            }
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
        public CommentParseInfo CommentToParseInfo = null;
        public FunctionParseInfo FunctionToParseInfo = null;
        public CommandBlockParseInfo CommandBlockToParseInfo = null;
        public int CommandBlockLevel = 0;

        private TraceLevels _OutputTracelevel = TraceLevels.Normal;
        public TraceLevels OutputTracelevel { get { return _OutputTracelevel; } set { _OutputTracelevel = value; } }

        private FormatProviderKind FormatProviderSrc = FormatProviderKind.DefaultFormatProvider; 
        public FormatProviderKind FormatProviderSource { get { return FormatProviderSrc; } set { FormatProviderSrc = value; } }

        private Boolean _AutoDefineUnits = false;
        public Boolean AutoDefineUnits { get { return _AutoDefineUnits; } set { _AutoDefineUnits = value; } }

        #region INameTableItem interface implementation

        public override IdentifierKind Identifierkind => IdentifierKind.Environment;

        public override String ToListString(String name) => $"Namespace {name}";

        #endregion INameTableItem interface implementation

        public (Boolean commentEnded, String resultLine) BeginParsingComment(ref String commandLine) 
        {
            Boolean commentEnded = false;
            String resultLine = null;
            (commandLine, String commentStartStr) = commandLine.ReadCommentStartToken();
            if (!String.IsNullOrWhiteSpace(commentStartStr))
            {
                resultLine = "" + ConsoleAnsiColors.ForgroundDarkGreen;
                CommentToParseInfo = new CommentParseInfo(commentStartStr, CommentToParseInfo, ParseState);
                ParseState = CommandParserState.ReadComment;
                if (!String.IsNullOrEmpty(commandLine))
                {
                    (commentEnded, resultLine) = ParsingCommentContent(ref commandLine);
                }
            }
            return (commentEnded, resultLine);
        }

        public (Boolean endOfComment, String resultLine) ParsingCommentContent(ref string commandLine)
        {
            String resultLine = null;
            String commentText;
            int endIndex = commandLine.IndexOf(CommentToParseInfo.CommentEndStr);
            Boolean endOfComment = endIndex >= 0 && (endIndex == 0 || commandLine[endIndex -1] != '*' || CommentToParseInfo.CommentEndStr.Length == 2);
            if (endOfComment)
            {
                commentText = commandLine.Substring(0, endIndex);
                commandLine = commandLine.Substring(endIndex + CommentToParseInfo.CommentEndStr.Length).TrimStart();
                if (!String.IsNullOrEmpty(commentText))
                {
                    CommentToParseInfo.AddCommentText(commentText);
                }
                resultLine = "" + ConsoleAnsiColors.ForgroundColorReset;
                EndParsingComment();
            }
            else
            {
                commentText = commandLine;
                commandLine = null;

                int beginOfInnerCommentIndex = commentText.IndexOf("/*");
                Boolean beginOfInnerComment = beginOfInnerCommentIndex >= 0;
                if (beginOfInnerComment)
                {
                    commentText = commentText.Substring(0, beginOfInnerCommentIndex);
                    if (!String.IsNullOrEmpty(commentText))
                    {
                        CommentToParseInfo.AddCommentText(commentText);
                    }
                    String innerCommentText = commentText.Substring(beginOfInnerCommentIndex);

                    (Boolean commentEnded, resultLine) = BeginParsingComment(ref innerCommentText);
                }
                else
                if (!String.IsNullOrEmpty(commentText))
                {
                    CommentToParseInfo.AddCommentText(commentText);
                }
            }

            return (endOfComment, resultLine);
        }

        public void EndParsingComment()
        {
            ParseState = CommentToParseInfo.PrevParseState;
            CommentToParseInfo = CommentToParseInfo.outerCommentToParseInfo;
        }
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

        public String ListIdentifiers(Boolean forceListContext = false, Boolean listNamedItems  = false, Boolean listSettings = false)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            Boolean HasItemsToShow = listNamedItems && (NamedItems.Count > 0);
            Boolean ListContext = forceListContext | HasItemsToShow | listSettings;
            String ListStr;
            if (OuterContext != null)
            {
                ListStr = OuterContext.ListIdentifiers(ListContext, listNamedItems, listSettings);
                ListStringBuilder.Append(ListStr);
                ListStringBuilder.AppendLine();
            }
            if (ListContext)
            {
                if (OuterContext != null)
                {
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.Append($"{Name}:");
                if (listSettings)
                {
                    ListStringBuilder.AppendLine();
                    ListStringBuilder.AppendLine("  Settings:");
                    
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

                    ListStringBuilder.Append($"    Tracelevel = {ListStr}");
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
                    ListStringBuilder.Append($"    FormatProvider = {ListStr}");
                }
                if (HasItemsToShow)
                {
                    ListStringBuilder.AppendLine();

                    if (listSettings)
                    {
                        ListStringBuilder.AppendLine("  Items:");
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

        public Boolean SystemSet(String systemName, bool setAsDefault, out INametableItem systemItem)
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
            if (setAsDefault)
            {
                Global.CurrentUnitSystems.Use((systemItem as NamedSystem).UnitSystem);
            }
            return context.SetLocalIdentifier(systemName, systemItem);
        }

        public Boolean UnitSet(IUnitSystem unitSystem, String unitName, OperandInfo unitValue, String unitSymbol, BaseUnitDimension? dimension, out INametableItem unitItem, out string errorMessage)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(unitName, out IEnvironment context, out unitItem);

            if (Found && (context == this) && (unitItem.Identifierkind != IdentifierKind.Unit))
            {   // Found locally, but is not a unit; Can't set as unit
                errorMessage = unitName + " is found locally but is not a unit; Can't set as unit";
                return false;
            }

            if (context == null)
            {
                context = this;
            }

            (bool OK, String errormessage) updateRes;
            if (unitItem == null)
            {
                // Either is identifier an unit in some context; set it to specified value
                // or identifier not found; No local identifier with that name, Declare local unit
                if (unitSystem == null)
                {
                    if (unitValue != null && unitValue.AsQuantity().Unit != null)
                    {   // Is same system as values unit
                        unitSystem = unitValue.AsQuantity().Unit.ExponentsSystem;
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
                // unitItem = new NamedUnit(unitSystem, unitName, unitSymbol, unitValue?.AsQuantity()?.Unit, this);
                unitItem = new NamedUnit(unitSystem, unitName, unitSymbol, unitValue?.AsQuantity(), dimension, this);
                updateRes = (true, "");
            }
            else
            {
                NamedUnit nui = (NamedUnit)unitItem;
                updateRes = nui.UpdateUnit(unitName, unitValue.AsQuantity().Unit);
                
            }
            errorMessage = updateRes.errormessage;
            if (!updateRes.OK)
            {
                return false;
            }

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

        public Boolean VariableSet(String variableName, OperandInfo variableValue)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(variableName, out var context, out var Item);
            if (Found && Item.Identifierkind == IdentifierKind.Variable)
            {   // Identifier is a variable in some context; set it to specified value
                context.SetLocalIdentifier(variableName, new NamedVariable(variableValue ?? new OperandInfo(new Quantity()), context as CalculatorEnvironment));
            }
            else
            {
                if (Found && Item.Identifierkind != IdentifierKind.Variable && context == this)
                {   // Found locally but not as variable; Can't set as variable
                    return false;
                }
                else
                {   // Variable not found; No local function with that name, Declare local variable
                    this.NamedItems.Add(variableName, new NamedVariable(variableValue ?? new OperandInfo(new Quantity()), this));
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

        public Boolean VariableGet(String variableName, out OperandInfo variableValue, ref String resultLine)
        {
            // Find identifier 
            Boolean Found = FindIdentifier(variableName, out var context, out var Item);
            if (Found && ((Item.Identifierkind == IdentifierKind.Variable) || (Item.Identifierkind == IdentifierKind.Constant)))
            {   // Identifier is a variable or constant in some context; Get it
                variableValue = Item as OperandInfo;
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
