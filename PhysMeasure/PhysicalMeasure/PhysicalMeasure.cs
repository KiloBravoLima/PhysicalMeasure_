/*   http://physicalmeasure.codeplex.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace PhysicalMeasure
{
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

    #region Physical Measure Constants

    public static partial class Physics
    {
        public const int NoOfMeasures = 7;
    }

    public enum MeasureKind
    {
        Length,
        Mass,
        Time,
        ElectricCurrent,
        ThermodynamicTemperature,
        AmountOfSubstance,
        LuminousIntensity
    }

    //public enum UnitKind { BaseUnit, DerivedUnit, ConvertibleUnit, CombinedUnit };
    public enum UnitKind { BaseUnit, DerivedUnit, ConvertibleUnit, CombinedUnit, MixedUnit };

    #endregion Physical Measure Constants

    #region Physical Measure Interfaces

    public interface INamed
    {
        String Name { get; /* set; */ }
    }

    public interface INamedSymbol : INamed
    {
        String Symbol { get; /* set; */ }
    }

    public interface ISystemItem
    {
        IUnitSystem System { get; /* set; */ }
    }

    public interface INamedSymbolSystemItem : ISystemItem, INamedSymbol
    {
    }

    public interface IUnit
    {
        UnitKind Kind { get; }
        SByte[] Exponents { get; }

        String UnitString();

        String UnitPrintString();

        String ToPrintString();

        String ReducedUnitString();


        String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false);

        String ValueString(double quantity);
        String ValueString(double quantity, String format, IFormatProvider formatProvider);
    }

    public interface ISystemUnit : ISystemItem, IUnit
    {
    }

    public interface IMixedUnit : ISystemUnit
    {
        IPhysicalUnit MainUnit { get; }
        IPhysicalUnit FractionalUnit { get; }
        String Separator { get; }
    }

    public interface IPrefixedUnit
    {
        SByte PrefixExponent { get; }
        IPhysicalUnit Unit { get;  }

        IPhysicalQuantity PhysicalQuantity();
        IPhysicalQuantity PhysicalQuantity(ref double quantity, IUnitSystem unitSystem);
    }

    public interface IPrefixedUnitExponent : IPrefixedUnit
    {
        SByte Exponent { get; }

        String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false);

        IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out Double scaleFactor);



        //IPhysicalQuantity PhysicalQuantity();
        //IPhysicalQuantity PhysicalQuantity(ref double quantity, IUnitSystem unitSystem);
    }

    public interface IPhysicalItemMath : IEquatable<IPhysicalUnit>
    {
        IPhysicalUnit Dimensionless { get; }

        Boolean IsDimensionless { get; }

        IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit);
        IPhysicalQuantity Divide(IPhysicalUnit physicalUnit);

        IPhysicalQuantity Pow(SByte exponent);
        IPhysicalQuantity Rot(SByte exponent);

        IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit);
        IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit);

        IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent);

        IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity Multiply(double quantity);
        IPhysicalQuantity Divide(double quantity);
    }

    public interface IPhysicalUnitMath : IPhysicalItemMath
    {
        IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity);

        //IPhysicalUnit Pow(SByte exponent);
        //IPhysicalUnit Rot(SByte exponent);

    }

    public interface IPhysicalUnitCombine 
    {
        IPhysicalUnit CombinePrefix(SByte prefixExponent);

        //IPhysicalUnit CombineMultiply(double quantity);
        //IPhysicalUnit CombineDivide(double quantity);

        IPhysicalUnit CombineMultiply(IPhysicalUnit physicalUnit);
        IPhysicalUnit CombineDivide(IPhysicalUnit physicalUnit);

        IPhysicalUnit CombineMultiply(IPrefixedUnit prefixedUnit);
        IPhysicalUnit CombineDivide(IPrefixedUnit prefixedUnit);

        IPhysicalUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent);
        IPhysicalUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent);

        IPhysicalUnit CombinePow(SByte exponent);
        IPhysicalUnit CombineRot(SByte exponent);
    }

    public interface IPhysicalQuantityConvertible
    {
        IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit();

        IPhysicalQuantity ConvertToBaseUnit();
    }

    public interface IPhysicalUnitConvertible : IPhysicalQuantityConvertible
    {
        IPhysicalQuantity ConvertTo(ref double quantity, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem);

        IPhysicalQuantity ConvertToSystemUnit(ref double quantity);

        IPhysicalQuantity ConvertToBaseUnit(double quantity);
        IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity);
    }

    public interface IPhysicalUnit : ISystemUnit, IPhysicalUnitMath, IPhysicalUnitCombine, IPhysicalUnitConvertible /*  : <BaseUnit | DerivedUnit | ConvertibleUnit>  */
    {
        
        // public static PhysicalQuantity operator *(IPhysicalUnit u1, IPhysicalUnit u2);

        // public static PhysicalQuantity operator /(IPhysicalUnit u1, IPhysicalUnit u2);
    }

    public interface INamedSymbolUnit : IPhysicalUnit, INamedSymbolSystemItem
    {
    }

    public interface IBaseUnit : INamedSymbolUnit
    {
        SByte BaseUnitNumber { get; }
    }

    public interface IDerivedUnit : IPhysicalUnit
    {
    }

    public interface INamedDerivedUnit : INamedSymbolUnit, IDerivedUnit
    {
    }

    public interface IConvertibleUnit : INamedSymbolUnit
    {
        IPhysicalUnit PrimaryUnit { get; }
        IValueConversion Conversion { get; }

        IPhysicalQuantity ConvertToPrimaryUnit();
        IPhysicalQuantity ConvertFromPrimaryUnit();

        //IPhysicalQuantity ConvertToPrimaryUnit(IPhysicalQuantity physicalQuantity);
        //IPhysicalQuantity ConvertFromPrimaryUnit(IPhysicalQuantity physicalQuantity);

        IPhysicalQuantity ConvertToPrimaryUnit(double quantity);
        IPhysicalQuantity ConvertFromPrimaryUnit(double quantity);
    }

    public interface IValueConversion
    {
        double Convert(double value, bool backwards = false);
        double ConvertFromPrimaryUnit(double value);
        double ConvertToPrimaryUnit(double value);
    }

    public interface ICombinedUnit : IPhysicalUnit
    {
        PrefixedUnitExponentList Numerators { get; /* set; */ }
        PrefixedUnitExponentList Denominators { get; /* set; */ }

        IUnitSystem SomeSystem { get; /* set; */ }
    }

    public interface IPhysicalQuantityMath : IComparable, IEquatable<double>, IEquatable<IPhysicalQuantity>, IEquatable<IPhysicalUnit>, IPhysicalItemMath
    {
        IPhysicalQuantity Zero { get; }
        IPhysicalQuantity One { get; }

        IPhysicalQuantity Add(IPhysicalQuantity physicalQuantity);
        IPhysicalQuantity Subtract(IPhysicalQuantity physicalQuantity);
    }

    public interface IPhysicalQuantity : IFormattable, IPhysicalQuantityMath, IPhysicalQuantityConvertible
    {
        //double Value { get; set; }
        double Value { get;  /* set; */ }
        //IPhysicalUnit Unit { get; set; }
        IPhysicalUnit Unit { get; /* set; */  }

        String ToPrintString();
    }

    public interface IUnitSystem : INamed
    {
        IUnitPrefixTable UnitPrefixes { get; }
        IBaseUnit[] BaseUnits { get; }
        INamedDerivedUnit[] NamedDerivedUnits { get; }
        IConvertibleUnit[] ConvertibleUnits { get; }

        INamedSymbolUnit UnitFromName(String unitName);
        INamedSymbolUnit UnitFromSymbol(String unitSymbol);

        IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol);
        IPhysicalUnit NamedDerivedUnitFromUnit(IPhysicalUnit derivedUnit);

        IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem);
    }

    public interface IUnitPrefix : INamed
    {
        char PrefixChar { get; }
        SByte PrefixExponent { get; }
        double PrefixValue { get; }
    }

    public interface IUnitPrefixTable
    {
        IUnitPrefix[] UnitPrefixes { get; }

        bool GetExponentFromPrefixChar(char somePrefixChar, out SByte exponent);
        bool GetPrefixCharFromExponent(SByte someExponent, out char prefixChar);
    }

    #endregion Physical Measure Interfaces

    #region Dimension Exponets Classes

    public static class DimensionExponents 
    {
        // SByte[Physic.NoOfMeasures] SomeExponents;
        static public bool Equals(SByte[] exponents1, SByte[] exponents2)
        {
            Debug.Assert(exponents1 != null);
            Debug.Assert(exponents2 != null);

            if (exponents1 == exponents2)
            {
                return true;
            }
            if (exponents1.Equals(exponents2))
            {
                return true;
            }

            bool equal = true;
            SByte i = 0;
            SByte MinNoOfBaseUnits = (SByte)Math.Min(exponents1.Length, exponents2.Length);
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(exponents1.Length, exponents2.Length);

            Debug.Assert(MaxNoOfBaseUnits <= Physics.NoOfMeasures);

            do
            {   // Compare exponents where defined in both arrays
                equal = exponents1[i] == exponents2[i];
                i++;
            } while (i < MinNoOfBaseUnits && equal);

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
                } while (i < MaxNoOfBaseUnits && equal);
            }
            return equal;
        }

        static public bool IsDimensionless(SByte[] exponents)
        {
            Debug.Assert(exponents != null);

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

            bool isDimensionless = true;
            SByte i = 0;
            do
            {
                isDimensionless = exponents[i] == 0;
                i++;
            } while (i < NoOfBaseUnits && isDimensionless);

            return isDimensionless;
        }

        public static SByte NoOfDimensions(SByte[] exponents)
        {
            Debug.Assert(exponents != null);

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

            SByte noOfDimensions = 0;
            SByte i = 0;
            do
            {
                if (exponents[i] != 0)
                {
                    noOfDimensions++;
                }
                i++;
            } while (i < NoOfBaseUnits);

            return noOfDimensions;
        }

        public static bool IsSameMeasureKind(IEnumerable<SByte> exponents1, IEnumerable<SByte> exponents2)
        {
            Debug.Assert(exponents1 != null);
            Debug.Assert(exponents2 != null);

            if (exponents1.Equals(exponents2))
            {
                return true;
            }
            else
            {
                IEnumerator<SByte> e1e = exponents1.GetEnumerator();
                IEnumerator<SByte> e2e = exponents2.GetEnumerator();
                bool E1HasElement = e1e.MoveNext();
                bool E2HasElement = e2e.MoveNext();
                while (E1HasElement && E2HasElement)
                {
                    if (e1e.Current != e2e.Current)
                    {
                        return false;
                    }
                    E1HasElement = e1e.MoveNext();
                    E2HasElement = e2e.MoveNext();
                };

                while (E1HasElement || E2HasElement)
                {
                    if (   (E1HasElement && e1e.Current != 0)
                        || (E2HasElement && e2e.Current != 0))
                    {
                        return false;
                    }
                    E1HasElement = e1e.MoveNext();
                    E2HasElement = e2e.MoveNext();
                };

                return true;
            }
        }

        static public SByte[] Power(SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null);
            Debug.Assert(exponent != 0);

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            do
            {
                NewExponents[i] = (SByte)(exponents[i]/exponent);

                i++;
            } while (i < NoOfBaseUnits);

            return NewExponents;
        }

        static public SByte[] Root(SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null);
            Debug.Assert(exponent != 0);

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            bool Ok = true;
            do
            {
                int Remainder;
                int NewExponent = Math.DivRem(exponents[i], exponent, out Remainder);
                Ok = Remainder == 0;
                NewExponents[i] = (SByte)(NewExponent);

                i++;
            } while (i < NoOfBaseUnits && Ok);

            if (!Ok) 
            {
                NewExponents = null;
            }
            return NewExponents;
        }

    }

    #endregion Dimension Exponets Classes

    #region Physical Unit prefix Classes

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private char _prefixChar;
        private SByte _prefixExponent;

        public char PrefixChar { get { return _prefixChar; } }
        public SByte PrefixExponent { get { return _prefixExponent; } }
        public Double PrefixValue { get { return Math.Pow(10, _prefixExponent); } } 

        public UnitPrefix(String someName, char somePrefixChar, SByte somePrefixExponent)
            : base(someName)
        {
            this._prefixChar = somePrefixChar;
            this._prefixExponent = somePrefixExponent;
        }
    }

    public class UnitPrefixTable : IUnitPrefixTable
    {
        private UnitPrefix[] _unitPrefixes;

        public IUnitPrefix[] UnitPrefixes { get { return _unitPrefixes; } }

        public UnitPrefixTable(UnitPrefix[] anUnitPrefixes)
        {
            this._unitPrefixes = anUnitPrefixes;
        }

        public bool GetExponentFromPrefixChar(char somePrefixChar, out SByte exponent) 
        {
            exponent = 0;

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

            // exponent = (from up in UnitPrefixes where up.PrefixChar == somePrefixChar select up.PrefixExponent).Single();  
            foreach (UnitPrefix up in UnitPrefixes)
            {
                if (up.PrefixChar == somePrefixChar)
                {
                    exponent = up.PrefixExponent;
                    return true;
                }
            }
            return false;
        }

        public bool GetPrefixCharFromExponent(SByte someExponent, out char prefixChar)
        {
            prefixChar = '\0';
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.PrefixExponent == someExponent)
                {
                    prefixChar = us.PrefixChar;
                    return true;
                }
            }
            return false;
        }

    }

    #endregion Physical Unit prefix Classes

    #region Value Conversion Classes

    public abstract class ValueConversion : IValueConversion
    {
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

        public LinearValueConversion(double someOffset, double someScale)
        {
            this.Offset = someOffset;
            this.Scale = someScale;
        }

        public override double ConvertFromPrimaryUnit(double value)
        {
            return value * this.Scale + this.Offset;
        }

        public override double ConvertToPrimaryUnit(double value)
        {
            double convertedValue =  (value - this.Offset) / this.Scale;
            return convertedValue;
        }
    }

    public class ScaledValueConversion : LinearValueConversion
    {
        public ScaledValueConversion(double someScale)
            : base(0, someScale)
        {
            Debug.Assert(someScale != 0);
            Debug.Assert(!double.IsInfinity(someScale));

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


        public CombinedValueConversion(IValueConversion firstValueConversion, Boolean firstValueConversionDirectionInverted, IValueConversion secondValueConversion, Boolean secondValueConversionDirectionInverted)
        {
            this._FirstValueConversion = firstValueConversion;
            this._FirstValueConversionDirectionInverted = firstValueConversionDirectionInverted;
            this._SecondValueConversion = secondValueConversion;
            this._SecondValueConversionDirectionInverted = secondValueConversionDirectionInverted;
        }

        public override double ConvertFromPrimaryUnit(double value)
        {
            return this._SecondValueConversion.Convert(this._FirstValueConversion.Convert(value, this._FirstValueConversionDirectionInverted), this._SecondValueConversionDirectionInverted);
        }

        public override double ConvertToPrimaryUnit(double value)
        {
            return this._FirstValueConversion.Convert(this._SecondValueConversion.Convert(value, !this._SecondValueConversionDirectionInverted), !this._FirstValueConversionDirectionInverted);
        }
    }

    #endregion Value Conversion Classes
    
    #region Physical Unit Classes

    public class NamedObject : INamed
    {
        private String _name;
        public String Name { get { return _name; } set { _name = value; } }

        public NamedObject(String someName)
        {
            this.Name = someName;
        }
    }

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

        public abstract IUnitSystem System { get; set; }


        public abstract UnitKind Kind { get; }
        public abstract SByte[] Exponents { get; }


        public virtual IPhysicalUnit Dimensionless { get { return Physics.dimensionless; } }
        public virtual Boolean IsDimensionless 
        { 
            get 
            {
                return DimensionExponents.IsDimensionless(Exponents);
            } 
        }

        public override int GetHashCode()
        {
            return this.System.GetHashCode() + this.Exponents.GetHashCode();
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
            //return ParseUnitLALR(pu, ref unitString);


            Char timeSeparator = ':';
            Char[] separators = { timeSeparator };

            Char FractionUnitSeparator = '\0';
            String FractionUnitSeparatorStr = null;

            int UnitStrCount = 0;
            int UnitStrStartCharIndex = 0;
            int NextUnitStrStartCharIndex = 0;
            Boolean ValidFractionalUnit = true;

            Stack<Tuple<string, IPhysicalUnit>> FractionalUnits = new Stack<Tuple<string, IPhysicalUnit>>();

            while (ValidFractionalUnit && (UnitStrStartCharIndex >= 0) && (UnitStrStartCharIndex < unitString.Length))
            {
                int UnitStrLen;

                int UnitStrSeparatorCharIndex = unitString.IndexOfAny(separators, UnitStrStartCharIndex);
                if (UnitStrSeparatorCharIndex == -1)
                {
                    UnitStrLen = unitString.Length - UnitStrStartCharIndex;
                    //FractionUnitSeparator = '\0';

                    NextUnitStrStartCharIndex = unitString.Length;
                }
                else
                {
                    UnitStrLen = UnitStrSeparatorCharIndex - UnitStrStartCharIndex;
                    //FractionUnitSeparator = UnitString[UnitStrSeparatorCharIndex];

                    NextUnitStrStartCharIndex = UnitStrSeparatorCharIndex + 1;
                }

                if (UnitStrLen > 0)
                {
                    UnitStrCount++;
                    string UnitFieldString = unitString.Substring(UnitStrStartCharIndex, UnitStrLen).Trim();

                    IPhysicalUnit tempPU = ParseUnitLALR(pu, ref UnitFieldString);

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
                    }
                }

                // Shift to next field
                if (UnitStrSeparatorCharIndex >= 0)
                {
                    FractionUnitSeparator = unitString[UnitStrSeparatorCharIndex];
                }
                UnitStrStartCharIndex = NextUnitStrStartCharIndex;
            }

            unitString = unitString.Substring(NextUnitStrStartCharIndex);

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
                        Debug.Assert(FractionUnitSeparatorStr != null);
                        pu = new PhysicalMeasure.MixedUnit(tempPU, FractionUnitSeparatorStr, pu);

                        FractionUnitSeparatorStr = tempFractionUnitSeparator;
                    }
                    else
                    {
                        Debug.Assert(resultLine == null);
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

        // Token kind enums
        enum TokenKind
        {
            None = 0,
            Unit = 1,
            Exponent = 2,
            Operator = 3
        }


        // Operator kind enums
        // Precedence for a group of operators is same as first (lowest) enum in the group
        enum OperatorKind
        {
            // Precediens == 2
            Mult = 2,
            Div = 3,

            // Precediens == 4
            Pow = 4,
            Root = 5,
        }

        static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            //OperatorKind precedence = (operatoren == OperatorKind.Div ? OperatorKind.Mult : operatoren);
            OperatorKind precedence = (OperatorKind )((int)operatoren & 0XE);

            return precedence;
        }

        class token
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

            public token(sbyte exponent)
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

        class expressionTokenizer
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

            TokenKind LastReadToken = TokenKind.None;
            
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
                //if (Operators.Count == 0 && Tokens.Count == 0) 
                if (Operators.Count <= 1 && Tokens.Count == 0) 
                {
                    //LastValidPos = Pos;
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

            private token RemoveFirstToken()
            {   // return first operator from post fix operators
                token Token = Tokens[0];
                Tokens.RemoveAt(0);

                return Token;
            }

            public token GetToken()
            {
                Debug.Assert(InputString != null);

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }
                int OperatorsCountForRecognizedTokens = Operators.Count;
                while ((InputString.Length > Pos) && (InputRecognized))
                {
                    Char c = InputString[Pos];
                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        Pos++;
                    }
                    else if (   c == '*'
                             || c == '·') // centre dot  '\0x0B7' (char)183 U+00B7
                    {
                        if (PushNewOperator(OperatorKind.Mult))
                        {
                            Pos++;
                        }
                        else
                        {
                            // return null;
                            InputRecognized = false;
                        }
                    }
                    else if (c == '/') 
                    {
                        if (PushNewOperator(OperatorKind.Div))
                        {
                            Pos++;
                        }
                        else
                        {
                            // return null;
                            InputRecognized = false;
                        }
                    }
                    else if (c == '^')
                    {
                        if (PushNewOperator(OperatorKind.Pow))
                        {
                            Pos++;
                        }
                        else
                        {
                            // return null;
                            InputRecognized = false;
                        }
                    }
                    else if (   c == '-'
                             || c == '+'
                             || Char.IsDigit(c))
                    {
                        // An exponent
                        if (   (LastReadToken != TokenKind.Unit)                // Exponent can follow unit directly 
                            && ((LastReadToken != TokenKind.Operator)          // or follow pow operator 
                                 || (Operators.Peek() != OperatorKind.Pow)))
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An exponent must follow a unit or pow operator. Invalid exponent at '" + c + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                // return null;
                                InputRecognized = false;
                            }
                        }
                        else
                        {
                            // Try to read an exponent from input

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
                                    // Exponent follow pow operator; 
                                    // Remove pow operator from operator stack since it is handled as implicit in parser.
                                    Operators.Pop();
                                }

                                //InputString = InputString.Substring(numlen);
                                Pos += numlen;
                                AfterLastOperandPos = Pos;

                                //puRes = pu.CombinePow(exponent);
                                //Operators.Push(exponent);
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
                            || ((LastReadToken == TokenKind.Operator)    // Unit follow pow operator; 
                                && (Operators.Peek() == OperatorKind.Pow)))
                         */
                        if (   (LastReadToken == TokenKind.Operator)    // Unit follow pow operator; 
                            && (Operators.Peek() == OperatorKind.Pow))
                        {
                            if (ThrowExceptionOnInvalidInput)
                            {
                                //throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An unit must not follow an unit. Missing operator at '" + c + "' at position " + Pos.ToString());
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An unit must not follow an pow operator. Missing exponent at '" + c + "' at position " + Pos.ToString());
                            }
                            else
                            {
                                // return null;
                                InputRecognized = false;
                            }
                        }
                        else
                        {
                            // Try to read a unit from input
                            int maxlen = Math.Min(1 + 3, InputString.Length - Pos); // Max length of scale and symbols to look for

                            String tempstr = InputString.Substring(Pos, maxlen);
                            maxlen = tempstr.IndexOfAny((new char[] { ' ', '*', '·', '/', '^', '+', '-', '(', ')' }));  // '·'  centre dot '\0x0B7' (char)183 U+00B7
                            if (maxlen < 0)
                            {
                                maxlen = tempstr.Length;
                            }

                            for (int unitlen = maxlen; unitlen > 0; unitlen--)
                            {
                                String UnitStr = tempstr.Substring(0, unitlen);
                                IPhysicalUnit su = Physics.ScaledUnitFromSymbol(UnitStr);
                                if (su != null)
                                {
                                    if (LastReadToken == TokenKind.Unit)
                                    {   // Assume implicit mult operator
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
                                // return null;
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
                // Retrieve remaining operators from stack
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

        public static IPhysicalUnit ParseUnitLALR(IPhysicalUnit dimensionless, ref String s)
        {
            if (dimensionless == null)
            {
                dimensionless = Physics.dimensionless;
            }

            expressionTokenizer Tokenizer = new expressionTokenizer(dimensionless, /* throwExceptionOnInvalidInput = */ false, s);

            // Tokenizer.ThrowExceptionOnInvalidInput = false;
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
                /*
                else if (Token.TokenKind == TokenKind.Exponent)
                {
                    // Stack exponent operand
                    Operands.Push(Token.PhysicalUnit);
                }
                */
                else if (Token.TokenKind == TokenKind.Exponent)
                {
                    IPhysicalUnit pu = Operands.Pop();

                    // Combine pu and expont to the new unit pu^exponent   
                    Operands.Push(pu.CombinePow(Token.Exponent));
                }
                else if (Token.TokenKind == TokenKind.Operator)
                {

                    /****
                     * pow operator is handled implicit
                     * 
                    if (Token.Operator == OperatorKind.Pow)
                    {
                        Debug.Assert(Operands.Count >= 1);
                        SByte exponentSecond = Operands.Pop();
                        IPhysicalUnit puFirst = Operands.Pop();
                        // Combine pu and expont to the new unit pu^exponent   
                        Operands.Push(puFirst.CombinePow(exponentSecond));
                    }
                    else
                    ****/
                    if (Operands.Count >= 2)
                    {
                        Debug.Assert(Operands.Count >= 2);

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
                    {   // Missing operant(s). Operator not valid part of (this) unit
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

            //s = Tokenizer.GetRemainingInput(); // Remaining of input string
            s = Tokenizer.GetRemainingInputForLastValidPos(); // Remaining of input string

            Debug.Assert(Operands.Count <= 1);  // 0 or 1
            
            //return (Operands.Count > 0) ? Operands.Pop() : null;
            return (Operands.Count > 0) ? Operands.Last() : null;
        }

        public static IPhysicalUnit ParseUnit(IPhysicalUnit dimensionless, ref String s)
        {
            if (dimensionless == null)
            {
                dimensionless = Physics.dimensionless;
            }

            IPhysicalUnit pu = ParseUnitFactor(dimensionless, ref s);
            if (pu != null)
            {
                pu = ParseOptionalUnit(pu, ref s);
            }
            return pu;
        }

        public static IPhysicalUnit ParseOptionalUnit(IPhysicalUnit physicalUnit, ref String s)
        {
            Debug.Assert(physicalUnit != null);

            if (physicalUnit == null)
            {
                throw new ArgumentNullException("physicalUnit");
            }

            IPhysicalUnit puRes = physicalUnit;
            if (!String.IsNullOrEmpty(s))
            {
                if (   (s[0] == '*')
                    || (s[0] == '·')) // centre dot  '\0x0B7' (char)183 U+00B7
                {
                    s = s.Substring(1);
                    IPhysicalUnit pu2 = ParseUnit(physicalUnit.Dimensionless, ref s);
                    if (pu2 != null)
                    {   // Combine pu and pu2 to the new unit pu*pu2
                        puRes = physicalUnit.CombineMultiply(pu2);
                    }
                    else
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing operand after '*'");
                    }
                }
                else if (s[0] == '/')
                {
                    s = s.Substring(1);
                    IPhysicalUnit pu2 = ParseUnit(physicalUnit.Dimensionless, ref s);
                    if (pu2 != null)
                    {   // Combine pu and pu2 to the new unit pu/pu2
                        puRes = physicalUnit.CombineDivide(pu2);
                    }
                    else
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing operand after '/'");
                    }
                }
                else if (s[0] == ' ')
                {   // Maybe an implicit *
                    // Check if it is a valid unit
                    try
                    {
                        String TempStr = s.Substring(1); // Skip ' '
                        IPhysicalUnit pu2 = ParseUnit(physicalUnit.Dimensionless, ref TempStr);
                        if (pu2 != null)
                        {   // Combine pu and pu2 to the new unit pu*pu2
                            puRes = physicalUnit.CombineMultiply(pu2);
                            s = TempStr;
                        }
                    }
                    //catch (PhysicalUnitFormatException e)
                    catch (PhysicalUnitFormatException)
                    {

                    };
                }
            }
            return puRes;
        }

        public static IPhysicalUnit ParseUnitFactor(IPhysicalUnit dimensionless, ref String s)
        {
            IPhysicalUnit pu = null;
            if (!String.IsNullOrEmpty(s))
            {
                if (s[0] == '(')
                { // paranteses
                    s = s.Substring(1); // Skip begin parantes '('
                    pu = ParseUnit(dimensionless, ref s);
                    if (s[0] == ')')
                    {
                        s = s.Substring(1); // Skip end parantes ')'
                    }
                    else
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Missing end brackets ')'");
                    }
                }
                else
                {
                    int i = s.IndexOfAny(new char[] { ' ', '*', '·', '/', '+', '-', '(', ')' });  // '·'  centre dot '\0x0B7' (char)183 U+00B7
                    if (i < 0)
                    {
                        i = s.Length;
                    }
                    int maxlen = Math.Min(i, 1 + 3); // Max length of scale and symbols to look for
                    for (int len = maxlen; len > 0; len--)
                    {
                        String UnitStr = s.Substring(0, len);
                        IPhysicalUnit su = Physics.ScaledUnitFromSymbol(UnitStr);
                        if (su != null)
                        {
                            s = s.Substring(len);
                            pu = ParseOptionalExponent(su, ref s);
                            break;
                        }
                    }
                }
            }
            return pu;
        }


        public static IPhysicalUnit ParseOptionalExponent(IPhysicalUnit physicalUnit, ref String s)
        {
            Debug.Assert(physicalUnit != null);
            if (physicalUnit == null)
            {
                throw new ArgumentNullException("physicalUnit");
            }

            IPhysicalUnit puRes = physicalUnit;
            if (!String.IsNullOrEmpty(s))
            {
                int numlen = 0;

                if ((s[0] == '-') || (s[0] == '+'))
                {
                    numlen = 1;
                }

                int maxlen = Math.Min(s.Length, 1 + 3); // Max length of sign and digits to look for
                while (numlen < maxlen && Char.IsDigit(s[numlen]))
                {
                    numlen++;
                }
                if (numlen > 0)
                {
                    SByte exponent = SByte.Parse(s.Substring(0, numlen));
                    puRes = physicalUnit.CombinePow(exponent);
                    s = s.Substring(numlen);
                }
            }
            return puRes;
        }

        #endregion IPhysicalUnit unit expression parser methods

        #endregion Unit Expression parser methods

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public abstract String UnitString();

                /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public virtual String UnitPrintString()
        {
            return UnitString();
        }

        public virtual String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            // 2012-01-15 ?? Stil valid to assert invertExponents == false ?? 
            Debug.Assert(invertExponents == false);
            return UnitString();
        }

        /// <summary>
        /// String with formatted by use of named derivated unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public virtual String ReducedUnitString()
        {
            return UnitString();
        }


        /// <summary>
        /// IFormattable.ToString implementation.
        /// Eventyally with system name prefixed.
        /// </summary>
        public override String ToString()
        {
            String UnitName = UnitString();
            IUnitSystem system = this.System;
            if (   (!String.IsNullOrEmpty(UnitName)) 
                && (system != null)
                && (system != Physics.Default_UnitSystem))
            {
                UnitName = this.System.Name + "." + UnitName;
            }

            return UnitName;
        }

        /// <summary>
        /// IPhysicalUnit.ToPrintString implementation.
        /// With system name prefixed if system specified.
        /// </summary>
        public virtual String ToPrintString()
        {
            String UnitName = UnitPrintString();
            if (String.IsNullOrEmpty(UnitName))
            {
                UnitName = "dimensionless";
            }
            else
            {
                IUnitSystem system = this.System;
                if (system != null)
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

        #region Unit convertion methods

        public virtual IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            double quantity = 1;
            return ConvertTo(ref quantity, convertToUnit);
        }

        public virtual IPhysicalQuantity ConvertTo(ref double quantity, IPhysicalUnit convertToUnit)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this);
            if (convertToUnit != this)
            {
                pq = this.System.ConvertTo(pq, convertToUnit);
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            double quantity = 1;
            return this.ConvertTo(ref quantity, convertToUnitSystem);
        }

        public virtual IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this);
            // Mark quantity as used now
            quantity = 1;
            if (convertToUnitSystem != this.System)
            {
                pq = this.System.ConvertTo(pq, convertToUnitSystem);
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertToSystemUnit()
        {
            double quantity = 1;
            return this.ConvertToSystemUnit(ref quantity);
        }

        public abstract IPhysicalQuantity ConvertToSystemUnit(ref double quantity);

        public virtual IPhysicalQuantity ConvertToBaseUnit()
        {
            return this.ConvertToBaseUnit(1);
        }

        public abstract IPhysicalQuantity ConvertToBaseUnit(double quantity);

        public abstract IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity);
        
        public virtual bool Equals(IPhysicalUnit other)
        {
            if (this.System != other.System)
            {   // Must be same unit system
                return new PhysicalQuantity(1, this) == other.ConvertTo(this.System);
            } 
            if (   (this.Kind != UnitKind.ConvertibleUnit)
                && (other.Kind != UnitKind.ConvertibleUnit)
                && (this.Kind != UnitKind.MixedUnit)
                && (other.Kind != UnitKind.MixedUnit))
            {
                //return (this.Exponents == other.Exponents));
                return DimensionExponents.Equals(this.Exponents, other.Exponents);
            }

            IPhysicalQuantity pq1 = this.ConvertToSystemUnit();
            IPhysicalQuantity pq2 = other.ConvertToSystemUnit();

            return pq1.Equals(pq2);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            if (obj is IPhysicalUnit)
            {
                return Equals(obj as IPhysicalUnit);
            }
            
            // throw new InvalidCastException("The 'obj' argument is not a IPhysicalUnit object.");
            Debug.Assert(obj is IPhysicalUnit);

            return false;
        }

        public static bool operator ==(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            Debug.Assert(null != unit1);

            return unit1.Equals(unit2);
        }

        public static bool operator !=(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            //Debug.Assert(null != unit1);

            return (!unit1.Equals(unit2));
        }

        #endregion Unit convertion methods

        #region Unit static operator methods

        //public delegate SByte CombineExponentsFunc(SByte e1, SByte e2);
        //public delegate Double CombineQuantitiesFunc(Double q1, Double q2);
        //protected internal 
        internal delegate SByte CombineExponentsFunc(SByte e1, SByte e2);
        //protected internal 
        internal delegate Double CombineQuantitiesFunc(Double q1, Double q2);

        // public
        // protected internal 
        internal static PhysicalQuantity CombineUnits(IPhysicalUnit u1, IPhysicalUnit u2, CombineExponentsFunc cef, CombineQuantitiesFunc cqf)
        {
            IPhysicalQuantity u1_pq = u1.ConvertToSystemUnit();
            IPhysicalQuantity u2_pq = u2.ConvertToSystemUnit();
            if (u2_pq.Unit.System != u1_pq.Unit.System)
            {
                u2_pq = u1_pq.Unit.System.ConvertTo(u2_pq, u1_pq.Unit.System);
                u2_pq = u2_pq.ConvertToSystemUnit();
            }
            SByte[] u1Exponents = u1_pq.Unit.Exponents;
            SByte[] u2Exponents = u2_pq.Unit.Exponents;
            SByte u1ExponentsLen = (SByte)u1_pq.Unit.Exponents.Length;
            SByte u2ExponentsLen = (SByte)u2_pq.Unit.Exponents.Length;
            int NoOfBaseUnits = Math.Max(u1ExponentsLen, u2ExponentsLen);
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

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
            Debug.Assert(u1.System != null);
            PhysicalUnit pu = new DerivedUnit(u1.System, combinedExponents);
            return new PhysicalQuantity(cqf(u1_pq.Value, u2_pq.Value), pu);
        }

        // public
        // protected internal 
        /*
        internal static PhysicalQuantity CombineUnitExponents(IPhysicalUnit u, SByte exponent, CombineExponentsFunc cef)
        {
            SByte[] exponents = u.Exponents;
            int NoOfBaseUnits = exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);
            SByte[] someexponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                someexponents[i] = cef(u.Exponents[i], exponent);
            }
            Debug.Assert(u.System != null);
            PhysicalUnit pu = new DerivedUnit(u.System, someexponents);
            return new PhysicalQuantity(1, pu);
        }
        */

        // public
        // protected internal 
        internal static PhysicalUnit CombineUnitExponents(IPhysicalUnit u, SByte exponent, CombineExponentsFunc cef)
        {
            SByte[] exponents = u.Exponents;
            int NoOfBaseUnits = exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfMeasures);

            SByte[] someexponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                someexponents[i] = cef(u.Exponents[i], exponent);
            }

            // Not valid during SI system initialisation: Debug.Assert(u.System != null);
            PhysicalUnit pu = new DerivedUnit(u.System, someexponents);
            return pu;
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, IUnitPrefix up)
        {
            Debug.Assert(up != null);

            return new PhysicalQuantity(up.PrefixValue, u);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalUnit u)
        {
            Debug.Assert(up != null);

            return new PhysicalQuantity(up.PrefixValue, u);
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
            return new PhysicalQuantity(d, 1/u);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPhysicalUnit u2)
        {
            // Debug.Assert(u1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, u1));

            return new PhysicalQuantity(u1.Multiply(u2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPhysicalUnit u2)
        {
            //Debug.Assert(u1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, u1));

            return new PhysicalQuantity(u1.Divide(u2));
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            //Debug.Assert(u1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, u1));

            return new PhysicalQuantity(u1.Multiply(pue2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            //Debug.Assert(u1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, u1));

            return new PhysicalQuantity(u1.Divide(pue2));
        }

        public static PhysicalQuantity operator ^(PhysicalUnit u, SByte exponent)
        {
            //Debug.Assert(u != null);
            Debug.Assert(!Object.ReferenceEquals(null, u));

            return new PhysicalQuantity(u.Pow(exponent));
        }

        public static PhysicalQuantity operator %(PhysicalUnit u, SByte exponent)
        {
            // Debug.Assert(u != null);
            Debug.Assert(!Object.ReferenceEquals(null, u));
            return new PhysicalQuantity(u.Rot(exponent));
        }

        #endregion Unit static operator methods


        //
        public virtual PhysicalQuantity Power(SByte exponent)
        //public virtual PhysicalUnit Power(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 * e2));
        }

        //
        public virtual PhysicalQuantity Root(SByte exponent)
        //public virtual PhysicalUnit Root(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 / e2));
        }

        #region Unit math methods

        //
        public IPhysicalQuantity Pow(SByte exponent)
        //public PhysicalUnit Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        /*
        public PhysicalQuantity Pow(SByte exponent)
        {
            return (IPhysicalQuantity)this.Power(exponent);
        }
        */

        //
        public IPhysicalQuantity Rot(SByte exponent)
        //public IPhysicalUnit Rot(SByte exponent)
        {
            return this.Root(exponent);
        }

        /*
        public IPhysicalQuantity Rot(SByte exponent)
        {
            return (IPhysicalQuantity)this.Root(exponent);
        }
        */

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

        public virtual IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            return CombineUnits(this, physicalUnit, (SByte e1, SByte e2) => (SByte)(e1 + e2), (Double e1, Double e2) => (Double)(e1 * e2));
        }

        public virtual IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            return CombineUnits(this, physicalUnit, (SByte e1, SByte e2) => (SByte)(e1 - e2), (Double e1, Double e2) => (Double)(e1 / e2));
        }

        public virtual IPhysicalQuantity Multiply(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalQuantity pq2 = this.Multiply(physicalQuantity.Unit);
            return pq2.Multiply(quantity * physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalQuantity pq2 = this.Divide(physicalQuantity.Unit);
            return pq2.Multiply(quantity / physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalQuantity pq2 = this.Multiply(physicalQuantity.Unit);
            return pq2.Multiply(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalQuantity pq2 = this.Divide(physicalQuantity.Unit);
            return pq2.Divide(physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null);
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnit.PrefixExponent), prefixedUnit.Unit);
            return this.Multiply(pq2);
        }

        public virtual IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null);
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnit.PrefixExponent), prefixedUnit.Unit);
            return this.Divide(pq2);
        }

        public virtual IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnitExponent.PrefixExponent), prefixedUnitExponent.Unit);
            pq2 = pq2.Pow(prefixedUnitExponent.Exponent);
            return this.Multiply(pq2);
        }

        public virtual IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, prefixedUnitExponent.PrefixExponent), prefixedUnitExponent.Unit);
            return this.Divide(pq2.Pow(prefixedUnitExponent.Exponent));
        }

        #endregion Unit math methods

        #region Unit Combine math methods

        public virtual IPhysicalUnit CombineMultiply(IPhysicalUnit physicalUnit)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual IPhysicalUnit CombineDivide(IPhysicalUnit physicalUnit)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual IPhysicalUnit CombinePow(SByte exponent)
        {
            IPhysicalUnit uRes = new CombinedUnit(new PrefixedUnitExponent(0, this, exponent));
            return uRes;
        }

        public virtual IPhysicalUnit CombineRot(SByte exponent)
        {
            IPhysicalUnit uRes = new CombinedUnit(new PrefixedUnitExponent(0, this, (SByte)(-exponent)));
            return uRes;
        }

        public virtual IPhysicalUnit CombinePrefix(SByte prefixExponent)
        {
            IPhysicalUnit uRes = new CombinedUnit(new PrefixedUnitExponent(prefixExponent, this, 1));
            return uRes;
        }


        public virtual IPhysicalUnit CombineMultiply(IPrefixedUnit prefixedUnit)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnit);
            return uRes;
        }

        public virtual IPhysicalUnit CombineDivide(IPrefixedUnit prefixedUnit)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnit);
            return uRes;
        }

        public virtual IPhysicalUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnitExponent);
            return uRes;
        }

        public virtual IPhysicalUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnitExponent);
            return uRes;
        }

        #endregion Unit Combine math methods

        public static implicit operator PhysicalQuantity(PhysicalUnit physicalUnit)
        {
            //return physicalunit. .PhysicalQuantity() as PhysicalQuantity;
            return new PhysicalQuantity(physicalUnit);
        }
    }

    public abstract class SystemUnit : PhysicalUnit, ISystemUnit/* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        private IUnitSystem _system;

        // property override get
        public override IUnitSystem System { get { return _system; } set { _system = value; } }

        protected SystemUnit(IUnitSystem someSystem = null)
        {
            this._system = someSystem;
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(quantity, this);
            return pq;
        }
    }

    public class BaseUnit : SystemUnit, INamedSymbol, IBaseUnit
    {
        private NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        private SByte _baseunitnumber;
        public SByte BaseUnitNumber { get { return _baseunitnumber; } }

        public override UnitKind Kind { get { return UnitKind.BaseUnit; } }

        public override SByte[] Exponents 
        { 
            get 
            {
                int NoOfBaseUnits = _baseunitnumber + 1;
                if (System != null && System.BaseUnits != null)
                {
                    NoOfBaseUnits = System.BaseUnits.Length;
                }

                SByte[] tempExponents = new SByte[NoOfBaseUnits]; 
                tempExponents[_baseunitnumber] = 1; 
                return tempExponents; 
            } 
        } 

        public BaseUnit(IUnitSystem someUnitSystem, SByte someBaseUnitNumber, NamedSymbol someNamedSymbol)
            : base(someUnitSystem)
        {
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
        public override String UnitString()
        {
            return this.Symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return new PhysicalQuantity(quantity, this);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            return physicalQuantity.ConvertTo(this); 
        }

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
            : this(null, someExponents)
        {
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String UnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit);

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
                        ExponentsStr += '·'; // centre dot '\0x0B7' (char)183 U+00B7
                    }
                    if (this.System != null)
                    {
                        ExponentsStr += this.System.BaseUnits[index].Symbol;
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


        /// <summary>
        /// 
        /// </summary>
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

    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, ISystemUnit, IPhysicalUnit, INamedDerivedUnit
    {
        private readonly NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } }

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
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        override public String UnitString()
        {
            return ReducedUnitString();
        }

        /// <summary>
        /// String with formatted by use of named derivated unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit);

            return Symbol;
        }


        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return new PhysicalQuantity(quantity, new DerivedUnit(this.System, this.Exponents));
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
            : base(somePrimaryUnit != null ? somePrimaryUnit.System : null)
        {
            this._NamedSymbol = someNamedSymbol;
            _primaryunit = somePrimaryUnit;
            _conversion = someConversion;
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
        public override String UnitString()
        {
            return ReducedUnitString();
        }


        /// <summary>
        /// String with formatted by use of named derivated unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.ConvertibleUnit);

            return this.Symbol;
        }

        public IPhysicalQuantity ConvertFromPrimaryUnit()
        {
            return this.ConvertFromPrimaryUnit(1);
        }

        public IPhysicalQuantity ConvertToPrimaryUnit()
        {
            return this.ConvertToPrimaryUnit(1);
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

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return PrimaryUnit.ConvertToBaseUnit(new PhysicalQuantity(quantity, this));
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            return PrimaryUnit.ConvertToBaseUnit(physicalQuantity.ConvertTo(this));
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
                return pq.ConvertTo(convertToUnit);
                // throw new ArgumentException("Physical unit is not convertibel to a " + convertToUnit.ToString());
            }
        }

        public override PhysicalQuantity Power(SByte exponent)
        {
            return new PhysicalQuantity(this.ConvertToSystemUnit().Pow(exponent));
        }

        public override PhysicalQuantity Root(SByte exponent)
        {
            return new PhysicalQuantity(this.ConvertToSystemUnit().Rot(exponent));
        }
    }

    #region Combined Unit Classes

    public class PrefixedUnit : IPrefixedUnit
    {
        private readonly SByte _PrefixExponent;
        private readonly IPhysicalUnit _Unit;

        public SByte PrefixExponent { get { return _PrefixExponent; }  }
        public IPhysicalUnit Unit { get { return _Unit; }  }

        public PrefixedUnit(IPhysicalUnit unit)
            : this(0, unit)
        {
        }

        public PrefixedUnit(SByte PrefixExponent, IPhysicalUnit unit)
        {
            this._PrefixExponent = PrefixExponent;
            this._Unit = unit;
        }

        public virtual IPhysicalQuantity PhysicalQuantity()
        {
            IPhysicalQuantity pue_pq = Unit.Multiply(Math.Pow(10.0, _PrefixExponent));
            return pue_pq;
        }

        public virtual IPhysicalQuantity PhysicalQuantity(ref double quantity, IUnitSystem unitSystem)
        {
            double dd = quantity;
            if (_PrefixExponent != 0)
            {
                dd *= Math.Pow(10.0, _PrefixExponent);
            }
            IPhysicalQuantity pue_pq = _Unit.ConvertTo(ref dd, unitSystem);
            if (dd == 1.0)
            {
                quantity = 1.0;
            }
            return pue_pq;
        }

        public static implicit operator PhysicalQuantity(PrefixedUnit prefixedUnit)
        {
            return prefixedUnit.PhysicalQuantity() as PhysicalQuantity;
        }
    }

    public class PrefixedUnitExponent : PrefixedUnit, IPrefixedUnitExponent
    {
        private readonly SByte _Exponent;

        public SByte Exponent { get { return _Exponent; } }

        public PrefixedUnitExponent(IPhysicalUnit Unit)
            : this(0, Unit, 1)
        {
        }

        public PrefixedUnitExponent(IPhysicalUnit Unit, SByte Exponent)
            : this(0, Unit, Exponent)
        {
        }

        public PrefixedUnitExponent(IPrefixedUnitExponent prefixedUnitExponent)
            : this(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, prefixedUnitExponent.Exponent)
        {
        }

        public PrefixedUnitExponent(SByte prefixExponent, IPhysicalUnit unit, SByte exponent)
            : base(prefixExponent, unit)
        {
            this._Exponent = exponent;
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        // public override String ToString()

        public override String ToString()
        {
            return this.CombinedUnitString();
        }

        public String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";
            Debug.Assert(_Exponent != 0);

            if (PrefixExponent != 0)
            {
                char Prefix;
                if (Physics.UnitPrefixes.GetPrefixCharFromExponent(PrefixExponent, out Prefix))
                {
                    Str += Prefix;
                }
                else
                {
                    Debug.Assert(PrefixExponent == 0);
                }
            }
            Str += Unit.CombinedUnitString(mayUseSlash, invertExponents);
            SByte expo = Exponent;
            if (invertExponents)
                expo = (SByte)(-expo);
            if (expo != 1)
            {
                Str += expo.ToString();
            }

            return Str;
        }

        public override IPhysicalQuantity PhysicalQuantity()
        {
            IPhysicalQuantity pue_pq = base.PhysicalQuantity();
            if (_Exponent != 1)
            {
                pue_pq = pue_pq.Pow(_Exponent);
            }
            return pue_pq;
        }

        public override IPhysicalQuantity PhysicalQuantity(ref double quantity, IUnitSystem unitSystem)
        {
            IPhysicalQuantity pue_pq = base.PhysicalQuantity(ref quantity, unitSystem);
            if (_Exponent != 1)
            {
                pue_pq = pue_pq.Pow(_Exponent);
            }
            return pue_pq;
        }

        public IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out Double scaleFactor)
        {
            SByte CombinedPrefix = 0;
            if (this.Exponent == 1 || outerPUE_PrefixExponent == 0)
            {
                scaleFactor = 1;
                CombinedPrefix = outerPUE_PrefixExponent;
            }
            else
            {
                int reminder;
                CombinedPrefix = (SByte)Math.DivRem(outerPUE_PrefixExponent, this.Exponent, out reminder);
                if ((reminder != 0))
                {
                    //scaleFactor = Math.Pow(10, outerPUE_PrefixExponent * 1.0 / this.Exponent);
                    scaleFactor = Math.Pow(10, 1.0 * CombinedPrefix);
                    CombinedPrefix = 0;
                }
                else
                {
                    scaleFactor = 1;
                }
            }

            PrefixedUnitExponent CombinedPUE = new PrefixedUnitExponent((SByte)(CombinedPrefix + this.PrefixExponent), this.Unit, (SByte)(this.Exponent * outerPUE_Exponent));
            return CombinedPUE;
        }

        public static implicit operator PhysicalQuantity(PrefixedUnitExponent prefixedUnitExponent)
        {
            return prefixedUnitExponent.PhysicalQuantity() as PhysicalQuantity;
        }
    }

    public class PrefixedUnitExponentList : List<IPrefixedUnitExponent>
    {

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";

            foreach (IPrefixedUnitExponent ue in this)
            {
                Debug.Assert(ue.Exponent != 0);
                if (!String.IsNullOrEmpty(Str))
                {
                    Str += '·';  // centre dot '\0x0B7' (char)183 U+00B7
                }

                Str += ue.CombinedUnitString(mayUseSlash, invertExponents);
            }
            return Str;
        }

        public PrefixedUnitExponentList Power(SByte exponent)
        {
            PrefixedUnitExponentList Result = new PrefixedUnitExponentList();
            foreach (IPrefixedUnitExponent pue in this)
            {
                Debug.Assert(pue.PrefixExponent == 0);
                SByte NewExponent = (SByte)(pue.Exponent * exponent);

                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(pue.PrefixExponent, pue.Unit, NewExponent);

                Result.Add(result_pue);
            }

            return Result;
        }

        public PrefixedUnitExponentList Root(SByte exponent)
        {
            PrefixedUnitExponentList Result = new PrefixedUnitExponentList();
            foreach (IPrefixedUnitExponent pue in this)
            {
                Debug.Assert(pue.PrefixExponent == 0);

                int Remainder;
                int NewExponent = Math.DivRem(pue.Exponent, exponent, out Remainder);
                if (Remainder != 0)
                {
                    return null;
                }

                //Debug.Assert(Math.IEEERemainder(pue.Exponent, exponent) == 0);
                //Debug.Assert(AllFactorsRootIsInteger);

                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(pue.PrefixExponent, pue.Unit, (SByte)NewExponent);

                Result.Add(result_pue);
            }

            return Result;
        }
    }

    public class CombinedUnit : PhysicalUnit, ICombinedUnit
    {
        private PrefixedUnitExponentList _Numerators;
        private PrefixedUnitExponentList _Denominators;

        public PrefixedUnitExponentList Numerators { get { return _Numerators; }  }
        public PrefixedUnitExponentList Denominators { get { return _Denominators; } }

        public CombinedUnit()
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
        }

        public CombinedUnit(PrefixedUnitExponentList someNumerators, PrefixedUnitExponentList someDenominators)
        {
            this._Numerators = someNumerators != null ? someNumerators : new PrefixedUnitExponentList();
            this._Denominators = someDenominators != null ? someDenominators : new PrefixedUnitExponentList();
        }

        public CombinedUnit(IPrefixedUnitExponent prefixedUnitExponent)
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
            this._Numerators.Add(prefixedUnitExponent);
        }

        public CombinedUnit(IPhysicalUnit physicalUnit)
            : this(new PrefixedUnitExponent(0, physicalUnit, 1))
        {
        }

        public override UnitKind Kind { get { return UnitKind.CombinedUnit; } }

        public override IUnitSystem System 
        { 
            get 
            {
                IUnitSystem system = null; // No unit system
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (system == null)
                    {
                        system = pue.Unit.System;
                    }
                    else if (system != pue.Unit.System)
                    {
                        // Multible unit systems
                        return null;
                    }
                }
                return system;
            } 
            set {  /* Just do nothing */ }
        }

        public IUnitSystem SomeSystem
        {
            get
            {
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    IUnitSystem somesystem = pue.Unit.System;
                    if (somesystem != null)
                    {
                        return somesystem;
                    }
                    else if (pue.Unit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit cu = (ICombinedUnit)pue.Unit;
                        somesystem = cu.SomeSystem;
                        if (somesystem != null)
                        {
                            return somesystem;
                        }
                    }
                }

                return null;
            }
        }

        public override SByte[] Exponents { get { IUnitSystem system = this.System; return ((system != null) ? ConvertTo(system).Unit.Exponents : null); } }

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            double quatity = 1;
            return this.ConvertToSystemUnit(ref quatity);
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            IPhysicalQuantity dq;
            IUnitSystem system = this.System;
            if (system == null)
            {
                system = Physics.Default_UnitSystem;
            }
            dq = this.ConvertTo(ref quantity, system);
            dq = dq.ConvertToSystemUnit();
            return dq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return this.ConvertToSystemUnit(ref quantity).ConvertToBaseUnit();
        }

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            //return physicalQuantity.ConvertTo(this).ConvertToSystemUnit().ConvertToBaseUnit();
            return physicalQuantity.ConvertTo(this).ConvertToBaseUnit();
        }

        public override IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            return this.ConvertTo(convertToUnit.System).ConvertTo(convertToUnit);
        }

        public override IPhysicalQuantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            double quatity = 1;
            return this.ConvertTo(ref quatity, convertToUnitSystem);
        }

        public override IPhysicalQuantity ConvertTo(ref double quantity, IUnitSystem convertToUnitSystem)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.PhysicalQuantity(ref quantity, convertToUnitSystem);
                pue_pq = pue_pq.ConvertToSystemUnit();

                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                IPhysicalQuantity pue_pq = pue.PhysicalQuantity(ref quantity, convertToUnitSystem);
                pue_pq = pue_pq.ConvertToSystemUnit();

                pq = pq.Divide(pue_pq);
            }

            return pq.Multiply(quantity);
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

        public override IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            if (prefixedUnitExponent.Unit.IsDimensionless)
            {
                PhysicalQuantity pqtemp = new PhysicalQuantity(1, this);
                return pqtemp;
            }

            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            SByte multExponent = prefixedUnitExponent.Exponent;
            Boolean PrimaryUnitFound = false;
            Boolean ChangedExponentSign = false;
            // Check if pue2.Unit is already among our Numerators or Denominators
            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                if (!PrimaryUnitFound && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    PrimaryUnitFound = true;
                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    sbyte NewExponent = (sbyte)(ue.Exponent - multExponent);
                    if (NewExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
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
                if (!PrimaryUnitFound && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    PrimaryUnitFound = true;
                    // Add the found CombinedUnit exponent with ue2´s exponent; 
                    SByte NewExponent = (SByte)(ue.Exponent + multExponent);

                    if (NewExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                        TempNumerators.Add(temp_pue);
                        // Done
                    }
                    else
                    {   // Convert to Denominator
                        multExponent = (SByte)(NewExponent);
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
                    IPhysicalQuantity result_pq = new PhysicalQuantity(1, this);
                    ICombinedUnit cu2 = prefixedUnitExponent.Unit as ICombinedUnit;
                    foreach (IPrefixedUnitExponent pue2Num_pue in cu2.Numerators)
                    {
                        Double PrefixScale;
                        IPrefixedUnitExponent CombinedPUE = pue2Num_pue.CombinePrefixAndExponents(prefixedUnitExponent.PrefixExponent, multExponent, out PrefixScale);

                        result_pq = result_pq.Multiply(CombinedPUE);
                        if (PrefixScale != 1)
                        {
                            result_pq = result_pq.Multiply(PrefixScale);
                        }
                    }
                    foreach (IPrefixedUnitExponent pue2DOM_pue in cu2.Denominators)
                    {
                        Double PrefixScale;
                        IPrefixedUnitExponent CombinedPUE = pue2DOM_pue.CombinePrefixAndExponents(prefixedUnitExponent.PrefixExponent, multExponent, out PrefixScale);

                        result_pq = result_pq.Divide(CombinedPUE);
                        if (PrefixScale != 1)
                        {
                            result_pq = result_pq.Divide(PrefixScale);
                        }
                    }

                    return result_pq;
                }
                else
                {
                    if (multExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, multExponent);
                        TempNumerators.Add(temp_pue);
                    }
                    else if (multExponent < 0)
                    {
                        multExponent = (SByte)(-multExponent);
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, multExponent);
                        TempDenominators.Add(temp_pue);
                    }
                }
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            IPhysicalQuantity pq = new PhysicalQuantity(1, cu);
            return pq;
        }

        public override IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte NewExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, NewExponent);
            return this.Multiply(temp_pue);
        }

        public override IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            return this.Multiply(new PrefixedUnitExponent(0, physicalUnit, 1));
        }

        public override IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            return this.Divide(new PrefixedUnitExponent(0, physicalUnit, 1));
        }

        public override IPhysicalQuantity Multiply(double quantity)
        {
            return this * quantity;
        }

        public override IPhysicalQuantity Divide(double quantity)
        {
            return this / quantity;
        }

        //
        public override PhysicalQuantity Power(SByte exponent)
        //public override PhysicalUnit Power(SByte exponent)
        {
            /*
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                ue.Exponent *= exponent;
                TempNumerators.Add(ue);
            }

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                ue.Exponent *= exponent;
                TempDenominators.Add(ue);
            }
            */
            CombinedUnit cu = new CombinedUnit(Numerators.Power(exponent), Denominators.Power(exponent));
            PhysicalQuantity pq = new PhysicalQuantity(1, cu);
            return pq;
        }

        //
        public override PhysicalQuantity Root(SByte exponent)
        //public override PhysicalUnit Root(SByte exponent)
        {
            /*
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();
            Boolean AllFactorsRootIsInteger = true;
            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                int NewExponent;
                int Remainder = Math.DivRem(ue.Exponent, exponent, out NewExponent);
                AllFactorsRootIsInteger &= (Remainder == 0);

                //Debug.Assert(Math.IEEERemainder(ue.Exponent, exponent) == 0);
                //Debug.Assert(AllFactorsRootIsInteger);

                ue.Exponent = (SByte)NewExponent;
                TempNumerators.Add(ue);
            }

            if (AllFactorsRootIsInteger)
            {
                foreach (IPrefixedUnitExponent ue in Denominators)
                {
                    int NewExponent;
                    int Remainder = Math.DivRem(ue.Exponent, exponent, out NewExponent);
                    AllFactorsRootIsInteger &= (Remainder == 0);

                    //Debug.Assert(Math.IEEERemainder(ue.Exponent, exponent) == 0);
                    //Debug.Assert(AllFactorsRootIsInteger);

                    ue.Exponent = (SByte)NewExponent;
                    TempDenominators.Add(ue);
                }
            }
            */
            PrefixedUnitExponentList TempNumerators;
            PrefixedUnitExponentList TempDenominators = null;
            TempNumerators = Numerators.Root(exponent);
            if (TempNumerators != null)
            { 
                TempDenominators = Denominators.Root(exponent);
            }

            if ((TempNumerators != null) && (TempDenominators != null))
            {
                CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
                PhysicalQuantity pq = new PhysicalQuantity(1, cu);
                return pq;
            }
            else
            {

                SByte[] NewExponents = this.Exponents;
                if (NewExponents != null)
                {
                    NewExponents = DimensionExponents.Root(NewExponents, exponent);
                    Debug.Assert(this.System != null);
                    DerivedUnit du = new DerivedUnit(this.System, NewExponents);
                    PhysicalQuantity pq = new PhysicalQuantity(1, du);
                    return pq;
                }
                else
                {
                    Debug.Assert(NewExponents != null);
                    //if (ThrowExceptionOnUnitMathError) {
                        throw new PhysicalUnitMathException("The result of the math operation on the PhysicalUnit argument can't be represented by this implementation of PhysicalMeasure: ("+  this.ToPrintString()+").Root(" + exponent.ToString()+ ")");
                    //}
                    //return null;
                }
            }
        }

        #endregion IPhysicalUnitMath Members

        #region Combine IPhysicalUnitMath Members

        public override IPhysicalUnit CombineMultiply(IPhysicalUnit physicalUnit)
        {
            return this.CombineMultiply(new PrefixedUnitExponent(0, physicalUnit, 1));
        }

        public override IPhysicalUnit CombineDivide(IPhysicalUnit physicalUnit)
        {
            return this.CombineDivide(new PrefixedUnitExponent(0, physicalUnit, 1));
        }

    
        public override IPhysicalUnit CombinePow(SByte exponent)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                SByte NewExponent = (SByte)(ue.Exponent * exponent);
                PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                TempNumerators.Add(temp_pue);
            }

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                SByte NewExponent = (SByte)(ue.Exponent * exponent);
                PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                TempDenominators.Add(temp_pue);
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineRot(SByte exponent)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                SByte NewExponent = (SByte)(ue.Exponent / exponent);
                PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                TempNumerators.Add(temp_pue);
            }

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                SByte NewExponent = (SByte)(ue.Exponent / exponent);
                PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                TempDenominators.Add(temp_pue);
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            SByte multExponent = prefixedUnitExponent.Exponent;
            Boolean Found = false;

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                if (!Found && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    SByte NewExponent = (SByte)(ue.Exponent - multExponent);
                    if (NewExponent > 0)
                    {
                        Found = true;
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                        TempDenominators.Add(temp_pue);
                        // Done
                    }
                    else
                    {   // Convert to Numerator
                        multExponent = (SByte)(NewExponent);
                    }
                }
                else
                {
                    TempDenominators.Add(ue);
                }
            }

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                if (!Found && prefixedUnitExponent.PrefixExponent.Equals(ue.PrefixExponent) && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    // Add the found CombinedUnit exponent with ue2´s exponent; 
                    SByte NewExponent = (SByte)(ue.Exponent + multExponent);
                    if (NewExponent > 0)
                    {
                        Found = true;
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.PrefixExponent, ue.Unit, NewExponent);
                        TempNumerators.Add(temp_pue);
                        // Done
                    }
                    else
                    {   // Convert to Denominator
                        multExponent = NewExponent;
                    }
                }
                else
                {
                    TempNumerators.Add(ue);
                }
            }

            if (!Found)
            {
                Found = true;
                if (multExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, multExponent);
                    TempNumerators.Add(temp_pue);
                }
                else if (multExponent < 0)
                {
                    multExponent = (SByte)(-multExponent);
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, multExponent);
                    TempDenominators.Add(temp_pue);
                }
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte NewExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.PrefixExponent, prefixedUnitExponent.Unit, NewExponent);
            return this.CombineMultiply(temp_pue);
        }

        #endregion IPhysicalUnitMath Members

        #region IEquatable<IPhysicalUnit> Members

        public override bool Equals(IPhysicalUnit other)
        {
            IPhysicalQuantity temp = this.ConvertTo(other);
            
            if (temp == null) 
                return false;

            return temp.Equals(other);
        }

        #endregion IEquatable<IPhysicalUnit> Members


        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String UnitString()
        {
            return CombinedUnitString(mayUseSlash : true, invertExponents : false);
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
                    UnitName = "1";
                }
            }

            if (Denominators.Count > 0)
            {
                if (mayUseSlash)
                {
                    UnitName += "/" + Denominators.CombinedUnitString(false, invertExponents);
                }
                else
                {
                    // centre dot '\0x0B7' (char)183 U+00B7
                    UnitName += '·' + Denominators.CombinedUnitString(false, !invertExponents);
                }
            }
            return UnitName;
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

        public override IUnitSystem System 
        { 
            get 
            {
                Debug.Assert(_MainUnit != null);
                return MainUnit.System;
            } 
            set 
            {
                Debug.Assert(_MainUnit != null);
                /* Just do nothing */ 
                //MainUnit.System = value; 
                Debug.Assert(MainUnit.System == value); 
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

        /*
        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            double quatity = 1;
            return this.ConvertToSystemUnit(ref quatity);
        }
        */

        public override IPhysicalQuantity ConvertToSystemUnit(ref double quantity)
        {
            Debug.Assert(_MainUnit != null);
            return MainUnit.ConvertToSystemUnit(ref quantity);
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            return this.ConvertToSystemUnit(ref quantity).ConvertToBaseUnit();
        }

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            //return physicalQuantity.ConvertTo(this).ConvertToSystemUnit().ConvertToBaseUnit();
            return physicalQuantity.ConvertTo(this).ConvertToBaseUnit();
        }

        public override string UnitString()
        {
            Debug.Assert(_MainUnit != null);

            string us = MainUnit.UnitString();
            if (FractionalUnit != null)
            {
                us = us + this._Separator + FractionalUnit.UnitString();
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
                    ValStr = _MainUnit.ValueString(integralValue, format, formatProvider) + _Separator + _FractionalUnit.ValueString(fracPQConv.Value, _FractionalValueFormat, null);
                }
                else
                {
                    //ValStr = _MainUnit.ValueString(quantity, format, null);
                    ValStr = _MainUnit.ValueString(quantity, format, formatProvider);
                }
            }
            else
            {
                //ValStr = _MainUnit.ValueString(quantity, format, null);
                ValStr = _MainUnit.ValueString(quantity, format, formatProvider);
            }
            return ValStr;
        }

    }

    #endregion Mixed Unit Classes

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public class UnitSystem : NamedObject, IUnitSystem
    {
        private UnitPrefixTable _unitprefixes;
        private BaseUnit[] _baseunits;
        private NamedDerivedUnit[] _namedderivedunits;
        private ConvertibleUnit[] _convertibleunits;

        public IUnitPrefixTable UnitPrefixes { get { return _unitprefixes; } }
        public IBaseUnit[] BaseUnits { get { return _baseunits; } set { _baseunits = (BaseUnit[])value; CheckBaseUnitSystem();  } }
        public INamedDerivedUnit[] NamedDerivedUnits { get { return _namedderivedunits; } }
        public IConvertibleUnit[] ConvertibleUnits { get { return _convertibleunits; } set { _convertibleunits = (ConvertibleUnit[])value; CheckConvertibleUnitSystem(); } }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes)
            : base(someName)
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
                if (baseunit.System != this)
                {
                    Debug.Assert(baseunit.System == null);
                    baseunit.System = this;
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
                    if (namedderivedunit.System != this)
                    {
                        namedderivedunit.System = this;
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
                    if (convertibleunit.System != this)
                    {
                        convertibleunit.System = this;
                    }
                    if (convertibleunit.PrimaryUnit.System == null)
                    {
                        (convertibleunit.PrimaryUnit as PhysicalUnit).System = this;
                    }


                }
            }
        }

        public override String ToString()
        {
            return this.Name;
        }

        private static INamedSymbolUnit UnitFromName(INamedSymbolUnit[] units, String unitname)
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

        private static INamedSymbolUnit UnitFromSymbol(INamedSymbolUnit[] units, String unitsymbol)
        {
            if (units != null)
            {
                foreach (INamedSymbolUnit u in units)
                {
                    //if (u.Symbol.Equals(unitSymbol, StringComparison.OrdinalIgnoreCase))
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

        public IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol)
        {
            SByte scaleExponent = 0;
            IPhysicalUnit unit = UnitFromSymbol(scaledUnitSymbol);
            if (unit == null && scaledUnitSymbol.Length > 1)
            {   /* Check for prefixed unit */
                char prefixchar = scaledUnitSymbol[0];
                if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleExponent))
                {
                    unit = UnitFromSymbol(scaledUnitSymbol.Substring(1));
                    if (unit != null && scaleExponent != 0)
                    {
                        //unit = unit.Dimensionless.CombineMultiply(new PrefixedUnit(scaleExponent, unit));
                        unit = unit.CombinePrefix(scaleExponent);
                    }
                }
            }

            return unit;
        }

        public IPhysicalUnit NamedDerivedUnitFromUnit(IPhysicalUnit derivedUnit)
        {
            IPhysicalQuantity pq = derivedUnit.ConvertToSystemUnit();
            if (PhysicalQuantity.IsPureUnit(pq))
            {
                IPhysicalUnit derunit = PhysicalQuantity.PureUnit(pq);
                SByte [] Exponents = derunit.Exponents;
                int NoOfDimensions = DimensionExponents.NoOfDimensions(Exponents);
                if (NoOfDimensions > 1)
                {
                    foreach (NamedDerivedUnit namedderivedunit in this._namedderivedunits)
                    {
                        if (DimensionExponents.Equals(Exponents, namedderivedunit.Exponents))
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

            if (convertFromUnit == convertToUnit)
            {
                return new PhysicalQuantity(1, convertToUnit);
            }
            else
            {
                if (convertToUnit.Kind == UnitKind.CombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)convertToUnit;

                    IPhysicalQuantity pqToUnit = icu.ConvertTo(convertFromUnit);

                    if (pqToUnit != null)
                    {
                        IPhysicalQuantity pq = convertFromUnit.Divide(pqToUnit.Unit);
                    
                        if (DimensionExponents.IsDimensionless(pq.Unit.Exponents))
                        {
                            return new PhysicalQuantity(pq.Value / pqToUnit.Value, convertToUnit);
                        }
                    }

                    return null;
                }
                else if (convertFromUnit.Kind == UnitKind.CombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)convertFromUnit;
                    IPhysicalQuantity pq = icu.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertFromUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)convertFromUnit;
                    return ConvertTo(new PhysicalQuantity(icu.Conversion.ConvertToPrimaryUnit(1), icu.PrimaryUnit), convertToUnit);
                } 
                else if (convertToUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)convertToUnit;
                    IPhysicalQuantity converted_fromunit = ConvertTo(convertFromUnit, icu.PrimaryUnit);
                    if (converted_fromunit != null)
                    {
                        converted_fromunit = new PhysicalQuantity(icu.Conversion.ConvertFromPrimaryUnit(converted_fromunit.Value), convertToUnit);
                    }

                    return converted_fromunit;
                }

                if ((convertFromUnit.System == this) && (convertToUnit.System == this))
                {   /* Intra unit system conversion */
                    Debug.Assert((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit));
                    Debug.Assert((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit));

                    if (!((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of BaseUnit or a DerivedUnit", "convertFromUnit");
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or a DerivedUnit", "convertToUnit");
                    }

                    if (DimensionExponents.Equals(convertFromUnit.Exponents, convertToUnit.Exponents))
                    {
                        return new PhysicalQuantity(1, convertToUnit);
                    }
                }
                else
                {   /* Inter unit system conversion */
                    UnitSystemConversion usc = Physics.GetUnitSystemConversion(convertFromUnit.System, convertToUnit.System);
                    if (usc != null)
                    {
                        return usc.ConvertTo(convertFromUnit, convertToUnit);
                    }
                }

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit)
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

            /**
            Not correct for convertible units with offset != 0
            IPhysicalQuantity pq = ConvertTo(physicalquantity.Unit, convertToUnit);
            if (pq != null)
            {
                pq = pq.Multiply(physicalquantity.Value);
            }
            return pq;
            **/

            if (physicalQuantity.Unit == convertToUnit)
            {
                return physicalQuantity;
            }
            else
            {
                IUnitSystem convertfromunitsystem = physicalQuantity.Unit.System;
                IUnitSystem converttounitsystem = convertToUnit.System;

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

                        tempconverttounitsystem = icu.SomeSystem;
                    }
                    if (tempconverttounitsystem != null) 
                    {
                        physicalQuantity = physicalQuantity.ConvertTo(tempconverttounitsystem);

                        if (physicalQuantity != null)
                        {
                            convertfromunitsystem = physicalQuantity.Unit.System;
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
                    tempconverttounitsystem = icu.SomeSystem;
                    Debug.Assert(tempconverttounitsystem != null);

                    // ?? What TO DO here ??
                    Debug.Assert(false);
                }

                if (converttounitsystem != null && convertfromunitsystem != converttounitsystem)
                {   /* Inter unit system conversion */

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
                        convertfromunitsystem = physicalQuantity.Unit.System;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (convertfromunitsystem != null && convertfromunitsystem == converttounitsystem)
                {   /* Intra unit system conversion */

                    if (physicalQuantity.Unit.Kind == UnitKind.MixedUnit)
                    {
                        IMixedUnit imu = (IMixedUnit)physicalQuantity.Unit;

                        IPhysicalQuantity pq = new PhysicalQuantity(physicalQuantity.Value, imu.MainUnit);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
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
                    else if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)physicalQuantity.Unit;
                        double d = physicalQuantity.Value;
                        IPhysicalQuantity pq = icu.ConvertToSystemUnit(ref d);
                        Debug.Assert(pq != null);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
                    }
                    else if (convertToUnit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)convertToUnit;
                        IPhysicalQuantity ToUnit_SystemUnit_pq = icu.ConvertToSystemUnit();
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
                    }
                    else if (physicalQuantity.Unit.Kind == UnitKind.ConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)physicalQuantity.Unit;
                        IPhysicalQuantity prim_pq = icu.ConvertToPrimaryUnit(physicalQuantity.Value);
                        return ConvertTo(prim_pq, convertToUnit);
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

                    if (DimensionExponents.Equals(physicalQuantity.Unit.Exponents, convertToUnit.Exponents))
                    {
                        return new PhysicalQuantity(physicalQuantity.Value, convertToUnit);
                    }
                }

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            if (physicalQuantity.Unit.System == convertToUnitSystem)
            {
                return physicalQuantity;
            }

            {   /* Inter unit system conversion */
                UnitSystemConversion usc = Physics.GetUnitSystemConversion(physicalQuantity.Unit.System, convertToUnitSystem);
                if (usc != null)
                {
                    return usc.ConvertTo(physicalQuantity, convertToUnitSystem);
                }

                /* Missing unit system conversion from  physicalquantity.Unit.System to ToUnitSystem */
                /* TO DO Find intermediate systems with conversions between physicalquantity.Unit.System and converttounitsystem */ 
                Debug.Assert(false);

                return null;
            }
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
            // set { _BaseUnitSystem = value; }
        }

        private IUnitSystem _ConvertedUnitSystem;

        public IUnitSystem ConvertedUnitSystem
        {
            get { return _ConvertedUnitSystem; }
            // set { _ConvertedUnitSystem = value; }
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
            int NoOfNonZeroExponents = 0;
            int NoOfNonOneExponents = 0;
            int FirstNonZeroExponent = -1;

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
                        /* throw new ArgumentException("object's physical unit is not convertibel to a " + ConvertedUnitSystem.name + " unit. " + ConvertedUnitSystem.name + " does not "); */
                        return null;
                    }
                }

                i++;
            }
            double value = convertproduct;
            IPhysicalUnit unit;
            IUnitSystem unitsystem = (backwards ? BaseUnitSystem: ConvertedUnitSystem);
            if ((NoOfNonZeroExponents == 1) && (NoOfNonOneExponents == 0))
            {
                /* BaseUnit */
                unit = (IPhysicalUnit)unitsystem.BaseUnits[FirstNonZeroExponent];
            }
            else
            {   
                /* Check if it is a NamedDerivedUnit */
                unit = null;
                if (unitsystem.NamedDerivedUnits != null)
                {
                    int namedderivedunitsindex = 0;

                    while (   (namedderivedunitsindex < unitsystem.NamedDerivedUnits.Length) 
                           && !DimensionExponents.Equals(unitsystem.NamedDerivedUnits[namedderivedunitsindex].Exponents, FromUnitExponents))
                    {
                        namedderivedunitsindex++;
                    }

                    if (namedderivedunitsindex < unitsystem.NamedDerivedUnits.Length)
                    {
                        /* NamedDerivedUnit */
                        unit = (IPhysicalUnit)unitsystem.NamedDerivedUnits[namedderivedunitsindex];
                    }
                }
                if (unit == null)
                {  
                    /* DerivedUnit */
                    Debug.Assert(unitsystem != null);
                    unit = new DerivedUnit(unitsystem, FromUnitExponents);
                }
            }
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

            if ((convertFromUnit.System == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(convertFromUnit);
            }
            else
                if ((convertFromUnit.System == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(convertFromUnit);
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(physicalQuantity != null);

            if ((physicalQuantity.Unit.System == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(physicalQuantity);
            }
            else
                if ((physicalQuantity.Unit.System == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
                {
                    return this.ConvertToBaseUnitSystem(physicalQuantity);
                }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertFromUnit, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IPhysicalQuantity pq = this.ConvertTo(convertFromUnit, convertToUnit.System);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnit);
            }
            return pq;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalQuantity, IPhysicalUnit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IPhysicalQuantity pq = this.ConvertTo(physicalQuantity, convertToUnit.System);
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
            /*
            set
            {
                this._value = value;
            }
            */ 
        }

        public IPhysicalUnit Unit
        {
            get
            {
                return this._unit;
            }
            /*
            set
            {
                this._unit = value;
            }
            */
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
                    return _value.CompareTo(tempconverted.Value);
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
            String ValStr = this.Unit.ValueString(this.Value, format, formatProvider);
            String UnitStr = this.Unit.ToString();
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
            String ValStr = this.Unit.ValueString(this.Value);
            String UnitStr = this.Unit.ToString();
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
            String ValStr = this.Unit.ValueString(this.Value);
            String UnitStr = this.Unit.ToPrintString();
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
        public static IPhysicalQuantity Parse(String physicalQuantityStr, System.Globalization.NumberStyles styles, IFormatProvider provider)
        {
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

                return new PhysicalQuantity(NumValue, unit);
            }

            throw new ArgumentException("Not a valid physical quantity format", "physicalQuantityStr");
        }


        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitSymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static IPhysicalQuantity Parse(String physicalQuantityStr)
        {
            //return Parse(physicalQuantityStr, System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            return Parse(physicalQuantityStr, System.Globalization.NumberStyles.Float, null);
        }

        public IPhysicalQuantity Zero { get { return new PhysicalQuantity(0, this.Unit.Dimensionless); } }
        public IPhysicalQuantity One { get { return new PhysicalQuantity(1, this.Unit.Dimensionless); } }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() + this.Unit.GetHashCode();
        }

        public IPhysicalQuantity ConvertToSystemUnit()
        {
            double d = this.Value;
            IPhysicalQuantity pq = this.Unit.ConvertToSystemUnit(ref d);
            return pq;
        }


        public IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = this.Unit.ConvertToBaseUnit(this.Value);
            return pq;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
            if (this.Unit == convertToUnit)
            {
                return this;
            }
            Debug.Assert(this.Unit != null);
            Debug.Assert(convertToUnit != null);

            if (this.Unit == null)
            {
                if (convertToUnit.IsDimensionless)
                {   // Any dimensionless can be converted to any systems dimensionless
                    // this.Unit = convertToUnit;
                    IPhysicalQuantity quantity = new PhysicalQuantity(this.Value, convertToUnit);
                    return quantity;
                }
                else
                {
                    throw new InvalidOperationException("Must have a unit to convert it");
                }
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException("convertToUnit");
            }

            IUnitSystem converttounitsystem = convertToUnit.System; 
            if (converttounitsystem == null)
            {
                converttounitsystem = this.Unit.System;
                Debug.WriteLine("converttounitsystem assigned from this.Unit.System");
            }
            if (converttounitsystem == null)
            {
                Debug.WriteLine("converttounitsystem assigned from Physics.Default_UnitSystem");
                converttounitsystem = Physics.Default_UnitSystem;
            }

            if (converttounitsystem != null)
            {
                IPhysicalQuantity quantity = converttounitsystem.ConvertTo(this as IPhysicalQuantity, convertToUnit);
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
            if (this.Unit.System == convertToUnitSystem)
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

            if (this.Unit.System != null)
            {
                IPhysicalQuantity quantity = this.Unit.System.ConvertTo(this as IPhysicalQuantity, convertToUnitSystem);
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

            if (!IsPureUnit(physicalQuantity))
            {
                throw new ArgumentException("Physical quantity is not a pure unit; but has a value = " + physicalQuantity.Value.ToString());
            }

            return physicalQuantity.Unit;
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

        public int ValueCompare(double otherValue)
        {   /* Limited precision handling */
            double RelativeDiff = (this.Value - otherValue) / this.Value;
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

        public bool Equals(IPhysicalQuantity other)
        {
            Debug.Assert(other != null);
            Debug.Assert(this.Unit != null);
            Debug.Assert(other.Unit != null);

            if (this.Unit != other.Unit)
            {
                other = other.ConvertTo(this.Unit);
                if (other == null)
                {
                    return false;
                }
            }
            // return double.Equals(this.Value, other.Value);
            return this.ValueCompare(other.Value) == 0;
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

            if (obj is IPhysicalQuantity)
            {
                return Equals(obj as IPhysicalQuantity);
            }

            if (obj is IPhysicalUnit)
            {
                return Equals(obj as IPhysicalUnit);
            }

            //throw new InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object or IPhysicalUnit object.");
            Debug.Assert(obj is IPhysicalQuantity || obj is IPhysicalUnit);

            return false;
        }

        public static bool operator ==(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.Equals(pq2);
        }

        public static bool operator !=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return (!pq1.Equals(pq2));
        }

        public static bool operator <(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) < 0;
        }

        public static bool operator <=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) <= 0;
        }

        public static bool operator >(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) > 0;
        }

        public static bool operator >=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            // Debug.Assert(pq1 != null);
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) >= 0;
        }


        #region Physical Quantity static operator methods

        // public delegate double CombineValuesFunc(double v1, double v2);
        // public delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);
        // protected internal delegate double CombineValuesFunc(double v1, double v2);
        // protected internal delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);
        internal delegate double CombineValuesFunc(double v1, double v2);
        internal delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);

        // public 
        // protected internal 
        internal static PhysicalQuantity CombineValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf)
        {
            if (pq1.Unit != pq2.Unit)
            {
                IPhysicalQuantity temp_pq2 = pq2.ConvertTo(pq1.Unit);
                if (temp_pq2 == null)
                {
                    throw new ArgumentException("object's physical unit " + pq2.Unit.ToPrintString() + " is not convertible to a " + pq1.Unit.ToPrintString());
                }

                pq2 = temp_pq2;
            }
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pq1.Unit);
        }

        // public 
        // protected internal 
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
                IConvertibleUnit pg2_unit = pq2.Unit as IConvertibleUnit;
                pq2 = pq2.ConvertTo(pg2_unit.PrimaryUnit);
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

            if (pq2.Unit.System != pq1.Unit.System)
            {   // Must be same unit system
                pq2 = pq2.ConvertTo(pq1.Unit.System);
            }

            SByte MinNoOfBaseUnits = (SByte)Math.Min(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            Debug.Assert(MaxNoOfBaseUnits <= Physics.NoOfMeasures);

            SByte[] someexponents = new SByte[Physics.NoOfMeasures];

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

            Debug.Assert(pq1.Unit.System != null);
            PhysicalUnit pu = new DerivedUnit(pq1.Unit.System, someexponents);
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
            return new PhysicalQuantity(pq.Value * up.PrefixValue, pq.Unit);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalQuantity pq)
        {
            return new PhysicalQuantity(pq.Value * up.PrefixValue, pq.Unit);
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
            return new PhysicalQuantity( new PhysicalQuantity(d).Divide(pq));
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

        #endregion Physical Quantity static operator methods

        public PhysicalQuantity Power(SByte exponent)
        {
            IPhysicalQuantity pq = this.Unit.Pow(exponent);
            Double Value = pq.Value * System.Math.Pow(this.Value, exponent);
            return new PhysicalQuantity(Value, pq.Unit);
        }

        public PhysicalQuantity Root(SByte exponent)
        {
            IPhysicalQuantity pq = this.Unit.Rot(exponent);
            //pq.Value = pq.Value * System.Math.Root(this.Value, exponent);
            Double Value = pq.Value * System.Math.Pow(this.Value, 1.0 / exponent);
            return new PhysicalQuantity(Value, pq.Unit);
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

        public IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit)
        {
            return this.Multiply(new PrefixedUnitExponent(0, physicalUnit, 1));
        }

        public IPhysicalQuantity Divide(IPhysicalUnit physicalUnit)
        {
            return this.Divide(new PrefixedUnitExponent(0, physicalUnit, 1));
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
            IPhysicalQuantity pq = this.Unit.Multiply(prefixedUnit);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
        }

        public IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit)
        {
            IPhysicalQuantity pq = this.Unit.Divide(prefixedUnit);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
        }

        public IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(prefixedUnitExponent);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
        }

        public IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            IPhysicalQuantity pq = this.Unit.Divide(prefixedUnitExponent);
            Double Value = this.Value * pq.Value;
            return new PhysicalQuantity(Value, pq.Unit);
        }

        #endregion Physical Quantity IPhysicalUnitMath implementation
    }

    #endregion Physical Quantity Classes

    #region Physical Measure Statics Class

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

        public static readonly UnitPrefixTable UnitPrefixes = new UnitPrefixTable(new UnitPrefix[] {new UnitPrefix("yotta", 'Y', 24),
                                                                                                    new UnitPrefix("zetta", 'Z', 21),
                                                                                                    new UnitPrefix("exa",   'E', 18),
                                                                                                    new UnitPrefix("peta",  'P', 15),
                                                                                                    new UnitPrefix("tera",  'T', 12),
                                                                                                    new UnitPrefix("giga",  'G', 9),
                                                                                                    new UnitPrefix("mega",  'M', 6),
                                                                                                    new UnitPrefix("kilo",  'K', 3),   /* k */
                                                                                                    new UnitPrefix("hecto", 'H', 2),   /* h */
                                                                                                    new UnitPrefix("deca",  'D', 1),   /* da */
                                                                                                    new UnitPrefix("deci",  'd', -1), 
                                                                                                    new UnitPrefix("centi", 'c', -2), 
                                                                                                    new UnitPrefix("milli", 'm', -3), 
                                                                                                    //new UnitPrefix("micro", 'μ', -6), // '\0x03BC' (char)956  
                                                                                                    new UnitPrefix("micro", 'µ', -6),  // Ansi '\0x00B5' (char)181   
                                                                                                    new UnitPrefix("nano",  'n', -9), 
                                                                                                    new UnitPrefix("pico",  'p', -12), 
                                                                                                    new UnitPrefix("femto", 'f', -15), 
                                                                                                    new UnitPrefix("atto",  'a', -18), 
                                                                                                    new UnitPrefix("zepto", 'z', -21), 
                                                                                                    new UnitPrefix("yocto", 'y', -24) });
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

        public static readonly BaseUnit[] SI_BaseUnits = new BaseUnit[] {  new BaseUnit(null, (SByte)MeasureKind.Length, "meter", "m"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.Mass, "kilogram", "Kg"), /* kg */
                                                                    new BaseUnit(null, (SByte)MeasureKind.Time, "second", "s"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.ElectricCurrent, "ampere", "A"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.AmountOfSubstance, "mol", "mol"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.LuminousIntensity, "candela", "cd") };

        public static readonly UnitSystem SI_Units = new UnitSystem("SI", UnitPrefixes,
                                                                 SI_BaseUnits,
                                                                 new NamedDerivedUnit[] {   new NamedDerivedUnit(SI_Units, "hertz",     "Hz",   new SByte[] { -1, 0, 0, 0, 0, 0, 0 }),
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
                                                                 new ConvertibleUnit[] { new ConvertibleUnit("gram", "g", SI_BaseUnits[(int)(MeasureKind.Mass)], new ScaledValueConversion(1000)),  /* [g] = 1000 * [Kg] */
                                                                                         //
                                                                                         new ConvertibleUnit("Celsius", "°C" /* degree sign:  C2 B0  (char)176 '\0x00B0' */ , SI_BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)], new LinearValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                                                                         //new ConvertibleUnit("Celsius", "@C", SI_BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)], new LinearValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                                                                         new ConvertibleUnit("hour", "h", SI_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(1.0/3600)), /* [h] = 1/3600 * [s] */
                                                                                         new ConvertibleUnit("litre", "l", SI_BaseUnits[(int)(MeasureKind.Length)].Pow(3).Unit, new ScaledValueConversion(1000) ) }) ; /* [l] = 1000 * [m3] */   
        public static readonly PhysicalUnit dimensionless = new DerivedUnit(SI_Units, new SByte[] { 0, 0, 0, 0, 0, 0, 0 });


        public static readonly UnitSystem CGS_Units = new UnitSystem("CGS", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(CGS_Units, (SByte)MeasureKind.Length, "centimeter", "cm"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Mass, "gram", "g"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Time, "second", "s"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.ElectricCurrent, "ampere", "A"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.AmountOfSubstance, "mol", "mol"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.LuminousIntensity, "candela", "cd")});

        public static readonly BaseUnit[] MGD_BaseUnits = new BaseUnit[] {  new BaseUnit(null, (SByte)MeasureKind.Length, "meter", "m"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.Mass, "kilogram", "Kg"), /* kg */
                                                                    /* new BaseUnit(null, (SByte)MeasureKind.Time, "second", "s"), */
                                                                    new BaseUnit(null, (SByte)MeasureKind.Time, "day", "d"),
                                                                    /*  new BaseUnit(MGD_Units, "moment", "ø"), */
                                                                    new BaseUnit(null, (SByte)MeasureKind.ElectricCurrent, "ampere", "A"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.AmountOfSubstance, "mol", "mol"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.LuminousIntensity, "candela", "cd") };

        public static readonly UnitSystem MGD_Units = new UnitSystem("MGD", UnitPrefixes,
                                                                  MGD_BaseUnits, 
                                                                 null,
                                                                 new ConvertibleUnit[] { new ConvertibleUnit("second", "sec", MGD_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(24 * 60 * 60)),  /* [sec]  = 24 * 60 * 60 * [d] */
                                                                                         new ConvertibleUnit("minute", "min", MGD_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(24 * 60)),       /* [min]  = 24 * 60 * [d] */
                                                                                         new ConvertibleUnit("hour", "hour", MGD_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(24)),             /* [hour] = 24 * [d] */
                                                                                         new ConvertibleUnit("day", "day", MGD_BaseUnits[(int)(MeasureKind.Time)], new IdentityValueConversion()),               /* [day]  = 1 * [d] */
                                                                                         new ConvertibleUnit("year", "year", MGD_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(1.0/365)) });     /* [year] = 1/365 * [d] */

        public static readonly UnitSystem MGM_Units = new UnitSystem("MGM", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGM_Units, (SByte)MeasureKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "second", "s"), */
                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "day", "d"), */
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "moment", "ø"), 

                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.ElectricCurrent, "ampere", "A"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.ThermodynamicTemperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.AmountOfSubstance, "mol", "mol"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.LuminousIntensity, "candela", "cd") });


        public static UnitSystem[] UnitSystems = new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units };

        public static Stack<UnitSystem> Default_UnitSystem_Stack = new Stack<UnitSystem>();
        public static UnitSystem Default_UnitSystem  
        { 
            get  
            { 
                if (Default_UnitSystem_Stack != null && Default_UnitSystem_Stack.Count <= 0)
                {
                    return SI_Units;
                }
                else
                {
                    return Default_UnitSystem_Stack.Peek();
                }
            }
        }

        public static readonly UnitSystemConversion SItoCGSConversion = new UnitSystemConversion(SI_Units, CGS_Units, new ValueConversion[] { new ScaledValueConversion(100),       /* 1 m       <SI> = 100 cm        <CGS>  */
                                                                                                                                     new ScaledValueConversion(1000),               /* 1 Kg      <SI> = 1000 g        <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 s       <SI> = 1 s           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 A       <SI> = 1 A           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 K       <SI> = 1 K           <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 mol     <SI> = 1 mol         <CGS>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 candela <SI> = 1 cadela      <CGS>  */
                                                                                                                                    });

        public static readonly UnitSystemConversion SItoMGDConversion = new UnitSystemConversion(SI_Units, MGD_Units, new ValueConversion[] { new IdentityValueConversion(),        /* 1 m       <SI> = 1 m           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 Kg      <SI> = 1 Kg          <MGD>  */
                                                                                                                                     new ScaledValueConversion(1.0/(24*60*60)),     /* 1 s       <SI> = 1/86400 d     <MGD>  */
                                                                                                                                  /* new ScaledValueConversion(10000/(24*60*60)),   /* 1 s       <SI> = 10000/86400 ø <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 A       <SI> = 1 A           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 K       <SI> = 1 K           <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 mol     <SI> = 1 mol         <MGD>  */
                                                                                                                                     new IdentityValueConversion(),                 /* 1 candela <SI> = 1 cadela      <MGD>  */
                                                                                                                                   });

        public static readonly UnitSystemConversion MGDtoMGMConversion = new UnitSystemConversion(MGD_Units, MGM_Units, new ValueConversion[] { new IdentityValueConversion(),      /* 1 m       <MGD> = 1 m           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 Kg      <MGD> = 1 Kg          <MGM>  */
                                                                                                                                       new ScaledValueConversion(10000),            /* 1 d       <MGD> = 10000 ø       <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 A       <MGD> = 1 A           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 K       <MGD> = 1 K           <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 mol     <MGD> = 1 mol         <MGM>  */
                                                                                                                                       new IdentityValueConversion(),               /* 1 candela <MGD> = 1 cadela      <MGM>  */
                                                                                                                                     });


        public static IList<UnitSystemConversion> UnitSystemConversions = new List<UnitSystemConversion> { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion };

        public static UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {

                if (   (usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                    || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                {
                    return usc;
                }
            }

            /*  Missing direct unit system conversion from  unitsystem1 to unitsystem2. 
             *  Try to find intermediere unit system with conversion to/from unitsystem1 and unitsystem2 */

            IList<IUnitSystem> unitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> unitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            return GetUnitSystemConversion(unitsystems1, unitsystems2, UnitSystemConversions);
        }

        public static UnitSystemConversion GetUnitSystemConversion(IEnumerable<IUnitSystem> unitsystems1, IEnumerable<IUnitSystem> unitsystems2, IEnumerable<UnitSystemConversion> unitSystemConversions)
        {
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>();
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>();

            IList<UnitSystemConversion> Unitsystems1Conversions = new List<UnitSystemConversion>();
            IList<UnitSystemConversion> Unitsystems2Conversions = new List<UnitSystemConversion>();
            foreach (UnitSystemConversion usc in unitSystemConversions )
            {
                foreach (IUnitSystem unitsystem1 in unitsystems1)
                {
                    foreach (IUnitSystem unitsystem2 in unitsystems2)
                    {
                        if ((usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                            || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                        {
                            return usc;
                        }

                        // Try to clasify the 
                        if (usc.BaseUnitSystem == unitsystem1)
                        {
                            if (   !unitsystems1.Contains(usc.ConvertedUnitSystem) 
                                && !unitSystemsConvertableToUnitsystems1.Contains(usc.ConvertedUnitSystem) )
                            {
                                unitSystemsConvertableToUnitsystems1.Add(usc.ConvertedUnitSystem);
                            }
                            if (!Unitsystems1Conversions.Contains(usc))
                            {
                                Unitsystems1Conversions.Add(usc);
                            }
                        }
                        else if (usc.ConvertedUnitSystem == unitsystem1)
                        {
                            if (   !unitsystems1.Contains(usc.BaseUnitSystem) 
                                && !unitSystemsConvertableToUnitsystems1.Contains(usc.BaseUnitSystem) )
                            {
                                unitSystemsConvertableToUnitsystems1.Add(usc.BaseUnitSystem);
                            }
                            if (!Unitsystems1Conversions.Contains(usc))
                            {
                                Unitsystems1Conversions.Add(usc);
                            }
                        } 
                        else if (usc.BaseUnitSystem == unitsystem2)
                        {
                            if (   !unitsystems2.Contains(usc.BaseUnitSystem) 
                                && !unitSystemsConvertableToUnitsystems2.Contains(usc.ConvertedUnitSystem) )
                            {
                                unitSystemsConvertableToUnitsystems2.Add(usc.ConvertedUnitSystem);
                            }
                            if (!Unitsystems2Conversions.Contains(usc))
                            {
                                Unitsystems2Conversions.Add(usc);
                            }
                        }
                        else if (usc.ConvertedUnitSystem == unitsystem2)
                        {
                            if (   !unitsystems2.Contains(usc.BaseUnitSystem) 
                                && !unitSystemsConvertableToUnitsystems2.Contains(usc.BaseUnitSystem) )
                            {
                                unitSystemsConvertableToUnitsystems2.Add(usc.BaseUnitSystem);
                            }
                            if (!Unitsystems2Conversions.Contains(usc))
                            {
                                Unitsystems2Conversions.Add(usc);
                            }
                        }
                    }
                }
            }

            /*  Missing direct unit system conversion from  unitsystems1 to unitsystems2. 
             *  Try to find intermediere unit system with conversion to/from unitsystems1 and unitsystems2 */

            if (unitSystemsConvertableToUnitsystems1.Count > 0 || unitSystemsConvertableToUnitsystems2.Count > 0)
            {
                IEnumerable<IUnitSystem> tempUnitSystems1 = unitsystems1.Union(unitSystemsConvertableToUnitsystems1);
                IEnumerable<IUnitSystem> tempUnitSystems2 = unitsystems2.Union(unitSystemsConvertableToUnitsystems2);

                IEnumerable<UnitSystemConversion> notIntermediereUnitSystemConversions = unitSystemConversions.Except(Unitsystems1Conversions.Union(Unitsystems2Conversions));
                UnitSystemConversion subIntermediereUnitSystemConversion = GetUnitSystemConversion(tempUnitSystems1, tempUnitSystems2, notIntermediereUnitSystemConversions);

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

                        UnitSystemConversion FirstUnitSystemConversion = GetUnitSystemConversion(unitsystems1, new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem }, Unitsystems1Conversions);
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
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));

                            CombinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;  
                            CombinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem; 
                        }

                        UnitSystemConversion SecondUnitSystemConversion = GetUnitSystemConversion(new List<IUnitSystem>() { CombinedUnitSystemConversionIntermedierUnitSystem }, unitsystems2, Unitsystems2Conversions);
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

        public static IPhysicalUnit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalUnit unit = us.UnitFromName(namestr);
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
                IPhysicalUnit unit = us.UnitFromSymbol(symbolstr);
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

        public static IPhysicalUnit ScaledUnitFromSymbol(IPhysicalQuantity one, String scaledsymbolstr)
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
        public static readonly UnitPrefix H = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[8];
        public static readonly UnitPrefix D = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[9];
        public static readonly UnitPrefix d = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[10];
        public static readonly UnitPrefix c = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[11];
        public static readonly UnitPrefix m = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[12];
        public static readonly UnitPrefix my = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[13];
        public static readonly UnitPrefix n = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[14];
        public static readonly UnitPrefix p = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[15];
        public static readonly UnitPrefix f = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[16];
        public static readonly UnitPrefix a = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[17];
        public static readonly UnitPrefix z = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[18];
        public static readonly UnitPrefix y = (UnitPrefix)Physics.UnitPrefixes.UnitPrefixes[19];
    }

    public static partial class SI
    {
        /* SI base units */
        public static readonly PhysicalUnit m = (PhysicalUnit)Physics.SI_Units.BaseUnits[0];
        public static readonly PhysicalUnit Kg = (PhysicalUnit)Physics.SI_Units.BaseUnits[1];
        public static readonly PhysicalUnit s = (PhysicalUnit)Physics.SI_Units.BaseUnits[2];
        public static readonly PhysicalUnit A = (PhysicalUnit)Physics.SI_Units.BaseUnits[3];
        public static readonly PhysicalUnit K = (PhysicalUnit)Physics.SI_Units.BaseUnits[4];
        public static readonly PhysicalUnit mol = (PhysicalUnit)Physics.SI_Units.BaseUnits[5];
        public static readonly PhysicalUnit cd = (PhysicalUnit)Physics.SI_Units.BaseUnits[6];

        /* Named units derived from SI base units */
        public static readonly PhysicalUnit Hz = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[0];
        public static readonly PhysicalUnit rad = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[1];
        public static readonly PhysicalUnit sr = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[2];
        public static readonly PhysicalUnit N = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[3];
        public static readonly PhysicalUnit Pa = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[4];
        public static readonly PhysicalUnit J = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[5];
        public static readonly PhysicalUnit W = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[6];
        public static readonly PhysicalUnit C = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[7];
        public static readonly PhysicalUnit V = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[8];
        public static readonly PhysicalUnit F = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[9];
        public static readonly PhysicalUnit Ohm = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[10];
        public static readonly PhysicalUnit S = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[11];
        public static readonly PhysicalUnit Wb = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[12];
        public static readonly PhysicalUnit T = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[13];
        public static readonly PhysicalUnit H = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[14];
        public static readonly PhysicalUnit lm = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[15];
        public static readonly PhysicalUnit lx = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[16];
        public static readonly PhysicalUnit Bq = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[17];
        public static readonly PhysicalUnit Gy = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[18];
        public static readonly PhysicalUnit kat = (PhysicalUnit)Physics.SI_Units.NamedDerivedUnits[19];

        /* Convertible units */
        public static readonly PhysicalUnit g = (PhysicalUnit)Physics.SI_Units.ConvertibleUnits[0];
        public static readonly PhysicalUnit Ce = (PhysicalUnit)Physics.SI_Units.ConvertibleUnits[1];
        public static readonly PhysicalUnit h = (PhysicalUnit)Physics.SI_Units.ConvertibleUnits[2];
        public static readonly PhysicalUnit l = (PhysicalUnit)Physics.SI_Units.ConvertibleUnits[3];
    }

    #endregion Physical Unit System Statics
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
