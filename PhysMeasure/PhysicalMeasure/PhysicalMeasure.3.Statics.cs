/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;

namespace PhysicalMeasure
{
    #region Physical Measure Static Classes

    public static class Prefixes
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


        public static readonly UnitPrefixTable PhysicsUnitPrefixes = new UnitPrefixTable(new UnitPrefix[] {new UnitPrefix(PhysicsUnitPrefixes, "yotta", 'Y', 24),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "zetta", 'Z', 21),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "exa",   'E', 18),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "peta",  'P', 15),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "tera",  'T', 12),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "giga",  'G', 9),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "mega",  'M', 6),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "kilo",  'K', 3),   /* k */
                                                                                       /* extra */  new UnitPrefix(PhysicsUnitPrefixes, "kilo",  'k', 3),   /* k */
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "hecto", 'H', 2),   /* h */
                                                                                       /* extra */  new UnitPrefix(PhysicsUnitPrefixes, "hecto", 'h', 2),   /* h */
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "deca",  'D', 1),   /* da */
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "deci",  'd', -1),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "centi", 'c', -2),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "milli", 'm', -3),
                                                                                                    // new UnitPrefix(UnitPrefixes, "micro", 'μ', -6), // '\0x03BC' (Char)956  
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "micro", 'µ', -6),  // ANSI '\0x00B5' (Char)181   
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "nano",  'n', -9),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "pico",  'p', -12),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "femto", 'f', -15),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "atto",  'a', -18),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "zepto", 'z', -21),
                                                                                                    new UnitPrefix(PhysicsUnitPrefixes, "yocto", 'y', -24) });

        /* SI unit prefixes */
        public static readonly UnitPrefix Y =  (UnitPrefix)PhysicsUnitPrefixes['Y'];
        public static readonly UnitPrefix Z =  (UnitPrefix)PhysicsUnitPrefixes['Z'];
        public static readonly UnitPrefix E =  (UnitPrefix)PhysicsUnitPrefixes['E'];
        public static readonly UnitPrefix P =  (UnitPrefix)PhysicsUnitPrefixes['P'];
        public static readonly UnitPrefix T =  (UnitPrefix)PhysicsUnitPrefixes['T'];
        public static readonly UnitPrefix G =  (UnitPrefix)PhysicsUnitPrefixes['G'];
        public static readonly UnitPrefix M =  (UnitPrefix)PhysicsUnitPrefixes['M'];
        public static readonly UnitPrefix K =  (UnitPrefix)PhysicsUnitPrefixes['K'];
        public static readonly UnitPrefix k =  (UnitPrefix)PhysicsUnitPrefixes['k'];
        public static readonly UnitPrefix H =  (UnitPrefix)PhysicsUnitPrefixes['H'];
        public static readonly UnitPrefix h =  (UnitPrefix)PhysicsUnitPrefixes['h'];
        public static readonly UnitPrefix D =  (UnitPrefix)PhysicsUnitPrefixes['D'];
        public static readonly UnitPrefix da = (UnitPrefix)PhysicsUnitPrefixes['D']; // Extra
        public static readonly UnitPrefix d =  (UnitPrefix)PhysicsUnitPrefixes['d'];
        public static readonly UnitPrefix c =  (UnitPrefix)PhysicsUnitPrefixes['c'];
        public static readonly UnitPrefix m =  (UnitPrefix)PhysicsUnitPrefixes['m'];
        public static readonly UnitPrefix my = (UnitPrefix)PhysicsUnitPrefixes['µ'];
        public static readonly UnitPrefix n =  (UnitPrefix)PhysicsUnitPrefixes['n'];
        public static readonly UnitPrefix p =  (UnitPrefix)PhysicsUnitPrefixes['p'];
        public static readonly UnitPrefix f =  (UnitPrefix)PhysicsUnitPrefixes['f'];
        public static readonly UnitPrefix a =  (UnitPrefix)PhysicsUnitPrefixes['a'];
        public static readonly UnitPrefix z =  (UnitPrefix)PhysicsUnitPrefixes['z'];
        public static readonly UnitPrefix y =  (UnitPrefix)PhysicsUnitPrefixes['y'];


        /* https://en.wikipedia.org/wiki/Units_of_information
    
        Prefixes:
            Symbol	Prefix	Multiple	    Example
            Ki	    kibi	 2^10, 1024	    kibibyte  (KiB)
            Mi	    mebi	 2^20, 1024^2	mebibyte  (MiB)
            Gi	    gibi	 2^30, 1024^3	gibibyte  (GiB)
            Ti	    tebi	 2^40, 1024^4	tebibyte  (TiB)
            Pi	    pebi	 2^50, 1024^5	pebibyte  (PiB)
            Ei	    exbi	 2^60, 1024^6	exbibyte  (EiB)
            Zi	    zebi	 2^70, 1024^7	zebibyte  (ZiB)
            Yi	    yobi	 2^80, 1024^8	yobibyte  (YiB)
            Ri	    robi	 2^90, 1024^9	robibyte  (RiB)
            Qi	    quebi	2^100, 1024^10	quebibyte (QiB)
        */
        const ushort Ki_ = 1024;
        public static readonly UnitPrefixTable DataUnitPrefixes = new UnitPrefixTable(new UnitPrefix[] {new UnitPrefix(DataUnitPrefixes, "quebi", "Qi", new UnitPrefixExponent(10, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "robi", "Ri", new UnitPrefixExponent(9, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "yobi", "Yi", new UnitPrefixExponent(8, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "zebi", "Zi", new UnitPrefixExponent(7, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "exbi", "Ei", new UnitPrefixExponent(6, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "pebi", "Pi", new UnitPrefixExponent(5, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "tebi", "Ti", new UnitPrefixExponent(4, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "gibi", "Gi", new UnitPrefixExponent(3, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "mebi", "Mi", new UnitPrefixExponent(2, Ki_)),
                                                                                                        new UnitPrefix(DataUnitPrefixes, "kibi", "Ki", new UnitPrefixExponent(1, Ki_)) });

        /* SI unit prefixes */
        public static readonly UnitPrefix Qi = (UnitPrefix)DataUnitPrefixes["Qi"];
        public static readonly UnitPrefix Ri = (UnitPrefix)DataUnitPrefixes["Ri"];
        public static readonly UnitPrefix Yi = (UnitPrefix)DataUnitPrefixes["Yi"];
        public static readonly UnitPrefix Zi = (UnitPrefix)DataUnitPrefixes["Zi"];
        public static readonly UnitPrefix Pi = (UnitPrefix)DataUnitPrefixes["Pi"];
        public static readonly UnitPrefix Ti = (UnitPrefix)DataUnitPrefixes["Ti"];
        public static readonly UnitPrefix Gi = (UnitPrefix)DataUnitPrefixes["Gi"];
        public static readonly UnitPrefix Ki = (UnitPrefix)DataUnitPrefixes["Ki"];
    }

    public static class SI
    {
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

        public static readonly UnitSystem Units
            = new UnitSystem("SI", Prefixes.PhysicsUnitPrefixes,
                (unitsystem) => new BaseUnit[] 
                                    { new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Length, "meter", "m"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Mass, "kilogram", "Kg"), /* kg */
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Time, "second", "s"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ElectricCurrent, "ampere", "A"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ThermodynamicTemperature, "kelvin", "K"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.AmountOfSubstance, "mol", "mol"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.LuminousIntensity, "candela", "cd") },
                (unitsystem) => new NamedDerivedUnit[] 
                                    { new NamedDerivedUnit(unitsystem, "hertz",     "Hz",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "radian",    "rad",  new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "steradian", "sr",   new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "newton",    "N",    new SByte[] { 1, 1, -2, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "pascal",    "Pa",   new SByte[] { -1, 1, -2, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "joule",     "J",    new SByte[] { 2, 1, -2, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "watt",      "W",    new SByte[] { 2, 1, -3, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "coulomb",   "C",    new SByte[] { 1, 0, 0, 1, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "volt",      "V",    new SByte[] { 2, 1, -3, -1, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "farad",     "F",    new SByte[] { -2, -1, 4, 2, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "ohm",       "Ω",    new SByte[] { 2, 1, -3, -2, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "siemens",   "S",    new SByte[] { -2, -1, 3, 2, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "weber",     "Wb",   new SByte[] { 2, 1, -2, -1, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "tesla",     "T",    new SByte[] { 0, 1, -2, -1, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "henry",     "H",    new SByte[] { 2, 1, -2, -2, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "lumen",     "lm",   new SByte[] { 0, 0, 0, 0, 0, 0, 1 }),
                                      new NamedDerivedUnit(unitsystem, "lux",       "lx",   new SByte[] { -2, 0, 0, 0, 0, 0, 1 }),
                                      new NamedDerivedUnit(unitsystem, "becquerel", "Bq",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "gray",      "Gy",   new SByte[] { 2, 0, -2, 0, 0, 0, 0 }),
                                      new NamedDerivedUnit(unitsystem, "katal",     "kat",  new SByte[] { 0, 0, -1, 0, 0, 1, 0 })},
                (unitsystem) => new ConvertibleUnit[] 
                                    { new ConvertibleUnit("gram", "g", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Mass], new ScaledValueConversion(1000)),  /* [g] = 1000 * [Kg] */
                                      new ConvertibleUnit("Celsius", "°C" /* degree sign:  C2 B0  (char)176 '\0x00B0' */ ,
                                                            unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.ThermodynamicTemperature], new LinearValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                      new ConvertibleUnit("liter", "l", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Length].Pow(3), new ScaledValueConversion(1000) ),  /* [l] = 1000 * [m3] */
                                      new ConvertibleUnit("hour", "h", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/3600)) } /* [h] = 1/3600 * [s] */ 
                           );

        public static readonly BaseUnit[] BaseUnits = Units.BaseUnits;
        public static readonly NamedDerivedUnit[] NamedDerivedUnits = Units.NamedDerivedUnits;
        public static readonly ConvertibleUnit[] ConvertibleUnits = Units.ConvertibleUnits;

        public static readonly ConvertibleUnit[] AdditionalTimeUnits
            = new ConvertibleUnit[] {   new ConvertibleUnit("minute", "min", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/60)),                /* [min] = 1/60 * [s] */
                                        new ConvertibleUnit("day", "d", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/86400)),                  /* [d] = 1/86400 * [s] */
                                        new ConvertibleUnit("year", "y", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/(86400 * 365.25))),      /* [y]    = 1/365.25 * [d] */
                                        new ConvertibleUnit("hour", "hour", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/3600)),               /* [hour] = 1/3600 * [s] */
                                        new ConvertibleUnit("day", "day", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/86400)),                /* [day] = 1/86400 * [s] */
                                        new ConvertibleUnit("year", "year", BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/(86400 * 365.25))) }; /* [year]    = 1/365.25 * [d] */

        public static readonly Unit[] MixedTimeUnits
            = new Unit[] { new MixedUnit(SI.AdditionalTimeUnits[2], "y ", new MixedUnit(SI.AdditionalTimeUnits[1], "d ", new MixedUnit(ConvertibleUnits[3], ":", new MixedUnit(AdditionalTimeUnits[0], ":", BaseUnits[2])))) };


        /* SI base units */
        public static readonly BaseUnit m = (BaseUnit)Units["m"];
        public static readonly BaseUnit Kg = (BaseUnit)Units["Kg"];
        public static readonly BaseUnit s = (BaseUnit)Units["s"];
        public static readonly BaseUnit A = (BaseUnit)Units["A"];
        public static readonly BaseUnit K = (BaseUnit)Units["K"];
        public static readonly BaseUnit mol = (BaseUnit)Units["mol"];
        public static readonly BaseUnit cd = (BaseUnit)Units["cd"];

        /* Named units derived from SI base units */
        public static readonly NamedDerivedUnit Hz = (NamedDerivedUnit)Units["Hz"];
        public static readonly NamedDerivedUnit rad = (NamedDerivedUnit)Units["rad"];
        public static readonly NamedDerivedUnit sr = (NamedDerivedUnit)Units["sr"];
        public static readonly NamedDerivedUnit N = (NamedDerivedUnit)Units["N"];
        public static readonly NamedDerivedUnit Pa = (NamedDerivedUnit)Units["Pa"];
        public static readonly NamedDerivedUnit J = (NamedDerivedUnit)Units["J"];
        public static readonly NamedDerivedUnit W = (NamedDerivedUnit)Units["W"];
        public static readonly NamedDerivedUnit C = (NamedDerivedUnit)Units["C"];
        public static readonly NamedDerivedUnit V = (NamedDerivedUnit)Units["V"];
        public static readonly NamedDerivedUnit F = (NamedDerivedUnit)Units["F"];
        public static readonly NamedDerivedUnit Ohm = (NamedDerivedUnit)Units["Ω"];
        public static readonly NamedDerivedUnit S = (NamedDerivedUnit)Units["S"];
        public static readonly NamedDerivedUnit Wb = (NamedDerivedUnit)Units["Wb"];
        public static readonly NamedDerivedUnit T = (NamedDerivedUnit)Units["T"];
        public static readonly NamedDerivedUnit H = (NamedDerivedUnit)Units["H"];
        public static readonly NamedDerivedUnit lm = (NamedDerivedUnit)Units["lm"];
        public static readonly NamedDerivedUnit lx = (NamedDerivedUnit)Units["lx"];
        public static readonly NamedDerivedUnit Bq = (NamedDerivedUnit)Units["Bq"];
        public static readonly NamedDerivedUnit Gy = (NamedDerivedUnit)Units["Gy"];
        public static readonly NamedDerivedUnit kat = (NamedDerivedUnit)Units["kat"];

        /* Convertible units */
        public static readonly ConvertibleUnit g = (ConvertibleUnit)Units["g"];
        public static readonly ConvertibleUnit Ce = (ConvertibleUnit)Units["°C"];
        public static readonly ConvertibleUnit h = (ConvertibleUnit)Units["h"];
        public static readonly ConvertibleUnit l = (ConvertibleUnit)Units["l"];

        /* Additional time units */
        public static readonly ConvertibleUnit min = (ConvertibleUnit)Units["min"];
        public static readonly ConvertibleUnit d   = (ConvertibleUnit)Units["d"];
        public static readonly ConvertibleUnit y   = (ConvertibleUnit)Units["y"];
    }

    public static class Data
    {
        /*  https://en.wikipedia.org/wiki/Units_of_information 
        Data base units
            Name      Symbol   Measure 
            bit       bit       digital data size

        Named scaled units derived from Data base units 
            Name        Symbol  Quantity                            Expression in terms of other units      Expression in terms of Data base units 
            byte        B       digital data size                   8 bit                                   8 bit 
        */

        public static readonly UnitSystem Units
        = new UnitSystem("Data", Prefixes.DataUnitPrefixes,
            (unitsystem) => new BaseUnit[]
                                { new BaseUnit(unitsystem, (SByte)DataBaseUnitKind.DataSize, "bit", "bit") },
            (unitsystem) => null,
            (unitsystem) => new ConvertibleUnit[]
                                { new ConvertibleUnit("byte", "B", unitsystem.BaseUnits[(int)DataBaseUnitKind.DataSize], new ScaledValueConversion(1.0/8)) }  /* [B] = 1/8 * [bit] */
                       );

        /* Data base units */
        public static readonly BaseUnit bit = (BaseUnit)Units["bit"];

        /* Named units derived from Data base units */
        // public static readonly NamedDerivedUnit Byte = (NamedDerivedUnit)Units["byte"];

        /* Convertible Data units */
        public static readonly ConvertibleUnit Byte = (ConvertibleUnit)Units["byte"];

    }

    /***
    public static / * partial * / class Data
    {
        #region Data Measure Constants

        public const int NoOfBaseUnits = (int)DataBaseUnitKind.DataUnitSystem_NoOfBaseUnits; // = 7;

        #endregion Data Measure Constants

        public static readonly Unit dimensionless = Data.Units.Dimensionless;

        public static readonly UnitPrefixTable ByteUnitPrefixes = Data.Units.UnitPrefixes;

        // public static UnitSystemStack CurrentUnitSystems = new UnitSystemStack();
    }

    public static / * partial * / class Physics
    {
        #region Physical Measure Constants

        public const int NoOfBaseUnits = (int)PhysicalBaseUnitKind.PhysicalUnitSystem_NoOfBaseUnits; // = 7;

        #endregion Physical Measure Constants

        public static readonly Unit dimensionless = SI.Units.Dimensionless;

        public static readonly UnitPrefixTable SiUnitPrefixes = SI.Units.UnitPrefixes;

        public static UnitSystemStack CurrentUnitSystems = new UnitSystemStack();
    }
    ***/

    public static /* partial */ class Global
    {
        #region Physical Measure Constants

        public const int PhysicsNoOfBaseUnits = (int)PhysicalBaseUnitKind.PhysicalUnitSystem_NoOfBaseUnits; // = 7;
        public static readonly UnitPrefixTable SiUnitPrefixes = SI.Units.UnitPrefixes;

        #endregion Physical Measure Constants

        #region Data Measure Constants

        public const int DataNoOfBaseUnits = (int)DataBaseUnitKind.DataUnitSystem_NoOfBaseUnits; // = 7;
        public static readonly UnitPrefixTable ByteUnitPrefixes = Data.Units.UnitPrefixes;

        #endregion Data Measure Constants

        public static readonly Unit dimensionless = SI.Units.Dimensionless;

        public static UnitSystemStack CurrentUnitSystems = new UnitSystemStack();

        /**
        public static void SetDefaultUnitSystems()
        {
            CurrentUnitSystems.Use(Data.Units);
            CurrentUnitSystems.Use(SI.Units);
        }
        **/

        public static IUnitSystem UnitSystemFromName(String systemValueStr)
        {
            // Look for a used (eventually user defined) unit system with specified name.
            IUnitSystem us = CurrentUnitSystems.UnitSystemFromName(systemValueStr);
            if (us == null)
            {   // Not a unit system from used (eventually user defined) unit systems; Try look for a unit system from PhysicalMeasure
                // Look for a unit system with specified name from PhysicalMeasure.
                // us = UnitSystems.Systems.UnitSystemFromName(systemValueStr);
            }
            return us;
        }

        public static Unit UnitFromName(String unitName)
        {
            // Look for a (eventually user defined) unit with specified name.
            Unit sus = CurrentUnitSystems.UnitFromName(unitName);
            if (sus == null)
            {   // Not a unit system from used (eventually user defined) unit systems; Try look for a unit from PhysicalMeasure
                // Look for a unit with specified name from PhysicalMeasure.
                // us = UnitSystems.Systems.UnitFromName(unitName);
            }
            return sus;
        }

        public static Unit UnitFromSymbol(String unitSymbol)
        {
            // Look for a (eventually user defined) unit with specified symbol.
            Unit sus = CurrentUnitSystems.UnitFromSymbol(unitSymbol);
            if (sus == null)
            {   // Not a unit system from used (eventually user defined) unit systems; Try look for a unit from PhysicalMeasure
                // Look for a unit with specified symbol from PhysicalMeasure.
                // us = UnitSystems.Systems.UnitFromSymbol(unitSymbol);
            }
            return sus;
        }

        public static Unit ScaledUnitFromSymbol(String scaledUnitSymbol)
        {
            // Look for a (eventually user defined) unit with specified symbol.
            Unit sus = CurrentUnitSystems.ScaledUnitFromSymbol(scaledUnitSymbol);
            if (sus == null)
            {   // Not a unit system from used (eventually user defined) unit systems; Try look for a unit from PhysicalMeasure
                // Look for a unit with specified symbol from PhysicalMeasure.
                // us = UnitSystems.Systems.ScaledUnitFromSymbol(scaledUnitSymbol);
            }
            return sus;
        }
    }

    public static class CGS_Units
    {
        public static readonly UnitSystem Units 
            = new UnitSystem("CGS", Prefixes.PhysicsUnitPrefixes,
                             (unitsystem) =>  new BaseUnit[] 
                                { new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Length, "centimeter", "cm"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Mass, "gram", "g"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Time, "second", "s"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ElectricCurrent, "ampere", "A"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ThermodynamicTemperature, "kelvin", "K"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.AmountOfSubstance, "mol", "mol"),
                                  new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.LuminousIntensity, "candela", "cd")});
    }

    public static class MGD_Units
    {
        public static readonly UnitSystem Units 
            = new UnitSystem("MGD", Prefixes.PhysicsUnitPrefixes,
                (unitsystem) => new BaseUnit[] 
                                    { new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Length, "meter", "m"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Mass, "kilogram", "Kg"), /* kg */
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Time, "day", "d"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ElectricCurrent, "ampere", "A"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ThermodynamicTemperature, "kelvin", "K"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.AmountOfSubstance, "mol", "mol"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.LuminousIntensity, "candela", "cd") }
                , null,
                (unitsystem) => new ConvertibleUnit[] 
                                    { new ConvertibleUnit("second", "sec", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(24 * 60 * 60)),  /* [sec]  = 24 * 60 * 60 * [d] */
                                      new ConvertibleUnit("minute", "min", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(24 * 60)),       /* [min]  = 24 * 60 * [d] */
                                      new ConvertibleUnit("hour", "hour", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(24)),             /* [hour] = 24 * [d] */
                                      new ConvertibleUnit("Day", "day", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new IdentityValueConversion()),               /* [day]  = 1 * [d] */
                                      new ConvertibleUnit("Year", "year", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/365.25)),     /* [year] = 1/365.25 * [d] */
                                      new ConvertibleUnit("year", "y", unitsystem.BaseUnits[(int)PhysicalBaseUnitKind.Time], new ScaledValueConversion(1.0/365.25)) });     /* [y]    = 1/365.25 * [d] */
    }

    public static class MGM_Units
    {
        public static readonly UnitSystem Units 
            = new UnitSystem("MGM", Prefixes.PhysicsUnitPrefixes,
                (unitsystem) => new BaseUnit[] 
                                    { new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Length, "meter", "m"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Mass, "kilogram", "Kg"), 
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.Time, "moment", "ø"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ElectricCurrent, "ampere", "A"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.ThermodynamicTemperature, "kelvin", "K"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.AmountOfSubstance, "mol", "mol"),
                                      new BaseUnit(unitsystem, (SByte)PhysicalBaseUnitKind.LuminousIntensity, "candela", "cd") });
    }

    public static /* partial */ class UnitSystems
    {
        // Just commented out for test of usage: public static UnitLookup Systems = new UnitLookup(new UnitSystem[] { SI.Units, CGS_Units.Units, MGD_Units.Units, MGM_Units.Units });

        public static readonly UnitSystemConversion SItoCGSConversion
            = new UnitSystemConversion(SI.Units, CGS_Units.Units,
                new ValueConversion[] { new ScaledValueConversion(100),       /* 1 m       <SI> = 100 cm        <CGS>  */
                                        new ScaledValueConversion(1000),      /* 1 Kg      <SI> = 1000 g        <CGS>  */
                                        new IdentityValueConversion(),        /* 1 s       <SI> = 1 s           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 A       <SI> = 1 A           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 K       <SI> = 1 K           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 mol     <SI> = 1 mol         <CGS>  */
                                        new IdentityValueConversion(),        /* 1 candela <SI> = 1 candela     <CGS>  */
                                    });

        public static readonly UnitSystemConversion SItoMGDConversion 
            = new UnitSystemConversion(SI.Units, MGD_Units.Units, 
                new ValueConversion[] {new IdentityValueConversion(),                   /* 1 m       <SI> = 1 m           <MGD>  */
                                       new IdentityValueConversion(),                   /* 1 Kg      <SI> = 1 Kg          <MGD>  */
                                       new ScaledValueConversion(1.0/(24*60*60)),       /* 1 s       <SI> = 1/86400 d     <MGD>  */
                                       /* new ScaledValueConversion(10000/(24*60*60)),  /* 1 s       <SI> = 10000/86400 ø <MGD>  */
                                       new IdentityValueConversion(),                   /* 1 A       <SI> = 1 A           <MGD>  */
                                       new IdentityValueConversion(),                   /* 1 K       <SI> = 1 K           <MGD>  */
                                       new IdentityValueConversion(),                   /* 1 mol     <SI> = 1 mol         <MGD>  */
                                       new IdentityValueConversion(),                   /* 1 candela <SI> = 1 candela     <MGD>  */
                                     });

        public static readonly UnitSystemConversion MGDtoMGMConversion = 
            new UnitSystemConversion(MGD_Units.Units, MGM_Units.Units, 
                new ValueConversion[] {new IdentityValueConversion(),      /* 1 m       <MGD> = 1 m           <MGM>  */
                                       new IdentityValueConversion(),      /* 1 Kg      <MGD> = 1 Kg          <MGM>  */
                                       new ScaledValueConversion(10000),   /* 1 d       <MGD> = 10000 ø       <MGM>  */
                                       new IdentityValueConversion(),      /* 1 A       <MGD> = 1 A           <MGM>  */
                                       new IdentityValueConversion(),      /* 1 K       <MGD> = 1 K           <MGM>  */
                                       new IdentityValueConversion(),      /* 1 mol     <MGD> = 1 mol         <MGM>  */
                                       new IdentityValueConversion(),      /* 1 candela <MGD> = 1 candela     <MGM>  */
                                    });

        public static UnitSystemConversionLookup Conversions = new UnitSystemConversionLookup(new List<UnitSystemConversion> { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion});
    }

    #endregion Physical Measure Static Classes
}
