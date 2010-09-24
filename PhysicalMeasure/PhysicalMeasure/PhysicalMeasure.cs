/*   http://physicalmeasure.codeplex.com   */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PhysicalMeasure
{
    #region Physical Measure Constants

    public static partial class Physic
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

    public enum UnitKind { ukBaseUnit, ukDerivedUnit, ukConvertibleUnit };

    #endregion Physical Measure Constants

    #region Physical Measure Interfaces

    public interface INamed
    {
        string Name { get; /* set; */ }
    }

    public interface INamedSymbol : INamed
    {
        string Symbol { get; /* set; */ }
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
        sbyte[] Exponents { get; }
    }

    public interface IBaseUnit : IUnit, INamed
    {
        sbyte BaseUnitNumber { get; }
    }

    public interface IDerivedUnit : IUnit
    {
    }

    public interface INamedDerivedUnit : IDerivedUnit, INamed
    {
    }

    public interface IConvertibleUnit : IUnit, INamed
    {
        IPhysicalUnit BaseUnit { get; }
        IValueConvertion Convertion { get; }
    }

    public interface ISystemUnit : ISystemItem, IUnit
    {
    }

    public interface IPhysicalUnitMath : IEquatable<IPhysicalUnit>
    {
        IPhysicalQuantity Multiply(IPhysicalUnit u2);
        IPhysicalQuantity Divide(IPhysicalUnit u2);
    }

    public interface IPhysicalUnit : ISystemUnit, IPhysicalUnitMath /*  : <BaseUnit | DerivedUnit | ConvertibleUnit>  */
    {
        IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem);
    }

    public interface IValueConvertion
    {
        double Convert(double value, bool backwards = false);
        double ConvertFromBaseunit(double value);
        double ConvertToBaseunit(double value);
    }

    public interface IPhysicalQuantityMath : IComparable, IEquatable<IPhysicalQuantity>
    {
        IPhysicalQuantity Add(IPhysicalQuantity pq2);
        IPhysicalQuantity Subtract(IPhysicalQuantity pq2);
        IPhysicalQuantity Multiply(IPhysicalQuantity pq2);
        IPhysicalQuantity Divide(IPhysicalQuantity pq2);

        IPhysicalQuantity Pow(sbyte exponent);
        IPhysicalQuantity Rot(sbyte exponent);
    }


    public interface IPhysicalQuantity : IFormattable, IPhysicalQuantityMath
    {
        double Value { get; set; }
        IPhysicalUnit Unit { get; set; }

        IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem);
    }

    public interface IUnitSystem : INamed
    {
        IBaseUnit[] BaseUnits { get; }
        INamedDerivedUnit[] NamedDerivedUnits { get; }
        IConvertibleUnit[] ConvertibleUnits { get; }

        IPhysicalUnit UnitFromName(string unitname);
        IPhysicalUnit UnitFromSymbol(string symbolname);
        IPhysicalQuantity ScaledUnitFromSymbol(string scaledunitname);

        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit);
        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IUnitSystem converttounitsystem);
    }

    public interface IUnitPrefix : INamed
    {
        char PrefixChar { get; }
        sbyte PrefixExponent { get; } 
    }

    public interface IUnitPrefixTable
    {
        IUnitPrefix[] UnitPrefixes { get; }

        bool GetExponentFromPrefixChar(char someprefixchar, out sbyte exponent); 
    }

    #endregion Physical Measure Interfaces

    #region Dimension Exponets Classes

    public static class DimensionExponets 
    {
        // sbyte[Physic.NoOfMeasures] SomeExponents;
        static public bool Equals(sbyte[] exponents1, sbyte[] exponents2)
        {
            bool equal = true;

            sbyte i = 0;
            do
            {
                equal = exponents1[i] == exponents2[i];
                i++;
            } while (i < Physic.NoOfMeasures && equal);

            return equal;
        }
    }

    #endregion Dimension Exponets Classes

    #region Physical Unit prefix Classes

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private char _prefixchar;
        private sbyte _prefixexponent;

        public char PrefixChar { get { return _prefixchar; } }
        public sbyte PrefixExponent { get { return _prefixexponent; } }

        public UnitPrefix(string somename, char someprefixchar, sbyte someprefixexponent)
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

        public bool GetExponentFromPrefixChar(char someprefixchar, out sbyte exponent) 
        {
            exponent = 0;
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
    }

    #endregion Physical Unit prefix Classes

    #region Value Conversion Classes

    public abstract class ValueConvertion : IValueConvertion
    {
        public abstract double Convert(double value, bool backwards = false);
        public abstract double ConvertFromBaseunit(double value);
        public abstract double ConvertToBaseunit(double value);
    }

    public class LinearyValueConvertion : ValueConvertion
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

        public LinearyValueConvertion(double someoffset, double somescale)
        {
            this.Offset = someoffset;
            this.Scale = somescale;
        }

        public override double Convert(double value, bool backwards)
        {
            if (backwards)
            {
                return ConvertToBaseunit(value);
            }
            else
            {
                return ConvertFromBaseunit(value);
            }
        }

        public override double ConvertFromBaseunit(double value)
        {
            return value * this.Scale + this.Offset;
        }

        public override double ConvertToBaseunit(double value)
        {
            return (value - this.Offset) / this.Scale;
        }
    }

    public class ScaledValueConvertion : LinearyValueConvertion
    {
        public ScaledValueConvertion(double somescale)
            : base(0, somescale)
        {
        }
    }

    public class IdentityValueConvertion : ScaledValueConvertion
    {
        public IdentityValueConvertion()
            : base(1)
        {
        }
    }

    #endregion Value Conversion Classes
    
    #region Physical Unit Classes

    public class NamedObject : INamed
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public NamedObject(string aname)
        {
            this.Name = aname;
        }
    }

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        private string _symbol;
        public string Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbol(string aname, string asymbol)
            : base(aname)
        {
            this.Symbol = asymbol;
        }
    }

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
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public NamedSystemObject(IUnitSystem somesystem, string somename)
            : base(somesystem)
        {
            this.Name = somename;
        }

        public NamedSystemObject(string aname)
            : this(null, aname)
        {
        }
    }

    public class NamedSymbolSystemObject : NamedSystemObject, INamedSymbol
    {
        private string _symbol;
        public string Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbolSystemObject(IUnitSystem somesystem, string somename, string somesymbol)
            : base(somesystem, somename)
        {
            this.Symbol = somesymbol;
        }

        public NamedSymbolSystemObject(string somename, string somesymbol)
            : this(null, somename, somesymbol)
        {
        }
    }

    public abstract class PhysicalUnit : SystemObject, IPhysicalUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        public PhysicalUnit(IUnitSystem somesystem)
            : base(somesystem)
        {
        }

        public abstract UnitKind Kind { get; }
        public abstract sbyte[] Exponents { get; }

        public virtual IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1, this);
            if (converttounit != this)
            {
                pq = this.System.ConvertTo(pq, converttounit);
            }
            return pq;
        }

        public virtual IPhysicalQuantity ConvertTo(IUnitSystem converttounitsystem)
        {
            IPhysicalQuantity pq = new PhysicalQuantity(1, this);
            if (converttounitsystem != this.System)
            {
                pq = this.System.ConvertTo(pq, converttounitsystem);
            }
            return pq;
        }

        public bool Equals(IPhysicalUnit other)
        {
            return ((this.System == other.System) && (this.Exponents == other.Exponents));
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) return base.Equals(obj);

            if (!(obj is IPhysicalUnit))
                throw new InvalidCastException("The 'obj' argument is not a IPhysicalUnit object.");
            else
                return Equals(obj as IPhysicalUnit);
        }

        public override int GetHashCode()
        {
            return this.System.GetHashCode() + this.Exponents.GetHashCode();
        }

        public static bool operator ==(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            return unit1.Equals(unit2);
        }

        public static bool operator !=(PhysicalUnit unit1, IPhysicalUnit unit2)
        {
            return (!unit1.Equals(unit2));
        }

        public delegate sbyte CombineExponentsFunc(sbyte e1, sbyte e2);

        public static PhysicalQuantity CombineUnits(IPhysicalUnit u1, IPhysicalUnit u2, CombineExponentsFunc cef)
        {
            IPhysicalQuantity pq2 = new PhysicalQuantity(1, u2);
            if (u2.System != u1.System)
            {
                pq2 = u1.System.ConvertTo(pq2, u1);
            }
            sbyte[] someexponents = new sbyte[Physic.NoOfMeasures];

            for (int i = 0; i < Physic.NoOfMeasures; i++)
            {
                someexponents[i] = cef(u1.Exponents[i], pq2.Unit.Exponents[i]);
            }
            PhysicalUnit pu = new DerivedUnit(u1.System, someexponents);
            return new PhysicalQuantity(pq2.Value, pu);
        }

        public PhysicalUnit Power(sbyte exponent)
        {
            sbyte[] someexponents = new sbyte[Physic.NoOfMeasures];

            for (int i = 0; i < Physic.NoOfMeasures; i++)
            {
                someexponents[i] = (sbyte)(this.Exponents[i] * exponent);
            }
            PhysicalUnit pu = new DerivedUnit(this.System, someexponents);
            return pu;
        }

        public static PhysicalUnit operator ^(PhysicalUnit pu, sbyte exponent)
        {
            return pu.Power(exponent);
        }

        public static PhysicalQuantity operator *(PhysicalUnit u1, IPhysicalUnit u2)
        {
            return CombineUnits(u1, u2, (sbyte e1, sbyte e2) => (sbyte)(e1 + e2));
        }

        public static PhysicalQuantity operator /(PhysicalUnit u1, IPhysicalUnit u2)
        {
            return CombineUnits(u1, u2, (sbyte e1, sbyte e2) => (sbyte)(e1 - e2));
        }

        public IPhysicalQuantity Multiply(IPhysicalUnit u2)
        {
            return this * u2;
        }

        public IPhysicalQuantity Divide(IPhysicalUnit u2)
        {
            return this / u2;
        }

    }

    public class BaseUnit : PhysicalUnit, INamedSymbol, IBaseUnit
    {
        private NamedSymbol NamedSymbol;

        public string Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public string Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        private sbyte _baseunitnumber;
        public sbyte BaseUnitNumber { get { return _baseunitnumber; } }

        public override UnitKind Kind { get { return UnitKind.ukBaseUnit; } }

        public override sbyte[] Exponents { get { sbyte[] tempexponents = new sbyte[System.BaseUnits.Length]; tempexponents[_baseunitnumber] = 1; return tempexponents; } } 

        public BaseUnit(IUnitSystem someunitsystem, sbyte somebaseunitnumber, NamedSymbol somenamedsymbol)
            : base(someunitsystem)
        {
            this._baseunitnumber = somebaseunitnumber;
            this.NamedSymbol = somenamedsymbol;
        }

        public BaseUnit(IUnitSystem someunitsystem, sbyte somebaseunitnumber, string somename, string somesymbol)
            : this(someunitsystem, somebaseunitnumber, new NamedSymbol(somename, somesymbol))
        {
        }

        public BaseUnit(sbyte somebaseunitnumber, string somename, string somesymbol)
            : this(null, somebaseunitnumber, somename, somesymbol)
        {
        }
    }

    public class DerivedUnit : PhysicalUnit, IDerivedUnit
    {
        private sbyte[] _exponents;

        public override UnitKind Kind { get { return UnitKind.ukDerivedUnit; } }

        public override sbyte[] Exponents { get { return _exponents; } }

        public DerivedUnit(IUnitSystem asystem, sbyte[] someexponents = null)
            : base(asystem)
        {
            this._exponents = someexponents;
        }

        public DerivedUnit(sbyte[] someexponents)
            : this(null, someexponents)
        {
        }
    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, ISystemUnit, IPhysicalUnit, INamedDerivedUnit
    {
        private NamedSymbol NamedSymbol;

        public string Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public string Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        public NamedDerivedUnit(UnitSystem somesystem, NamedSymbol somenamedsymbol, sbyte[] someexponents = null)
            : base(somesystem, someexponents)
        {
            this.NamedSymbol = somenamedsymbol;
        }

        public NamedDerivedUnit(UnitSystem somesystem, string somename, string somesymbol, sbyte[] someexponents = null)
            : this(somesystem, new NamedSymbol(somename, somesymbol), someexponents)
        {
        }
    }

    public class ConvertibleUnit : PhysicalUnit, INamedSymbol, IConvertibleUnit
    {
        private NamedSymbol _NamedSymbol;

        public string Name { get { return this._NamedSymbol.Name; } set { this._NamedSymbol.Name = value; } }
        public string Symbol { get { return this._NamedSymbol.Symbol; } set { this._NamedSymbol.Symbol = value; } }

        private IPhysicalUnit _baseunit;
        private IValueConvertion _convertion;

        public IPhysicalUnit BaseUnit { get { return _baseunit; } }
        public IValueConvertion Convertion { get { return _convertion; } }

        public ConvertibleUnit(NamedSymbol somenamedsymbol, IPhysicalUnit somebaseunit = null, ValueConvertion someconvertion = null)
            : base(somebaseunit != null ? somebaseunit.System : null)
        {
            this._NamedSymbol = somenamedsymbol;
            _baseunit = somebaseunit;
            _convertion = someconvertion;
        }

        public ConvertibleUnit(string somename, string somesymbol, IPhysicalUnit somesystemunit = null, ValueConvertion someconvertion = null)
            : this(new NamedSymbol(somename, somesymbol), somesystemunit, someconvertion)
        {
        }

        public override UnitKind Kind { get { return UnitKind.ukConvertibleUnit; } }

        public override sbyte[] Exponents { get { /* return null; */  return BaseUnit.Exponents; } }

        /** public UnitSystem system { get { return baseunit.system; } / * set { systemunit.system = value; } * / } **/

        public override IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            if (converttounit == this) 
            {
                return new PhysicalQuantity(1, this);
            }
            else 
            {
                PhysicalQuantity pq = new PhysicalQuantity(Convertion.ConvertFromBaseunit(1), BaseUnit);
                if (converttounit == BaseUnit)
                {
                    return pq;
                }

                return pq.ConvertTo(converttounit);

                // throw new ArgumentException("Physical unit is not convertibel to a " + converttounit.ToString());
            }
        }
    }

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

        public UnitSystem(string aname, UnitPrefixTable anunitprefixes)
            : base(aname)
        {
            this._unitprefixes = anunitprefixes;
        }

        public UnitSystem(string aname, UnitPrefixTable anunitprefixes, BaseUnit[] units)
            : this(aname, anunitprefixes)
        {
            this._baseunits = units;
            foreach (BaseUnit baseunit in this._baseunits)
            {
                Debug.Assert(baseunit.Kind == UnitKind.ukBaseUnit);
                if (baseunit.System != this)
                    baseunit.System = this;
            }
        }

        public UnitSystem(string somename, UnitPrefixTable someunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits)
            : this(somename, someunitprefixes, baseunits)
        {
            this._namedderivedunits = somenamedderivedunits;
            foreach (NamedDerivedUnit namedderivedunit in this._namedderivedunits)
            {
                Debug.Assert(namedderivedunit.Kind == UnitKind.ukDerivedUnit);
                if (namedderivedunit.System != this)
                    namedderivedunit.System = this;
            }
        }

        public UnitSystem(string somename, UnitPrefixTable someunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits, ConvertibleUnit[] someconvertibleunits)
            : this(somename, someunitprefixes, baseunits, somenamedderivedunits)
        {
            this._convertibleunits = someconvertibleunits;

            foreach (ConvertibleUnit convertibleunit in this._convertibleunits)
            {
                Debug.Assert(convertibleunit.Kind == UnitKind.ukConvertibleUnit);
                if (convertibleunit.System != this)
                    convertibleunit.System = this;
            }
        }

        public IPhysicalUnit UnitFromName(string unitname)
        {
            foreach (BaseUnit u in this.BaseUnits)
            {
                if (u.Name == unitname)
                    return u;
            }
            foreach (NamedDerivedUnit u in this.NamedDerivedUnits)
            {
                if (u.Name == unitname)
                    return u;
            }
            foreach (ConvertibleUnit u in this.ConvertibleUnits)
            {
                if (u.Name == unitname)
                    return u;
            }

            return null;
        }

        public IPhysicalUnit UnitFromSymbol(string symbolname)
        {
            if (this.BaseUnits != null)
            {
                foreach (BaseUnit u in this.BaseUnits)
                {
                    if (u.Symbol == symbolname)
                        return u;
                }
            }
            if (this.NamedDerivedUnits != null)
            {
                foreach (NamedDerivedUnit u in this.NamedDerivedUnits)
                {
                    if (u.Symbol == symbolname)
                        return u;
                }
            }
            if (this.ConvertibleUnits != null)
            {
                foreach (ConvertibleUnit u in this.ConvertibleUnits)
                {
                    if (u.Symbol == symbolname)
                        return u;
                }
            }
            return null;
        }

        public IPhysicalQuantity ScaledUnitFromSymbol(string scaledsymbolname)
        {
            double value = 1;
            IPhysicalUnit unit = UnitFromSymbol(scaledsymbolname);
            if (unit == null)
            {   /* Check for prefixed unit */
                sbyte scaleexponent;
                char prefixchar = scaledsymbolname[0];
                if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleexponent))
                {
                    value = Math.Pow(10, scaleexponent);
                    unit = UnitFromSymbol(scaledsymbolname.Substring(1));
                }
            }

            if (unit != null)
            {
                return new PhysicalQuantity(value, unit);
            }

            return null;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit)
        {
            Debug.Assert(physicalquantity.Unit != null);
            Debug.Assert(converttounit != null);

            if (physicalquantity.Unit == converttounit)
            {
                return physicalquantity;
            }
            else
            {
                if ((physicalquantity.Unit.System == this) && (converttounit.System == this))
                {   /* Intra unit system conversion */
                    if (converttounit.Kind == UnitKind.ukBaseUnit)
                    {
                        if (physicalquantity.Unit.Kind == UnitKind.ukConvertibleUnit)
                        {
                            IConvertibleUnit icu = (IConvertibleUnit)physicalquantity.Unit;
                            if (icu.BaseUnit == converttounit)
                            {
                                return new PhysicalQuantity(icu.Convertion.ConvertToBaseunit(physicalquantity.Value), converttounit);
                            }
                        }
                    }
                    else if (converttounit.Kind == UnitKind.ukConvertibleUnit)
                    {
                        if (physicalquantity.Unit.Kind == UnitKind.ukBaseUnit)
                        {
                            IConvertibleUnit icu = (IConvertibleUnit)converttounit;
                            if (icu.BaseUnit == physicalquantity.Unit)
                            {
                                return new PhysicalQuantity(icu.Convertion.ConvertFromBaseunit(physicalquantity.Value), converttounit);
                            }
                        }
                    }

                    if (DimensionExponets.Equals(converttounit.Exponents, physicalquantity.Unit.Exponents))
                    {
                        return new PhysicalQuantity(physicalquantity.Value, converttounit);
                    }
                }
                else
                {   /* Inter unit system conversion */
                    UnitSystemConversion usc = Physic.GetUnitSystemConversion(physicalquantity.Unit.System, converttounit.System);
                    if (usc != null)
                    {
                        return usc.ConvertTo(physicalquantity, converttounit);
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
                UnitSystemConversion usc = Physic.GetUnitSystemConversion(physicalquantity.Unit.System, converttounitsystem);
                if (usc != null)
                {
                    return usc.ConvertTo(physicalquantity, converttounitsystem);
                }

                /* Missing unit system conversion from  physicalquantity.Unit.System to ToUnitSystem */
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

        public ValueConvertion[] BaseUnitConversions;

        public UnitSystemConversion(IUnitSystem somebaseunitsystem, IUnitSystem someconvertedunitsystem, ValueConvertion[] somebaseunitconversions)
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
                return IsSameMeasureKind(unit1, unit2);
            }
            return false;
        }

        public static bool IsSameMeasureKind(IEnumerable<sbyte> exponents1, IEnumerable<sbyte> exponents2)
        {
            if (exponents1.Equals(exponents2))
            {
                return true;
            }
            else
            {
                IEnumerator<sbyte> e1e = exponents1.GetEnumerator();
                IEnumerator<sbyte> e2e = exponents2.GetEnumerator();
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

        public static bool IsSameMeasureKind(IUnit unit1, IUnit unit2)
        {
            if (unit1 == unit2)
            {
                return true;
            }
            else
            {
                return IsSameMeasureKind(unit1.Exponents, unit2.Exponents);
            }
        }

        public IPhysicalQuantity Convert(IPhysicalQuantity physicalquantity, bool backwards = false)
        {
            sbyte[] FromUnitExponents = physicalquantity.Unit.Exponents; 

            double convertproduct = 1;
            int NoOfNonZeroExponents = 0;
            int NoOfNonOneExponents = 0;
            int FirstNonZeroExponent = -1;

            sbyte i = 0;
            foreach (sbyte exponent in FromUnitExponents)
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
                    ValueConvertion vc = BaseUnitConversions[i];
                    if (vc != null)
                    {
                        double baseunitconvertedvalue = vc.Convert(1, backwards);
                        double baseunitfactor = Math.Pow(baseunitconvertedvalue, exponent);
                        convertproduct = convertproduct * baseunitfactor;
                    }
                    else
                    {
                        /* throw new ArgumentException("object's physical unit is not convertibel to a " + ConvertedUnitSystem.name + " unit. " + ConvertedUnitSystem.name+ " does not "); */
                        return null;
                    }
                }

                i++;
            }
            double value = physicalquantity.Value * convertproduct;
            IPhysicalUnit unit;
            if ((NoOfNonZeroExponents == 1) && (NoOfNonOneExponents == 0))
            {
                /* BaseUnit */
                unit = (IPhysicalUnit)ConvertedUnitSystem.BaseUnits[FirstNonZeroExponent];
            }
            else
            {
                int namedderivedunitsindex = 0;
                while (!IsSameMeasureKind(ConvertedUnitSystem.NamedDerivedUnits[namedderivedunitsindex].Exponents, FromUnitExponents))
                {
                    namedderivedunitsindex++;
                }

                if (namedderivedunitsindex < ConvertedUnitSystem.NamedDerivedUnits.Length)
                {   /* NamedDerivedUnit */
                    unit = (IPhysicalUnit)ConvertedUnitSystem.NamedDerivedUnits[namedderivedunitsindex];
                }
                else
                {   /* DerivedUnit */
                    unit = new DerivedUnit(ConvertedUnitSystem, FromUnitExponents);
                }
            }
            return new PhysicalQuantity(value, unit);
        }

        public IPhysicalQuantity ConvertFromBaseUnitSystem(IPhysicalQuantity physicalquantity)
        {
            return Convert(physicalquantity, false);
        }

        public IPhysicalQuantity ConvertToBaseUnitSystem(IPhysicalQuantity physicalquantity)
        {
            return Convert(physicalquantity, true);
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

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit)
        {
            IPhysicalQuantity pq = this.ConvertTo(physicalquantity, converttounit.System);
            if (!IsSameUnit(pq.Unit, converttounit))
            {
                pq = null;
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

        public PhysicalQuantity()
        {
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

                throw new ArgumentException("object's physical unit " + temp.Unit.ToString() + " is not convertible to a " + this.Unit.ToString());
            }

            throw new ArgumentException("object is not a IPhysicalQuantity");
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.Value.ToString(format, formatProvider) + " " + this.Unit.ToString();
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// [whitespace] [number] [whitespace]  [prefix] [unitsymbol] [whitespace]
        /// </summary>
        public static IPhysicalQuantity Parse(string s, System.Globalization.NumberStyles styles, IFormatProvider provider)
        {
            IPhysicalQuantity temp;
            string[] Strings = s.Trim().Split(' ');
            string ValueStr = Strings[0];
            string UnitStr  = Strings[1];

            temp = Physic.ScaledUnitFromSymbol(UnitStr);
            temp.Value *= Double.Parse(ValueStr, styles, provider);

            return temp;
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// [whitespace] [number] [whitespace]  [prefix] [unitsymbol] [whitespace]
        /// </summary>
        public static IPhysicalQuantity Parse(string s)
        {
            IPhysicalQuantity temp;
            string[] Strings = s.Trim().Split(' ');
            string ValueStr = Strings[0];
            string UnitStr = Strings[1];

            temp = Physic.ScaledUnitFromSymbol(UnitStr);
            temp.Value *= Double.Parse(ValueStr);

            return temp;
        }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            if (this.Unit == converttounit)
            {
                return this;
            }
            Debug.Assert(this.Unit != null);
            Debug.Assert(converttounit != null);

            if (this.Unit.System != null)
            {
                IPhysicalQuantity quantity = this.Unit.System.ConvertTo(this as IPhysicalQuantity, converttounit);
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

            if (this.Unit.System != null)
            {
                IPhysicalQuantity quantity = this.Unit.System.ConvertTo(this as IPhysicalQuantity, converttounitsystem);
                return quantity;
            }

            return null;
        }

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
            return this.ValueCompare(other.Value) == 0;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            if (!(obj is IPhysicalQuantity))
            {
                throw new InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object.");
            }
            
            return Equals(obj as IPhysicalQuantity);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() + this.Unit.GetHashCode();
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

        public delegate double CombineValuesFunc(double v1, double v2);
        public delegate IPhysicalUnit CombineUnitsFunc(IPhysicalUnit u1, IPhysicalUnit u2);

        public static PhysicalQuantity CombineValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf)
        {
            if (pq1.Unit != pq2.Unit)
            {
                pq2 = pq2.ConvertTo(pq1.Unit);
                if (pq2 == null)
                {
                    throw new ArgumentException("object's physical unit " + pq2.Unit.ToString()+ "is not convertible to a " + pq1.Unit.ToString());
                }
            }
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pq1.Unit);
        }

        public static PhysicalQuantity CombineUnitsAndValues(IPhysicalQuantity pq1, IPhysicalQuantity pq2, CombineValuesFunc cvf, PhysicalUnit.CombineExponentsFunc cef)
        {
            if (pq2.Unit.System != pq1.Unit.System)
            {   // Must be same unit system
                pq2 = pq2.ConvertTo(pq1.Unit.System);
            }
            if (pq1.Unit.Kind == UnitKind.ukConvertibleUnit)
            {
                IConvertibleUnit pg1_unit = pq1.Unit as IConvertibleUnit;
                pq1 = pq1.ConvertTo(pg1_unit.BaseUnit);
            }
            if (pq2.Unit.Kind == UnitKind.ukConvertibleUnit)
            {
                IConvertibleUnit pg2_unit = pq2.Unit as IConvertibleUnit;
                pq2 = pq2.ConvertTo(pg2_unit.BaseUnit);
            }
            sbyte[] someexponents = new sbyte[Physic.NoOfMeasures];

            for (int i = 0; i < Physic.NoOfMeasures; i++)
            {
                someexponents[i] = cef(pq1.Unit.Exponents[i], pq2.Unit.Exponents[i]);
            }
            PhysicalUnit pu = new DerivedUnit(pq1.Unit.System, someexponents);
            return new PhysicalQuantity(cvf(pq1.Value, pq2.Value), pu);
        }

        public static PhysicalQuantity operator +(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return CombineValues(pq1, pq2, (double v1, double v2) => v1 + v2);
        }

        public static PhysicalQuantity operator -(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return CombineValues(pq1, pq2, (double v1, double v2) => v1 - v2);
        }

        public static PhysicalQuantity operator *(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return CombineUnitsAndValues(pq1, pq2, (double v1, double v2) => v1 * v2, (sbyte e1, sbyte e2) => (sbyte)(e1 + e2));
        }

        public static PhysicalQuantity operator /(PhysicalQuantity pq1, IPhysicalQuantity pq2)
        {
            return CombineUnitsAndValues(pq1, pq2, (double v1, double v2) => v1 / v2, (sbyte e1, sbyte e2) => (sbyte)(e1 - e2));
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
            return pq * d;
        }

        public static PhysicalQuantity operator /(double d, PhysicalQuantity pq)
        {
            return (pq ^ -1) * d;
        }

        public PhysicalQuantity Power(sbyte exponent)
        {
            IPhysicalQuantity pq = this;
            sbyte[] someexponents = new sbyte[Physic.NoOfMeasures];

            for (int i = 0; i < Physic.NoOfMeasures; i++)
            {
                someexponents[i] = (sbyte)(pq.Unit.Exponents[i] * exponent);
            }
            PhysicalUnit pu = new DerivedUnit(pq.Unit.System, someexponents);
            return new PhysicalQuantity(System.Math.Pow(pq.Value, exponent), pu);
        }


        public PhysicalQuantity Root(sbyte exponent)
        {
            IPhysicalQuantity pq = this;
            sbyte[] someexponents = new sbyte[Physic.NoOfMeasures];

            for (int i = 0; i < Physic.NoOfMeasures; i++)
            {
                someexponents[i] = (sbyte)(pq.Unit.Exponents[i] / exponent);
            }
            PhysicalUnit pu = new DerivedUnit(pq.Unit.System, someexponents);
            return new PhysicalQuantity(System.Math.Pow(pq.Value, 1.0/exponent), pu);
        }

        public static PhysicalQuantity operator ^(PhysicalQuantity pq, sbyte exponent)
        {
            return pq.Power(exponent);
        }

        public static PhysicalQuantity operator %(PhysicalQuantity pq, sbyte exponent)
        {
            return pq.Root(exponent);
        }

        public IPhysicalQuantity Add(IPhysicalQuantity pq2)
        {
            return this + pq2;
        }

        public IPhysicalQuantity Subtract(IPhysicalQuantity pq2)
        {
            return this - pq2;
        }

        public IPhysicalQuantity Multiply(IPhysicalQuantity pq2)
        {
            return this * pq2;
        }

        public IPhysicalQuantity Divide(IPhysicalQuantity pq2)
        {
            return this / pq2;
        }

        public IPhysicalQuantity Pow(sbyte exponent)
        {
            return this.Power(exponent);
        }

        public IPhysicalQuantity Rot(sbyte exponent)
        {
            return this.Root(exponent);
        }
      
    }

    #endregion Physical Quantity Classes

    #region Physical Measure Statics Class

    public static partial class Physic
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

        private static readonly BaseUnit[] SI_BaseUnits = new BaseUnit[] {  new BaseUnit(null, (sbyte)MeasureKind.Length, "meter", "m"), 
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Mass, "kilogram", "Kg"), /* kg */
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Time, "second", "s"), 
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                    new BaseUnit(null, (sbyte)MeasureKind.Luminous_intensity, "cadela", "cd") };

        public static readonly UnitSystem SI_Units = new UnitSystem("SI", UnitPrefixes,
                                                                 SI_BaseUnits,
                                                                 new NamedDerivedUnit[] {   new NamedDerivedUnit(SI_Units, "hertz",     "Hz",   new sbyte[] { -1, 0, 0, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "radian",    "rad",  new sbyte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "steradian", "sr",   new sbyte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "newton",    "N",    new sbyte[] { 1, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "pascal",    "Pa",   new sbyte[] { -1, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "joule",     "J",    new sbyte[] { 2, 1, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "watt",      "W",    new sbyte[] { 2, 1, -3, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "coulomb",   "C",    new sbyte[] { 1, 0, 0, 1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "volt",      "V",    new sbyte[] { 2, 1, -3, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "farad",     "F",    new sbyte[] { -2, -1, 4, 2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "ohm",       "Ω",    new sbyte[] { 2, 1, -3, -2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "siemens",   "S",    new sbyte[] { -2, -1, 3, 2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "weber",     "Wb",   new sbyte[] { 2, 1, -2, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "tesla",     "T",    new sbyte[] { 0, 1, -2, -1, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "henry",     "H",    new sbyte[] { 2, 1, -2, -2, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "lumen",     "lm",   new sbyte[] { 0, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "lux",       "lx",   new sbyte[] { -2, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "becquerel", "Bq",   new sbyte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "gray",      "Gy",   new sbyte[] { 2, 0, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "katal",     "kat",  new sbyte[] { 0, 0, -1, 0, 0, 1, 0 }) },
                                                                 new ConvertibleUnit[] { new ConvertibleUnit("gram", "g", SI_BaseUnits[(int)(MeasureKind.Mass)], new ScaledValueConvertion(1000)),  /* [g] = 1000 * [Kg] */
                                                                                         new ConvertibleUnit("Celsius", "°C", SI_BaseUnits[(int)(MeasureKind.Thermodynamic_temperature)], new LinearyValueConvertion(-273.15, 1)) }); /* [°C] = 1 * [K] - 273.15 */
        public static readonly PhysicalUnit dimensionless = new DerivedUnit(SI_Units, new sbyte[] { 0, 0, 0, 0, 0, 0, 0 });

        public static partial class SI 
        {
            /* SI base units */
            public static readonly PhysicalUnit m = (PhysicalUnit)SI_Units.BaseUnits[0];
            public static readonly PhysicalUnit kg = (PhysicalUnit)SI_Units.BaseUnits[1];
            public static readonly PhysicalUnit s = (PhysicalUnit)SI_Units.BaseUnits[2];
            public static readonly PhysicalUnit A = (PhysicalUnit)SI_Units.BaseUnits[3];
            public static readonly PhysicalUnit K = (PhysicalUnit)SI_Units.BaseUnits[4];
            public static readonly PhysicalUnit mol = (PhysicalUnit)SI_Units.BaseUnits[5];
            public static readonly PhysicalUnit cd = (PhysicalUnit)SI_Units.BaseUnits[6];

            /* Named units derived from SI base units */
            public static readonly PhysicalUnit Hz = (PhysicalUnit)SI_Units.NamedDerivedUnits[0];
            public static readonly PhysicalUnit rad = (PhysicalUnit)SI_Units.NamedDerivedUnits[1];
            public static readonly PhysicalUnit sr = (PhysicalUnit)SI_Units.NamedDerivedUnits[2];
            public static readonly PhysicalUnit N = (PhysicalUnit)SI_Units.NamedDerivedUnits[3];
            public static readonly PhysicalUnit Pa = (PhysicalUnit)SI_Units.NamedDerivedUnits[4];
            public static readonly PhysicalUnit J = (PhysicalUnit)SI_Units.NamedDerivedUnits[5];
            public static readonly PhysicalUnit W = (PhysicalUnit)SI_Units.NamedDerivedUnits[6];
            public static readonly PhysicalUnit C = (PhysicalUnit)SI_Units.NamedDerivedUnits[7];
            public static readonly PhysicalUnit V = (PhysicalUnit)SI_Units.NamedDerivedUnits[8];
            public static readonly PhysicalUnit F = (PhysicalUnit)SI_Units.NamedDerivedUnits[9];
            public static readonly PhysicalUnit ohm = (PhysicalUnit)SI_Units.NamedDerivedUnits[10];
            public static readonly PhysicalUnit S = (PhysicalUnit)SI_Units.NamedDerivedUnits[11];
            public static readonly PhysicalUnit Wb = (PhysicalUnit)SI_Units.NamedDerivedUnits[12];
            public static readonly PhysicalUnit T = (PhysicalUnit)SI_Units.NamedDerivedUnits[13];
            public static readonly PhysicalUnit H = (PhysicalUnit)SI_Units.NamedDerivedUnits[14];
            public static readonly PhysicalUnit lm = (PhysicalUnit)SI_Units.NamedDerivedUnits[15];
            public static readonly PhysicalUnit lx = (PhysicalUnit)SI_Units.NamedDerivedUnits[16];
            public static readonly PhysicalUnit Bq = (PhysicalUnit)SI_Units.NamedDerivedUnits[17];
            public static readonly PhysicalUnit Gy = (PhysicalUnit)SI_Units.NamedDerivedUnits[18];
            public static readonly PhysicalUnit kat = (PhysicalUnit)SI_Units.NamedDerivedUnits[19];

            /* Convertible units */
            public static readonly PhysicalUnit g = (PhysicalUnit)SI_Units.ConvertibleUnits[0];
            public static readonly PhysicalUnit Ce = (PhysicalUnit)SI_Units.ConvertibleUnits[1];
        }

        public static readonly UnitSystem CGS_Units = new UnitSystem("CGS", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(CGS_Units, (sbyte)MeasureKind.Length, "centimeter", "cm"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Mass, "gram", "g"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Time, "second", "s"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(CGS_Units, (sbyte)MeasureKind.Luminous_intensity, "cadela", "cd")});

        public static readonly UnitSystem MGD_Units = new UnitSystem("MGD", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGD_Units, (sbyte)MeasureKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGD_Units, (sbyte)MeasureKind.Time, "second", "s"), */
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Time, "day", "d"),
                                                                                /*  new BaseUnit(MGD_Units, "moment", "ø"), */

                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Luminous_intensity, "cadela", "cd") } );

        public static readonly UnitSystem MGM_Units = new UnitSystem("MGM", UnitPrefixes,
                                                                  new BaseUnit[] {  new BaseUnit(MGD_Units, (sbyte)MeasureKind.Length, "meter", "m"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Mass, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGD_Units, (sbyte)MeasureKind.Time, "second", "s"), */
                                                                                /*  new BaseUnit(MGD_Units, (sbyte)MeasureKind.Time, "day", "d"), */
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Time, "moment", "ø"), 

                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Electric_current, "ampere", "A"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Thermodynamic_temperature, "kelvin", "K"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Amount_of_substance, "mol", "mol"), 
                                                                                    new BaseUnit(MGD_Units, (sbyte)MeasureKind.Luminous_intensity, "cadela", "cd") } );


        public static UnitSystem[] UnitSystems = new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units };


        public static readonly UnitSystemConversion SItoCGSConversion = new UnitSystemConversion(SI_Units, CGS_Units, new ValueConvertion[] { new ScaledValueConvertion(100),                /* 1 m      <SI> = 100 cm        <CGS>  */
                                                                                                                                     new ScaledValueConvertion(1000),               /* 1 Kg     <SI> = 1000 g        <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 s      <SI> = 1 s           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 A      <SI> = 1 A           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 K      <SI> = 1 K           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 mol    <SI> = 1 mol         <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 cadela <SI> = 1 cadela      <CGS>  */
                                                                                                                                    });

        public static readonly UnitSystemConversion SItoMGDConversion = new UnitSystemConversion(SI_Units, MGD_Units, new ValueConvertion[] { new IdentityValueConvertion(),                 /* 1 m      <SI> = 1 m           <MGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 Kg     <SI> = 1 Kg          <MGD>  */
                                                                                                                                     new ScaledValueConvertion(1/(24*60*60)),       /* 1 s      <SI> = 1/86400 d     <MGD>  */
                                                                                                                                  /* new ScaledValueConvertion(10000/(24*60*60)),   /* 1 s      <SI> = 10000/86400 ø <MGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 A      <SI> = 1 A           <MGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 K      <SI> = 1 K           <MGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 mol    <SI> = 1 mol         <MGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 cadela <SI> = 1 cadela      <MGD>  */
                                                                                                                                   });

        public static readonly UnitSystemConversion MGDtoMGMConversion = new UnitSystemConversion(MGD_Units, MGM_Units, new ValueConvertion[] { new IdentityValueConvertion(),               /* 1 m      <MGD> = 1 m           <MGM>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 Kg     <MGD> = 1 Kg          <MGM>  */
                                                                                                                                       new ScaledValueConvertion(10000),            /* 1 d      <MGD> = 10000 ø       <MGM>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 A      <MGD> = 1 A           <MGM>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 K      <MGD> = 1 K           <MGM>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 mol    <MGD> = 1 mol         <MGM>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 cadela <MGD> = 1 cadela      <MGM>  */
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

            /* To do: Missing unit system conversion from  SomeUnitSystem to SomeOtherUnitSystem */
            return null;
        }

        public static IUnit UnitFromSymbol(string symbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IUnit unit = us.UnitFromSymbol(symbolstr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalQuantity ScaledUnitFromSymbol(string scaledsymbolstr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalQuantity scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }
    }

    #endregion Physical Unit System Statics

}
