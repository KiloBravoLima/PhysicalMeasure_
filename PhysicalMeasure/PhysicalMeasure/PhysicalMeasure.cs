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
        length,
        mass,
        time,
        electric_current,
        thermodynamic_temperature,
        amount_of_substance,
        luminous_intensity
    }

    public enum unitkind { ukBaseUnit, ukDerivedUnit, ukConvertabelUnit };

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
        unitkind Kind { get; }
        sbyte[] Exponents { get; }
    }

    public interface IBaseUnit : IUnit
    {
        sbyte BaseUnitNumber { get; }
    }

    public interface IDerivedUnit : IUnit
    {
    }

    public interface INamedDerivedUnit : IDerivedUnit
    {
    }

    public interface IConvertabelUnit : IUnit
    {
        IPhysicalUnit BaseUnit { get; }
        IValueConvertion Convertion { get; }
    }

    public interface ISystemUnit : ISystemItem, IUnit
    {
    }

    public interface IPhysicalUnit : ISystemUnit, IEquatable<IPhysicalUnit> /* ? */   /*  : <BaseUnit | DerivedUnit | ConvertabelUnit>  */
    {
        IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit);
    }

    public interface IValueConvertion
    {
        double Convert(double value, bool backwards = false);
        double ConvertFromBaseunit(double value);
        double ConvertToBaseunit(double value);
    }

    public interface IPhysicalQuantity : IComparable, IFormattable, IEquatable<IPhysicalQuantity>
    {
        double Value { get; set; }
        IPhysicalUnit Unit { get; set; }

        IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit);
    }

    public interface IUnitSystem : INamed
    {
        IBaseUnit[] BaseUnits { get; }
        INamedDerivedUnit[] NamedDerivedUnits { get; }
        IConvertabelUnit[] ConvertabelUnits { get; }

        IPhysicalUnit UnitFromName(string UnitName);
        IPhysicalUnit UnitFromSymbol(string SymbolName);
        IPhysicalQuantity ScaledUnitFromSymbol(string ScaledUnitName);

        IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IPhysicalUnit converttounit);
    }

    public interface IUnitPrefix : INamed
    {
        char PrefixChar { get; }
        sbyte PrefixExponent { get; } 
    }

    public interface IUnitPrefixTabel
    {
        IUnitPrefix[] UnitPrefixes { get; }

        bool GetExponentFromPrefixChar(char Somescalechar, out sbyte exponent); 
    }

    #endregion Physical Measure Interfaces

    #region Physical Unit prefix Classes

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        protected char _prefixchar;
        protected sbyte _prefixexponent;

        public sbyte PrefixExponent { get { return _prefixexponent; } }
        public char PrefixChar { get { return _prefixchar; } }

        public UnitPrefix(string aname, char aprefixchar, sbyte aprefixexponent)
            : base(aname)
        {
            this._prefixchar = aprefixchar;
            this._prefixexponent = aprefixexponent;
        }
    }

    public class UnitPrefixTabel : IUnitPrefixTabel
    {
        protected UnitPrefix[] _unitprefixes;

        public IUnitPrefix[] UnitPrefixes { get { return _unitprefixes; } }

        public UnitPrefixTabel(UnitPrefix[] anunitprefixes)
        {
            this._unitprefixes = anunitprefixes;
        }

        public bool GetExponentFromPrefixChar(char Someprefixchar, out sbyte exponent) 
        {
            exponent = 0;
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.PrefixChar == Someprefixchar)
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
        public double Offset;
        public double Scale;

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
        protected string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public NamedObject(string aname)
        {
            this.Name = aname;
        }
    }

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        protected string _symbol;
        public string Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbol(string aname, string asymbol)
            : base(aname)
        {
            this.Symbol = asymbol;
        }
    }

    public class SystemObject : ISystemItem
    {
        protected IUnitSystem _system;
        public IUnitSystem System { get { return _system; } set { _system = value; } }

        public SystemObject(IUnitSystem asystem = null)
        {
            this.System = asystem;
        }
    }

    public class NamedSystemObject : SystemObject
    {
        protected string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public NamedSystemObject(IUnitSystem asystem, string aname)
            : base(asystem)
        {
            this.Name = aname;
        }

        public NamedSystemObject(string aname)
            : this(null, aname)
        {
        }
    }

    public class NamedSymbolSystemObject : NamedSystemObject, INamedSymbol
    {
        protected string _symbol;
        public string Symbol { get { return _symbol; } set { _symbol = value; } }

        public NamedSymbolSystemObject(IUnitSystem asystem, string aname, string asymbol)
            : base(asystem, aname)
        {
            this.Symbol = asymbol;
        }

        public NamedSymbolSystemObject(string aname, string asymbol)
            : this(null, aname, asymbol)
        {
        }
    }

    public abstract class PhysicalUnit : SystemObject, IPhysicalUnit /* <BaseUnit | DerivedUnit | ConvertabelUnit> */
    {
        public PhysicalUnit(IUnitSystem asystem)
            : base(asystem)
        {
        }

        public abstract unitkind Kind { get; }
        public abstract sbyte[] Exponents { get; }

        public IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
        {
            if (converttounit == this)
            {
                return new PhysicalQuantity(1, this);
            }
            else
            {
                return this.System.ConvertTo(new PhysicalQuantity(1, this), converttounit);
            }
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

    }

    public class BaseUnit : PhysicalUnit, INamedSymbol, IBaseUnit
    {
        public NamedSymbol NamedSymbol;

        public string Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public string Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        protected sbyte _baseunitnumber;
        public sbyte BaseUnitNumber { get { return _baseunitnumber; } }

        public override unitkind Kind { get { return unitkind.ukBaseUnit; } }

        public override sbyte[] Exponents { get { sbyte[] tempexponents = new sbyte[_system.BaseUnits.Length]; tempexponents[_baseunitnumber] = 1; return tempexponents; } } 

        public BaseUnit(IUnitSystem asystem, NamedSymbol anamedsymbol)
            : base(asystem)
        {
            this.NamedSymbol = anamedsymbol;
        }

        public BaseUnit(IUnitSystem asystem, string aname, string asymbol)
            : this(asystem, new NamedSymbol(aname, asymbol))
        {
        }

        public BaseUnit(string aname, string asymbol)
            : this(null, aname, asymbol)
        {
        }
    }

    public class DerivedUnit : PhysicalUnit, IDerivedUnit
    {
        public sbyte[] _exponents;

        public override unitkind Kind { get { return unitkind.ukDerivedUnit; } }

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
        public NamedSymbol NamedSymbol;

        public string Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public string Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        public NamedDerivedUnit(UnitSystem asystem, NamedSymbol anamedsymbol, sbyte[] someexponents = null)
            : base(asystem, someexponents)
        {
            this.NamedSymbol = anamedsymbol;
        }

        public NamedDerivedUnit(UnitSystem asystem, string aname, string asymbol, sbyte[] someexponents = null)
            : this(asystem, new NamedSymbol(aname, asymbol), someexponents)
        {
        }
    }

    public class ConvertabelUnit : PhysicalUnit, INamedSymbol, IConvertabelUnit
    {
        public NamedSymbol NamedSymbol;

        public string Name { get { return this.NamedSymbol.Name; } set { this.NamedSymbol.Name = value; } }
        public string Symbol { get { return this.NamedSymbol.Symbol; } set { this.NamedSymbol.Symbol = value; } }

        protected IPhysicalUnit _baseunit;
        protected IValueConvertion _convertion;

        public IPhysicalUnit BaseUnit { get { return _baseunit; } }
        public IValueConvertion Convertion { get { return _convertion; } }

        public ConvertabelUnit(NamedSymbol anamedsymbol, IPhysicalUnit somebaseunit = null, ValueConvertion someconvertion = null)
            : base(somebaseunit != null ? somebaseunit.System : null)
        {
            this.NamedSymbol = anamedsymbol;
            _baseunit = somebaseunit;
            _convertion = someconvertion;
        }

        public ConvertabelUnit(string aname, string asymbol, IPhysicalUnit somesystemunit = null, ValueConvertion someconvertion = null)
            : this(new NamedSymbol(aname, asymbol), somesystemunit, someconvertion)
        {
        }

        public override unitkind Kind { get { return unitkind.ukConvertabelUnit; } }

        public override sbyte[] Exponents { get { return null; } }

        /** public UnitSystem system { get { return baseunit.system; } / * set { systemunit.system = value; } * / } **/

        public IPhysicalQuantity ConvertTo(IPhysicalUnit converttounit)
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

                // throw new ArgumentException("Physical unit is not converterbel to a " + converttounit.ToString());
            }
        }
    }

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public class UnitSystem : NamedObject, IUnitSystem
    {
        protected UnitPrefixTabel _unitprefixes;
        protected BaseUnit[] _baseunits;
        protected NamedDerivedUnit[] _namedderivedunits;
        protected ConvertabelUnit[] _convertabelunits;

        public IUnitPrefixTabel UnitPrefixes { get { return _unitprefixes; } }
        public IBaseUnit[] BaseUnits { get { return _baseunits; } }
        public INamedDerivedUnit[] NamedDerivedUnits { get { return _namedderivedunits; } }
        public IConvertabelUnit[] ConvertabelUnits { get { return _convertabelunits; } }

        public UnitSystem(string aname, UnitPrefixTabel anunitprefixes)
            : base(aname)
        {
            this._unitprefixes = anunitprefixes;
        }

        public UnitSystem(string aname, UnitPrefixTabel anunitprefixes, BaseUnit[] units)
            : this(aname, anunitprefixes)
        {
            this._baseunits = units;
            foreach (BaseUnit baseunit in this._baseunits)
            {
                Debug.Assert(baseunit.Kind == unitkind.ukBaseUnit);
                if (baseunit.System != this)
                    baseunit.System = this;
            }
        }

        public UnitSystem(string aname, UnitPrefixTabel anunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits)
            : this(aname, anunitprefixes, baseunits)
        {
            this._namedderivedunits = somenamedderivedunits;
            foreach (NamedDerivedUnit namedderivedunit in this._namedderivedunits)
            {
                Debug.Assert(namedderivedunit.Kind == unitkind.ukDerivedUnit);
                if (namedderivedunit.System != this)
                    namedderivedunit.System = this;
            }
        }

        public UnitSystem(string aname, UnitPrefixTabel anunitprefixes, BaseUnit[] baseunits, NamedDerivedUnit[] somenamedderivedunits, ConvertabelUnit[] someconvertabelunits)
            : this(aname, anunitprefixes, baseunits, somenamedderivedunits)
        {
            this._convertabelunits = someconvertabelunits;

            foreach (ConvertabelUnit convertabelunit in this._convertabelunits)
            {
                Debug.Assert(convertabelunit.Kind == unitkind.ukConvertabelUnit);
                if (convertabelunit.System != this)
                    convertabelunit.System = this;
            }
        }

        public IPhysicalUnit UnitFromName(string UnitName)
        {
            foreach (BaseUnit u in this.BaseUnits)
            {
                if (u.Name == UnitName)
                    return u;
            }
            foreach (NamedDerivedUnit u in this.NamedDerivedUnits)
            {
                if (u.Name == UnitName)
                    return u;
            }
            foreach (ConvertabelUnit u in this.ConvertabelUnits)
            {
                if (u.Name == UnitName)
                    return u;
            }

            return null;
        }

        public IPhysicalUnit UnitFromSymbol(string SymbolName)
        {
            if (this.BaseUnits != null)
            {
                foreach (BaseUnit u in this.BaseUnits)
                {
                    if (u.Symbol == SymbolName)
                        return u;
                }
            }
            if (this.NamedDerivedUnits != null)
            {
                foreach (NamedDerivedUnit u in this.NamedDerivedUnits)
                {
                    if (u.Symbol == SymbolName)
                        return u;
                }
            }
            if (this.ConvertabelUnits != null)
            {
                foreach (ConvertabelUnit u in this.ConvertabelUnits)
                {
                    if (u.Symbol == SymbolName)
                        return u;
                }
            }
            return null;
        }

        public IPhysicalQuantity ScaledUnitFromSymbol(string ScaledSymbolName)
        {
            double value = 1;
            IPhysicalUnit unit = UnitFromSymbol(ScaledSymbolName);
            if (unit == null)
            {   /* Check for prefixed unit */
                sbyte scaleexponent;
                char prefixchar = ScaledSymbolName[0];
                if (UnitPrefixes.GetExponentFromPrefixChar(prefixchar, out scaleexponent))
                {
                    value = Math.Pow(10, scaleexponent);
                    unit = UnitFromSymbol(ScaledSymbolName.Substring(1));
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
                    if (converttounit.Kind == unitkind.ukBaseUnit)
                    {
                        if (physicalquantity.Unit.Kind == unitkind.ukConvertabelUnit)
                        {
                            IConvertabelUnit icu = (IConvertabelUnit)physicalquantity.Unit;
                            if (icu.BaseUnit == converttounit)
                            {
                                return new PhysicalQuantity(icu.Convertion.ConvertToBaseunit(physicalquantity.Value), converttounit);
                            }
                        }
                    }
                    else if (converttounit.Kind == unitkind.ukConvertabelUnit)
                    {
                        if (physicalquantity.Unit.Kind == unitkind.ukBaseUnit)
                        {
                            IConvertabelUnit icu = (IConvertabelUnit)converttounit;
                            if (icu.BaseUnit == physicalquantity.Unit)
                            {
                                return new PhysicalQuantity(icu.Convertion.ConvertFromBaseunit(physicalquantity.Value), converttounit);
                            }
                        }
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
    }

    #endregion Physical Unit System Classes

    #region Physical Unit System Conversion Classes

    public class UnitSystemConversion
    {
        public IUnitSystem BaseUnitSystem;
        public IUnitSystem ConvertedUnitSystem;

        public ValueConvertion[] BaseUnitConversions;

        public UnitSystemConversion(IUnitSystem SomeBaseUnitSystem, IUnitSystem SomeConvertedUnitSystem, ValueConvertion[] SomeBaseUnitConversions)
        {
            this.BaseUnitSystem = SomeBaseUnitSystem;
            this.ConvertedUnitSystem = SomeConvertedUnitSystem;
            this.BaseUnitConversions = SomeBaseUnitConversions;
        }

        public bool IsSameUnit(IPhysicalUnit SomeUnit, IPhysicalUnit SomeOtherUnit)
        {
            if (SomeUnit == SomeOtherUnit)
            {
                return true;
            }
            else
            if (SomeUnit.System == SomeOtherUnit.System)
            {
                return IsSameMeasureKind(SomeUnit, SomeOtherUnit);
            }
            return false;
        }

        public bool IsSameMeasureKind(IEnumerable<sbyte> SomeExponents, IEnumerable<sbyte> SomeOtherExponents)
        {
            if (SomeExponents.Equals(SomeOtherExponents))
            {
                return true;
            }
            else
            {
                IEnumerator<sbyte> see = SomeExponents.GetEnumerator();
                IEnumerator<sbyte> soee = SomeOtherExponents.GetEnumerator();
                bool seeHasElement = see.MoveNext();
                bool soeeHasElement = soee.MoveNext();
                while (seeHasElement && soeeHasElement)
                {
                    if (see.Current != soee.Current)
                    {
                        return false;
                    }
                    seeHasElement = see.MoveNext();
                    soeeHasElement = soee.MoveNext();
                };

                while (seeHasElement || soeeHasElement)
                {
                    if (   (seeHasElement && see.Current != 0)
                        || (soeeHasElement && soee.Current != 0))
                    {
                        return false;
                    }
                    seeHasElement = see.MoveNext();
                    soeeHasElement = soee.MoveNext();
                };

                return true;
            }
        }

        public bool IsSameMeasureKind(IPhysicalUnit SomeUnit, IPhysicalUnit SomeOtherUnit)
        {
            if (SomeUnit == SomeOtherUnit)
            {
                return true;
            }
            else
            {
                return IsSameMeasureKind(SomeUnit.Exponents, SomeOtherUnit.Exponents);
            }
        }

        public IPhysicalQuantity Convert(IPhysicalQuantity physicalquantity, bool backwards = false)
        {
            sbyte[] FromUnitExponents = physicalquantity.Unit.Exponents; 

            double convertproduct = 1;
            int NoOfMeasures = 0;
            int NoOfNonOneExponentMeasures = 0;
            int FirstNonZeroMeasure = -1;

            sbyte i = 0;
            foreach (sbyte exponent in FromUnitExponents)
            {
                if (exponent != 0)
                {
                    if (FirstNonZeroMeasure == -1)
                    {
                        FirstNonZeroMeasure = i;
                    }
                    NoOfMeasures++;
                    if (exponent != 1)
                    {
                        NoOfNonOneExponentMeasures++;
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
                        /* throw new ArgumentException("object's physical unit is not converterbel to a " + ConvertedUnitSystem.name + " unit. " + ConvertedUnitSystem.name+ " does not "); */
                        return null;
                    }
                }

                i++;
            }
            double value = physicalquantity.Value * convertproduct;
            IPhysicalUnit unit;
            if ((NoOfMeasures == 1) && (NoOfNonOneExponentMeasures == 0))
            {
                /* BaseUnit */
                unit = (IPhysicalUnit)ConvertedUnitSystem.BaseUnits[FirstNonZeroMeasure];
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

        public IPhysicalQuantity ConvertTo(IPhysicalQuantity physicalquantity, IUnitSystem ToUnitSystem)
        {
            if ((physicalquantity.Unit.System == BaseUnitSystem) && (ToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(physicalquantity);
            }
            else
            if ((physicalquantity.Unit.System == ConvertedUnitSystem) && (ToUnitSystem == BaseUnitSystem))
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
        protected double _value;
        public IPhysicalUnit _unit;

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
            this._value = 0;
            this._unit = null;
        }

        public PhysicalQuantity(double avalue, IPhysicalUnit anunit)
        {
            this._value = avalue;
            this._unit = anunit;
        }

        public PhysicalQuantity(IPhysicalQuantity aphysicalquantity)
        {
            this._value = aphysicalquantity.Value;
            this._unit = aphysicalquantity.Unit;
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is IPhysicalQuantity)
            {
                IPhysicalQuantity temp = (IPhysicalQuantity)obj;

                temp = temp.ConvertTo(this._unit);
                if (temp != null)
                {
                    return _value.CompareTo(temp.Value);
                }

                throw new ArgumentException("object's physical unit is not converterbel to a " + _unit.ToString());
            }

            throw new ArgumentException("object is not a IPhysicalQuantity");
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            return this.Value.ToString(format, provider) + " " + this.Unit.ToString();
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

        public IPhysicalQuantity ConvertTo(IPhysicalUnit unit)
        {
            if (this.Unit == unit)
            {
                return this;
            }
            else
            {
                Debug.Assert(this.Unit != null);
                Debug.Assert(unit != null);

                if (this.Unit.System != null)
                {
                    IPhysicalQuantity quantity = this.Unit.System.ConvertTo(this as IPhysicalQuantity, unit);
                    return quantity;
                }

                return null;
            }
        }

        public bool Equals(IPhysicalQuantity other)
        {
            if (this.Unit == other.Unit)
            {
                return (this.Value == other.Value);
            }
            IPhysicalQuantity pq = other.ConvertTo(this.Unit);
            return (pq != null) && (pq.Value == this.Value);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) return base.Equals(obj);

            if (!(obj is IPhysicalQuantity))
                throw new InvalidCastException("The 'obj' argument is not a IPhysicalQuantity object.");
            else
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

        public static UnitPrefixTabel UnitPrefixes = new UnitPrefixTabel(new UnitPrefix[] { new UnitPrefix("yotta", 'Y', 24),
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

        private static BaseUnit[] SI_BaseUnits =  new BaseUnit[] {  new BaseUnit(null, "meter", "m"), 
                                                                    new BaseUnit(null, "kilogram", "Kg"), /* kg */
                                                                    new BaseUnit(null, "second", "s"), 
                                                                    new BaseUnit(null, "ampere", "A"), 
                                                                    new BaseUnit(null, "kelvin", "K"), 
                                                                    new BaseUnit(null, "mol", "mol"), 
                                                                    new BaseUnit(null, "cadela", "cd") };

        public static UnitSystem SI_Units = new UnitSystem("SI", UnitPrefixes,
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
                                                                                         /* new NamedDerivedUnit(SI_Units, "Celsius",   "°C",   new sbyte[] { 0, 0, 0, 0, 1, 0, 0 }), */
                                                                                            new NamedDerivedUnit(SI_Units, "lumen",     "lm",   new sbyte[] { 0, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "lux",       "lx",   new sbyte[] { -2, 0, 0, 0, 0, 0, 1 }),
                                                                                            new NamedDerivedUnit(SI_Units, "becquerel", "Bq",   new sbyte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "gray",      "Gy",   new sbyte[] { 2, 0, -2, 0, 0, 0, 0 }),
                                                                                            new NamedDerivedUnit(SI_Units, "katal",     "kat",  new sbyte[] { 0, 0, -1, 0, 0, 1, 0 }) },
                                                                 new ConvertabelUnit[] { new ConvertabelUnit("gram", "g", SI_BaseUnits[(int)(MeasureKind.mass)], new ScaledValueConvertion(1000)),
                                                                                         new ConvertabelUnit("Celsius", "°C", SI_BaseUnits[(int)(MeasureKind.thermodynamic_temperature)], new LinearyValueConvertion(273.15, 1)) });

        public static UnitSystem CGS_Units = new UnitSystem("CGS", UnitPrefixes,
                                                                   new BaseUnit[] { new BaseUnit(CGS_Units, "centimeter", "cm"), 
                                                                                    new BaseUnit(CGS_Units, "gram", "g"), 
                                                                                    new BaseUnit(CGS_Units, "second", "s"),
                                                                                    new BaseUnit(CGS_Units, "ampere", "A"),
                                                                                    new BaseUnit(CGS_Units, "kelvin", "K"),
                                                                                    new BaseUnit(CGS_Units, "mol", "mol"), 
                                                                                    new BaseUnit(CGS_Units, "cadela", "cd") });

        public static UnitSystem MGD_Units = new UnitSystem("MGD", UnitPrefixes,
                                                                   new BaseUnit[] { new BaseUnit(MGD_Units, "meter", "m"), 
                                                                                    new BaseUnit(MGD_Units, "kilogram", "Kg"), 

                                                                                    new BaseUnit(MGD_Units, "day", "d"),
                                                                                /*  new BaseUnit(MGD_Units, "moment", "ø"), */

                                                                                    new BaseUnit(MGD_Units, "ampere", "A"),
                                                                                    new BaseUnit(MGD_Units, "kelvin", "K"),
                                                                                    new BaseUnit(MGD_Units, "mol", "mol"), 
                                                                                    new BaseUnit(MGD_Units, "cadela", "cd") });

        public static UnitSystem MGM_Units = new UnitSystem("MGM", UnitPrefixes,
                                                                   new BaseUnit[] { new BaseUnit(MGM_Units, "meter", "m"), 
                                                                                    new BaseUnit(MGM_Units, "kilogram", "Kg"), 

                                                                                /*  new BaseUnit(MGM_Units, "day", "d"),  */
                                                                                    new BaseUnit(MGM_Units, "moment", "ø"),

                                                                                    new BaseUnit(MGM_Units, "ampere", "A"),
                                                                                    new BaseUnit(MGM_Units, "kelvin", "K"),
                                                                                    new BaseUnit(MGM_Units, "mol", "mol"), 
                                                                                    new BaseUnit(MGM_Units, "cadela", "cd") });

        public static UnitSystem[] UnitSystems = new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units };


        public static UnitSystemConversion SItoCGSConversion = new UnitSystemConversion(SI_Units, CGS_Units, new ValueConvertion[] { new ScaledValueConvertion(100),                /* 1 m      <SI> = 100 cm        <CGS>  */
                                                                                                                                     new ScaledValueConvertion(1000),               /* 1 Kg     <SI> = 1000 g        <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 s      <SI> = 1 s           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 A      <SI> = 1 A           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 K      <SI> = 1 K           <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 mol    <SI> = 1 mol         <CGS>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 cadela <SI> = 1 cadela      <CGS>  */
                                                                                                                                    });

        public static UnitSystemConversion SItoMGDConversion = new UnitSystemConversion(SI_Units, MGD_Units, new ValueConvertion[] { new IdentityValueConvertion(),                 /* 1 m      <SI> = 1 m           <KGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 Kg     <SI> = 1 Kg          <KGD>  */
                                                                                                                                     new ScaledValueConvertion(1/(24*60*60)),       /* 1 s      <SI> = 1/86400 d     <KGD>  */
                                                                                                                                  /* new ScaledValueConvertion(10000/(24*60*60)),   /* 1 s      <SI> = 10000/86400 ø <KGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 A      <SI> = 1 A           <KGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 K      <SI> = 1 K           <KGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 mol    <SI> = 1 mol         <KGD>  */
                                                                                                                                     new IdentityValueConvertion(),                 /* 1 cadela <SI> = 1 cadela      <KGD>  */
                                                                                                                                   });

        public static UnitSystemConversion MGDtoMGMConversion = new UnitSystemConversion(MGD_Units, MGM_Units, new ValueConvertion[] { new IdentityValueConvertion(),               /* 1 m      <SI> = 1 m           <KGD>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 Kg     <SI> = 1 Kg          <KGD>  */
                                                                                                                                       new ScaledValueConvertion(10000),            /* 1 d      <SI> = 10000 ø       <KGD>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 A      <SI> = 1 A           <KGD>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 K      <SI> = 1 K           <KGD>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 mol    <SI> = 1 mol         <KGD>  */
                                                                                                                                       new IdentityValueConvertion(),               /* 1 cadela <SI> = 1 cadela      <KGD>  */
                                                                                                                                     });


        public static UnitSystemConversion[] UnitSystemConversions = new UnitSystemConversion[] { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion };

        public static UnitSystemConversion GetUnitSystemConversion(IUnitSystem SomeUnitSystem, IUnitSystem SomeOtherUnitSystem)
        {
            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                if ((usc.BaseUnitSystem == SomeUnitSystem && usc.ConvertedUnitSystem == SomeOtherUnitSystem)
                    || (usc.BaseUnitSystem == SomeOtherUnitSystem && usc.ConvertedUnitSystem == SomeUnitSystem))
                {
                    return usc;
                }
            }
            return null;
        }

        public static IUnit UnitFromSymbol(string SymbolStr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IUnit unit = us.UnitFromSymbol(SymbolStr);
                if (unit != null)
                {
                    return unit;
                }
            }
            return null;
        }

        public static IPhysicalQuantity ScaledUnitFromSymbol(string ScaledSymbolStr)
        {
            foreach (UnitSystem us in UnitSystems)
            {
                IPhysicalQuantity scaledunit = us.ScaledUnitFromSymbol(ScaledSymbolStr);
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
