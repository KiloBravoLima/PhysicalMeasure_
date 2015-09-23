/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;

namespace PhysicalMeasure
{

    #region Physical Measure Constants

    /**
    public static partial class Physics
    {
        public const int NoOfBaseQuanties = 7;
    }
    
    public static partial class Economics
    {
        public const int NoOfBaseQuanties = 1;
    }
    **/

    public enum PhysicalBaseQuantityKind
    {
        Length,
        Mass,
        Time,
        ElectricCurrent,
        ThermodynamicTemperature,
        AmountOfSubstance,
        LuminousIntensity
    }

    public enum MonetaryBaseQuantityKind
    {
        Currency // Monetary unit
    }

    public enum UnitKind
    {
        BaseUnit,
        DerivedUnit,
        ConvertibleUnit,

        PrefixedUnit,
        PrefixedUnitExponent,
        CombinedUnit,
        MixedUnit
    }


    #endregion Physical Measure Constants

    #region Physical Measure Interfaces

    public interface INamed
    {
        String Name { get; }
    }

    public interface INamedSymbol : INamed
    {
        String Symbol { get; }
    }

    public interface ISystemItem
    {
        /** Returns the unit system that this item is defined as a part of. 
         *  Must be a single simple unit system (not a combined unit system). 
         *  Can be null if and only if a combined unit combines sub units from different single in-convertible unit systems.
         **/
        IUnitSystem SimpleSystem { get; }
    }

    public interface INamedSymbolSystemItem : ISystemItem, INamedSymbol
    {
    }

    public interface IUnitPrefixExponentMath
    {
        IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix);
        IUnitPrefixExponent Divide(IUnitPrefixExponent prefix);

        IUnitPrefixExponent Power(SByte exponent);
        IUnitPrefixExponent Root(SByte exponent);
    }

    public interface IUnitPrefixExponent : IUnitPrefixExponentMath
    {
        SByte Exponent { get; }
        double Value { get; }
    }

    public interface IUnitPrefix : INamed, IUnitPrefixExponent
    {
        Char PrefixChar { get; }
    }

    public interface IUnitPrefixTable
    {
        IUnitPrefix[] UnitPrefixes { get; }

        Boolean GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix);

        Boolean GetUnitPrefixFromPrefixChar(char somePrefixChar, out IUnitPrefix unitPrefix);

        // 
        Boolean GetExponentFromPrefixChar(char somePrefixChar, out IUnitPrefixExponent exponent);

        Boolean GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out char prefixChar);

        IUnitPrefix UnitPrefixFromPrefixChar(char somePrefixChar);

        IUnitPrefixExponent ExponentFromPrefixChar(char somePrefixChar);

    }

    public interface INamedSymbolUnit : INamedSymbol, IPhysicalUnit //  <BaseUnit | NamedDerivedUnit | ConvertiableUnit>
    {
        // Unprefixed unit, coherent unit symbol

    }

    public interface IPrefixedUnit : IPhysicalUnit
    {
        IUnitPrefix Prefix { get; }
        INamedSymbolUnit Unit { get; }
    }

    public interface ICombinedUnitFormat
    {
        String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false);
    }

    public interface IAsPhysicalQuantity
    {
        IPhysicalQuantity AsPhysicalQuantity();
    }

    public interface IPrefixedUnitExponent : IPrefixedUnit, ICombinedUnitFormat, IAsPhysicalQuantity
    {
        SByte Exponent { get; }

        IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent, out Double scaleFactor);
    }

    public interface IPrefixedUnitExponentList : IList<IPrefixedUnitExponent>, ICombinedUnitFormat
    {
        IPrefixedUnitExponentList Root(SByte exponent);
        IPrefixedUnitExponentList Power(SByte exponent);
    }

    public interface IUnit
    {
        UnitKind Kind { get; }

        /** Returns the same unit system as property ISystemItem.SimpleSystem if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems which are not convertible to each other (neither directly or indirectly). 
         *  Can not be null.
         **/
        IUnitSystem ExponentsSystem { get; }

        SByte[] Exponents { get; }

        String PureUnitString();

        String UnitString();

        String UnitPrintString();

        String ToPrintString();

        String ReducedUnitString();

        String ValueString(double quantity);
        String ValueString(double quantity, String format, IFormatProvider formatProvider);

        double FactorValue { get; }
        IPhysicalUnit PureUnit { get; }
    }

    public interface ISystemUnit : ISystemItem, IUnit
    {

    }


    public interface IUnitMath
    {
        IPhysicalUnit Multiply(IUnitPrefixExponent prefix);
        IPhysicalUnit Divide(IUnitPrefixExponent prefix);

        IPhysicalUnit Multiply(INamedSymbolUnit namedSymbolUnit);
        IPhysicalUnit Divide(INamedSymbolUnit namedSymbolUnit);


        IPhysicalUnit Multiply(IPrefixedUnit prefixedUnit);
        IPhysicalUnit Divide(IPrefixedUnit prefixedUnit);

        IPhysicalUnit Multiply(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalUnit Divide(IPrefixedUnitExponent prefixedUnitExponent);


        IPhysicalUnit Multiply(IPhysicalUnit physicalUnit);
        IPhysicalUnit Divide(IPhysicalUnit physicalUnit);

        IPhysicalUnit Pow(SByte exponent);
        IPhysicalUnit Rot(SByte exponent);
    }

    public interface ICombinedUnitMath
    {
        ICombinedUnit CombineMultiply(Double factor);
        ICombinedUnit CombineDivide(Double factor);

        ICombinedUnit CombineMultiply(IUnitPrefixExponent prefix);
        ICombinedUnit CombineDivide(IUnitPrefixExponent prefix);

        ICombinedUnit CombineMultiply(INamedSymbolUnit namedSymbolUnit);
        ICombinedUnit CombineDivide(INamedSymbolUnit namedSymbolUnit);


        ICombinedUnit CombineMultiply(IPrefixedUnit prefixedUnit);
        ICombinedUnit CombineDivide(IPrefixedUnit prefixedUnit);

        ICombinedUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent);
        ICombinedUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent);

        ICombinedUnit CombineMultiply(IPhysicalUnit physicalUnit);
        ICombinedUnit CombineDivide(IPhysicalUnit physicalUnit);

        ICombinedUnit CombinePow(SByte exponent);
        ICombinedUnit CombineRot(SByte exponent);
    }


    public interface IPhysicalItemMath
    {
        IPhysicalUnit Dimensionless { get; }
        Boolean IsDimensionless { get; }

        IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity Multiply(double quantity);
        IPhysicalQuantity Divide(double quantity);

        IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity);
    }

    public interface IEquivalence<T>
    {
        // Like Equal, but allow to be off by a factor: this.Equivalent(other, out quotient) means (this == other * quotient) is true.
        bool Equivalent(T other, out double quotient);

        // double Quotient(T other);   // quotient = 0 means not equivalent
    }

    public interface IPhysicalUnitMath : IEquatable<IPhysicalUnit>, IEquivalence<IPhysicalUnit>, IUnitMath, ICombinedUnitMath, IPhysicalItemMath
    {

    }

    public interface IPhysicalUnitConvertible
    {
        bool IsLinearConvertible();


        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        IPhysicalQuantity this[IPhysicalUnit convertToUnit] { get; }
        IPhysicalQuantity this[IPhysicalQuantity convertToUnit] { get; }


        IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit();   /// Unique defined simple unit system this unit is part of
        IPhysicalQuantity ConvertToBaseUnit();     /// Express the unit by base units only; No ConvertibleUnit, MixedUnit or NamedDerivatedUnit.
        IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToDerivedUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        IPhysicalQuantity ConvertTo(ref double quantity, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit(ref double quantity);

        IPhysicalQuantity ConvertToBaseUnit(double quantity);
        IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem);
        IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem);
    }

    public interface IPhysicalUnit : ISystemUnit, IPhysicalUnitMath, IPhysicalUnitConvertible, IAsPhysicalQuantity /*  : <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit | MixedUnit> */
    {
#if DEBUG
        // [Conditional("DEBUG")]
        void TestPropertiesPrint();
#endif
    }

    public interface IQuantityMath
    {
        IPhysicalQuantity Multiply(IUnitPrefixExponent prefix);
        IPhysicalQuantity Divide(IUnitPrefixExponent prefix);

        IPhysicalQuantity Multiply(INamedSymbolUnit namedSymbolUnit);
        IPhysicalQuantity Divide(INamedSymbolUnit namedSymbolUnit);


        IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit);
        IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit);

        IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent);

        IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit);
        IPhysicalQuantity Divide(IPhysicalUnit physicalUnit);

        IPhysicalQuantity Pow(SByte exponent);
        IPhysicalQuantity Rot(SByte exponent);
    }

    public interface IPhysicalQuantityMath : IComparable, IEquivalence<IPhysicalQuantity>, IEquatable<double>, IEquatable<IPhysicalUnit>, IEquatable<IPhysicalQuantity>, IQuantityMath, IPhysicalItemMath
    {
        IPhysicalQuantity Zero { get; }
        IPhysicalQuantity One { get; }

        IPhysicalQuantity Add(IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Subtract(IPhysicalQuantity physicalQuantity);
    }

    public interface IPhysicalQuantityConvertible
    {
        // Auto detecting if specific or relative unit conversion 
        IPhysicalQuantity this[IPhysicalUnit convertToUnit] { get; }
        IPhysicalQuantity this[IPhysicalQuantity convertToUnit] { get; }

        IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit();
        IPhysicalQuantity ConvertToBaseUnit();

        IPhysicalQuantity ConvertToDerivedUnit();

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        IPhysicalQuantity RelativeConvertTo(IPhysicalUnit convertToUnit);
        IPhysicalQuantity RelativeConvertTo(IUnitSystem convertToUnitSystem);

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        IPhysicalQuantity SpecificConvertTo(IPhysicalUnit convertToUnit);
        IPhysicalQuantity SpecificConvertTo(IUnitSystem convertToUnitSystem);
    }

    public interface IBaseUnit : IPhysicalUnit, INamedSymbolUnit
    {
        SByte BaseUnitNumber { get; }
    }

    public interface IDerivedUnit : IPhysicalUnit
    {
    }

    public interface INamedDerivedUnit : INamedSymbolUnit, IDerivedUnit
    {
    }

    public interface IValueConversion
    {
        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        double Convert(bool backwards = false);
        double ConvertToPrimaryUnit();
        double ConvertFromPrimaryUnit();

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        double Convert(double value, bool backwards = false);
        double ConvertToPrimaryUnit(double value);
        double ConvertFromPrimaryUnit(double value);

        double LinearOffset { get; }
        double LinearScale { get; }
    }

    public interface IConvertibleUnit : INamedSymbolUnit
    {
        IPhysicalUnit PrimaryUnit { get; }
        IValueConversion Conversion { get; }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        IPhysicalQuantity ConvertToPrimaryUnit();
        IPhysicalQuantity ConvertFromPrimaryUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        IPhysicalQuantity ConvertToPrimaryUnit(double quantity);
        IPhysicalQuantity ConvertFromPrimaryUnit(double quantity);
    }

    public interface ICombinedUnit : IPhysicalUnit
    {
        IPrefixedUnitExponentList Numerators { get; }
        IPrefixedUnitExponentList Denominators { get; }


        /** Returns the same unit system as property SimpleSystem if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems which are not convertible to each other (neither directly or indirectly). 
         *  Can not be null.
        IUnitSystem ExponentsSystem { get; }
         **/


        /** Returns the same unit system as property System if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems. 
         *  Can not be null.
         **/
        IUnitSystem SomeSimpleSystem { get; }

        ICombinedUnit OnlySingleSystemUnits(IUnitSystem us);

        // Specific conversion
        IPhysicalQuantity ConvertFrom(IPhysicalQuantity physicalQuantity);
    }

    public interface IMixedUnit : IPhysicalUnit
    {
        IPhysicalUnit MainUnit { get; }
        IPhysicalUnit FractionalUnit { get; }
        String Separator { get; }
    }

    public interface IPhysicalQuantity : IFormattable, IPhysicalQuantityMath, IPhysicalQuantityConvertible
    {
        double Value { get; }
        IPhysicalUnit Unit { get; }

        String ToPrintString();
    }

    public interface IUnitSystem : INamed
    {
        bool IsIsolatedUnitSystem { get; }
        bool IsCombinedUnitSystem { get; }

        IUnitPrefixTable UnitPrefixes { get; }
        IBaseUnit[] BaseUnits { get; }
        INamedDerivedUnit[] NamedDerivedUnits { get; }
        IConvertibleUnit[] ConvertibleUnits { get; }

        IPhysicalUnit Dimensionless { get; }
        INamedSymbolUnit UnitFromName(String unitName);
        INamedSymbolUnit UnitFromSymbol(String unitSymbol);

        //IPrefixedUnit ScaledUnitFromSymbol(String scaledUnitSymbol);
        IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol);

        INamedSymbolUnit NamedDerivedUnitFromUnit(IPhysicalUnit derivedUnit);
        IPhysicalUnit UnitFromExponents(SByte[] exponents);
        IPhysicalUnit UnitFromUnitInfo(SByte[] exponents, SByte NoOfNonZeroExponents, SByte NoOfNonOneExponents, SByte FirstNonZeroExponent);

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IUnitSystem convertToUnitSystem);

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem);

        IPhysicalQuantity SpecificConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit);
    }

    public interface ICombinedUnitSystem : IUnitSystem, IEquatable<ICombinedUnitSystem>
    {
        SByte[] UnitExponents(ICombinedUnit cu);
        IPhysicalQuantity ConvertToBaseUnit(ICombinedUnit cu);

        IUnitSystem[] UnitSystemes { get; }
        Boolean ContainsSubUnitSystem(IUnitSystem unitsystem);
        Boolean ContainsSubUnitSystems(IEnumerable<IUnitSystem> unitsystems);
    }

    #endregion Physical Measure Interfaces

}

