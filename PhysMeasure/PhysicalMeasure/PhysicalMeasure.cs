/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using Extensions;

namespace PhysicalMeasure
{

    #region Physical Measure Constants

    //public enum PhysicsMeasureKind
    public enum PhysicsBaseQuantityKind
    {
        Length,
        Mass,
        Time,
        ElectricCurrent,
        ThermodynamicTemperature,
        AmountOfSubstance,
        LuminousIntensity
    }

    //public enum ValutasMeasureKind
    public enum ValutasBaseQuantityKind
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

    public static partial class Physics
    {
        // public const int NoOfMeasures = 7;
        public const int NoOfBaseQuanties = 7;
    }

    public static partial class Valutas
    {
        // public const int NoOfMeasures = 1;
        public const int NoOfBaseQuanties = 1;
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


        // IPrefixedUnit Multiply(INamedSymbolUnit physicalUnit);

        // IPrefixedUnit Divide(INamedSymbolUnit physicalUnit);

        /*
        IPhysicalUnit Multiply(IPhysicalUnit physicalUnit);

        IPhysicalUnit Divide(IPhysicalUnit physicalUnit);
        */
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

        bool GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix);

        bool GetUnitPrefixFromPrefixChar(char somePrefixChar, out IUnitPrefix unitPrefix);

        // 
        bool GetExponentFromPrefixChar(char somePrefixChar, out IUnitPrefixExponent exponent);

        bool GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out char prefixChar);

        IUnitPrefix UnitPrefixFromPrefixChar(char somePrefixChar);

        IUnitPrefixExponent ExponentFromPrefixChar(char somePrefixChar);

    }


    //public interface INamedSymbolUnit : INamedSymbol, IPhysicalUnit // maybe : INamedSymbolSystemItem 
    //public interface INamedSymbolUnit : INamedSymbol, IUnderUnit // maybe : INamedSymbolSystemItem,   IUnderUnit  <BaseUnit | NamedDerivedUnit>
    //public interface INamedSymbolUnit : INamedSymbol, IUnit // maybe : INamedSymbolSystemItem,   IUnit  <BaseUnit | NamedDerivedUnit>
    public interface INamedSymbolUnit : INamedSymbol, IPhysicalUnit //  <BaseUnit | NamedDerivedUnit | ConvertiableUnit>
    {
        // Un-prefixed unit, coherent unit symbol

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

        // IPhysicalQuantity AsPhysicalQuantity(IUnitSystem convertToUnitSystem);
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

        /***
        IPhysicalQuantity AsPhysicalQuantity();

        IPhysicalQuantity AsPhysicalQuantity(double quantity);

        IPhysicalQuantity AsPhysicalQuantity(IUnitSystem unitSystem);
        // Obsolete IPhysicalQuantity PhysicalQuantity(ref double quantity, IUnitSystem unitSystem);
        ***/

        /** Returns the same unit system as property ISystemItem.SimpleSystem if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems which are not convertible to each other (neither directly or indirectly). 
         *  Can not be null.
         **/
        IUnitSystem ExponentsSystem { get; }

        SByte[] Exponents { get; }

        //Byte[] UnsignedExponents { get; }

        String PureUnitString();

        String UnitString();

        String UnitPrintString();

        String ToPrintString();

        String ReducedUnitString();

        /*** String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false); ***/

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


        /*
        IPhysicalUnit operator *(IUnitPrefixExponent prefix);
        IPhysicalUnit Divide(IUnitPrefixExponent prefix);

        IPhysicalUnit operator *(INamedSymbolUnit namedSymbolUnit);
        IPhysicalUnit Divide(INamedSymbolUnit namedSymbolUnit);


        IPhysicalUnit operator *(IPrefixedUnit prefixedUnit);
        IPhysicalUnit Divide(IPrefixedUnit prefixedUnit);

        IPhysicalUnit operator *(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalUnit Divide(IPrefixedUnitExponent prefixedUnitExponent);


        IPhysicalUnit operator *(IPhysicalUnit physicalUnit);
        IPhysicalUnit Divide(IPhysicalUnit physicalUnit);

        IPhysicalUnit operator ^(SByte exponent);   // operator Pow
        //IPhysicalUnit operator %(SByte exponent); // operator Rot
        IPhysicalUnit operator |(SByte exponent);   // operator Rot
        */
    }

    /**
    public interface ICombineUnitMath
    {
        //IPhysicalUnit CombinePrefix(SByte prefixExponent);

        IPhysicalUnit CombineMultiply(IUnitPrefixExponent prefix);
        IPhysicalUnit CombineDivide(IUnitPrefixExponent prefix);

        IPhysicalUnit CombineMultiply(INamedSymbolUnit namedSymbolUnit);
        IPhysicalUnit CombineDivide(INamedSymbolUnit namedSymbolUnit);

        
        IPhysicalUnit CombineMultiply(IPrefixedUnit prefixedUnit);
        IPhysicalUnit CombineDivide(IPrefixedUnit prefixedUnit);

        IPhysicalUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent);

        IPhysicalUnit CombineMultiply(IPhysicalUnit physicalUnit);
        IPhysicalUnit CombineDivide(IPhysicalUnit physicalUnit);

        IPhysicalUnit CombinePow(SByte exponent);
        IPhysicalUnit CombineRot(SByte exponent);
    }
    **/

    public interface ICombinedUnitMath
    {
        //IPhysicalUnit CombinePrefix(SByte prefixExponent);

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

        //ICombinedUnit CombinePrefix(SByte prefixExponent);
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

        /*
        IPhysicalQuantity operator *(IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity operator /(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity operator *(double quantity);
        IPhysicalQuantity operator /(double quantity);

        IPhysicalQuantity operator *(double quantity, IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity operator /(double quantity, IPhysicalQuantity physicalQuantity);
        */
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

        // IPhysicalQuantity ConvertToDerivedUnit(IUnitSystem convertToUnitSystem);


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        IPhysicalQuantity ConvertTo(ref double quantity, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit(ref double quantity);

        IPhysicalQuantity ConvertToBaseUnit(double quantity);
        IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem);
        IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem);
    }

    public interface IPhysicalUnit : ISystemUnit, IPhysicalUnitMath, IPhysicalUnitConvertible, IAsPhysicalQuantity/*  : <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit | MixedUnit>  */
    {
#if DEBUG
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

        //// IPhysicalQuantity ConvertToPrimaryUnit(IPhysicalQuantity physicalQuantity);
        //// IPhysicalQuantity ConvertFromPrimaryUnit(IPhysicalQuantity physicalQuantity);

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

    #region Physical Measure Classes

    #region Physical Measure Exceptions

    [Serializable]
    public class PhysicalUnitFormatException : FormatException
    {
        public PhysicalUnitFormatException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PhysicalUnitFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PhysicalUnitFormatException()
            : this("The string argument is not in a valid physical unit format.")
        {
        }

        public PhysicalUnitFormatException(String message)
            : base(message)
        {
        }
    }

    [Serializable]
    public class PhysicalUnitMathException : Exception
    {
        public PhysicalUnitMathException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PhysicalUnitMathException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PhysicalUnitMathException()
            : this("The result of the math operation on the PhysicalUnit argument can't be represented by this implementation of PhysicalMeasure.")
        {
        }

        public PhysicalUnitMathException(String message)
            : base(message)
        {
        }
    }

    #endregion Physical Measure Exceptions

    #region Dimension Exponents Classes

    public class DimensionExponents : IEquatable<DimensionExponents>
    {

        private SByte[] exponents;

        public DimensionExponents(SByte[] exponents)
        {
            this.exponents = exponents;
        }

        public override int GetHashCode()
        {
            if (exponents == null)
            {
                return base.GetHashCode();
            }
            return exponents.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;

            DimensionExponents DimensionExponentsObj = obj as DimensionExponents;
            if (DimensionExponentsObj == null)
                return false;
            else
                return Equals(DimensionExponentsObj);
        }

        public Boolean Equals(DimensionExponents other)
        {
            if (other == null)
                return false;

            return Equals(this.exponents, other.exponents);
        }

    }

    public static class DimensionExponentsExtension
    {
        // SByte[Physic.NoOfBaseQuanties] exponents;
        public static Boolean DimensionEquals(this SByte[] exponents1, SByte[] exponents2)
        {
            Debug.Assert(exponents1 != null, "Parameter must be specified");
            Debug.Assert(exponents2 != null, "Parameter must be specified");

            if (exponents1 == exponents2)
            {
                return true;
            }

            if (exponents1.Equals(exponents2))
            {
                return true;
            }

            Boolean equal = true;
            SByte i = 0;
            SByte MinNoOfBaseUnits = (SByte)Math.Min(exponents1.Length, exponents2.Length);
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(exponents1.Length, exponents2.Length);

            Debug.Assert(MaxNoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + MaxNoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            do
            {   // Compare exponents where defined in both arrays
                equal = exponents1[i] == exponents2[i];
                i++;
            }
            while (i < MinNoOfBaseUnits && equal);

            if (equal && i < MaxNoOfBaseUnits)
            {   // Check tail of longest array to contain only zeros
                do
                {
                    if (exponents1.Length > exponents2.Length)
                    {
                        equal = exponents1[i] == 0;
                    }
                    else
                    {
                        equal = exponents2[i] == 0;
                    }
                    i++;
                }
                while (i < MaxNoOfBaseUnits && equal);
            }
            return equal;
        }

        public static Boolean IsDimensionless(this SByte[] exponents)
        {
            Debug.Assert(exponents != null, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            Boolean isDimensionless = true;
            SByte i = 0;
            do
            {
                isDimensionless = exponents[i] == 0;
                i++;
            }
            while (i < NoOfBaseUnits && isDimensionless);

            return isDimensionless;
        }

        public static SByte NoOfDimensions(this SByte[] exponents)
        {
            Debug.Assert(exponents != null, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte noOfDimensions = 0;
            SByte i = 0;
            do
            {
                if (exponents[i] != 0)
                {
                    noOfDimensions++;
                }
                i++;
            }
            while (i < NoOfBaseUnits);

            return noOfDimensions;
        }


        public static SByte[] Multiply(this SByte[] exponents1, SByte[] exponents2)
        {
            Debug.Assert(exponents1 != null, "Parameter exponents1 needed");
            Debug.Assert(exponents2 != null, "Parameter exponents2 needed");


            SByte NoOfBaseUnits1 = (SByte)exponents1.Length;
            SByte NoOfBaseUnits2 = (SByte)exponents2.Length;
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(NoOfBaseUnits1, NoOfBaseUnits2);
            SByte MinNoOfBaseUnits = (SByte)Math.Min(NoOfBaseUnits1, NoOfBaseUnits2);

            Debug.Assert(NoOfBaseUnits1 <= Physics.NoOfBaseQuanties + 1, "exponents1 has too many base units:" + NoOfBaseUnits1.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");
            Debug.Assert(NoOfBaseUnits2 <= Physics.NoOfBaseQuanties + 1, "exponents2 has too many base units:" + NoOfBaseUnits2.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[MaxNoOfBaseUnits];
            SByte i = 0;

            do
            {
                NewExponents[i] = (SByte)(exponents1[i] + exponents2[i]);
                i++;
            }
            while (i < MinNoOfBaseUnits);

            while (i < MaxNoOfBaseUnits)
            {
                if (NoOfBaseUnits1 > NoOfBaseUnits2)
                {
                    NewExponents[i] = exponents1[i];
                }
                else
                {
                    NewExponents[i] = exponents2[i];
                }

                i++;
            }

            return NewExponents;
        }

        public static SByte[] Divide(this SByte[] exponents1, SByte[] exponents2)
        {
            Debug.Assert(exponents1 != null, "Parameter exponents1 needed");
            Debug.Assert(exponents2 != null, "Parameter exponents2 needed");


            SByte NoOfBaseUnits1 = (SByte)exponents1.Length;
            SByte NoOfBaseUnits2 = (SByte)exponents2.Length;
            SByte MaxNoOfBaseUnits = Math.Max(NoOfBaseUnits1, NoOfBaseUnits2);
            SByte MinNoOfBaseUnits = Math.Min(NoOfBaseUnits1, NoOfBaseUnits2);

            Debug.Assert(NoOfBaseUnits1 <= Physics.NoOfBaseQuanties + 1, "exponents1 has too many base units:" + NoOfBaseUnits1.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");
            Debug.Assert(NoOfBaseUnits2 <= Physics.NoOfBaseQuanties + 1, "exponents2 has too many base units:" + NoOfBaseUnits2.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[MaxNoOfBaseUnits];
            SByte i = 0;

            do
            {
                NewExponents[i] = (SByte)(exponents1[i] - exponents2[i]);
                i++;
            }
            while (i < MinNoOfBaseUnits);

            while (i < MaxNoOfBaseUnits)
            {
                if (NoOfBaseUnits1 > NoOfBaseUnits2)
                {
                    NewExponents[i] = exponents1[i];
                }
                else
                {
                    NewExponents[i] = (SByte)(-exponents2[i]);
                }

                i++;
            }

            return NewExponents;
        }


        public static SByte[] Power(this SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null, "Parameter needed");
            Debug.Assert(exponent != 0, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            do
            {
                NewExponents[i] = (SByte)(exponents[i] * exponent);

                i++;
            }
            while (i < NoOfBaseUnits);

            return NewExponents;
        }

        public static SByte[] Root(this SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null, "Parameter needed");
            Debug.Assert(exponent != 0, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            bool OK = true;
            do
            {
                int Remainder;
                int NewExponent = Math.DivRem(exponents[i], exponent, out Remainder);
                OK = Remainder == 0;
                NewExponents[i] = (SByte)NewExponent;

                i++;
            }
            while (i < NoOfBaseUnits && OK);

            if (!OK)
            {
                Debug.Assert(OK, "Verify to not happening");

                //if (ThrowExceptionOnUnitMathError) {
                throw new PhysicalUnitMathException("The result of the math operation on the PhysicalUnit argument can't be represented by this implementation of PhysicalMeasure: (" + exponents.ToString() + ").Root(" + exponent.ToString() + ")");
                //}
                //NewExponents = null;
            }
            return NewExponents;
        }

        /*
        public static Byte[] UnsignedExponents(this SByte[] exponents)
        {
            int length = exponents.Length;
            Byte[] resExponents = new Byte[length];
            foreach (Byte i in Enumerable.Range(0, length))
            {
                i
                resExponents[i] = (Byte)exponents[i];
            }
            Debug.Assert(resExponents[length - 1] == (Byte)exponents[length - 1], "Just last element");

            return resExponents;
        }

        public static SByte[] Exponents(this Byte[] unsignedExponents)
        {
            int length = unsignedExponents.Length;
            SByte[] resExponents = new SByte[length];
            foreach (int i in Enumerable.Range(0, length))
            {
                resExponents[i] = (SByte)unsignedExponents[i];
            }
            return resExponents;
        }
        **/

        public static SByte[] AllExponents(this SByte[] Exponents, SByte length)
        {
            SByte[] resExponents;
            if (Exponents.Length < length)
            {
                resExponents = new SByte[length];
                foreach (int i in Enumerable.Range(0, length))
                {
                    resExponents[i] = Exponents[i];
                }
            }
            else
            {
                Debug.Assert(Exponents.Length == length);
                resExponents = Exponents;
            }

            return resExponents;
        }

        public static String ArrayToString(this SByte[] exponents)
        {
            string str = "[";

            foreach (int i in Enumerable.Range(0, exponents.Length))
            {
                if (i > 0)
                {
                    str = str + ", ";
                }
                str = str + exponents[i].ToString();
            }
            str = str + "]";

            return str;
        }
    }
    #endregion Dimension Exponents Classes

    public class NamedObject : INamed
    {
        private String _name;
        public String Name { get { return _name; } set { _name = value; } }

        public NamedObject(String someName)
        {
            this.Name = someName;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    #region Physical Unit prefix Classes

    public class UnitPrefixExponent : IUnitPrefixExponent
    {
        private SByte _exponent;

        public SByte Exponent { get { return _exponent; } }
        public Double Value { get { return Math.Pow(10, _exponent); } }

        public UnitPrefixExponent(SByte somePrefixExponent)
        {
            this._exponent = somePrefixExponent;
        }

        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix)
        {
            return new UnitPrefixExponent((sbyte)(this._exponent + prefix.Exponent));
        }

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix)
        {
            return new UnitPrefixExponent((sbyte)(this._exponent - prefix.Exponent));
        }

        public IUnitPrefixExponent Power(SByte exponent)
        {
            return new UnitPrefixExponent((sbyte)(this._exponent * exponent));
        }
        public IUnitPrefixExponent Root(SByte exponent)
        {
            sbyte result_exponent = (sbyte)(this._exponent / exponent);
            Debug.Assert(result_exponent * exponent == this._exponent, " Root result exponent must be an integer");
            return new UnitPrefixExponent(result_exponent);
        }


        /** * /
        public IPrefixedUnit Multiply(INamedSymbolUnit symbolUnit)
        {
            return new PrefixedUnit(this, symbolUnit);
        }
        / * **/

        #endregion IUnitPrefixExponentMath implementation

        public override string ToString()
        {
            return Exponent.ToString();
        }
    }

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private IUnitPrefixTable _unitPrefixTable;
        private char _prefixChar;
        IUnitPrefixExponent _prefixExponent;

        #region IUnitPrefix implementation

        public char PrefixChar { get { return _prefixChar; } }

        public SByte Exponent { get { return _prefixExponent.Exponent; } }
        public Double Value { get { return _prefixExponent.Value; } }

        #endregion IUnitPrefix implementation

        public UnitPrefix(IUnitPrefixTable unitPrefixTable, String someName, char somePrefixChar, IUnitPrefixExponent somePrefixExponent)
            : base(someName)
        {
            this._unitPrefixTable = unitPrefixTable;
            this._prefixChar = somePrefixChar;
            this._prefixExponent = somePrefixExponent;
        }

        public UnitPrefix(IUnitPrefixTable unitPrefixTable, String someName, char somePrefixChar, SByte somePrefixExponent)
            : this(unitPrefixTable, someName, somePrefixChar, new UnitPrefixExponent(somePrefixExponent))
        {
        }

        public IUnitPrefix Multiply(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Multiply(prefix);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }

        public IUnitPrefix Divide(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Divide(prefix);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }


        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Multiply(prefix);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Divide(prefix);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Power(SByte exponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Power(exponent);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }
        public IUnitPrefixExponent Root(SByte exponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this._prefixExponent.Root(exponent);
            if (!_unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IPrefixedUnit Multiply(INamedSymbolUnit symbolUnit)
        {
            return new PrefixedUnit(this, symbolUnit);
        }


        #endregion IUnitPrefixExponentMath implementation

        public override string ToString()
        {
            return PrefixChar.ToString();
        }
    }

    public class UnitPrefixTable : IUnitPrefixTable
    {
        private readonly IUnitPrefix[] _unitPrefixes;

        public IUnitPrefix[] UnitPrefixes { get { return _unitPrefixes; } }

        public UnitPrefixTable(IUnitPrefix[] anUnitPrefixes)
        {
            this._unitPrefixes = anUnitPrefixes;
        }

        public bool GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix)
        {
            Debug.Assert(someExponent.Exponent != 0);

            /*
            unitPrefix = null;
            foreach (UnitPrefix us in UnitPrefixes)
            {
                //if (us.PrefixExponent == someExponent.PrefixExponent)
                if (us.Exponent == someExponent.Exponent)
                {
                    unitPrefix = us;
                    return true;
                }
            }
            return false;
            */
            IUnitPrefix TempUnitPrefix;
            SByte ScaleFactorExponent;

            GetFloorUnitPrefixAndScaleFactorFromExponent(someExponent.Exponent, out TempUnitPrefix, out ScaleFactorExponent);

            if (ScaleFactorExponent == 0)
            {
                unitPrefix = TempUnitPrefix;
                return true;
            }
            else
            {
                unitPrefix = null;
                return false;
            }
        }

        //public void GetFloorUnitPrefixAndScaleFactorFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix, out SByte ScaleFactorExponent)
        public void GetFloorUnitPrefixAndScaleFactorFromExponent(SByte someExponent, out IUnitPrefix unitPrefix, out SByte ScaleFactorExponent)
        {
            Debug.Assert(someExponent != 0);

            int UnitPrefix = 11; // 10^1
            while (UnitPrefix - 1 >= 0 && UnitPrefixes[UnitPrefix - 1].Exponent <= someExponent)
            {
                UnitPrefix--;
            }
            while (UnitPrefix + 1 < UnitPrefixes.Length && UnitPrefixes[UnitPrefix + 1].Exponent >= someExponent)
            {
                UnitPrefix++;
            }
            unitPrefix = UnitPrefixes[UnitPrefix];
            ScaleFactorExponent = (SByte)(someExponent - unitPrefix.Exponent);
        }

        public bool GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out char prefixChar)
        {
            prefixChar = '\0';
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.Exponent == someExponent.Exponent)
                {
                    prefixChar = us.PrefixChar;
                    return true;
                }
            }
            return false;
        }


        public bool GetUnitPrefixFromPrefixChar(char somePrefixChar, out IUnitPrefix unitPrefix)
        {
            switch (somePrefixChar)
            {
                case '\x03BC':

                    // 'μ' // '\0x03BC' (char)956  
                    // 'µ' // '\0x00B5' (char)181
                    somePrefixChar = 'µ'; // 'µ' MICRO SIGN  '\0x00B5' (char)181
                    break;
                case 'k':
                    somePrefixChar = 'K'; // Kilo
                    break;
                case 'h':
                    somePrefixChar = 'H'; // Hecto
                    break;
            }

            foreach (UnitPrefix up in UnitPrefixes)
            {
                if (up.PrefixChar == somePrefixChar)
                {
                    //exponent = up.PrefixExponent;
                    unitPrefix = up;
                    return true;
                }
            }
            unitPrefix = null;
            return false;
        }

        public bool GetExponentFromPrefixChar(char somePrefixChar, out IUnitPrefixExponent exponent)
        {
            switch (somePrefixChar)
            {
                case '\x03BC':

                    // 'μ' // '\0x03BC' (char)956  
                    // 'µ' // '\0x00B5' (char)181
                    somePrefixChar = 'µ'; // 'µ' MICRO SIGN  '\0x00B5' (char)181
                    break;
                case 'k':
                    somePrefixChar = 'K'; // Kilo
                    break;
                case 'h':
                    somePrefixChar = 'H'; // Hecto
                    break;
            }

            foreach (UnitPrefix up in UnitPrefixes)
            {
                if (up.PrefixChar == somePrefixChar)
                {
                    //exponent = up.PrefixExponent;
                    exponent = up;
                    return true;
                }
            }
            exponent = null;
            return false;
        }


        public IUnitPrefix UnitPrefixFromPrefixChar(char somePrefixChar)
        {
            IUnitPrefix unitPrefix = null;
            GetUnitPrefixFromPrefixChar(somePrefixChar, out unitPrefix);
            return unitPrefix;
        }

        public IUnitPrefixExponent ExponentFromPrefixChar(char somePrefixChar)
        {
            IUnitPrefixExponent exponent = null;
            GetExponentFromPrefixChar(somePrefixChar, out exponent);
            return exponent;
        }
    }

    #endregion Physical Unit prefix Classes

    #region Value Conversion Classes

    public abstract class ValueConversion : IValueConversion
    {
        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public double Convert(double value, bool backwards = false)
        {
            if (backwards)
            {
                return ConvertToPrimaryUnit(value);
            }
            else
            {
                return ConvertFromPrimaryUnit(value);
            }
        }

        public abstract double ConvertFromPrimaryUnit(double value);
        public abstract double ConvertToPrimaryUnit(double value);

        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public double Convert(bool backwards = false)
        {
            if (backwards)
            {
                return ConvertToPrimaryUnit();
            }
            else
            {
                return ConvertFromPrimaryUnit();
            }
        }

        public abstract double ConvertFromPrimaryUnit();
        public abstract double ConvertToPrimaryUnit();

        public abstract double LinearOffset { get; }
        public abstract double LinearScale { get; }
    }

    public class LinearValueConversion : ValueConversion
    {
        private double _offset;
        private double _scale;

        public double Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public override double LinearOffset
        {
            get { return _offset; }
        }

        public override double LinearScale
        {
            get { return _scale; }
        }

        public LinearValueConversion(double someOffset, double someScale)
        {
            this.Offset = someOffset;
            this.Scale = someScale;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public override double ConvertFromPrimaryUnit(double value)
        {
            return (value * this.Scale) + this.Offset;
        }

        public override double ConvertToPrimaryUnit(double value)
        {
            double convertedValue = (value - this.Offset) / this.Scale;
            return convertedValue;
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public override double ConvertFromPrimaryUnit()
        {
            //return (value * this.Scale) + this.Offset;
            return 1.0d * this.Scale;
        }

        public override double ConvertToPrimaryUnit()
        {
            //double convertedValue =  (value - this.Offset) / this.Scale;
            double convertedValue = 1.0d / this.Scale;
            return convertedValue;
        }
    }

    public class ScaledValueConversion : LinearValueConversion
    {
        public ScaledValueConversion(double someScale)
            : base(0, someScale)
        {
            Debug.Assert(someScale != 0, "Parameter needed");
            Debug.Assert(!double.IsInfinity(someScale), "Finite scale value needed");

            if (someScale == 0)
            {
                throw new ArgumentException("0 is not a valid scale", "someScale");
            }
            if (double.IsInfinity(someScale))
            {
                throw new ArgumentException("Infinity is not a valid scale", "someScale");
            }
        }
    }

    public class IdentityValueConversion : ScaledValueConversion
    {
        public IdentityValueConversion()
            : base(1)
        {
        }
    }

    public class CombinedValueConversion : ValueConversion
    {
        private IValueConversion _FirstValueConversion;
        private IValueConversion _SecondValueConversion;

        private Boolean _FirstValueConversionDirectionInverted;
        private Boolean _SecondValueConversionDirectionInverted;

        public override double LinearOffset
        {
            get { return _FirstValueConversion.LinearOffset + _SecondValueConversion.LinearOffset; }
        }

        public override double LinearScale
        {
            get { return _FirstValueConversion.LinearScale * _SecondValueConversion.LinearScale; }
        }


        public CombinedValueConversion(IValueConversion firstValueConversion, Boolean firstValueConversionDirectionInverted, IValueConversion secondValueConversion, Boolean secondValueConversionDirectionInverted)
        {
            this._FirstValueConversion = firstValueConversion;
            this._FirstValueConversionDirectionInverted = firstValueConversionDirectionInverted;
            this._SecondValueConversion = secondValueConversion;
            this._SecondValueConversionDirectionInverted = secondValueConversionDirectionInverted;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.

        public override double ConvertFromPrimaryUnit(double value)
        {
            return this._SecondValueConversion.Convert(this._FirstValueConversion.Convert(value, this._FirstValueConversionDirectionInverted), this._SecondValueConversionDirectionInverted);
        }

        public override double ConvertToPrimaryUnit(double value)
        {
            return this._FirstValueConversion.Convert(this._SecondValueConversion.Convert(value, !this._SecondValueConversionDirectionInverted), !this._FirstValueConversionDirectionInverted);
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.

        public override double ConvertFromPrimaryUnit()
        {
            return this._SecondValueConversion.Convert(this._FirstValueConversion.Convert(this._FirstValueConversionDirectionInverted), this._SecondValueConversionDirectionInverted);
        }

        public override double ConvertToPrimaryUnit()
        {
            return this._FirstValueConversion.Convert(this._SecondValueConversion.Convert(!this._SecondValueConversionDirectionInverted), !this._FirstValueConversionDirectionInverted);
        }
    }

    #endregion Value Conversion Classes

    #region Physical Unit Classes

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        private String _symbol;
        public String Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbol(String someName, String someSymbol)
            : base(someName)
        {
            this.Symbol = someSymbol;
        }
    }

    public abstract class PhysicalUnit : ISystemItem, IPhysicalUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit> */
    {
        protected PhysicalUnit()
        {
        }

        public static IPhysicalUnit MakePhysicalUnit(SByte[] exponents, double ConversionFactor = 1, double ConversionOffset = 0)
        {
            return MakePhysicalUnit(Physics.SI_Units, exponents, ConversionFactor, ConversionOffset);
        }

        public static IPhysicalUnit MakePhysicalUnit(IUnitSystem system, SByte[] exponents, double ConversionFactor = 1, double ConversionOffset = 0)
        {
            IPhysicalUnit res_unit = null;
            // int nod = DimensionExponents.NoOfDimensions(exponents);
            int nod = exponents.NoOfDimensions();
            if (nod == 0)
            {
                res_unit = Physics.dimensionless;
            }
            else
            {
                res_unit = system.UnitFromExponents(exponents);
            }
            if (ConversionFactor != 1 || ConversionOffset != 0)
            {
                if (ConversionOffset == 0)
                {
                    res_unit = new ConvertibleUnit(null, res_unit, new ScaledValueConversion(ConversionFactor));
                }
                else
                {
                    res_unit = new ConvertibleUnit(null, res_unit, new LinearValueConversion(ConversionOffset, ConversionFactor));
                }
            }

            Debug.Assert(res_unit != null, "res_unit must be found");
            return res_unit;
        }

        public void TestPropertiesPrint()
        {
            bool test = true;
            if (test)
            {
                string KindStr = this.Kind.ToString();
                string SimpleSystemStr = SimpleSystem?.ToString();
                string ExponentsSystemStr = ExponentsSystem?.ToString();
                string ExponentsStr = Exponents?.ArrayToString();

                string DimensionlessStr = Dimensionless?.ToPrintString();
                string IsDimensionlessStr = IsDimensionless.ToString();

                string ThisPrintStr = this.ToPrintString();
                string ThisStr = this.ToString();
            }
        }

        public abstract IUnitSystem SimpleSystem { get; set; }
        public abstract IUnitSystem ExponentsSystem { get; }

        public abstract UnitKind Kind { get; }
        public abstract SByte[] Exponents { get; }
        /**
        public Byte[] UnsignedExponents
        {
            get
            {
                return DimensionExponents.UnsignedExponents(Exponents);
            }
        }
        **/
        public virtual IPhysicalUnit Dimensionless { get { return Physics.dimensionless; } }
        public virtual Boolean IsDimensionless
        {
            get
            {
                SByte[] exponents = Exponents;
                if (Exponents == null)
                {
                    Debug.Assert(Exponents != null, "Exponents must be found"); // 2014-09-10 Check if this actually happens
                    return false; // Maybe combined unit with Assume 
                }
                return Exponents.IsDimensionless();
            }
        }

        public override int GetHashCode()
        {
            return this.ExponentsSystem.GetHashCode() + this.Exponents.GetHashCode();
        }

        #region Unit Expression parser methods
        /**
            U = U "*" F | U F | U "/" F | F .
            F = SUX | "(" U ")" .
            SUX = U | S U | U X | S U X .
         
            U = F Uopt .
            //Uopt = "*" F Uopt | "/" F Uopt | UX | e .
            Uopt = "*" F Uopt | "/" F Uopt| U | e .
            F = SUX | "(" U ")" .
            SUX = SU Xopt .
            SU = Sopt u .
            Sopt = s | e . 
            Xopt = x | e .
          
         *  s : scale prefix char
         *  u : unit symbol
         *  x : exponent number
         **/

        #region IPhysicalUnit unit expression parser methods

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// [prefix] [unitSymbol] 
        /// </summary>
        public static IPhysicalUnit Parse(String unitString)
        {
            IPhysicalUnit pu = null;
            String resultLine = null;
            pu = ParseUnit(ref unitString, ref resultLine, ThrowExceptionOnInvalidInput: true);
            return pu;
        }

        public static IPhysicalUnit ParseUnit(ref String unitString, ref String resultLine, Boolean ThrowExceptionOnInvalidInput = true)
        {
            IPhysicalUnit pu = null;

            Char timeSeparator = ':';
            Char[] separators = { timeSeparator };

            Char FractionUnitSeparator = '\0';
            String FractionUnitSeparatorStr = null;

            int UnitStrCount = 0;
            int UnitStrStartCharIndex = 0;
            int NextUnitStrStartCharIndex = 0;
            Boolean ValidFractionalUnit = true;
            int LastUnitFieldRemainingLen = 0;

            Stack<Tuple<string, IPhysicalUnit>> FractionalUnits = new Stack<Tuple<string, IPhysicalUnit>>();

            while (ValidFractionalUnit && (UnitStrStartCharIndex >= 0) && (UnitStrStartCharIndex < unitString.Length))
            {
                int UnitStrLen;

                int UnitStrSeparatorCharIndex = unitString.IndexOfAny(separators, UnitStrStartCharIndex);
                if (UnitStrSeparatorCharIndex == -1)
                {
                    UnitStrLen = unitString.Length - UnitStrStartCharIndex;

                    NextUnitStrStartCharIndex = unitString.Length;
                }
                else
                {
                    UnitStrLen = UnitStrSeparatorCharIndex - UnitStrStartCharIndex;

                    NextUnitStrStartCharIndex = UnitStrSeparatorCharIndex + 1;
                }

                if (UnitStrLen > 0)
                {
                    UnitStrCount++;
                    string UnitFieldString = unitString.Substring(UnitStrStartCharIndex, UnitStrLen).Trim();

                    IPhysicalUnit tempPU = ParseUnit(pu, ref UnitFieldString);

                    if (tempPU == null)
                    {
                        ValidFractionalUnit = false;
                        resultLine = "'" + UnitFieldString + "' is not a valid unit.";
                        if (ThrowExceptionOnInvalidInput)
                        {
                            throw new PhysicalUnitFormatException("The string argument unitString is not in a valid physical unit format. " + resultLine);
                        }
                    }
                    else
                    {
                        FractionUnitSeparatorStr = FractionUnitSeparator.ToString();
                        FractionalUnits.Push(new Tuple<string, IPhysicalUnit>(FractionUnitSeparatorStr, tempPU));

                        LastUnitFieldRemainingLen = UnitFieldString.Length;
                        if (LastUnitFieldRemainingLen != 0)
                        {   // Unparsed chars in (last?) field
                            UnitStrLen -= LastUnitFieldRemainingLen;
                        }
                    }
                }

                // Shift to next field
                if (UnitStrSeparatorCharIndex >= 0)
                {
                    FractionUnitSeparator = unitString[UnitStrSeparatorCharIndex];
                }
                UnitStrStartCharIndex = NextUnitStrStartCharIndex;
            }

            unitString = unitString.Substring(NextUnitStrStartCharIndex - LastUnitFieldRemainingLen);

            foreach (Tuple<string, IPhysicalUnit> tempFU in FractionalUnits)
            {
                IPhysicalUnit tempPU = tempFU.Item2;
                String tempFractionUnitSeparator = tempFU.Item1;
                if (pu == null)
                {
                    pu = tempPU;
                    FractionUnitSeparatorStr = tempFractionUnitSeparator;
                }
                else
                {
                    if (new PhysicalQuantity(tempPU).ConvertTo(pu) != null)
                    {
                        Debug.Assert(FractionUnitSeparatorStr != null, "Unit separator needed");
                        pu = new PhysicalMeasure.MixedUnit(tempPU, FractionUnitSeparatorStr, pu);

                        FractionUnitSeparatorStr = tempFractionUnitSeparator;
                    }
                    else
                    {
                        Debug.Assert(resultLine == null, "No resultLine expected");
                        resultLine = tempPU.ToPrintString() + " is not a valid fractional unit for " + pu.ToPrintString() + ".";

                        if (ThrowExceptionOnInvalidInput)
                        {
                            throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. " + resultLine);
                        }
                    }
                }
            }
            return pu;
        }

        // Token kind enum values
        public enum TokenKind
        {
            None = 0,
            Unit = 1,
            Exponent = 2,
            Operator = 3
        }

        // Operator kind enum values
        // Precedence for a group of operators is same as first (lowest) enum in the group
        public enum OperatorKind
        {
            // Precedence == 2
            Mult = 2,
            Div = 3,

            //Precedence == 4
            Pow = 4,
            Root = 5,
        }

        private static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            OperatorKind precedence = (OperatorKind)((int)operatoren & 0XE);
            return precedence;
        }

        private class token
        {
            public readonly TokenKind TokenKind;

            public readonly IPhysicalUnit PhysicalUnit;
            public readonly SByte Exponent;
            public readonly OperatorKind Operator;

            public token(IPhysicalUnit physicalUni)
            {
                this.TokenKind = TokenKind.Unit;
                this.PhysicalUnit = physicalUni;
            }

            public token(SByte exponent)
            {
                this.TokenKind = TokenKind.Exponent;
                this.Exponent = exponent;
            }

            public token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }
        }

        private class expressionTokenizer
        {
            private String InputString;
            private int Pos = 0;
            private int AfterLastOperandPos = 0;
            private int LastValidPos = 0;
            private Boolean InputRecognized = true;
            private IPhysicalUnit Dimensionless = Physics.dimensionless;
            private Boolean ThrowExceptionOnInvalidInput = false;

            private Stack<OperatorKind> Operators = new Stack<OperatorKind>();
            private List<token> Tokens = new List<token>();

            private TokenKind LastReadToken = TokenKind.None;

            public expressionTokenizer(String InputString)
            {
                this.InputString = InputString;
            }

            public expressionTokenizer(IPhysicalUnit dimensionless, String InputString)
            {
                this.Dimensionless = dimensionless;
                this.InputString = InputString;
            }

            public expressionTokenizer(IPhysicalUnit dimensionless, Boolean throwExceptionOnInvalidInput, String InputString)
            {
                this.Dimensionless = dimensionless;
                this.ThrowExceptionOnInvalidInput = throwExceptionOnInvalidInput;
                this.InputString = InputString;
            }

            public string GetRemainingInput()
            {
                return InputString.Substring(Pos);
            }

            public string GetRemainingInputForLastValidPos()
            {
                return InputString.Substring(LastValidPos);
            }

            public void SetValidPos()
            {
                if (Operators.Count <= 1 && Tokens.Count == 0)
                {
                    LastValidPos = AfterLastOperandPos;
                }
            }

            private Boolean PushNewOperator(OperatorKind newOperator)
            {
                if (LastReadToken != TokenKind.Operator)
                {
                    if (Operators.Count > 0)
                    {
                        // Pop operators with precedence higher than new operator
                        OperatorKind Precedence = OperatorPrecedence(newOperator);
                        while ((Operators.Count > 0) && (Operators.Peek() >= Precedence))
                        {
                            Tokens.Add(new token(Operators.Pop()));
                        }
                    }
                    Operators.Push(newOperator);
                    LastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    if (ThrowExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing unit at position " + Pos.ToString());
                    }

                    return false;
                }
            }

            private void HandleNewOperator(OperatorKind newOperator)
            {   // Push newOperator and shift Pos or mark as failed
                if (PushNewOperator(newOperator))
                {
                    Pos++;
                }
                else
                {
                    InputRecognized = false;
                }
            }

            private token RemoveFirstToken()
            {   // return first operator from post fix operators
                token Token = Tokens[0];
                Tokens.RemoveAt(0);

                return Token;
            }

            public token GetToken()
            {
                Debug.Assert(InputString != null, "Source needed");

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }
                int OperatorsCountForRecognizedTokens = Operators.Count;
                while ((InputString.Length > Pos) && InputRecognized)
                {
                    Char c = InputString[Pos];
                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        Pos++;
                    }
                    else if (c == '*'
                             || c == '·') // center dot  '\0x0B7' (char)183 U+00B7
                    {
                        HandleNewOperator(OperatorKind.Mult);
                    }
                    else if (c == '/')
                    {
                        HandleNewOperator(OperatorKind.Div);
                    }
                    else if (c == '^')
                    {
                        HandleNewOperator(OperatorKind.Pow);
                    }
                    else if (c == '-'
                             || c == '+'
                             || Char.IsDigit(c))
                    {
                        // An exponent
                        if ((LastReadToken != TokenKind.Unit)                // Exponent can follow unit directly 
                            && ((LastReadToken != TokenKind.Operator)          // or follow Pow operator 
                                 || (Operators.Peek() != OperatorKind.Pow)))
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An exponent must follow a unit or Pow operator. Invalid exponent at '" + c + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                // return null;
                                InputRecognized = false;
                            }
                        }
                        else
                        {
                            //// Try to read an exponent from input

                            Int16 numlen = 1;

                            int maxlen = Math.Min(InputString.Length - Pos, 1 + 3); // Max length of sign and digits to look for
                            while (numlen < maxlen && Char.IsDigit(InputString[Pos + numlen]))
                            {
                                numlen++;
                            }

                            SByte exponent;
                            if (numlen > 0 && SByte.TryParse(InputString.Substring(Pos, numlen), out exponent))
                            {
                                if ((LastReadToken == TokenKind.Operator)
                                    && (Operators.Peek() == OperatorKind.Pow))
                                {
                                    // Exponent follow Pow operator; 
                                    // Remove Pow operator from operator stack since it is handled as implicit in parser.
                                    Operators.Pop();
                                }

                                Pos += numlen;
                                AfterLastOperandPos = Pos;

                                LastReadToken = TokenKind.Exponent;

                                return new token(exponent);
                            }
                            else
                            {
                                if (ThrowExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing exponent after '" + c + "' at position " + Pos.ToString());
                                }
                                else
                                {
                                    // return null;
                                    InputRecognized = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        /*
                        if ((LastReadToken == TokenKind.Unit)
                            || (LastReadToken == TokenKind.Exponent)
                            || ((LastReadToken == TokenKind.Operator)    // Unit follow Pow operator; 
                                && (Operators.Peek() == OperatorKind.Pow)))
                         */
                        if ((LastReadToken == TokenKind.Operator)    // Unit follow Pow operator; 
                            && (Operators.Peek() == OperatorKind.Pow))
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An unit must not follow an pow operator. Missing exponent at '" + c + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                InputRecognized = false;
                            }
                        }
                        else
                        {
                            // Try to read a unit from input
                            int maxlen = Math.Min(1 + 3, InputString.Length - Pos); // Max length of scale and symbols to look for

                            String tempstr = InputString.Substring(Pos, maxlen);
                            maxlen = tempstr.IndexOfAny(new Char[] { ' ', '*', '·', '/', '^', '+', '-', '(', ')' });  // '·'  center dot '\0x0B7' (Char)183 U+00B7
                            if (maxlen < 0)
                            {
                                maxlen = tempstr.Length;
                            }

                            for (int unitlen = maxlen; unitlen > 0; unitlen--)
                            {
                                String UnitStr = tempstr.Substring(0, unitlen);
                                IPhysicalUnit su = Physics.UnitSystems.ScaledUnitFromSymbol(UnitStr);
                                if (su != null)
                                {
                                    if (LastReadToken == TokenKind.Unit)
                                    {   // Assume implicit Mult operator
                                        PushNewOperator(OperatorKind.Mult);
                                    }

                                    Pos += unitlen;
                                    AfterLastOperandPos = Pos;

                                    LastReadToken = TokenKind.Unit;
                                    return new token(su);
                                }
                            }

                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid unit '" + InputString.Substring(Pos, maxlen) + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                InputRecognized = false;
                            }
                        }
                    }

                    if (Tokens.Count > 0)
                    {   // return first operator from post fix operators
                        return RemoveFirstToken();
                    }
                };

                if (!InputRecognized)
                {
                    // Remove operators from stack which was pushed for not recognized input
                    while (Operators.Count > OperatorsCountForRecognizedTokens)
                    {
                        Operators.Pop();
                    }
                }
                //// Retrieve remaining operators from stack
                while (Operators.Count > 0)
                {
                    Tokens.Add(new token(Operators.Pop()));
                }

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                return null;
            }
        }

        public static IPhysicalUnit ParseUnit(IPhysicalUnit dimensionless, ref String s)
        {
            if (dimensionless == null)
            {
                dimensionless = Physics.dimensionless;
            }

            expressionTokenizer Tokenizer = new expressionTokenizer(dimensionless, /* throwExceptionOnInvalidInput = */ false, s);

            Stack<IPhysicalUnit> Operands = new Stack<IPhysicalUnit>();

            Boolean InputTokenInvalid = false;
            Tokenizer.SetValidPos();
            token Token = Tokenizer.GetToken();

            while (Token != null && !InputTokenInvalid)
            {
                if (Token.TokenKind == TokenKind.Unit)
                {
                    // Stack unit operand
                    Operands.Push(Token.PhysicalUnit);
                }
                else if (Token.TokenKind == TokenKind.Exponent)
                {
                    IPhysicalUnit pu = Operands.Pop();

                    // Combine pu and exponent to the new unit pu^exponent   
                    Operands.Push(pu.CombinePow(Token.Exponent));
                }
                else if (Token.TokenKind == TokenKind.Operator)
                {
                    /****
                     * Pow operator is handled implicit
                     * 
                    if (Token.Operator == OperatorKind.Pow)
                    {
                        Debug.Assert(Operands.Count >= 1, "The Operands.Count must be 1 or more");
                        SByte exponentSecond = Operands.Pop();
                        IPhysicalUnit puFirst = Operands.Pop();
                        // Combine pu and exponent to the new unit pu^exponent   
                        Operands.Push(puFirst.CombinePow(exponentSecond));
                    }
                    else
                    ****/
                    if (Operands.Count >= 2)
                    {
                        Debug.Assert(Operands.Count >= 2, "Two operands needed");

                        IPhysicalUnit puSecond = Operands.Pop();
                        IPhysicalUnit puFirst = Operands.Pop();

                        if (Token.Operator == OperatorKind.Mult)
                        {
                            // Combine pu1 and pu2 to the new unit pu1*pu2   
                            Operands.Push(puFirst.CombineMultiply(puSecond));
                        }
                        else if (Token.Operator == OperatorKind.Div)
                        {
                            // Combine pu1 and pu2 to the new unit pu1/pu2
                            Operands.Push(puFirst.CombineDivide(puSecond));
                        }
                    }
                    else
                    {   // Missing operand(s). Operator not valid part of (this) unit
                        InputTokenInvalid = true;
                    }
                }
                if (!InputTokenInvalid)
                {
                    if (Operands.Count == 1)
                    {
                        Tokenizer.SetValidPos();
                    }
                    Token = Tokenizer.GetToken();
                }
            }

            s = Tokenizer.GetRemainingInputForLastValidPos(); // Remaining of input string

            Debug.Assert(Operands.Count <= 1, "Only one operand is allowed");  // 0 or 1

            return (Operands.Count > 0) ? Operands.Last() : null;
        }

        #endregion IPhysicalUnit unit expression parser methods

        #endregion Unit Expression parser methods

        #region Unit print string methods
        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public abstract String PureUnitString();

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// and prefixed with '(FactorValue)' if FactorValue is not 1
        /// </summary>
        public String UnitString()
        {
            String unitStr = PureUnitString();
            if (FactorValue != 1)
            {
                unitStr = "(" + FactorValue + ") " + unitStr;
            }

            return unitStr;
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public virtual String UnitPrintString()
        {
            return this.UnitString();
        }

        public virtual String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            Debug.Assert(invertExponents == false, "The invertExponents must be false");
            return this.UnitString();
        }

        /// <summary>
        /// String formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.2
        /// </summary>
        public virtual String ReducedUnitString()
        {
            return this.UnitString();
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// Eventually with system name prefixed.
        /// </summary>
        public override String ToString()
        {
            String UnitName = this.UnitString();
            IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
            if ((!String.IsNullOrEmpty(UnitName))
                && (system != null)
                && (system != Physics.CurrentUnitSystems.Default)
                 /*
                 && (!(    Physics.SI_Units == Physics.Default_UnitSystem 
                        && system.IsCombinedUnitSystem 
                        && ((ICombinedUnitSystem)system).ContainsSubUnitSystem(Physics.Default_UnitSystem) ))
                  */
                 )
            {
                UnitName = system.Name + "." + UnitName;
            }

            return UnitName;
        }

        /// <summary>
        /// IPhysicalUnit.ToPrintString implementation.
        /// With system name prefixed if system specified.
        /// </summary>
        public virtual String ToPrintString()
        {
            String UnitName = this.UnitPrintString();
            if (String.IsNullOrEmpty(UnitName))
            {
                UnitName = "dimensionless";
            }
            else
            {
                IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
                if ((system != null)
                    && (system != Physics.CurrentUnitSystems.Default))
                {
                    UnitName = system.Name + "." + UnitName;
                }
            }
            return UnitName;
        }

        public virtual string ValueString(double quantity)
        {
            return quantity.ToString();
        }

        public virtual string ValueString(double quantity, String format, IFormatProvider formatProvider)
        {
            String ValStr = null;
            try
            {
                ValStr = quantity.ToString(format, formatProvider);
            }
            catch
            {
                ValStr = quantity.ToString() + " ?" + format + "?";
            }
            return ValStr;
        }

        /*
        public abstract Double FactorValue { get; }
        //public abstract IPhysicalUnit PureUnit { get; }
        public abstract IUnit PureUnit { get; }
        */

        public virtual double FactorValue { get { return 1; } }
        public virtual IPhysicalUnit PureUnit { get { return this; } }

        #endregion Unit print string methods

        #region Unit conversion methods

        public abstract bool IsLinearConvertible();


        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)

        public IPhysicalQuantity this[IPhysicalUnit convertToUnit]
        {
            get { return this.ConvertTo(convertToUnit); }
        }

        public IPhysicalQuantity this[IPhysicalQuantity convertToUnit]
        {
            get { return this.ConvertTo(convertToUnit.Unit).Multiply(convertToUnit.Value); }
        }

        public virtual IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null, "The convertToUnit must be specified");

            // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
            IPhysicalQuantity pq = null;
            IPhysicalQuantity pq_systemUnit = this.ConvertToSystemUnit();
            if (pq_systemUnit != null)
            {
                IPhysicalQuantity pq_toUnit = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit.Unit, convertToUnit);
                if (pq_toUnit != null)
                {
                    pq = pq_toUnit.Multiply(pq_systemUnit.Value);
                }
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertToUnitSystem != null, "The convertToUnitSystem must be specified");

            // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
            IPhysicalQuantity pq = null;
            IPhysicalQuantity pq_systemUnit = this.ConvertToSystemUnit();
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit.Unit, convertToUnitSystem);
                if (pq != null && pq_systemUnit.Value != 1)
                {
                    pq = pq.Multiply(pq_systemUnit.Value);
                }
            }
            return pq;
        }

        public abstract IPhysicalQuantity ConvertToSystemUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        public virtual IPhysicalQuantity ConvertTo(ref double quantity, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null, "The convertToUnit must be specified");

            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            IPhysicalQuantity pq = null;
            IPhysicalQuantity pq_systemUnit = this.ConvertToSystemUnit(ref quantity);
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit, convertToUnit);
            }
            //// Mark quantity as used now
            quantity = 1;
            return pq;
        }

        public virtual IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertToUnitSystem != null, "The convertToUnitSystem must be specified");

            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            IPhysicalQuantity pq = null;
            IPhysicalQuantity pq_systemUnit = this.ConvertToSystemUnit(ref quantity);
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit, convertToUnitSystem);
            }
            //// Mark quantity as used now
            quantity = 1;
            return pq;
        }

        public virtual IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            // TODO : Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            Debug.Assert(false);  //
            IPhysicalQuantity pq = this.ConvertToSystemUnit().Multiply(quantity);
            //// Mark quantity as used now
            quantity = 1;
            return pq;
        }

        public abstract IPhysicalQuantity ConvertToBaseUnit();

        public abstract IPhysicalQuantity ConvertToBaseUnit(double quantity);

        //public abstract IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        public virtual IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }


        /*
        public abstract IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem);

        public abstract IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem);

        public abstract IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem);
        */


        /// <summary>
        /// 
        /// </summary>
        public /* virtual */ IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem)
        {
            // return this.ConvertTo(convertToUnitSystem).pq.ConvertToBaseUnit();
            IPhysicalQuantity pq = this.ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.ConvertToBaseUnit();
            }
            return pq;
        }

        /// <summary>
        /// 
        public /* virtual */ IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem)
        {
            // return new PhysicalQuantity(quantity, this).ConvertTo(convertToUnitSystem).ConvertToBaseUnit();
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this).ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.ConvertToBaseUnit();
            }
            return pq;
        }


        /// <summary>
        /// 
        /// </summary>
        public /* virtual */ IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            //return physicalQuantity.ConvertTo(this).ConvertTo(convertToUnitSystem).ConvertToBaseUnit();
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnitSystem);

                if (pq != null)
                {
                    pq = pq.ConvertToBaseUnit();
                }
            }
            return pq;
        }

        public abstract IPhysicalQuantity ConvertToDerivedUnit();
        /**
        {
            IPhysicalQuantity pq = this.ConvertToBaseUnit();
            IPhysicalQuantity pq_unit = pq.Unit.
            return pq;
        }
        **/

        public bool Equivalent(IPhysicalUnit other, out double quotient)
        {
            // quotient = 0;
            // return false;


            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                quotient = 0;
                return false;
            }


            /*
             * This will make recursive call to this function
            if (this == other)
            {
                return true;
            }
             */

            IPhysicalQuantity pq1;
            IPhysicalQuantity pq2;

            if (this.ExponentsSystem != other.ExponentsSystem)
            {   // Must be same unit system


                if (this.ExponentsSystem == null || other.ExponentsSystem == null)
                {
                    if (this.IsDimensionless && other.IsDimensionless)
                    {
                        // Any dimensionless can be converted to any systems dimensionless
                        //return this.FactorValue == other.FactorValue;
                        quotient = other.FactorValue / this.FactorValue;
                        return true;
                    }
                }

                if (this.ExponentsSystem.IsCombinedUnitSystem || other.ExponentsSystem.IsCombinedUnitSystem)
                {
                    if (this.ExponentsSystem.IsCombinedUnitSystem && other.ExponentsSystem.IsCombinedUnitSystem)
                    {
                        // Check for same sub systems
                        CombinedUnitSystem cus_this = (CombinedUnitSystem)this.ExponentsSystem;
                        CombinedUnitSystem cus_other = (CombinedUnitSystem)other.ExponentsSystem;
                        if (cus_this.Equals(cus_other))
                        {
                            Debug.Assert(false); // Missing check for scale factors and converted units
                            if (!this.Exponents.DimensionEquals(other.Exponents))
                            {
                                quotient = 0;
                                return false;
                            }
                        }
                    }
                }

                pq2 = other.ConvertTo(this.ExponentsSystem);
                if (Object.ReferenceEquals(null, pq2))
                {
                    quotient = 0;
                    return false;
                }

                // return new PhysicalQuantity(1, this) == other_pq;
                pq1 = new PhysicalQuantity(1, this);
                //return pq1.Equals(pq2);
                return pq1.Equivalent(pq2, out quotient);
            }


            if (this.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit this_imu = (IMixedUnit)this;
                return this_imu.MainUnit.Equivalent(other, out quotient);
            }
            else if (other.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit other_imu = (IMixedUnit)other;
                return this.Equivalent(other_imu.MainUnit, out quotient);
            }
            else if (this.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit this_icu = (IConvertibleUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq1 = this_icu.ConvertToPrimaryUnit();
                pq2 = other.ConvertTo(this_icu.PrimaryUnit);
                if (pq2 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;

            }
            else if (other.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit other_icu = (IConvertibleUnit)other;

                if (!other_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq2 = other_icu.ConvertToPrimaryUnit();
                pq1 = this.ConvertTo(pq2.Unit);
                if (pq1 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;

            }
            else if (this.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit this_pu = (IPrefixedUnit)this;
                Double tempQuotient;
                Boolean equivalent = this_pu.Unit.Equivalent(other, out tempQuotient);
                if (equivalent)
                {
                    quotient = this_pu.Prefix.Value * tempQuotient;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (other.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit other_pu = (IPrefixedUnit)other;
                Double tempQuotient;
                Boolean equivalent = this.Equivalent(other_pu.Unit, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / other_pu.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (this.Kind == UnitKind.PrefixedUnitExponent)
            {
                IPrefixedUnitExponent this_pue = (IPrefixedUnitExponent)this;
                Double tempQuotient;
                Boolean equivalent = this_pue.Unit.Equivalent(other, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / this_pue.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (other.Kind == UnitKind.PrefixedUnitExponent)
            {
                IPrefixedUnitExponent other_pue = (IPrefixedUnitExponent)other;
                Double tempQuotient;
                Boolean equivalent = this.Equivalent(other_pue.Unit, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / other_pue.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (this.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit this_icu = (ICombinedUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq2 = other.ConvertToDerivedUnit();
                IPhysicalQuantity pq_this = this_icu.ConvertToDerivedUnit();
                pq1 = pq_this.ConvertTo(pq2.Unit);
                if (pq1 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }
            else if (other.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit other_icu = (ICombinedUnit)other;
                if (!other_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq1 = this.ConvertToDerivedUnit();
                IPhysicalQuantity pq_other = other_icu.ConvertToDerivedUnit();
                pq2 = pq_other.ConvertTo(pq1.Unit);
                if (pq2 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }

            Debug.Assert(this.Kind == UnitKind.BaseUnit || this.Kind == UnitKind.DerivedUnit);

            Boolean equals2 = this.Exponents.DimensionEquals(other.Exponents);
            if (!equals2)
            {
                quotient = 0;
                return false;
            }

            quotient = other.FactorValue / this.FactorValue;
            return true;
        }


        public virtual bool Equals(IPhysicalUnit other)
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                return false;
            }

            double quotient;
            bool equals = this.Equivalent(other, out quotient);
            return equals && quotient == 1;
        }
#if NEVER
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                return false;
            }


            /*
             * This will make recursive call to this function
            if (this == other)
            {
                return true;
            }
             */

            IPhysicalQuantity pq1;
            IPhysicalQuantity pq2;

            if (this.ExponentsSystem != other.ExponentsSystem)
            {   // Must be same unit system


                if (this.ExponentsSystem == null || other.ExponentsSystem == null)
                {
                    if (this.IsDimensionless && other.IsDimensionless)
                    {
                        // Any dimensionless can be converted to any systems dimensionless
                        return this.FactorValue == other.FactorValue;
                    }
                }

                if (this.ExponentsSystem.IsCombinedUnitSystem || other.ExponentsSystem.IsCombinedUnitSystem)
                {
                    if (this.ExponentsSystem.IsCombinedUnitSystem && other.ExponentsSystem.IsCombinedUnitSystem)
                    {
                        // Check for same sub systems
                        CombinedUnitSystem cus_this = (CombinedUnitSystem)this.ExponentsSystem;
                        CombinedUnitSystem cus_other = (CombinedUnitSystem)other.ExponentsSystem;
                        if (cus_this.Equals(cus_other))
                        {
                            Debug.Assert(false); // Missing check for scale factors and converted units
                            if (!this.Exponents.DimensionEquals(other.Exponents))
                            {
                                return false;
                            }
                        }
                    }
                }

                pq2 = other.ConvertTo(this.ExponentsSystem);
                if (Object.ReferenceEquals(null, pq2))
                {
                    return false;
                }

                // return new PhysicalQuantity(1, this) == other_pq;
                pq1 = new PhysicalQuantity(1, this);
                return pq1.Equals(pq2);
            }


            if (this.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit this_imu = (IMixedUnit)this;
                return this_imu.MainUnit.Equals(other);
            }
            else if (other.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit other_imu = (IMixedUnit)other;
                return this.Equals(other_imu.MainUnit);
            }
            else if (this.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit this_icu = (IConvertibleUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    return false;
                }
                pq1 = this_icu.ConvertToPrimaryUnit();
                pq2 = other.ConvertTo(this_icu.PrimaryUnit);
                return pq2 != null && pq1.Equals(pq2);
            }
            else if (other.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit other_icu = (IConvertibleUnit)other;

                if (!other_icu.IsLinearConvertible())
                {
                    return false;
                }
                pq2 = other_icu.ConvertToPrimaryUnit();
                pq1 = this.ConvertTo(pq2.Unit);
                return pq1 != null && pq1.Equals(pq2);
            }
            else if (this.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit this_icu = (ICombinedUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    return false;
                }
                pq2 = other.ConvertToBaseUnit();
                IPhysicalQuantity pq_this = this_icu.ConvertToBaseUnit();
                pq1 = pq_this.ConvertTo(pq2.Unit);
                return pq1 != null && pq1.Equals(pq2);
            }
            else if (other.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit other_icu = (ICombinedUnit)other;
                if (!other_icu.IsLinearConvertible())
                {
                    return false;
                }
                pq1 = this.ConvertToBaseUnit();
                IPhysicalQuantity pq_other = other_icu.ConvertToBaseUnit();
                pq2 = pq_other.ConvertTo(pq1.Unit);
                return pq2 != null && pq1.Equals(pq2);
            }

            Debug.Assert(this.Kind == UnitKind.BaseUnit || this.Kind == UnitKind.DerivedUnit);

            return this.Exponents.DimensionEquals( other.Exponents);


            /**
            if (   (this.Kind != UnitKind.ConvertibleUnit)
                && (other.Kind != UnitKind.ConvertibleUnit)
                && (this.Kind != UnitKind.MixedUnit)
                && (other.Kind != UnitKind.MixedUnit))
            {
                return this.Exponents.DimentionsEquals(other.Exponents);
            }

            pq1 = this.ConvertToSystemUnit();
            pq2 = other.ConvertToSystemUnit();

            return pq1.Equals(pq2);
            **/
        }
#endif // NEVER

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            if (obj is IPhysicalUnit)
            {
                return this.Equals(obj as IPhysicalUnit);
            }

            Debug.Assert(obj is IPhysicalUnit, "The 'obj' argument is not a IPhysicalUnit object");

            return false;
        }

        public static bool operator ==(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            Debug.Assert(null != unit1, "The 'unit1' parameter must be specified");

            return unit1.Equals(unit2);
        }

        public static bool operator !=(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            ////Debug.Assert(null != unit1, "The 'unit1' parameter must be specified");

            return !unit1.Equals(unit2);
        }


        #endregion Unit conversion methods

        #region Unit static operator methods

        internal delegate SByte CombineExponentsFunc(SByte e1, SByte e2);
        internal delegate Double CombineQuantitiesFunc(Double q1, Double q2);

        internal static PhysicalQuantity CombineUnits(IPhysicalUnit u1, IPhysicalUnit u2, CombineExponentsFunc cef, CombineQuantitiesFunc cqf)
        {
            /*
            IPhysicalQuantity u1_pq = u1.ConvertToSystemUnit().ConvertToBaseUnit();
            IPhysicalQuantity u2_pq = u2.ConvertToSystemUnit().ConvertToBaseUnit();
            if (u2_pq.Unit.ExponentsSystem != u1_pq.Unit.ExponentsSystem)
            {
                u2_pq = u1_pq.Unit.ExponentsSystem.ConvertTo(u2_pq, u1_pq.Unit.ExponentsSystem);
                if (u2_pq == null)
                {   // Found no conversion from u1_pq.Unit.System to u2_pq.Unit.System
                    return null; 
                }

                u2_pq = u2_pq.ConvertToBaseUnit();
            }
            */

            IUnitSystem us = u1.ExponentsSystem;
            IPhysicalQuantity u1_pq = u1.ConvertToBaseUnit(us);
            IPhysicalQuantity u2_pq = u2.ConvertToBaseUnit(us);

            if (u1_pq == null || u2_pq == null)
            {
                // Found no conversion from u1_pq.Unit.System to u2_pq.Unit.System
                return null;
            }

            SByte[] u1Exponents = u1_pq.Unit.Exponents;
            SByte[] u2Exponents = u2_pq.Unit.Exponents;
            SByte u1ExponentsLen = (SByte)u1_pq.Unit.Exponents.Length;
            SByte u2ExponentsLen = (SByte)u2_pq.Unit.Exponents.Length;
            int NoOfBaseUnits = Math.Max(u1ExponentsLen, u2ExponentsLen);
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties, "The 'NoOfBaseUnits' must be <= Physics.NoOfBaseQuanties");

            SByte[] combinedExponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                SByte u1Exponent = 0;
                SByte u2Exponent = 0;
                if (i < u1ExponentsLen)
                {
                    u1Exponent = u1Exponents[i];
                }
                if (i < u2ExponentsLen)
                {
                    u2Exponent = u2Exponents[i];
                }
                combinedExponents[i] = cef(u1Exponent, u2Exponent);
            }
            Debug.Assert(u1.ExponentsSystem != null, "The 'u1.ExponentsSystem' must be specified");
            PhysicalUnit pu = new DerivedUnit(u1.ExponentsSystem, combinedExponents);
            return new PhysicalQuantity(cqf(u1_pq.Value, u2_pq.Value), pu);
        }

        internal static PhysicalUnit CombineUnitExponents(IPhysicalUnit u, SByte exponent, CombineExponentsFunc cef)
        {
            SByte[] exponents = u.Exponents;
            int NoOfBaseUnits = exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties, "The 'NoOfBaseUnits' must be <= Physics.NoOfBaseQuanties");

            SByte[] someExponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                someExponents[i] = cef(u.Exponents[i], exponent);
            }

            // Not valid during SI system initialization: Debug.Assert(u.System != null);
            PhysicalUnit pu = new DerivedUnit(u.ExponentsSystem, someExponents);
            return pu;
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, IUnitPrefix up)
        {
            Debug.Assert(up != null, "The 'up' parameter must be specified");

            return new PhysicalQuantity(up.Value, u);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalUnit u)
        {
            Debug.Assert(up != null, "The 'up' parameter must be specified");

            return new PhysicalQuantity(up.Value, u);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, double d)
        {
            return new PhysicalQuantity(d, u);
        }

        public static PhysicalQuantity operator /(PhysicalUnit u, double d)
        {
            return new PhysicalQuantity(1 / d, u);
        }

        public static PhysicalQuantity operator *(double d, PhysicalUnit u)
        {
            return new PhysicalQuantity(d, u);
        }

        public static PhysicalQuantity operator /(double d, PhysicalUnit u)
        {
            return new PhysicalQuantity(d, 1 / u);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPhysicalUnit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The 'u1' parameter must be specified");

            return new PhysicalQuantity(u1.Multiply(u2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPhysicalUnit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The 'u1' parameter must be specified");

            return new PhysicalQuantity(u1.Divide(u2));
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The 'u1' parameter must be specified");

            return new PhysicalQuantity(u1.Multiply(pue2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The 'u1' parameter must be specified");

            return new PhysicalQuantity(u1.Divide(pue2));
        }

        public static PhysicalQuantity operator ^(PhysicalUnit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The 'u' parameter must be specified");

            return new PhysicalQuantity(u.Pow(exponent));
        }

        public static PhysicalQuantity operator %(PhysicalUnit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The 'u' parameter must be specified");
            return new PhysicalQuantity(u.Rot(exponent));
        }

        #endregion Unit static operator methods

        public virtual IPhysicalQuantity AsPhysicalQuantity()
        {
            return AsPhysicalQuantity(1);
        }

        public virtual IPhysicalQuantity AsPhysicalQuantity(double quantity)
        {
            IPhysicalQuantity pue_pq = new PhysicalQuantity(quantity, this);
            return pue_pq;
        }

        /**
        public virtual IPhysicalQuantity AsPhysicalQuantity(IUnitSystem unitSystem)
        {
            Debug.Assert(unitSystem != null, "The 'unitSystem' must be valid and not null");

            IPhysicalQuantity pue_pq = this.ConvertTo(unitSystem);
            return pue_pq;
        }
        **/

        /*
        public virtual IPhysicalQuantity AsPhysicalQuantity(ref double quantity, IUnitSystem unitSystem)
        {
            // Obsolete
            Debug.Assert(false);

            IPhysicalQuantity pue_pq = this.AsPhysicalQuantity(unitSystem).Multiply(quantity);
            return pue_pq;
        }
        */

        /**
        public static implicit operator AsPhysicalQuantity(PrefixedUnit prefixedUnit)
        {
            return prefixedUnit.AsPhysicalQuantity() as PhysicalQuantity;
        }
        **/

        //
        // public virtual PhysicalQuantity Power(SByte exponent)
        public virtual PhysicalUnit Power(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 * e2));
        }

        //
        //public virtual PhysicalQuantity Root(SByte exponent)
        public virtual PhysicalUnit Root(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 / e2));
        }

        #region Unit math methods
        //public interface IUnitMath : IEquatable<IPhysicalUnit>  // Maybe IOverUnitMath : IEquatable<IPhysicalUnit>


        public virtual IPhysicalUnit Multiply(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The 'prefix' parameter must be specified");
            return this.CombineMultiply(prefix);
        }

        public virtual IPhysicalUnit Divide(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The 'prefix' parameter must be specified");
            return this.CombineDivide(prefix);
        }

        public virtual IPhysicalUnit Multiply(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The 'namedSymbolUnit' parameter must be specified");
            return this.CombineMultiply(namedSymbolUnit);
        }

        public virtual IPhysicalUnit Divide(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The 'namedSymbolUnit' parameter must be specified");
            return this.CombineDivide(namedSymbolUnit);
        }


        public virtual IPhysicalUnit Multiply(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The 'prefixedUnit' parameter must be specified");
            return this.CombineMultiply(prefixedUnit);
        }

        public virtual IPhysicalUnit Divide(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The 'prefixedUnit' parameter must be specified");
            return this.CombineDivide(prefixedUnit);
        }


        public virtual IPhysicalUnit Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The 'prefixedUnitExponent' parameter must be specified");
            return this.CombineMultiply(prefixedUnitExponent);
        }

        public virtual IPhysicalUnit Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The 'prefixedUnitExponent' parameter must be specified");
            return this.CombineDivide(prefixedUnitExponent);
        }


        public virtual IPhysicalUnit Multiply(IPhysicalUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineMultiply(physicalUnit);
        }

        public virtual IPhysicalUnit Divide(IPhysicalUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineDivide(physicalUnit);
        }



        /*
        //
        public IPhysicalQuantity Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        //
        public IPhysicalQuantity Rot(SByte exponent)
        {
            return this.Root(exponent);
        }
        */

        public virtual IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalUnit pu = this.Multiply(physicalQuantity.Unit);
            return pu.Multiply(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalUnit pu = this.Divide(physicalQuantity.Unit);
            return pu.Divide(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Multiply(double quantity)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this);
            return pq;
        }

        public virtual IPhysicalQuantity Divide(double quantity)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1 / quantity, this);
            return pq;
        }


        public virtual IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalUnit pu = this.Multiply(physicalQuantity.Unit);
            return pu.Multiply(quantity * physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalUnit pu = this.Divide(physicalQuantity.Unit);
            return pu.Multiply(quantity / physicalQuantity.Value);
        }

        /***
        public virtual IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            IPhysicalQuantity res = CombineUnits(this, physicalUnit, (SByte e1, SByte e2) => (SByte)(e1 + e2), (Double e1, Double e2) => (Double)(e1 * e2));
            if (res == null)
            {   // Found no conversion from this.System to physicalUnit.System
                // return result as combined unit
                PrefixedUnitExponentList numerators = new PrefixedUnitExponentList();
                PrefixedUnitExponentList denominators = new PrefixedUnitExponentList();
                numerators.Add(new PrefixedUnitExponent(this));
                numerators.Add(new PrefixedUnitExponent(physicalUnit));
                res = new PhysicalQuantity(1, new CombinedUnit(numerators, denominators));
            }
            return res;
        }

        public virtual IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            IPhysicalQuantity res = CombineUnits(this, physicalUnit, (SByte e1, SByte e2) => (SByte)(e1 - e2), (Double e1, Double e2) => (Double)(e1 / e2));
            if (res == null)
            {   // Found no conversion from this.System to physicalUnit.System
                // return result as combined unit
                PrefixedUnitExponentList numerators = new PrefixedUnitExponentList();
                PrefixedUnitExponentList denominators = new PrefixedUnitExponentList();
                numerators.Add(new PrefixedUnitExponent(this));
                denominators.Add(new PrefixedUnitExponent(physicalUnit));
                res = new PhysicalQuantity(1, new CombinedUnit(numerators, denominators));
            }
            return res;
        }
        
        
        public virtual IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalQuantity pq2 = this.Multiply(physicalQuantity.Unit);
            return pq2.Multiply(quantity * physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = this.Divide(physicalQuantity.Unit);
            return pq2.Multiply(quantity / physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = this.Multiply(physicalQuantity.Unit);
            return pq2.Multiply(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = this.Divide(physicalQuantity.Unit);
            return pq2.Divide(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = new PhysicalQuantity((Math.Pow(10, prefixedUnit.Prefix.PrefixExponent)), prefixedUnit.Unit);
            return this.Multiply(pq2);
        }

        public virtual IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnit.Prefix.PrefixExponent), prefixedUnit.Unit);
            return this.Divide(pq2);
        }

        public virtual IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnitExponent.Prefix.PrefixExponent), prefixedUnitExponent.Unit);
            pq2 = pq2.Pow(prefixedUnitExponent.Exponent);
            return this.Multiply(pq2);
        }

        public virtual IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnitExponent.Prefix.PrefixExponent), prefixedUnitExponent.Unit);
            return this.Divide(pq2.Pow(prefixedUnitExponent.Exponent));
        }
        ***/

        /**
        public virtual IPhysicalQuantity Multiply(INamedSymbolUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);
            return this.Multiply(pue);
        }

        public virtual IPhysicalQuantity Divide(INamedSymbolUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);

            return this.Divide(pue);
        }
        **/

        public IPhysicalUnit Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        //
        public IPhysicalUnit Rot(SByte exponent)
        {
            return this.Root(exponent);
        }


        #endregion Unit math methods

        #region Unit Combine math methods

        public virtual ICombinedUnit CombineMultiply(double quantity)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(quantity);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(double quantity)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(quantity);
            return uRes;
        }

        public virtual ICombinedUnit CombineMultiply(IUnitPrefixExponent prefixExponent)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixExponent);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(IUnitPrefixExponent prefixExponent)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixExponent);
            return uRes;
        }

        public virtual ICombinedUnit CombineMultiply(INamedSymbolUnit physicalUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(INamedSymbolUnit physicalUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombineMultiply(IPhysicalUnit physicalUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(IPhysicalUnit physicalUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombinePow(SByte exponent)
        {
            // ICombinedUnit uRes = new CombinedUnit(new PrefixedUnitExponent(null, this, exponent));
            ICombinedUnit asCombinedUnit = new CombinedUnit(this);
            ICombinedUnit uRes = asCombinedUnit.CombinePow(exponent);
            return uRes;
        }

        public virtual ICombinedUnit CombineRot(SByte exponent)
        {
            //ICombinedUnit uRes = new CombinedUnit(new PrefixedUnitExponent(null, this, (SByte)(-exponent)));
            ICombinedUnit asCombinedUnit = new CombinedUnit(this);
            ICombinedUnit uRes = asCombinedUnit.CombineRot(exponent);
            return uRes;
        }

        /*
        public virtual ICombinedUnit CombinePrefix(SByte prefixExponent)
        {
            // ICombinedUnit uRes = new CombinedUnit(new PrefixedUnitExponent(new UnitPrefixExponent(prefixExponent), this, 1));
            ICombinedUnit asCombinedUnit = new CombinedUnit(this);
            ICombinedUnit uRes = asCombinedUnit.CombinePrefix(prefixExponent);
            return uRes;
        }
        */

        public virtual ICombinedUnit CombineMultiply(IPrefixedUnit prefixedUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(IPrefixedUnit prefixedUnit)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnit);
            return uRes;
        }

        public virtual ICombinedUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnitExponent);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnitExponent);
            return uRes;
        }

        #endregion Unit Combine math methods

        public static implicit operator PhysicalQuantity(PhysicalUnit physicalUnit)
        {
            return new PhysicalQuantity(physicalUnit);
        }
    }

    public abstract class SystemUnit : PhysicalUnit, ISystemUnit/* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        private IUnitSystem _system;

        // property override get
        public override IUnitSystem SimpleSystem { get { return _system; } set { _system = value; } }
        public override IUnitSystem ExponentsSystem { get { return _system; } }

        protected SystemUnit(IUnitSystem someSystem = null)
        {
            this._system = someSystem;
        }


        //public override IPhysicalQuantity SpecificConvertToSystemUnit(ref double quantity)
        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this);
            return pq;
        }

        //public override IPhysicalQuantity RelativeConvertToSystemUnit()
        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1, this);
            return pq;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem)
        {
            return this.ConvertTo(convertToUnitSystem).ConvertToBaseUnit();
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem)
        {
            return new PhysicalQuantity(quantity, this).ConvertTo(convertToUnitSystem).ConvertToBaseUnit();
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            return physicalQuantity.ConvertTo(this).ConvertTo(convertToUnitSystem).ConvertToBaseUnit();
        }
        **/
    }

    public class BaseUnit : SystemUnit, INamedSymbol, IBaseUnit //, IPrefixedUnit
    {
        private NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } }

        // IUnitPrefix IPrefixedUnit.Prefix { get { return null; } }

        // INamedSymbolUnit IPrefixedUnit.Unit { get { return this; } }

        private SByte _baseunitnumber;
        public SByte BaseUnitNumber { get { return _baseunitnumber; } }

        public override UnitKind Kind { get { return UnitKind.BaseUnit; } }

        public override SByte[] Exponents
        {
            get
            {
                if (_baseunitnumber < 0)
                {
                    Debug.Assert(_baseunitnumber >= 0);
                }

                int NoOfBaseUnits = _baseunitnumber + 1;
                if (SimpleSystem != null && SimpleSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = SimpleSystem.BaseUnits.Length;
                }

                SByte[] tempExponents = new SByte[NoOfBaseUnits];
                tempExponents[_baseunitnumber] = 1;
                return tempExponents;
            }
        }

        public BaseUnit(IUnitSystem someUnitSystem, SByte someBaseUnitNumber, NamedSymbol someNamedSymbol)
            : base(someUnitSystem)
        {
            if (someBaseUnitNumber < 0)
            {
                Debug.Assert(someBaseUnitNumber >= 0);
            }
            this._baseunitnumber = someBaseUnitNumber;
            this.NamedSymbol = someNamedSymbol;
        }

        public BaseUnit(IUnitSystem someUnitSystem, SByte someBaseUnitNumber, String someName, String someSymbol)
            : this(someUnitSystem, someBaseUnitNumber, new NamedSymbol(someName, someSymbol))
        {
        }

        public BaseUnit(SByte someBaseUnitNumber, String someName, String someSymbol)
            : this(null, someBaseUnitNumber, someName, someSymbol)
        {
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            return this.Symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsLinearConvertible()
        {
            //return false;
            return true;
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            return new PhysicalQuantity(1, this);
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            return new PhysicalQuantity(1, this);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return new PhysicalQuantity(quantity, this);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            return physicalQuantity.ConvertTo(this);
        }

        /*
        public override IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem)
        {
            return new PhysicalQuantity(1, this).ConvertTo(convertToUnitSystem);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem)
        {
            return new PhysicalQuantity(quantity, this).ConvertTo(convertToUnitSystem);
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            return physicalQuantity.ConvertTo(this).ConvertTo(convertToUnitSystem);
        }
        */
    }

    public class DerivedUnit : SystemUnit, IDerivedUnit
    {
        private readonly SByte[] _exponents;

        public override UnitKind Kind { get { return UnitKind.DerivedUnit; } }

        public override SByte[] Exponents { get { return _exponents; } }

        public DerivedUnit(IUnitSystem someSystem, SByte[] someExponents = null)
            : base(someSystem)
        {
            this._exponents = someExponents;
        }


        public DerivedUnit(SByte[] someExponents)
            // : this(null, someExponents)
            : this(Physics.SI_Units, someExponents)
        {
        }


        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit, "The 'this.Kind' must be UnitKind.DerivedUnit");

            String ExponentsStr = "";
#if DEBUG // Error traces only included in debug build
            Boolean UnitMissingSystem = false;
#endif
            int index = 0;
            foreach (SByte exponent in Exponents)
            {
                if (exponent != 0)
                {
                    if (!String.IsNullOrEmpty(ExponentsStr))
                    {
                        ExponentsStr += '·'; // center dot '\0x0B7' (Char)183 U+00B7
                    }
                    IUnitSystem thisExponentsSystem = this.ExponentsSystem;
                    if (thisExponentsSystem != null)
                    {
                        ExponentsStr += thisExponentsSystem.BaseUnits[index].Symbol;
                    }
                    else
                    {
#if DEBUG // Error traces only included in debug build
                        //Debug.Assert(false);
                        //Debug.WriteLine("Unit missing system");
                        UnitMissingSystem = true;
#endif
                        ExponentsStr += "<" + index.ToString() + ">";
                    }
                    if (exponent != 1)
                    {
                        ExponentsStr += exponent.ToString();
                    }
                }

                index++;
            }

#if DEBUG // Error traces only included in debug build
            if (UnitMissingSystem)
            {
                // Do some trace of error    
                Debug.WriteLine(global::System.Reflection.Assembly.GetExecutingAssembly().ToString() + " Unit " + this.Kind.ToString() + " { " + ExponentsStr + "} missing unit system.");
            }
#endif

            return ExponentsStr;
        }

        public override bool IsLinearConvertible()
        {
            return true;
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            return new PhysicalQuantity(1, this);
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            return new PhysicalQuantity(1, this);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return new PhysicalQuantity(quantity, this);
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            return physicalQuantity.ConvertTo(this);
        }


        public override IPhysicalUnit Multiply(IPhysicalUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");

            if (physicalUnit.Kind != UnitKind.CombinedUnit)
            {
                if (physicalUnit.SimpleSystem != this.SimpleSystem)
                {
                    IPhysicalQuantity pq_pu = physicalUnit.ConvertTo(this.SimpleSystem);
                    if (pq_pu != null)
                    {
                        IPhysicalQuantity pq = this.Multiply(pq_pu);
                        if (pq.Value.Equals(1.0))
                        {
                            return pq.Unit;
                        }
                    }
                }

                if (physicalUnit.SimpleSystem == this.SimpleSystem)
                {
                    if (physicalUnit.Kind == UnitKind.BaseUnit)
                    {
                        IBaseUnit bu = physicalUnit as IBaseUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Multiply(bu.Exponents));
                    }

                    if (physicalUnit.Kind == UnitKind.DerivedUnit)
                    {
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Multiply(du.Exponents));
                    }

                    /**
                    if (physicalUnit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit cu = physicalUnit as IConvertibleUnit;

                        return ???;
                    }
                    **/

                    //**  Debug.Assert(false, "Kind: " + physicalUnit.Kind.ToString() + " are falling trough");
                }
            }

            return this.CombineMultiply(physicalUnit);
        }

        public override IPhysicalUnit Divide(IPhysicalUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");

            if (physicalUnit.Kind != UnitKind.CombinedUnit)
            {
                if (physicalUnit.SimpleSystem != this.SimpleSystem)
                {
                    IPhysicalQuantity pq_pu = physicalUnit.ConvertTo(this.SimpleSystem);
                    IPhysicalQuantity pq = this.Divide(pq_pu);
                    if (pq.Value.Equals(1.0))
                    {
                        return pq.Unit;
                    }
                }

                if (physicalUnit.SimpleSystem == this.SimpleSystem)
                {
                    if (physicalUnit.Kind == UnitKind.BaseUnit)
                    {
                        IBaseUnit bu = physicalUnit as IBaseUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Divide(bu.Exponents));
                    }

                    if (physicalUnit.Kind == UnitKind.DerivedUnit)
                    {
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Divide(du.Exponents));
                    }

                    /**
                    if (physicalUnit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit cu = physicalUnit as IConvertibleUnit;

                        return ???;
                    }
                    **/

                    //** Debug.Assert(false, "Kind: " +physicalUnit.Kind.ToString() + " are falling trough");
                }
            }

            return this.CombineDivide(physicalUnit);
        }

    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, INamedDerivedUnit // , IPrefixedUnit
    {
        private readonly NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } }

        // IUnitPrefix IPrefixedUnit.Prefix { get { return null; } }

        // INamedSymbolUnit IPrefixedUnit.Unit { get { return this; } }

        public NamedDerivedUnit(UnitSystem someSystem, NamedSymbol someNamedSymbol, SByte[] someExponents = null)
            : base(someSystem, someExponents)
        {
            this.NamedSymbol = someNamedSymbol;
        }

        public NamedDerivedUnit(UnitSystem someSystem, String someName, String someSymbol, SByte[] someExponents = null)
            : this(someSystem, new NamedSymbol(someName, someSymbol), someExponents)
        {
        }

        /// <summary>
        /// String PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            return ReducedUnitString();
        }

        /// <summary>
        /// String formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit, "The 'this.Kind' must be UnitKind.DerivedUnit");

            return Symbol;
        }


        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            return this.ConvertToBaseUnit(1);
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            IUnitSystem system = this.SimpleSystem;
            Debug.Assert(system != null, "The 'System' must be valid for this unit");
            return new PhysicalQuantity(quantity, new DerivedUnit(system, this.Exponents));
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }
    }

    public class ConvertibleUnit : SystemUnit, INamedSymbol, IConvertibleUnit
    {
        private readonly NamedSymbol _NamedSymbol;

        public String Name { get { return this._NamedSymbol.Name; } }
        public String Symbol { get { return this._NamedSymbol.Symbol; } }

        private readonly IPhysicalUnit _primaryunit;
        private readonly IValueConversion _conversion;

        public IPhysicalUnit PrimaryUnit { get { return _primaryunit; } }
        public IValueConversion Conversion { get { return _conversion; } }

        public ConvertibleUnit(NamedSymbol someNamedSymbol, IPhysicalUnit somePrimaryUnit = null, ValueConversion someConversion = null)
            : base(somePrimaryUnit != null ? somePrimaryUnit.SimpleSystem : null)
        {
            this._NamedSymbol = someNamedSymbol;
            _primaryunit = somePrimaryUnit;
            _conversion = someConversion;

            if (this._NamedSymbol == null)
            {
                String name;
                if (someConversion == null || someConversion.LinearOffset == 0)
                {
                    name = this.ConvertToPrimaryUnit().ToPrintString();
                }
                else
                {
                    name = this._primaryunit.ToPrintString();
                    if (someConversion.LinearScale != 1)
                    {
                        name = name + "/" + someConversion.LinearScale;
                    }

                    if (someConversion.LinearOffset >= 0)
                    {
                        name = name + " + " + someConversion.LinearOffset;
                    }
                    else
                    {
                        name = name + " - " + -someConversion.LinearOffset;
                    }
                }
                this._NamedSymbol = new NamedSymbol(name, name);
            }

            Debug.Assert(this._NamedSymbol != null, "The 'someNamedSymbol' must be valid and not null");
        }

        public ConvertibleUnit(String someName, String someSymbol, IPhysicalUnit somePrimaryUnit = null, ValueConversion someConversion = null)
            : this(new NamedSymbol(someName, someSymbol), somePrimaryUnit, someConversion)
        {
        }

        public override UnitKind Kind { get { return UnitKind.ConvertibleUnit; } }

        public override SByte[] Exponents { get { /* return null; */  return PrimaryUnit.Exponents; } }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            return ReducedUnitString();
        }

        /// <summary>
        /// String with formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.ConvertibleUnit, "The 'this.Kind' must be valid and an UnitKind.ConvertibleUnit");

            return this._NamedSymbol.Symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsLinearConvertible()
        {
            Debug.Assert(_conversion != null, "The '_conversion' must be valid and not null");
            return _conversion.LinearOffset == 0;
        }

        public IPhysicalQuantity ConvertFromPrimaryUnit()
        {
            return new PhysicalQuantity(Conversion.ConvertFromPrimaryUnit(), this);
        }

        public IPhysicalQuantity ConvertToPrimaryUnit()
        {
            return new PhysicalQuantity(Conversion.ConvertToPrimaryUnit(), PrimaryUnit);
        }

        public IPhysicalQuantity ConvertFromPrimaryUnit(double quantity)
        {
            return new PhysicalQuantity(Conversion.ConvertFromPrimaryUnit(quantity), this);
        }

        public IPhysicalQuantity ConvertToPrimaryUnit(double quantity)
        {
            IValueConversion temp_conversion = Conversion;
            IPhysicalUnit temp_primaryunit = PrimaryUnit;
            double convertedValue = temp_conversion.ConvertToPrimaryUnit(quantity);
            return new PhysicalQuantity(convertedValue, temp_primaryunit);
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            IPhysicalQuantity pq = this.ConvertToPrimaryUnit(quantity);
            pq = pq.ConvertToSystemUnit();
            return pq;
        }


        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = this.ConvertToPrimaryUnit();
            pq = pq.Unit.ConvertToBaseUnit().Multiply(pq.Value);
            return pq;
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            return this.ConvertToBaseUnit();
        }


        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return PrimaryUnit.ConvertToBaseUnit(new PhysicalQuantity(quantity, this));
        }

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return PrimaryUnit.ConvertToBaseUnit(pq);
        }

        public override IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            if (convertToUnit == this)
            {
                return new PhysicalQuantity(1, this);
            }
            else
            {
                IPhysicalQuantity pq = this.ConvertToPrimaryUnit();
                if (convertToUnit == PrimaryUnit)
                {
                    return pq;
                }
                IPhysicalQuantity pq_toUnit = pq.Unit.ConvertTo(convertToUnit);
                if (pq_toUnit != null)
                {
                    return pq_toUnit.Multiply(pq.Value);
                }
                //// throw new ArgumentException("Physical unit is not convertible to a " + convertToUnit.ToString());
                return null;
            }
        }

        /***
        public override IPhysicalUnit Multiply(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The 'prefix' parameter must be specified");
            // return PrimaryUnit.Multiply(prefix);

            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Multiply(prefix);
            // return pq as IPhysicalUnit;
            //return pq.AsPhysicalUnit();
            return new DerivedUnit(); // pq.Unit.;
        }

        public override IPhysicalUnit Divide(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The 'prefix' parameter must be specified");
            // return PrimaryUnit.Divide(prefix);

            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Divide(prefix);
            //return pq as IPhysicalUnit;
            return pq.AsPhysicalUnit();
        }
        ***/

        //public override PhysicalQuantity Power(SByte exponent)
        public override PhysicalUnit Power(SByte exponent)
        {
            // return new PhysicalQuantity(this.ConvertToSystemUnit().Pow(exponent));

            //return new PhysicalQuantity(this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent));
            //return new PhysicalQuantity(this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent));
            // Abstract class can't be newed return new PhysicalUnit(this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent));

            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }

        //public override PhysicalQuantity Root(SByte exponent)
        public override PhysicalUnit Root(SByte exponent)
        {
            // return new PhysicalQuantity(this.ConvertToSystemUnit().Rot(exponent));
            // return new PhysicalQuantity(this.ConvertToPrimaryUnit().ConvertToSystemUnit().Rot(exponent));
            // Abstract class can't be newed return new PhysicalUnit(this.ConvertToPrimaryUnit().ConvertToSystemUnit().Rot(exponent));
            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Rot(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }
    }

    #region Combined Unit Classes

    public class PrefixedUnit : PhysicalUnit, IPrefixedUnit
    {
        //private readonly SByte _PrefixExponent;
        //private readonly IUnitPrefixExponent _Prefix;
        private readonly IUnitPrefix _Prefix;
        //private readonly IPhysicalUnit _Unit;
        private readonly INamedSymbolUnit _Unit;

        //public SByte PrefixExponent { get { return _PrefixExponent; }  }
        //public IUnitPrefixExponent Prefix { get { return _Prefix; } }
        public IUnitPrefix Prefix { get { return _Prefix; } }
        public INamedSymbolUnit Unit { get { return _Unit; } }


        public override IUnitSystem SimpleSystem { get { return _Unit.SimpleSystem; } set { /* _Unit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem { get { return _Unit.ExponentsSystem; } }

        public override UnitKind Kind { get { return UnitKind.PrefixedUnit; } }

        public override SByte[] Exponents { get { return _Unit.Exponents; } }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            String unitString = Prefix.PrefixChar + Unit.Symbol;
            return unitString;
        }

        /*
        public PrefixedUnit(IPhysicalUnit unit)
            // : this(0, unit)
            : this(null, unit)
        {
        }
        */

        /*
        public PrefixedUnit(SByte PrefixExponent, IPhysicalUnit unit)
        {
            this._PrefixExponent = PrefixExponent;
            this._Unit = unit;
        }
        */

        /*
        public PrefixedUnit(IUnitPrefixExponent prefix, INamedSymbolUnit unit)
        {
            //this._PrefixExponent = PrefixExponent;
            this._Prefix = Prefix;
            this._Unit = unit;
        }
        */


        public PrefixedUnit(IUnitPrefix prefix, INamedSymbolUnit unit)
        {
            //this._PrefixExponent = PrefixExponent;
            this._Prefix = prefix;
            this._Unit = unit;
        }

        /*
        public virtual IPhysicalQuantity AsPhysicalQuantity()
        {
            return AsPhysicalQuantity(1);
        }
        */

        /*
        public virtual IPhysicalQuantity AsPhysicalQuantity(double quantity)
        {
            IPhysicalQuantity pue_pq = new PhysicalQuantity(quantity, Unit);
            if (_PrefixExponent != 0)
            {
                double dd = 1;
                dd *= Math.Pow(10.0, _PrefixExponent);
                pue_pq = pue_pq.Multiply(dd);
            }
            return pue_pq;
        }
        */

        public override IPhysicalQuantity AsPhysicalQuantity(double quantity)
        {
            IPhysicalQuantity pue_pq = new PhysicalQuantity(quantity, this);
            return pue_pq;
        }

        /**
        public virtual IPhysicalQuantity AsPhysicalQuantity(IUnitSystem unitSystem)
        {
            Debug.Assert(unitSystem != null, "The 'unitSystem' must be valid and not null");

            IPhysicalQuantity pue_pq;
            if (_Unit != null)
            {
                pue_pq = _Unit.ConvertTo(unitSystem);
            }
            else
            {
                pue_pq = new PhysicalQuantity(unitSystem.Dimensionless);
            }
            if (pue_pq != null && _Prefix != null && _Prefix.Exponent != 0)
            {
                double dd = 1;
                dd *= Math.Pow(10.0, _Prefix.Exponent);
                pue_pq = pue_pq.Multiply(dd);
            }
            return pue_pq;
        }
        **/

        /*
        public virtual IPhysicalQuantity AsPhysicalQuantity(ref double quantity, IUnitSystem unitSystem)
        {
            // Obsolete
            Debug.Assert(false);

            IPhysicalQuantity pue_pq = this.AsPhysicalQuantity(unitSystem).Multiply(quantity);
            return pue_pq;
        }
        */

        public override bool IsLinearConvertible()
        {
            return _Unit.IsLinearConvertible();
        }

        public static implicit operator PhysicalQuantity(PrefixedUnit prefixedUnit)
        {
            return prefixedUnit.AsPhysicalQuantity() as PhysicalQuantity;
        }

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IPhysicalQuantity pq = _Unit.ConvertToSystemUnit();
            if (pq != null && _Prefix != null && _Prefix.Exponent != 0)
            {
                // pq = pq.Multiply(_Prefix);
                pq = pq.Multiply(_Prefix.Value);
            }
            return pq;
        }


        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = _Unit.ConvertToBaseUnit();
            if (pq != null && _Prefix != null && _Prefix.Exponent != 0)
            {
                // pq = pq.Multiply(_Prefix);
                pq = pq.Multiply(_Prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IPhysicalQuantity pq = _Unit.ConvertToDerivedUnit();
            if (pq != null && _Prefix != null && _Prefix.Exponent != 0)
            {
                // pq = pq.Multiply(_Prefix);
                pq = pq.Multiply(_Prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            IPhysicalQuantity pq = this.ConvertToBaseUnit();
            pq = pq.Multiply(quantity);
            return pq;
        }

        /**
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }
        **/


        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String combinedUnitString = Prefix.PrefixChar + Unit.Symbol;

            return combinedUnitString;
        }

    }

    // public class PrefixedUnitExponent : PrefixedUnit, IPrefixedUnitExponent, IPhysicalUnit
    public class PrefixedUnitExponent : PhysicalUnit, IPrefixedUnitExponent // , IPhysicalUnit
    {
        private readonly IPrefixedUnit _prefixedUnit;
        private readonly SByte _exponent;

        public SByte Exponent { get { return _exponent; } }

        /***
        public PrefixedUnitExponent(IPhysicalUnit Unit)
            : this(new UnitPrefixExponent(0), Unit, 1)
        {
        }

        public PrefixedUnitExponent(IPhysicalUnit Unit, SByte Exponent)
            : this(new UnitPrefixExponent(0), Unit, Exponent)
        {
        }
        ***/

        public PrefixedUnitExponent(INamedSymbolUnit Unit)
            : this(null, Unit, 1)
        {
        }

        public PrefixedUnitExponent(INamedSymbolUnit Unit, SByte Exponent)
            : this(null, Unit, Exponent)
        {
        }


        public PrefixedUnitExponent(IPrefixedUnitExponent prefixedUnitExponent)
            : this(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, prefixedUnitExponent.Exponent)
        {
        }

        public PrefixedUnitExponent(IUnitPrefix prefix, INamedSymbolUnit unit, SByte exponent)
            : this(new PrefixedUnit(prefix, unit), exponent)
        {
        }

        public PrefixedUnitExponent(IPrefixedUnit prefixedUnit, SByte exponent)
        {
            this._prefixedUnit = prefixedUnit;
            this._exponent = exponent;

            Debug.Assert(exponent != 0, "The 'exponent' must be valid and not zero");
        }

        public IUnitPrefix Prefix { get { return _prefixedUnit.Prefix; } }
        public INamedSymbolUnit Unit { get { return _prefixedUnit.Unit; } }


        public override UnitKind Kind { get { return UnitKind.PrefixedUnitExponent; } }


        public override IUnitSystem SimpleSystem { get { return _prefixedUnit.SimpleSystem; } set { /* _Unit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem { get { return _prefixedUnit.ExponentsSystem; } }


        public override SByte[] Exponents {
            get
            {
                SByte[] exponents = _prefixedUnit.Exponents;
                if (_exponent != 1)
                {
                    exponents = exponents.Power(_exponent);
                }
                return exponents;
            }
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            IPhysicalUnit pu = _prefixedUnit;
            if (_exponent != 1)
            {
                pu = pu.Pow(_exponent);
            }
            String unitString = pu.PureUnitString();
            return unitString;
        }

        public override bool IsLinearConvertible()
        {
            return _prefixedUnit.IsLinearConvertible();
        }

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IPhysicalQuantity pq = _prefixedUnit.ConvertToSystemUnit();
            if (_exponent != 1)
            {
                pq = pq.Pow(_exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = _prefixedUnit.ConvertToBaseUnit();
            if (_exponent != 1)
            {
                pq = pq.Pow(_exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IPhysicalQuantity pq = _prefixedUnit.ConvertToDerivedUnit();
            if (_exponent != 1)
            {
                pq = pq.Pow(_exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            IPhysicalQuantity pq = _prefixedUnit.ConvertToBaseUnit();
            if (_exponent != 1)
            {
                pq = pq.Pow(_exponent);
            }
            pq = pq.Multiply(quantity);
            return pq;
        }


        /**
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }
        **/


        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            return this.CombinedUnitString();
        }

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";
            Debug.Assert(_exponent != 0, "The '_Exponent' must be valid and not zero");
            if (_prefixedUnit.Unit != null)
            {
                //string UnitStr = Unit.CombinedUnitString(mayUseSlash, invertExponents);
                string UnitStr = _prefixedUnit.Unit.UnitString();

                if (_prefixedUnit.Prefix != null && _prefixedUnit.Prefix.Exponent != 0)
                {
                    Char PrefixChar = _prefixedUnit.Prefix.PrefixChar;
                    /*
                    if (Physics.UnitPrefixes.GetPrefixCharFromExponent(Prefix.Exponent, out PrefixChar))
                    {
                        UnitStr = PrefixChar + UnitStr;
                    }
                    else
                    {
                        Debug.Assert(false, "GetPrefixCharFromExponent must find a PrefixChar");
                    }
                    */
                    UnitStr = PrefixChar + UnitStr;
                }

                SByte expo = Exponent;
                if (invertExponents)
                {
                    expo = (SByte)(-expo);
                }

                if ((UnitStr.Contains('·') || UnitStr.Contains('/') || UnitStr.Contains('^')) && (expo != 1))
                {
                    Str = "(" + UnitStr + ")";
                }
                else
                {
                    Str = UnitStr;
                }

                if (expo != 1)
                {
                    Str += expo.ToString();
                }

            }
            else
            {
                if (_prefixedUnit.Prefix != null && _prefixedUnit.Prefix.Exponent != 0)
                {
                    SByte expo = Exponent;
                    if (invertExponents)
                    {
                        expo = (SByte)(-expo);
                    }

                    expo = (SByte)(_prefixedUnit.Prefix.Exponent * expo);

                    Str = "10";
                    if (expo != 1)
                    {
                        Str += "^" + expo.ToString();
                    }
                }
            }

            return Str;
        }

        public override IPhysicalQuantity AsPhysicalQuantity()
        {
            IPhysicalQuantity pue_pq = _prefixedUnit.AsPhysicalQuantity();
            if (_exponent != 1)
            {
                pue_pq = pue_pq.Pow(_exponent);
            }
            return pue_pq;
        }

        /*
        public override IPhysicalQuantity AsPhysicalQuantity(IUnitSystem unitSystem)
        {
            Debug.Assert(unitSystem != null, "UnitSystem must be specified");

            IPhysicalQuantity pue_pq = base.AsPhysicalQuantity(unitSystem);
            if (pue_pq != null && _Exponent != 1)
            {
                pue_pq = pue_pq.Pow(_Exponent);
            }
            return pue_pq;
        }
        **/

        /*
        public override IPhysicalQuantity AsPhysicalQuantity(ref double quantity, IUnitSystem unitSystem)
        {
            // Obsolete
            Debug.Assert(false);

            //IPhysicalQuantity pue_pq = base.PhysicalQuantity(ref quantity, unitSystem);
            IPhysicalQuantity pue_pq = this.AsPhysicalQuantity(unitSystem).Multiply(quantity);
            if (_Exponent != 1)
            {
                pue_pq = pue_pq.Pow(_Exponent);
            }
            return pue_pq;
        }
        */

        //
        public IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent, out Double scaleFactor)
        // public IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent)
        {
            SByte CombinedPrefixExponent = 0;
            if (this.Exponent == 1 || outerPUE_PrefixExponent == 0)
            {
                //
                scaleFactor = 1;
                scaleExponent = 0;
                CombinedPrefixExponent = (SByte)(outerPUE_PrefixExponent + this._prefixedUnit.Prefix.Exponent);
            }
            else
            {
                int reminder;
                CombinedPrefixExponent = (SByte)Math.DivRem(outerPUE_PrefixExponent, this.Exponent, out reminder);
                if (reminder != 0)
                {
                    //
                    scaleFactor = Math.Pow(10, 1.0 * reminder);
                    //
                    scaleExponent = (SByte)reminder;
                    //CombinedPrefixExponent = 0;
                }
                else
                {
                    // 
                    scaleFactor = 1;
                    //
                    scaleExponent = 0;
                }
                CombinedPrefixExponent += this._prefixedUnit.Prefix.Exponent;
            }

            IUnitPrefix CombinedUnitPrefix;
            SByte CombinedScaleFactorExponent;

            Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(CombinedPrefixExponent, out CombinedUnitPrefix, out CombinedScaleFactorExponent);


            IUnitPrefixExponent PE = new UnitPrefixExponent((SByte)(CombinedPrefixExponent + this._prefixedUnit.Prefix.Exponent));

            IUnitPrefix UP = null;
            if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(PE, out UP))
            {
                PrefixedUnitExponent CombinedPUE = new PrefixedUnitExponent(UP, this._prefixedUnit.Unit, (SByte)(this.Exponent * outerPUE_Exponent));
                return CombinedPUE;
            }
            else
            {
                // TO DO: Handle to make result as IPrefixedUnitExponent
                Debug.Assert(false);
                return null;
            }
        }

        public static implicit operator PhysicalQuantity(PrefixedUnitExponent prefixedUnitExponent)
        {
            return prefixedUnitExponent.AsPhysicalQuantity() as PhysicalQuantity;
        }
    }

    public class PrefixedUnitExponentList : List<IPrefixedUnitExponent>, IPrefixedUnitExponentList
    {
        public PrefixedUnitExponentList()
        {
        }

        public PrefixedUnitExponentList(IEnumerable<IPrefixedUnitExponent> elements)
            : base(elements)
        {
        }



        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";

            foreach (IPrefixedUnitExponent ue in this)
            {
                Debug.Assert(ue.Exponent != 0, "ue.Exponent must be <> 0");
                if (!String.IsNullOrEmpty(Str))
                {
                    Str += '·';  // center dot '\0x0B7' (Char)183 U+00B7
                }

                Str += ue.CombinedUnitString(mayUseSlash, invertExponents);
            }
            return Str;
        }

        public IPrefixedUnitExponentList Power(SByte exponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
            double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                SByte newExponent = (SByte)(pue.Exponent * exponent);

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (exponent == 1) || (exponent == -1), "Power: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^" + exponent);
                }

                //PrefixedUnitExponent result_pue = new PrefixedUnitExponent(new UnitPrefixExponent(NewPrefixExponent), pue.Unit, NewExponent);
                /*
                UnitPrefix UP = ;
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(UP, pue.Unit, NewExponent);
                */

                IUnitPrefix newUnitPrefix = null;
                if (newPrefixExponent != 0)
                {
                    /*
                    IUnitPrefixExponent PE = new UnitPrefixExponent(NewPrefixExponent);
                    if (!Physics.UnitPrefixes.GetUnitPrefixFromExponent(PE, out tempUnitPrefix))
                    {
                        // TO DO: Handle to make result as PrefixedUnitExponent
                        Debug.Assert(false);
                    }
                    */
                    SByte scaleFactorExponent;

                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(newPrefixExponent, out newUnitPrefix, out scaleFactorExponent);

                    if (scaleFactorExponent != 0)
                    {
                        factorValue *= Math.Pow(10, scaleFactorExponent * newExponent);
                    }
                }
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(newUnitPrefix, pue.Unit, newExponent);

                result.Add(result_pue);
            }
            Debug.Assert(factorValue == 1.0);
            return result;
        }

        public IPrefixedUnitExponentList Root(SByte exponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
            double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                //SByte newExponent = (SByte)(pue.Exponent / exponent);
                int remainder;
                int newExponent = Math.DivRem(pue.Exponent, exponent, out remainder);
                if (remainder != 0)
                {
                    Debug.Assert(remainder == 0);
                    return null;
                }

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (exponent == 1) || (exponent == -1), "Root: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^(1/" + exponent + ")");
                }

                IUnitPrefix newUnitPrefix = null;
                if (newPrefixExponent != 0)
                {
                    SByte scaleFactorExponent;
                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(newPrefixExponent, out newUnitPrefix, out scaleFactorExponent);
                    if (scaleFactorExponent != 0)
                    {
                        factorValue *= Math.Pow(10, scaleFactorExponent * newExponent);
                    }
                }
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(newUnitPrefix, pue.Unit, (SByte)newExponent);
                result.Add(result_pue);
            }
            Debug.Assert(factorValue == 1.0);
            return result;
        }
    }

    public class CombinedUnit : PhysicalUnit, ICombinedUnit
    {
        private Double _scaleFactor = 1;

        private IPrefixedUnitExponentList _Numerators;
        private IPrefixedUnitExponentList _Denominators;

        public IPrefixedUnitExponentList Numerators { get { return _Numerators as IPrefixedUnitExponentList; } }
        public IPrefixedUnitExponentList Denominators { get { return _Denominators as IPrefixedUnitExponentList; } }

        public CombinedUnit()
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
        }

        public CombinedUnit(IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
        {
            if ((someNumerators == null || someNumerators.Count == 0) && (someDenominators != null && someDenominators.Count > 0))
            {
                someNumerators = new PrefixedUnitExponentList(someDenominators.Select(pue => new PrefixedUnitExponent(pue.Prefix, pue.Unit, (sbyte)(-pue.Exponent))));
                someDenominators = null;
            }

            this._Numerators = someNumerators != null ? someNumerators : new PrefixedUnitExponentList();
            this._Denominators = someDenominators != null ? someDenominators : new PrefixedUnitExponentList();
        }

        public CombinedUnit(Double scaleFactor, IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
            : this(someNumerators, someDenominators)
        {
            this._scaleFactor = scaleFactor;
        }

        public CombinedUnit(ICombinedUnit combinedUnit)
            : this(combinedUnit.FactorValue, combinedUnit.Numerators, combinedUnit.Denominators)
        {

        }


        public CombinedUnit(IPrefixedUnitExponent prefixedUnitExponent)
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
            this._Numerators.Add(prefixedUnitExponent);
        }

        public CombinedUnit(IPhysicalUnit physicalUnit)
        //  : this(new PrefixedUnitExponent(new UnitPrefixExponent(0), physicalUnit, 1))
        // : this(new PrefixedUnitExponent(null, physicalUnit, 1))
        {
            ICombinedUnit CU = null;

            INamedSymbolUnit NSU = physicalUnit as INamedSymbolUnit;
            if (NSU != null)
            {
                CU = new CombinedUnit(NSU);
            }
            else
            {
                switch (physicalUnit.Kind)
                {
                    case UnitKind.PrefixedUnit:
                        IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                        Debug.Assert(pu != null);
                        CU = new CombinedUnit(pu);
                        break;
                    case UnitKind.PrefixedUnitExponent:
                        IPrefixedUnitExponent pue = physicalUnit as IPrefixedUnitExponent;
                        Debug.Assert(pue != null);
                        CU = new CombinedUnit(pue);
                        break;
                    case UnitKind.DerivedUnit:
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        Debug.Assert(du != null);
                        CU = new CombinedUnit(du);
                        break;
                    case UnitKind.MixedUnit:
                        IMixedUnit mu = physicalUnit as IMixedUnit;
                        Debug.Assert(mu != null);
                        CU = new CombinedUnit(mu.MainUnit);
                        break;
                    case UnitKind.CombinedUnit:
                        ICombinedUnit cu = physicalUnit as ICombinedUnit;
                        Debug.Assert(cu != null);
                        CU = new CombinedUnit(cu);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            if (CU != null)
            {
                this._scaleFactor = CU.FactorValue;
                this._Numerators = CU.Numerators;
                this._Denominators = CU.Denominators;
            }
            else
            {
                // TO DO: Convert physicalUnit to CombinedUnit
                Debug.Assert(false);
            }
        }

        public CombinedUnit(Double scaleFactor, IPhysicalUnit physicalUnit)
            : this(physicalUnit)
        {
            this._scaleFactor *= scaleFactor;
        }

        public CombinedUnit(INamedSymbolUnit namedSymbolUnit)
        //  : this(new PrefixedUnitExponent(new UnitPrefixExponent(0), namedSymbolUnit, 1))
            : this(new PrefixedUnitExponent(null, namedSymbolUnit, 1))
        {
        }

        public CombinedUnit(IPrefixedUnit prefixedUnit)
            : this(new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, 1))
        {
        }


        public CombinedUnit(IDerivedUnit derivedUnit)
        {
            IPrefixedUnitExponentList numerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList denominators = new PrefixedUnitExponentList();

            IUnitSystem system = derivedUnit.ExponentsSystem;

            int length = derivedUnit.Exponents.Length;
            foreach (Byte i in Enumerable.Range(0, length))
            {
                SByte exp = derivedUnit.Exponents[i];
                if (exp != 0)
                {
                    if (exp > 0)
                    {
                        numerators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], exp));
                    }
                    else
                    {
                        denominators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], (sbyte)(-exp)));
                    }

                }
            }

            this._scaleFactor = derivedUnit.FactorValue;
            this._Numerators = numerators;
            this._Denominators = denominators;
        }

        public override UnitKind Kind { get { return UnitKind.CombinedUnit; } }

        public override IUnitSystem SimpleSystem
        {
            get
            {
                IUnitSystem system = null; // No unit system
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit == null)
                    {
                        // This could be at prefix only
                        // Just ignore this pue; it can't affect unit system info
                        // return null;
                    }
                    else
                    {
                        IUnitSystem subsystem = pue.Unit.SimpleSystem;

                        if (system == null)
                        {
                            system = subsystem;
                        }
                        else
                        {
                            if (system != subsystem)
                            {
                                // Multiple unit systems
                                return null;
                            }
                        }
                    }
                }

                return system;  // The one and only system for all sub units
            }
            set {  /* Just do nothing */ throw new NotImplementedException(); }
        }

        public override IUnitSystem ExponentsSystem
        {
            get
            {
                IUnitSystem system = null; // No unit system
                List<IUnitSystem> SubUnitSystems = null;
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit != null)
                    {
                        if (system == null)
                        {
                            system = pue.Unit.ExponentsSystem;
                        }
                        else
                        {
                            IUnitSystem pue_system = pue.Unit.ExponentsSystem; // pue.Unit.SomeSimpleSystem;
                            if (system != pue_system
                                && ((!system.IsCombinedUnitSystem && !pue_system.IsCombinedUnitSystem)
                                    || (system.IsCombinedUnitSystem
                                        && !pue_system.IsCombinedUnitSystem
                                        && !((CombinedUnitSystem)system).ContainsSubUnitSystem(pue_system))
                                    /*
                                        We must still include pue_system sub unit systems
                                        || (   !system.IsCombinedUnitSystem
                                            && pue_system.IsCombinedUnitSystem
                                            && !((CombinedUnitSystem)pue_system).ContainsSubUnitSystem(system))
                                    */
                                    || (system.IsCombinedUnitSystem
                                        && pue_system.IsCombinedUnitSystem
                                        && !((CombinedUnitSystem)system).ContainsSubUnitSystems(((CombinedUnitSystem)pue_system).UnitSystemes))
                                   )
                                )
                            {
                                // Multiple unit systems and some could be an isolated unit system 
                                if (SubUnitSystems == null)
                                {   // First time we have found a second system. Add system as first system in list of systems
                                    SubUnitSystems = new List<IUnitSystem>();
                                    if (!system.IsCombinedUnitSystem)
                                    {
                                        SubUnitSystems.Add(system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)system;
                                        SubUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }

                                {   // Add pue_system to list of systems
                                    if (!pue_system.IsCombinedUnitSystem)
                                    {
                                        SubUnitSystems.Add(pue_system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)pue_system;
                                        SubUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }
                            }
                        }
                    }
                }

                if (SubUnitSystems != null)
                {
                    if (SubUnitSystems.Any(us => us.IsIsolatedUnitSystem))
                    {   // Must combine the unit systems into one unit system
                        system = CombinedUnitSystem.GetCombinedUnitSystem(SubUnitSystems.Distinct().ToArray());
                    }
                    else
                    {
                        IUnitSystem DefaultUnitSystem = Physics.CurrentUnitSystems.Default;
                        if (SubUnitSystems.Contains(DefaultUnitSystem))
                        {
                            system = DefaultUnitSystem;
                        }
                        else
                        {
                            // system = SubUnitSystems.First(us => !us.IsIsolatedUnitSystem);
                            system = SubUnitSystems.First();
                        }
                    }
                }
                if (system == null)
                {
                    system = Physics.CurrentUnitSystems.Default;
                }

                Debug.Assert(system != null, "CombinedUnit.ExponentsSystem is null");
                return system;
            }
        }


        public IUnitSystem SomeSimpleSystem
        {
            get
            {
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit != null)
                    {
                        IPhysicalUnit pu = pue.Unit;
                        IUnitSystem somesystem = pu.SimpleSystem;
                        if (somesystem != null)
                        {
                            return somesystem;
                        }
                        else if (pu.Kind == UnitKind.CombinedUnit)
                        {
                            ICombinedUnit cu = (ICombinedUnit)pu;
                            somesystem = cu.SomeSimpleSystem;
                            if (somesystem != null)
                            {
                                return somesystem;
                            }
                        }
                    }

                }

                return null;
            }
        }

        public override SByte[] Exponents
        {
            get
            {
                SByte[] exponents = null;
                int elementCount = Numerators.Count + Denominators.Count;
                if (elementCount == 0)
                {   // No exponents at all; return array of zeros
                    exponents = new SByte[1];
                    exponents[0] = 0;
                }
                else
                {
                    IUnitSystem anySystem = this.ExponentsSystem;
                    if (anySystem == null)
                    {
#if DEBUG // Error traces only included in debug build
                        Debug.WriteLine("CombinedUnit.Exponents() missing ExponentsSystem");
                        Debug.Assert(false, "CombinedUnit.Exponents() missing ExponentsSystem");
#endif
                        anySystem = this.SomeSimpleSystem;
                        if (anySystem == null)
                        {
#if DEBUG // Error traces only included in debug build
                            Debug.WriteLine("CombinedUnit.Exponents() missing also SomeSystem");
                            Debug.Assert(false, "CombinedUnit.Exponents() missing also SomeSystem");
#endif
                            anySystem = Physics.SI_Units;
                        }
                    }
                    if (anySystem != null)
                    {
                        IPhysicalQuantity baseUnit_pq = null;
                        IPhysicalUnit baseUnit_pu = null;

                        try
                        {
                            // exponents = this.ConvertToBaseUnit().Unit.Exponents;

                            baseUnit_pq = this.ConvertToDerivedUnit();
                            if (baseUnit_pq != null)
                            {
                                baseUnit_pu = baseUnit_pq.Unit;
                                if (baseUnit_pu != null)
                                {
                                    IUnitSystem system = this.ExponentsSystem;
                                    Debug.Assert(system != null);

                                    if (system.IsCombinedUnitSystem)
                                    {
                                        ICombinedUnitSystem cus = system as ICombinedUnitSystem;

                                        exponents = cus.UnitExponents(this);
                                    }
                                    else
                                    {
                                        UnitKind uk = baseUnit_pu.Kind;
                                        exponents = baseUnit_pu.Exponents;
                                    }
                                }
                            }
#if DEBUG // Error traces only included in debug build
                            if (exponents == null)
                            {
                                baseUnit_pq = this.ConvertToDerivedUnit(); // Just for extra change to debug

                                Debug.WriteLine("CombinedUnit.ConvertToDerivedUnit() are missing base unit and exponents");
                                Debug.Assert(false, "CombinedUnit.ConvertToDerivedUnit() are missing base unit and exponents");
                            }
#endif
                        }
                        catch
                        {
#if DEBUG // Error traces only included in debug build
                            baseUnit_pq = this.ConvertToDerivedUnit(); // Just for extra change to debug

                            Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents");
                            Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents");
#endif
                        }
                    }
                    else
                    {
#if DEBUG // Error traces only included in debug build
                        Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() missing exponents");
                        Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() missing exponents");
#endif
                    }
                }
                return exponents;
            }
        }

        public ICombinedUnit OnlySingleSystemUnits(IUnitSystem us)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    TempNumerators.Add(pue);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    TempDenominators.Add(pue);
                }
            }
            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }


        /*
        public abstract Double Value { get; }
        //public abstract IPhysicalUnit PureUnit { get; }
        public abstract IUnit PureUnit { get; }
        */
        public override double FactorValue
        {
            get
            {
                /*
                double value = 1; 
                if (Numerators.Count >= 1 && Numerators[0].Unit == null) 
                {
                    Debug.Assert(Numerators[0].Exponent == 1);

                    value = Math.Pow(10, Numerators[0].Prefix.Exponent);
                }
                return value; 
                */
                return _scaleFactor;
            }
        }
        public override IPhysicalUnit PureUnit
        {
            get
            {
                /**
                IPhysicalUnit pureunit = this;
                if (Numerators.Count >= 1 && Numerators[0].Unit == null)
                {
                    Debug.Assert(Numerators[0].Exponent == 1);

                    IPrefixedUnitExponentList tempNumerators = Numerators;
                    tempNumerators.RemoveAt(0);
                    pureunit = new CombinedUnit(tempNumerators, Denominators);
                }
                return pureunit; 
                **/
                IPhysicalUnit pureunit = this;
                if (_scaleFactor != 0)
                {
                    pureunit = new CombinedUnit(Numerators, Denominators);
                }
                return pureunit;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsLinearConvertible()
        {
            if (Numerators.Count == 1 && Denominators.Count == 0)
            {
                IPrefixedUnitExponent pue = Numerators[0];
                if (pue.Exponent == 1)
                {
                    IPhysicalUnit unit = pue.Unit;
                    if (unit != null)
                    {
                        return unit.IsLinearConvertible();
                    }
                }
            }
            return true;
        }

        // Relative conversion

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = this.SomeSimpleSystem;
                if (system == null)
                {
                    system = this.ExponentsSystem;
                }
                Debug.Assert(system != null);
                IPhysicalQuantity pq = this.ConvertTo(system);
                if (pq == null)
                {
                    //Debug.Assert(pq == null || pq.Unit.SimpleSystem == system);
                    //Debug.Assert(pq != null);
                }
                return pq;
            }
            return new PhysicalQuantity(1, this);
        }

        // Absolute conversion
        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = Physics.CurrentUnitSystems.Default;
            }
            Debug.Assert(system != null);
            IPhysicalQuantity pq = this.ConvertTo(ref quantity, system);
            Debug.Assert(pq == null || pq.Unit.SimpleSystem != null);
            return pq;
        }


        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            /****
            IUnitSystem system = this.ExponentsSystem;
            ****/

            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = this.ExponentsSystem;
                Debug.Assert(system != null);

                if (system.IsCombinedUnitSystem)
                {
                    ICombinedUnitSystem cus = system as ICombinedUnitSystem;

                    return cus.ConvertToBaseUnit(this);
                }

                // This happens for combined unit with sub units of different but convertible systems Debug.Assert(false);

                // Combined unit with sub units of of different but convertible systems 
                IPhysicalQuantity pq = this.ConvertToSystemUnit();

                if (pq != null)
                {
                    pq = pq.ConvertToBaseUnit();
                }
                return pq;
            }

            Debug.Assert(system != null);

            // double value = 1;
            double value = _scaleFactor;
            IPhysicalUnit unit = null;

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity();
                IPhysicalUnit pue_pq_Unit = pue_pq.Unit;
                if (pue_pq_Unit != null)
                {
                    IPhysicalQuantity pq_baseunit = pue_pq_Unit.ConvertToBaseUnit();
                    IPhysicalUnit baseunit = (IPhysicalUnit)(pq_baseunit.Unit);

                    value *= pue_pq.Value * pq_baseunit.Value;

                    if (unit == null)
                    {
                        unit = baseunit;
                    }
                    else
                    {
                        /*
                        IPhysicalQuantity pq = unit.Multiply(baseunit);
                        if (PhysicalQuantity.IsPureUnit(pq))
                        {
                            unit = PhysicalQuantity.PureUnit(pq);
                        }
                        else
                        {
                            value *= pq.Value;
                            unit = pq.Unit;
                        }
                        */

                        unit = unit.Multiply(baseunit);
                    }
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity();
                IPhysicalUnit pue_pq_Unit = pue_pq.Unit;
                if (pue_pq_Unit != null)
                {
                    IPhysicalQuantity pq_baseunit = pue_pq_Unit.ConvertToBaseUnit();
                    IPhysicalUnit baseunit = (IPhysicalUnit)(pq_baseunit.Unit);

                    value /= pue_pq.Value * pq_baseunit.Value;

                    if (unit == null)
                    {
                        unit = baseunit.CombinePow(-1);
                    }
                    else
                    {
                        /*
                        IPhysicalQuantity pq = unit.Divide(baseunit);
                        if (PhysicalQuantity.IsPureUnit(pq))
                        {
                            unit = PhysicalQuantity.PureUnit(pq);
                        }
                        else
                        {
                            value *= pq.Value;
                            unit = pq.Unit;
                        }
                        */
                        unit = unit.Divide(baseunit);
                    }
                }
            }

            return new PhysicalQuantity(value, unit);

        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                Double ScaledQuantity = _scaleFactor * quantity;
                IPhysicalQuantity pq1 = this.ConvertToSystemUnit(ref ScaledQuantity).ConvertToBaseUnit();
                Debug.Assert(ScaledQuantity == 1.0);
                return pq1;
            }
            Debug.Assert(system != null);

            IPhysicalQuantity pq = new PhysicalQuantity(quantity);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity().ConvertToBaseUnit();

                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity().ConvertToBaseUnit();

                pq = pq.Divide(pue_pq);
            }

            return pq;
        }

        /*
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            return physicalQuantity.ConvertTo(this).ConvertToBaseUnit();
        }
        */


        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                Double ScaledQuantity = _scaleFactor;
                // IPhysicalQuantity pq1 = this.ConvertToSystemUnit(ref ScaledQuantity);  
                IPhysicalQuantity pq1 = this.ConvertToSystemUnit();// _scaleFactor is part of this, and should be handled by this.ConvertToSystemUnit()

                if (pq1 != null)
                {
                    // Simple system DerivedUnit
                    Debug.Assert(ScaledQuantity == 1.0);
                    Debug.Assert(pq1.Unit.SimpleSystem != null);
                }
                else
                {
                    // Combined system DerivedUnit
                    system = this.ExponentsSystem;
                    Debug.Assert(system != null && system.IsCombinedUnitSystem);

                    // pq1 = this.ConvertToBaseUnit().Multiply(_scaleFactor);
                    pq1 = this.ConvertToBaseUnit();  // _scaleFactor is part of this, and should be handled by this.ConvertToBaseUnit()
                }

                pq1 = pq1.ConvertToDerivedUnit();
                return pq1;
            }

            Debug.Assert(system != null);

            IPhysicalQuantity pq = new PhysicalQuantity(_scaleFactor, system.Dimensionless);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.ConvertToDerivedUnit();
                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                IPhysicalQuantity pue_pq = pue.ConvertToDerivedUnit();
                pq = pq.Divide(pue_pq);
            }

            return pq;
        }


        public override IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IUnitSystem system = this.SimpleSystem;
            IUnitSystem convertToSystem = convertToUnit.SimpleSystem;
            if (system == null || system != convertToSystem)
            {
                if (convertToSystem == null)
                {
                    if (convertToUnit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit cu = convertToUnit as ICombinedUnit;
                        convertToSystem = cu.ExponentsSystem;
                    }
                }

                if (convertToSystem != null)
                {
                    IPhysicalQuantity this_as_ToSystemUnit = this.ConvertTo(convertToSystem);
                    if (this_as_ToSystemUnit != null)
                    {
                        if (this_as_ToSystemUnit.Unit != null
                            && this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem
                            && (!convertToSystem.IsCombinedUnitSystem || !((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)))
                        {
                            Debug.Assert(this_as_ToSystemUnit.Unit == null || this_as_ToSystemUnit.Unit.ExponentsSystem == convertToSystem || (convertToSystem.IsCombinedUnitSystem && ((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)), "PRE this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem");
                        }
                        Debug.Assert(this_as_ToSystemUnit.Unit == null || this_as_ToSystemUnit.Unit.ExponentsSystem == convertToSystem || (convertToSystem.IsCombinedUnitSystem && ((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)), "this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem");
                        return this_as_ToSystemUnit.ConvertTo(convertToUnit);
                    }
                    return null;
                }
                Debug.Assert(false);
                return null;
            }
            Debug.Assert(system != null && system == convertToSystem);
            /*
            IPhysicalQuantity pq_baseunit = this.ConvertToBaseUnit();
            Debug.Assert(pq_baseunit.Unit != this); // Some reduction must be performed; else infinite recursive calls can occur
            IPhysicalQuantity pq_tounit = pq_baseunit.Unit.ConvertTo(convertToUnit);
            */
            IPhysicalQuantity pq_tounit = null;
            IPhysicalQuantity pq_baseunit = null;

            if (convertToUnit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit convertToUnit_cu = convertToUnit as ICombinedUnit;
                ICombinedUnit relativeUnit_cu = this.CombineDivide(convertToUnit_cu);
                IPhysicalQuantity pq = relativeUnit_cu.ConvertToDerivedUnit();
                if (pq.IsDimensionless)
                {
                    return new PhysicalQuantity(pq.Value, convertToUnit);
                }
                return null;
            }
            else
            {
                pq_baseunit = this.ConvertToDerivedUnit();
                Debug.Assert(pq_baseunit.Unit != this); // Some reduction must be performed; else infinite recursive calls can occur
                pq_tounit = pq_baseunit.Unit.ConvertTo(convertToUnit);
            }

            if (pq_tounit != null)
            {
                // Valid conversion
                return new PhysicalQuantity(pq_baseunit.Value * pq_tounit.Value, pq_tounit.Unit);
            }
            // Invalid conversion
            return null;
        }

        public override IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            double value = this.FactorValue;
            IPhysicalUnit unit = null;

            Debug.Assert(convertToUnitSystem != null);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                // IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity(convertToUnitSystem);
                IPhysicalQuantity pue_pq = pue.ConvertTo(convertToUnitSystem);
                if (pue_pq == null)
                {
                    return null;
                }

                value *= pue_pq.Value;

                if (unit == null)
                {
                    unit = pue_pq.Unit;
                }
                else
                {
                    var e = pue_pq.Unit.Exponents;
                    unit = unit.CombineMultiply(pue_pq.Unit);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                // IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity(convertToUnitSystem);
                IPhysicalQuantity pue_pq = pue.ConvertTo(convertToUnitSystem);
                if (pue_pq == null)
                {
                    return null;
                }

                value /= pue_pq.Value;

                if (unit == null)
                {
                    unit = pue_pq.Unit.CombinePow(-1);
                }
                else
                {
                    unit = unit.CombineDivide(pue_pq.Unit);
                }
            }

            if (unit == null)
            { // dimension less unit
                unit = convertToUnitSystem.Dimensionless;
            }
            return new PhysicalQuantity(value, unit);
        }

        public override IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem)
        {
            IPhysicalQuantity pq = this.ConvertTo(convertToUnitSystem);
            if (pq!= null)
            {
                pq = pq.Multiply(quantity);
            }
            return pq;
        }

        public IPhysicalQuantity ConvertFrom(IPhysicalQuantity physicalQuantity)
        {

            IPhysicalQuantity pq_unit = physicalQuantity;
            //////////////////////
            if (Numerators.Count == 1 && Denominators.Count == 0)
            {
                IPrefixedUnitExponent pue = Numerators[0];

                Debug.Assert(pue.Exponent == 1);

                pq_unit = pq_unit.ConvertTo(pue.Unit);

                Debug.Assert(pq_unit != null);

                if (pq_unit != null)
                {
                    if (pue.Prefix.Exponent != 0)
                    {
                        pq_unit = pq_unit.Multiply(Math.Pow(10, -pue.Prefix.Exponent));
                    }

                    pq_unit = new PhysicalQuantity(pq_unit.Value, this);
                }
            }
            else
            {
                // Not implemented yet
                Debug.Assert(false);
            }
            ///////////////////////

            return pq_unit;
        }

        #region IPhysicalUnitMath Members

        public override IPhysicalUnit Dimensionless { get { return new CombinedUnit(); } }

        public override Boolean IsDimensionless
        {
            get
            {
                if (Numerators.Count == 0 && Denominators.Count == 0)
                {
                    return true;
                }
                return base.IsDimensionless;
            }
        }


        public override IPhysicalUnit Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            if (prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit cue = ((ICombinedUnit)(prefixedUnitExponent.Unit));
                if (prefixedUnitExponent.Prefix.Exponent != 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, null, 1);
                    cue = cue.CombineMultiply(temp_pue);
                }
                cue = cue.CombinePow(prefixedUnitExponent.Exponent);
                ICombinedUnit unit = this.CombineMultiply(cue);
                //return new PhysicalQuantity(1, unit);
                return unit;
            }

            SByte multExponent = prefixedUnitExponent.Exponent;
            SByte prefixExponent = prefixedUnitExponent.Prefix.Exponent;
            SByte prefixedUnitExponentScaleing = 1;

            /*
            IPhysicalQuantity pq = null;
            
            if (prefixedUnitExponent.Unit.IsDimensionless && prefixedUnitExponent.Unit.IsDimensionless == 1 ) // == One )
            {
                Debug.Assert(prefixExponent == 0);
                Debug.Assert(multExponent == 1);
                pq = new PhysicalQuantity(1, this);
            }
            else
            */
            {
                PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
                PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

                Boolean PrimaryUnitFound = false;
                Boolean ChangedExponentSign = false;
                //// Check if pue2.Unit is already among our Numerators or Denominators
                foreach (IPrefixedUnitExponent ue in Denominators)
                {
                    // if (!PrimaryUnitFound && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                    if (!PrimaryUnitFound && prefixedUnitExponent.Unit.Equals(ue.Unit))
                    {
                        PrimaryUnitFound = true;

                        if (!prefixExponent.Equals(ue.Prefix.Exponent))
                        {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                            prefixedUnitExponentScaleing = (SByte)((ue.Prefix.Exponent - prefixExponent) * multExponent);
                            prefixExponent = ue.Prefix.Exponent;
                        }

                        // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                        SByte NewExponent = (SByte)(ue.Exponent - multExponent);
                        if (NewExponent > 0)
                        {
                            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                            TempDenominators.Add(temp_pue);
                            // Done
                        }
                        else
                        {   // Convert to Numerator
                            multExponent = (SByte)(-NewExponent);
                            ChangedExponentSign = true;
                        }
                    }
                    else
                    {
                        TempDenominators.Add(ue);
                    }
                }

                foreach (IPrefixedUnitExponent ue in Numerators)
                {
                    // if (!PrimaryUnitFound && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                    if (!PrimaryUnitFound && prefixedUnitExponent.Unit.Equals(ue.Unit))
                    {
                        PrimaryUnitFound = true;

                        if (!prefixExponent.Equals(ue.Prefix.Exponent))
                        {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                            prefixedUnitExponentScaleing = (SByte)((ue.Prefix.Exponent - prefixExponent) * multExponent);
                            prefixExponent = ue.Prefix.Exponent;
                        }

                        // Add the found CombinedUnit exponent with ue2´s exponent; 
                        SByte NewExponent = (SByte)(ue.Exponent + multExponent);

                        if (NewExponent > 0)
                        {
                            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                            TempNumerators.Add(temp_pue);
                            // Done
                        }
                        else
                        {   // Convert to Denominator
                            multExponent = (SByte)NewExponent;
                            ChangedExponentSign = true;
                        }
                    }
                    else
                    {
                        TempNumerators.Add(ue);
                    }
                }

                if (!PrimaryUnitFound || ChangedExponentSign)
                {   // pue2.Unit is not among our Numerators or Denominators (or has changed from Numerators to Denominators)
                    if (!PrimaryUnitFound && (prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit))
                    {
                        // TO DO: Can this happend? The case of prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit should already have been handled  
                        Debug.Assert(false);

                        //IPhysicalQuantity result_pq = new PhysicalQuantity(1, this);
                        // CombinedUnit cu1 = new CombinedUnit(new PrefixedUnitExponentList(this.Numerators), new PrefixedUnitExponentList(this.Denominators));
                        ICombinedUnit cu1 = new CombinedUnit(TempNumerators, TempDenominators);
                        ICombinedUnit cu2 = prefixedUnitExponent.Unit as ICombinedUnit;

                        Double PrefixScale = 1;
                        SByte PrefixExponent = 0;
                        foreach (IPrefixedUnitExponent pue2Num_pue in cu2.Numerators)
                        {
                            /*
                            Double PrefixScale;
                            IPrefixedUnitExponent CombinedPUE = pue2Num_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out PrefixScale);

                            result_pq = result_pq.Multiply(CombinedPUE);
                            if (PrefixScale != 1)
                            {
                                result_pq = result_pq.Multiply(PrefixScale);
                            }
                            */
                            /*
                            SByte TempPrefixExponent = 0;
                            IPrefixedUnitExponent CombinedPUE = pue2Num_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out TempPrefixExponent);
                            cu1 = cu1.CombinedUnitMultiply(CombinedPUE);
                            if (TempPrefixExponent != 0)
                            {
                                PrefixExponent = (SByte)(PrefixExponent + TempPrefixExponent);
                            }
                            */
                            Double TempPrefixScale;
                            SByte TempPrefixExponent;
                            IPrefixedUnitExponent CombinedPUE = pue2Num_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out TempPrefixExponent, out TempPrefixScale);

                            cu1 = cu1.CombineMultiply(CombinedPUE);
                            PrefixExponent += TempPrefixExponent;
                            if (PrefixScale != 1)
                            {
                                PrefixScale *= TempPrefixScale;
                            }
                        }
                        foreach (IPrefixedUnitExponent pue2DOM_pue in cu2.Denominators)
                        {
                            /*
                            Double PrefixScale;
                            IPrefixedUnitExponent CombinedPUE = pue2DOM_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out PrefixScale);

                            result_pq = result_pq.Divide(CombinedPUE);
                            if (PrefixScale != 1)
                            {
                                result_pq = result_pq.Divide(PrefixScale);
                            }
                            */
                            /*
                            cu1 = cu1.CombinedUnitDivide(CombinedPUE);
                            if (PrefixScale != 1)
                            {
                                cu1 = cu1.Divide(PrefixScale);
                            }*/

                            Double TempPrefixScale;
                            SByte TempPrefixExponent;
                            IPrefixedUnitExponent CombinedPUE = pue2DOM_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out TempPrefixExponent, out TempPrefixScale);

                            cu1 = cu1.CombineMultiply(CombinedPUE);
                            PrefixExponent -= TempPrefixExponent;
                            if (PrefixScale != 1)
                            {
                                PrefixScale *= TempPrefixScale;
                            }
                        }
                        Double Exponent_d = Math.Log10(PrefixScale);
                        SByte Exp = (SByte)Exponent_d;
                        Double Diff = Exponent_d - Exp;
                        Debug.Assert(Diff == 0);

                        //PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(new UnitPrefixExponent(Exp), null, 1);
                        PrefixedUnitExponent temp_pue = null; ;
                        IUnitPrefixExponent PE = new UnitPrefixExponent(Exp);
                        IUnitPrefix UP = null;
                        if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(PE, out UP))
                        {
                            temp_pue = new PrefixedUnitExponent(UP, null, 1);
                        }
                        else
                        {
                            // TO DO: Handle to make result as PrefixedUnitExponent
                            Debug.Assert(false);
                            temp_pue = null;
                        }
                        cu1 = cu1.CombineMultiply(temp_pue);

                        // return result_pq;
                        return cu1;
                    }
                    else
                    {
                        if (multExponent > 0)
                        {
                            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                            TempNumerators.Add(temp_pue);
                        }
                        else if (multExponent < 0)
                        {
                            multExponent = (SByte)(-multExponent);
                            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                            TempDenominators.Add(temp_pue);
                        }
                    }
                }

                CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
                // pq = new PhysicalQuantity(1, cu);
                return cu;
            }
            // return pq;
        }

        public override IPhysicalUnit Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte NewExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, NewExponent);
            //return this.Multiply(temp_pue);
            return temp_pue;
        }

        /**
        public override IPhysicalQuantity Multiply(INamedSymbolUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);
            return this.Multiply(pue);
        }

        public override IPhysicalQuantity Divide(INamedSymbolUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);

            return this.Divide(pue);
        }

        public override IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);
            return this.Multiply(pue);
        }

        public override IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            PrefixedUnitExponent pue = new PrefixedUnitExponent(null, physicalUnit, 1);

            return this.Divide(pue);
        }
        **/

        public override IPhysicalQuantity Multiply(double quantity)
        {
            return this * quantity;
        }

        public override IPhysicalQuantity Divide(double quantity)
        {
            return this / quantity;
        }

        //
        public override PhysicalUnit Power(SByte exponent)
        {
            CombinedUnit cu = new CombinedUnit(Numerators.Power(exponent), Denominators.Power(exponent));
            //PhysicalQuantity pq = new PhysicalQuantity(1, cu);
            //return pq;
            return cu;
        }

        //
        public override PhysicalUnit Root(SByte exponent)
        {
            IPrefixedUnitExponentList TempNumerators;
            IPrefixedUnitExponentList TempDenominators = null;
            TempNumerators = Numerators.Root(exponent);
            if (TempNumerators != null)
            {
                TempDenominators = Denominators.Root(exponent);
            }

            if ((TempNumerators != null) && (TempDenominators != null))
            {
                CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
                /*
                PhysicalQuantity pq = new PhysicalQuantity(1, cu);
                return pq;
                */
                return cu;
            }
            else
            {
                SByte[] NewExponents = this.Exponents;
                if (NewExponents != null)
                {
                    NewExponents = NewExponents.Root(exponent);
                    Debug.Assert(this.ExponentsSystem != null);
                    DerivedUnit du = new DerivedUnit(this.ExponentsSystem, NewExponents);
                    /*
                    PhysicalQuantity pq = new PhysicalQuantity(1, du);
                    return pq;
                    */
                    return du;
                }
                else
                {
                    Debug.Assert(NewExponents != null);
                    //if (ThrowExceptionOnUnitMathError) {
                    throw new PhysicalUnitMathException("The result of the math operation on the PhysicalUnit argument can't be represented by this implementation of PhysicalMeasure: (" + this.ToPrintString() + ").Root(" + exponent.ToString() + ")");
                    //}
                    //return null;
                }
            }
        }

        #endregion IPhysicalUnitMath Members

        #region Combine IPhysicalUnitMath Members


        public override ICombinedUnit CombineMultiply(double quantity)
        {
            double factor = this.FactorValue * quantity;
            ICombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }

        public override ICombinedUnit CombineDivide(double quantity)
        {
            double factor = this.FactorValue / quantity;
            ICombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }
        public override ICombinedUnit CombineMultiply(IUnitPrefixExponent prefixExponent)
        {
            return this.CombineMultiply(prefixExponent.Value);
        }

        public override ICombinedUnit CombineDivide(IUnitPrefixExponent prefixExponent)
        {
            return this.CombineDivide(prefixExponent.Value);
        }


        public override ICombinedUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            /** * /
            return this.CombinedUnitMultiply(prefixedUnitExponent);
        }

        public ICombinedUnit CombinedUnitMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            / * **/
            Debug.Assert(prefixedUnitExponent != null);

            /*
             Not valid. Even if  unit is dimension less; the factor can differ from 1
            if (this.IsDimensionless) 
            {
                CombinedUnit cu2 = new CombinedUnit(prefixedUnitExponent);
                return cu2;
            }
            */

            if (prefixedUnitExponent.Unit != null && prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit cue = ((ICombinedUnit)(prefixedUnitExponent.Unit));
                if (prefixedUnitExponent.Prefix.Exponent != 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, null, 1);
                    cue = cue.CombineMultiply(temp_pue);
                }
                cue = cue.CombinePow(prefixedUnitExponent.Exponent);
                ICombinedUnit unit = this.CombineMultiply(cue);
                return unit;
            }


            SByte multPrefixExponent = 0; // 10^0 = 1
            SByte multExponent = 1;

            SByte scalingPrefixExponent = 0;  // 10^0 = 1
            SByte scalingExponent = 1;


            int pue_prefixExp = 0; // 10^0 = 1
            if (prefixedUnitExponent.Prefix != null)
            {
                pue_prefixExp = prefixedUnitExponent.Prefix.Exponent;
            }

            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            Boolean PrimaryUnitFound = false;
            Boolean ChangedExponentSign = false;

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                //if (!Found && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                if (!PrimaryUnitFound && prefixedUnitExponent.Unit != null && ue.Unit != null && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    PrimaryUnitFound = true;

                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    SByte NewExponent = (SByte)(ue.Exponent - prefixedUnitExponent.Exponent);

                    int ue_prefixExp = 0; // 10^0 = 1
                    if (ue.Prefix != null)
                    {
                        ue_prefixExp = ue.Prefix.Exponent;
                    }

                    if (!pue_prefixExp.Equals(ue_prefixExp))
                    {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                        scalingPrefixExponent = (SByte)(pue_prefixExp - ue_prefixExp);
                        scalingExponent = prefixedUnitExponent.Exponent;
                    }

                    if (NewExponent > 0)
                    {   // Still some exponent left for a denominator element
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                        TempDenominators.Add(temp_pue);
                        // Done
                    }
                    else
                    if (NewExponent < 0)
                    {   // Convert to Numerator
                        multPrefixExponent = ue.Prefix.Exponent;
                        multExponent = (SByte)(-NewExponent);
                        ChangedExponentSign = true;
                    }
                }
                else
                {
                    if (ue.Exponent > 0)
                    {
                        TempDenominators.Add(ue);
                    }
                    else
                    {
                        TempNumerators.Add(new PrefixedUnitExponent(ue.Prefix, ue.Unit, (SByte)(-ue.Exponent)));
                    }
                }
            }

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                //if (!Found && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                if (!PrimaryUnitFound && prefixedUnitExponent.Unit != null && ue.Unit != null && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    PrimaryUnitFound = true;

                    SByte ue_prefixExp = 0; // 10^0 = 1
                    if (ue.Prefix != null)
                    {
                        ue_prefixExp = ue.Prefix.Exponent;
                    }

                    if (!pue_prefixExp.Equals(ue_prefixExp))
                    {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                        scalingPrefixExponent = (SByte)(pue_prefixExp - ue_prefixExp);
                        scalingExponent = prefixedUnitExponent.Exponent;
                    }

                    // Add the found CombinedUnit exponent with ue2´s exponent; 
                    SByte NewExponent = (SByte)(ue.Exponent + prefixedUnitExponent.Exponent);
                    if (NewExponent > 0)
                    {
                        // Still some exponent left for a numerator element
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                        TempNumerators.Add(temp_pue);
                        // Done
                    }
                    else
                    if (NewExponent < 0)
                    {   // Convert to Denominator
                        multPrefixExponent = ue.Prefix.Exponent;
                        multExponent = NewExponent;
                        ChangedExponentSign = true;
                    }
                }
                else
                {
                    if (ue.Exponent > 0)
                    {
                        TempNumerators.Add(ue);
                    }
                    else
                    {
                        TempDenominators.Add(new PrefixedUnitExponent(ue.Prefix, ue.Unit, (SByte)(-ue.Exponent)));
                    }
                }
            }

            if (!PrimaryUnitFound || ChangedExponentSign)
            {
                if (!PrimaryUnitFound)
                {
                    if (prefixedUnitExponent.Prefix != null)
                    {
                        multPrefixExponent = prefixedUnitExponent.Prefix.Exponent;
                    }
                    multExponent = prefixedUnitExponent.Exponent;
                }

                IUnitPrefix unitPrefix = null;
                SByte RestMultPrefixExponent = 0;
                if (multPrefixExponent != 0)
                {
                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(multPrefixExponent, out unitPrefix, out RestMultPrefixExponent);
                    multPrefixExponent = RestMultPrefixExponent;
                }

                if (multExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, multExponent);
                    TempNumerators.Add(temp_pue);
                }
                else if (multExponent < 0)
                {
                    multExponent = (SByte)(-multExponent);
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, multExponent);
                    TempDenominators.Add(temp_pue);
                }
            }

            Double ResScaleFactor = _scaleFactor;
            if (scalingPrefixExponent != 0 && scalingExponent != 0)
            {   // Add scaling factor without unit
                sbyte exp = (sbyte)(scalingPrefixExponent * scalingExponent);

                ResScaleFactor = _scaleFactor * Math.Pow(10, exp);
                /*
                if (TempNumerators.Count > 0 && TempNumerators[0].Unit == null && TempNumerators[0].Exponent == 1)
                {   // Already has a scaling factor; Adjust it
                    exp += TempNumerators[0].Prefix.Exponent;

                    if (exp != 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(new UnitPrefixExponent(exp), null, 1);
                        TempNumerators[0] = temp_pue; 
                    } 
                    else
                    {
                        // Scaling factor is 1 now; Remove the scaling factor; 
                        TempNumerators.RemoveAt(0);
                    }
                }
                else
                {   // Don't have a scaling factor; Add it
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(new UnitPrefixExponent(exp), null, 1);
                    TempNumerators.Insert(0, temp_pue);
                }
                */

            }

            CombinedUnit cu = new CombinedUnit(ResScaleFactor, TempNumerators, TempDenominators);
            return cu;
        }

        public override ICombinedUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            /** * /
            return this.CombinedUnitDivide(prefixedUnitExponent);
        }

        public ICombinedUnit CombinedUnitDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            / * **/
            Debug.Assert(prefixedUnitExponent != null);

            SByte NewExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, NewExponent);
            //return this.CombinedUnitMultiply(temp_pue);
            return this.CombineMultiply(temp_pue);
        }


        //public override ICombinedUnit CombineMultiply(IDerivedUnit derivedUnit)
        public ICombinedUnit CombineMultiply(IDerivedUnit derivedUnit)
        {
            ICombinedUnit result = new CombinedUnit(this);
            int baseUnitIndex = 0;
            IUnitSystem sys = derivedUnit.ExponentsSystem;
            foreach (SByte exp in derivedUnit.Exponents)
            {
                if (exp != 0)
                {
                    result = result.CombineMultiply(new PrefixedUnitExponent(null, sys.BaseUnits[baseUnitIndex], exp));
                }
                baseUnitIndex++;
            }

            return result;
        }

        public override ICombinedUnit CombineMultiply(IPrefixedUnit prefixedUnit)
        {
            ICombinedUnit result = this.CombineMultiply(new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, 1));
            return result;
        }

        public override ICombinedUnit CombineDivide(IPrefixedUnit prefixedUnit)
        {
            ICombinedUnit result = this.CombineMultiply(new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, -1));
            return result;
        }

        public override ICombinedUnit CombineMultiply(INamedSymbolUnit namedSymbolUnit)
        {
            ICombinedUnit result = this.CombineMultiply(new PrefixedUnitExponent(null, namedSymbolUnit, 1));
            return result;
        }

        public override ICombinedUnit CombineDivide(INamedSymbolUnit namedSymbolUnit)
        {
            // ICombinedUnit uRes = this.CombineDivide(physicalUnit);
            ICombinedUnit result = this.CombineMultiply(new PrefixedUnitExponent(null, namedSymbolUnit, -1));
            return result;
        }

        public override ICombinedUnit CombineMultiply(IPhysicalUnit physicalUnit)
        {
            /** * /
            return this.CombinedUnitMultiply(physicalUnit);
        }

        public ICombinedUnit CombinedUnitMultiply(IPhysicalUnit physicalUnit)
        {
            / * **/
            if (physicalUnit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit cu = physicalUnit as ICombinedUnit;
                Debug.Assert(cu != null);
                return this.CombineMultiply(cu);
            }

            if (physicalUnit.Kind == UnitKind.BaseUnit || physicalUnit.Kind == UnitKind.ConvertibleUnit)
            {
                INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
                Debug.Assert(nsu != null);
                return this.CombineMultiply(new PrefixedUnitExponent(null, nsu, 1));
            }

            if (physicalUnit.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit mu = physicalUnit as IMixedUnit;
                Debug.Assert(mu != null);
                return this.CombineMultiply(mu.MainUnit);
            }

            if (physicalUnit.Kind == UnitKind.DerivedUnit)
            {
                IDerivedUnit du = physicalUnit as IDerivedUnit;
                Debug.Assert(du != null);
                return this.CombineMultiply(du);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                Debug.Assert(pu != null);
                return this.CombineMultiply(pu);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnitExponent)
            {
                IPrefixedUnitExponent pue = physicalUnit as IPrefixedUnitExponent;
                Debug.Assert(pue != null);
                return this.CombineMultiply(pue);
            }

            // PrefixedUnitExponent will not accept an IPhysicalUnit: return this.CombinedUnitMultiply(new PrefixedUnitExponent(null, physicalUnit, 1));
            // Will make recursive call without reduction: return this.CombinedUnitMultiply(physicalUnit);
            //return this.CombinedUnitMultiply(new PrefixedUnitExponent(null, physicalUnit, 1));

            // Just try to use as INamedSymbolUnit
            INamedSymbolUnit nsu2 = physicalUnit as INamedSymbolUnit;
            Debug.Assert(nsu2 != null);
            return this.CombineMultiply(nsu2);
        }

        public override ICombinedUnit CombineDivide(IPhysicalUnit physicalUnit)
        {
            /** * /
            return this.CombinedUnitDivide(physicalUnit);
        }

        public ICombinedUnit CombinedUnitDivide(IPhysicalUnit physicalUnit)
        {
            / * **/
            if (physicalUnit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit cu = physicalUnit as ICombinedUnit;
                Debug.Assert(cu != null);
                return this.CombineDivide(cu);
            }

            if (physicalUnit.Kind == UnitKind.BaseUnit || physicalUnit.Kind == UnitKind.ConvertibleUnit
                || (physicalUnit.Kind == UnitKind.DerivedUnit && physicalUnit as INamedSymbolUnit != null))
            {
                INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
                Debug.Assert(nsu != null);
                return this.CombineDivide(new PrefixedUnitExponent(null, nsu, 1));
            }

            if (physicalUnit.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit mu = physicalUnit as IMixedUnit;
                Debug.Assert(mu != null);
                return this.CombineDivide(mu.MainUnit);
            }

            if (physicalUnit.Kind == UnitKind.DerivedUnit)
            {
                IDerivedUnit du = physicalUnit as IDerivedUnit;
                Debug.Assert(du != null);
                return this.CombineDivide(du);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                Debug.Assert(pu != null);
                return this.CombineDivide(pu);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnitExponent)
            {
                IPrefixedUnitExponent pue = physicalUnit as IPrefixedUnitExponent;
                Debug.Assert(pue != null);
                return this.CombineDivide(pue);
            }
            // PrefixedUnitExponent will not accept an IPhysicalUnit: return this.CombinedUnitDivide(new PrefixedUnitExponent(null, physicalUnit, 1));
            // Will make recursive call without reduction: return this.CombinedUnitDivide(physicalUnit);
            // return this.CombinedUnitDivide(new PrefixedUnitExponent(null, physicalUnit, 1));

            // Just try to use as INamedSymbolUnit
            INamedSymbolUnit nsu2 = physicalUnit as INamedSymbolUnit;
            Debug.Assert(nsu2 != null);
            return this.CombineDivide(nsu2);
        }

        //public override ICombinedUnit CombineDivide(IDerivedUnit derivedUnit)
        public ICombinedUnit CombineDivide(IDerivedUnit derivedUnit)
        {
            ICombinedUnit result = new CombinedUnit(this);
            int baseUnitIndex = 0;
            IUnitSystem sys = derivedUnit.ExponentsSystem;
            foreach (SByte exp in derivedUnit.Exponents)
            {
                if (exp != 0)
                {
                    result = result.CombineDivide(new PrefixedUnitExponent(null, sys.BaseUnits[baseUnitIndex], exp));
                }
                baseUnitIndex++;
            }

            return result;
        }

        public override ICombinedUnit CombinePow(SByte exponent)
        {
            /** * /
            return this.CombinedUnitPow(exponent);
        }

        public ICombinedUnit CombinedUnitPow(SByte exponent)
        {
            / * **/
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
                PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

                foreach (IPrefixedUnitExponent ue in Numerators)
                {
                    SByte NewExponent = (SByte)(ue.Exponent * exponent);
                    if (NewExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                        TempNumerators.Add(temp_pue);
                    }
                    if (NewExponent < 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, (sbyte)(-NewExponent));
                        TempDenominators.Add(temp_pue);
                    }
                }

                foreach (IPrefixedUnitExponent ue in Denominators)
                {
                    SByte NewExponent = (SByte)(ue.Exponent * exponent);
                    if (NewExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                        TempDenominators.Add(temp_pue);
                    }
                    if (NewExponent < 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, (sbyte)(-NewExponent));
                        TempNumerators.Add(temp_pue);
                    }
                }

                CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
                return cu;
            }
        }

        public override ICombinedUnit CombineRot(SByte exponent)
        {
            /** */
            return this.CombinedUnitRot(exponent);
        }

        public ICombinedUnit CombinedUnitRot(SByte exponent)
        {
            /* **/
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
                PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

                foreach (IPrefixedUnitExponent ue in Numerators)
                {
                    SByte NewExponent = (SByte)(ue.Exponent / exponent);
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                    TempNumerators.Add(temp_pue);
                }

                foreach (IPrefixedUnitExponent ue in Denominators)
                {
                    SByte NewExponent = (SByte)(ue.Exponent / exponent);
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                    TempDenominators.Add(temp_pue);
                }

                CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
                return cu;
            }
        }


        public ICombinedUnit CombineMultiply(ICombinedUnit cu2)
        {
            if (this.IsDimensionless)
            {
                return cu2.CombineMultiply(this.FactorValue);
            }

            ICombinedUnit cu1 = new CombinedUnit(this.FactorValue * cu2.FactorValue, this.Numerators, this.Denominators);

            foreach (IPrefixedUnitExponent pue in cu2.Numerators)
            {
                /*
                IPrefixedUnitExponent pue_n = new PrefixedUnitExponent(pue);

                cu1 = cu1.CombinedUnitMultiply(pue_n);
                */
                cu1 = cu1.CombineMultiply(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                /*
                IPrefixedUnitExponent pue_d = new PrefixedUnitExponent(pue);

                cu1 = cu1.CombinedUnitDivide(pue_d);
                */
                cu1 = cu1.CombineDivide(pue);
            }

            return cu1;
        }

        public ICombinedUnit CombineDivide(ICombinedUnit cu2)
        {
            ICombinedUnit cu1 = new CombinedUnit(this.FactorValue / cu2.FactorValue, this.Numerators, this.Denominators);

            foreach (IPrefixedUnitExponent pue in cu2.Numerators)
            {
                /*
                IPrefixedUnitExponent pue_n = new PrefixedUnitExponent(pue);
                cu1 = cu1.CombineDivide(pue_n);
                */
                cu1 = cu1.CombineDivide(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                /*
                IPrefixedUnitExponent pue_d = new PrefixedUnitExponent(pue);
                cu1 = cu1.CombineMultiply(pue_d);
                */
                cu1 = cu1.CombineMultiply(pue);
            }

            return cu1;
        }


        #endregion IPhysicalUnitMath Members

        #region IEquatable<IPhysicalUnit> Members

        public override int GetHashCode()
        {
            return _Numerators.GetHashCode() + _Denominators.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            IPhysicalUnit otherIPU = other as IPhysicalUnit;

            if (otherIPU == null)
                return false;

            return this.Equals(otherIPU);
        }

        /**
        public override bool Equals(IPhysicalUnit other)
        {
            if (other == null)
            {
                return false;
            }

            IPhysicalQuantity temp = this.ConvertTo(other);

            if (temp == null)
            {
                return false;
            }

            return temp.Equals(other);
        }
        **/

        #endregion IEquatable<IPhysicalUnit> Members

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            return CombinedUnitString(mayUseSlash: true, invertExponents: false);
        }

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String UnitName = "";
            Boolean NextLevelMayUseSlash = mayUseSlash && Denominators.Count == 0;
            if (Numerators.Count > 0)
            {
                UnitName = Numerators.CombinedUnitString(NextLevelMayUseSlash, invertExponents);
            }
            else
            {
                if (Denominators.Count > 0)
                {
                    //UnitName = "1";
                }
            }

            if (Denominators.Count > 0)
            {
                if (mayUseSlash && !String.IsNullOrWhiteSpace(UnitName))
                {
                    UnitName += "/" + Denominators.CombinedUnitString(false, invertExponents);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(UnitName))
                    {
                        // center dot '\0x0B7' (Char)183 U+00B7
                        UnitName += '·' + Denominators.CombinedUnitString(false, !invertExponents);
                    }
                    else
                    {
                        UnitName = Denominators.CombinedUnitString(false, !invertExponents);
                    }
                }
            }
            return UnitName;
        }
        public override String ToString()
        {
            return UnitString();
        }

    }

    #endregion Combined Unit Classes

    #region Mixed Unit Classes

    public class MixedUnit : PhysicalUnit, IMixedUnit
    {
        protected readonly IPhysicalUnit _MainUnit;
        protected readonly IPhysicalUnit _FractionalUnit;

        protected readonly String _Separator;
        protected readonly String _FractionalValueFormat;

        public IPhysicalUnit MainUnit
        {
            get
            {
                Debug.Assert(_MainUnit != null);
                return this._MainUnit;
            }
        }

        public IPhysicalUnit FractionalUnit
        {
            get
            {
                Debug.Assert(_MainUnit != null);
                return this._FractionalUnit;
            }
        }

        public String Separator
        {
            get
            {
                Debug.Assert(_Separator != null);
                return this._Separator;
            }
        }

        public MixedUnit(IPhysicalUnit mainUnit, String separator, IPhysicalUnit fractionalUnit, String fractionalValueFormat)
        {
            this._MainUnit = mainUnit;
            this._Separator = separator;
            this._FractionalUnit = fractionalUnit;
            this._FractionalValueFormat = fractionalValueFormat;
        }

        public MixedUnit(IPhysicalUnit mainUnit, String separator, IPhysicalUnit fractionalUnit)
            : this(mainUnit, separator, fractionalUnit, "00.################")
        {
        }

        public MixedUnit(IPhysicalUnit mainUnit, IPhysicalUnit fractionalUnit)
            : this(mainUnit, ":", fractionalUnit)
        {
        }

        public override UnitKind Kind { get { return UnitKind.MixedUnit; } }

        public override IUnitSystem SimpleSystem
        {
            get
            {
                Debug.Assert(_MainUnit != null);
                return MainUnit.SimpleSystem;
            }
            set
            {
                Debug.Assert(_MainUnit != null);
                /* Just do nothing */
                //MainUnit.System = value; 
                Debug.Assert(MainUnit.SimpleSystem == value);

            }
        }

        public override IUnitSystem ExponentsSystem
        {
            get
            {
                Debug.Assert(_MainUnit != null);
                return MainUnit.ExponentsSystem;
            }
        }

        public override SByte[] Exponents
        {
            get
            {
                Debug.Assert(_MainUnit != null);
                return MainUnit.Exponents;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsLinearConvertible()
        {
            Debug.Assert(_MainUnit != null);
            return _MainUnit.IsLinearConvertible();
        }


        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            Debug.Assert(_MainUnit != null);
            return MainUnit.ConvertToSystemUnit(ref quantity);
        }

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            Debug.Assert(_MainUnit != null);
            return MainUnit.ConvertToSystemUnit();
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            return this.ConvertToBaseUnit(1);
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            return this.ConvertToBaseUnit();
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return this.ConvertToSystemUnit(ref quantity).ConvertToBaseUnit();
        }

        /*
        Previous base class PhysicalUnit has abstract declaration.
        Now general method is implemented for PhysicalUnit
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {e
            return physicalQuantity.ConvertTo(this).ConvertToBaseUnit();
        }
        */

        public override string PureUnitString()
        {
            Debug.Assert(_MainUnit != null);

            string us = MainUnit.UnitString();
            if (FractionalUnit != null)
            {
                us = us + this.Separator + FractionalUnit.UnitString();
            }
            return us;
        }

        public override string ValueString(double quantity)
        {
            return ValueString(quantity, null, null);
        }

        public override string ValueString(double quantity, String format, IFormatProvider formatProvider)
        {
            Debug.Assert(_MainUnit != null);

            string ValStr;
            if (FractionalUnit != null)
            {
                double integralValue = Math.Truncate(quantity);
                double fractionalValue = quantity - integralValue;
                IPhysicalQuantity fracPQ = new PhysicalQuantity(fractionalValue, this.MainUnit);
                IPhysicalQuantity fracPQConv = fracPQ.ConvertTo(this.FractionalUnit);
                if (fracPQConv != null)
                {
                    ValStr = MainUnit.ValueString(integralValue, format, formatProvider) + Separator + FractionalUnit.ValueString(fracPQConv.Value, _FractionalValueFormat, null);
                }
                else
                {
                    ValStr = MainUnit.ValueString(quantity, format, formatProvider);
                }
            }
            else
            {
                ValStr = MainUnit.ValueString(quantity, format, formatProvider);
            }
            return ValStr;
        }
    }

    #endregion Mixed Unit Classes

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public abstract class UnitSystemBase : NamedObject, IUnitSystem
    {

        public abstract IUnitPrefixTable UnitPrefixes { get; }
        public abstract IBaseUnit[] BaseUnits { get; /* */ set; /* */ }
        public abstract INamedDerivedUnit[] NamedDerivedUnits { get; /* */ set; /* */ }
        public abstract IConvertibleUnit[] ConvertibleUnits { get; /* */ set; /* */ }

        public abstract Boolean IsIsolatedUnitSystem { get; }
        public abstract Boolean IsCombinedUnitSystem { get; }

        protected IPhysicalUnit dimensionless;
        public virtual IPhysicalUnit Dimensionless
        { get
            {
                if (dimensionless == null)
                {
                    dimensionless = new DerivedUnit(this, new SByte[] { 0 });
                }
                return dimensionless;
            }
        }

        public UnitSystemBase(String someName)
            : base(someName)
        {
        }

        public override String ToString()
        {
            return this.Name;
        }

        protected static INamedSymbolUnit UnitFromName(INamedSymbolUnit[] units, String unitname)
        {
            if (units != null)
            {
                foreach (INamedSymbolUnit u in units)
                {
                    if (u.Name.Equals(unitname, StringComparison.OrdinalIgnoreCase))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        protected static INamedSymbolUnit UnitFromSymbol(INamedSymbolUnit[] units, String unitsymbol)
        {
            if (units != null)
            {
                foreach (INamedSymbolUnit u in units)
                {
                    // StringComparison must consider case))
                    if (u.Symbol.Equals(unitsymbol, StringComparison.Ordinal))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        public INamedSymbolUnit UnitFromName(String unitName)
        {
            INamedSymbolUnit unit;

            unit = UnitFromName(this.BaseUnits, unitName);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.NamedDerivedUnits, unitName);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.ConvertibleUnits, unitName);
            return unit;
        }

        public INamedSymbolUnit UnitFromSymbol(String unitSymbol)
        {
            INamedSymbolUnit unit;

            unit = UnitFromSymbol(this.BaseUnits, unitSymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.NamedDerivedUnits, unitSymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.ConvertibleUnits, unitSymbol);
            return unit;
        }

        //public IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol)
        // public IPrefixedUnit ScaledUnitFromSymbol(String scaledUnitSymbol)
        public IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol)
        {
            //IPhysicalUnit unit = UnitFromSymbol(scaledUnitSymbol);
            INamedSymbolUnit unit = UnitFromSymbol(scaledUnitSymbol);
            if (scaledUnitSymbol.Length > 1)
            {   // Check for prefixed unit 
                Char prefixchar = scaledUnitSymbol[0];
                /*
                SByte scaleExponent = 0;
                if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleExponent))
                */
                //IUnitPrefixExponent scaleExponent;
                // if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleExponent))
                IUnitPrefix unitPrefix;
                if (UnitPrefixes.GetUnitPrefixFromPrefixChar(prefixchar, out unitPrefix))
                {
                    //IPhysicalUnit unit2 = UnitFromSymbol(scaledUnitSymbol.Substring(1));
                    INamedSymbolUnit unit2 = UnitFromSymbol(scaledUnitSymbol.Substring(1));
                    if (unit2 != null)
                    {   // Found both a prefix and an unit; Must be the right unit. 
                        // Overwrite unit even if set by non-prefixed unit (first call to UnitFromSymbol())
                        if (unit != null)
                        {
                            // SI.Kg <-> SI_prefix.K·SI.g           Prefer (non-prefixed) unit
                            // SI.K (Kelvin) <-> SI_prefix.K·...    Prefer (prefixed) unit2

                            if (unit == SI.Kg && prefixchar == 'K' && unit2 == SI.g)  // SI.Kg <-> SI_prefix.K·SI.g       Prefer (non-prefixed) unit
                            {
                                // Prefer (non-prefixed) unit  
                                //Debug.Assert(unit == null); // For debug. Manually check if overwritten unit is a better choice.
                                //return (IPrefixedUnit)unit;
                                return unit;
                            }
                            // Prefer unit2
                            // Overwrite unit even if set by non-prefixed unit (first call to UnitFromSymbol())
                            Debug.Assert(unit == null); // For debug. Manually check if overwritten unit could be a better choice than unit 2.
                        }

                        // Found both a prefix and an unit; Must be the right unit. 
                        unit = unit2;
                        //if (scaleExponent.Exponent != 0)

                        Debug.Assert(unitPrefix != null); // GetUnitPrefixFromPrefixChar must have returned a valid unitPrefix
                        if (unitPrefix != null)
                        {
                            //unit = unit.CombinePrefix(scaleExponent);
                            //unit = unit.Multiply(scaleExponent);
                            //IPrefixedUnit  pu = unit.Multiply(scaleExponent);
                            IPrefixedUnit pu = new PrefixedUnit(unitPrefix, unit);
                            return pu;
                        }
                    }
                }
            }
            return unit;
        }

        public IPhysicalUnit UnitFromExponents(SByte[] exponents)
        {
            SByte NoOfNonZeroExponents = 0;
            SByte NoOfNonOneExponents = 0;
            SByte FirstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in exponents)
            {
                if (exponent != 0)
                {
                    if (FirstNonZeroExponent == -1)
                    {
                        FirstNonZeroExponent = i;
                    }
                    NoOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        NoOfNonOneExponents++;
                    }
                }

                i++;
            }

            return UnitFromUnitInfo(exponents, NoOfNonZeroExponents, NoOfNonOneExponents, FirstNonZeroExponent);
        }

        public IPhysicalUnit UnitFromUnitInfo(SByte[] exponents, SByte NoOfNonZeroExponents, SByte NoOfNonOneExponents, SByte FirstNonZeroExponent)
        {
            IPhysicalUnit unit;

            if ((NoOfNonZeroExponents == 1) && (NoOfNonOneExponents == 0))
            {
                // BaseUnit 
                unit = (IPhysicalUnit)BaseUnits[FirstNonZeroExponent];
            }
            else
            {
                // Check if it is a NamedDerivedUnit 
                unit = null;
                if (NamedDerivedUnits != null)
                {
                    int namedderivedunitsindex = 0;

                    while ((namedderivedunitsindex < NamedDerivedUnits.Length)
                           && !NamedDerivedUnits[namedderivedunitsindex].Exponents.DimensionEquals(exponents))
                    {
                        namedderivedunitsindex++;
                    }

                    if (namedderivedunitsindex < NamedDerivedUnits.Length)
                    {
                        // NamedDerivedUnit 
                        unit = (IPhysicalUnit)NamedDerivedUnits[namedderivedunitsindex];
                    }
                }
                if (unit == null)
                {
                    // DerivedUnit 
                    unit = new DerivedUnit(this, exponents);
                }
            }

            return unit;
        }

        public INamedSymbolUnit NamedDerivedUnitFromUnit(IDerivedUnit derivedUnit)
        {
            SByte[] exponents = derivedUnit.Exponents;
            int NoOfDimensions = exponents.NoOfDimensions();
            if (NoOfDimensions > 1)
            {
                INamedSymbolUnit ns = NamedDerivedUnits.FirstOrNull(namedderivedunit => exponents.DimensionEquals(namedderivedunit.Exponents));
                return ns;
            }

            return null;
        }

        public INamedSymbolUnit NamedDerivedUnitFromUnit(IPhysicalUnit derivedUnit)
        {
            //IPhysicalQuantity pq = derivedUnit.ConvertToSystemUnit();
            //IPhysicalQuantity pq = derivedUnit.ConvertToBaseUnit();
            IPhysicalQuantity pq = derivedUnit.ConvertToDerivedUnit();
            if (PhysicalQuantity.IsPureUnit(pq))
            {
                IPhysicalUnit derunit = PhysicalQuantity.PureUnit(pq);
                SByte[] Exponents = derunit.Exponents;
                int NoOfDimensions = Exponents.NoOfDimensions();
                if (NoOfDimensions > 1)
                {
                    foreach (NamedDerivedUnit namedderivedunit in this.NamedDerivedUnits)
                    {
                        if (Exponents.DimensionEquals(namedderivedunit.Exponents))
                        {
                            return namedderivedunit;
                        }
                    }
                }
            }

            return null;
        }


        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IPhysicalUnit convertToUnit)
        {
            // Relative conversion is assumed
            // Handle relative unit e.g. temperature interval ....

            Debug.Assert(convertFromUnit != null);
            Debug.Assert(convertToUnit != null);

            if (convertFromUnit == null)
            {
                throw new ArgumentNullException("convertFromUnit");
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException("convertToUnit");
            }
            //bool isSameUnit = convertFromUnit == convertToUnit;
            //bool isSameUnit = convertFromUnit.Equals(convertToUnit);
            Double quotient = 1;  // 0 means not equivalent unit
            bool isEquivalentUnit = convertFromUnit.Equivalent(convertToUnit, out quotient);
            //bool isEquivalentUnit = convertFromUnit.Equals(convertToUnit);
            if (isEquivalentUnit)
            {
                //bool isEquivalentUnit2 = convertFromUnit.Equals(convertToUnit);
                // return new PhysicalQuantity(1, convertToUnit);
                return new PhysicalQuantity(quotient, convertToUnit);
            }
            else
            {
                if (convertFromUnit.Kind == UnitKind.MixedUnit)
                {
                    IMixedUnit imu = (IMixedUnit)convertFromUnit;
                    IPhysicalQuantity pq = ConvertTo(imu.MainUnit, convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.MixedUnit)
                {
                    IMixedUnit imu = (IMixedUnit)convertToUnit;
                    IPhysicalQuantity pq = ConvertTo(convertFromUnit, imu.MainUnit);
                    return new PhysicalQuantity(pq.Value, convertToUnit);
                }
                else if (convertFromUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)convertFromUnit;
                    IPhysicalQuantity pq_prim = icu.ConvertToPrimaryUnit();
                    IPhysicalQuantity pq = pq_prim.Unit.ConvertTo(convertToUnit);
                    if (pq != null)
                    {
                        pq = pq.Multiply(pq_prim.Value);
                    }
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)convertToUnit;
                    IPhysicalQuantity converted_fromunit = convertFromUnit.ConvertTo(icu.PrimaryUnit);
                    if (converted_fromunit != null)
                    {
                        converted_fromunit = icu.ConvertFromPrimaryUnit(converted_fromunit.Value);
                    }
                    return converted_fromunit;
                }
                else if (convertFromUnit.Kind == UnitKind.PrefixedUnit)
                {
                    IPrefixedUnit icu = (IPrefixedUnit)convertFromUnit;
                    IPhysicalQuantity pq_derivedUnit = icu.ConvertToDerivedUnit();
                    IPhysicalQuantity pq = pq_derivedUnit.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.PrefixedUnit)
                {
                    IPrefixedUnit icu = (IPrefixedUnit)convertToUnit;
                    IPhysicalQuantity pq_unit = convertFromUnit.ConvertTo(icu.Unit);
                    if (pq_unit != null)
                    {
                        IPhysicalQuantity pq = pq_unit.Divide(icu.Prefix.Value);
                        if (pq != null)
                        {
                            return new PhysicalQuantity(pq.Value, convertToUnit);
                        }
                    }
                    return null;
                }
                else if (convertFromUnit.Kind == UnitKind.PrefixedUnitExponent)
                {
                    IPrefixedUnitExponent pue = (IPrefixedUnitExponent)convertFromUnit;
                    IPhysicalQuantity pq_derivedUnit = pue.ConvertToDerivedUnit();
                    IPhysicalQuantity pq = pq_derivedUnit.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.PrefixedUnitExponent)
                {
                    IPrefixedUnitExponent pue = (IPrefixedUnitExponent)convertToUnit;
                    IPhysicalQuantity pue_derivedUnit = pue.ConvertToDerivedUnit();
                    IPhysicalQuantity converted_fromunit = convertFromUnit.ConvertTo(pue_derivedUnit.Unit);
                    if (converted_fromunit != null)
                    {
                        return new PhysicalQuantity(converted_fromunit.Value / pue_derivedUnit.Value, convertToUnit);
                    }
                    return null;
                }
                else if (convertFromUnit.Kind == UnitKind.CombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)convertFromUnit;
                    IPhysicalQuantity pq = icu.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.CombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)convertToUnit;
                    IPhysicalQuantity pqToUnit;
                    pqToUnit = icu.ConvertTo(convertFromUnit);
                    if (pqToUnit != null)
                    {
                        /*
                        IPhysicalQuantity pq = convertFromUnit.Divide(pqToUnit.Unit);
                        if (pq.Unit == null || DimensionExponents.IsDimensionless(pq.Unit.Exponents))
                        {
                            return new PhysicalQuantity(pq.Value / pqToUnit.Value, convertToUnit);
                        }
                        */
                        IPhysicalUnit pu = convertFromUnit.Divide(pqToUnit.Unit);
                        if (pu == null || pu.Exponents.IsDimensionless())
                        {
                            return new PhysicalQuantity(1 / pqToUnit.Value, convertToUnit);
                        }
                    }

                    return null;
                }

                // From some simple system to some simple system
                if ((convertFromUnit.SimpleSystem == this) && (convertToUnit.SimpleSystem == this))
                {   // Intra unit system conversion 
                    Debug.Assert((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit));
                    Debug.Assert((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit));

                    if (!((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of BaseUnit or DerivedUnit", "convertFromUnit");
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or DerivedUnit", "convertToUnit");
                    }

                    if (convertFromUnit.Exponents.DimensionEquals(convertToUnit.Exponents))
                    {
                        return new PhysicalQuantity(1, convertToUnit);
                    }
                }
                else
                {   // Inter unit system conversion 
                    UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnit.SimpleSystem, convertToUnit.SimpleSystem);
                    if (usc != null)
                    {
                        return usc.ConvertTo(convertFromUnit, convertToUnit);
                    }
                }
                return null;
            }
        }


        public virtual IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertFromUnit != null);
            Debug.Assert(convertToUnitSystem != null);

            IUnitSystem convertFromUnitSystem = convertFromUnit.SimpleSystem;

            if (convertFromUnitSystem == convertToUnitSystem
                || (convertToUnitSystem.IsCombinedUnitSystem && ((CombinedUnitSystem)convertToUnitSystem).ContainsSubUnitSystem(convertFromUnitSystem)))
            {
                return new PhysicalQuantity(1, convertFromUnit);
            }


            {   // Inter unit system conversion 
                UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnitSystem, convertToUnitSystem);
                if (usc != null)
                {
                    return usc.ConvertTo(convertFromUnit.ConvertToBaseUnit(), convertToUnitSystem);
                }

                if (convertFromUnitSystem.IsIsolatedUnitSystem || convertToUnitSystem.IsIsolatedUnitSystem)
                {
                    // Unit system declared to be isolated (user defined) without conversion to other (physical) unit systems.
                    return null;
                }

                /* Missing unit system conversion from physicalquantity.Unit.System to ToUnitSystem */
                /* TO DO Find intermediate systems with conversions between physicalquantity.Unit.System and convertToUnitSystem */
                Debug.Assert(false, "Missing unit system conversion from " + convertFromUnitSystem.Name + " to " + convertToUnitSystem.Name);

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit)
        {
            // return RelativeConvertTo(physicalQuantity, convertToUnit);
            // We need to use specific conversion of unit, if either convertFromUnit or convertToUnit are a pure linear scaled unit.
            bool physicalQuantityUnitRelativeconversion = physicalQuantity.Unit.IsLinearConvertible();
            bool convertToUnitRelativeconversion = convertToUnit.IsLinearConvertible();
            bool Relativeconversion = physicalQuantityUnitRelativeconversion && convertToUnitRelativeconversion;
            if (Relativeconversion)
            {
                IPhysicalQuantity pq = this.ConvertTo(physicalQuantity.Unit, convertToUnit);
                if (pq != null)
                {
                    pq = pq.Multiply(physicalQuantity.Value);
                }
                return pq;
            }
            else
            {
                return SpecificConvertTo(physicalQuantity, convertToUnit);
            }
        }

        public IPhysicalQuantity SpecificConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(physicalQuantity.Unit != null);
            Debug.Assert(convertToUnit != null);

            if (physicalQuantity.Unit == null)
            {
                throw new ArgumentException("Must have a unit", "physicalQuantity");
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException("convertToUnit");
            }

            if (physicalQuantity.Unit == convertToUnit)
            {
                return physicalQuantity;
            }

            if (physicalQuantity.Unit.SimpleSystem != null && physicalQuantity.Unit.SimpleSystem != this)
            {
                return physicalQuantity.Unit.SimpleSystem.SpecificConvertTo(physicalQuantity, convertToUnit);
            }
            else
            {
                IUnitSystem convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                IUnitSystem converttounitsystem = convertToUnit.SimpleSystem;

                if (convertfromunitsystem == null)
                {
                    string physicalQuantityUnitKind_debug_trace_str = physicalQuantity.Unit.Kind.ToString();
                    string physicalQuantity_debug_trace_str = physicalQuantity.ToPrintString();
                    Debug.Assert(physicalQuantity.Unit.Kind == UnitKind.CombinedUnit);

                    IUnitSystem tempconverttounitsystem = converttounitsystem;
                    if (tempconverttounitsystem == null)
                    {   // Find some system to convert into

                        ICombinedUnit icu = (ICombinedUnit)physicalQuantity.Unit;
                        Debug.Assert(icu != null);

                        tempconverttounitsystem = icu.SomeSimpleSystem;
                    }
                    if (tempconverttounitsystem != null)
                    {
                        physicalQuantity = physicalQuantity.ConvertTo(tempconverttounitsystem);

                        if (physicalQuantity != null)
                        {
                            convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                        }
                        else
                        {
                            // ?? What TO DO here ??
                            Debug.Assert(false);

                            return null;
                        }
                    }
                    else
                    {
                        // ?? What TO DO here ??
                        Debug.Assert(false);

                        return null;
                    }
                }

                if (converttounitsystem == null)
                {
                    Debug.Assert(convertToUnit.Kind == UnitKind.CombinedUnit);

                    ICombinedUnit icu = (ICombinedUnit)convertToUnit;
                    Debug.Assert(icu != null);

                    IUnitSystem tempconverttounitsystem;
                    tempconverttounitsystem = icu.SomeSimpleSystem;
                    Debug.Assert(tempconverttounitsystem != null);

                    // ?? What TO DO here ??
                    Debug.Assert(false);
                }

                if (converttounitsystem != null && convertfromunitsystem != converttounitsystem)
                {   // Inter unit system conversion 

                    if (physicalQuantity.Unit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)physicalQuantity.Unit;
                        double d = physicalQuantity.Value;
                        physicalQuantity = icu.ConvertToSystemUnit(ref d);
                    }

                    IPhysicalQuantity pq = this.ConvertTo(physicalQuantity, converttounitsystem);
                    if (pq != null)
                    {
                        physicalQuantity = pq;
                        convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (convertfromunitsystem != null && convertfromunitsystem == converttounitsystem)
                {   // Intra unit system conversion 

                    if (physicalQuantity.Unit.Kind == UnitKind.MixedUnit)
                    {
                        IMixedUnit imu = (IMixedUnit)physicalQuantity.Unit;

                        IPhysicalQuantity pq = new PhysicalQuantity(physicalQuantity.Value, imu.MainUnit);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
                    }
                    else if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)physicalQuantity.Unit;
                        double d = physicalQuantity.Value;
                        IPhysicalQuantity pq = icu.ConvertToSystemUnit(ref d);
                        Debug.Assert(pq != null);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
                    }
                    else if (physicalQuantity.Unit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)physicalQuantity.Unit;
                        IPhysicalQuantity prim_pq = icu.ConvertToPrimaryUnit(physicalQuantity.Value);
                        return ConvertTo(prim_pq, convertToUnit);
                    }
                    else if (convertToUnit.Kind == UnitKind.MixedUnit)
                    {
                        IMixedUnit imu = (IMixedUnit)convertToUnit;

                        IPhysicalQuantity pq = ConvertTo(physicalQuantity, imu.MainUnit);
                        if (pq != null)
                        {
                            pq = new PhysicalQuantity(pq.Value, convertToUnit);
                        }
                        return pq;
                    }
                    else if (convertToUnit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)convertToUnit;
                        /*
                        IPhysicalQuantity ToUnit_SystemUnit_pq = icu.ConvertToSystemUnit();
                        IPhysicalQuantity ToUnit_BaseUnit_pq = icu.ConvertToBaseUnit();

                        Debug.Assert(ToUnit_SystemUnit_pq != null);

                        IPhysicalQuantity pq_ToSystemUnit = physicalQuantity.ConvertTo(ToUnit_SystemUnit_pq.Unit);
                        if (pq_ToSystemUnit != null)
                        {
                            return new PhysicalQuantity(pq_ToSystemUnit.Value / ToUnit_SystemUnit_pq.Value, convertToUnit);
                        }
                        else
                        {
                            return null;
                        }
                         */
                        IPhysicalQuantity pq = icu.ConvertFrom(physicalQuantity);
                        return pq;
                    }
                    else if (convertToUnit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)convertToUnit;
                        IPhysicalQuantity pq = ConvertTo(physicalQuantity, icu.PrimaryUnit);
                        if (pq != null)
                        {
                            pq = icu.ConvertFromPrimaryUnit(pq.Value);
                        }
                        return pq;
                    }

                    Debug.Assert((physicalQuantity.Unit.Kind == UnitKind.BaseUnit) || (physicalQuantity.Unit.Kind == UnitKind.DerivedUnit));
                    Debug.Assert((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit));

                    if (!((physicalQuantity.Unit.Kind == UnitKind.BaseUnit) || (physicalQuantity.Unit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of BaseUnit or a DerivedUnit", "physicalQuantity");
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or a DerivedUnit", "convertToUnit");
                    }

                    if (physicalQuantity.Unit.Exponents.DimensionEquals(convertToUnit.Exponents))
                    {
                        return new PhysicalQuantity(physicalQuantity.Value, convertToUnit);
                    }
                }

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitSystem = physicalQuantity.Unit.SimpleSystem;
            if (convertFromUnitSystem == convertToUnitSystem)
            {
                return physicalQuantity;
            }

            if (convertFromUnitSystem.IsIsolatedUnitSystem || convertToUnitSystem.IsIsolatedUnitSystem)
            {
                // Unit system declared to be isolated (user defined) without conversion to other (physical) unit systems.
                return null;
            }
            else
            {   // Inter unit system conversion 
                UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnitSystem, convertToUnitSystem);
                if (usc != null)
                {
                    return usc.ConvertTo(physicalQuantity.ConvertToBaseUnit(), convertToUnitSystem);
                }

                /* Missing unit system conversion from  physicalquantity.Unit.System to ToUnitSystem */
                /* TO DO Find intermediate systems with conversions between physicalquantity.Unit.System and convertToUnitSystem */
                Debug.Assert(false);

                return null;
            }
        }
    }

    public class UnitSystem : UnitSystemBase
    {
        private /* readonly */ UnitPrefixTable _unitprefixes;
        private /* readonly */ BaseUnit[] _baseunits;
        private /* readonly */ NamedDerivedUnit[] _namedderivedunits;
        private /* readonly */ ConvertibleUnit[] _convertibleunits;

        private /* readonly */ Boolean _isIsolated;

        public override IUnitPrefixTable UnitPrefixes { get { return _unitprefixes; } }
        public override IBaseUnit[] BaseUnits { get { return _baseunits; } /* */ set { _baseunits = (BaseUnit[])value; CheckBaseUnitSystem(); } /* */ }
        public override INamedDerivedUnit[] NamedDerivedUnits { get { return _namedderivedunits; } /* */ set { _namedderivedunits = (NamedDerivedUnit[])value; CheckNamedDerivedUnitSystem(); } /* */  }
        public override IConvertibleUnit[] ConvertibleUnits { get { return _convertibleunits; } /* */ set { _convertibleunits = (ConvertibleUnit[])value; CheckConvertibleUnitSystem(); } /* */  }

        public override Boolean IsIsolatedUnitSystem { get { return _isIsolated; } }
        public override Boolean IsCombinedUnitSystem { get { return false; } }

        public UnitSystem(String someName, Boolean isIsolated)
            : base(someName)
        {
            this._isIsolated = isIsolated;
        }

        public UnitSystem(String someName)
            : this(someName, true)
        {
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes)
            : this(someName, false)
        {
            this._unitprefixes = someUnitPrefixes;
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] baseUnits)
            : this(someName, someUnitPrefixes)
        {
            this._baseunits = baseUnits;

            CheckBaseUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] baseUnits, NamedDerivedUnit[] someNamedDerivedUnits)
            : this(someName, someUnitPrefixes, baseUnits)
        {
            this._namedderivedunits = someNamedDerivedUnits;

            CheckNamedDerivedUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] baseUnits, NamedDerivedUnit[] someNamedDerivedUnits, ConvertibleUnit[] someConvertibleUnits)
            : this(someName, someUnitPrefixes, baseUnits, someNamedDerivedUnits)
        {
            this._convertibleunits = someConvertibleUnits;

            CheckConvertibleUnitSystem();
        }

        private void CheckBaseUnitSystem()
        {
            Debug.Assert(this._baseunits != null);

            foreach (BaseUnit baseunit in this._baseunits)
            {
                Debug.Assert(baseunit.Kind == UnitKind.BaseUnit);
                if (baseunit.Kind != UnitKind.BaseUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.BaseUnit", "BaseUnits");
                }
                if (baseunit.SimpleSystem != this)
                {
                    Debug.Assert(baseunit.SimpleSystem == null);
                    baseunit.SimpleSystem = this;
                }
            }
        }

        private void CheckNamedDerivedUnitSystem()
        {
            if (this._namedderivedunits != null)
            {
                foreach (NamedDerivedUnit namedderivedunit in this._namedderivedunits)
                {
                    Debug.Assert(namedderivedunit.Kind == UnitKind.DerivedUnit);
                    if (namedderivedunit.Kind != UnitKind.DerivedUnit)
                    {
                        throw new ArgumentException("Must only contain units with Kind = UnitKind.DerivedUnit", "someNamedDerivedUnits");
                    }
                    if (namedderivedunit.SimpleSystem != this)
                    {
                        namedderivedunit.SimpleSystem = this;
                    }
                }
            }
        }

        private void CheckConvertibleUnitSystem()
        {
            if (this._convertibleunits != null)
            {
                foreach (ConvertibleUnit convertibleunit in this._convertibleunits)
                {
                    Debug.Assert(convertibleunit.Kind == UnitKind.ConvertibleUnit);
                    if (convertibleunit.Kind != UnitKind.ConvertibleUnit)
                    {
                        throw new ArgumentException("Must only contain units with Kind = UnitKind.DerivedUnit", "someConvertibleUnits");
                    }
                    if (convertibleunit.SimpleSystem != this)
                    {
                        convertibleunit.SimpleSystem = this;
                    }
                    if (convertibleunit.PrimaryUnit.SimpleSystem == null)
                    {
                        (convertibleunit.PrimaryUnit as PhysicalUnit).SimpleSystem = this;
                    }
                }
            }
        }


    }



    public class CombinedUnitSystem : UnitSystemBase, ICombinedUnitSystem
    {
        private /* readonly */ IUnitSystem[] _unitSystemes;

        private /* readonly */ IUnitPrefixTable _unitprefixes;
        private /* readonly */ IBaseUnit[] _baseunits;
        private /* readonly */ INamedDerivedUnit[] _namedderivedunits;
        private /* readonly */ IConvertibleUnit[] _convertibleunits;


        public IUnitSystem[] UnitSystemes { get { return _unitSystemes; }  /*  set { throw new NotImplementedException(); }  */ }

        public override IUnitPrefixTable UnitPrefixes { get { return _unitprefixes; } }
        /**
        public override IBaseUnit[] BaseUnits { get { return UnitSystemes[0].BaseUnits.Concat( UnitSystemes[1].BaseUnits).ToArray(); }  /* * / set { throw new NotImplementedException(); } /* * / }
        public override INamedDerivedUnit[] NamedDerivedUnits { get { return UnitSystemes[0].NamedDerivedUnits.Concat(UnitSystemes[1].NamedDerivedUnits).ToArray(); } /* * / set { throw new NotImplementedException(); } /* * / }
        public override IConvertibleUnit[] ConvertibleUnits { get { return UnitSystemes[0].ConvertibleUnits.Concat(UnitSystemes[1].ConvertibleUnits).ToArray(); } /* * / set { throw new NotImplementedException(); } /* * / }
        **/

        public override IBaseUnit[] BaseUnits { get { return _baseunits; }  /* */ set { throw new NotImplementedException(); } /* */ }

        public override INamedDerivedUnit[] NamedDerivedUnits { get { return _namedderivedunits; } /* */ set { throw new NotImplementedException(); } /* */ }
        public override IConvertibleUnit[] ConvertibleUnits { get { return _convertibleunits; } /* */ set { throw new NotImplementedException(); } /* */ }


        public override IPhysicalUnit Dimensionless { get { return UnitSystemes[0].Dimensionless; } }

        public override Boolean IsIsolatedUnitSystem { get { return UnitSystemes.All(us => us.IsIsolatedUnitSystem); } }
        public override Boolean IsCombinedUnitSystem { get { return true; } }

        public Boolean ContainsSubUnitSystem(IUnitSystem unitsystem)
        {
            // return !UnitSystemes.All(us => unitsystem != us); 
            return UnitSystemes.Contains(unitsystem);
        }

        public Boolean ContainsSubUnitSystems(IEnumerable<IUnitSystem> unitsystems)
        {
            // return !UnitSystemes.All(us => unitsystem != us); 
            //return UnitSystemes.Contains(unitsystem);
            return UnitSystemes.Union(unitsystems).Count() == UnitSystemes.Count();
        }

        private static List<ICombinedUnitSystem> CombinedUnitSystems = new List<ICombinedUnitSystem>();

        public static ICombinedUnitSystem GetCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            Debug.Assert(!subUnitSystems.Any(us => us.IsCombinedUnitSystem));
            IUnitSystem[] sortedSubUnitSystems = subUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray();
            ICombinedUnitSystem cus = null;

            if (CombinedUnitSystems.Count() > 0)
            {
                IEnumerable<ICombinedUnitSystem> tempUnitSystems = CombinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                if (tempUnitSystems.Count() >= 1)
                {
                    Debug.Assert(tempUnitSystems.Count() == 1);
                    cus = tempUnitSystems.First();
                }
            }
            if (cus == null)
            {
                lock (CombinedUnitSystems)
                {
                    IEnumerable<ICombinedUnitSystem> tempUnitSystems = CombinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                    if (tempUnitSystems.Count() >= 1)
                    {
                        cus = tempUnitSystems.First();
                    }
                    else
                    {
                        cus = new CombinedUnitSystem(null, sortedSubUnitSystems);
                        CombinedUnitSystems.Add(cus);
                        /*
                        if (CombinedUnitSystems.Count() > 1)
                        {
                            Debug.Assert(CombinedUnitSystems.Count() == 1, "GetCombinedUnitSystem found several combined unit systems ");
                        }
                        */
                    }
                }
            }

            return cus;
        }

        public CombinedUnitSystem(String someName, IUnitSystem us1, IUnitSystem us2)
            : this(someName, new IUnitSystem[] { us1, us2 })
        {

        }

        public CombinedUnitSystem(String someName, IUnitSystem[] subUnitSystems)
            : base(someName != null ? someName : "<" + subUnitSystems.Aggregate("", ((str, us) => String.IsNullOrWhiteSpace(str) ? us.Name : str + ", " + us.Name)) + ">")
        {
            SetupCombinedUnitSystem(subUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray());
        }

        public void SetupCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            /**
            IUnitSystem[] tailSubSystems = new IUnitSystem[subUnitSystems.Length - 1];
            tailSubSystems[0] = new CombinedUnitSystem(subUnitSystems[0].Name + ", " + subUnitSystems[1].Name, subUnitSystems[0], subUnitSystems[1]);

            Array.Copy(subUnitSystems, 2, tailSubSystems, 1, subUnitSystems.Length -2);
            SetupCombinedUnitSystem(tailSubSystems);
            **/

            _unitSystemes = subUnitSystems;

            IUnitPrefix[] tempUnitprefixes = null;
            IBaseUnit[] tempBaseUnits = null;
            INamedDerivedUnit[] tempNamedDerivedUnits = null;
            IConvertibleUnit[] tempConvertibleUnits = null;

            foreach (IUnitSystem us in UnitSystemes)
            {

#if FrameworkVersion_4_6
                // CSharp_6 
                tempUnitprefixes = ArrayExtensions.Concat<IUnitPrefix>(tempUnitprefixes, us.UnitPrefixes?.UnitPrefixes);
#else
//#if FrameworkVersion_4_5_2 || FrameworkVersion_4_5_1 || FrameworkVersion_4_5
                // CSharp_5 
                if (us.UnitPrefixes != null)
                {
                    tempUnitprefixes = ArrayExtension.Concat<IUnitPrefix>(tempUnitprefixes, us.UnitPrefixes.UnitPrefixes);
                }
//#endif
#endif
                tempBaseUnits = ArrayExtensions.Concat<IBaseUnit>(tempBaseUnits, us.BaseUnits);
                tempNamedDerivedUnits = ArrayExtensions.Concat<INamedDerivedUnit>(tempNamedDerivedUnits, us.NamedDerivedUnits);
                tempConvertibleUnits = ArrayExtensions.Concat<IConvertibleUnit>(tempConvertibleUnits, us.ConvertibleUnits);
            }

            _unitprefixes = new UnitPrefixTable(tempUnitprefixes);

            _baseunits = tempBaseUnits;
            _namedderivedunits = tempNamedDerivedUnits;
            _convertibleunits = tempConvertibleUnits;
        }


        public SByte[] UnitExponents(ICombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] UnitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] UnitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                UnitSystemExponentsOffsets[index] = noOfDimensions;
                UnitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += UnitSystemExponentsLength[index];

            }

            SByte[] resExponents = new sbyte[0];

            // Split cu in to parts for each sub unit system in UnitSystemes
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems];

            // Split into subUnitParts
            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);
                SByte[] us_exponents = subUnitParts[i].Exponents;
                if (us_exponents.Length < UnitSystemExponentsLength[index])
                {
                    us_exponents = us_exponents.AllExponents(UnitSystemExponentsLength[index]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            return resExponents;
        }

        /*******
        public IPhysicalQuantity ConvertToBaseUnit(ICombinedUnit cu)
        {
            // Split cu in to parts for each sub unit system in UnitSystemes
            int noOfSubUnitSystems = UnitSystemes.Length;
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems];

            // Split into subUnitParts
            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);
            }

            // 
            Double resValue = 1;
            ICombinedUnit resBaseUnit = new CombinedUnit();
            // foreach (ICombinedUnit subCombinedUnit in subUnitParts)
            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                ICombinedUnit subCombinedUnit = subUnitParts[i];
                if (subCombinedUnit != null)
                {
                    IPhysicalQuantity pq = subCombinedUnit.ConvertToBaseUnit(UnitSystemes[i]);
                    resValue *= pq.Value;
                    resBaseUnit = resBaseUnit.CombinedUnitMultiply(pq.Unit);
                }
            }

            return new PhysicalQuantity(resValue, resBaseUnit);
        }
        *******/

        public IPhysicalQuantity ConvertToBaseUnit(ICombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] UnitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] UnitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                UnitSystemExponentsOffsets[index] = noOfDimensions;
                UnitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += UnitSystemExponentsLength[index];
                index++;
            }

            SByte[] resExponents = new sbyte[0];

            // Split cu in to parts for each sub unit system in UnitSystemes
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems + 1];

            // Split into subUnitParts
            Double resValue = cu.FactorValue;

            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);

                IPhysicalQuantity pq = subUnitParts[i].ConvertToBaseUnit(UnitSystemes[i]);
                resValue *= pq.Value;
                Debug.Assert(pq.Unit != null);
                SByte[] us_exponents = pq.Unit.Exponents;
                if (us_exponents.Length < UnitSystemExponentsLength[i])
                {
                    us_exponents = us_exponents.AllExponents(UnitSystemExponentsLength[i]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            // Handle part of cu without (sub-)unitsystem
            subUnitParts[noOfSubUnitSystems] = cu.OnlySingleSystemUnits(null); // no unit system

            IPhysicalQuantity pq2 = subUnitParts[noOfSubUnitSystems].ConvertToBaseUnit();
            resValue *= pq2.Value;

            IPhysicalUnit derivatedUnit = new DerivedUnit(this, resExponents);
            return new PhysicalQuantity(resValue, derivatedUnit);
        }

        public override int GetHashCode()
        {
            if (_unitSystemes == null)
            {
                return base.GetHashCode();
            }
            return _unitSystemes.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            ICombinedUnitSystem ICombinedUnitSystemObj = obj as ICombinedUnitSystem;
            if (ICombinedUnitSystemObj == null)
                return false;
            else
                return Equals(ICombinedUnitSystemObj);
        }

        public bool Equals(ICombinedUnitSystem other)
        {
            return Equals(this.UnitSystemes, other.UnitSystemes);
        }
    }

    #endregion Physical Unit System Classes

    #region Physical Unit System Conversion Classes

    public class UnitSystemConversion
    {
        private IUnitSystem _BaseUnitSystem;

        public IUnitSystem BaseUnitSystem
        {
            get { return _BaseUnitSystem; }
        }

        private IUnitSystem _ConvertedUnitSystem;

        public IUnitSystem ConvertedUnitSystem
        {
            get { return _ConvertedUnitSystem; }
        }

        public ValueConversion[] BaseUnitConversions;

        public UnitSystemConversion(IUnitSystem someBaseUnitsystem, IUnitSystem someConvertedUnitsystem, ValueConversion[] someBaseUnitConversions)
        {
            this._BaseUnitSystem = someBaseUnitsystem;
            this._ConvertedUnitSystem = someConvertedUnitsystem;
            this.BaseUnitConversions = someBaseUnitConversions;
        }

        public IPhysicalQuantity Convert(IPhysicalUnit convertUnit, bool backwards = false)
        {
            Debug.Assert(convertUnit.Kind == UnitKind.BaseUnit || convertUnit.Kind == UnitKind.DerivedUnit);

            SByte[] FromUnitExponents = convertUnit.Exponents;

            double convertproduct = 1;

            SByte NoOfNonZeroExponents = 0;
            SByte NoOfNonOneExponents = 0;
            SByte FirstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in FromUnitExponents)
            {
                if (exponent != 0)
                {
                    if (FirstNonZeroExponent == -1)
                    {
                        FirstNonZeroExponent = i;
                    }
                    NoOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        NoOfNonOneExponents++;
                    }
                    ValueConversion vc = BaseUnitConversions[i];
                    if (vc != null)
                    {
                        double baseunitconvertedvalue = vc.Convert(1, backwards);
                        double baseunitfactor = Math.Pow(baseunitconvertedvalue, exponent);
                        convertproduct = convertproduct * baseunitfactor;
                    }
                    else
                    {
                        /* throw new ArgumentException("object's physical unit is not convertible to a " + ConvertedUnitSystem.name + " unit. " + ConvertedUnitSystem.name + " does not "); */
                        return null;
                    }
                }

                i++;
            }
            double value = convertproduct;

            IUnitSystem unitsystem = (backwards ? BaseUnitSystem : ConvertedUnitSystem);

            IPhysicalUnit unit = unitsystem.UnitFromUnitInfo(FromUnitExponents, NoOfNonZeroExponents, NoOfNonOneExponents, FirstNonZeroExponent);
            return new PhysicalQuantity(value, unit);
        }

        public IPhysicalQuantity Convert(IPhysicalQuantity physicalQuantity, bool backwards = false)
        {
            Debug.Assert(physicalQuantity != null);

            IPhysicalQuantity pq = Convert(physicalQuantity.Unit, backwards);
            return new PhysicalQuantity(physicalQuantity.Value * pq.Value, pq.Unit);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalUnit convertUnit)
        {
            return Convert(convertUnit, false);
        }

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalUnit convertUnit)
        {
            return Convert(convertUnit, true);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalQuantity physicalQuantity)
        {
            return Convert(physicalQuantity, false);
        }

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalQuantity physicalQuantity)
        {
            return Convert(physicalQuantity, true);
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertFromUnit != null);

            if ((convertFromUnit.SimpleSystem == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(convertFromUnit);
            }
            else
                if ((convertFromUnit.SimpleSystem == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(convertFromUnit);
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(physicalQuantity != null);

            if ((physicalQuantity.Unit.SimpleSystem == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(physicalQuantity);
            }
            else
                if ((physicalQuantity.Unit.SimpleSystem == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(physicalQuantity);
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IPhysicalQuantity pq = this.ConvertTo(convertFromUnit, convertToUnit.SimpleSystem);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnit);
            }
            return pq;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IPhysicalQuantity pq = this.ConvertTo(physicalQuantity, convertToUnit.SimpleSystem);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnit);
            }
            return pq;
        }
    }

    #endregion Physical Unit System Conversions

    #region Physical Quantity Classes

    public class PhysicalQuantity : IPhysicalQuantity
    {
        // The value holders
        private readonly double _value;
        private readonly IPhysicalUnit _unit;

        public double Value
        {
            get
            {
                return this._value;
            }
        }

        public IPhysicalUnit Unit
        {
            get
            {
                return this._unit;
            }
        }

        public IPhysicalUnit Dimensionless { get { return Unit.Dimensionless; } }
        public Boolean IsDimensionless { get { return _unit == null || _unit.IsDimensionless; } }

        public PhysicalQuantity()
            : this(0)
        {
        }

        public PhysicalQuantity(double somevalue)
            : this(somevalue, Physics.dimensionless)
        {
        }

        public PhysicalQuantity(IPhysicalUnit someunit)
            : this(1, someunit)
        {
        }

        public PhysicalQuantity(double somevalue, IPhysicalUnit someunit)
        {
            this._value = somevalue;
            this._unit = someunit;
        }

        public PhysicalQuantity(IPhysicalQuantity somephysicalquantity)
        {
            if (somephysicalquantity != null)
            {
                this._value = somephysicalquantity.Value;
                this._unit = somephysicalquantity.Unit;
            }
            else
            {
                this._value = 0;
                this._unit = Physics.dimensionless;
            }
        }

        public PhysicalQuantity(double somevalue, IPhysicalQuantity somephysicalquantity)
            : this(somevalue * somephysicalquantity.Value, somephysicalquantity.Unit)
        {
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is IPhysicalQuantity)
            {
                IPhysicalQuantity temp = (IPhysicalQuantity)obj;

                IPhysicalQuantity tempconverted = temp.ConvertTo(this.Unit);
                if (tempconverted != null)
                {
                    return _value.EpsilonCompareTo(tempconverted.Value);
                }

                throw new ArgumentException("object's physical unit " + temp.Unit.ToPrintString() + " is not convertible to a " + this.Unit.ToPrintString());
            }

            throw new ArgumentException("object is not a IPhysicalQuantity");
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public String ToString(String format, IFormatProvider formatProvider)
        {
            double UnitValue = this.Unit.FactorValue;
            IUnit PureUnit = this.Unit.PureUnit;
            String ValStr = PureUnit.ValueString(this.Value * UnitValue, format, formatProvider);
            String UnitStr = PureUnit.ToString();
            if (String.IsNullOrEmpty(UnitStr))
            {
                return ValStr;
            }
            else
            {
                return ValStr + " " + UnitStr;
            }
        }

        public override String ToString()
        {
            double UnitValue = this.Unit.FactorValue;
            IUnit PureUnit = this.Unit.PureUnit;
            String ValStr = PureUnit.ValueString(this.Value * UnitValue);
            String UnitStr = PureUnit.ToString();
            if (String.IsNullOrEmpty(UnitStr))
            {
                return ValStr;
            }
            else
            {
                return ValStr + " " + UnitStr;
            }
        }

        public virtual String ToPrintString()
        {
            double UnitValue = this.Unit.FactorValue;
            IUnit PureUnit = this.Unit.PureUnit;
            String ValStr = PureUnit.ValueString(this.Value * UnitValue);
            String UnitStr = PureUnit.ToString();

            if (String.IsNullOrEmpty(UnitStr))
            {
                return ValStr;
            }
            else
            {
                return ValStr + " " + UnitStr;
            }
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitsymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        //public static IPhysicalQuantity Parse(String physicalQuantityStr, System.Globalization.NumberStyles styles, IFormatProvider provider)
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, IFormatProvider provider, out PhysicalQuantity result)
        {
            result = null;

            String[] Strings = physicalQuantityStr.Trim().Split(' ');

            if (Strings.GetLength(0) > 0)
            {
                // Parse numerical value
                String NumValueStr = Strings[0];
                Double NumValue;

                //if (!Double.TryParse(NumValueStr, styles, provider, out NumValue) && provider != null)
                if (!Double.TryParse(NumValueStr, styles, provider, out NumValue))
                {
                    if (!Double.TryParse(NumValueStr, styles, null, out NumValue)) // Try  to use Default Format Provider
                    {
                        NumValue = Double.Parse(NumValueStr, styles, NumberFormatInfo.InvariantInfo);     // Try  to use invariant Format Provider
                    }
                }

                IPhysicalUnit unit = null;

                if (Strings.GetLength(0) > 1)
                {
                    // Parse unit
                    String UnitStr = Strings[1];
                    unit = PhysicalUnit.Parse(UnitStr);
                }
                else
                {
                    unit = Physics.dimensionless;
                }

                result = new PhysicalQuantity(NumValue, unit);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitSymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        //public static IPhysicalQuantity Parse(String physicalQuantityStr)
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, out PhysicalQuantity result)
        {
            //return Parse(physicalQuantityStr, System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            return TryParse(physicalQuantityStr, styles, null, out result);
        }

        public static Boolean TryParse(String physicalQuantityStr, out PhysicalQuantity result)
        {
            //return Parse(physicalQuantityStr, System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            return TryParse(physicalQuantityStr, System.Globalization.NumberStyles.Float, null, out result);
        }

        public static PhysicalQuantity Parse(String physicalQuantityStr, System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Float, IFormatProvider provider = null)
        {
            PhysicalQuantity result;
            if (!TryParse(physicalQuantityStr, styles, provider, out result))
            {
                throw new ArgumentException("Not a valid physical quantity format", "physicalQuantityStr");
            }

            return result;
        }


        public IPhysicalQuantity Zero { get { return new PhysicalQuantity(0, this.Unit.Dimensionless); } }
        public IPhysicalQuantity One { get { return new PhysicalQuantity(1, this.Unit.Dimensionless); } }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() + this.Unit.GetHashCode();
        }

        public IPhysicalQuantity ConvertToSystemUnit()
        {
            /*
            TO DO: IsSystemUnit() is not implemented yet
            if (this.Unit.IsSystemUnit())
            */
            if (this.Unit.SimpleSystem != null)
            {
                return this;
            }

            double d = this.Value;
            IPhysicalQuantity pq = this.Unit.ConvertToSystemUnit(ref d);
            return pq;
        }

        public IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = this.Unit.ConvertToBaseUnit(this.Value);
            return pq;
        }

        public IPhysicalQuantity ConvertToDerivedUnit()
        {
            IPhysicalQuantity pq_baseunit = this.Unit.ConvertToBaseUnit(this.Value);
            IPhysicalQuantity pq_derivedunit = pq_baseunit.Unit.ConvertToDerivedUnit().Multiply(pq_baseunit.Value);
            return pq_derivedunit;
        }


        /**
        public IUnitSystem UnitSystem()
        {
            Debug.Assert(this.Unit != null);

            if (this.Unit == null)
            {
                // return null;
                throw new InvalidOperationException("Must have a unit to get a unit system");
            }

            return this.Unit.System;
        }
        **/

        // Auto detecting if specific or relative unit conversion 


        public IPhysicalQuantity this[IPhysicalUnit convertToUnit]
        {
            get { return this.ConvertTo(convertToUnit); }
        }

        public IPhysicalQuantity this[IPhysicalQuantity convertToUnit]
        {
            get { return this.ConvertTo(convertToUnit); }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            //if (this.Unit == convertToUnit)
            if (Object.ReferenceEquals(this.Unit, convertToUnit))
            {
                // Convert to its own unit; No conversion needed
                return this;
            }

            if (this.Unit == null)
            {
                if (convertToUnit == null || (convertToUnit.IsDimensionless)) // || convertToUnit == 1)) // One ))
                {   // Any dimensionless can be converted to any systems dimensionless
                    Debug.Assert(convertToUnit != null);
                    IPhysicalQuantity quantity = new PhysicalQuantity(this.Value * 1 / convertToUnit.FactorValue, convertToUnit);
                    return quantity;
                }
                else
                {   // No dimensionless can be converted to or from any non dimensionless.
                    return null;
                }
            }

            Debug.Assert(this.Unit != null);
            Debug.Assert(convertToUnit != null);

            if (convertToUnit == null)
            {
                throw new ArgumentNullException("convertToUnit");
            }

            if (this.Unit.Kind != UnitKind.CombinedUnit && convertToUnit.Kind != UnitKind.CombinedUnit)
            {
                bool thisIsDimensionless = this.Unit.IsDimensionless;
                bool toIsDimensionless = convertToUnit.IsDimensionless;
                if (thisIsDimensionless != toIsDimensionless)
                {   // No dimensionless can be converted to or from any non-dimensionless.
                    return null;
                }
                if (thisIsDimensionless && toIsDimensionless)
                {
                    // Any dimensionless can be converted to any systems dimensionless
                    IPhysicalQuantity quantity = new PhysicalQuantity(this.Value * this.Unit.FactorValue / convertToUnit.FactorValue, convertToUnit);
                    return quantity;
                }
            }
            else
            if (this.Unit.Kind == UnitKind.CombinedUnit && convertToUnit.Kind == UnitKind.CombinedUnit)
            {

            }
            else
            {


            }

            /**
            This test for equality results in infinite recursive calls of Equals(convertToUnit) and ConvertTo(convertToUnit)
            if (this.Unit.Equals(convertToUnit))
            {
                // Equal units must have same value 
                IPhysicalQuantity quantity = new PhysicalQuantity(this.Value, convertToUnit);
                return quantity;
            }
            **/

            IUnitSystem convertToUnitsystem = convertToUnit.SimpleSystem;
            if (convertToUnitsystem == null)
            {
                convertToUnitsystem = convertToUnit.ExponentsSystem;
                Debug.WriteLine("convertToUnitsystem assigned from convertToUnit.ExpresionsSystem");
            }

            Debug.Assert(convertToUnitsystem != null);
            if (convertToUnitsystem == null)
            {
                convertToUnitsystem = this.Unit.SimpleSystem;
                Debug.WriteLine("convertToUnitsystem assigned from this.Unit.System");
            }
            if (convertToUnitsystem == null)
            {
                Debug.WriteLine("convertToUnitsystem assigned from Physics.Default_UnitSystem");
                convertToUnitsystem = Physics.CurrentUnitSystems.Default;
            }

            if (convertToUnitsystem != null)
            {
                // Let convertToUnitsystem do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertToUnitsystem.ConvertTo(this as IPhysicalQuantity, convertToUnit);
                return quantity;
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity convertToUnit)
        {
            return this.ConvertTo(convertToUnit.Unit);
        }

        public IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;

            if (convertFromUnitsystem == convertToUnitSystem)
            {
                return this;
            }

            Debug.Assert(this.Unit != null);
            Debug.Assert(convertToUnitSystem != null);

            if (this.Unit == null)
            {
                throw new InvalidOperationException("Must have a unit to convert it");
            }

            if (convertToUnitSystem == null)
            {
                throw new ArgumentNullException("convertToUnitSystem");
            }

            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertFromUnitsystem.ConvertTo(this as IPhysicalQuantity, convertToUnitSystem);
                return quantity;
            }

            return null;
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)1
        public IPhysicalQuantity RelativeConvertTo(IPhysicalUnit convertToUnit)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertFromUnitsystem.ConvertTo(this.Unit, convertToUnit).Multiply(this.Value);
                return quantity;
            }
            return null;
        }

        public IPhysicalQuantity RelativeConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertFromUnitsystem.ConvertTo(this.Unit, convertToUnitSystem).Multiply(this.Value);
                return quantity;
            }
            return null;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        public IPhysicalQuantity SpecificConvertTo(IPhysicalUnit convertToUnit)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertFromUnitsystem.ConvertTo(this as IPhysicalQuantity, convertToUnit);
                return quantity;
            }
            return null;
        }

        public IPhysicalQuantity SpecificConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion 
                IPhysicalQuantity quantity = convertFromUnitsystem.ConvertTo(this as IPhysicalQuantity, convertToUnitSystem);
                return quantity;
            }
            return null;
        }


        public static Boolean IsPureUnit(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            return physicalQuantity.Value == 1;
        }

        public static IPhysicalUnit PureUnit(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            if (IsPureUnit(physicalQuantity))
            {
                return physicalQuantity.Unit;
            }

            Double prefixExponentD = Math.Log10(physicalQuantity.Value);
            SByte prefixExponent = (SByte)Math.Floor(prefixExponentD);
            if (prefixExponentD - prefixExponent == 0)
            {
                IUnitPrefix unitPrefix;
                if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent(prefixExponent), out unitPrefix))
                {
                    INamedSymbolUnit namedSymbolUnit = physicalQuantity.Unit as INamedSymbolUnit;
                    if (namedSymbolUnit != null)
                    {
                        return new PrefixedUnit(unitPrefix, namedSymbolUnit);
                    }
                }
            }

            throw new ArgumentException("Physical quantity is not a pure unit; but has a value = " + physicalQuantity.Value.ToString());

            //return null;
        }

        public static IPhysicalUnit operator !(PhysicalQuantity physicalQuantity)
        {
            return PureUnit(physicalQuantity);
        }

        public static implicit operator PhysicalUnit(PhysicalQuantity physicalQuantity)
        {
            return PureUnit(physicalQuantity) as PhysicalUnit;
        }

        /*
        public static implicit operator IPhysicalQuantity(PhysicalQuantity pq)
        {
            return pq as IPhysicalQuantity;
        }
        */

        public bool Equivalent(IPhysicalQuantity other, out double quotient)
        {
            Debug.Assert(other != null);
            Debug.Assert(this.Unit != null);
            Debug.Assert(other.Unit != null);

            if (Object.ReferenceEquals(null, other))
            {
                quotient = 0;
                return false;
            }

            IPhysicalQuantity other2 = other.ConvertTo(this.Unit);
            if (Object.ReferenceEquals(null, other2))
            {
                quotient = 0;
                return false;
            }

            quotient = this.Value / other2.Value;
            return true;
        }

        public bool Equals(IPhysicalQuantity other)
        {
            Debug.Assert(other != null);
            Debug.Assert(this.Unit != null);
            Debug.Assert(other.Unit != null);

            if (Object.ReferenceEquals(null, other))
            {
                return false;
            }

            IPhysicalQuantity other2 = other.ConvertTo(this.Unit);
            if (Object.ReferenceEquals(null, other2))
            {
                return false;
            }
            return this.Value.EpsilonCompareTo(other2.Value) == 0;
        }


        public bool Equals(IPhysicalUnit other)
        {
            IPhysicalQuantity otherPhysicalQuantity = new PhysicalQuantity(1, other);
            return this.Equals(otherPhysicalQuantity);
        }

        public bool Equals(double other)
        {
            IPhysicalQuantity otherPhysicalQuantity = new PhysicalQuantity(other);
            return this.Equals(otherPhysicalQuantity);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            IPhysicalQuantity pq = obj as IPhysicalQuantity;
            if (pq != null)
            {
                return Equals(pq);
            }

            IPhysicalUnit pu = obj as IPhysicalUnit;
            if (pu != null)
            {
                return Equals(pu);
            }

            //throw new InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object or IPhysicalUnit object.");
            Debug.Assert(obj is IPhysicalQuantity || obj is IPhysicalUnit);

            return false;
        }

        public static bool operator ==(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.Equals(pq2);
        }

        public static bool operator !=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return !pq1.Equals(pq2);
        }

        public static bool operator <(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) < 0;
        }

        public static bool operator <=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) <= 0;
        }

        public static bool operator >(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) > 0;
        }

        public static bool operator >=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) >= 0;
        }

        #region Physical Quantity static operator methods

        internal delegate double CombineValuesFunc(double v1, double v2);
        internal delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);

        internal static PhysicalQuantity CombineValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf)
        {
            if (pq1.Unit != pq2.Unit)
            {
#if DEBUG
                pq1.Unit.TestPropertiesPrint();
#endif
                IPhysicalQuantity temp_pq2 = pq2.ConvertTo(pq1.Unit);
                if (temp_pq2 == null)
                {
                    throw new ArgumentException("object's physical unit " + pq2.Unit.ToPrintString() + " is not convertible to unit " + pq1.Unit.ToPrintString());
                }

                pq2 = temp_pq2;
            }
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pq1.Unit);
        }

        internal static PhysicalQuantity CombineUnitsAndValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf, PhysicalUnit.CombineExponentsFunc cef)
        {
            Debug.Assert(pq1.Unit.Kind != UnitKind.CombinedUnit);
            Debug.Assert(pq2.Unit.Kind != UnitKind.CombinedUnit);

            if (pq1.Unit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit pg1_unit = pq1.Unit as ICombinedUnit;
                pq1 = pq1.ConvertToSystemUnit();
            }

            if (pq2.Unit.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit pg2_unit = pq2.Unit as ICombinedUnit;
                pq2 = pq2.ConvertToSystemUnit();
            }

            while (pq1.Unit.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit pg1_unit = pq1.Unit as IConvertibleUnit;
                pq1 = pq1.ConvertTo(pg1_unit.PrimaryUnit);
            }
            while (pq2.Unit.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit pg2_unit = pq2.Unit as IConvertibleUnit;
                pq2 = pq2.ConvertTo(pg2_unit.PrimaryUnit);
            }

            IUnitSystem pq2UnitSystem = pq2.Unit.SimpleSystem;
            IUnitSystem pq1UnitSystem = pq1.Unit.SimpleSystem;
            if (pq2UnitSystem != pq1UnitSystem)
            {   // Must be same unit system
                pq2 = pq2.ConvertTo(pq1UnitSystem);
                Debug.Assert(pq2 != null);
            }

            SByte MinNoOfBaseUnits = (SByte)Math.Min(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            Debug.Assert(MaxNoOfBaseUnits <= Physics.NoOfBaseQuanties);

            SByte[] someexponents = new SByte[Physics.NoOfBaseQuanties];

            for (int i = 0; i < MinNoOfBaseUnits; i++)
            {
                someexponents[i] = cef(pq1.Unit.Exponents[i], pq2.Unit.Exponents[i]);
            }

            for (int i = MinNoOfBaseUnits; i < MaxNoOfBaseUnits; i++)
            {
                if (pq1.Unit.Exponents.Length > pq2.Unit.Exponents.Length)
                {
                    someexponents[i] = cef(pq1.Unit.Exponents[i], 0);
                }
                else
                {
                    someexponents[i] = cef(0, pq2.Unit.Exponents[i]);
                }
            }

            Debug.Assert(pq1.Unit.ExponentsSystem != null);
            PhysicalUnit pu = new DerivedUnit(pq1.Unit.ExponentsSystem, someexponents);
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pu);
        }

        public static PhysicalQuantity operator +(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return new PhysicalQuantity(pq1.Add(pq2));
        }

        public static PhysicalQuantity operator -(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return new PhysicalQuantity(pq1.Subtract(pq2));
        }

        public static PhysicalQuantity operator *(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            IPhysicalQuantity pq = pq1.Unit.Multiply(pq1.Value, pq2);
            return new PhysicalQuantity(pq);
        }

        public static PhysicalQuantity operator /(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            IPhysicalQuantity pq = pq1.Unit.Divide(pq1.Value, pq2);
            return new PhysicalQuantity(pq);
        }

        public static PhysicalQuantity operator *(PhysicalQuantity pq, IUnitPrefix up)
        {
            return new PhysicalQuantity(pq.Value * up.Value, pq.Unit);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(pq.Value * up.Value, pq.Unit);
        }

        public static PhysicalQuantity operator *(PhysicalQuantity pq, double d)
        {
            return new PhysicalQuantity(pq.Value * d, pq.Unit);
        }

        public static PhysicalQuantity operator /(PhysicalQuantity pq, double d)
        {
            return new PhysicalQuantity(pq.Value / d, pq.Unit);
        }

        public static PhysicalQuantity operator *(double d, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(pq.Multiply(d));
        }

        public static PhysicalQuantity operator /(double d, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(new PhysicalQuantity(d).Divide(pq));
        }

        public static PhysicalQuantity operator *(PhysicalQuantity pq, IPhysicalUnit pu)
        {
            return new PhysicalQuantity(pq.Value, pq.Unit.Multiply(pu));
        }

        public static PhysicalQuantity operator /(PhysicalQuantity pq, IPhysicalUnit pu)
        {
            return new PhysicalQuantity(pq.Value, pq.Unit.Divide(pu));
        }

        public static PhysicalQuantity operator *(IPhysicalUnit pu, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(pq.Value, pu.Multiply(pq.Unit));
        }

        public static PhysicalQuantity operator /(IPhysicalUnit pu, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(pq.Value, pu.Divide(pq.Unit));
        }

        public static PhysicalQuantity operator ^(PhysicalQuantity pq, SByte exponent)
        {
            return pq.Power(exponent);
        }

        public static PhysicalQuantity operator %(PhysicalQuantity pq, SByte exponent)
        {
            return pq.Root(exponent);
        }

        public static PhysicalQuantity operator |(PhysicalQuantity pq, SByte exponent)
        {
            return pq.Root(exponent);
        }

        #endregion Physical Quantity static operator methods

        public PhysicalQuantity Power(SByte exponent)
        {
            IPhysicalUnit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            /*
            IPhysicalQuantity pq = pu.Pow(exponent);
            Double Value = pq.Value * System.Math.Pow(this.Value, exponent);
            return new PhysicalQuantity(Value, pq.Unit);
            */
            IPhysicalUnit pu_pow = pu.Pow(exponent);
            Double Value = System.Math.Pow(this.Value, exponent);
            return new PhysicalQuantity(Value, pu_pow);
        }

        public PhysicalQuantity Root(SByte exponent)
        {
            IPhysicalUnit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            // IPhysicalQuantity pq = pu.Rot(exponent);
            //pq.Value = pq.Value * System.Math.Root(this.Value, exponent);
            // Double Value = pq.Value * System.Math.Pow(this.Value, 1.0 / exponent);
            //return new PhysicalQuantity(Value, pq.Unit);  

            IPhysicalUnit pu_rot = pu.Rot(exponent);
            Double Value = System.Math.Pow(this.Value, 1.0 / exponent);
            return new PhysicalQuantity(Value, pu_rot);
        }

        #region Physical Quantity IPhysicalUnitMath implementation

        public IPhysicalQuantity Add(IPhysicalQuantity physicalQuantity)
        {
            return CombineValues(this, physicalQuantity, (double v1, double v2) => v1 + v2);
        }

        public IPhysicalQuantity Subtract(IPhysicalQuantity physicalQuantity)
        {
            return CombineValues(this, physicalQuantity, (double v1, double v2) => v1 - v2);
        }


        public IPhysicalQuantity Multiply(INamedSymbolUnit physicalUnit)
        {
            //return this.Multiply(new PrefixedUnitExponent(0, physicalUnit, 1));
            return this.Multiply(new PrefixedUnitExponent(null, physicalUnit, 1));
        }

        public IPhysicalQuantity Divide(INamedSymbolUnit physicalUnit)
        {
            //return this.Divide(new PrefixedUnitExponent(0, physicalUnit, 1));
            return this.Divide(new PrefixedUnitExponent(null, physicalUnit, 1));
        }

        public IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            //return this.Multiply(new PrefixedUnitExponent(0, physicalUnit, 1));
            //return this.Multiply(new PrefixedUnitExponent(null, physicalUnit, 1));
            IPhysicalQuantity pq = this.Unit.Multiply(physicalUnit).Multiply(this.Value);
            return pq;
        }

        public IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            //return this.Divide(new PrefixedUnitExponent(0, physicalUnit, 1));
            // return this.Divide(new PrefixedUnitExponent(null, physicalUnit, 1));
            IPhysicalQuantity pq = this.Unit.Divide(physicalUnit).Multiply(this.Value);
            return pq;
        }

        public IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(this.Value, physicalQuantity);
            return pq;
        }

        public IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Divide(this.Value, physicalQuantity);
            return pq;
        }

        public IPhysicalQuantity Multiply(double quantity)
        {
            return new PhysicalQuantity(this.Value * quantity, this.Unit);
        }

        public IPhysicalQuantity Divide(double quantity)
        {
            return new PhysicalQuantity(this.Value / quantity, this.Unit);
        }

        public IPhysicalQuantity Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        public IPhysicalQuantity Rot(SByte exponent)
        {
            return this.Root(exponent);
        }

        public IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit)
        {
            /*
            IPhysicalQuantity pq = this.Unit.Multiply(prefixedUnit);
            IPhysicalUnit pq = this.Unit.Multiply(prefixedUnit);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
            */
            IPhysicalUnit pu = this.Unit.Multiply(prefixedUnit);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit)
        {
            /*
            IPhysicalQuantity pq = this.Unit.Divide(prefixedUnit);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
            */
            IPhysicalUnit pu = this.Unit.Divide(prefixedUnit);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            /*
            IPhysicalQuantity pq = this.Unit.Multiply(prefixedUnitExponent);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
            */
            IPhysicalUnit pu = this.Unit.Multiply(prefixedUnitExponent);
            return new PhysicalQuantity(Value, pu);
        }

        public IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            /*
            IPhysicalQuantity pq = this.Unit.Divide(prefixedUnitExponent);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
            */
            IPhysicalUnit pu = this.Unit.Divide(prefixedUnitExponent);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Multiply(IUnitPrefixExponent prefix)
        {
            IPhysicalUnit pu = this.Unit.Multiply(prefix);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Divide(IUnitPrefixExponent prefix)
        {
            IPhysicalUnit pu = this.Unit.Divide(prefix);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(this.Value * quantity, physicalQuantity);
            return pq;
        }

        public IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Divide(quantity, physicalQuantity).Multiply(this.Value);
            return pq;
        }

        #endregion Physical Quantity IPhysicalUnitMath implementation        
    }

    #endregion Physical Quantity Classes


    public class UnitSystemStack
    {
        protected Stack<IUnitSystem> Default_UnitSystem_Stack = new Stack<IUnitSystem>();

        public IUnitSystem Default
        {
            get
            {
                if (Default_UnitSystem_Stack == null || Default_UnitSystem_Stack.Count <= 0)
                {
                    return Physics.SI_Units;
                }
                else
                {
                    return Default_UnitSystem_Stack.Peek();
                }
            }
        }

        public bool Use(IUnitSystem NewUnitSystem)
        {
            if (Default != NewUnitSystem)
            {
                Default_UnitSystem_Stack.Push(NewUnitSystem);
                return true;
            }
            return false;
        }

        public bool Unuse(IUnitSystem OldUnitSystem)
        {
            if (Default_UnitSystem_Stack != null && Default_UnitSystem_Stack.Count > 0 && Default_UnitSystem_Stack.Peek() == OldUnitSystem)
            {
                Default_UnitSystem_Stack.Pop();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Default_UnitSystem_Stack.Clear();
        }
    }

    public class UnitLookup //(UnitSystem[] unitSystems)
    {
        protected UnitSystem[] UnitSystems;

        public UnitLookup(UnitSystem[] unitSystems)
        {
            UnitSystems = unitSystems;
        }

        public IPhysicalUnit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromName(namestr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public IPhysicalUnit UnitFromSymbol(String symbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromSymbol(symbolstr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public IPhysicalUnit ScaledUnitFromSymbol(String scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalUnit scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }

        public IUnitSystem UnitSystemFromName(String UnitSystemsymbolstr)
        {
            IUnitSystem result_us = UnitSystems.FirstOrNull<IUnitSystem>(us => us.Name == UnitSystemsymbolstr);
            return result_us;
        }
    }

    public class UnitSystemConversionLookup
    {

        public IList<UnitSystemConversion> UnitSystemConversions;

        public UnitSystemConversionLookup(IList<UnitSystemConversion> unitSystemConversions)
        {
            UnitSystemConversions = unitSystemConversions;
        }

        public UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            UnitSystemConversion usc = GetDirectUnitSystemConversion(unitsystem1, unitsystem2);
            if (usc != null)
            {
                return usc;
            }

            /*  No direct unit system conversion from  unitsystem1 to unitsystem2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */
            return GetIntermediateUnitSystemConversion(unitsystem1, unitsystem2);

            /**
            IList<IUnitSystem> OldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> OldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            usc = GetUnitSystemConversion(OldUnitsystems1, NewUnitSystemsConvertableToUnitsystems1, OldUnitsystems2, NewUnitSystemsConvertableToUnitsystems2, unitSystemConversions);
            return usc;
            **/
        }

        public UnitSystemConversion GetDirectUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                if ((usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                    || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                {
                    return usc;
                }
            }

            return null;
        }

        public UnitSystemConversion GetIntermediateUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            /*  No direct unit system conversion from  unitsystem1 to unitsystem2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */

            IList<IUnitSystem> OldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> OldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            return GetIntermediateUnitSystemConversion(UnitSystemConversions, OldUnitsystems1, NewUnitSystemsConvertableToUnitsystems1, OldUnitsystems2, NewUnitSystemsConvertableToUnitsystems2);
        }


        public UnitSystemConversion GetIntermediateUnitSystemConversion(IList<UnitSystemConversion> unitSystemConversions,
                                                                        IList<IUnitSystem> oldUnitsystems1, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems1,
                                                                        IList<IUnitSystem> oldUnitsystems2, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems2)
        {
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>();
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>();

            IList<UnitSystemConversion> unitsystems1Conversions = new List<UnitSystemConversion>();
            IList<UnitSystemConversion> unitsystems2Conversions = new List<UnitSystemConversion>();

            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                Boolean BUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.ConvertedUnitSystem);

                Boolean BUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.ConvertedUnitSystem);

                if (BUS_in_US1 || CUS_in_US1)
                {
                    if (BUS_in_US2 || CUS_in_US2)
                    {
                        return usc;
                    }

                    Debug.Assert(!unitsystems1Conversions.Contains(usc));
                    unitsystems1Conversions.Add(usc);

                    if (!(BUS_in_US1 && CUS_in_US1))
                    {
                        if (BUS_in_US1)
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.BaseUnitSystem);
                        }
                    }
                }
                else if (BUS_in_US2 || CUS_in_US2)
                {
                    Debug.Assert(!unitsystems2Conversions.Contains(usc));
                    unitsystems2Conversions.Add(usc);

                    if (!(BUS_in_US2 && CUS_in_US2))
                    {
                        if (BUS_in_US2)
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.BaseUnitSystem);
                        }
                    }
                }
            }

            /*  No direct unit system conversion from  unitsystems1 to unitsystems2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystems1 and unitsystems2 */

            if (unitSystemsConvertableToUnitsystems1.Count > 0 || unitSystemsConvertableToUnitsystems2.Count > 0)
            {
                IList<IUnitSystem> unitsystems1 = (IList<IUnitSystem>)oldUnitsystems1.Union(newUnitSystemsConvertableToUnitsystems1).ToList();
                IList<IUnitSystem> unitsystems2 = (IList<IUnitSystem>)oldUnitsystems2.Union(newUnitSystemsConvertableToUnitsystems2).ToList();

                UnitSystemConversion subIntermediereUnitSystemConversion = null;

                IList<IUnitSystem> IntersectUnitsystemsList = unitSystemsConvertableToUnitsystems1.Intersect(unitSystemsConvertableToUnitsystems2).ToList();

                if (IntersectUnitsystemsList.Count > 0)
                {
                    IUnitSystem IntersectUnitsystem = IntersectUnitsystemsList[0];
                    subIntermediereUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, newUnitSystemsConvertableToUnitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { IntersectUnitsystem });
                    Debug.Assert(subIntermediereUnitSystemConversion != null);
                }
                else
                {
                    IList<UnitSystemConversion> notIntermediereUnitSystemConversions = (IList<UnitSystemConversion>)unitSystemConversions.Except(unitsystems1Conversions.Union(unitsystems2Conversions)).ToList();
                    if (notIntermediereUnitSystemConversions.Count > 0)
                    {
                        subIntermediereUnitSystemConversion = GetIntermediateUnitSystemConversion(notIntermediereUnitSystemConversions, unitsystems1, unitSystemsConvertableToUnitsystems1, unitsystems2, unitSystemsConvertableToUnitsystems2);
                    }
                }
                if (subIntermediereUnitSystemConversion != null)
                {
                    if (!unitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from some unit system in unitsystems1 to one of subIntermediereUnitSystemConversion's systems
                        // Find Pre UnitSystemConversion

                        IUnitSystem CombinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion SecondUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean SecondValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem);
                        if (!SecondValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            CombinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            CombinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion FirstUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, unitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem });
                        Boolean FirstValueConversionDirectionInverted = FirstUnitSystemConversion.BaseUnitSystem == CombinedUnitSystemConversionIntermedierUnitSystem;

                        if (!FirstValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = FirstUnitSystemConversion.BaseUnitSystem;
                        }
                        else
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = FirstUnitSystemConversion.ConvertedUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[0], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[0], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[1], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[1], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[2], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[2], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[3], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[3], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[4], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[4], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[5], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[5], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[6], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[6], SecondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(CombinedUnitSystemConversionBaseUnitSystem, CombinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }

                    if (!unitsystems2.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from one of subIntermediereUnitSystemConversion's systems to some unit system in unitsystems2
                        // Find Post UnitSystemConversion

                        IUnitSystem CombinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion FirstUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean FirstValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem);
                        if (!FirstValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem) || unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));

                            CombinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion SecondUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems2Conversions, new List<IUnitSystem>() { }, new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem }, new List<IUnitSystem>() { }, unitsystems2);
                        Boolean SecondValueConversionDirectionInverted = SecondUnitSystemConversion.ConvertedUnitSystem == CombinedUnitSystemConversionIntermedierUnitSystem;

                        if (!SecondValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionConvertedUnitSystem = SecondUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            CombinedUnitSystemConversionConvertedUnitSystem = SecondUnitSystemConversion.BaseUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[0], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[0], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[1], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[1], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[2], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[2], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[3], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[3], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[4], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[4], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[5], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[5], SecondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[6], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[6], SecondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(CombinedUnitSystemConversionBaseUnitSystem, CombinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }
                    return subIntermediereUnitSystemConversion;
                }
            }

            return null;
        }

        /***
        public static IPhysicalUnit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in UnitSystems.)
            {
                INamedSymbolUnit unit = us.UnitFromName(namestr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalUnit UnitFromSymbol(String symbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromSymbol(symbolstr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalUnit ScaledUnitFromSymbol(String scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalUnit scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }

        public static IUnitSystem UnitSystemFromName(String UnitSystemsymbolstr)
        {
            IUnitSystem result_us = UnitSystems.FirstOrNull<IUnitSystem>(us => us.Name == UnitSystemsymbolstr);
            return result_us;
        }
        ***/

    }
    #region Physical Measure Static Classes

    public static partial class Physics
    {
        /*  http://en.wikipedia.org/wiki/SI_prefix 
            The International System of Units specifies twenty SI prefixes:

            SI prefixes   
                Prefix Symbol   1000^m      10^n    Decimal                     Short scale     Long scale      Since
                yotta   Y       1000^8      10^24   1000000000000000000000000   Septillion      Quadrillion     1991 
                zetta   Z       1000^7      10^21   1000000000000000000000      Sextillion      Trilliard       1991 
                exa     E       1000^6      10^18   1000000000000000000         Quintillion     Trillion        1975 
                peta    P       1000^5      10^15   1000000000000000            Quadrillion     Billiard        1975 
                tera    T       1000^4      10^12   1000000000000               Trillion        Billion         1960 
                giga    G       1000^3      10^9    1000000000                  Billion         Milliard        1960 
                mega    M       1000^2      10^6    1000000                             Million                 1960 
                kilo    k       1000^1      10^3    1000                                Thousand                1795 
                hecto   h       1000^2⁄3    10^2    100                                 Hundred                 1795 
                deca    da      1000^1⁄3    10^1    10                                  Ten                     1795 
                                1000^0      10^0    1                                   One  
                deci    d       1000^−1⁄3   10^−1   0.1                                 Tenth                   1795 
                centi   c       1000^−2⁄3   10^−2   0.01                                Hundredth               1795 
                milli   m       1000^−1     10^−3   0.001                               Thousandth              1795 
                micro   μ       1000^−2     10^−6   0.000001                            Millionth               1960 
                nano    n       1000^−3     10^−9   0.000000001                 Billionth       Milliardth      1960 
                pico    p       1000^−4     10^−12  0.000000000001              Trillionth      Billionth       1960 
                femto   f       1000^−5     10^−15  0.000000000000001           Quadrillionth   Billiardth      1964 
                atto    a       1000^−6     10^−18  0.000000000000000001        Quintillionth   Trillionth      1964 
                zepto   z       1000^−7     10^−21  0.000000000000000000001     Sextillionth    Trilliardth     1991 
                yocto   y       1000^−8     10^−24  0.000000000000000000000001  Septillionth    Quadrillionth   1991 
        */

        public static readonly UnitPrefixTable UnitPrefixes = new UnitPrefixTable(new UnitPrefix[] {new UnitPrefix(UnitPrefixes, "yotta", 'Y', 24), 
                                                                                                    new UnitPrefix(UnitPrefixes, "zetta", 'Z', 21),
                                                                                                    new UnitPrefix(UnitPrefixes, "exa",   'E', 18),
                                                                                                    new UnitPrefix(UnitPrefixes, "peta",  'P', 15),
                                                                                                    new UnitPrefix(UnitPrefixes, "tera",  'T', 12),
                                                                                                    new UnitPrefix(UnitPrefixes, "giga",  'G', 9),
                                                                                                    new UnitPrefix(UnitPrefixes, "mega",  'M', 6),
                                                                                                    new UnitPrefix(UnitPrefixes, "kilo",  'K', 3),   /* k */
                                                                                       /* extra */  new UnitPrefix(UnitPrefixes, "kilo",  'k', 3),   /* k */
                                                                                                    new UnitPrefix(UnitPrefixes, "hecto", 'H', 2),   /* h */
                                                                                       /* extra */  new UnitPrefix(UnitPrefixes, "hecto", 'h', 2),   /* h */
                                                                                                    new UnitPrefix(UnitPrefixes, "deca",  'D', 1),   /* da */
                                                                                                    new UnitPrefix(UnitPrefixes, "deci",  'd', -1), 
                                                                                                    new UnitPrefix(UnitPrefixes, "centi", 'c', -2), 
                                                                                                    new UnitPrefix(UnitPrefixes, "milli", 'm', -3),
                                                                                                    // new UnitPrefix(UnitPrefixes, "micro", 'μ', -6), // '\0x03BC' (Char)956  
                                                                                                    new UnitPrefix(UnitPrefixes, "micro", 'µ', -6),  // Ansi '\0x00B5' (Char)181   
                                                                                                    new UnitPrefix(UnitPrefixes, "nano",  'n', -9), 
                                                                                                    new UnitPrefix(UnitPrefixes, "pico",  'p', -12), 
                                                                                                    new UnitPrefix(UnitPrefixes, "femto", 'f', -15), 
                                                                                                    new UnitPrefix(UnitPrefixes, "atto",  'a', -18), 
                                                                                                    new UnitPrefix(UnitPrefixes, "zepto", 'z', -21), 
                                                                                                    new UnitPrefix(UnitPrefixes, "yocto", 'y', -24) });
        /*  http://en.wikipedia.org/wiki/Category:SI_units 
            SI base units
                Name        Symbol  Measure 
                metre       m       length 
                kilogram    kg      mass
                second      s       time
                ampere      A       electric current
                kelvin      K       thermodynamic temperature
                mole        mol     amount of substance 
                candela     cd      luminous intensity
          
            http://en.wikipedia.org/wiki/SI_derived_unit
            Named units derived from SI base units 
                Name        Symbol  Quantity                            Expression in terms of other units      Expression in terms of SI base units 
                hertz       Hz      frequency                           1/s                                     s-1 
                radian      rad     angle                               m∙m-1                                   dimensionless 
                steradian   sr      solid angle                         m2∙m-2                                  dimensionless 
                newton      N       force, weight                       kg∙m/s2                                 kg∙m∙s−2 
                pascal      Pa      pressure, stress                    N/m2                                    m−1∙kg∙s−2 
                joule       J       energy, work, heat                  N∙m = C·V = W·s                         m2∙kg∙s−2 
                watt        W       power, radiant flux                 J/s = V·A                               m2∙kg∙s−3 
                coulomb     C       electric charge or electric flux    s∙A                                     s∙A 
                volt        V       voltage, 
                                    electrical potential difference, 
                                    electromotive force                 W/A = J/C                               m2∙kg∙s−3∙A−1 
                farad       F       electric capacitance                C/V                                     m−2∙kg−1∙s4∙A2 
                ohm         Ω       electric resistance,
                                    impedance, reactance                V/A                                     m2∙kg∙s−3∙A−2 
                siemens     S       electrical conductance              1/Ω                                     m−2∙kg−1∙s3∙A2 
                weber       Wb      magnetic flux                       J/A                                     m2∙kg∙s−2∙A−1 
                tesla       T       magnetic field strength, 
                                    magnetic flux density               V∙s/m2 = Wb/m2 = N/(A∙m)                kg∙s−2∙A−1 
                henry       H       inductance                          V∙s/A = Wb/A                            m2∙kg∙s−2∙A−2 
         
        
                Celsius     C       temperature                         K − 273.15                              K − 273.15 
                lumen       lm      luminous flux                       lx·m2                                   cd·sr 
                lux         lx      illuminance                         lm/m2                                   m−2∙cd∙sr 
                becquerel   Bq      radioactivity 
                                    (decays per unit time)              1/s                                     s−1 
                gray        Gy      absorbed dose 
                                    (of ionizing radiation)             J/kg                                    m2∙s−2 
                sievert     Sv      equivalent dose 
                                    (of ionizing radiation)             J/kg                                    m2∙s−2 
                katal       kat     catalytic activity                  mol/s                                   s−1∙mol 
         
        */

        public static readonly BaseUnit[] SI_BaseUnits = new BaseUnit[] {  new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Length, "meter", "m"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Mass, "kilogram", "Kg"), /* kg */
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Time, "second", "s"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.ElectricCurrent, "ampere", "A"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.AmountOfSubstance, "mol", "mol"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.LuminousIntensity, "candela", "cd") };

        public static readonly UnitSystem SI_Units = new UnitSystem("SI", UnitPrefixes,
                                                                 SI_BaseUnits,
                                                                 new NamedDerivedUnit[] {   new NamedDerivedUnit(SI_Units, "hertz",     "Hz",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "radian",    "rad",  new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "steradian", "sr",   new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "newton",    "N",    new SByte[] { 1, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "pascal",    "Pa",   new SByte[] { -1, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "joule",     "J",    new SByte[] { 2, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "watt",      "W",    new SByte[] { 2, 1, -3, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "coulomb",   "C",    new SByte[] { 1, 0, 0, 1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "volt",      "V",    new SByte[] { 2, 1, -3, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "farad",     "F",    new SByte[] { -2, -1, 4, 2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "ohm",       "Ω",    new SByte[] { 2, 1, -3, -2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "siemens",   "S",    new SByte[] { -2, -1, 3, 2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "weber",     "Wb",   new SByte[] { 2, 1, -2, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "tesla",     "T",    new SByte[] { 0, 1, -2, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "henry",     "H",    new SByte[] { 2, 1, -2, -2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "lumen",     "lm",   new SByte[] { 0, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "lux",       "lx",   new SByte[] { -2, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "becquerel", "Bq",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "gray",      "Gy",   new SByte[] { 2, 0, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "katal",     "kat",  new SByte[] { 0, 0, -1, 0, 0, 1, 0 }) },
                                                                 new ConvertibleUnit[] { new ConvertibleUnit("gram", "g", SI_BaseUnits[(int)PhysicsBaseQuantityKind.Mass], new ScaledValueConversion(1000)),  /* [g] = 1000 * [Kg] */
                                                                                         new ConvertibleUnit("Celsius", "°C" /* degree sign:  C2 B0  (char)176 '\0x00B0' */ , SI_BaseUnits[(int)PhysicsBaseQuantityKind.ThermodynamicTemperature], new LinearValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                                                                         new ConvertibleUnit("hour", "h", SI_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(1.0/3600)), /* [h] = 1/3600 * [s] */
                                                                                         new ConvertibleUnit("liter", "l", SI_BaseUnits[(int)PhysicsBaseQuantityKind.Length].Pow(3), new ScaledValueConversion(1000) ) }) ; /* [l] = 1000 * [m3] */


        //public static readonly PhysicalUnit dimensionless = new DerivedUnit(SI_Units, new SByte[] { 0, 0, 0, 0, 0, 0, 0 });
        public static readonly PhysicalUnit dimensionless = SI_Units.Dimensionless as PhysicalUnit;

        public static readonly UnitSystem CGS_Units = new UnitSystem("CGS", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.Length, "centimeter", "cm"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.Mass, "gram", "g"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.Time, "second", "s"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.ElectricCurrent, "ampere", "A"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.AmountOfSubstance, "mol", "mol"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)PhysicsBaseQuantityKind.LuminousIntensity, "candela", "cd")});

        public static readonly BaseUnit[] MGD_BaseUnits = new BaseUnit[] {  new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Length, "meter", "m"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Mass, "kilogram", "Kg"), /* kg */
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.Time, "day", "d"),
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.ElectricCurrent, "ampere", "A"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.AmountOfSubstance, "mol", "mol"), 
                                                                    new BaseUnit(null, (SByte)PhysicsBaseQuantityKind.LuminousIntensity, "candela", "cd") };

        public static readonly UnitSystem MGD_Units = new UnitSystem("MGD", UnitPrefixes,
                                                                  MGD_BaseUnits, 
                                                                 null,
                                                                 new ConvertibleUnit[] { new ConvertibleUnit("second", "sec", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(24 * 60 * 60)),  /* [sec]  = 24 * 60 * 60 * [d] */
                                                                                         new ConvertibleUnit("minute", "min", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(24 * 60)),       /* [min]  = 24 * 60 * [d] */
                                                                                         new ConvertibleUnit("hour", "hour", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(24)),             /* [hour] = 24 * [d] */
                                                                                         new ConvertibleUnit("day", "day", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new IdentityValueConversion()),               /* [day]  = 1 * [d] */
                                                                                         new ConvertibleUnit("year", "year", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(1.0/36.25)),      /* [year] = 1/365.25 * [d] */
                                                                                         new ConvertibleUnit("year", "y", MGD_BaseUnits[(int)PhysicsBaseQuantityKind.Time], new ScaledValueConversion(1.0/365.25)) });     /* [y]    = 1/365.25 * [d] */

        public static readonly UnitSystem MGM_Units = new UnitSystem("MGM", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "second", "s"), */
                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "day", "d"), */
                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.Time, "moment", "ø"), 

                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.ElectricCurrent, "ampere", "A"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.AmountOfSubstance, "mol", "mol"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)PhysicsBaseQuantityKind.LuminousIntensity, "candela", "cd") });

        //public static UnitSystem[] UnitSystems = new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units };
        public static UnitSystemStack CurrentUnitSystems = new UnitSystemStack();
        public static UnitLookup UnitSystems = new UnitLookup(new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units });

        public static readonly UnitSystemConversion SItoCGSConversion = new UnitSystemConversion(SI_Units, CGS_Units, new ValueConversion[] { new ScaledValueConversion(100),       /* 1 m       <SI> = 100 cm        <CGS>  */
                                                                                                                                     new ScaledValueConversion(1000),               /* 1 Kg      <SI> = 1000 g        <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 s       <SI> = 1 s           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 A       <SI> = 1 A           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 K       <SI> = 1 K           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 mol     <SI> = 1 mol         <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 candela <SI> = 1 candela     <CGS>  */
                                                                                                                                    });

        public static readonly UnitSystemConversion SItoMGDConversion = new UnitSystemConversion(SI_Units, MGD_Units, new ValueConversion[] { new IdentityValueConversion(),        /* 1 m       <SI> = 1 m           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 Kg      <SI> = 1 Kg          <MGD>  */
                                                                                                                                     new ScaledValueConversion(1.0/(24*60*60)),     /* 1 s       <SI> = 1/86400 d     <MGD>  */
                                                                                                                                  /* new ScaledValueConversion(10000/(24*60*60)),   /* 1 s       <SI> = 10000/86400 ø <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 A       <SI> = 1 A           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 K       <SI> = 1 K           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 mol     <SI> = 1 mol         <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 candela <SI> = 1 candela     <MGD>  */
                                                                                                                                   });

        public static readonly UnitSystemConversion MGDtoMGMConversion = new UnitSystemConversion(MGD_Units, MGM_Units, new ValueConversion[] { new IdentityValueConversion(),      /* 1 m       <MGD> = 1 m           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 Kg      <MGD> = 1 Kg          <MGM>  */
                                                                                                                                       new ScaledValueConversion(10000),            /* 1 d       <MGD> = 10000 ø       <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 A       <MGD> = 1 A           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 K       <MGD> = 1 K           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 mol     <MGD> = 1 mol         <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 candela <MGD> = 1 candela     <MGM>  */
                                                                                                                                     });

        //public static IList<UnitSystemConversion> UnitSystemConversions = new List<UnitSystemConversion> { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion };

        public static UnitSystemConversionLookup UnitSystemConversions = new UnitSystemConversionLookup(new List<UnitSystemConversion> { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion });

#if NEVER
        public static UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            UnitSystemConversion usc = GetDirectUnitSystemConversion(unitsystem1, unitsystem2, UnitSystemConversions);
            if (usc != null)
            {
                return usc;
            }

            /*  No direct unit system conversion from  unitsystem1 to unitsystem2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */

            IList<IUnitSystem> OldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> OldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            usc = GetUnitSystemConversion(OldUnitsystems1, NewUnitSystemsConvertableToUnitsystems1, OldUnitsystems2, NewUnitSystemsConvertableToUnitsystems2, UnitSystemConversions);
            return usc;
        }

        public static UnitSystemConversion GetDirectUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2, IList<UnitSystemConversion> unitSystemConversions)
        {
            foreach (UnitSystemConversion usc in unitSystemConversions)
            {
                if (   (usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                    || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                {
                    return usc;
                }
            }

            return null;
        }

        public static UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2, IList<UnitSystemConversion> unitSystemConversions)
        {
            /*  No direct unit system conversion from  unitsystem1 to unitsystem2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */

            IList<IUnitSystem> OldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> OldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> NewUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            return GetUnitSystemConversion(OldUnitsystems1, NewUnitSystemsConvertableToUnitsystems1, OldUnitsystems2, NewUnitSystemsConvertableToUnitsystems2, UnitSystemConversions);
        }

        public static UnitSystemConversion GetUnitSystemConversion(IList<IUnitSystem> oldUnitsystems1, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems1,
                                                                   IList<IUnitSystem> oldUnitsystems2, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems2,
                                                                   IList<UnitSystemConversion> unitSystemConversions)
        {
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>();
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>();

            IList<UnitSystemConversion> Unitsystems1Conversions = new List<UnitSystemConversion>();
            IList<UnitSystemConversion> Unitsystems2Conversions = new List<UnitSystemConversion>();

            foreach (UnitSystemConversion usc in unitSystemConversions)
            {
                Boolean BUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.ConvertedUnitSystem);

                Boolean BUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.ConvertedUnitSystem);

                if (BUS_in_US1 || CUS_in_US1)
                {
                    if (BUS_in_US2 || CUS_in_US2)
                    {
                        return usc;
                    }

                    Debug.Assert(!Unitsystems1Conversions.Contains(usc));
                    Unitsystems1Conversions.Add(usc);

                    if (!(BUS_in_US1 && CUS_in_US1))
                    {
                        if (BUS_in_US1)
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.BaseUnitSystem);
                        }
                    }
                }
                else if (BUS_in_US2 || CUS_in_US2)
                {
                    Debug.Assert(!Unitsystems2Conversions.Contains(usc));
                    Unitsystems2Conversions.Add(usc);

                    if (!(BUS_in_US2 && CUS_in_US2))
                    {
                        if (BUS_in_US2)
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.BaseUnitSystem);
                        }
                    }
                }
            }

            /*  No direct unit system conversion from  unitsystems1 to unitsystems2. 
             *  Try to find an intermediate unit system with conversion to/from unitsystems1 and unitsystems2 */

            if (unitSystemsConvertableToUnitsystems1.Count > 0 || unitSystemsConvertableToUnitsystems2.Count > 0)
            {
                IList<IUnitSystem> unitsystems1 = (IList<IUnitSystem>)oldUnitsystems1.Union(newUnitSystemsConvertableToUnitsystems1).ToList();
                IList<IUnitSystem> unitsystems2 = (IList<IUnitSystem>)oldUnitsystems2.Union(newUnitSystemsConvertableToUnitsystems2).ToList();

                UnitSystemConversion subIntermediereUnitSystemConversion = null;

                IList<IUnitSystem> IntersectUnitsystemsList = unitSystemsConvertableToUnitsystems1.Intersect(unitSystemsConvertableToUnitsystems2).ToList();

                if (IntersectUnitsystemsList.Count > 0)
                {
                    IUnitSystem IntersectUnitsystem = IntersectUnitsystemsList[0];
                    subIntermediereUnitSystemConversion = GetUnitSystemConversion(new List<IUnitSystem>() { }, newUnitSystemsConvertableToUnitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { IntersectUnitsystem }, Unitsystems1Conversions);
                    Debug.Assert(subIntermediereUnitSystemConversion != null);
                }
                else
                { 
                    IList<UnitSystemConversion> notIntermediereUnitSystemConversions = (IList<UnitSystemConversion>)unitSystemConversions.Except(Unitsystems1Conversions.Union(Unitsystems2Conversions)).ToList();
                    if (notIntermediereUnitSystemConversions.Count > 0)
                    {
                        subIntermediereUnitSystemConversion = GetUnitSystemConversion(unitsystems1, unitSystemsConvertableToUnitsystems1, unitsystems2, unitSystemsConvertableToUnitsystems2, notIntermediereUnitSystemConversions);
                    }
                }
                if (subIntermediereUnitSystemConversion != null)
                {
                    if (   !unitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from some unit system in unitsystems1 to one of subIntermediereUnitSystemConversion's systems
                        // Find Pre UnitSystemConversion

                        IUnitSystem CombinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion SecondUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean SecondValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem);
                        if (!SecondValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            CombinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            CombinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion FirstUnitSystemConversion = GetUnitSystemConversion(new List<IUnitSystem>() { }, unitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem }, Unitsystems1Conversions);
                        Boolean FirstValueConversionDirectionInverted = FirstUnitSystemConversion.BaseUnitSystem == CombinedUnitSystemConversionIntermedierUnitSystem;

                        if (!FirstValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = FirstUnitSystemConversion.BaseUnitSystem;
                        }
                        else
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = FirstUnitSystemConversion.ConvertedUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[0], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[0], SecondValueConversionDirectionInverted),
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[1], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[1], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[2], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[2], SecondValueConversionDirectionInverted),
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[3], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[3], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[4], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[4], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[5], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[5], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[6], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[6], SecondValueConversionDirectionInverted)    
                                                                                            };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(CombinedUnitSystemConversionBaseUnitSystem, CombinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }

                    if (   !unitsystems2.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from one of subIntermediereUnitSystemConversion's systems to some unit system in unitsystems2
                        // Find Post UnitSystemConversion

                        IUnitSystem CombinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem CombinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion FirstUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean FirstValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem);
                        if (!FirstValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem) || unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));

                            CombinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;  
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem; 
                        }

                        UnitSystemConversion SecondUnitSystemConversion = GetUnitSystemConversion(new List<IUnitSystem>() { }, new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem }, new List<IUnitSystem>() { }, unitsystems2, Unitsystems2Conversions);
                        Boolean SecondValueConversionDirectionInverted = SecondUnitSystemConversion.ConvertedUnitSystem == CombinedUnitSystemConversionIntermedierUnitSystem;

                        if (!SecondValueConversionDirectionInverted)
                        {
                            CombinedUnitSystemConversionConvertedUnitSystem = SecondUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            CombinedUnitSystemConversionConvertedUnitSystem = SecondUnitSystemConversion.BaseUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[0], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[0], SecondValueConversionDirectionInverted),
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[1], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[1], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[2], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[2], SecondValueConversionDirectionInverted),
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[3], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[3], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[4], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[4], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[5], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[5], SecondValueConversionDirectionInverted),  
                                                                                                new CombinedValueConversion(FirstUnitSystemConversion.BaseUnitConversions[6], FirstValueConversionDirectionInverted, SecondUnitSystemConversion.BaseUnitConversions[6], SecondValueConversionDirectionInverted)    
                                                                                            };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(CombinedUnitSystemConversionBaseUnitSystem, CombinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }
                    return subIntermediereUnitSystemConversion;
                }
            }
 
            return null;
        }

        /***
        public static IPhysicalUnit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in UnitSystems.)
            {
                INamedSymbolUnit unit = us.UnitFromName(namestr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalUnit UnitFromSymbol(String symbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromSymbol(symbolstr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalUnit ScaledUnitFromSymbol(String scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalUnit scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }

        public static IUnitSystem UnitSystemFromName(String UnitSystemsymbolstr)
        {
            IUnitSystem result_us = UnitSystems.FirstOrNull<IUnitSystem>(us => us.Name == UnitSystemsymbolstr);
            return result_us;
        }
        ***/
#endif // NEVER


    }

    public static partial class Prefix
    {
        /* SI unit prefixes */
        public static readonly UnitPrefix Y = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[0];
        public static readonly UnitPrefix Z = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[1];
        public static readonly UnitPrefix E = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[2];
        public static readonly UnitPrefix P = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[3];
        public static readonly UnitPrefix T = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[4];
        public static readonly UnitPrefix G = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[5];
        public static readonly UnitPrefix M = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[6];
        public static readonly UnitPrefix K = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[7];
        public static readonly UnitPrefix k = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[8];
        public static readonly UnitPrefix H = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[9];
        public static readonly UnitPrefix h = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[10];
        public static readonly UnitPrefix D = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[11];
        public static readonly UnitPrefix da = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[11]; // Extra
        public static readonly UnitPrefix d = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[12];
        public static readonly UnitPrefix c = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[13];
        public static readonly UnitPrefix m = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[14];
        public static readonly UnitPrefix my = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[15];
        public static readonly UnitPrefix n = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[16];
        public static readonly UnitPrefix p = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[17];
        public static readonly UnitPrefix f = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[18];
        public static readonly UnitPrefix a = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[19];
        public static readonly UnitPrefix z = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[20];
        public static readonly UnitPrefix y = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[21];
    }

#region Physical Unit System Static Classes

    public static partial class SI 
    {
        /* SI base units */
        public static readonly BaseUnit m   = (BaseUnit)Physics.SI_Units.BaseUnits[0];
        public static readonly BaseUnit Kg  = (BaseUnit)Physics.SI_Units.BaseUnits[1];
        public static readonly BaseUnit s   = (BaseUnit)Physics.SI_Units.BaseUnits[2];
        public static readonly BaseUnit A   = (BaseUnit)Physics.SI_Units.BaseUnits[3];
        public static readonly BaseUnit K   = (BaseUnit)Physics.SI_Units.BaseUnits[4];
        public static readonly BaseUnit mol = (BaseUnit)Physics.SI_Units.BaseUnits[5];
        public static readonly BaseUnit cd  = (BaseUnit)Physics.SI_Units.BaseUnits[6];

        /* Named units derived from SI base units */
        public static readonly NamedDerivedUnit Hz  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[0];
        public static readonly NamedDerivedUnit rad = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[1];
        public static readonly NamedDerivedUnit sr  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[2];
        public static readonly NamedDerivedUnit N   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[3];
        public static readonly NamedDerivedUnit Pa  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[4];
        public static readonly NamedDerivedUnit J   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[5];
        public static readonly NamedDerivedUnit W   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[6];
        public static readonly NamedDerivedUnit C   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[7];
        public static readonly NamedDerivedUnit V   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[8];
        public static readonly NamedDerivedUnit F   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[9];
        public static readonly NamedDerivedUnit Ohm = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[10];
        public static readonly NamedDerivedUnit S   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[11];
        public static readonly NamedDerivedUnit Wb  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[12];
        public static readonly NamedDerivedUnit T   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[13];
        public static readonly NamedDerivedUnit H   = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[14];
        public static readonly NamedDerivedUnit lm  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[15];
        public static readonly NamedDerivedUnit lx  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[16];
        public static readonly NamedDerivedUnit Bq  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[17];
        public static readonly NamedDerivedUnit Gy  = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[18];
        public static readonly NamedDerivedUnit kat = (NamedDerivedUnit)Physics.SI_Units.NamedDerivedUnits[19];

        /* Convertible units */
        public static readonly ConvertibleUnit g  = (ConvertibleUnit)Physics.SI_Units.ConvertibleUnits[0];
        public static readonly ConvertibleUnit Ce = (ConvertibleUnit)Physics.SI_Units.ConvertibleUnits[1];
        public static readonly ConvertibleUnit h  = (ConvertibleUnit)Physics.SI_Units.ConvertibleUnits[2];
        public static readonly ConvertibleUnit l  = (ConvertibleUnit)Physics.SI_Units.ConvertibleUnits[3];
    }

#endregion Physical Unit System Static Classes

#endregion Physical Measure Static Classes

#endregion Physical Measure Classes

}

namespace PhysicalMeasure.Constants
{
#region Physical Constants Statics

    public static partial class Constants
    {
        /* http://en.wikipedia.org/wiki/Physical_constant 
         
            Table of universal constants
                Quantity                            Symbol      Value                               Relative Standard Uncertainty 
                speed of light in vacuum            c           299792458 m·s−1                     defined 
                Newtonian constant of gravitation   G           6.67428(67)×10−11 m3·kg−1·s−2       1.0 × 10−4 
                Planck constant                     h           6.62606896(33) × 10−34 J·s          5.0 × 10−8 
                reduced Planck constant             h/2 pi      1.054571628(53) × 10−34 J·s         5.0 × 10−8 
         */
        public static readonly PhysicalQuantity c = new PhysicalQuantity(299792458, SI.m / SI.s);
        public static readonly PhysicalQuantity G = new PhysicalQuantity(6.67428E-11, (SI.m^3) / (SI.Kg * (SI.s ^ 2)));
        public static readonly PhysicalQuantity h = new PhysicalQuantity(6.62E-34, SI.J * SI.s);
        public static readonly PhysicalQuantity h_bar = new PhysicalQuantity(1.054571628 - 34, SI.J * SI.s);

        /*
            Table of electromagnetic constants
                Quantity                            Symbol      Value                           (SI units)              Relative Standard Uncertainty 
                magnetic constant 
                     (vacuum permeability)          my0         4π × 10−7                       N·A−2 
                                                                   = 1.256637061× 10−6          N·A−2                   defined 
                electric constant 
                     (vacuum permittivity)          epsilon0    8.854187817×10−12               F·m−1                   defined 
                characteristic impedance of vacuum  Z0          376.730313461                   Ω                       defined 
                Coulomb's constant                  ke          8.987551787×109                 N·m²·C−2                defined 
                elementary charge                   e           1.602176487×10−19               C                       2.5 × 10−8 
                Bohr magneton                       myB         9.27400915(23)×10−24            J·T−1                   2.5 × 10−8 
                conductance quantum                 G0          7.7480917004(53)×10−5           S                       6.8 × 10−10 
                inverse conductance quantum         1/G0        12906.4037787(88)               Ω                       6.8 × 10−10 
                Josephson constant                  KJ          4.83597891(12)×1014             Hz·V−1                  2.5 × 10−8 
                magnetic flux quantum               phi0        2.067833667(52)×10−15           Wb                      2.5 × 10−8 
                nuclear magneton                    myN         5.05078343(43)×10−27            J·T−1                   8.6 × 10−8 
                von Klitzing constant               RK          25812.807557(18)                Ω                       6.8 × 10−10 
        */
        public static readonly PhysicalQuantity my0 = new PhysicalQuantity(1.256637061E-6, SI.N / (SI.A^2));
        public static readonly PhysicalQuantity epsilon0 = new PhysicalQuantity(8.854187817E-12, SI.F / SI.m);
        public static readonly PhysicalQuantity Z0 = new PhysicalQuantity(376.730313461, SI.Ohm);
        public static readonly PhysicalQuantity ke = new PhysicalQuantity(8.987551787E9, SI.N * (SI.m^2) /(SI.C^2));
        public static readonly PhysicalQuantity e = new PhysicalQuantity(1.602176487E-19, SI.C);
        public static readonly PhysicalQuantity myB = new PhysicalQuantity(9.27400915E-24, SI.J / SI.T);
        public static readonly PhysicalQuantity G0 = new PhysicalQuantity(7.7480917004E-5, SI.S);
        public static readonly PhysicalQuantity KJ = new PhysicalQuantity(4.83597891E14, SI.Hz / SI.V);
        public static readonly PhysicalQuantity phi0 = new PhysicalQuantity(2.067833667E-15, SI.Wb);
        public static readonly PhysicalQuantity myN = new PhysicalQuantity(5.05078343E-27, SI.J / SI.T);
        public static readonly PhysicalQuantity RK = new PhysicalQuantity(25812.807557, SI.Ohm);

        /*
            Table of atomic and nuclear constants
                Quantity                            Symbol      Value                           (SI units)              Relative Standard Uncertainty 
                Bohr radius                         a0          5.291772108(18)×10−11           m                       3.3 × 10−9 
                classical electron radius           re          2.8179402894(58)×10−15          m                       2.1 × 10−9 
                electron mass                       me          9.10938215(45)×10−31            kg                      5.0 × 10−8 
                Fermi coupling constant             GF          1.16639(1)×10−5                 GeV−2                   8.6 × 10−6 
                fine-structure constant             alpha       7.2973525376(50)×10−3                                   6.8 × 10−10 
                Hartree energy                      Eh          4.35974417(75)×10−18            J                       1.7 × 10−7 
                proton mass                         mp          1.672621637(83)×10−27           kg                      5.0 × 10−8 
                quantum of circulation              h2me        3.636947550(24)×10−4            m² s−1                  6.7 × 10−9 
                Rydberg constant                    Rinf        10973731.568525(73)             m−1                     6.6 × 10−12 
                Thomson cross section               tcs         6.65245873(13)×10−29            m²                      2.0 × 10−8 
                weak mixing angle                   ThetaW      0.22215(76)                                             3.4 × 10−3 
        */
        public static readonly PhysicalQuantity a0 = new PhysicalQuantity(5.291772108E-11, SI.m);
        public static readonly PhysicalQuantity re = new PhysicalQuantity(2.8179402894E-15, SI.m);
        public static readonly PhysicalQuantity me = new PhysicalQuantity(9.10938215E-31, SI.Kg);
        /** */
        public static readonly PhysicalQuantity GF = new PhysicalQuantity(1.16639E-5, (Prefix.G * Constants.e * SI.V) ^ -2);
        /* **/
        public static readonly PhysicalQuantity alpha = new PhysicalQuantity(7.2973525376E-3, Physics.dimensionless);
        public static readonly PhysicalQuantity Eh = new PhysicalQuantity(4.35974417E-18, SI.J);
        public static readonly PhysicalQuantity mp = new PhysicalQuantity(1.672621637E-27, SI.Kg);
        public static readonly PhysicalQuantity h2me = new PhysicalQuantity(3.636947550E-4, (SI.m ^ 2) / SI.s);
        public static readonly PhysicalQuantity Rinf = new PhysicalQuantity(10973731.568525, SI.m ^ -1);
        public static readonly PhysicalQuantity tcs = new PhysicalQuantity(6.65245873E-29, SI.m ^ 2);
        public static readonly PhysicalQuantity ThetaW = new PhysicalQuantity(0.22215, Physics.dimensionless);
    }
#endregion Physical Constants Statics
}



namespace Extensions
{
    public static class DoubleExtensions
    {
        public static int EpsilonCompareTo(this double thisValue, double otherValue)
        {   /* Limited precision handling */
            double RelativeDiff = (thisValue - otherValue) / thisValue;
            if (RelativeDiff < -1e-15)
            {
                return -1;
            }
            if (RelativeDiff > 1e-15)
            {
                return 1;
            }
            return 0;
        }
    }

    public static class IEnumerableExtensions
    {
        public static T FirstOrNull<T>(this IEnumerable<T> sequence) where T : class
        {
            //return values.DefaultIfEmpty(null).FirstOrDefault();

            foreach (T item in sequence)
                return item;
            return null;
        }

        public static T FirstOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : class
        {
            foreach (T item in sequence.Where(predicate))
                return item;
            return null;
        }


        public static T? FirstStructOrNull<T>(this IEnumerable<T> sequence) where T : struct
        {
            foreach (T item in sequence)
                return item;
            return null;
        }

        public static T? FirstStructOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) where T : struct
        {
            foreach (T item in sequence.Where(predicate))
                return item;
            return null;
        }
    }



    public static class ArrayExtensions
    {
        public static T[] Concat<T>(T[] a1, T[] a2)
        {
            if (a1 != null && a2 != null)
            {
                return a1.Concat(a2).ToArray();
            }
            else
            if (a2 != null)
            {
                return a2;
            }
            return a1;
        }


        public static T FirstOrNull<T>(this T[] values) where T : class
        {
            foreach (T item in values)
                return item;
            return null;
        }

        public static T FirstOrNull<T>(this T[] values, Func<T, bool> predicate) where T : class
        {
            foreach (T item in values.Where(predicate))
                return item;
            return null;
        }



        public static T? FirstStructOrNull<T>(this T[] values) where T : struct
        {
            foreach (T item in values)
                return item;
            return null;
        }

        public static T? FirstStructOrNull<T>(this T[] values, Func<T, bool> predicate) where T : struct
        {
            foreach (T item in values.Where(predicate))
                return item;
            return null;
        }
    }
}

