/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

using Extensions;
using static PhysicalMeasure.DimensionExponentsExtension;

namespace PhysicalMeasure
{
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

    #region Dimension Exponents Delegates
    #endregion Dimension Exponents Delegates

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
        
        public delegate SByte CombineExponentsFunc(SByte e1, SByte e2);

        public static SByte SByte_Mult(SByte e1, SByte e2) => (SByte)(e1 * e2);
        public static SByte SByte_Div(SByte e1, SByte e2) => (SByte)(e1 / e2);

        public static SByte SByte_Add(SByte e1, SByte e2) => (SByte)(e1 + e2);
        public static SByte SByte_Sub(SByte e1, SByte e2) => (SByte)(e1 - e2);


        public static SByte[] CombineExponentArrays(this SByte[] exponents1, SByte[] exponents2, CombineExponentsFunc cef)
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
                NewExponents[i] = cef(exponents1[i], exponents2[i]);
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
                    NewExponents[i] = cef(0, exponents2[i]);
                }

                i++;
            }

            return NewExponents;
        }
        public static SByte[] Multiply(this SByte[] exponents1, SByte[] exponents2)
        {
            SByte[] NewExponents = CombineExponentArrays(exponents1, exponents2, SByte_Add);
            return NewExponents;
        }

        public static SByte[] Divide(this SByte[] exponents1, SByte[] exponents2)
        {
            SByte[] NewExponents = CombineExponentArrays(exponents1, exponents2, SByte_Sub);
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
            Boolean OK = true;
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
            String str = "[";

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
        private readonly String name;
        public String Name => name;

        public NamedObject(String someName)
        {
            this.name = someName;
        }

        public override String ToString() => Name;
    }

    #region Physical Unit prefix Classes

    public class UnitPrefixExponent : IUnitPrefixExponent
    {
        private SByte exponent;

        public SByte Exponent => exponent;
        public Double Value => Math.Pow(10, exponent);

        public UnitPrefixExponent(SByte somePrefixExponent)
        {
            this.exponent = somePrefixExponent;
        }

        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix) => new UnitPrefixExponent((SByte)(this.exponent + prefix.Exponent));

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix) => new UnitPrefixExponent((SByte)(this.exponent - prefix.Exponent));

        public IUnitPrefixExponent Power(SByte someExponent) => new UnitPrefixExponent((SByte)(this.exponent * someExponent));

        public IUnitPrefixExponent Root(SByte someExponent)
        {
            SByte result_exponent = (SByte)(this.exponent / someExponent);
            Debug.Assert(result_exponent * someExponent == this.exponent, " Root result exponent must be an integer");
            return new UnitPrefixExponent(result_exponent);
        }

        #endregion IUnitPrefixExponentMath implementation

        public override String ToString() => Exponent.ToString();
    }

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private IUnitPrefixTable unitPrefixTable;
        private Char prefixChar;
        IUnitPrefixExponent prefixExponent;

        #region IUnitPrefix implementation

        public Char PrefixChar => prefixChar;

        public SByte Exponent => prefixExponent.Exponent;

        public Double Value => prefixExponent.Value;

        #endregion IUnitPrefix implementation

        public UnitPrefix(IUnitPrefixTable someUnitPrefixTable, String someName, Char somePrefixChar, IUnitPrefixExponent somePrefixExponent)
            : base(someName)
        {
            this.unitPrefixTable = someUnitPrefixTable;
            this.prefixChar = somePrefixChar;
            this.prefixExponent = somePrefixExponent;
        }

        public UnitPrefix(IUnitPrefixTable someUnitPrefixTable, String someName, Char somePrefixChar, SByte somePrefixExponent)
            : this(someUnitPrefixTable, someName, somePrefixChar, new UnitPrefixExponent(somePrefixExponent))
        {
        }

        public IUnitPrefix Multiply(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Multiply(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }

        public IUnitPrefix Divide(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Divide(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }


        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Multiply(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Divide(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Power(SByte someExponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Power(someExponent);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Root(SByte someExponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Root(someExponent);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IPrefixedUnit Multiply(INamedSymbolUnit symbolUnit) => new PrefixedUnit(this, symbolUnit);


        #endregion IUnitPrefixExponentMath implementation

        public override String ToString() => PrefixChar.ToString();
    }

    public class UnitPrefixTable : IUnitPrefixTable
    {
        private readonly IUnitPrefix[] unitPrefixes;

        public IUnitPrefix[] UnitPrefixes => unitPrefixes;

        public UnitPrefixTable(IUnitPrefix[] someUnitPrefix)
        {
            this.unitPrefixes = someUnitPrefix;
        }

        public Boolean GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix)
        {
            Debug.Assert(someExponent.Exponent != 0);

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

        public Boolean GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out Char prefixChar)
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

        public Boolean GetUnitPrefixFromPrefixChar(Char somePrefixChar, out IUnitPrefix unitPrefix)
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
                    unitPrefix = up;
                    return true;
                }
            }
            unitPrefix = null;
            return false;
        }

        public Boolean GetExponentFromPrefixChar(Char somePrefixChar, out IUnitPrefixExponent exponent)
        {
            switch (somePrefixChar)
            {
                case '\x03BC':

                    // 'μ' // '\0x03BC' (Char)956  
                    // 'µ' // '\0x00B5' (Char)181
                    somePrefixChar = 'µ'; // 'µ' MICRO SIGN  '\0x00B5' (Char)181
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
        public Double Convert(Double value, Boolean backwards = false)
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

        public abstract Double ConvertFromPrimaryUnit(Double value);
        public abstract Double ConvertToPrimaryUnit(Double value);

        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public Double Convert(Boolean backwards = false)
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

        public abstract Double ConvertFromPrimaryUnit();
        public abstract Double ConvertToPrimaryUnit();

        public abstract Double LinearOffset { get; }
        public abstract Double LinearScale { get; }
    }

    public class LinearValueConversion : ValueConversion
    {
        private Double offset;
        private Double scale;

        public Double Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public Double Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public override Double LinearOffset => offset;

        public override Double LinearScale => scale;

        public LinearValueConversion(Double someOffset, Double someScale)
        {
            this.Offset = someOffset;
            this.Scale = someScale;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public override Double ConvertFromPrimaryUnit(Double value) => (value * this.Scale) + this.Offset;

        public override Double ConvertToPrimaryUnit(Double value)
        {
           Double convertedValue = (value - this.Offset) / this.Scale;
            return convertedValue;
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public override Double ConvertFromPrimaryUnit() => 1.0d * this.Scale;

        public override Double ConvertToPrimaryUnit() => 1.0d / this.Scale;
    }

    public class ScaledValueConversion : LinearValueConversion
    {
        public ScaledValueConversion(Double someScale)
            : base(0, someScale)
        {
            Debug.Assert(someScale != 0, "Parameter needed");
            Debug.Assert(!Double.IsInfinity(someScale), "Finite scale value needed");

            if (someScale == 0)
            {
                throw new ArgumentException("0 is not a valid scale", nameof(someScale));
            }
            if (Double.IsInfinity(someScale))
            {
                throw new ArgumentException("Infinity is not a valid scale", nameof(someScale));
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
        private IValueConversion firstValueConversion;
        private IValueConversion secondValueConversion;

        private Boolean firstValueConversionDirectionInverted;
        private Boolean secondValueConversionDirectionInverted;

        public override Double LinearOffset => firstValueConversion.LinearOffset + secondValueConversion.LinearOffset;

        public override Double LinearScale => firstValueConversion.LinearScale * secondValueConversion.LinearScale;


        public CombinedValueConversion(IValueConversion firstValueConversion, Boolean firstValueConversionDirectionInverted, IValueConversion secondValueConversion, Boolean secondValueConversionDirectionInverted)
        {
            this.firstValueConversion = firstValueConversion;
            this.firstValueConversionDirectionInverted = firstValueConversionDirectionInverted;
            this.secondValueConversion = secondValueConversion;
            this.secondValueConversionDirectionInverted = secondValueConversionDirectionInverted;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.

        public override Double ConvertFromPrimaryUnit(Double value) => this.secondValueConversion.Convert(this.firstValueConversion.Convert(value, this.firstValueConversionDirectionInverted), this.secondValueConversionDirectionInverted);

        public override Double ConvertToPrimaryUnit(Double value) => this.firstValueConversion.Convert(this.secondValueConversion.Convert(value, !this.secondValueConversionDirectionInverted), !this.firstValueConversionDirectionInverted);

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.

        public override Double ConvertFromPrimaryUnit() => this.secondValueConversion.Convert(this.firstValueConversion.Convert(this.firstValueConversionDirectionInverted), this.secondValueConversionDirectionInverted);

        public override Double ConvertToPrimaryUnit() => this.firstValueConversion.Convert(this.secondValueConversion.Convert(!this.secondValueConversionDirectionInverted), !this.firstValueConversionDirectionInverted);
    }

    #endregion Value Conversion Classes

    #region Physical Unit Classes

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        private String symbol;
        public String Symbol { get { return symbol; } set { symbol = value; } }

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

        public static IPhysicalUnit MakePhysicalUnit(SByte[] exponents, Double ConversionFactor = 1, Double ConversionOffset = 0)
        {
            return MakePhysicalUnit(Physics.SI_Units, exponents, ConversionFactor, ConversionOffset);
        }

        public static IPhysicalUnit MakePhysicalUnit(IUnitSystem system, SByte[] exponents, Double ConversionFactor = 1, Double ConversionOffset = 0)
        {
            IPhysicalUnit res_unit = null;
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


#if DEBUG
        // [Conditional("DEBUG")]
        public void TestPropertiesPrint()
        {
            Boolean test = true;
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
#endif // DEBUG

        public abstract IUnitSystem SimpleSystem { get; set; }
        public abstract IUnitSystem ExponentsSystem { get; }

        public abstract UnitKind Kind { get; }
        public abstract SByte[] Exponents { get; }


        public virtual IPhysicalUnit Dimensionless => Physics.dimensionless;
        public virtual Boolean IsDimensionless
        {
            get
            {
                SByte[] exponents = Exponents;
                if (Exponents == null)
                {
                    Debug.Assert(Exponents != null, "Exponents must be found");
                    return false; // Maybe combined unit with Assume 
                }
                return Exponents.IsDimensionless();
            }
        }

        public override int GetHashCode() => this.ExponentsSystem.GetHashCode() + this.Exponents.GetHashCode();

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
            pu = ParseUnit(ref unitString, ref resultLine, throwExceptionOnInvalidInput: true);
            return pu;
        }

        public static IPhysicalUnit ParseUnit(ref String unitString, ref String resultLine, Boolean throwExceptionOnInvalidInput = true)
        {
            IPhysicalUnit pu = null;

            Char timeSeparator = ':';
            Char[] separators = { timeSeparator };

            Char fractionUnitSeparator = '\0';
            String fractionUnitSeparatorStr = null;

            int unitStrCount = 0;
            int unitStrStartCharIndex = 0;
            int nextUnitStrStartCharIndex = 0;
            Boolean validFractionalUnit = true;
            int lastUnitFieldRemainingLen = 0;

            Stack<Tuple<string, IPhysicalUnit>> FractionalUnits = new Stack<Tuple<string, IPhysicalUnit>>();

            while (validFractionalUnit && (unitStrStartCharIndex >= 0) && (unitStrStartCharIndex < unitString.Length))
            {
                int unitStrLen;

                int unitStrSeparatorCharIndex = unitString.IndexOfAny(separators, unitStrStartCharIndex);
                if (unitStrSeparatorCharIndex == -1)
                {
                    unitStrLen = unitString.Length - unitStrStartCharIndex;

                    nextUnitStrStartCharIndex = unitString.Length;
                }
                else
                {
                    unitStrLen = unitStrSeparatorCharIndex - unitStrStartCharIndex;

                    nextUnitStrStartCharIndex = unitStrSeparatorCharIndex + 1;
                }

                if (unitStrLen > 0)
                {
                    unitStrCount++;
                    string unitFieldString = unitString.Substring(unitStrStartCharIndex, unitStrLen).Trim();

                    IPhysicalUnit tempPU = ParseUnit(pu, ref unitFieldString);

                    if (tempPU == null)
                    {
                        validFractionalUnit = false;
                        resultLine = "'" + unitFieldString + "' is not a valid unit.";
                        if (throwExceptionOnInvalidInput)
                        {
                            throw new PhysicalUnitFormatException("The string argument unitString is not in a valid physical unit format. " + resultLine);
                        }
                    }
                    else
                    {
                        fractionUnitSeparatorStr = fractionUnitSeparator.ToString();
                        FractionalUnits.Push(new Tuple<string, IPhysicalUnit>(fractionUnitSeparatorStr, tempPU));

                        lastUnitFieldRemainingLen = unitFieldString.Length;
                        if (lastUnitFieldRemainingLen != 0)
                        {   // Unparsed chars in (last?) field
                            unitStrLen -= lastUnitFieldRemainingLen;
                        }
                    }
                }

                // Shift to next field
                if (unitStrSeparatorCharIndex >= 0)
                {
                    fractionUnitSeparator = unitString[unitStrSeparatorCharIndex];
                }
                unitStrStartCharIndex = nextUnitStrStartCharIndex;
            }

            unitString = unitString.Substring(nextUnitStrStartCharIndex - lastUnitFieldRemainingLen);

            foreach (Tuple<string, IPhysicalUnit> tempFU in FractionalUnits)
            {
                IPhysicalUnit tempPU = tempFU.Item2;
                String tempFractionUnitSeparator = tempFU.Item1;
                if (pu == null)
                {
                    pu = tempPU;
                    fractionUnitSeparatorStr = tempFractionUnitSeparator;
                }
                else
                {
                    if (new PhysicalQuantity(tempPU).ConvertTo(pu) != null)
                    {
                        Debug.Assert(fractionUnitSeparatorStr != null, "Unit separator needed");
                        pu = new MixedUnit(tempPU, fractionUnitSeparatorStr, pu);

                        fractionUnitSeparatorStr = tempFractionUnitSeparator;
                    }
                    else
                    {
                        Debug.Assert(resultLine == null, "No resultLine expected");
                        resultLine = tempPU.ToPrintString() + " is not a valid fractional unit for " + pu.ToPrintString() + ".";

                        if (throwExceptionOnInvalidInput)
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
            None = 0,

            // Precedence == 2
            Mult = 2,
            Div = 3,

            //Precedence == 4
            Pow = 4,
            Root = 5
        }

        private static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            OperatorKind precedence = (OperatorKind)((int)operatoren & 0XE);
            return precedence;
        }

        private class Token
        {
            public readonly TokenKind TokenKind;

            public readonly IPhysicalUnit PhysicalUnit;
            public readonly SByte Exponent;
            public readonly OperatorKind Operator;

            public Token(IPhysicalUnit physicalUni)
            {
                this.TokenKind = TokenKind.Unit;
                this.PhysicalUnit = physicalUni;
            }

            public Token(SByte exponent)
            {
                this.TokenKind = TokenKind.Exponent;
                this.Exponent = exponent;
            }

            public Token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }
        }

        private class ExpressionTokenizer
        {
            private String inputString;
            private int pos = 0;
            private int afterLastOperandPos = 0;
            private int lastValidPos = 0;
            private Boolean inputRecognized = true;
            private IPhysicalUnit dimensionless = Physics.dimensionless;
            private Boolean throwExceptionOnInvalidInput = false;

            private Stack<OperatorKind> operators = new Stack<OperatorKind>();
            private List<Token> tokens = new List<Token>();

            private TokenKind lastReadToken = TokenKind.None;

            public ExpressionTokenizer(String inputStr)
            {
                this.inputString = inputStr;
            }

            public ExpressionTokenizer(IPhysicalUnit someDimensionless, String someInputStr)
            {
                this.dimensionless = someDimensionless;
                this.inputString = someInputStr;
            }

            public ExpressionTokenizer(IPhysicalUnit someDimensionless, Boolean someThrowExceptionOnInvalidInput, String someInputStr)
            {
                this.dimensionless = someDimensionless;
                this.throwExceptionOnInvalidInput = someThrowExceptionOnInvalidInput;
                this.inputString = someInputStr;
            }

            public string GetRemainingInput() => inputString.Substring(pos);

            public string GetRemainingInputForLastValidPos() => inputString.Substring(lastValidPos);

            public void SetValidPos()
            {
                if (operators.Count <= 1 && tokens.Count == 0)
                {
                    lastValidPos = afterLastOperandPos;
                }
            }

            private Boolean PushNewOperator(OperatorKind newOperator)
            {
                if (lastReadToken != TokenKind.Operator)
                {
                    if (operators.Count > 0)
                    {
                        // Pop operators with precedence higher than new operator
                        OperatorKind precedence = OperatorPrecedence(newOperator);
                        while ((operators.Count > 0) && (operators.Peek() >= precedence))
                        {
                            tokens.Add(new Token(operators.Pop()));
                        }
                    }
                    operators.Push(newOperator);
                    lastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    if (throwExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing unit at position " + pos.ToString());
                    }

                    return false;
                }
            }

            private void HandleNewOperator(OperatorKind newOperator)
            {   // Push newOperator and shift Pos or mark as failed
                if (PushNewOperator(newOperator))
                {
                    pos++;
                }
                else
                {
                    inputRecognized = false;
                }
            }

            private Token RemoveFirstToken()
            {   // return first operator from post fix operators
                Token token = tokens[0];
                tokens.RemoveAt(0);

                return token;
            }

            public Token GetToken()
            {
                Debug.Assert(inputString != null, "Source needed");

                if (tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }
                int OperatorsCountForRecognizedTokens = operators.Count;
                while ((inputString.Length > pos) && inputRecognized)
                {
                    Char c = inputString[pos];
                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        pos++;
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
                        if ((lastReadToken != TokenKind.Unit)                // Exponent can follow unit directly 
                            && ((lastReadToken != TokenKind.Operator)          // or follow Pow operator 
                                 || (operators.Peek() != OperatorKind.Pow)))
                        {
                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An exponent must follow a unit or Pow operator. Invalid exponent at '" + c + "' at position " + pos.ToString());
                            }
                            else
                            {
                                // return null;
                                inputRecognized = false;
                            }
                        }
                        else
                        {
                            //// Try to read an exponent from input

                            Int16 numLen = 1;

                            int maxLen = Math.Min(inputString.Length - pos, 1 + 3); // Max length of sign and digits to look for
                            while (numLen < maxLen && Char.IsDigit(inputString[pos + numLen]))
                            {
                                numLen++;
                            }

                            SByte exponent;
                            if (numLen > 0 && SByte.TryParse(inputString.Substring(pos, numLen), out exponent))
                            {
                                if ((lastReadToken == TokenKind.Operator)
                                    && (operators.Peek() == OperatorKind.Pow))
                                {
                                    // Exponent follow Pow operator; 
                                    // Remove Pow operator from operator stack since it is handled as implicit in parser.
                                    operators.Pop();
                                }

                                pos += numLen;
                                afterLastOperandPos = pos;

                                lastReadToken = TokenKind.Exponent;

                                return new Token(exponent);
                            }
                            else
                            {
                                if (throwExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing exponent after '" + c + "' at position " + pos.ToString());
                                }
                                else
                                {
                                    // return null;
                                    inputRecognized = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((lastReadToken == TokenKind.Operator)    // Unit follow Pow operator; 
                            && (operators.Peek() == OperatorKind.Pow))
                        {
                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An unit must not follow an pow operator. Missing exponent at '" + c + "' at position " + pos.ToString());
                            }
                            else
                            {
                                inputRecognized = false;
                            }
                        }
                        else
                        {
                            // Try to read a unit from input
                            int maxLen = Math.Min(1 + 3, inputString.Length - pos); // Max length of scale and symbols to look for

                            String tempStr = inputString.Substring(pos, maxLen);
                            maxLen = tempStr.IndexOfAny(new Char[] { ' ', '*', '·', '/', '^', '+', '-', '(', ')' });  // '·'  center dot '\0x0B7' (Char)183 U+00B7
                            if (maxLen < 0)
                            {
                                maxLen = tempStr.Length;
                            }

                            for (int unitLen = maxLen; unitLen > 0; unitLen--)
                            {
                                String unitStr = tempStr.Substring(0, unitLen);
                                IPhysicalUnit su = Physics.UnitSystems.ScaledUnitFromSymbol(unitStr);
                                if (su != null)
                                {
                                    if (lastReadToken == TokenKind.Unit)
                                    {   // Assume implicit Mult operator
                                        PushNewOperator(OperatorKind.Mult);
                                    }

                                    pos += unitLen;
                                    afterLastOperandPos = pos;

                                    lastReadToken = TokenKind.Unit;
                                    return new Token(su);
                                }
                            }

                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid unit '" + inputString.Substring(pos, maxLen) + "' at position " + pos.ToString());
                            }
                            else
                            {
                                inputRecognized = false;
                            }
                        }
                    }

                    if (tokens.Count > 0)
                    {   // return first operator from post fix operators
                        return RemoveFirstToken();
                    }
                };

                if (!inputRecognized)
                {
                    // Remove operators from stack which was pushed for not recognized input
                    while (operators.Count > OperatorsCountForRecognizedTokens)
                    {
                        operators.Pop();
                    }
                }
                //// Retrieve remaining operators from stack
                while (operators.Count > 0)
                {
                    tokens.Add(new Token(operators.Pop()));
                }

                if (tokens.Count > 0)
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

            ExpressionTokenizer tokenizer = new ExpressionTokenizer(dimensionless, /* throwExceptionOnInvalidInput = */ false, s);

            Stack<IPhysicalUnit> operands = new Stack<IPhysicalUnit>();

            Boolean inputTokenInvalid = false;
            tokenizer.SetValidPos();
            Token token = tokenizer.GetToken();

            while (token != null && !inputTokenInvalid)
            {
                if (token.TokenKind == TokenKind.Unit)
                {
                    // Stack unit operand
                    operands.Push(token.PhysicalUnit);
                }
                else if (token.TokenKind == TokenKind.Exponent)
                {
                    IPhysicalUnit pu = operands.Pop();

                    // Combine pu and exponent to the new unit pu^exponent   
                    operands.Push(pu.CombinePow(token.Exponent));
                }
                else if (token.TokenKind == TokenKind.Operator)
                {
                    /****
                     * Pow operator is handled implicit
                     * 
                    if (token.Operator == OperatorKind.Pow)
                    {
                        Debug.Assert(operands.Count >= 1, "The operands.Count must be 1 or more");
                        SByte exponentSecond = operands.Pop();
                        IPhysicalUnit puFirst = operands.Pop();
                        // Combine pu and exponent to the new unit pu^exponent   
                        operands.Push(puFirst.CombinePow(exponentSecond));
                    }
                    else
                    ****/
                    if (operands.Count >= 2)
                    {
                        Debug.Assert(operands.Count >= 2, "Two operands needed");

                        IPhysicalUnit puSecond = operands.Pop();
                        IPhysicalUnit puFirst = operands.Pop();

                        if (token.Operator == OperatorKind.Mult)
                        {
                            // Combine pu1 and pu2 to the new unit pu1*pu2   
                            operands.Push(puFirst.CombineMultiply(puSecond));
                        }
                        else if (token.Operator == OperatorKind.Div)
                        {
                            // Combine pu1 and pu2 to the new unit pu1/pu2
                            operands.Push(puFirst.CombineDivide(puSecond));
                        }
                    }
                    else
                    {   // Missing operand(s). Operator not valid part of (this) unit
                        inputTokenInvalid = true;
                    }
                }
                if (!inputTokenInvalid)
                {
                    if (operands.Count == 1)
                    {
                        tokenizer.SetValidPos();
                    }
                    token = tokenizer.GetToken();
                }
            }

            s = tokenizer.GetRemainingInputForLastValidPos(); // Remaining of input string

            Debug.Assert(operands.Count <= 1, "Only one operand is allowed");  // 0 or 1

            return (operands.Count > 0) ? operands.Last() : null;
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
        public virtual String UnitPrintString() => this.UnitString();

        public virtual String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            Debug.Assert(invertExponents == false, "The invertExponents must be false");
            return this.UnitString();
        }

        /// <summary>
        /// String formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.2
        /// </summary>
        public virtual String ReducedUnitString() => this.UnitString();

        /// <summary>
        /// IFormattable.ToString implementation.
        /// Eventually with system name prefixed.
        /// </summary>
        public override String ToString()
        {
            String unitName = this.UnitString();
            IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
            if ((!String.IsNullOrEmpty(unitName))
                && (system != null)
                && (system != Physics.CurrentUnitSystems.Default)
                 /*
                 && (!(    Physics.SI_Units == Physics.Default_UnitSystem 
                        && system.IsCombinedUnitSystem 
                        && ((ICombinedUnitSystem)system).ContainsSubUnitSystem(Physics.Default_UnitSystem) ))
                  */
                 )
            {
                unitName = system.Name + "." + unitName;
            }

            return unitName;
        }

        /// <summary>
        /// IPhysicalUnit.ToPrintString implementation.
        /// With system name prefixed if system specified.
        /// </summary>
        public virtual String ToPrintString()
        {
            String unitName = this.UnitPrintString();
            if (String.IsNullOrEmpty(unitName))
            {
                unitName = "dimensionless";
            }
            else
            {
                IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
                if ((system != null)
                    && (system != Physics.CurrentUnitSystems.Default))
                {
                    unitName = system.Name + "." + unitName;
                }
            }
            return unitName;
        }

        public virtual string ValueString(double quantity) => quantity.ToString();

        public virtual string ValueString(double quantity, String format, IFormatProvider formatProvider)
        {
            String valStr = null;
            try
            {
                valStr = quantity.ToString(format, formatProvider);
            }
            catch
            {
                valStr = quantity.ToString() + " ?" + format + "?";
            }
            return valStr;
        }

        public virtual Double FactorValue => 1;

        public virtual IPhysicalUnit PureUnit => this;

        #endregion Unit print string methods

        #region Unit conversion methods

        public abstract Boolean IsLinearConvertible();


        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)

        public IPhysicalQuantity this[IPhysicalUnit convertToUnit] => this.ConvertTo(convertToUnit);

        public IPhysicalQuantity this[IPhysicalQuantity convertToUnit] => this.ConvertTo(convertToUnit.Unit).Multiply(convertToUnit.Value);

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
        public virtual IPhysicalQuantity ConvertTo(ref Double quantity, IPhysicalUnit convertToUnit)
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

        public virtual IPhysicalQuantity ConvertTo(ref Double quantity, IUnitSystem convertToUnitSystem)
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

        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public abstract IPhysicalQuantity ConvertToSystemUnit(ref Double quantity);

        public abstract IPhysicalQuantity ConvertToBaseUnit();

        public abstract IPhysicalQuantity ConvertToBaseUnit(Double quantity);

        public virtual IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }



        /// <summary>
        /// 
        /// </summary>
        public /* virtual */ IPhysicalQuantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem)
        {
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

        public Boolean Equivalent(IPhysicalUnit other, out Double quotient)
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                quotient = 0;
                return false;
            }

            IPhysicalQuantity pq1;
            IPhysicalQuantity pq2;

            if (this.ExponentsSystem != other.ExponentsSystem)
            {   // Must be same unit system
                if (this.ExponentsSystem == null || other.ExponentsSystem == null)
                {
                    if (this.IsDimensionless && other.IsDimensionless)
                    {
                        // Any dimensionless can be converted to any systems dimensionless
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

                pq1 = new PhysicalQuantity(1, this);
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


        public virtual Boolean Equals(IPhysicalUnit other)
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                return false;
            }

           Double quotient;
            Boolean equals = this.Equivalent(other, out quotient);
            return equals && quotient == 1;
        }

        public override Boolean Equals(Object obj)
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

        public static Boolean operator ==(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            Debug.Assert(null != unit1, "The 'unit1' parameter must be specified");

            return unit1.Equals(unit2);
        }

        public static Boolean operator !=(PhysicalUnit unit1, IPhysicalUnit unit2) => !unit1.Equals(unit2);

        #endregion Unit conversion methods

        #region Unit static operator methods

        protected delegate Double CombineQuantitiesFunc(Double q1, Double q2);


        protected static PhysicalQuantity CombineUnits(IPhysicalUnit u1, IPhysicalUnit u2, CombineExponentsFunc cef, CombineQuantitiesFunc cqf)
        {
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

        protected static PhysicalUnit CombineUnitExponents(IPhysicalUnit u, SByte exponent, CombineExponentsFunc cef)
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
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PhysicalQuantity(up.Value, u);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalUnit u)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PhysicalQuantity(up.Value, u);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, Double d) => new PhysicalQuantity(d, u);

        public static PhysicalQuantity operator /(PhysicalUnit u, Double d) => new PhysicalQuantity(1 / d, u);

        public static PhysicalQuantity operator *(Double d, PhysicalUnit u) => new PhysicalQuantity(d, u);

        public static PhysicalQuantity operator /(Double d, PhysicalUnit u) => new PhysicalQuantity(d, 1 / u);

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPhysicalUnit u2) 
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return new PhysicalQuantity(u1.Multiply(u2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPhysicalUnit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return new PhysicalQuantity(u1.Divide(u2));
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return new PhysicalQuantity(u1.Multiply(pue2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return new PhysicalQuantity(u1.Divide(pue2));
        }

        public static PhysicalQuantity operator ^(PhysicalUnit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The " + nameof(u) + " parameter must be specified");

            return new PhysicalQuantity(u.Pow(exponent));
        }

        public static PhysicalQuantity operator %(PhysicalUnit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The "+ nameof(u) +" parameter must be specified");
            return new PhysicalQuantity(u.Rot(exponent));
        }

        #endregion Unit static operator methods

        public virtual IPhysicalQuantity AsPhysicalQuantity() => AsPhysicalQuantity(1);

        public virtual IPhysicalQuantity AsPhysicalQuantity(double quantity) => new PhysicalQuantity(quantity, this);

        public virtual PhysicalUnit Power(SByte exponent) => CombineUnitExponents(this, exponent, SByte_Mult);

        public virtual PhysicalUnit Root(SByte exponent) => CombineUnitExponents(this, exponent, SByte_Div);

        #region Unit math methods

        public virtual IPhysicalUnit Multiply(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The " + nameof(prefix) + " parameter must be specified");
            return this.CombineMultiply(prefix);
        }

        public virtual IPhysicalUnit Divide(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The " + nameof(prefix) + " parameter must be specified");
            return this.CombineDivide(prefix);
        }

        public virtual IPhysicalUnit Multiply(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The " + nameof(namedSymbolUnit) + " parameter must be specified");
            return this.CombineMultiply(namedSymbolUnit);
        }

        public virtual IPhysicalUnit Divide(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The " + nameof(namedSymbolUnit) + " parameter must be specified");
            return this.CombineDivide(namedSymbolUnit);
        }

        public virtual IPhysicalUnit Multiply(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The " + nameof(prefixedUnit) + " parameter must be specified");
            return this.CombineMultiply(prefixedUnit);
        }

        public virtual IPhysicalUnit Divide(IPrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The " + nameof(prefixedUnit) + " parameter must be specified");
            return this.CombineDivide(prefixedUnit);
        }


        public virtual IPhysicalUnit Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The " + nameof(prefixedUnitExponent) + " parameter must be specified");
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

        public virtual IPhysicalQuantity Multiply(Double quantity) => new PhysicalQuantity(quantity, this);

        public virtual IPhysicalQuantity Divide(Double quantity) => new PhysicalQuantity(1 / quantity, this);


        public virtual IPhysicalQuantity Multiply(Double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            IPhysicalUnit pu = this.Multiply(physicalQuantity.Unit);
            return pu.Multiply(quantity * physicalQuantity.Value);
        }

        public virtual IPhysicalQuantity Divide(Double quantity, IPhysicalQuantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            IPhysicalUnit pu = this.Divide(physicalQuantity.Unit);
            return pu.Multiply(quantity / physicalQuantity.Value);
        }

        public IPhysicalUnit Pow(SByte exponent) => this.Power(exponent);

        public IPhysicalUnit Rot(SByte exponent) => this.Root(exponent);

        #endregion Unit math methods

        #region Unit Combine math methods

        public virtual ICombinedUnit CombineMultiply(Double quantity)
        {
            ICombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(quantity);
            return uRes;
        }

        public virtual ICombinedUnit CombineDivide(Double quantity)
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
            ICombinedUnit asCombinedUnit = new CombinedUnit(this);
            ICombinedUnit uRes = asCombinedUnit.CombinePow(exponent);
            return uRes;
        }

        public virtual ICombinedUnit CombineRot(SByte exponent)
        {
            ICombinedUnit asCombinedUnit = new CombinedUnit(this);
            ICombinedUnit uRes = asCombinedUnit.CombineRot(exponent);
            return uRes;
        }

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

        public static implicit operator PhysicalQuantity(PhysicalUnit physicalUnit) => new PhysicalQuantity(physicalUnit);
    }

    public abstract class SystemUnit : PhysicalUnit, ISystemUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        private IUnitSystem system;

        public override IUnitSystem SimpleSystem { get { return system; } set { system = value; } }
        public override IUnitSystem ExponentsSystem => system;

        protected SystemUnit(IUnitSystem someSystem = null)
        {
            this.system = someSystem;
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity) => new PhysicalQuantity(quantity, this);

        public override IPhysicalQuantity ConvertToSystemUnit() => new PhysicalQuantity(1, this);
    }

    public class BaseUnit : SystemUnit, INamedSymbol, IBaseUnit //, IPrefixedUnit
    {
        private NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;


        private SByte baseunitnumber;
        public SByte BaseUnitNumber => baseunitnumber;

        public override UnitKind Kind => UnitKind.BaseUnit;

        public override SByte[] Exponents
        {
            get
            {
                if (baseunitnumber < 0)
                {
                    Debug.Assert(baseunitnumber >= 0);
                }

                int NoOfBaseUnits = baseunitnumber + 1;
                if (SimpleSystem != null && SimpleSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = SimpleSystem.BaseUnits.Length;
                }

                SByte[] tempExponents = new SByte[NoOfBaseUnits];
                tempExponents[baseunitnumber] = 1;
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
            this.baseunitnumber = someBaseUnitNumber;
            this.namedSymbol = someNamedSymbol;
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
        public override String PureUnitString() => this.Symbol;

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsLinearConvertible() => true;

        public override IPhysicalQuantity ConvertToBaseUnit() => new PhysicalQuantity(1, this);

        public override IPhysicalQuantity ConvertToDerivedUnit() => new PhysicalQuantity(1, this);

        public override IPhysicalQuantity ConvertToBaseUnit(Double quantity) => new PhysicalQuantity(quantity, this);

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity) => physicalQuantity.ConvertTo(this);
    }

    public class DerivedUnit : SystemUnit, IDerivedUnit
    {
        private readonly SByte[] exponents;

        public override UnitKind Kind => UnitKind.DerivedUnit;

        public override SByte[] Exponents => exponents;

        public DerivedUnit(IUnitSystem someSystem, SByte[] someExponents = null)
            : base(someSystem)
        {
            this.exponents = someExponents;
        }


        public DerivedUnit(SByte[] someExponents)
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
            Boolean UnitIsMissingSystem = false;
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
                        UnitIsMissingSystem = true;
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
            if (UnitIsMissingSystem)
            {
                // Do some trace of error    
                Debug.WriteLine(global::System.Reflection.Assembly.GetExecutingAssembly().ToString() + " Unit " + this.Kind.ToString() + " { " + ExponentsStr + "} missing unit system.");
            }
#endif

            return ExponentsStr;
        }

        public override Boolean IsLinearConvertible() => true;

        public override IPhysicalQuantity ConvertToBaseUnit() => new PhysicalQuantity(1, this);

        public override IPhysicalQuantity ConvertToDerivedUnit() => new PhysicalQuantity(1, this);

        public override IPhysicalQuantity ConvertToBaseUnit(Double quantity) => new PhysicalQuantity(quantity, this);

        public override IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity physicalQuantity) => physicalQuantity.ConvertTo(this);

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
                }
            }

            return this.CombineDivide(physicalUnit);
        }

    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, INamedDerivedUnit // , IPrefixedUnit
    {
        private readonly NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;

        public NamedDerivedUnit(UnitSystem someSystem, NamedSymbol someNamedSymbol, SByte[] someExponents = null)
            : base(someSystem, someExponents)
        {
            this.namedSymbol = someNamedSymbol;
        }

        public NamedDerivedUnit(UnitSystem someSystem, String someName, String someSymbol, SByte[] someExponents = null)
            : this(someSystem, new NamedSymbol(someName, someSymbol), someExponents)
        {
        }

        public static PhysicalUnit operator *(NamedDerivedUnit u, IUnitPrefix up)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PrefixedUnit(up, u);
        }

        public static PhysicalUnit operator *(IUnitPrefix up, NamedDerivedUnit u)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PrefixedUnit(up, u);
        }

        /// <summary>
        /// String PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => ReducedUnitString();

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
        public override IPhysicalQuantity ConvertToBaseUnit() => this.ConvertToBaseUnit(1);

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
        private readonly NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;

        private readonly IPhysicalUnit primaryunit;
        private readonly IValueConversion conversion;

        public IPhysicalUnit PrimaryUnit => primaryunit;
        public IValueConversion Conversion => conversion;

        public ConvertibleUnit(NamedSymbol someNamedSymbol, IPhysicalUnit somePrimaryUnit = null, ValueConversion someConversion = null)
            : base(somePrimaryUnit != null ? somePrimaryUnit.SimpleSystem : null)
        {
            this.namedSymbol = someNamedSymbol;
            primaryunit = somePrimaryUnit;
            conversion = someConversion;

            if (this.namedSymbol == null)
            {
                String name;
                if (someConversion == null || someConversion.LinearOffset == 0)
                {
                    name = this.ConvertToPrimaryUnit().ToPrintString();
                }
                else
                {
                    name = this.primaryunit.ToPrintString();
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
                this.namedSymbol = new NamedSymbol(name, name);
            }

            Debug.Assert(this.namedSymbol != null, "The 'someNamedSymbol' must be valid and not null");
        }

        public ConvertibleUnit(String someName, String someSymbol, IPhysicalUnit somePrimaryUnit = null, ValueConversion someConversion = null)
            : this(new NamedSymbol(someName, someSymbol), somePrimaryUnit, someConversion)
        {
        }

        public override UnitKind Kind => UnitKind.ConvertibleUnit;

        public override SByte[] Exponents => PrimaryUnit.Exponents;

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => ReducedUnitString();

        /// <summary>
        /// String with formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.ConvertibleUnit, "The 'this.Kind' must be valid and an UnitKind.ConvertibleUnit");

            return this.namedSymbol.Symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsLinearConvertible()
        {
            Debug.Assert(conversion != null, "The '_conversion' must be valid and not null");
            return conversion.LinearOffset == 0;
        }

        public IPhysicalQuantity ConvertFromPrimaryUnit() => new PhysicalQuantity(Conversion.ConvertFromPrimaryUnit(), this);

        public IPhysicalQuantity ConvertToPrimaryUnit() => new PhysicalQuantity(Conversion.ConvertToPrimaryUnit(), PrimaryUnit);

        public IPhysicalQuantity ConvertFromPrimaryUnit(Double quantity) => new PhysicalQuantity(Conversion.ConvertFromPrimaryUnit(quantity), this);

        public IPhysicalQuantity ConvertToPrimaryUnit(Double quantity)
        {
            IValueConversion temp_conversion = Conversion;
            IPhysicalUnit temp_primaryunit = PrimaryUnit;
            Double convertedValue = temp_conversion.ConvertToPrimaryUnit(quantity);
            return new PhysicalQuantity(convertedValue, temp_primaryunit);
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity)
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

        public override IPhysicalQuantity ConvertToDerivedUnit() => this.ConvertToBaseUnit();


        public override IPhysicalQuantity ConvertToBaseUnit(double quantity) => PrimaryUnit.ConvertToBaseUnit(new PhysicalQuantity(quantity, this));

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

        public override PhysicalUnit Power(SByte exponent)
        {
            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }

        public override PhysicalUnit Root(SByte exponent)
        {
            IPhysicalQuantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Rot(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }
    }

    #region Combined Unit Classes

    public class PrefixedUnit : PhysicalUnit, IPrefixedUnit
    {
        private readonly IUnitPrefix prefix;
        private readonly INamedSymbolUnit unit;

        public IUnitPrefix Prefix => prefix;
        public INamedSymbolUnit Unit => unit;


        public override IUnitSystem SimpleSystem { get { return unit.SimpleSystem; } set { /* unit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem => unit.ExponentsSystem;

        public override UnitKind Kind => UnitKind.PrefixedUnit;

        public override SByte[] Exponents => unit.Exponents;

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => Prefix.PrefixChar + Unit.Symbol;

        public PrefixedUnit(IUnitPrefix somePrefix, INamedSymbolUnit someUnit)
        {
            this.prefix = somePrefix;
            this.unit = someUnit;
        }

        public override IPhysicalQuantity AsPhysicalQuantity(Double quantity) => new PhysicalQuantity(quantity, this);

        public override Boolean IsLinearConvertible() => unit.IsLinearConvertible();

        public static implicit operator PhysicalQuantity(PrefixedUnit prefixedUnit) => prefixedUnit.AsPhysicalQuantity() as PhysicalQuantity;

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IPhysicalQuantity pq = unit.ConvertToSystemUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity)
        {
            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            IPhysicalQuantity pq = unit.ConvertToSystemUnit(ref quantity);
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = unit.ConvertToBaseUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IPhysicalQuantity pq = unit.ConvertToDerivedUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit(Double quantity)
        {
            IPhysicalQuantity pq = this.ConvertToBaseUnit();
            pq = pq.Multiply(quantity);
            return pq;
        }

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String combinedUnitString = Prefix.PrefixChar + Unit.Symbol;

            return combinedUnitString;
        }

    }

    public class PrefixedUnitExponent : PhysicalUnit, IPrefixedUnitExponent
    {
        private readonly IPrefixedUnit prefixedUnit;
        private readonly SByte exponent;

        public SByte Exponent => exponent;


        public PrefixedUnitExponent(INamedSymbolUnit someUnit)
            : this(null, someUnit, 1)
        {
        }

        public PrefixedUnitExponent(INamedSymbolUnit someUnit, SByte someExponent)
            : this(null, someUnit, someExponent)
        {
        }


        public PrefixedUnitExponent(IPrefixedUnitExponent prefixedUnitExponent)
            : this(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, prefixedUnitExponent.Exponent)
        {
        }

        public PrefixedUnitExponent(IUnitPrefix somePrefix, INamedSymbolUnit someUnit, SByte someExponent)
            : this(new PrefixedUnit(somePrefix, someUnit), someExponent)
        {
        }

        public PrefixedUnitExponent(IPrefixedUnit somePrefixedUnit, SByte someExponent)
        {
            this.prefixedUnit = somePrefixedUnit;
            this.exponent = someExponent;

            Debug.Assert(someExponent != 0, "The 'exponent' must be valid and not zero");
        }

        public IUnitPrefix Prefix => prefixedUnit.Prefix;
        public INamedSymbolUnit Unit => prefixedUnit.Unit;


        public override UnitKind Kind => UnitKind.PrefixedUnitExponent;


        public override IUnitSystem SimpleSystem { get { return prefixedUnit.SimpleSystem; } set { /* prefixedUnit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem => prefixedUnit.ExponentsSystem;


        public override SByte[] Exponents
        {
            get
            {
                SByte[] exponents = prefixedUnit.Exponents;
                if (exponent != 1)
                {
                    exponents = exponents.Power(exponent);
                }
                return exponents;
            }
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            IPhysicalUnit pu = prefixedUnit;
            if (exponent != 1)
            {
                pu = pu.Pow(exponent);
            }
            String unitString = pu.PureUnitString();
            return unitString;
        }

        public override Boolean IsLinearConvertible() => prefixedUnit.IsLinearConvertible();

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            IPhysicalQuantity pq = prefixedUnit.ConvertToSystemUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity)
        {
            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            IPhysicalQuantity pq = prefixedUnit.ConvertToSystemUnit(ref quantity);
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit()
        {
            IPhysicalQuantity pq = prefixedUnit.ConvertToBaseUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IPhysicalQuantity pq = prefixedUnit.ConvertToDerivedUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override IPhysicalQuantity ConvertToBaseUnit(double quantity)
        {
            IPhysicalQuantity pq = prefixedUnit.ConvertToBaseUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            pq = pq.Multiply(quantity);
            return pq;
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString() => this.CombinedUnitString();

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";
            Debug.Assert(exponent != 0, "The '_Exponent' must be valid and not zero");
            if (prefixedUnit.Unit != null)
            {
                string UnitStr = prefixedUnit.Unit.UnitString();

                if (prefixedUnit.Prefix != null && prefixedUnit.Prefix.Exponent != 0)
                {
                    Char PrefixChar = prefixedUnit.Prefix.PrefixChar;
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
                if (prefixedUnit.Prefix != null && prefixedUnit.Prefix.Exponent != 0)
                {
                    SByte expo = Exponent;
                    if (invertExponents)
                    {
                        expo = (SByte)(-expo);
                    }

                    expo = (SByte)(prefixedUnit.Prefix.Exponent * expo);

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
            IPhysicalQuantity pue_pq = prefixedUnit.AsPhysicalQuantity();
            if (exponent != 1)
            {
                pue_pq = pue_pq.Pow(exponent);
            }
            return pue_pq;
        }


        public IPrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent, out Double scaleFactor)
        {
            SByte combinedPrefixExponent = 0;
            if (this.Exponent == 1 || outerPUE_PrefixExponent == 0)
            {
                //
                scaleFactor = 1;
                scaleExponent = 0;
                combinedPrefixExponent = (SByte)(outerPUE_PrefixExponent + this.prefixedUnit.Prefix.Exponent);
            }
            else
            {
                int reminder;
                combinedPrefixExponent = (SByte)Math.DivRem(outerPUE_PrefixExponent, this.Exponent, out reminder);
                if (reminder != 0)
                {
                    scaleFactor = Math.Pow(10, 1.0 * reminder);
                    scaleExponent = (SByte)reminder;
                }
                else
                {
                    scaleFactor = 1;
                    scaleExponent = 0;
                }
                combinedPrefixExponent += this.prefixedUnit.Prefix.Exponent;
            }

            IUnitPrefix combinedUnitPrefix;
            SByte combinedScaleFactorExponent;

            Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(combinedPrefixExponent, out combinedUnitPrefix, out combinedScaleFactorExponent);


            IUnitPrefixExponent pe = new UnitPrefixExponent((SByte)(combinedPrefixExponent + this.prefixedUnit.Prefix.Exponent));

            IUnitPrefix up = null;
            if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(pe, out up))
            {
                PrefixedUnitExponent CombinedPUE = new PrefixedUnitExponent(up, this.prefixedUnit.Unit, (SByte)(this.Exponent * outerPUE_Exponent));
                return CombinedPUE;
            }
            else
            {
                // TO DO: Handle to make result as IPrefixedUnitExponent
                Debug.Assert(false);
                return null;
            }
        }

        public static implicit operator PhysicalQuantity(PrefixedUnitExponent prefixedUnitExponent) => prefixedUnitExponent.AsPhysicalQuantity() as PhysicalQuantity;
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
            String str = "";

            foreach (IPrefixedUnitExponent ue in this)
            {
                Debug.Assert(ue.Exponent != 0, "ue.Exponent must be <> 0");
                if (!String.IsNullOrEmpty(str))
                {
                    str += '·';  // center dot '\0x0B7' (Char)183 U+00B7
                }

                str += ue.CombinedUnitString(mayUseSlash, invertExponents);
            }
            return str;
        }

        public IPrefixedUnitExponentList Power(SByte exponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
           Double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                SByte newExponent = (SByte)(pue.Exponent * exponent);

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (exponent == 1) || (exponent == -1), "Power: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^" + exponent);
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
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(newUnitPrefix, pue.Unit, newExponent);

                result.Add(result_pue);
            }
            Debug.Assert(factorValue == 1.0);
            return result;
        }

        public IPrefixedUnitExponentList Root(SByte someExponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
           Double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                int remainder;
                int newExponent = Math.DivRem(pue.Exponent, someExponent, out remainder);
                if (remainder != 0)
                {
                    Debug.Assert(remainder == 0);
                    return null;
                }

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (someExponent == 1) || (someExponent == -1), "Root: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^(1/" + someExponent + ")");
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
        private Double scaleFactor = 1;

        private IPrefixedUnitExponentList numerators;
        private IPrefixedUnitExponentList denominators;

        public IPrefixedUnitExponentList Numerators => numerators as IPrefixedUnitExponentList;
        public IPrefixedUnitExponentList Denominators => denominators as IPrefixedUnitExponentList;

        public CombinedUnit()
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
        }

        public CombinedUnit(IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
        {
            if ((someNumerators == null || someNumerators.Count == 0) && (someDenominators != null && someDenominators.Count > 0))
            {
                someNumerators = new PrefixedUnitExponentList(someDenominators.Select(pue => new PrefixedUnitExponent(pue.Prefix, pue.Unit, (SByte)(-pue.Exponent))));
                someDenominators = null;
            }

            this.numerators = someNumerators != null ? someNumerators : new PrefixedUnitExponentList();
            this.denominators = someDenominators != null ? someDenominators : new PrefixedUnitExponentList();
        }

        public CombinedUnit(Double someScaleFactor, IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
            : this(someNumerators, someDenominators)
        {
            this.scaleFactor = someScaleFactor;
        }

        public CombinedUnit(ICombinedUnit combinedUnit)
            : this(combinedUnit.FactorValue, combinedUnit.Numerators, combinedUnit.Denominators)
        {

        }
        
        public CombinedUnit(IUnitPrefix prefix, ICombinedUnit combinedUnit)
        {
            IUnitPrefix somePrefix = prefix;
            Double someFactorValue = combinedUnit.FactorValue;

            Boolean found = false;
            PrefixedUnitExponentList someNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList someDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent pue in combinedUnit.Numerators)
            {
                Boolean added = false;

                IUnitPrefix tempPrefix = null;
                SByte ScaleFactorExponent = 0;
                Double somePrefixExponentDiff = 0;

                if (!found )
                {
                    if (someFactorValue != 1)
                    {
                        // Try to combine specified prefix with combinedUnit valueFactor to a prefix
                        Double tempSomeFactorValue = Math.Pow(10, somePrefix.Exponent) * someFactorValue;
                        Double somePrefixExponentD = Math.Log10(tempSomeFactorValue);
                        SByte somePrefixExponent = (SByte)Math.Ceiling(somePrefixExponentD);
                        somePrefixExponentDiff = somePrefixExponentD - somePrefixExponent;

                        Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(somePrefixExponent, out tempPrefix, out ScaleFactorExponent);
                    }
                    else
                    {   
                        // Just use specified prefix
                        tempPrefix = somePrefix;
                    }
                }

                if (!found && tempPrefix != null)
                {
                    IPrefixedUnitExponent tempPue = new PrefixedUnitExponent(tempPrefix, pue.Unit, pue.Exponent);
                    someNumerators.Add(tempPue);
                    somePrefix = null;
                    if (ScaleFactorExponent != 0)
                    {
                        someFactorValue = Math.Pow(10, ScaleFactorExponent + somePrefixExponentDiff);
                    }

                    found = true;
                    added = true; 
                }
                
                if (!added)
                {
                    someNumerators.Add(pue);
                }
            }

            foreach (IPrefixedUnitExponent pue in combinedUnit.Denominators)
            {
                someDenominators.Add(pue);
            }

            this.scaleFactor = someFactorValue;
            this.numerators = someNumerators;
            this.denominators = someDenominators;
        }

        public CombinedUnit(IPrefixedUnitExponent prefixedUnitExponent)
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
            this.numerators.Add(prefixedUnitExponent);
        }

        public CombinedUnit(IPhysicalUnit physicalUnit)
        {
            ICombinedUnit cu = null;

            INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
            if (nsu != null)
            {
                cu = new CombinedUnit(nsu);
            }
            else
            {
                switch (physicalUnit.Kind)
                {
                    case UnitKind.PrefixedUnit:
                        IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                        Debug.Assert(pu != null);
                        cu = new CombinedUnit(pu);
                        break;
                    case UnitKind.PrefixedUnitExponent:
                        IPrefixedUnitExponent pue = physicalUnit as IPrefixedUnitExponent;
                        Debug.Assert(pue != null);
                        cu = new CombinedUnit(pue);
                        break;
                    case UnitKind.DerivedUnit:
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        Debug.Assert(du != null);
                        cu = new CombinedUnit(du);
                        break;
                    case UnitKind.MixedUnit:
                        IMixedUnit mu = physicalUnit as IMixedUnit;
                        Debug.Assert(mu != null);
                        cu = new CombinedUnit(mu.MainUnit);
                        break;
                    case UnitKind.CombinedUnit:
                        ICombinedUnit cu2 = physicalUnit as ICombinedUnit;
                        Debug.Assert(cu2 != null);
                        cu = new CombinedUnit(cu2);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            if (cu != null)
            {
                this.scaleFactor = cu.FactorValue;
                this.numerators = cu.Numerators;
                this.denominators = cu.Denominators;
            }
            else
            {
                // TO DO: Convert physicalUnit to CombinedUnit
                Debug.Assert(false);
            }
        }

        public CombinedUnit(Double someScaleFactor, IPhysicalUnit physicalUnit)
            : this(physicalUnit)
        {
            this.scaleFactor *= someScaleFactor;
        }

        public CombinedUnit(INamedSymbolUnit namedSymbolUnit)
            : this(new PrefixedUnitExponent(null, namedSymbolUnit, 1))
        {
        }

        public CombinedUnit(IPrefixedUnit prefixedUnit)
            : this(new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, 1))
        {
        }


        public CombinedUnit(IDerivedUnit derivedUnit)
        {
            IPrefixedUnitExponentList someNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList someDenominators = new PrefixedUnitExponentList();

            IUnitSystem system = derivedUnit.ExponentsSystem;

            int length = derivedUnit.Exponents.Length;
            foreach (Byte i in Enumerable.Range(0, length))
            {
                SByte exp = derivedUnit.Exponents[i];
                if (exp != 0)
                {
                    if (exp > 0)
                    {
                        someNumerators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], exp));
                    }
                    else
                    {
                        someDenominators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], (sbyte)(-exp)));
                    }

                }
            }

            this.scaleFactor = derivedUnit.FactorValue;
            this.numerators = someNumerators;
            this.denominators = someDenominators;
        }

        public override UnitKind Kind => UnitKind.CombinedUnit;

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
                List<IUnitSystem> subUnitSystems = null;
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
                                if (subUnitSystems == null)
                                {   // First time we have found a second system. Add system as first system in list of systems
                                    subUnitSystems = new List<IUnitSystem>();
                                    if (!system.IsCombinedUnitSystem)
                                    {
                                        subUnitSystems.Add(system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)system;
                                        subUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }

                                {   // Add pue_system to list of systems
                                    if (!pue_system.IsCombinedUnitSystem)
                                    {
                                        subUnitSystems.Add(pue_system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)pue_system;
                                        subUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }
                            }
                        }
                    }
                }

                if (subUnitSystems != null)
                {
                    if (subUnitSystems.Any(us => us.IsIsolatedUnitSystem))
                    {   // Must combine the unit systems into one unit system
                        system = CombinedUnitSystem.GetCombinedUnitSystem(subUnitSystems.Distinct().ToArray());
                    }
                    else
                    {
                        IUnitSystem DefaultUnitSystem = Physics.CurrentUnitSystems.Default;
                        if (subUnitSystems.Contains(DefaultUnitSystem))
                        {
                            system = DefaultUnitSystem;
                        }
                        else
                        {
                            // system = SubUnitSystems.First(us => !us.IsIsolatedUnitSystem);
                            system = subUnitSystems.First();
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
                        Debug.WriteLine("CombinedUnit.Exponents() missing ExponentsSystem");
                        Debug.Assert(anySystem != null, "CombinedUnit.Exponents() missing ExponentsSystem");
                        anySystem = this.SomeSimpleSystem;
                        if (anySystem == null)
                        {
                            Debug.WriteLine("CombinedUnit.Exponents() missing also SomeSystem");
                            Debug.Assert(anySystem != null, "CombinedUnit.Exponents() missing also SomeSystem");

                            anySystem = Physics.SI_Units;
                        }
                    }
                    if (anySystem != null)
                    {
                        IPhysicalQuantity baseUnit_pq = null;
                        IPhysicalUnit baseUnit_pu = null;

                        try
                        {
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
                            Debug.WriteLine("CombinedUnit.ConvertToDerivedUnit() are missing base unit and exponents");
                            Debug.Assert(exponents != null, "CombinedUnit.ConvertToDerivedUnit() are missing base unit and exponents");
                        }
                        catch
                        {
                            Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents");
                            Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() missing exponents");
                        Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() missing exponents");
                    }
                }
                return exponents;
            }
        }

        public ICombinedUnit OnlySingleSystemUnits(IUnitSystem us)
        {
            PrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    tempNumerators.Add(pue);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    tempDenominators.Add(pue);
                }
            }
            CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
            return cu;
        }


        public override Double FactorValue => scaleFactor;

        public override IPhysicalUnit PureUnit
        {
            get
            {
                IPhysicalUnit pureunit = this;
                if (scaleFactor != 0)
                {
                    pureunit = new CombinedUnit(Numerators, Denominators);
                }
                return pureunit;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsLinearConvertible()
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
        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity)
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

                // Combined unit with sub units of different but convertible systems 
                IPhysicalQuantity pq = this.ConvertToSystemUnit();

                if (pq != null)
                {
                    pq = pq.ConvertToBaseUnit();
                }
                return pq;
            }

            Debug.Assert(system != null);

           Double value = scaleFactor;
            IPhysicalUnit unit = null;

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.AsPhysicalQuantity();
                IPhysicalUnit pue_pq_Unit = pue_pq.Unit;
                if (pue_pq_Unit != null)
                {
                    IPhysicalQuantity pq_baseunit = pue_pq_Unit.ConvertToBaseUnit();
                    IPhysicalUnit baseunit = pq_baseunit.Unit;

                    value *= pue_pq.Value * pq_baseunit.Value;

                    if (unit == null)
                    {
                        unit = baseunit;
                    }
                    else
                    {
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
                    IPhysicalUnit baseunit = pq_baseunit.Unit;

                    value /= pue_pq.Value * pq_baseunit.Value;

                    if (unit == null)
                    {
                        unit = baseunit.CombinePow(-1);
                    }
                    else
                    {
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
               Double ScaledQuantity = scaleFactor * quantity;
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


        public override IPhysicalQuantity ConvertToDerivedUnit()
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
               Double scaledQuantity = scaleFactor;
                IPhysicalQuantity pq1 = this.ConvertToSystemUnit();

                if (pq1 != null)
                {
                    // Simple system DerivedUnit
                    Debug.Assert(scaledQuantity == 1.0);
                    Debug.Assert(pq1.Unit.SimpleSystem != null);
                }
                else
                {
                    // Combined system DerivedUnit
                    system = this.ExponentsSystem;
                    Debug.Assert(system != null && system.IsCombinedUnitSystem);

                    pq1 = this.ConvertToBaseUnit();
                }

                pq1 = pq1.ConvertToDerivedUnit();
                return pq1;
            }

            Debug.Assert(system != null);

            IPhysicalQuantity pq = new PhysicalQuantity(scaleFactor, system.Dimensionless);

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
           Double value = this.FactorValue;
            IPhysicalUnit unit = null;

            Debug.Assert(convertToUnitSystem != null);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
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

        public override IPhysicalQuantity ConvertTo(ref Double quantity, IUnitSystem convertToUnitSystem)
        {
            IPhysicalQuantity pq = this.ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.Multiply(quantity);
            }
            return pq;
        }

        public IPhysicalQuantity ConvertFrom(IPhysicalQuantity physicalQuantity)
        {

            IPhysicalQuantity pq_unit = physicalQuantity;
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

            return pq_unit;
        }

        #region IPhysicalUnitMath Members

        public override IPhysicalUnit Dimensionless => new CombinedUnit();

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

        private void MultiplyPUEL(IPrefixedUnitExponent prefixedUnitExponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1,
                  ref Boolean primaryUnitFound, ref SByte prefixExponent, ref SByte prefixedUnitExponentScaleing,
                  ref SByte multExponent, ref Boolean changedExponentSign)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                if (!primaryUnitFound && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    primaryUnitFound = true;

                    if (!prefixExponent.Equals(ue.Prefix.Exponent))
                    {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                        prefixedUnitExponentScaleing = (SByte)((ue.Prefix.Exponent - prefixExponent) * multExponent);
                        prefixExponent = ue.Prefix.Exponent;
                    }

                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    SByte newExponent = cef(ue.Exponent, multExponent);
                    if (newExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, newExponent);
                        outPuel1.Add(temp_pue);
                        // Done
                    }
                    else
                    {   // Convert to uppersit Numerator/Denominator
                        multExponent = cef(0, newExponent); 
                        changedExponentSign = true;
                    }
                }
                else
                {
                    outPuel1.Add(ue);
                }
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
                return unit;
            }

            SByte multExponent = prefixedUnitExponent.Exponent;
            SByte prefixExponent = prefixedUnitExponent.Prefix.Exponent;
            SByte prefixedUnitExponentScaleing = 1;


            IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            Boolean primaryUnitFound = false;
            Boolean changedExponentSign = false;
            //// Check if pue2.Unit is already among our Numerators or Denominators

            MultiplyPUEL(prefixedUnitExponent, SByte_Sub, Denominators, ref tempDenominators,
                  ref primaryUnitFound, ref prefixExponent, ref prefixedUnitExponentScaleing,
                  ref multExponent, ref changedExponentSign);

            MultiplyPUEL(prefixedUnitExponent, SByte_Add, Numerators, ref tempNumerators,
                  ref primaryUnitFound, ref prefixExponent, ref prefixedUnitExponentScaleing,
                  ref multExponent, ref changedExponentSign);

            if (!primaryUnitFound || changedExponentSign)
            {   // pue2.Unit is not among our Numerators or Denominators (or has changed from Numerators to Denominators)
                if (!primaryUnitFound && (prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit))
                {
                    // TO DO: Can this happen? The case of prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit should already have been handled  
                    Debug.Assert(false);

                    ICombinedUnit cu1 = new CombinedUnit(tempNumerators, tempDenominators);
                    ICombinedUnit cu2 = prefixedUnitExponent.Unit as ICombinedUnit;

                   Double PrefixScale = 1;
                    SByte PrefixExponent = 0;
                    foreach (IPrefixedUnitExponent pue2Num_pue in cu2.Numerators)
                    {
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
                       Double tempPrefixScale;
                        SByte tempPrefixExponent;
                        IPrefixedUnitExponent combinedPUE = pue2DOM_pue.CombinePrefixAndExponents(prefixedUnitExponent.Prefix.Exponent, multExponent, out tempPrefixExponent, out tempPrefixScale);

                        cu1 = cu1.CombineMultiply(combinedPUE);
                        PrefixExponent -= tempPrefixExponent;
                        if (PrefixScale != 1)
                        {
                            PrefixScale *= tempPrefixScale;
                        }
                    }
                   Double exponent_d = Math.Log10(PrefixScale);
                    SByte exp = (SByte)exponent_d;
                   Double diff = exponent_d - exp;
                    Debug.Assert(diff == 0);

                    PrefixedUnitExponent temp_pue = null; ;
                    IUnitPrefixExponent pe = new UnitPrefixExponent(exp);
                    IUnitPrefix up = null;
                    if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(pe, out up))
                    {
                        temp_pue = new PrefixedUnitExponent(up, null, 1);
                    }
                    else
                    {
                        // TO DO: Handle to make result as PrefixedUnitExponent
                        Debug.Assert(false);
                        temp_pue = null;
                    }
                    cu1 = cu1.CombineMultiply(temp_pue);

                    return cu1;
                }
                else
                {
                    if (multExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                        tempNumerators.Add(temp_pue);
                    }
                    else if (multExponent < 0)
                    {
                        multExponent = (SByte)(-multExponent);
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                        tempDenominators.Add(temp_pue);
                    }
                }
            }

            CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
            return cu;
        }

        public override IPhysicalUnit Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte newExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, newExponent);
            return temp_pue;
        }


        public override IPhysicalQuantity Multiply(double quantity) => this * quantity;

        public override IPhysicalQuantity Divide(double quantity) => this / quantity;

        public override PhysicalUnit Power(SByte exponent) => new CombinedUnit(Numerators.Power(exponent), Denominators.Power(exponent));

        public override PhysicalUnit Root(SByte exponent)
        {
            IPrefixedUnitExponentList tempNumerators;
            IPrefixedUnitExponentList tempDenominators = null;
            tempNumerators = Numerators.Root(exponent);
            if (tempNumerators != null)
            {
                tempDenominators = Denominators.Root(exponent);
            }

            if ((tempNumerators != null) && (tempDenominators != null))
            {
                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
                return cu;
            }
            else
            {
                SByte[] newExponents = this.Exponents;
                if (newExponents != null)
                {
                    newExponents = newExponents.Root(exponent);
                    Debug.Assert(this.ExponentsSystem != null);
                    DerivedUnit du = new DerivedUnit(this.ExponentsSystem, newExponents);
                    return du;
                }
                else
                {
                    Debug.Assert(newExponents != null);
                    //if (throwExceptionOnUnitMathError) {
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
           Double factor = this.FactorValue * quantity;
            ICombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }

        public override ICombinedUnit CombineDivide(double quantity)
        {
           Double factor = this.FactorValue / quantity;
            ICombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }
        public override ICombinedUnit CombineMultiply(IUnitPrefixExponent prefixExponent) => this.CombineMultiply(prefixExponent.Value);

        public override ICombinedUnit CombineDivide(IUnitPrefixExponent prefixExponent) => this.CombineDivide(prefixExponent.Value);

        private void CombineFactorPUEL(IPrefixedUnitExponent prefixedUnitExponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1, ref IPrefixedUnitExponentList outPuel2, 
                  ref Boolean primaryUnitFound, ref SByte pue_prefixExp, ref sbyte scalingPrefixExponent, ref SByte scalingExponent,
                  ref SByte multPrefixExponent, ref SByte multExponent, ref Boolean ChangedExponentSign)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                if (!primaryUnitFound && prefixedUnitExponent.Unit != null && ue.Unit != null && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    primaryUnitFound = true;

                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    SByte newExponent = (SByte)(cef(ue.Exponent, prefixedUnitExponent.Exponent));

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

                    if (newExponent > 0)
                    {   // Still some exponent left for a denominator element
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, newExponent);
                        outPuel1.Add(temp_pue);
                        // Done
                    }
                    else
                    if (newExponent< 0)
                    {   // Convert to Numerator
                        multPrefixExponent = ue.Prefix.Exponent;
                        multExponent = (SByte)(-newExponent);
                        ChangedExponentSign = true;
                    }
                }
                else
                {
                    if (ue.Exponent > 0)
                    {
                        outPuel1.Add(ue);
                    }
                    else
                    {
                        outPuel2.Add(new PrefixedUnitExponent(ue.Prefix, ue.Unit, (SByte)(-ue.Exponent)));
                    }
                }
            }
        }

        public override ICombinedUnit CombineMultiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

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

            SByte pue_prefixExp = 0; // 10^0 = 1
            if (prefixedUnitExponent.Prefix != null)
            {
                pue_prefixExp = prefixedUnitExponent.Prefix.Exponent;
            }

            IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            Boolean primaryUnitFound = false;
            Boolean changedExponentSign = false;

            CombineFactorPUEL(prefixedUnitExponent, SByte_Sub, Denominators, ref tempDenominators, ref tempNumerators,
                  ref primaryUnitFound, ref pue_prefixExp, ref scalingPrefixExponent, ref scalingExponent,
                  ref multPrefixExponent, ref multExponent, ref changedExponentSign);
            CombineFactorPUEL(prefixedUnitExponent, SByte_Add, Numerators, ref tempNumerators, ref tempDenominators,
                  ref primaryUnitFound, ref pue_prefixExp, ref scalingPrefixExponent, ref scalingExponent,
                  ref multPrefixExponent, ref multExponent, ref changedExponentSign);

            if (!primaryUnitFound || changedExponentSign)
            {
                if (!primaryUnitFound)
                {
                    if (prefixedUnitExponent.Prefix != null)
                    {
                        multPrefixExponent = prefixedUnitExponent.Prefix.Exponent;
                    }
                    multExponent = prefixedUnitExponent.Exponent;
                }

                IUnitPrefix unitPrefix = null;
                SByte restMultPrefixExponent = 0;
                if (multPrefixExponent != 0)
                {
                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(multPrefixExponent, out unitPrefix, out restMultPrefixExponent);
                    multPrefixExponent = restMultPrefixExponent;
                }

                if (multExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, multExponent);
                    tempNumerators.Add(temp_pue);
                }
                else if (multExponent < 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, (SByte)(-multExponent));
                    tempDenominators.Add(temp_pue);
                }
            }

           Double resScaleFactor = scaleFactor;
            if (scalingPrefixExponent != 0 && scalingExponent != 0)
            {   // Add scaling factor without unit
                sbyte exp = (sbyte)(scalingPrefixExponent * scalingExponent);

                resScaleFactor = scaleFactor * Math.Pow(10, exp);
            }

            CombinedUnit cu = new CombinedUnit(resScaleFactor, tempNumerators, tempDenominators);
            return cu;
        }

        public override ICombinedUnit CombineDivide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte newExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, newExponent);
            return this.CombineMultiply(temp_pue);
        }


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
            ICombinedUnit result = this.CombineMultiply(new PrefixedUnitExponent(null, namedSymbolUnit, -1));
            return result;
        }

        public override ICombinedUnit CombineMultiply(IPhysicalUnit physicalUnit)
        {
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


        protected void CombineExponentPUEL(SByte exponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1, ref IPrefixedUnitExponentList outPuel2)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                SByte NewExponent = (SByte)(cef(ue.Exponent , exponent));
                if (NewExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                    outPuel1.Add(temp_pue);
                }
                if (NewExponent< 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, (sbyte)(-NewExponent));
                    outPuel2.Add(temp_pue);
                }
            }
        }


        public override ICombinedUnit CombinePow(SByte exponent)
        {
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
                IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

                CombineExponentPUEL(exponent, SByte_Mult, Numerators, ref tempNumerators, ref tempDenominators);
                CombineExponentPUEL(exponent, SByte_Mult, Denominators, ref tempDenominators, ref tempNumerators);

                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
                return cu;
            }
        }

        public override ICombinedUnit CombineRot(SByte exponent)
        {
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
                IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

                CombineExponentPUEL(exponent, SByte_Div, Numerators, ref tempNumerators, ref tempDenominators);
                CombineExponentPUEL(exponent, SByte_Div, Denominators, ref tempDenominators, ref tempNumerators);

                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
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
                cu1 = cu1.CombineMultiply(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                cu1 = cu1.CombineDivide(pue);
            }

            return cu1;
        }

        public ICombinedUnit CombineDivide(ICombinedUnit cu2)
        {
            ICombinedUnit cu1 = new CombinedUnit(this.FactorValue / cu2.FactorValue, this.Numerators, this.Denominators);

            foreach (IPrefixedUnitExponent pue in cu2.Numerators)
            {
                cu1 = cu1.CombineDivide(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                cu1 = cu1.CombineMultiply(pue);
            }

            return cu1;
        }

        #endregion IPhysicalUnitMath Members

        #region IEquatable<IPhysicalUnit> Members

        public override Int32 GetHashCode() => numerators.GetHashCode() + denominators.GetHashCode();

        public override Boolean Equals(object other)
        {
            if (other == null)
                return false;

            IPhysicalUnit otherIPU = other as IPhysicalUnit;

            if (otherIPU == null)
                return false;

            return this.Equals(otherIPU);
        }

        #endregion IEquatable<IPhysicalUnit> Members

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => CombinedUnitString(mayUseSlash: true, invertExponents: false);

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String unitName = "";
            Boolean nextLevelMayUseSlash = mayUseSlash && Denominators.Count == 0;
            if (Numerators.Count > 0)
            {
                unitName = Numerators.CombinedUnitString(nextLevelMayUseSlash, invertExponents);
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
                if (mayUseSlash && !String.IsNullOrWhiteSpace(unitName))
                {
                    unitName += "/" + Denominators.CombinedUnitString(false, invertExponents);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(unitName))
                    {
                        // center dot '\0x0B7' (Char)183 U+00B7
                        unitName += '·' + Denominators.CombinedUnitString(false, !invertExponents);
                    }
                    else
                    {
                        unitName = Denominators.CombinedUnitString(false, !invertExponents);
                    }
                }
            }
            return unitName;
        }
        public override String ToString() => UnitString();

    }

    #endregion Combined Unit Classes

    #region Mixed Unit Classes

    public class MixedUnit : PhysicalUnit, IMixedUnit
    {
        protected readonly IPhysicalUnit mainUnit;
        protected readonly IPhysicalUnit fractionalUnit;

        protected readonly String separator;
        protected readonly String fractionalValueFormat;

        public IPhysicalUnit MainUnit
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return this.mainUnit;
            }
        }

        public IPhysicalUnit FractionalUnit
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return this.fractionalUnit;
            }
        }

        public String Separator
        {
            get
            {
                Debug.Assert(separator != null);
                return this.separator;
            }
        }

        public MixedUnit(IPhysicalUnit someMainUnit, String someSeparator, IPhysicalUnit someFractionalUnit, String someFractionalValueFormat)
        {
            this.mainUnit = someMainUnit;
            this.separator = someSeparator;
            this.fractionalUnit = someFractionalUnit;
            this.fractionalValueFormat = someFractionalValueFormat;
        }

        public MixedUnit(IPhysicalUnit someMainUnit, String separator, IPhysicalUnit someFractionalUnit)
            : this(someMainUnit, separator, someFractionalUnit, "00.################")
        {
        }

        public MixedUnit(IPhysicalUnit someMainUnit, IPhysicalUnit someFractionalUnit)
            : this(someMainUnit, ":", someFractionalUnit)
        {
        }

        public override UnitKind Kind => UnitKind.MixedUnit;

        public override IUnitSystem SimpleSystem
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.SimpleSystem;
            }
            set
            {
                Debug.Assert(mainUnit != null);
                /* Just do nothing */
                Debug.Assert(MainUnit.SimpleSystem == value);

            }
        }

        public override IUnitSystem ExponentsSystem
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.ExponentsSystem;
            }
        }

        public override SByte[] Exponents
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.Exponents;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsLinearConvertible()
        {
            Debug.Assert(mainUnit != null);
            return mainUnit.IsLinearConvertible();
        }


        public override IPhysicalQuantity ConvertToSystemUnit(ref Double quantity)
        {
            Debug.Assert(mainUnit != null);
            return MainUnit.ConvertToSystemUnit(ref quantity);
        }

        public override IPhysicalQuantity ConvertToSystemUnit()
        {
            Debug.Assert(mainUnit != null);
            return MainUnit.ConvertToSystemUnit();
        }

        public override IPhysicalQuantity ConvertToBaseUnit() => this.ConvertToBaseUnit(1);

        public override IPhysicalQuantity ConvertToDerivedUnit() => this.ConvertToBaseUnit();

        public override IPhysicalQuantity ConvertToBaseUnit(Double quantity) => this.ConvertToSystemUnit(ref quantity).ConvertToBaseUnit();

        public override string PureUnitString()
        {
            Debug.Assert(mainUnit != null);

            string us = MainUnit.UnitString();
            if (FractionalUnit != null)
            {
                us = us + this.Separator + FractionalUnit.UnitString();
            }
            return us;
        }

        public override string ValueString(Double quantity) => ValueString(quantity, null, null);

        public override string ValueString(Double quantity, String format, IFormatProvider formatProvider)
        {
            Debug.Assert(mainUnit != null);

            string valStr;
            if (FractionalUnit != null)
            {
               Double integralValue = Math.Truncate(quantity);
               Double fractionalValue = quantity - integralValue;
                IPhysicalQuantity fracPQ = new PhysicalQuantity(fractionalValue, this.MainUnit);
                IPhysicalQuantity fracPQConv = fracPQ.ConvertTo(this.FractionalUnit);
                if (fracPQConv != null)
                {
                    valStr = MainUnit.ValueString(integralValue, format, formatProvider) + separator + FractionalUnit.ValueString(fracPQConv.Value, fractionalValueFormat, null);
                }
                else
                {
                    valStr = MainUnit.ValueString(quantity, format, formatProvider);
                }
            }
            else
            {
                valStr = MainUnit.ValueString(quantity, format, formatProvider);
            }
            return valStr;
        }
    }

    #endregion Mixed Unit Classes

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public abstract class AbstractUnitSystem : NamedObject, IUnitSystem
    {

        public abstract IUnitPrefixTable UnitPrefixes { get; }
        public abstract IBaseUnit[] BaseUnits { get; /* */ set; /* */ }
        public abstract INamedDerivedUnit[] NamedDerivedUnits { get; /* */ set; /* */ }
        public abstract IConvertibleUnit[] ConvertibleUnits { get; /* */ set; /* */ }

        public abstract Boolean IsIsolatedUnitSystem { get; }
        public abstract Boolean IsCombinedUnitSystem { get; }

        protected IPhysicalUnit dimensionless;
        public virtual IPhysicalUnit Dimensionless
        {
            get
            {
                if (dimensionless == null)
                {
                    dimensionless = new DerivedUnit(this, new SByte[] { 0 });
                }
                return dimensionless;
            }
        }

        public AbstractUnitSystem(String someName)
            : base(someName)
        {
        }

        public override String ToString() => this.Name;

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

        public IPhysicalUnit ScaledUnitFromSymbol(String scaledUnitSymbol)
        {
            INamedSymbolUnit unit = UnitFromSymbol(scaledUnitSymbol);
            if (scaledUnitSymbol.Length > 1)
            {   // Check for prefixed unit 
                Char prefixchar = scaledUnitSymbol[0];
                IUnitPrefix unitPrefix;
                if (UnitPrefixes.GetUnitPrefixFromPrefixChar(prefixchar, out unitPrefix))
                {
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
                                return unit;
                            }
                            // Prefer unit2
                            // Overwrite unit even if set by non-prefixed unit (first call to UnitFromSymbol())
                            Debug.Assert(unit == null); // For debug. Manually check if overwritten unit could be a better choice than unit 2.
                        }

                        // Found both a prefix and an unit; Must be the right unit. 
                        unit = unit2;

                        Debug.Assert(unitPrefix != null); // GetUnitPrefixFromPrefixChar must have returned a valid unitPrefix
                        if (unitPrefix != null)
                        {
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
            SByte noOfNonZeroExponents = 0;
            SByte noOfNonOneExponents = 0;
            SByte firstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in exponents)
            {
                if (exponent != 0)
                {
                    if (firstNonZeroExponent == -1)
                    {
                        firstNonZeroExponent = i;
                    }
                    noOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        noOfNonOneExponents++;
                    }
                }

                i++;
            }

            return UnitFromUnitInfo(exponents, noOfNonZeroExponents, noOfNonOneExponents, firstNonZeroExponent);
        }

        public IPhysicalUnit UnitFromUnitInfo(SByte[] exponents, SByte noOfNonZeroExponents, SByte noOfNonOneExponents, SByte firstNonZeroExponent)
        {
            IPhysicalUnit unit;

            if ((noOfNonZeroExponents == 1) && (noOfNonOneExponents == 0))
            {
                // BaseUnit 
                unit = (IPhysicalUnit)BaseUnits[firstNonZeroExponent];
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
            int noOfDimensions = exponents.NoOfDimensions();
            if (noOfDimensions > 1)
            {
                INamedSymbolUnit ns = NamedDerivedUnits.FirstOrNull(namedderivedunit => exponents.DimensionEquals(namedderivedunit.Exponents));
                return ns;
            }

            return null;
        }

        public INamedSymbolUnit NamedDerivedUnitFromUnit(IPhysicalUnit derivedUnit)
        {
            IPhysicalQuantity pq = derivedUnit.ConvertToDerivedUnit();
            if (PhysicalQuantity.IsPureUnit(pq))
            {
                IPhysicalUnit derunit = PhysicalQuantity.PureUnit(pq);
                SByte[] exponents = derunit.Exponents;
                int noOfDimensions = exponents.NoOfDimensions();
                if (noOfDimensions > 1)
                {
                    foreach (NamedDerivedUnit namedderivedunit in this.NamedDerivedUnits)
                    {
                        if (exponents.DimensionEquals(namedderivedunit.Exponents))
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
                throw new ArgumentNullException(nameof(convertFromUnit));
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException(nameof(convertToUnit));
            }
           Double quotient = 1;  // 0 means not equivalent unit
            Boolean isEquivalentUnit = convertFromUnit.Equivalent(convertToUnit, out quotient);
            if (isEquivalentUnit)
            {
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
                        throw new ArgumentException("Must have a unit of BaseUnit or DerivedUnit", nameof(convertFromUnit));
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or DerivedUnit", nameof(convertToUnit));
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
            Boolean physicalQuantityUnitRelativeconversion = physicalQuantity.Unit.IsLinearConvertible();
            Boolean convertToUnitRelativeconversion = convertToUnit.IsLinearConvertible();
            Boolean relativeconversion = physicalQuantityUnitRelativeconversion && convertToUnitRelativeconversion;
            if (relativeconversion)
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
                throw new ArgumentException("Must have a unit", nameof(physicalQuantity));
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException(nameof(convertToUnit));
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
                       Double d = physicalQuantity.Value;
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
                       Double d = physicalQuantity.Value;
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
                        throw new ArgumentException("Must have a unit of BaseUnit or a DerivedUnit", nameof(physicalQuantity));
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or a DerivedUnit", nameof(convertToUnit));
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

    public class UnitSystem : AbstractUnitSystem
    {
        private /* readonly */ UnitPrefixTable unitPrefixes;
        private /* readonly */ BaseUnit[] baseUnits;
        private /* readonly */ NamedDerivedUnit[] namedDerivedUnits;
        private /* readonly */ ConvertibleUnit[] convertibleUnits;

        private /* readonly */ Boolean isIsolated;

        public override IUnitPrefixTable UnitPrefixes => unitPrefixes;
        public override IBaseUnit[] BaseUnits { get { return baseUnits; } /* */ set { baseUnits = (BaseUnit[])value; CheckBaseUnitSystem(); } /* */ }
        public override INamedDerivedUnit[] NamedDerivedUnits { get { return namedDerivedUnits; } /* */ set { namedDerivedUnits = (NamedDerivedUnit[])value; CheckNamedDerivedUnitSystem(); } /* */  }
        public override IConvertibleUnit[] ConvertibleUnits { get { return convertibleUnits; } /* */ set { convertibleUnits = (ConvertibleUnit[])value; CheckConvertibleUnitSystem(); } /* */  }

        public override Boolean IsIsolatedUnitSystem => isIsolated;
        public override Boolean IsCombinedUnitSystem => false;

        public UnitSystem(String someName, Boolean someIsIsolated)
            : base(someName)
        {
            this.isIsolated = someIsIsolated;
        }

        public UnitSystem(String someName)
            : this(someName, true)
        {
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes)
            : this(someName, false)
        {
            this.unitPrefixes = someUnitPrefixes;
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits)
            : this(someName, someUnitPrefixes)
        {
            this.baseUnits = someBaseUnits;

            CheckBaseUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits, NamedDerivedUnit[] someNamedDerivedUnits)
            : this(someName, someUnitPrefixes, someBaseUnits)
        {
            this.namedDerivedUnits = someNamedDerivedUnits;

            CheckNamedDerivedUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits, NamedDerivedUnit[] someNamedDerivedUnits, ConvertibleUnit[] someConvertibleUnits)
            : this(someName, someUnitPrefixes, someBaseUnits, someNamedDerivedUnits)
        {
            this.convertibleUnits = someConvertibleUnits;

            CheckConvertibleUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit someBaseUnit, NamedDerivedUnit[] someNamedDerivedUnits, ConvertibleUnit[] someConvertibleUnits)
            : this(someName, someUnitPrefixes, new BaseUnit[] { someBaseUnit }, someNamedDerivedUnits)
        {
            this.isIsolated = someBaseUnit.BaseUnitNumber == (SByte)MonetaryBaseQuantityKind.Currency;
            this.convertibleUnits = someConvertibleUnits;

            CheckConvertibleUnitSystem();
        }

        private void CheckBaseUnitSystem()
        {
            Debug.Assert(this.baseUnits != null);

            foreach (BaseUnit aBaseUnit in this.baseUnits)
            {
                Debug.Assert(aBaseUnit.Kind == UnitKind.BaseUnit);
                if (aBaseUnit.Kind != UnitKind.BaseUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.BaseUnit", "BaseUnits");
                }
                if (aBaseUnit.SimpleSystem != this)
                {
                    Debug.Assert(aBaseUnit.SimpleSystem == null);
                    aBaseUnit.SimpleSystem = this;
                }
            }
        }

        private void CheckNamedDerivedUnitSystem()
        {
            if (this.namedDerivedUnits != null)
            {
                foreach (NamedDerivedUnit namedderivedunit in this.namedDerivedUnits)
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
            if (this.convertibleUnits != null)
            {
                foreach (ConvertibleUnit convertibleunit in this.convertibleUnits)
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



    public class CombinedUnitSystem : AbstractUnitSystem, ICombinedUnitSystem
    {
        private /* readonly */ IUnitSystem[] unitSystemes;

        private /* readonly */ IUnitPrefixTable unitprefixes;
        private /* readonly */ IBaseUnit[] baseunits;
        private /* readonly */ INamedDerivedUnit[] namedderivedunits;
        private /* readonly */ IConvertibleUnit[] convertibleunits;


        public IUnitSystem[] UnitSystemes => unitSystemes;

        public override IUnitPrefixTable UnitPrefixes => unitprefixes;

        public override IBaseUnit[] BaseUnits { get { return baseunits; }  /* */ set { throw new NotImplementedException(); } /* */ }

        public override INamedDerivedUnit[] NamedDerivedUnits { get { return namedderivedunits; } /* */ set { throw new NotImplementedException(); } /* */ }
        public override IConvertibleUnit[] ConvertibleUnits { get { return convertibleunits; } /* */ set { throw new NotImplementedException(); } /* */ }


        public override IPhysicalUnit Dimensionless => UnitSystemes[0].Dimensionless;

        public override Boolean IsIsolatedUnitSystem => UnitSystemes.All(us => us.IsIsolatedUnitSystem);
        public override Boolean IsCombinedUnitSystem => true;

        public Boolean ContainsSubUnitSystem(IUnitSystem unitsystem) => UnitSystemes.Contains(unitsystem);

        public Boolean ContainsSubUnitSystems(IEnumerable<IUnitSystem> someUnitSystems) => UnitSystemes.Union(someUnitSystems).Count() == UnitSystemes.Count();

        private static List<ICombinedUnitSystem> combinedUnitSystems = new List<ICombinedUnitSystem>();

        public static ICombinedUnitSystem GetCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            Debug.Assert(!subUnitSystems.Any(us => us.IsCombinedUnitSystem));
            IUnitSystem[] sortedSubUnitSystems = subUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray();
            ICombinedUnitSystem cus = null;

            if (combinedUnitSystems.Count() > 0)
            {
                IEnumerable<ICombinedUnitSystem> tempUnitSystems = combinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                if (tempUnitSystems.Count() >= 1)
                {
                    Debug.Assert(tempUnitSystems.Count() == 1);
                    cus = tempUnitSystems.First();
                }
            }
            if (cus == null)
            {
                lock (combinedUnitSystems)
                {
                    IEnumerable<ICombinedUnitSystem> tempUnitSystems = combinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                    if (tempUnitSystems.Count() >= 1)
                    {
                        cus = tempUnitSystems.First();
                    }
                    else
                    {
                        cus = new CombinedUnitSystem(null, sortedSubUnitSystems);
                        combinedUnitSystems.Add(cus);
                    }
                }
            }

            return cus;
        }

        public CombinedUnitSystem(String someName, IUnitSystem us1, IUnitSystem us2)
            : this(someName, new IUnitSystem[] { us1, us2 })
        {

        }

        public CombinedUnitSystem(String someName, IUnitSystem[] someSubUnitSystems)
            : base(someName != null ? someName : "<" + someSubUnitSystems.Aggregate("", ((str, us) => String.IsNullOrWhiteSpace(str) ? us.Name : str + ", " + us.Name)) + ">")
        {
            SetupCombinedUnitSystem(someSubUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray());
        }

        public void SetupCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            unitSystemes = subUnitSystems;

            IUnitPrefix[] tempUnitprefixes = null;
            IBaseUnit[] tempBaseUnits = null;
            INamedDerivedUnit[] tempNamedDerivedUnits = null;
            IConvertibleUnit[] tempConvertibleUnits = null;

            foreach (IUnitSystem us in unitSystemes)
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

            unitprefixes = new UnitPrefixTable(tempUnitprefixes);

            baseunits = tempBaseUnits;
            namedderivedunits = tempNamedDerivedUnits;
            convertibleunits = tempConvertibleUnits;
        }


        public SByte[] UnitExponents(ICombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] unitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] unitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                unitSystemExponentsOffsets[index] = noOfDimensions;
                unitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += unitSystemExponentsLength[index];

            }

            SByte[] resExponents = new sbyte[0];

            // Split cu in to parts for each sub unit system in UnitSystemes
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems];

            // Split into subUnitParts
            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);
                SByte[] us_exponents = subUnitParts[i].Exponents;
                if (us_exponents.Length < unitSystemExponentsLength[index])
                {
                    us_exponents = us_exponents.AllExponents(unitSystemExponentsLength[index]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            return resExponents;
        }

        public IPhysicalQuantity ConvertToBaseUnit(ICombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] unitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] unitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                unitSystemExponentsOffsets[index] = noOfDimensions;
                unitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += unitSystemExponentsLength[index];
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
                if (us_exponents.Length < unitSystemExponentsLength[i])
                {
                    us_exponents = us_exponents.AllExponents(unitSystemExponentsLength[i]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            // Handle part of cu without (sub-)unit system
            subUnitParts[noOfSubUnitSystems] = cu.OnlySingleSystemUnits(null); // no unit system

            IPhysicalQuantity pq2 = subUnitParts[noOfSubUnitSystems].ConvertToBaseUnit();
            resValue *= pq2.Value;

            IPhysicalUnit derivatedUnit = new DerivedUnit(this, resExponents);
            return new PhysicalQuantity(resValue, derivatedUnit);
        }

        public override int GetHashCode()
        {
            if (unitSystemes == null)
            {
                return base.GetHashCode();
            }
            return unitSystemes.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;

            ICombinedUnitSystem ICombinedUnitSystemObj = obj as ICombinedUnitSystem;
            if (ICombinedUnitSystemObj == null)
                return false;
            else
                return Equals(ICombinedUnitSystemObj);
        }

        public Boolean Equals(ICombinedUnitSystem other) => Equals(this.UnitSystemes, other.UnitSystemes);
    }

    #endregion Physical Unit System Classes

    #region Physical Unit System Conversion Classes

    public class UnitSystemConversion
    {
        private IUnitSystem baseUnitSystem;

        public IUnitSystem BaseUnitSystem => baseUnitSystem;

        private IUnitSystem convertedUnitSystem;

        public IUnitSystem ConvertedUnitSystem => convertedUnitSystem;

        public ValueConversion[] BaseUnitConversions;

        public UnitSystemConversion(IUnitSystem someBaseUnitsystem, IUnitSystem someConvertedUnitsystem, ValueConversion[] someBaseUnitConversions)
        {
            this.baseUnitSystem = someBaseUnitsystem;
            this.convertedUnitSystem = someConvertedUnitsystem;
            this.BaseUnitConversions = someBaseUnitConversions;
        }

        public IPhysicalQuantity Convert(IPhysicalUnit convertUnit, Boolean backwards = false)
        {
            Debug.Assert(convertUnit.Kind == UnitKind.BaseUnit || convertUnit.Kind == UnitKind.DerivedUnit);

            SByte[] fromUnitExponents = convertUnit.Exponents;

           Double convertproduct = 1;

            SByte noOfNonZeroExponents = 0;
            SByte noOfNonOneExponents = 0;
            SByte firstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in fromUnitExponents)
            {
                if (exponent != 0)
                {
                    if (firstNonZeroExponent == -1)
                    {
                        firstNonZeroExponent = i;
                    }
                    noOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        noOfNonOneExponents++;
                    }
                    ValueConversion vc = BaseUnitConversions[i];
                    if (vc != null)
                    {
                       Double baseunitconvertedvalue = vc.Convert(1, backwards);
                       Double baseunitfactor = Math.Pow(baseunitconvertedvalue, exponent);
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
           Double value = convertproduct;

            IUnitSystem unitsystem = (backwards ? BaseUnitSystem : ConvertedUnitSystem);

            IPhysicalUnit unit = unitsystem.UnitFromUnitInfo(fromUnitExponents, noOfNonZeroExponents, noOfNonOneExponents, firstNonZeroExponent);
            return new PhysicalQuantity(value, unit);
        }

        public IPhysicalQuantity Convert(IPhysicalQuantity physicalQuantity, Boolean backwards = false)
        {
            Debug.Assert(physicalQuantity != null);

            IPhysicalQuantity pq = Convert(physicalQuantity.Unit, backwards);
            return new PhysicalQuantity(physicalQuantity.Value * pq.Value, pq.Unit);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalUnit convertUnit) => Convert(convertUnit, false);

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalUnit convertUnit) => Convert(convertUnit, true);

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalQuantity physicalQuantity) => Convert(physicalQuantity, false);

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalQuantity physicalQuantity) => Convert(physicalQuantity, true);

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
        private readonly Double value;
        private readonly IPhysicalUnit unit;

        public Double Value => this.value;

        public IPhysicalUnit Unit => this.unit;

        public IPhysicalUnit Dimensionless => unit.Dimensionless;
        public Boolean IsDimensionless => unit == null || unit.IsDimensionless;

        public PhysicalQuantity()
            : this(0)
        {
        }

        public PhysicalQuantity(Double somevalue)
            : this(somevalue, Physics.dimensionless)
        {
        }

        public PhysicalQuantity(IPhysicalUnit someunit)
            : this(1, someunit)
        {
        }

        public PhysicalQuantity(Double somevalue, IPhysicalUnit someunit)
        {
            this.value = somevalue;
            this.unit = someunit;
        }

        public PhysicalQuantity(IPhysicalQuantity somephysicalquantity)
        {
            if (somephysicalquantity != null)
            {
                this.value = somephysicalquantity.Value;
                this.unit = somephysicalquantity.Unit;
            }
            else
            {
                this.value = 0;
                this.unit = Physics.dimensionless;
            }
        }

        public PhysicalQuantity(Double somevalue, IPhysicalQuantity somephysicalquantity)
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
                    return value.EpsilonCompareTo(tempconverted.Value);
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
           Double unitValue = this.Unit.FactorValue;
            IUnit pureUnit = this.Unit.PureUnit;
            String valStr = pureUnit.ValueString(this.Value * unitValue, format, formatProvider);
            String unitStr = pureUnit.ToString();
            if (String.IsNullOrEmpty(unitStr))
            {
                return valStr;
            }
            else
            {
                return valStr + " " + unitStr;
            }
        }

        public override String ToString()
        {
           Double unitValue = this.Unit.FactorValue;
            IUnit pureUnit = this.Unit.PureUnit;
            String valStr = pureUnit.ValueString(this.Value * unitValue);
            String unitStr = pureUnit.ToString();
            if (String.IsNullOrEmpty(unitStr))
            {
                return valStr;
            }
            else
            {
                return valStr + " " + unitStr;
            }
        }

        public virtual String ToPrintString()
        {
           Double unitValue = this.Unit.FactorValue;
            IUnit pureUnit = this.Unit.PureUnit;
            String valStr = pureUnit.ValueString(this.Value * unitValue);
            String unitStr = pureUnit.ToString();

            if (String.IsNullOrEmpty(unitStr))
            {
                return valStr;
            }
            else
            {
                return valStr + " " + unitStr;
            }
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitsymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, IFormatProvider provider, out PhysicalQuantity result)
        {
            result = null;

            String[] strings = physicalQuantityStr.Trim().Split(' ');

            if (strings.GetLength(0) > 0)
            {
                // Parse numerical value
                String numValueStr = strings[0];
               Double numValue;

                if (!Double.TryParse(numValueStr, styles, provider, out numValue))
                {
                    if (!Double.TryParse(numValueStr, styles, null, out numValue)) // Try  to use Default Format Provider
                    {
                        numValue = Double.Parse(numValueStr, styles, NumberFormatInfo.InvariantInfo);     // Try  to use invariant Format Provider
                    }
                }

                IPhysicalUnit unit = null;

                if (strings.GetLength(0) > 1)
                {
                    // Parse unit
                    String unitStr = strings[1];
                    unit = PhysicalUnit.Parse(unitStr);
                }
                else
                {
                    unit = Physics.dimensionless;
                }

                result = new PhysicalQuantity(numValue, unit);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitSymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, out PhysicalQuantity result) => TryParse(physicalQuantityStr, styles, null, out result);

        public static Boolean TryParse(String physicalQuantityStr, out PhysicalQuantity result) => TryParse(physicalQuantityStr, System.Globalization.NumberStyles.Float, null, out result);

        public static PhysicalQuantity Parse(String physicalQuantityStr, System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Float, IFormatProvider provider = null)
        {
            PhysicalQuantity result;
            if (!TryParse(physicalQuantityStr, styles, provider, out result))
            {
                throw new ArgumentException("Not a valid physical quantity format", nameof(physicalQuantityStr));
            }

            return result;
        }

        public IPhysicalQuantity Zero => new PhysicalQuantity(0, this.Unit.Dimensionless);
        public IPhysicalQuantity One => new PhysicalQuantity(1, this.Unit.Dimensionless);

        public override Int32 GetHashCode() => this.Value.GetHashCode() + this.Unit.GetHashCode();

        public IPhysicalQuantity ConvertToSystemUnit()
        {
            if (this.Unit.SimpleSystem != null)
            {
                return this;
            }

           Double d = this.Value;
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


        // Auto detecting if specific or relative unit conversion 

        public IPhysicalQuantity this[IPhysicalUnit convertToUnit] => this.ConvertTo(convertToUnit);

        public IPhysicalQuantity this[IPhysicalQuantity convertToUnit] => this.ConvertTo(convertToUnit);

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertToUnit)
        {
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
                throw new ArgumentNullException(nameof(convertToUnit));
            }

            if (this.Unit.Kind != UnitKind.CombinedUnit && convertToUnit.Kind != UnitKind.CombinedUnit)
            {
                Boolean thisIsDimensionless = this.Unit.IsDimensionless;
                Boolean toIsDimensionless = convertToUnit.IsDimensionless;
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

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity convertToUnit) => this.ConvertTo(convertToUnit.Unit);

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
                throw new ArgumentNullException(nameof(convertToUnitSystem));
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
                if (physicalQuantity.Unit.Kind == UnitKind.PrefixedUnit)
                {
                    PrefixedUnit prefixUnit = physicalQuantity.Unit as PrefixedUnit;
                    if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent((SByte)(prefixExponent + prefixUnit.Prefix.Exponent)), out unitPrefix))
                    {
                        return new PrefixedUnit(unitPrefix, prefixUnit.Unit);
                    }
                }
                    
                if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent(prefixExponent), out unitPrefix))
                {
                    INamedSymbolUnit namedSymbolUnit = physicalQuantity.Unit as INamedSymbolUnit;
                    if (namedSymbolUnit != null)
                    {
                        return new PrefixedUnit(unitPrefix, namedSymbolUnit);
                    }

                    if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit combinedUnit = physicalQuantity.Unit as ICombinedUnit;
                        return new CombinedUnit(unitPrefix, combinedUnit);
                    }
                }
            }

            throw new ArgumentException("Physical quantity is not a pure unit; but has a value = " + physicalQuantity.Value.ToString());

            //return null;
        }

        public static IPhysicalUnit operator !(PhysicalQuantity physicalQuantity) => PureUnit(physicalQuantity);

        public static implicit operator PhysicalUnit(PhysicalQuantity physicalQuantity) => PureUnit(physicalQuantity) as PhysicalUnit;

        public Boolean Equivalent(IPhysicalQuantity other, out Double quotient)
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

        public Boolean Equals(IPhysicalQuantity other)
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


        public Boolean Equals(IPhysicalUnit other)
        {
            IPhysicalQuantity otherPhysicalQuantity = new PhysicalQuantity(1, other);
            return this.Equals(otherPhysicalQuantity);
        }

        public Boolean Equals(double other)
        {
            IPhysicalQuantity otherPhysicalQuantity = new PhysicalQuantity(other);
            return this.Equals(otherPhysicalQuantity);
        }

        public override Boolean Equals(Object obj)
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

        public static Boolean operator ==(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.Equals(pq2);
        }

        public static Boolean operator !=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return !pq1.Equals(pq2);
        }

        public static Boolean operator <(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) < 0;
        }

        public static Boolean operator <=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) <= 0;
        }

        public static Boolean operator >(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) > 0;
        }

        public static Boolean operator >=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) >= 0;
        }

        #region Physical Quantity static operator methods

        protected delegate Double CombineValuesFunc(Double v1, Double v2);
        protected delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);

        protected static PhysicalQuantity CombineValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf)
        {
            if (pq1.Unit != pq2.Unit)
            {
                IPhysicalQuantity temp_pq2 = pq2.ConvertTo(pq1.Unit);
                if (temp_pq2 == null)
                {
                    throw new ArgumentException("object's physical unit " + pq2.Unit.ToPrintString() + " is not convertible to unit " + pq1.Unit.ToPrintString());
                }

                pq2 = temp_pq2;
            }
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pq1.Unit);
        }

        protected static PhysicalQuantity CombineUnitsAndValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf, CombineExponentsFunc cef)
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

            SByte minNoOfBaseUnits = (SByte)Math.Min(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            SByte maxNoOfBaseUnits = (SByte)Math.Max(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            Debug.Assert(maxNoOfBaseUnits <= Physics.NoOfBaseQuanties);

            SByte[] someexponents = new SByte[Physics.NoOfBaseQuanties];

            for (int i = 0; i < minNoOfBaseUnits; i++)
            {
                someexponents[i] = cef(pq1.Unit.Exponents[i], pq2.Unit.Exponents[i]);
            }

            for (int i = minNoOfBaseUnits; i < maxNoOfBaseUnits; i++)
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

        public static PhysicalQuantity operator +(PhysicalQuantity pq1, IPhysicalQuantity pq2) => new PhysicalQuantity(pq1.Add(pq2));

        public static PhysicalQuantity operator -(PhysicalQuantity pq1, IPhysicalQuantity pq2) => new PhysicalQuantity(pq1.Subtract(pq2));

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

        public static PhysicalQuantity operator *(PhysicalQuantity pq, IUnitPrefix up) => new PhysicalQuantity(pq.Value * up.Value, pq.Unit);

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalQuantity pq) => new PhysicalQuantity(pq.Value * up.Value, pq.Unit);

        public static PhysicalQuantity operator *(PhysicalQuantity pq, Double d) => new PhysicalQuantity(pq.Value * d, pq.Unit);

        public static PhysicalQuantity operator /(PhysicalQuantity pq, Double d) => new PhysicalQuantity(pq.Value / d, pq.Unit);

        public static PhysicalQuantity operator *(Double d, PhysicalQuantity pq) => new PhysicalQuantity(pq.Multiply(d));

        public static PhysicalQuantity operator /(Double d, PhysicalQuantity pq) => new PhysicalQuantity(new PhysicalQuantity(d).Divide(pq));

        public static PhysicalQuantity operator *(PhysicalQuantity pq, IPhysicalUnit pu) => new PhysicalQuantity(pq.Value, pq.Unit.Multiply(pu));

        public static PhysicalQuantity operator /(PhysicalQuantity pq, IPhysicalUnit pu) => new PhysicalQuantity(pq.Value, pq.Unit.Divide(pu));

        public static PhysicalQuantity operator *(IPhysicalUnit pu, PhysicalQuantity pq) => new PhysicalQuantity(pq.Value, pu.Multiply(pq.Unit));

        public static PhysicalQuantity operator /(IPhysicalUnit pu, PhysicalQuantity pq) => new PhysicalQuantity(pq.Value, pu.Divide(pq.Unit));

        public static PhysicalQuantity operator ^(PhysicalQuantity pq, SByte exponent) => pq.Power(exponent);

        public static PhysicalQuantity operator %(PhysicalQuantity pq, SByte exponent) => pq.Root(exponent);

        public static PhysicalQuantity operator |(PhysicalQuantity pq, SByte exponent) => pq.Root(exponent);

        #endregion Physical Quantity static operator methods

        public PhysicalQuantity Power(SByte exponent)
        {
            IPhysicalUnit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            IPhysicalUnit pu_pow = pu.Pow(exponent);
           Double value = System.Math.Pow(this.Value, exponent);
            return new PhysicalQuantity(value, pu_pow);
        }

        public PhysicalQuantity Root(SByte exponent)
        {
            IPhysicalUnit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            IPhysicalUnit pu_rot = pu.Rot(exponent);
           Double value = System.Math.Pow(this.Value, 1.0 / exponent);
            return new PhysicalQuantity(value, pu_rot);
        }

        #region Physical Quantity IPhysicalUnitMath implementation

        public IPhysicalQuantity Add(IPhysicalQuantity physicalQuantity) => CombineValues(this, physicalQuantity, (Double v1, Double v2) => v1 + v2);

        public IPhysicalQuantity Subtract(IPhysicalQuantity physicalQuantity) => CombineValues(this, physicalQuantity, (Double v1, Double v2) => v1 - v2);


        public IPhysicalQuantity Multiply(INamedSymbolUnit physicalUnit) => this.Multiply(new PrefixedUnitExponent(null, physicalUnit, 1));

        public IPhysicalQuantity Divide(INamedSymbolUnit physicalUnit) => this.Divide(new PrefixedUnitExponent(null, physicalUnit, 1));

        public IPhysicalQuantity Multiply(IPhysicalUnit physicalUnit) => this.Unit.Multiply(physicalUnit).Multiply(this.Value);

        public IPhysicalQuantity Divide(IPhysicalUnit physicalUnit) => this.Unit.Divide(physicalUnit).Multiply(this.Value);

        public IPhysicalQuantity Multiply(IPhysicalQuantity physicalQuantity) => this.Unit.Multiply(this.Value, physicalQuantity);

        public IPhysicalQuantity Divide(IPhysicalQuantity physicalQuantity) => this.Unit.Divide(this.Value, physicalQuantity);

        public IPhysicalQuantity Multiply(Double quantity) => new PhysicalQuantity(this.Value * quantity, this.Unit);

        public IPhysicalQuantity Divide(Double quantity) => new PhysicalQuantity(this.Value / quantity, this.Unit);

        public IPhysicalQuantity Pow(SByte exponent) => this.Power(exponent);

        public IPhysicalQuantity Rot(SByte exponent) => this.Root(exponent);

        public IPhysicalQuantity Multiply(IPrefixedUnit prefixedUnit)
        {
            IPhysicalUnit pu = this.Unit.Multiply(prefixedUnit);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Divide(IPrefixedUnit prefixedUnit)
        {
            IPhysicalUnit pu = this.Unit.Divide(prefixedUnit);
            return new PhysicalQuantity(this.Value, pu);
        }

        public IPhysicalQuantity Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            IPhysicalUnit pu = this.Unit.Multiply(prefixedUnitExponent);
            return new PhysicalQuantity(Value, pu);
        }

        public IPhysicalQuantity Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
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

        public IPhysicalQuantity Multiply(Double quantity, IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(this.Value * quantity, physicalQuantity);
            return pq;
        }

        public IPhysicalQuantity Divide(Double quantity, IPhysicalQuantity physicalQuantity)
        {
            IPhysicalQuantity pq = this.Unit.Divide(quantity, physicalQuantity).Multiply(this.Value);
            return pq;
        }

        #endregion Physical Quantity IPhysicalUnitMath implementation        
    }

    #endregion Physical Quantity Classes


    public class UnitSystemStack
    {
        protected Stack<IUnitSystem> default_UnitSystem_Stack = new Stack<IUnitSystem>();

        public IUnitSystem Default
        {
            get
            {
                if (default_UnitSystem_Stack == null || default_UnitSystem_Stack.Count <= 0)
                {
                    return Physics.SI_Units;
                }
                else
                {
                    return default_UnitSystem_Stack.Peek();
                }
            }
        }

        public Boolean Use(IUnitSystem newUnitSystem)
        {
            if (Default != newUnitSystem)
            {
                default_UnitSystem_Stack.Push(newUnitSystem);
                return true;
            }
            return false;
        }

        public Boolean Unuse(IUnitSystem oldUnitSystem)
        {
            if (default_UnitSystem_Stack != null && default_UnitSystem_Stack.Count > 0 && default_UnitSystem_Stack.Peek() == oldUnitSystem)
            {
                default_UnitSystem_Stack.Pop();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            default_UnitSystem_Stack.Clear();
        }
    }

    public class UnitLookup //(UnitSystem[] unitSystems)
    {
        protected UnitSystem[] unitSystems;

        public UnitLookup(UnitSystem[] someUnitSystems)
        {
            unitSystems = someUnitSystems;
        }

        public IPhysicalUnit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in unitSystems)
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
            foreach (UnitSystem us in unitSystems)
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
            foreach (UnitSystem us in unitSystems)
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
            IUnitSystem result_us = unitSystems.FirstOrNull<IUnitSystem>(us => us.Name == UnitSystemsymbolstr);
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

            IList<IUnitSystem> oldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> oldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            return GetIntermediateUnitSystemConversion(UnitSystemConversions, oldUnitsystems1, newUnitSystemsConvertableToUnitsystems1, oldUnitsystems2, newUnitSystemsConvertableToUnitsystems2);
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

                IList<IUnitSystem> intersectUnitsystemsList = unitSystemsConvertableToUnitsystems1.Intersect(unitSystemsConvertableToUnitsystems2).ToList();

                if (intersectUnitsystemsList.Count > 0)
                {
                    IUnitSystem intersectUnitsystem = intersectUnitsystemsList[0];
                    subIntermediereUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, newUnitSystemsConvertableToUnitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { intersectUnitsystem });
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
                        // Find the first and second UnitSystemConversions which will be combined into a two step conversion

                        IUnitSystem combinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem combinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem combinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion secondUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean secondValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem);
                        if (!secondValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            combinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            combinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion firstUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, unitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { combinedUnitSystemConversionIntermedierUnitSystem });
                        Boolean firstValueConversionDirectionInverted = firstUnitSystemConversion.BaseUnitSystem == combinedUnitSystemConversionIntermedierUnitSystem;

                        if (!firstValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionBaseUnitSystem = firstUnitSystemConversion.BaseUnitSystem;
                        }
                        else
                        {
                            combinedUnitSystemConversionBaseUnitSystem = firstUnitSystemConversion.ConvertedUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[0], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[0], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[1], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[1], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[2], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[2], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[3], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[3], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[4], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[4], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[5], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[5], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[6], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[6], secondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(combinedUnitSystemConversionBaseUnitSystem, combinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }

                    if (!unitsystems2.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from one of subIntermediereUnitSystemConversion's systems to some unit system in unitsystems2
                        // Find Post UnitSystemConversion

                        IUnitSystem combinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem combinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem combinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion firstUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean firstValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem);
                        if (!firstValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem) || unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));

                            combinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion secondUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems2Conversions, new List<IUnitSystem>() { }, new List<IUnitSystem>() { combinedUnitSystemConversionIntermedierUnitSystem }, new List<IUnitSystem>() { }, unitsystems2);
                        Boolean secondValueConversionDirectionInverted = secondUnitSystemConversion.ConvertedUnitSystem == combinedUnitSystemConversionIntermedierUnitSystem;

                        if (!secondValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionConvertedUnitSystem = secondUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            combinedUnitSystemConversionConvertedUnitSystem = secondUnitSystemConversion.BaseUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] combinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[0], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[0], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[1], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[1], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[2], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[2], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[3], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[3], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[4], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[4], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[5], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[5], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[6], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[6], secondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(combinedUnitSystemConversionBaseUnitSystem, combinedUnitSystemConversionConvertedUnitSystem, combinedValueConversions);
                    }
                    return subIntermediereUnitSystemConversion;
                }
            }

            return null;
        }
    }

    #endregion Physical Measure Classes
}


