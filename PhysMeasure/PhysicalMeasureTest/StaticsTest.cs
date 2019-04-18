using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PhysicalMeasure;
using static PhysicalMeasure.SI;
//using static PhysicalMeasure.Prefix;

namespace PhysicalMeasureTest
{
     
    /// <summary>
    ///This is a test class for StaticsTest and is intended
    ///to contain all StaticsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StaticsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void MeasureKindValues()
        {
            Assert.AreEqual((int)PhysicalBaseQuantityKind.Length, 0);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.Mass, 1);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.Time, 2);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.ElectricCurrent, 3);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.ThermodynamicTemperature, 4);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.AmountOfSubstance, 5);
            Assert.AreEqual((int)PhysicalBaseQuantityKind.LuminousIntensity, 6);
        }

        public Boolean ExponentsValuesEquals(SByte[] actual, SByte[] expected)
        {
            Boolean equals = true;
            int index = 0;
            while (equals && (actual.Length > index || expected.Length > index))
            {
                if (actual.Length > index && expected.Length > index)
                {
                    equals = actual[index] == expected[index];
                }
                else if (actual.Length > index)
                {
                    equals = actual[index] == 0;
                }
                else // if (expected.Length > index)
                {
                    equals = expected[index] == 0;
                }
                index++;
            }

            return equals;
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ExponentsValues()
        {
            IUnit unit0= (IUnit)(SI.Units.BaseUnits[0]);
            SByte[] expected0 = new SByte[7] { 1, 0, 0, 0, 0, 0, 0 };
            SByte[] actual0 = unit0.Exponents;
            Assert.AreEqual(actual0[0], expected0[0]);
            Assert.AreEqual(ExponentsValuesEquals(actual0, expected0), true);

            IUnit unit1 = (IUnit)(SI.Units.BaseUnits[1]);
            SByte[] expected1 = new SByte[7] { 0, 1, 0, 0, 0, 0, 0 };
            SByte[] actual1 = unit1.Exponents;
            Assert.AreEqual(actual1[1], expected1[1]);
            Assert.AreEqual(ExponentsValuesEquals(actual1, expected1), true);

            IUnit unit2 = (IUnit)(SI.Units.BaseUnits[2]);
            SByte[] expected2 = new SByte[7] { 0, 0, 1, 0, 0, 0, 0 };
            SByte[] actual2 = unit2.Exponents;
            Assert.AreEqual(actual2[2], expected2[2]);
            Assert.AreEqual(ExponentsValuesEquals(actual2, expected2), true);

            IUnit unit3 = (IUnit)(SI.Units.BaseUnits[3]);
            SByte[] expected3 = new SByte[7] { 0, 0, 0, 1, 0, 0, 0 };
            SByte[] actual3 = unit3.Exponents;
            Assert.AreEqual(actual3[3], expected3[3]);
            Assert.AreEqual(ExponentsValuesEquals(actual3, expected3), true);

            IUnit unit4 = (IUnit)(SI.Units.BaseUnits[4]);
            SByte[] expected4 = new SByte[7] { 0, 0, 0, 0, 1, 0, 0 };
            SByte[] actual4 = unit4.Exponents;
            Assert.AreEqual(actual4[4], expected4[4]);
            Assert.AreEqual(ExponentsValuesEquals(actual4, expected4), true);

            IUnit unit5 = (IUnit)(SI.Units.BaseUnits[5]);
            SByte[] expected5 = new SByte[7] { 0, 0, 0, 0, 0, 1, 0 };
            SByte[] actual5 = unit5.Exponents;
            Assert.AreEqual(actual5[5], expected5[5]);
            Assert.AreEqual(ExponentsValuesEquals(actual5, expected5), true);

            IUnit unit6 = (IUnit)(SI.Units.BaseUnits[6]);
            SByte[] expected6 = new SByte[7] { 0, 0, 0, 0, 0, 0, 1 };
            SByte[] actual6 = unit6.Exponents;
            Assert.AreEqual(actual6[6], expected6[6]);
            Assert.AreEqual(ExponentsValuesEquals(actual6, expected6), true);

        }


        /***
        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnsignedExponentsValues()
        {
            IUnit unit0 = (IUnit)(SI.Units.BaseUnits[0]);
            Byte[] expected0 = new Byte[7] { 1, 0, 0, 0, 0, 0, 0 };
            Byte[] actual0 = unit0.UnsignedExponents;
            Assert.AreEqual(actual0[0], expected0[0]);

            IUnit unit1 = (IUnit)(SI.Units.BaseUnits[1]);
            Byte[] expected1 = new Byte[7] { 0, 1, 0, 0, 0, 0, 0 };
            Byte[] actual1 = unit1.UnsignedExponents;
            Assert.AreEqual(actual1[1], expected1[1]);

            IUnit unit2 = (IUnit)(SI.Units.BaseUnits[2]);
            Byte[] expected2 = new Byte[7] { 0, 0, 1, 0, 0, 0, 0 };
            Byte[] actual2 = unit2.UnsignedExponents;
            Assert.AreEqual(actual2[2], expected2[2]);

            IUnit unit3 = (IUnit)(SI.Units.BaseUnits[3]);
            Byte[] expected3 = new Byte[7] { 0, 0, 0, 1, 0, 0, 0 };
            Byte[] actual3 = unit3.UnsignedExponents;
            Assert.AreEqual(actual3[3], expected3[3]);

            IUnit unit4 = (IUnit)(SI.Units.BaseUnits[4]);
            Byte[] expected4 = new Byte[7] { 0, 0, 0, 0, 1, 0, 0 };
            Byte[] actual4 = unit4.UnsignedExponents;
            Assert.AreEqual(actual4[4], expected4[4]);

            IUnit unit5 = (IUnit)(SI.Units.BaseUnits[5]);
            Byte[] expected5 = new Byte[7] { 0, 0, 0, 0, 0, 1, 0 };
            Byte[] actual5 = unit5.UnsignedExponents;
            Assert.AreEqual(actual5[5], expected5[5]);

            IUnit unit6 = (IUnit)(SI.Units.BaseUnits[6]);
            Byte[] expected6 = new Byte[7] { 0, 0, 0, 0, 0, 0, 1 };
            Byte[] actual6 = unit6.UnsignedExponents;
            Assert.AreEqual(actual6[6], expected6[6]);
        }
        ***/

        #region SI Unit Prefix symbols test

        /// <summary>
        ///A test for SI Unit Prefix symbols values
        ///</summary>
        [TestMethod()]
        public void PrefixSymbolValues()
        {
            Assert.AreEqual(Prefixes.Y.Value, 1E24);
            Assert.AreEqual(Prefixes.Z.Value, 1E21);
            Assert.AreEqual(Prefixes.E.Value, 1E18);
            Assert.AreEqual(Prefixes.P.Value, 1E15);
            Assert.AreEqual(Prefixes.T.Value, 1E12);
            Assert.AreEqual(Prefixes.G.Value, 1E9);
            Assert.AreEqual(Prefixes.M.Value, 1E6);
            Assert.AreEqual(Prefixes.K.Value, 1E3);
            Assert.AreEqual(Prefixes.H.Value, 1E2);
            Assert.AreEqual(Prefixes.D.Value, 1E1);
            Assert.AreEqual(Prefixes.d.Value, 1E-1);
            Assert.AreEqual(Prefixes.c.Value, 1E-2);
            Assert.AreEqual(Prefixes.m.Value, 1E-3);
            Assert.AreEqual(Prefixes.my.Value, 1E-6);
            Assert.AreEqual(Prefixes.n.Value, 1E-9);
            Assert.AreEqual(Prefixes.p.Value, 1E-12);
            Assert.AreEqual(Prefixes.f.Value, 1E-15);
            Assert.AreEqual(Prefixes.a.Value, 1E-18);
            Assert.AreEqual(Prefixes.z.Value, 1E-21);
            Assert.AreEqual(Prefixes.y.Value, 1E-24);
        }

        #endregion SI Unit Prefix symbols test

        #region Units must differ test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthMass()
        {
            IUnit LengthUnit = (IUnit)(SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)]);
            IUnit MassUnit = (IUnit)(SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.AreNotEqual(LengthUnit, MassUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthElectricCharge()
        {
            IUnit LengthUnit = (IUnit)(SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)]);
            IUnit ElectricChargeUnit  = (IUnit)(SI.Units.NamedDerivedUnits[7]);

            Assert.AreNotEqual(LengthUnit, ElectricChargeUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKilogramGram()
        {
            IUnit MassUnit = (IUnit)(SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            IUnit CelsiusTemperatureUnit = (IUnit)(SI.Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestMassCelsius()
        {
            IUnit MassUnit = (IUnit)(SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            IUnit CelsiusTemperatureUnit = (IUnit)(SI.Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestCoulombCelsius()
        {
            IUnit Coulomb = (IUnit)(SI.Units.NamedDerivedUnits[7]);
            IUnit CelsiusTemperatureUnit = (IUnit)(SI.Units.ConvertibleUnits[0]);

            Assert.AreNotEqual(Coulomb, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKelvinCelsius()
        {
            IUnit KelvinUnit = (IUnit)(SI.Units.NamedDerivedUnits[5]);
            IUnit CelsiusTemperatureUnit = (IUnit)(SI.Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(KelvinUnit, CelsiusTemperatureUnit);
        }

        #endregion Units must differ test

        #region GetUnitSystemConversion test

        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void StaticTest_GetUnitSystemConversionTest()
        {
            IUnitSystem SomeUnitSystem = SI.Units;
            IUnitSystem SomeOtherUnitSystem = MGD_Units.Units;
            UnitSystemConversion expected = UnitSystems.SItoMGDConversion; 
            UnitSystemConversion actual = UnitSystems.Conversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void StaticTest_GetUnitSystemCombinedConversionTest()
        {
            IUnitSystem SomeUnitSystem = CGS_Units.Units;
            IUnitSystem SomeOtherUnitSystem = MGM_Units.Units;
            //UnitSystemConversion expected = UnitSystems.SItoMGDConversion;
            UnitSystemConversion actual = UnitSystems.Conversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(actual.BaseUnitSystem, SomeUnitSystem);
            Assert.AreEqual(actual.ConvertedUnitSystem, SomeOtherUnitSystem);
        }

        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void StaticTest_GetUnitSystemCombinedReversedConversionTest()
        {
            IUnitSystem SomeUnitSystem = MGM_Units.Units;
            IUnitSystem SomeOtherUnitSystem = CGS_Units.Units;
            //UnitSystemConversion expected = UnitSystems.SItoMGDConversion;
            UnitSystemConversion actual = UnitSystems.Conversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(actual.BaseUnitSystem, SomeUnitSystem);
            Assert.AreEqual(actual.ConvertedUnitSystem, SomeOtherUnitSystem);
        }


        #endregion GetUnitSystemConversion test

        #region UnitFromSymbol test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestAmpere()
        {
            String SymbolStr = "A";
            IUnit expected = (IUnit)(SI.Units.BaseUnits[3]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCoulomb()
        {
            String SymbolStr = "C";
            IUnit expected = (IUnit)(SI.Units.NamedDerivedUnits[7]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCelsius()
        {
            String SymbolStr = "°C";
            IUnit expected = (IUnit)(SI.Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion UnitFromSymbol test

        #region UnitFromName test

        /// <summary>
        ///A test for UnitFromName on BaseUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestMeter()
        {
            String NameStr = "meter";
            IUnit expected = (IUnit)(SI.Units.BaseUnits[0]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on BaseUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestAmpere()
        {
            String NameStr = "ampere";
            IUnit expected = (IUnit)(SI.Units.BaseUnits[3]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on BaseUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestCadela()
        {
            String NameStr = "candela";
            IUnit expected = (IUnit)(SI.Units.BaseUnits[6]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestHertz()
        {
            String NameStr = "hertz";
            IUnit expected = (IUnit)(SI.Units.NamedDerivedUnits[0]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestJoule()
        {
            String NameStr = "joule";
            IUnit expected = (IUnit)(SI.Units.NamedDerivedUnits[5]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestKatal()
        {
            String NameStr = "katal";
            IUnit expected = (IUnit)(SI.Units.NamedDerivedUnits[19]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on ConvertibleUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestGram()
        {
            String NameStr = "gram";
            IUnit expected = (IUnit)(SI.Units.ConvertibleUnits[0]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on ConvertibleUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestCelsius()
        {
            String NameStr = "Celsius";
            IUnit expected = (IUnit)(SI.Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = UnitSystems.Systems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion UnitFromName test

        #region ScaledUnitFromSymbol test
        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestMilliKelvin()
        {
            String ScaledSymbolStr = "mK";
            //Quantity expected = (Quantity)(new Quantity(0.001, (Unit)(PhysicalMeasure.SI.Units.BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)])));
            //Quantity actual;
            Unit expected = new CombinedUnit(new PrefixedUnitExponent(Prefixes.m, SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)], 1));
            Unit actual;
            actual = UnitSystems.Systems.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestGram()
        {
            String ScaledSymbolStr = "g";
            //Quantity expected = (Quantity)(new Quantity(0.001, (Unit)(PhysicalMeasure.SI.Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //Quantity actual;
            //Unit expected = (Unit)(new CombinedUnit(new PrefixedUnitExponent(-3, (Unit)(PhysicalMeasure.SI.Units.BaseUnits[(int)(MeasureKind.Mass)]), 1)));
            Unit expected = SI.Units.ConvertibleUnits[0];
            Unit actual;
            actual = SI.Units.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestKiloGram()
        {
            String ScaledSymbolStr = "Kg";
            //Quantity expected = (Quantity)(new Quantity(1, (Unit)(PhysicalMeasure.SI.Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //Quantity actual;
            Unit expected = SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)];
            Unit actual;
            actual = UnitSystems.Systems.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion ScaledUnitFromSymbol test


        #region Convertible unit test

        /// <summary>
        ///A test for convertible unit Celsius
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestCelsiusParseString()
        {
            String ConvertibleUnitQuantityStr = "-173 °C";

            Quantity expected = new Quantity(-173, SI.Units.ConvertibleUnits[1]); // °C

            Quantity actual   = Quantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for convertible unit Celsius
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestCelsiusEqualsKelvin()
        {
            Quantity InCelsius = new Quantity(-173, SI.Units.ConvertibleUnits[1]); // °C
            Quantity InKelvin = new Quantity(100.15, SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)]); // K

            Assert.AreEqual(InCelsius, InKelvin);
        }

        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestGramParseString()
        {
            String ConvertibleUnitQuantityStr = "123 g";

            Quantity expected = new Quantity(123, SI.Units.ConvertibleUnits[0]);

            Quantity actual = Quantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestMilliLiterParseString()
        {
            String ConvertibleUnitQuantityStr = "1234 ml";

            Quantity expected = new Quantity(1.234, SI.Units["l"]);

            Quantity actual = Quantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestGramEqualsKilogram()
        {
            Quantity InGram = new Quantity(123, SI.Units["g"]);
            Quantity InKilogram = new Quantity(0.123, SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.AreEqual(InGram, InKilogram);
        }


        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestLitreEqualsCubicMeter()
        {
            Quantity InLitre = new Quantity(123, SI.Units["l"]);
            Quantity InCubicMeter = new Quantity(0.123, SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)].Pow(3));

            Assert.AreEqual(InLitre, InCubicMeter);
        }



        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestSomeLitreNotEqualsAnyCubicMeter()
        {
            Quantity InLitre = new Quantity(123, (Unit)(SI.Units["l"]));
            Quantity InCubicMeter = new Quantity(0.1234, (SI.Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)].Pow(3)));

            Assert.AreNotEqual(InLitre, InCubicMeter);
        }


        #endregion Convertible unit test

        #region Mixed unit  test

        /// <summary>
        ///A test for Mixed unit 
        ///</summary>
        [TestMethod()]
        public void MixedUnitTestParseString_DHMinS()
        {
            String MixedUnitStr = "d:h:min:s";

            //Unit expected = new MixedUnit((Unit)Physics.MGD_Units.UnitFromSymbol("d"), ":" , new MixedUnit((Unit)SI.Units.UnitFromSymbol("h"), ":" , new MixedUnit((Unit)Physics.MGD_Units.UnitFromSymbol("min"), ":" , SI.s)));
            Unit expected = new MixedUnit((Unit)SI.Units.UnitFromSymbol("d"), ":", new MixedUnit((Unit)SI.Units.UnitFromSymbol("h"), ":", new MixedUnit((Unit)SI.Units.UnitFromSymbol("min"), ":", SI.s)));

            Unit actual = Unit.Parse(MixedUnitStr);

            Assert.IsNotNull( actual);
            Assert.IsInstanceOfType(actual, typeof(MixedUnit) );
            Assert.AreEqual(expected, actual);
        }

        #endregion Mixed unit test


        #region SI unit symbol test

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void CalculateSpeedOf1GramAfterFalling10MeterAtEarthSurface()
        {
            Quantity m = new Quantity(1, SI.g);
            Quantity h = new Quantity(10, SI.m);

            //!!! To do: make this work: Quantity g = Quantity.Parse("9.81 m/s^2");
            // Unit MeterPerSecond2 = new DerivedUnit(Physic.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            Unit MeterPerSecond2 = SI.m / (SI.s ^ 2);
            Quantity g = new Quantity(9.81, MeterPerSecond2);

            Quantity E = m * g * h;

            double ENumProd = (0.001 * 9.81 * 10);
            Unit EUnitProd = (SI.Kg * MeterPerSecond2 * SI.m);

            Quantity Eexpected = new Quantity(ENumProd, EUnitProd);
            Assert.AreEqual(Eexpected, E);

            Quantity v = (2 * E / m) % 2; /* OBS! The '%' operator is used for root. "% 2" is square root, */

            Unit MeterPerSecond = SI.m / SI.s;

            double speed = Math.Pow(2 * (0.001 * 9.81 * 10) / 0.001, 0.5);
            Quantity expected = new Quantity(speed, MeterPerSecond);

            Assert.AreEqual(expected, v);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_version_1()
        {
            Quantity m = new Quantity(0.001, SI.Kg);
            Quantity c = new Quantity(299792458, SI.m / SI.s);

            Quantity E = m * c.Pow(2);

            Quantity expected = new Quantity(0.001 * 299792458 * 299792458, SI.J);
            
            Assert.AreEqual(expected, E);

            //Unit TJ = new PrefixedUnit(Prefix.T, SI.J);
            Unit TJ = Prefixes.T * SI.J;

            Quantity E_in_TJ = E.ConvertTo(TJ);
            String E_in_TJ_as_String = E_in_TJ.ToString();

            String expected_E_formatedAsTJ = "89,8755178736818 TJ";

            Assert.AreEqual(expected_E_formatedAsTJ, E_in_TJ_as_String);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_Version_2()
        {
            Quantity m = new Quantity(0.001, SI.Kg);
            Quantity c = new Quantity(299792458, SI.m / SI.s);

            Quantity E = m * c.Pow(2);

            Quantity expected = new Quantity(0.001 * 299792458 * 299792458, SI.J);
        
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_Version_3()
        {
            Quantity M = 0.001 * SI.Kg;

            Unit MeterPerSecond = SI.m / SI.s;
            Quantity c = 299792458 * MeterPerSecond;

            Quantity expected = (0.001 * 299792458 * 299792458) * SI.J;

            Quantity E = M * (c^2);

            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_Version_4()
        {
            // using static PhysicalMeasure.SI;

            Quantity M = 0.001 * Kg;

            Unit MeterPerSecond = m/s;
            Quantity c = 299792458 * MeterPerSecond;

            Quantity expected = (0.001 * 299792458 * 299792458) * J;

            Quantity E = M * c.Pow(2);

            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculatePriceInEuroForEnergiConsumed()
        {
            // using static PhysicalMeasure.SI;
            // using static Prefix;

            BaseUnit Euro = null;
            ConvertibleUnit Cent = null;
            UnitSystem EuroUnitSystem = new UnitSystem("Euros", Prefixes.UnitPrefixes,
                (us) => { Euro = new BaseUnit(us, (SByte)MonetaryBaseQuantityKind.Currency, "Euro", "€"); return new BaseUnit[] { Euro }; },
                null,
                (us) => { Cent = new ConvertibleUnit("Euro-cent", "¢", us.BaseUnits[0], new ScaledValueConversion(100)); return new ConvertibleUnit[] { Cent }; /* [¢] = 100 * [€] */ });

            Unit EurosAndCents = new MixedUnit(Euro, " ", Cent,"00", true);

            Unit kWh = Prefixes.k * W * SI.h; // Kilo Watt hour

            Quantity EnergyUnitPrice = 31.75 * Cent / kWh;

            Quantity EnergyConsumed = 1234.56 * kWh;

            Quantity PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice;

            Quantity PriceEnergyConsumedEurosAndCents = PriceEnergyConsumed.ConvertTo(EurosAndCents);

            Double PriceInEuroForEnergyConsumed = PriceEnergyConsumed.ConvertTo(Euro).Value;

            String PriceInEuroForEnergyConsumedStr = PriceEnergyConsumedEurosAndCents.ToString();

            Assert.AreEqual(PriceInEuroForEnergyConsumed, 31.75/100 * 1234.56 );
            Assert.AreEqual(PriceInEuroForEnergyConsumedStr, "391 € 97 ¢");

        }
        #endregion SI unit symbol test

        #region Physical Constants Statics test

        /// <summary>
        ///A test for the Physical Constants Statics class
        ///</summary>
        [TestMethod()]
        public void CalculateSunlightsTimeToEarth()
        {
            Quantity distanceSunEarth = new Quantity(150E12, SI.m);
            Quantity lightspeed = Constants.c;
            Quantity time = distanceSunEarth / lightspeed;

            Quantity expected = new Quantity(150E12 / 299792458, SI.s);
            Assert.AreEqual(expected, time);
        }

        #endregion Physical Constants Statics test
    }
}
