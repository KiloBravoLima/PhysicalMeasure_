**PhysicalMeasure on gitHub**
This project is also cloned to 
[http://github.com/KiloBravoLima/PhysicalMeasure](http://github.com/KiloBravoLima/PhysicalMeasure)

**Project Description**
PhysicalMeasure is a C# library for handling physical quantities by specifying value and unit. Scaling of units and conversions between multiple unit systems.

**Concept**

This library is a C# implementation of the  concepts of system of physical units.  eg. as found on Wikipedia: [http://en.wikipedia.org/wiki/International_System_of_Units](http://en.wikipedia.org/wiki/International_System_of_Units).

Requries .NET framework 4.6.

**Examples**

PhysicalMeasure can be used to represent and handle physical measures and calculations of physical properties. like this:

{code:c#}
using PhysicalMeasure;
using static PhysicalMeasure.SI;

public Quantity CalculateEnergyIn1Gram()
{
   Quantity M = 0.001 * Kg;
   Unit MeterPerSecond = m / s;
   Quantity c = 299792458 * MeterPerSecond;

   Quantity E = M * c.Pow(2);

   Quantity expected = (0.001 * 299792458 * 299792458) * J;
   Debug.Assert(expected == E);
   return E;
}
{code:c#}

PhysicalMeasure also supports user defined units and unit systems:

{code:c#}
using PhysicalMeasure;
using static PhysicalMeasure.SI;

public String CalculatePriceInEuroForEnergiConsumed()
{
   BaseUnit Euro = new BaseUnit(null, (SByte)MonetaryBaseQuantityKind.Currency, 
                                           "Euro", "€");
   ConvertibleUnit Cent = new ConvertibleUnit("Euro-cent", "¢", Euro, 
                            new ScaledValueConversion(100));  /* [¢](¢) = 100 * [€](€) */
   UnitSystem EuroUnitSystem = new UnitSystem("Euros", Physics.UnitPrefixes, Euro, null, 
                                               new ConvertibleUnit[]() { Cent });
   Unit EurosAndCents = new MixedUnit(Euro, " ", Cent, "00", true);

   Unit kWh = Prefix.k * W * SI.h; // Kilo Watt hour
   Quantity EnergyUnitPrice = 31.75 * Cent / kWh;
   Quantity EnergyConsumed = 1234.56 * kWh;
   Quantity PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice;
   Quantity PriceEnergyConsumedEurosAndCents =
                                           PriceEnergyConsumed.ConvertTo(EurosAndCents);
   Double PriceInEuroForEnergyConsumed = PriceEnergyConsumed.ConvertTo(Euro).Value;

   String PriceInEuroForEnergyConsumedStr = PriceEnergyConsumedEurosAndCents.ToString();

   Debug.Assert(PriceInEuroForEnergyConsumed == 31.75 / 100 * 1234.56);
   Debug.Assert(PriceInEuroForEnergyConsumedStr == "391 € 97 ¢");

   return PriceInEuroForEnergyConsumedStr;
}
{code:c#}

**PhysCalc console application**

PhysCalc is a calculator using PhysicalMeasure to evaluate physical expressions. 

**Nuget package**

PhysicalMeasure are available as a Nuget package at 
   [http://www.nuget.org/packages/PhysicalMeasure](http://www.nuget.org/packages/PhysicalMeasure)
   [http://www.nuget.org/packages/PhysicalMeasure.Sample](http://www.nuget.org/packages/PhysicalMeasure.Sample)

**Links**

Additional information from Wikipedia on physical quantities and units:
 - [Physical quantity](http://en.wikipedia.org/wiki/Physical_quantity)
 - [Units of measurement](http://en.wikipedia.org/wiki/Units_of_measurement)

Additional information from Wikipedia on SI units that I have covered in this library:
 - [SI base units](http://en.wikipedia.org/wiki/SI_base_unit)
 - [SI derived units](http://en.wikipedia.org/wiki/SI_derived_unit)
 - [SI unit prefixes](http://en.wikipedia.org/wiki/SI_prefix)

Other .Net libraries and utilities related to unit conversion
 - [List of .Net libraries and utilities related to unit conversion](Links-to-other-.Net-libraries-and-utilities-related-to-unit-conversion)



