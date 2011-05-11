/*   http://physicalmeasure.codeplex.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace PhysicalMeasure
{
    #region Physical Measure Exceptions

    public class PhysicalUnitFormatException : FormatException
    {
        public PhysicalUnitFormatException()
            : this("The string argument is not in a valid physical unit format.")
        {
        }

        public PhysicalUnitFormatException(String message)
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
        Electric_current,
        Thermodynamic_temperature,
        Amount_of_substance,
        Luminous_intensity
    }

    public enum UnitKind { ukBaseUnit, ukDerivedUnit, ukConvertibleUnit, ukCombinedUnit };

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

        String ToPrintString();
    }

    public interface ISystemUnit : ISystemItem, IUnit
    {
    }

    public interface IPrefixedUnit
    {
        SByte PrefixExponent { get; set; }
        IPhysicalUnit Unit { get; set; }
    }

    public interface IPrefixedUnitExponent : IPrefixedUnit
    {
        SByte Exponent { get; set; }

        IPhysicalQuantity PhysicalQuantity();
        IPhysicalQuantity PhysicalQuantity(ref double d, IUnitSystem ISystem);
    }

    public interface IPhysicalItemMath : IEquatable<IPhysicalUnit>
    {
        IPhysicalUnit Dimensionless { get; }

        IPhysicalQuantity Multiply(IPhysicalUnit u2);
        IPhysicalQuantity Divide(IPhysicalUnit u2);

        IPhysicalQuantity Pow(SByte exponent);
        IPhysicalQuantity Rot(SByte exponent);

        IPhysicalQuantity Multiply(IPrefixedUnitExponent pue);
        IPhysicalQuantity Divide(IPrefixedUnitExponent pue);

        IPhysicalQuantity Multiply(IPhysicalQuantity pq2);
        IPhysicalQuantity Divide(IPhysicalQuantity pq2);

    }

    public interface IPhysicalUnitMath : IPhysicalItemMath
    {
        IPhysicalQuantity Multiply(double d, IPhysicalQuantity pq2);
        IPhysicalQuantity Divide(double d, IPhysicalQuantity pq2);
    }

    public interface IPhysicalUnitCombine 
    {
        IPhysicalUnit CombineMultiply(IPhysicalUnit u2);
        IPhysicalUnit CombineDivide(IPhysicalUnit u2);

        IPhysicalUnit CombinePow(SByte exponent);
        IPhysicalUnit CombineRot(SByte exponent);

        IPhysicalUnit CombineMultiply(IPrefixedUnitExponent pue);
        IPhysicalUnit CombineDivide(IPrefixedUnitExponent pue);
    }

    public interface IPhysicalQuantityConvertable
    {
        IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem);

        IPhysicalQuantity ConvertToSystemUnit();
    }

    public interface IPhysicalUnitConvertable : IPhysicalQuantityConvertable
    {
        IPhysicalQuantity ConvertTo(ref double d, IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(ref double d, IUnitSystem converttounitsystem);

        IPhysicalQuantity ConvertToSystemUnit(ref double d);
    }

    public interface IPhysicalUnit : ISystemUnit, IPhysicalUnitMath, IPhysicalUnitCombine, IPhysicalUnitConvertable /*  : <BaseUnit | DerivedUnit | ConvertibleUnit>  */
    {
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
        IPhysicalUnit BaseUnit { get; }
        IValueConversion Conversion { get; }

        IPhysicalQuantity ConvertToBaseUnit();
        IPhysicalQuantity ConvertFromBaseUnit();

        //IPhysicalQuantity ConvertToBaseUnit(IPhysicalQuantity pq);
        //IPhysicalQuantity ConvertFromBaseUnit(IPhysicalQuantity pq);

        IPhysicalQuantity ConvertToBaseUnit(double d);
        IPhysicalQuantity ConvertFromBaseUnit(double d);
    }

    public interface IValueConversion
    {
        double Convert(double value, bool backwards = false);
        double ConvertFromBaseUnit(double value);
        double ConvertToBaseUnit(double value);
    }

    //public interface ICombinedPhysicalUnit : IPhysicalUnitMath, IPhysicalUnitConvertable
    public interface ICombinedUnit : IPhysicalUnit
    {
        PrefixedUnitExponentList Numerators { get; /* set; */ }
        PrefixedUnitExponentList Denominators { get; /* set; */ }

        IUnitSystem SomeSystem { get; /* set; */ }
    }

    public interface IPhysicalQuantityMath : IComparable, IEquatable<IPhysicalQuantity>, IPhysicalItemMath
    {
        IPhysicalQuantity Zero { get; }
        IPhysicalQuantity One { get; }

        IPhysicalQuantity Add(IPhysicalQuantity pq2);
        IPhysicalQuantity Subtract(IPhysicalQuantity pq2);
    }

    public interface IPhysicalQuantity : IFormattable, IPhysicalQuantityMath, IPhysicalQuantityConvertable
    {
        double Value { get; set; }
        IPhysicalUnit Unit { get; set; }
    }

    public interface IUnitSystem : INamed
    {
        IUnitPrefixTable UnitPrefixes { get; }
        IBaseUnit[] BaseUnits { get; }
        INamedDerivedUnit[] NamedDerivedUnits { get; }
        IConvertibleUnit[] ConvertibleUnits { get; }

        INamedSymbolUnit UnitFromName(String unitname);
        INamedSymbolUnit UnitFromSymbol(String symbolname);
        /**
        IPhysicalQuantity ScaledUnitFromSymbol(String scaledunitname);
        IPhysicalQuantity ScaledUnitFromSymbol(IPhysicalQuantity one, String scaledunitname);
        **/
        IPhysicalUnit ScaledUnitFromSymbol(String scaledunitname);

        IPhysicalQuantity ConvertTo(IPhysicalUnit convertfromunit, IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IUnitSystem converttounitsystem);
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

        bool GetExponentFromPrefixChar(char someprefixchar, out SByte exponent);
        bool GetPrefixCharFromExponent(SByte someexponent, out char prefixchar);
    }

    #endregion Physical Measure Interfaces

    #region Dimension Exponets Classes

    public static class DimensionExponets 
    {
        // SByte[Physic.NoOfMeasures] SomeExponents;
        static public bool Equals(SByte[] exponents1, SByte[] exponents2)
        {
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
            do
            {
                equal = exponents1[i] == exponents2[i];
                i++;
            } while (i < Physics.NoOfMeasures && equal);

            return equal;
        }

        static public bool IsDimensionless(SByte[] exponents)
        {
            bool isdimensionless = true;
            SByte i = 0;
            do
            {
                isdimensionless = exponents[i] == 0;
                i++;
            } while (i < Physics.NoOfMeasures && isdimensionless);

            return isdimensionless;
        }

        public static bool IsSameMeasureKind(IEnumerable<SByte> exponents1, IEnumerable<SByte> exponents2)
        {
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

    }

    #endregion Dimension Exponets Classes

    #region Physical Unit prefix Classes

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private char _prefixchar;
        private SByte _prefixexponent;

        public char PrefixChar { get { return _prefixchar; } }
        public SByte PrefixExponent { get { return _prefixexponent; } }
        public Double PrefixValue { get { return Math.Pow(10, _prefixexponent); } } 

        public UnitPrefix(String somename, char someprefixchar, SByte someprefixexponent)
            : base(somename)
        {
            this._prefixchar = someprefixchar;
            this._prefixexponent = someprefixexponent;
        }
    }

    public class UnitPrefixTable : IUnitPrefixTable
    {
        private UnitPrefix[] _unitprefixes;

        public IUnitPrefix[] UnitPrefixes { get { return _unitprefixes; } }

        public UnitPrefixTable(UnitPrefix[] anunitprefixes)
        {
            this._unitprefixes = anunitprefixes;
        }

        public bool GetExponentFromPrefixChar(char someprefixchar, out SByte exponent) 
        {
            exponent = 0;
            // exponent = (from us in UnitPrefixes where us.PrefixChar == someprefixchar select us.PrefixExponent).Single();  
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.PrefixChar == someprefixchar)
                {
                    exponent = us.PrefixExponent;
                    return true;
                }
            }
            return false;
        }

        public bool GetPrefixCharFromExponent(SByte someexponent, out char prefixchar)
        {
            prefixchar = '\0';
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.PrefixExponent == someexponent)
                {
                    prefixchar = us.PrefixChar;
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
        public abstract double Convert(double value, bool backwards = false);
        public abstract double ConvertFromBaseUnit(double value);
        public abstract double ConvertToBaseUnit(double value);
    }

    public class LinearyValueConversion : ValueConversion
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

        public LinearyValueConversion(double someoffset, double somescale)
        {
            this.Offset = someoffset;
            this.Scale = somescale;
        }

        public override double Convert(double value, bool backwards)
        {
            if (backwards)
            {
                return ConvertToBaseUnit(value);
            }
            else
            {
                return ConvertFromBaseUnit(value);
            }
        }

        public override double ConvertFromBaseUnit(double value)
        {
            return value * this.Scale + this.Offset;
        }

        public override double ConvertToBaseUnit(double value)
        {
            return (value - this.Offset) / this.Scale;
        }
    }

    public class ScaledValueConversion : LinearyValueConversion
    {
        public ScaledValueConversion(double somescale)
            : base(0, somescale)
        {
            Debug.Assert(somescale != 0);
            Debug.Assert(!double.IsInfinity(somescale));

            if (somescale == 0)
            {
                throw new ArgumentException("0 is not a valid scale", "somescale");
            }
            if (double.IsInfinity(somescale))
            {
                throw new ArgumentException("Infinity is not a valid scale", "somescale");
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

    #endregion Value Conversion Classes
    
    #region Physical Unit Classes

    public class NamedObject : INamed
    {
        private String _name;
        public String Name { get { return _name; } set { _name = value; } }

        public NamedObject(String aname)
        {
            this.Name = aname;
        }
    }

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        private String _symbol;
        public String Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbol(String aname, String asymbol)
            : base(aname)
        {
            this.Symbol = asymbol;
        }
    }

    /***

    public class SystemObject : ISystemItem
    {
        private IUnitSystem _system;
        public IUnitSystem System { get { return _system; } set { _system = value; } }

        public SystemObject(IUnitSystem asystem = null)
        {
            this.System = asystem;
        }
    }

    public class NamedSystemObject : SystemObject
    {
        private String _name;
        public String Name { get { return _name; } set { _name = value; } }

        public NamedSystemObject(IUnitSystem somesystem, String somename)
            : base(somesystem)
        {
            this.Name = somename;
        }

        public NamedSystemObject(String aname)
            : this(null, aname)
        {
        }
    }

    public class NamedSymbolSystemObject : NamedSystemObject, INamedSymbol
    {
        private String _symbol;
        public String Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbolSystemObject(IUnitSystem somesystem, String somename, String somesymbol)
            : base(somesystem, somename)
        {
            this.Symbol = somesymbol;
        }

        public NamedSymbolSystemObject(String somename, String somesymbol)
            : this(null, somename, somesymbol)
        {
        }
    }
    ***/

    //public abstract class PhysicalUnit : SystemObject, IPhysicalUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    public abstract class PhysicalUnit : ISystemItem, IPhysicalUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit> */
    {
        /**
        public PhysicalUnit(IUnitSystem somesystem)
            : base(somesystem)
        {
        }
        **/

        public PhysicalUnit()
        {
        }

        public abstract IUnitSystem System { get; set; }


        public abstract UnitKind Kind { get; }
        public abstract SByte[] Exponents { get; }


        public virtual IPhysicalUnit Dimensionless { get { return Physics.dimensionless; } }

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
        /// [prefix] [unitsymbol] 
        /// </summary>
        public static IPhysicalUnit Parse(String s)
        {
            IPhysicalUnit pu = null;
            //pq = Physics.ScaledUnitFromSymbol(UnitStr);
            pu = ParseUnit(ref s);
            return pu;
        }

        public static IPhysicalUnit ParseUnit(ref String s)
        {
            IPhysicalUnit pu = null;
            return Parse(pu, ref s);
        }

        public static IPhysicalUnit Parse(IPhysicalUnit dimensionless, ref String s)
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

        public static IPhysicalUnit ParseOptionalUnit(IPhysicalUnit pu, ref String s)
        {
            Debug.Assert(pu != null);

            if (pu == null)
            {
                throw new ArgumentNullException("pu");
            }

            IPhysicalUnit puRes = pu;
            if (!String.IsNullOrEmpty(s))
            {
                if (   (s[0] == '*')
                    || (s[0] == '·')) // centre dot
                {
                    s = s.Substring(1);
                    IPhysicalUnit pu2 = Parse(pu.Dimensionless, ref s);
                    if (pu2 != null)
                    {   // Combine pu and pu2 to the new unit pu*pu2
                        puRes = pu.CombineMultiply(pu2);
                    }
                    else
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing operand after '*'");
                    }
                }
                else if (s[0] == '/')
                {
                    s = s.Substring(1);
                    IPhysicalUnit pu2 = Parse(pu.Dimensionless, ref s);
                    if (pu2 != null)
                    {   // Combine pu and pu2 to the new unit pu/pu2
                        puRes = pu.CombineDivide(pu2);
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
                        IPhysicalUnit pu2 = Parse(pu.Dimensionless, ref TempStr);
                        if (pu2 != null)
                        {   // Combine pu and pu2 to the new unit pu*pu2
                            puRes = pu.CombineMultiply(pu2);
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
                    pu = Parse(dimensionless, ref s);
                    if (s[0] == ')')
                    {
                        s = s.Substring(1); // Skip end parantes ')'
                    }
                    else
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Missing end parantes ')'");
                    }
                }
                else
                {
                    int i = s.IndexOfAny(new char[] { ' ', '*', '·', '/', '+', '-', '(', ')' });
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


        public static IPhysicalUnit ParseOptionalExponent(IPhysicalUnit pu, ref String s)
        {
            Debug.Assert(pu != null);
            if (pu == null)
            {
                throw new ArgumentNullException("pu");
            }

            IPhysicalUnit puRes = pu;
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
                    puRes = pu.CombinePow(exponent);
                    s = s.Substring(numlen);
                }
            }
            return puRes;
        }

        #endregion IPhysicalUnit unit expression parser methods

        #endregion Unit Expression parser methods

        /// <summary>
        /// 
        /// </summary>
        public abstract String UnitString();

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            String UnitName = UnitString();
            IUnitSystem system = this.System;
            if (   (UnitName != String.Empty) 
                && (system != null)
                && (system != Physics.Default_UnitSystem))
            {
                UnitName = this.System.Name + "." + UnitName;
            }

            return UnitName;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual String ToPrintString()
        {
            String UnitName = UnitString();
            if (UnitName == String.Empty)
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

        #region Unit convertion methods

        public virtual IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            double d = 1;
            return ConvertTo(ref d, converttounit);
        }

        public virtual IPhysicalQuantity ConvertTo(ref double d, IPhysicalUnit converttounit)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(d, this);
            if (converttounit != this)
            {
                pq = this.System.ConvertTo(pq, converttounit);
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem)
        {
            double d = 1;
            return this.ConvertTo(ref d, converttounitsystem);
        }

        public virtual IPhysicalQuantity ConvertTo(ref double d, IUnitSystem converttounitsystem)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(d, this);
            // Mark d as used now
            d = 1;
            if (converttounitsystem != this.System)
            {
                pq = this.System.ConvertTo(pq, converttounitsystem);
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertToSystemUnit()
        {
            double d = 1;
            return this.ConvertToSystemUnit(ref d);
        }

        public abstract IPhysicalQuantity ConvertToSystemUnit(ref double d);

        public virtual bool Equals(IPhysicalUnit other)
        {
            if (this.System != other.System)
            {   // Must be same unit system
                return new PhysicalQuantity(1, this) == other.ConvertTo(this.System);
            } 
            if (   (this.Kind != UnitKind.ukConvertibleUnit)
                && (other.Kind != UnitKind.ukConvertibleUnit))
            {
                //return (this.Exponents == other.Exponents));
                return DimensionExponets.Equals(this.Exponents, other.Exponents);
            }

            IPhysicalQuantity pq1 = this.ConvertToSystemUnit();
            IPhysicalQuantity pq2 = other.ConvertToSystemUnit();

            return pq1 == pq2;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            if (!(obj is IPhysicalUnit))
                throw new InvalidCastException("The 'obj' argument is not a IPhysicalUnit object.");
            else
                return Equals(obj as IPhysicalUnit);
        }

        public static bool operator ==(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            return unit1.Equals(unit2);
        }

        public static bool operator !=(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            return (!unit1.Equals(unit2));
        }

        #endregion Unit convertion methods

        #region Unit static operator methods

        public delegate SByte CombineExponentsFunc(SByte e1, SByte e2);
        public delegate Double CombineQuantitiesFunc(Double q1, Double q2);

        public static PhysicalQuantity CombineUnits(IPhysicalUnit u1, IPhysicalUnit u2, CombineExponentsFunc cef, CombineQuantitiesFunc cqf)
        {

            IPhysicalQuantity u1_pq = u1.ConvertToSystemUnit();
            IPhysicalQuantity u2_pq = u2.ConvertToSystemUnit();
            if (u2_pq.Unit.System != u1_pq.Unit.System)
            {
                u2_pq = u1_pq.Unit.System.ConvertTo(u2_pq, u1_pq.Unit.System);
                u2_pq = u2_pq.ConvertToSystemUnit();
            }
            SByte[] someexponents = new SByte[Physics.NoOfMeasures];

            for (int i = 0; i < Physics.NoOfMeasures; i++)
            {
                someexponents[i] = cef(u1_pq.Unit.Exponents[i], u2_pq.Unit.Exponents[i]);
            }
            PhysicalUnit pu = new DerivedUnit(u1.System, someexponents);
            return new PhysicalQuantity(cqf(u1_pq.Value, u2_pq.Value), pu);
        }

        public static PhysicalQuantity CombineUnitExponents(IPhysicalUnit u, SByte exponent, CombineExponentsFunc cef)
        {
            SByte[] someexponents = new SByte[Physics.NoOfMeasures];

            for (int i = 0; i < Physics.NoOfMeasures; i++)
            {
                someexponents[i] = cef(u.Exponents[i], exponent);
            }
            PhysicalUnit pu = new DerivedUnit(u.System, someexponents);
            return new PhysicalQuantity(1, pu);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, IUnitPrefix up)
        {
            return new PhysicalQuantity(up.PrefixValue, u);
        }

        public static PhysicalQuantity operator *(IUnitPrefix up, PhysicalUnit u)
        {
            return new PhysicalQuantity(up.PrefixValue, u);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u, double d)
        {
            return new PhysicalQuantity(d, u);
        }

        public static PhysicalQuantity operator /(PhysicalUnit u, double d)
        {
            return new PhysicalQuantity(1/d, u);
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
            return new PhysicalQuantity(u1.Multiply(u2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPhysicalUnit u2)
        {
            return new PhysicalQuantity(u1.Divide(u2));
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            return new PhysicalQuantity(u1.Multiply(pue2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPrefixedUnitExponent pue2)
        {
            return new PhysicalQuantity(u1.Divide(pue2));
        }

        public static PhysicalQuantity operator ^(PhysicalUnit u, SByte exponent)
        {
            return new PhysicalQuantity(u.Pow(exponent));
        }

        public static PhysicalQuantity operator %(PhysicalUnit u, SByte exponent)
        {
            return new PhysicalQuantity(u.Rot(exponent));
        }

        #endregion Unit static operator methods


        public virtual PhysicalQuantity Power(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 * e2));
        }

        public virtual PhysicalQuantity Root(SByte exponent)
        {
            return CombineUnitExponents(this, exponent, (SByte e1, SByte e2) => (SByte)(e1 / e2));
        }

        #region Unit math methods

        public IPhysicalQuantity Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        public IPhysicalQuantity Rot(SByte exponent)
        {
            return this.Root(exponent);
        }

        public virtual IPhysicalQuantity Multiply(IPhysicalUnit u2)
        {
            return CombineUnits(this, u2, (SByte e1, SByte e2) => (SByte)(e1 + e2), (Double e1, Double e2) => (Double)(e1 * e2));
        }

        public virtual IPhysicalQuantity Divide(IPhysicalUnit u2)
        {
            return CombineUnits(this, u2, (SByte e1, SByte e2) => (SByte)(e1 - e2), (Double e1, Double e2) => (Double)(e1 / e2));
        }

        public virtual IPhysicalQuantity Multiply(double d, IPhysicalQuantity pq)
        {
            IPhysicalQuantity pq2 = this.Multiply(pq.Unit);
            pq2.Value = pq2.Value * d * pq.Value;
            return pq2;
        }

        public virtual IPhysicalQuantity Divide(double d, IPhysicalQuantity pq)
        {
            IPhysicalQuantity pq2 = this.Divide(pq.Unit);
            pq2.Value = pq2.Value * d / pq.Value;
            return pq2;
        }

        public virtual IPhysicalQuantity Multiply(IPhysicalQuantity pq)
        {
            IPhysicalQuantity pq2 = this.Multiply(pq.Unit);
            pq2.Value = pq2.Value * pq.Value;
            return pq2;
        }

        public virtual IPhysicalQuantity Divide(IPhysicalQuantity pq)
        {
            IPhysicalQuantity pq2 = this.Divide(pq.Unit);
            pq2.Value = pq2.Value / pq.Value;
            return pq2;
        }

        public virtual IPhysicalQuantity Multiply(IPrefixedUnitExponent pue)
        {
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, pue.PrefixExponent), pue.Unit);
            pq2 = pq2.Pow(pue.Exponent);
            return this.Multiply(pq2);
        }

        public virtual IPhysicalQuantity Divide(IPrefixedUnitExponent pue)
        {
            IPhysicalQuantity pq2 = new PhysicalQuantity(Math.Pow(10, pue.PrefixExponent), pue.Unit);
            return this.Divide(pq2.Pow(pue.Exponent));
        }

        public virtual IPhysicalQuantity Multiply(double d)
        {
            return this.Multiply(d);
        }

        public virtual IPhysicalQuantity Divide(double d)
        {
            return this.Divide(d);
        }

        #endregion Unit math methods

        #region Unit Combine math methods

        public virtual IPhysicalUnit CombineMultiply(IPhysicalUnit u2)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(u2);
            return uRes;
        }

        public virtual IPhysicalUnit CombineDivide(IPhysicalUnit u2)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(u2);
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

        public virtual IPhysicalUnit CombineMultiply(IPrefixedUnitExponent pue)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(pue);
            return uRes;
        }

        public virtual IPhysicalUnit CombineDivide(IPrefixedUnitExponent pue)
        {
            IPhysicalUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(pue);
            return uRes;
        }

        #endregion Unit Combine math methods
    }

    public abstract class SystemUnit : PhysicalUnit, ISystemUnit/* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        private IUnitSystem _system;

        // property override get
        public override IUnitSystem System { get { return _system; } set { _system = value; } }

        public SystemUnit(IUnitSystem asystem = null)
        {
            this._system = asystem;
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double d)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(d, this);
            return pq;
        }

        /// <summary>
        /// 
        /// </summary>
        public override String UnitString()
        {
            String ExponentsStr = "";
            int index = 0;
            foreach (SByte exponent in Exponents)
            {
                if (exponent != 0)
                {
                    if (!String.IsNullOrEmpty(ExponentsStr))
                    {
                        ExponentsStr += '·'; // centre dot
                    }
                    ExponentsStr += this.System.BaseUnits[index].Symbol;
                    if (exponent != 1)
                    {
                        ExponentsStr += exponent.ToString();
                    }
                }

                index++;
            }

            return ExponentsStr;
        }

    }

    public class BaseUnit : SystemUnit, INamedSymbol, IBaseUnit
    {
        private NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        private SByte _baseunitnumber;
        public SByte BaseUnitNumber { get { return _baseunitnumber; } }

        public override UnitKind Kind { get { return UnitKind.ukBaseUnit; } }

        public override SByte[] Exponents { get { SByte[] tempexponents = new SByte[System.BaseUnits.Length]; tempexponents[_baseunitnumber] = 1; return tempexponents; } } 

        public BaseUnit(IUnitSystem someunitsystem, SByte somebaseunitnumber, NamedSymbol somenamedsymbol)
            : base(someunitsystem)
        {
            this._baseunitnumber = somebaseunitnumber;
            this.NamedSymbol = somenamedsymbol;
        }

        public BaseUnit(IUnitSystem someunitsystem, SByte somebaseunitnumber, String somename, String somesymbol)
            : this(someunitsystem, somebaseunitnumber, new NamedSymbol(somename, somesymbol))
        {
        }

        public BaseUnit(SByte somebaseunitnumber, String somename, String somesymbol)
            : this(null, somebaseunitnumber, somename, somesymbol)
        {
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            String ExponentsStr = this.Symbol;

            String UnitName = ExponentsStr;
            if (this.System != Physics.Default_UnitSystem)
            {
                UnitName = this.System.Name + "." + ExponentsStr;
            }

            return UnitName;
        }
    }

    public class DerivedUnit : SystemUnit, IDerivedUnit
    {
        private SByte[] _exponents;

        public override UnitKind Kind { get { return UnitKind.ukDerivedUnit; } }

        public override SByte[] Exponents { get { return _exponents; } }

        public DerivedUnit(IUnitSystem asystem, SByte[] someexponents = null)
            : base(asystem)
        {
            this._exponents = someexponents;
        }

        public DerivedUnit(SByte[] someexponents)
            : this(null, someexponents)
        {
        }
    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, ISystemUnit, IPhysicalUnit, INamedDerivedUnit
    {
        private NamedSymbol NamedSymbol;

        public String Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public String Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        public NamedDerivedUnit(UnitSystem somesystem, NamedSymbol somenamedsymbol, SByte[] someexponents = null)
            : base(somesystem, someexponents)
        {
            this.NamedSymbol = somenamedsymbol;
        }

        public NamedDerivedUnit(UnitSystem somesystem, String somename, String somesymbol, SByte[] someexponents = null)
            : this(somesystem, new NamedSymbol(somename, somesymbol), someexponents)
        {
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            String ExponentsStr = this.Symbol;

            String UnitName = ExponentsStr;
            if (this.System != Physics.Default_UnitSystem)
            {
                UnitName = this.System.Name + "." + ExponentsStr;
            }

            return UnitName;
        }

    }

    public class ConvertibleUnit : SystemUnit, INamedSymbol, IConvertibleUnit
    {
        private NamedSymbol _NamedSymbol;

        public String Name { get { return this._NamedSymbol.Name; } set { this._NamedSymbol.Name = value; } }
        public String Symbol { get { return this._NamedSymbol.Symbol; } set { this._NamedSymbol.Symbol = value; } }

        private IPhysicalUnit _baseunit;
        private IValueConversion _conversion;

        public IPhysicalUnit BaseUnit { get { return _baseunit; } }
        public IValueConversion Conversion { get { return _conversion; } }

        public ConvertibleUnit(NamedSymbol somenamedsymbol, IPhysicalUnit somebaseunit = null, ValueConversion someconversion = null)
            : base(somebaseunit != null ? somebaseunit.System : null)
        {
            this._NamedSymbol = somenamedsymbol;
            _baseunit = somebaseunit;
            _conversion = someconversion;
        }

        public ConvertibleUnit(String somename, String somesymbol, IPhysicalUnit somesystemunit = null, ValueConversion someconversion = null)
            : this(new NamedSymbol(somename, somesymbol), somesystemunit, someconversion)
        {
        }

        public override UnitKind Kind { get { return UnitKind.ukConvertibleUnit; } }

        public override SByte[] Exponents { get { /* return null; */  return BaseUnit.Exponents; } }



        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            String ExponentsStr = this.Symbol;
            String UnitName = ExponentsStr;
            if (this.System != Physics.Default_UnitSystem)
            {
                UnitName = this.System.Name + "." + ExponentsStr;
            }

            return UnitName;
        }

        public IPhysicalQuantity ConvertFromBaseUnit()
        {
            return this.ConvertFromBaseUnit(1);
        }

        public IPhysicalQuantity ConvertToBaseUnit()
        {
            return this.ConvertToBaseUnit(1);
        }

        public IPhysicalQuantity ConvertFromBaseUnit(double d)
        {
            return new PhysicalQuantity(Conversion.ConvertFromBaseUnit(d), BaseUnit);
        }

        public IPhysicalQuantity ConvertToBaseUnit(double d)
        {
            return new PhysicalQuantity(Conversion.ConvertToBaseUnit(d), BaseUnit);
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double d)
        {
            IPhysicalQuantity pq = this.ConvertToBaseUnit(d);
            pq = pq.ConvertToSystemUnit();
            return pq;
        }

        public override IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            if (converttounit == this) 
            {
                return new PhysicalQuantity(1, this);
            }
            else 
            {
                IPhysicalQuantity pq = this.ConvertToBaseUnit();
                if (converttounit == BaseUnit)
                {
                    return pq;
                }
                return pq.ConvertTo(converttounit);
                // throw new ArgumentException("Physical unit is not convertibel to a " + converttounit.ToString());
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

    public class PrefixedUnitExponent : IPrefixedUnitExponent
    {
        private SByte _PrefixExponent;
        private IPhysicalUnit _Unit;
        private SByte _Exponent;

        public SByte PrefixExponent { get { return _PrefixExponent; } set { _PrefixExponent = value; } }
        public IPhysicalUnit Unit { get { return _Unit; } set { _Unit = value; } }
        public SByte Exponent { get { return _Exponent; } set { _Exponent = value; } }

        public PrefixedUnitExponent(IPhysicalUnit Unit, SByte Exponent)
            : this(0, Unit, Exponent)
        {
        }

        public PrefixedUnitExponent(SByte PrefixExponent, IPhysicalUnit Unit, SByte Exponent)
        {
            this.PrefixExponent = PrefixExponent;
            this.Unit = Unit;
            this.Exponent = Exponent;
        }

        public IPhysicalQuantity PhysicalQuantity()
        {
            IPhysicalQuantity pue_pq = _Unit.Pow(_Exponent);
            pue_pq.Value *= Math.Pow(10.0, _PrefixExponent * _Exponent);
            return pue_pq;
        }

        public IPhysicalQuantity PhysicalQuantity(ref double d, IUnitSystem ISystem)
        {
            double dd = d;
            if (_PrefixExponent != 0)
            {
                dd *= Math.Pow(10.0, _PrefixExponent);
            }
            IPhysicalQuantity pue_pq = _Unit.ConvertTo(ref dd, ISystem);
            if (dd == 1.0)
            {
                d = 1.0;
            }
            if (_Exponent != 1)
            {
                pue_pq = pue_pq.Pow(_Exponent);
            }
            return pue_pq;
        }

        public static implicit operator PhysicalQuantity(PrefixedUnitExponent pue)
        {
            return pue.PhysicalQuantity() as PhysicalQuantity;
        }
    }

    public class PrefixedUnitExponentList : List<IPrefixedUnitExponent>
    {
        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString()
        {
            String Str = "";

            foreach (IPrefixedUnitExponent ue in this)
            {
                Debug.Assert(ue.Exponent != 0);
                if (Str != "")
                {
                    //Str += "*";
                    Str += '·';
                }

                if (ue.PrefixExponent != 0)
                {
                    char Prefix;
                    if (Physics.UnitPrefixes.GetPrefixCharFromExponent(ue.PrefixExponent, out Prefix))
                    {
                        Str += Prefix;
                    }
                    else
                    {
                        Debug.Assert(ue.PrefixExponent == 0);
                    }
                }
                
                Str += ue.Unit.ToString();
                if (ue.Exponent != 1)
                {
                    Str += ue.Exponent.ToString();
                }
            }
            return Str;
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
            this._Numerators = someNumerators;
            this._Denominators = someDenominators;
        }

        public CombinedUnit(IPrefixedUnitExponent pue)
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
            this._Numerators.Add(pue);
        }

        public CombinedUnit(IPhysicalUnit u)
            : this(new PrefixedUnitExponent(0, u, 1))
        {
        }

        public override UnitKind Kind { get { return UnitKind.ukCombinedUnit; } }

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
                    else if (pue.Unit.Kind == UnitKind.ukCombinedUnit)
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
            double d = 1;
            return this.ConvertToSystemUnit(ref d);
        }

        public override IPhysicalQuantity ConvertToSystemUnit(ref double d)
        {
            IPhysicalQuantity dq;
            IUnitSystem system = this.System;
            if (system == null)
            {
                system = Physics.Default_UnitSystem;
            }
            dq = this.ConvertTo(ref d, system);
            dq = dq.ConvertToSystemUnit();
            return dq;
        }

        public override IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            return this.ConvertTo(converttounit.System).ConvertTo(converttounit);
        }

        public override IPhysicalQuantity ConvertTo(IUnitSystem ISystem)
        {
            double d = 1;
            return this.ConvertTo(ref d, ISystem);
        }

        public override IPhysicalQuantity ConvertTo(ref double d, IUnitSystem ISystem)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IPhysicalQuantity pue_pq = pue.PhysicalQuantity(ref d, ISystem).ConvertToSystemUnit();
                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                pq = pq.Divide(pue.PhysicalQuantity(ref d, ISystem));
            }

            pq.Value *= d; 

            return pq;
        }


        #region IPhysicalUnitMath Members

        public override IPhysicalUnit Dimensionless { get { return new CombinedUnit(); } }

        public override IPhysicalQuantity Multiply(IPrefixedUnitExponent pue2)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            Boolean Found = false;

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                if (!Found && pue2.PrefixExponent.Equals(ue.PrefixExponent) && pue2.Unit.Equals(ue.Unit))
                {
                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    ue.Exponent -= pue2.Exponent;
                    if (ue.Exponent > 0)
                    {
                        Found = true;
                        TempDenominators.Add(ue);
                        // Done
                    }
                    else
                    {   // Convert to Numerator
                        pue2.Exponent = (SByte)(-ue.Exponent);
                    }
                }
                else
                {
                    TempDenominators.Add(ue);
                }
            }

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                if (!Found && pue2.PrefixExponent.Equals(ue.PrefixExponent) && pue2.Unit.Equals(ue.Unit))
                {
                    // Add the found CombinedUnit exponent with ue2´s exponent; 
                    ue.Exponent += pue2.Exponent;
                    if (ue.Exponent > 0)
                    {
                        Found = true;
                        TempNumerators.Add(ue);
                        // Done
                    }
                    else
                    {   // Convert to Denominator
                        pue2.Exponent = ue.Exponent;
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
                if (pue2.Exponent > 0)
                {
                    TempNumerators.Add(pue2);
                }
                else if (pue2.Exponent < 0)
                {
                    pue2.Exponent = (SByte)(-pue2.Exponent);
                    TempDenominators.Add(pue2);
                }
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            PhysicalQuantity pq = new PhysicalQuantity(1, cu);
            return pq;
        }

        public override IPhysicalQuantity Divide(IPrefixedUnitExponent ue2)
        {
            ue2.Exponent = (SByte)(-ue2.Exponent);
            return this.Multiply(ue2);
        }

        public override IPhysicalQuantity Multiply(IPhysicalUnit u2)
        {
            return this.Multiply(new PrefixedUnitExponent(0, u2, 1));
        }

        public override IPhysicalQuantity Divide(IPhysicalUnit u2)
        {
            return this.Divide(new PrefixedUnitExponent(0, u2, 1));
        }

        public override IPhysicalQuantity Multiply(double d)
        {
            return this * d;
        }

        public override IPhysicalQuantity Divide(double d)
        {
            return this / d;
        }

        public override PhysicalQuantity Power(SByte exponent)
        {
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

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            PhysicalQuantity pq = new PhysicalQuantity(1, cu);
            return pq;
        }

        public override PhysicalQuantity Root(SByte exponent)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                ue.Exponent /= exponent;
                TempNumerators.Add(ue);
            }

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                ue.Exponent /= exponent;
                TempDenominators.Add(ue);
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            PhysicalQuantity pq = new PhysicalQuantity(1, cu);
            return pq;
        }

        #endregion IPhysicalUnitMath Members

        #region Combine IPhysicalUnitMath Members

        public override IPhysicalUnit CombineMultiply(IPhysicalUnit u2)
        {
            return this.CombineMultiply(new PrefixedUnitExponent(0, u2, 1));
        }

        public override IPhysicalUnit CombineDivide(IPhysicalUnit u2)
        {
            return this.CombineDivide(new PrefixedUnitExponent(0, u2, 1));
        }

    
        public override IPhysicalUnit CombinePow(SByte exponent)
        {
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

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineRot(SByte exponent)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                ue.Exponent /= exponent;
                TempNumerators.Add(ue);
            }

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                ue.Exponent /= exponent;
                TempDenominators.Add(ue);
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineMultiply(IPrefixedUnitExponent pue2)
        {
            PrefixedUnitExponentList TempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList TempDenominators = new PrefixedUnitExponentList();

            Boolean Found = false;

            foreach (IPrefixedUnitExponent ue in Denominators)
            {
                if (!Found && pue2.PrefixExponent.Equals(ue.PrefixExponent) && pue2.Unit.Equals(ue.Unit))
                {
                    // Reduce the found CombinedUnit exponent with ue2´s exponent; 
                    ue.Exponent -= pue2.Exponent;
                    if (ue.Exponent > 0)
                    {
                        Found = true;
                        TempDenominators.Add(ue);
                        // Done
                    }
                    else
                    {   // Convert to Numerator
                        pue2.Exponent = (SByte)(-ue.Exponent);
                    }
                }
                else
                {
                    TempDenominators.Add(ue);
                }
            }

            foreach (IPrefixedUnitExponent ue in Numerators)
            {
                if (!Found && pue2.PrefixExponent.Equals(ue.PrefixExponent) && pue2.Unit.Equals(ue.Unit))
                {
                    // Add the found CombinedUnit exponent with ue2´s exponent; 
                    ue.Exponent += pue2.Exponent;
                    if (ue.Exponent > 0)
                    {
                        Found = true;
                        TempNumerators.Add(ue);
                        // Done
                    }
                    else
                    {   // Convert to Denominator
                        pue2.Exponent = ue.Exponent;
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
                if (pue2.Exponent > 0)
                {
                    TempNumerators.Add(pue2);
                }
                else if (pue2.Exponent < 0)
                {
                    pue2.Exponent = (SByte)(-pue2.Exponent);
                    TempDenominators.Add(pue2);
                }
            }

            CombinedUnit cu = new CombinedUnit(TempNumerators, TempDenominators);
            return cu;
        }

        public override IPhysicalUnit CombineDivide(IPrefixedUnitExponent ue2)
        {
            ue2.Exponent = (SByte)(-ue2.Exponent);
            return this.CombineMultiply(ue2);
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


        public override String UnitString()
        {
            String UnitName = "";
            if (Numerators.Count > 0)
            {
                UnitName = Numerators.ToString();
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
                UnitName += "/" + Denominators.ToString();
            }
            return UnitName;
        }
    }

    #endregion Combined Unit Classes

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public class UnitSystem : NamedObject, IUnitSystem
    {
        private UnitPrefixTable _unitprefixes;
        private BaseUnit[] _baseunits;
        private NamedDerivedUnit[] _namedderivedunits;
        private ConvertibleUnit[] _convertibleunits;

        public IUnitPrefixTable UnitPrefixes { get { return _unitprefixes; } }
        public IBaseUnit[] BaseUnits { get { return _baseunits; } }
        public INamedDerivedUnit[] NamedDerivedUnits { get { return _namedderivedunits; } }
        public IConvertibleUnit[] ConvertibleUnits { get { return _convertibleunits; } }

        public UnitSystem(String aname, UnitPrefixTable anunitprefixes)
            : base(aname)
        {
            this._unitprefixes = anunitprefixes;
        }

        public UnitSystem(String aname, UnitPrefixTable anunitprefixes, BaseUnit[] baseunits)
            : this(aname, anunitprefixes)
        {
            this._baseunits = baseunits;
            foreach (BaseUnit baseunit in this._baseunits)
            {
                Debug.Assert(baseunit.Kind == UnitKind.ukBaseUnit);
                if (baseunit.Kind != UnitKind.ukBaseUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.ukBaseUnit", "baseunits");
                }
                if (baseunit.System != this)
                {
                    Debug.Assert(baseunit.System == null);
                    baseunit.System = this;
                }
            }
        }

        public UnitSystem(String somename, UnitPrefixTable someunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits)
            : this(somename, someunitprefixes, baseunits)
        {
            this._namedderivedunits = somenamedderivedunits;
            foreach (NamedDerivedUnit namedderivedunit in this._namedderivedunits)
            {
                Debug.Assert(namedderivedunit.Kind == UnitKind.ukDerivedUnit);
                if (namedderivedunit.Kind != UnitKind.ukDerivedUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.ukDerivedUnit", "somenamedderivedunits");
                }
                if (namedderivedunit.System != this)
                {
                    namedderivedunit.System = this;
                }
            }
        }

        public UnitSystem(String somename, UnitPrefixTable someunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits, ConvertibleUnit[] someconvertibleunits)
            : this(somename, someunitprefixes, baseunits, somenamedderivedunits)
        {
            this._convertibleunits = someconvertibleunits;

            foreach (ConvertibleUnit convertibleunit in this._convertibleunits)
            {
                Debug.Assert(convertibleunit.Kind == UnitKind.ukConvertibleUnit);
                if (convertibleunit.Kind != UnitKind.ukConvertibleUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.ukDerivedUnit", "someconvertibleunits");
                }
                if (convertibleunit.System != this)
                {
                    convertibleunit.System = this;
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
                    //if (u.Symbol.Equals(unitsymbol, StringComparison.OrdinalIgnoreCase))
                    if (u.Symbol.Equals(unitsymbol, StringComparison.Ordinal))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        public INamedSymbolUnit UnitFromName(String unitname)
        {
            INamedSymbolUnit unit;

            unit = UnitFromName(this.BaseUnits, unitname);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.NamedDerivedUnits, unitname);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.ConvertibleUnits, unitname);
            return unit;
        }

        public INamedSymbolUnit UnitFromSymbol(String unitsymbol)
        {
            INamedSymbolUnit unit;

            unit = UnitFromSymbol(this.BaseUnits, unitsymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.NamedDerivedUnits, unitsymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.ConvertibleUnits, unitsymbol);
            return unit;
        }

        public IPhysicalUnit ScaledUnitFromSymbol(String scaledsymbolname)
        {
            SByte scaleexponent = 0;
            IPhysicalUnit unit = UnitFromSymbol(scaledsymbolname);
            if (unit == null)
            {   /* Check for prefixed unit */
                char prefixchar = scaledsymbolname[0];
                if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleexponent))
                {
                    unit = UnitFromSymbol(scaledsymbolname.Substring(1));
                }
            }

            if (unit != null && scaleexponent != 0)
            {
                unit = unit.Dimensionless.CombineMultiply(new PrefixedUnitExponent(scaleexponent, unit, 1));
            }

            return unit;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertfromunit, IPhysicalUnit converttounit)
        {
            Debug.Assert(convertfromunit != null);
            Debug.Assert(converttounit != null);

            if (convertfromunit == null)
            {
                throw new ArgumentNullException("convertfromunit");
            }

            if (converttounit == null)
            {
                throw new ArgumentNullException("converttounit");
            }

            if (convertfromunit == converttounit)
            {
                return new PhysicalQuantity(1, converttounit);
            }
            else
            {
                if (converttounit.Kind == UnitKind.ukCombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)converttounit;

                    IPhysicalQuantity pqToUnit = icu.ConvertTo(convertfromunit);

                    if (pqToUnit != null)
                    {
                        IPhysicalQuantity pq = convertfromunit.Divide(pqToUnit.Unit);
                    
                        if (DimensionExponets.IsDimensionless(pq.Unit.Exponents))
                        {
                            return new PhysicalQuantity(pq.Value / pqToUnit.Value, converttounit);
                        }
                    }

                    return null;
                }
                else if (convertfromunit.Kind == UnitKind.ukCombinedUnit)
                {
                    ICombinedUnit icu = (ICombinedUnit)convertfromunit;
                    IPhysicalQuantity pq = icu.ConvertTo(converttounit);
                    return pq;
                }
                else if (convertfromunit.Kind == UnitKind.ukConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)convertfromunit;
                    return ConvertTo(new PhysicalQuantity(icu.Conversion.ConvertToBaseUnit(1), icu.BaseUnit), converttounit);
                } 
                else if (converttounit.Kind == UnitKind.ukConvertibleUnit)
                {
                    IConvertibleUnit icu = (IConvertibleUnit)converttounit;
                    IPhysicalQuantity converted_fromunit = ConvertTo(convertfromunit, icu.BaseUnit);
                    if (converted_fromunit != null)
                    {
                        converted_fromunit = new PhysicalQuantity(icu.Conversion.ConvertFromBaseUnit(converted_fromunit.Value), converttounit);
                    }

                    return converted_fromunit;
                }

                if ((convertfromunit.System == this) && (converttounit.System == this))
                {   /* Intra unit system conversion */
                    Debug.Assert((convertfromunit.Kind == UnitKind.ukBaseUnit) || (convertfromunit.Kind == UnitKind.ukDerivedUnit));
                    Debug.Assert((converttounit.Kind == UnitKind.ukBaseUnit) || (converttounit.Kind == UnitKind.ukDerivedUnit));

                    if (!((convertfromunit.Kind == UnitKind.ukBaseUnit) || (convertfromunit.Kind == UnitKind.ukDerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of ukBaseUnit or a ukDerivedUnit", "convertfromunit");
                    }

                    if (!((converttounit.Kind == UnitKind.ukBaseUnit) || (converttounit.Kind == UnitKind.ukDerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of ukBaseUnit or a ukDerivedUnit", "converttounit");
                    }

                    if (DimensionExponets.Equals(convertfromunit.Exponents, converttounit.Exponents))
                    {
                        return new PhysicalQuantity(1, converttounit);
                    }
                }
                else
                {   /* Inter unit system conversion */
                    UnitSystemConversion usc = Physics.GetUnitSystemConversion(convertfromunit.System, converttounit.System);
                    if (usc != null)
                    {
                        return usc.ConvertTo(convertfromunit, converttounit);
                    }
                }

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit)
        {
            Debug.Assert(physicalquantity.Unit != null);
            Debug.Assert(converttounit != null);

            if (physicalquantity.Unit == null)
            {
                throw new ArgumentException("Must have a unit", "physicalquantity");
            }

            if (converttounit == null)
            {
                throw new ArgumentNullException("converttounit");
            }

            /**
            Not correct for convertible units with offset != 0
            IPhysicalQuantity pq = ConvertTo(physicalquantity.Unit, converttounit);
            if (pq != null)
            {
                pq = pq.Multiply(physicalquantity.Value);
            }
            return pq;
            **/

            if (physicalquantity.Unit == converttounit)
            {
                return physicalquantity;
            }
            else
            {
                IUnitSystem convertfromunitsystem = physicalquantity.Unit.System;
                IUnitSystem converttounitsystem   = converttounit.System;

                if (convertfromunitsystem == null)
                {   
                    Debug.Assert(physicalquantity.Unit.Kind == UnitKind.ukCombinedUnit);

                    IUnitSystem tempconverttounitsystem = converttounitsystem;
                    if (tempconverttounitsystem == null)
                    {   // Find some system to convert into

                        ICombinedUnit icu = (ICombinedUnit)physicalquantity.Unit;
                        Debug.Assert(icu != null);

                        tempconverttounitsystem = icu.SomeSystem;
                    }
                    if (tempconverttounitsystem != null) 
                    {
                        physicalquantity = physicalquantity.ConvertTo(tempconverttounitsystem);

                        if (physicalquantity != null)
                        {
                            convertfromunitsystem = physicalquantity.Unit.System;
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
                    Debug.Assert(converttounit.Kind == UnitKind.ukCombinedUnit);

                    ICombinedUnit icu = (ICombinedUnit)converttounit;
                    Debug.Assert(icu != null);

                    IUnitSystem tempconverttounitsystem;
                    tempconverttounitsystem = icu.SomeSystem;
                    Debug.Assert(tempconverttounitsystem != null);

                    // ?? What TO DO here ??
                    Debug.Assert(false);
                }

                if (converttounitsystem != null && convertfromunitsystem != converttounitsystem)
                {   /* Inter unit system conversion */
                    IPhysicalQuantity pq = this.ConvertTo(physicalquantity, converttounitsystem);
                    if (pq != null)
                    {
                        physicalquantity = pq;
                        convertfromunitsystem = physicalquantity.Unit.System;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (convertfromunitsystem != null && convertfromunitsystem == converttounitsystem)
                {   /* Intra unit system conversion */

                    if (physicalquantity.Unit.Kind == UnitKind.ukCombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)physicalquantity.Unit;
                        double d = physicalquantity.Value;
                        IPhysicalQuantity pq = icu.ConvertToSystemUnit(ref d);
                        Debug.Assert(pq != null);
                        pq = pq.ConvertTo(converttounit);
                        return pq;
                    }
                    else if (converttounit.Kind == UnitKind.ukCombinedUnit)
                    {
                        ICombinedUnit icu = (ICombinedUnit)converttounit;
                        IPhysicalQuantity ToUnit_SystemUnit_pq = icu.ConvertToSystemUnit();
                        Debug.Assert(ToUnit_SystemUnit_pq != null);

                        IPhysicalQuantity pq_ToSystemUnit = physicalquantity.ConvertTo(ToUnit_SystemUnit_pq.Unit);
                        if (pq_ToSystemUnit != null)
                        {
                            return new PhysicalQuantity(pq_ToSystemUnit.Value / ToUnit_SystemUnit_pq.Value, converttounit);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (physicalquantity.Unit.Kind == UnitKind.ukConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)physicalquantity.Unit;
                        return ConvertTo(icu.ConvertToBaseUnit(physicalquantity.Value), converttounit);
                    }
                    else if (converttounit.Kind == UnitKind.ukConvertibleUnit)
                    {
                        IConvertibleUnit icu = (IConvertibleUnit)converttounit;
                        IPhysicalQuantity pq = ConvertTo(physicalquantity, icu.BaseUnit);
                        if (pq != null)
                        {
                            pq = icu.ConvertFromBaseUnit(pq.Value);
                        }

                        return pq;
                    }

                    Debug.Assert((physicalquantity.Unit.Kind == UnitKind.ukBaseUnit) || (physicalquantity.Unit.Kind == UnitKind.ukDerivedUnit));
                    Debug.Assert((converttounit.Kind == UnitKind.ukBaseUnit) || (converttounit.Kind == UnitKind.ukDerivedUnit));

                    if (!((physicalquantity.Unit.Kind == UnitKind.ukBaseUnit) || (physicalquantity.Unit.Kind == UnitKind.ukDerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of ukBaseUnit or a ukDerivedUnit", "convertfromunit");
                    }

                    if (!((converttounit.Kind == UnitKind.ukBaseUnit) || (converttounit.Kind == UnitKind.ukDerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of ukBaseUnit or a ukDerivedUnit", "converttounit");
                    }

                    if (DimensionExponets.Equals(physicalquantity.Unit.Exponents, converttounit.Exponents))
                    {
                        return new PhysicalQuantity(physicalquantity.Value, converttounit);
                    }
                }

                return null;
            }
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IUnitSystem converttounitsystem)
        {
            if (physicalquantity.Unit.System == converttounitsystem)
            {
                return physicalquantity;
            }

            {   /* Inter unit system conversion */
                UnitSystemConversion usc = Physics.GetUnitSystemConversion(physicalquantity.Unit.System, converttounitsystem);
                if (usc != null)
                {
                    return usc.ConvertTo(physicalquantity, converttounitsystem);
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
            set { _BaseUnitSystem = value; }
        }
        private IUnitSystem _ConvertedUnitSystem;

        public IUnitSystem ConvertedUnitSystem
        {
            get { return _ConvertedUnitSystem; }
            set { _ConvertedUnitSystem = value; }
        }

        public ValueConversion[] BaseUnitConversions;

        public UnitSystemConversion(IUnitSystem somebaseunitsystem, IUnitSystem someconvertedunitsystem, ValueConversion[] somebaseunitconversions)
        {
            this.BaseUnitSystem = somebaseunitsystem;
            this.ConvertedUnitSystem = someconvertedunitsystem;
            this.BaseUnitConversions = somebaseunitconversions;
        }

        public static bool IsSameUnit(IPhysicalUnit unit1, IPhysicalUnit unit2)
        {
            if (unit1 == unit2)
            {
                return true;
            }
            else
                if (unit1.System == unit2.System)
                {
                    return DimensionExponets.Equals(unit1.Exponents, unit2.Exponents);
                }
            return false;
        }

        public IPhysicalQuantity Convert(IPhysicalUnit convertunit, bool backwards = false)
        {
            SByte[] FromUnitExponents = convertunit.Exponents; 

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
                           && !DimensionExponets.Equals(unitsystem.NamedDerivedUnits[namedderivedunitsindex].Exponents, FromUnitExponents))
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
                    unit = new DerivedUnit(unitsystem, FromUnitExponents);
                }
            }
            return new PhysicalQuantity(value, unit);
        }

        public IPhysicalQuantity Convert(IPhysicalQuantity physicalquantity, bool backwards = false)
        {
            IPhysicalQuantity pq = Convert(physicalquantity.Unit, backwards);
            return new PhysicalQuantity(physicalquantity.Value * pq.Value, pq.Unit);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalUnit convertunit)
        {
            return Convert(convertunit, false);
        }

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalUnit convertunit)
        {
            return Convert(convertunit, true);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalQuantity physicalquantity)
        {
            return Convert(physicalquantity, false);
        }

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalQuantity physicalquantity)
        {
            return Convert(physicalquantity, true);
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertfromunit, IUnitSystem convertounitsystem)
        {
            if ((convertfromunit.System == BaseUnitSystem) && (convertounitsystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(convertfromunit);
            }
            else
                if ((convertfromunit.System == ConvertedUnitSystem) && (convertounitsystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(convertfromunit);
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IUnitSystem convertounitsystem)
        {
            if ((physicalquantity.Unit.System == BaseUnitSystem) && (convertounitsystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(physicalquantity);
            }
            else
                if ((physicalquantity.Unit.System == ConvertedUnitSystem) && (convertounitsystem == BaseUnitSystem))
                {
                    return this.ConvertToBaseUnitSystem(physicalquantity);
                }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit convertfromunit, IPhysicalUnit converttounit)
        {
            IPhysicalQuantity pq = this.ConvertTo(convertfromunit, converttounit.System);
            if (pq != null)
            {
                pq = pq.ConvertTo(converttounit);
            }
            return pq;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit)
        {
            IPhysicalQuantity pq = this.ConvertTo(physicalquantity, converttounit.System);
            if (pq != null)
            {
                pq = pq.ConvertTo(converttounit);
            }
            return pq;
        }
    }

    #endregion Physical Unit System Conversions

    #region Physical Quantity Classes
     
    public class PhysicalQuantity : IPhysicalQuantity
    {
        // The value holders
        private double _value;
        private IPhysicalUnit _unit;

        public double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public IPhysicalUnit Unit
        {
            get
            {
                return this._unit;
            }
            set
            {
                this._unit = value;
            }
        }

        public IPhysicalUnit Dimensionless { get { return Unit.Dimensionless; } }

        public PhysicalQuantity()
            : this(0)
        {
        }

        public PhysicalQuantity(double somevalue)
            : this(somevalue, Physics.dimensionless)
        {
            this._value = somevalue;
        }

        public PhysicalQuantity(double somevalue, IPhysicalUnit someunit)
        {
            this._value = somevalue;
            this._unit = someunit;
        }

        public PhysicalQuantity(IPhysicalQuantity somephysicalquantity)
            : this(somephysicalquantity.Value, somephysicalquantity.Unit)
        {
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
            return this.Value.ToString(format, formatProvider) + " " + this.Unit.ToString();
        }

        public override String ToString()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture) + " " + this.Unit.ToString();
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitsymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static IPhysicalQuantity Parse(String s, System.Globalization.NumberStyles styles, IFormatProvider provider)
        {
            String[] Strings = s.Trim().Split(' ');

            if (Strings.GetLength(0) > 0)
            {
                // Parse value
                String ValueStr = Strings[0];
                Double value = Double.Parse(ValueStr, styles, provider);

                IPhysicalUnit unit = null;

                if (Strings.GetLength(0) > 1)
                {
                    // Parse unit
                    String UnitStr = Strings[1];
                    unit = PhysicalUnit.ParseUnit(ref UnitStr);
                }
                else
                {
                    unit = Physics.dimensionless;
                }

                return new PhysicalQuantity(value, unit);
            }

            throw new ArgumentException("Not a valid physical quantity format", "s");
        }


        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitsymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static IPhysicalQuantity Parse(String s)
        {

            String[] Strings = s.Trim().Split(' ');

            if (Strings.GetLength(0) > 0)
            {
                // Parse value
                String ValueStr = Strings[0];
                Double value = Double.Parse(ValueStr, System.Globalization.NumberStyles.Float, NumberFormatInfo.InvariantInfo);

                IPhysicalUnit unit = null;

                if (Strings.GetLength(0) > 1)
                {
                    // Parse unit
                    String UnitStr = Strings[1];
                    unit = PhysicalUnit.ParseUnit(ref UnitStr);
                }
                else
                {
                    unit = Physics.dimensionless;
                }

                return new PhysicalQuantity(value, unit);
            }

            throw new ArgumentException("Not a valid physical quantity format", "s");
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

        public IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            if (this.Unit == converttounit)
            {
                return this;
            }
            Debug.Assert(this.Unit != null);
            Debug.Assert(converttounit != null);

            if (this.Unit == null)
            {
                throw new InvalidOperationException("Must have a unit to convert it");
            }

            if (converttounit == null)
            {
                throw new ArgumentNullException("converttounit");
            }

            IUnitSystem converttounitsystem = converttounit.System; 
            if (converttounitsystem == null)
            {
                converttounitsystem = this.Unit.System; 
            }
            if (converttounitsystem == null)
            {
                converttounitsystem = Physics.Default_UnitSystem;
            }

            if (converttounitsystem != null)
            {
                IPhysicalQuantity quantity = converttounitsystem.ConvertTo(this as IPhysicalQuantity, converttounit);
                return quantity;
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem)
        {
            if (this.Unit.System == converttounitsystem)
            {
                return this;
            }

            Debug.Assert(this.Unit != null);
            Debug.Assert(converttounitsystem != null);

            if (this.Unit == null)
            {
                throw new InvalidOperationException("Must have a unit to convert it");
            }

            if (converttounitsystem == null)
            {
                throw new ArgumentNullException("converttounitsystem");
            }

            if (this.Unit.System != null)
            {
                IPhysicalQuantity quantity = this.Unit.System.ConvertTo(this as IPhysicalQuantity, converttounitsystem);
                return quantity;
            }

            return null;
        }

        public static IPhysicalUnit PureUnit(IPhysicalQuantity pq)
        {
            if (pq.Value != 1)
            {
                throw new ArgumentException("Physical quantity is not a pure unit; but has a value = " + pq.Value.ToString());
            }

            return pq.Unit;
        }

        public static IPhysicalUnit operator !(PhysicalQuantity pq)
        {
            return PureUnit(pq);
        }

        public static implicit operator PhysicalUnit(PhysicalQuantity pq)
        {
            return PureUnit(pq) as PhysicalUnit;
        }

        /*
        public static implicit operator IPhysicalQuantity(PhysicalQuantity pq)
        {
            return pq as IPhysicalQuantity;
        }
        */

        public int ValueCompare(double othervalue)
        {   /* Limited precision handling */
            double RelativeDiff = (this.Value - othervalue)/ this.Value;
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

        public bool Equals(IPhysicalUnit someunit)
        {
            IPhysicalQuantity other = new PhysicalQuantity(1, someunit);
            return this.Equals(other);
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

            throw new InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object or IPhysicalUnit object.");
        }

        public static bool operator ==(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return pq1.Equals(pq2);
        }

        public static bool operator !=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return (!pq1.Equals(pq2));
        }

        public static bool operator <(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return pq1.CompareTo(pq2) < 0;
        }

        public static bool operator <=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return pq1.CompareTo(pq2) <= 0;
        }

        public static bool operator >(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return pq1.CompareTo(pq2) > 0;
        }

        public static bool operator >=(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return pq1.CompareTo(pq2) >= 0;
        }


        #region Physical Quantity static operator methods

        public delegate double CombineValuesFunc(double v1, double v2);
        public delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);

        public static PhysicalQuantity CombineValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf)
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

        public static PhysicalQuantity CombineUnitsAndValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf, PhysicalUnit.CombineExponentsFunc cef)
        {

            Debug.Assert(pq1.Unit.Kind != UnitKind.ukCombinedUnit);
            Debug.Assert(pq2.Unit.Kind != UnitKind.ukCombinedUnit);

            if (pq1.Unit.Kind == UnitKind.ukCombinedUnit) 
            {
                ICombinedUnit pg1_unit = pq1.Unit as ICombinedUnit;
                pq1 = pq1.ConvertToSystemUnit();

            }

            if (pq2.Unit.Kind == UnitKind.ukCombinedUnit)
            {
                IConvertibleUnit pg2_unit = pq2.Unit as IConvertibleUnit;
                pq2 = pq2.ConvertTo(pg2_unit.BaseUnit);
            }

            while (pq1.Unit.Kind == UnitKind.ukConvertibleUnit)
            {
                IConvertibleUnit pg1_unit = pq1.Unit as IConvertibleUnit;
                pq1 = pq1.ConvertTo(pg1_unit.BaseUnit);
            }
            while (pq2.Unit.Kind == UnitKind.ukConvertibleUnit)
            {
                IConvertibleUnit pg2_unit = pq2.Unit as IConvertibleUnit;
                pq2 = pq2.ConvertTo(pg2_unit.BaseUnit);
            }

            if (pq2.Unit.System != pq1.Unit.System)
            {   // Must be same unit system
                pq2 = pq2.ConvertTo(pq1.Unit.System);
            }

            SByte[] someexponents = new SByte[Physics.NoOfMeasures];

            for (int i = 0; i < Physics.NoOfMeasures; i++)
            {
                someexponents[i] = cef(pq1.Unit.Exponents[i], pq2.Unit.Exponents[i]);
            }
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
            IPhysicalQuantity pq2 = new PhysicalQuantity(1);
            pq2 = pq2.Divide(pq);
            pq2.Value = pq2.Value * d;
            return new PhysicalQuantity(pq2);
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
            pq.Value = pq.Value * System.Math.Pow(this.Value, exponent);
            return new PhysicalQuantity(pq);
        }

        public PhysicalQuantity Root(SByte exponent)
        {
            IPhysicalQuantity pq = this.Unit.Rot(exponent);
            pq.Value = pq.Value * System.Math.Pow(this.Value, 1.0/exponent);
            return new PhysicalQuantity(pq);
        }

        #region Physical Quantity IPhysicalUnitMath implementation

        public IPhysicalQuantity Add(IPhysicalQuantity pq2)
        {
            return CombineValues(this, pq2, (double v1, double v2) => v1 + v2);
        }

        public IPhysicalQuantity Subtract(IPhysicalQuantity pq2)
        {
            return CombineValues(this, pq2, (double v1, double v2) => v1 - v2);
        }

        public IPhysicalQuantity Multiply(IPhysicalUnit u2)
        {
            return this.Multiply(new PrefixedUnitExponent(0, u2, 1));
        }

        public IPhysicalQuantity Divide(IPhysicalUnit u2)
        {
            return this.Divide(new PrefixedUnitExponent(0, u2, 1));
        }

        public IPhysicalQuantity Multiply(IPhysicalQuantity pq2)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(this.Value, pq2);
            return pq;
        }

        public IPhysicalQuantity Divide(IPhysicalQuantity pq2)
        {
            IPhysicalQuantity pq = this.Unit.Divide(this.Value, pq2);
            return pq;
        }

        public IPhysicalQuantity Multiply(double d)
        {
            return new PhysicalQuantity(this.Value * d, this.Unit);
        }

        public IPhysicalQuantity Divide(double d)
        {
            return new PhysicalQuantity(this.Value / d, this.Unit);
        }
 
        public IPhysicalQuantity Pow(SByte exponent)
        {
            return this.Power(exponent);
        }

        public IPhysicalQuantity Rot(SByte exponent)
        {
            return this.Root(exponent);
        }

        public IPhysicalQuantity Multiply(IPrefixedUnitExponent pue)
        {
            IPhysicalQuantity pq = this.Unit.Multiply(pue);
            pq.Value = this.Value * pq.Value;
            return pq;
        }

        public IPhysicalQuantity Divide(IPrefixedUnitExponent pue)
        {
            IPhysicalQuantity pq = this.Unit.Divide(pue);
            pq.Value = this.Value * pq.Value;
            return pq;
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
                                                                                                    new UnitPrefix("micro", 'μ', -6), 
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
                                                                    new BaseUnit(null, (SByte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                    new BaseUnit(null, (SByte)MeasureKind.Luminous_intensity, "candela", "cd") };

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
                                                                                         new ConvertibleUnit("Celsius", "°C", SI_BaseUnits[(int)(MeasureKind.Thermodynamic_temperature)], new LinearyValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                                                                         //new ConvertibleUnit("Celsius", "@C", SI_BaseUnits[(int)(MeasureKind.Thermodynamic_temperature)], new LinearyValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                                                                         new ConvertibleUnit("hour", "h", SI_BaseUnits[(int)(MeasureKind.Time)], new ScaledValueConversion(1.0/3600)) }); /* [h] = 1/3600 * [s] */
        public static readonly PhysicalUnit dimensionless = new DerivedUnit(SI_Units, new SByte[] { 0, 0, 0, 0, 0, 0, 0 });


        public static readonly UnitSystem CGS_Units = new UnitSystem("CGS", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(CGS_Units, (SByte)MeasureKind.Length, "centimeter", "cm"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Mass, "gram", "g"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Time, "second", "s"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(CGS_Units, (SByte)MeasureKind.Luminous_intensity, "candela", "cd")});

        public static readonly UnitSystem MGD_Units = new UnitSystem("MGD", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGD_Units, (SByte)MeasureKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGD_Units, (SByte)MeasureKind.Time, "second", "s"), */
                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Time, "day", "d"),
                                                                                /*  new BaseUnit(MGD_Units, "moment", "ø"), */

                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(MGD_Units, (SByte)MeasureKind.Luminous_intensity, "candela", "cd") } );

        public static readonly UnitSystem MGM_Units = new UnitSystem("MGM", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGM_Units, (SByte)MeasureKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "second", "s"), */
                                                                                /*  new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "day", "d"), */
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Time, "moment", "ø"), 

                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(MGM_Units, (SByte)MeasureKind.Luminous_intensity, "candela", "cd") });


        public static UnitSystem[] UnitSystems = new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units };

        public static UnitSystem Default_UnitSystem = SI_Units; 

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


        public static UnitSystemConversion[] UnitSystemConversions = new UnitSystemConversion[] { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion };

        public static UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                if ((usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                    || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                {
                    return usc;
                }
            }

            /* TO DO: Missing direct unit system conversion from  SomeUnitSystem to SomeOtherUnitSystem. 
             *        Try to find intermediere unit system with conversion to/from SomeUnitSystem and SomeOtherUnitSystem */
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

        //public static IPhysicalQuantity ScaledUnitFromSymbol(String scaledsymbolstr)
        public static IPhysicalUnit ScaledUnitFromSymbol(String scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                //IPhysicalQuantity scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                IPhysicalUnit scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }

        //public static IPhysicalQuantity ScaledUnitFromSymbol(IPhysicalQuantity one, String scaledsymbolstr)
        public static IPhysicalUnit ScaledUnitFromSymbol(IPhysicalQuantity one, String scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                //IPhysicalQuantity scaledunit = us.ScaledUnitFromSymbol(one, scaledsymbolstr);
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
        /**
        public static readonly PhysicalQuantity GF = new PhysicalQuantity(1.16639E-5, (Prefix.G * Constants.e * SI.V) ^ -2);
        **/
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
