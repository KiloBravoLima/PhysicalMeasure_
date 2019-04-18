/*   http://physicalmeasure.codeplex.com                          */

using System;
using System.Diagnostics;

using PhysicalMeasure;
using static PhysicalMeasure.SI;

namespace PhysicalMeasureExamples
{
    public class PhysicalMeasureExamples
    {
        public Quantity CalculateEnergyIn1Gram()
        {
            Quantity M = 0.001 * Kg;

            Unit MeterPerSecond = m / s;
            Quantity c = 299792458 * MeterPerSecond;

            Quantity expected = (0.001 * 299792458 * 299792458) * J;

            Quantity E = M * c.Pow(2);

            Debug.Assert(expected == E);

            return E;
        }

        public String CalculatePriceInEuroForEnergiConsumed()
        {
            BaseUnit Euro = null;
            ConvertibleUnit Cent = null;
            UnitSystem EuroUnitSystem = new UnitSystem("Euros", Prefixes.UnitPrefixes,
                (us) => { Euro = new BaseUnit(us, (SByte)MonetaryBaseQuantityKind.Currency, "Euro", "�"); return new BaseUnit[] { Euro }; },
                null,
                (us) => { Cent = new ConvertibleUnit("Euro-cent", "�", us.BaseUnits[0], new ScaledValueConversion(100)); return new ConvertibleUnit[] { Cent }; /* [�] = 100 * [�] */
            });
            Unit EurosAndCents = new MixedUnit(Euro, " ", Cent, "00", true);

            Unit kWh = Prefixes.k * W * SI.h; // Kilo Watt hour

            Quantity EnergyUnitPrice = 31.75 * Cent / kWh;

            Quantity EnergyConsumed = 1234.56 * kWh;

            Quantity PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice;

            Quantity PriceEnergyConsumedEurosAndCents = PriceEnergyConsumed.ConvertTo(EurosAndCents);

            Double PriceInEuroForEnergyConsumed = PriceEnergyConsumed.ConvertTo(Euro).Value;

            String PriceInEuroForEnergyConsumedStr = PriceEnergyConsumedEurosAndCents.ToString();

            Debug.Assert(PriceInEuroForEnergyConsumed == 31.75 / 100 * 1234.56);
            Debug.Assert(PriceInEuroForEnergyConsumedStr == "391 � 97 �");

            return PriceInEuroForEnergyConsumedStr;
        }
    }
}