﻿using System;
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

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ExponentsValues()
        {
            IUnit unit0= (IUnit)(Physics.SI_Units.BaseUnits[0]);
            SByte[] expected0 = new SByte[7] { 1, 0, 0, 0, 0, 0, 0 };
            SByte[] actual0 = unit0.Exponents;
            Assert.AreEqual(actual0[0], expected0[0]);

            IUnit unit1 = (IUnit)(Physics.SI_Units.BaseUnits[1]);
            SByte[] expected1 = new SByte[7] { 0, 1, 0, 0, 0, 0, 0 };
            SByte[] actual1 = unit1.Exponents;
            Assert.AreEqual(actual1[1], expected1[1]);

            IUnit unit2 = (IUnit)(Physics.SI_Units.BaseUnits[2]);
            SByte[] expected2 = new SByte[7] { 0, 0, 1, 0, 0, 0, 0 };
            SByte[] actual2 = unit2.Exponents;
            Assert.AreEqual(actual2[2], expected2[2]);

            IUnit unit3 = (IUnit)(Physics.SI_Units.BaseUnits[3]);
            SByte[] expected3 = new SByte[7] { 0, 0, 0, 1, 0, 0, 0 };
            SByte[] actual3 = unit3.Exponents;
            Assert.AreEqual(actual3[3], expected3[3]);

            IUnit unit4 = (IUnit)(Physics.SI_Units.BaseUnits[4]);
            SByte[] expected4 = new SByte[7] { 0, 0, 0, 0, 1, 0, 0 };
            SByte[] actual4 = unit4.Exponents;
            Assert.AreEqual(actual4[4], expected4[4]);

            IUnit unit5 = (IUnit)(Physics.SI_Units.BaseUnits[5]);
            SByte[] expected5 = new SByte[7] { 0, 0, 0, 0, 0, 1, 0 };
            SByte[] actual5 = unit5.Exponents;
            Assert.AreEqual(actual5[5], expected5[5]);

            IUnit unit6 = (IUnit)(Physics.SI_Units.BaseUnits[6]);
            SByte[] expected6 = new SByte[7] { 0, 0, 0, 0, 0, 0, 1 };
            SByte[] actual6 = unit6.Exponents;
            Assert.AreEqual(actual6[6], expected6[6]);

        }


        /***
        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnsignedExponentsValues()
        {
            IUnit unit0 = (IUnit)(Physics.SI_Units.BaseUnits[0]);
            Byte[] expected0 = new Byte[7] { 1, 0, 0, 0, 0, 0, 0 };
            Byte[] actual0 = unit0.UnsignedExponents;
            Assert.AreEqual(actual0[0], expected0[0]);

            IUnit unit1 = (IUnit)(Physics.SI_Units.BaseUnits[1]);
            Byte[] expected1 = new Byte[7] { 0, 1, 0, 0, 0, 0, 0 };
            Byte[] actual1 = unit1.UnsignedExponents;
            Assert.AreEqual(actual1[1], expected1[1]);

            IUnit unit2 = (IUnit)(Physics.SI_Units.BaseUnits[2]);
            Byte[] expected2 = new Byte[7] { 0, 0, 1, 0, 0, 0, 0 };
            Byte[] actual2 = unit2.UnsignedExponents;
            Assert.AreEqual(actual2[2], expected2[2]);

            IUnit unit3 = (IUnit)(Physics.SI_Units.BaseUnits[3]);
            Byte[] expected3 = new Byte[7] { 0, 0, 0, 1, 0, 0, 0 };
            Byte[] actual3 = unit3.UnsignedExponents;
            Assert.AreEqual(actual3[3], expected3[3]);

            IUnit unit4 = (IUnit)(Physics.SI_Units.BaseUnits[4]);
            Byte[] expected4 = new Byte[7] { 0, 0, 0, 0, 1, 0, 0 };
            Byte[] actual4 = unit4.UnsignedExponents;
            Assert.AreEqual(actual4[4], expected4[4]);

            IUnit unit5 = (IUnit)(Physics.SI_Units.BaseUnits[5]);
            Byte[] expected5 = new Byte[7] { 0, 0, 0, 0, 0, 1, 0 };
            Byte[] actual5 = unit5.UnsignedExponents;
            Assert.AreEqual(actual5[5], expected5[5]);

            IUnit unit6 = (IUnit)(Physics.SI_Units.BaseUnits[6]);
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
            Assert.AreEqual(Prefix.Y.Value, 1E24);
            Assert.AreEqual(Prefix.Z.Value, 1E21);
            Assert.AreEqual(Prefix.E.Value, 1E18);
            Assert.AreEqual(Prefix.P.Value, 1E15);
            Assert.AreEqual(Prefix.T.Value, 1E12);
            Assert.AreEqual(Prefix.G.Value, 1E9);
            Assert.AreEqual(Prefix.M.Value, 1E6);
            Assert.AreEqual(Prefix.K.Value, 1E3);
            Assert.AreEqual(Prefix.H.Value, 1E2);
            Assert.AreEqual(Prefix.D.Value, 1E1);
            Assert.AreEqual(Prefix.d.Value, 1E-1);
            Assert.AreEqual(Prefix.c.Value, 1E-2);
            Assert.AreEqual(Prefix.m.Value, 1E-3);
            Assert.AreEqual(Prefix.my.Value, 1E-6);
            Assert.AreEqual(Prefix.n.Value, 1E-9);
            Assert.AreEqual(Prefix.p.Value, 1E-12);
            Assert.AreEqual(Prefix.f.Value, 1E-15);
            Assert.AreEqual(Prefix.a.Value, 1E-18);
            Assert.AreEqual(Prefix.z.Value, 1E-21);
            Assert.AreEqual(Prefix.y.Value, 1E-24);
        }

        #endregion SI Unit Prefix symbols test

        #region Units must differ test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthMass()
        {
            IUnit LengthUnit = (IUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)]);
            IUnit MassUnit = (IUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.AreNotEqual(LengthUnit, MassUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthElectricCharge()
        {
            IUnit LengthUnit = (IUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)]);
            IUnit ElectricChargeUnit  = (IUnit)(Physics.SI_Units.NamedDerivedUnits[7]);

            Assert.AreNotEqual(LengthUnit, ElectricChargeUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKilogramGram()
        {
            IUnit MassUnit = (IUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            IUnit CelsiusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestMassCelsius()
        {
            IUnit MassUnit = (IUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            IUnit CelsiusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestCoulombCelsius()
        {
            IUnit Coulomb = (IUnit)(Physics.SI_Units.NamedDerivedUnits[7]);
            IUnit CelsiusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[0]);

            Assert.AreNotEqual(Coulomb, CelsiusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKelvinCelsius()
        {
            IUnit KelvinUnit = (IUnit)(Physics.SI_Units.NamedDerivedUnits[5]);
            IUnit CelsiusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

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
            IUnitSystem SomeUnitSystem = Physics.SI_Units;
            IUnitSystem SomeOtherUnitSystem = Physics.MGD_Units;
            UnitSystemConversion expected = Physics.SItoMGDConversion; 
            UnitSystemConversion actual = Physics.UnitSystemConversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void StaticTest_GetUnitSystemCombinedConversionTest()
        {
            IUnitSystem SomeUnitSystem = Physics.CGS_Units;
            IUnitSystem SomeOtherUnitSystem = Physics.MGM_Units;
            //UnitSystemConversion expected = Physics.SItoMGDConversion;
            UnitSystemConversion actual = Physics.UnitSystemConversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(actual.BaseUnitSystem, SomeUnitSystem);
            Assert.AreEqual(actual.ConvertedUnitSystem, SomeOtherUnitSystem);
        }

        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void StaticTest_GetUnitSystemCombinedReversedConversionTest()
        {
            IUnitSystem SomeUnitSystem = Physics.MGM_Units;
            IUnitSystem SomeOtherUnitSystem = Physics.CGS_Units;
            //UnitSystemConversion expected = Physics.SItoMGDConversion;
            UnitSystemConversion actual = Physics.UnitSystemConversions.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
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
            IUnit expected = (IUnit)(Physics.SI_Units.BaseUnits[3]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCoulomb()
        {
            String SymbolStr = "C";
            IUnit expected = (IUnit)(Physics.SI_Units.NamedDerivedUnits[7]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCelsius()
        {
            String SymbolStr = "°C";
            IUnit expected = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromSymbol(SymbolStr);
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
            IUnit expected = (IUnit)(Physics.SI_Units.BaseUnits[0]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on BaseUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestAmpere()
        {
            String NameStr = "ampere";
            IUnit expected = (IUnit)(Physics.SI_Units.BaseUnits[3]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on BaseUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestCadela()
        {
            String NameStr = "candela";
            IUnit expected = (IUnit)(Physics.SI_Units.BaseUnits[6]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestHertz()
        {
            String NameStr = "hertz";
            IUnit expected = (IUnit)(Physics.SI_Units.NamedDerivedUnits[0]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestJoule()
        {
            String NameStr = "joule";
            IUnit expected = (IUnit)(Physics.SI_Units.NamedDerivedUnits[5]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on NamedDerivedUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestKatal()
        {
            String NameStr = "katal";
            IUnit expected = (IUnit)(Physics.SI_Units.NamedDerivedUnits[19]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on ConvertibleUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestGram()
        {
            String NameStr = "gram";
            IUnit expected = (IUnit)(Physics.SI_Units.ConvertibleUnits[0]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on ConvertibleUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestCelsius()
        {
            String NameStr = "Celsius";
            IUnit expected = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = Physics.UnitSystems.UnitFromName(NameStr);
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
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)])));
            //IPhysicalQuantity actual;
            IPhysicalUnit expected = new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)], 1));
            IPhysicalUnit actual;
            actual = Physics.UnitSystems.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestGram()
        {
            String ScaledSymbolStr = "g";
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //IPhysicalQuantity actual;
            //IPhysicalUnit expected = (IPhysicalUnit)(new CombinedUnit(new PrefixedUnitExponent(-3, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]), 1)));
            IPhysicalUnit expected = (IPhysicalUnit)(Physics.SI_Units.ConvertibleUnits[0]);
            IPhysicalUnit actual;
            actual = Physics.SI_Units.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestKiloGram()
        {
            String ScaledSymbolStr = "Kg";
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(1, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //IPhysicalQuantity actual;
            IPhysicalUnit expected = Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)];
            IPhysicalUnit actual;
            actual = Physics.UnitSystems.ScaledUnitFromSymbol(ScaledSymbolStr);
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

            IPhysicalQuantity expected = new PhysicalQuantity(-173, Physics.SI_Units.ConvertibleUnits[1]); // °C

            IPhysicalQuantity actual   = PhysicalQuantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for convertible unit Celsius
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestCelsiusEqualsKelvin()
        {
            IPhysicalQuantity InCelsius = new PhysicalQuantity(-173, Physics.SI_Units.ConvertibleUnits[1]); // °C
            IPhysicalQuantity InKelvin = new PhysicalQuantity(100.15, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)]); // K

            Assert.AreEqual(InCelsius, InKelvin);
        }

        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestGramParseString()
        {
            String ConvertibleUnitQuantityStr = "123 g";

            IPhysicalQuantity expected = new PhysicalQuantity(123, Physics.SI_Units.ConvertibleUnits[0]);

            IPhysicalQuantity actual = PhysicalQuantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestMilliLiterParseString()
        {
            String ConvertibleUnitQuantityStr = "1234 ml";

            IPhysicalQuantity expected = new PhysicalQuantity(1.234, Physics.SI_Units.ConvertibleUnits[3]);

            IPhysicalQuantity actual = PhysicalQuantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestGramEqualsKilogram()
        {
            IPhysicalQuantity InGram = new PhysicalQuantity(123, Physics.SI_Units.ConvertibleUnits[0]);
            IPhysicalQuantity InKilogram = new PhysicalQuantity(0.123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.AreEqual(InGram, InKilogram);
        }


        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestLitreEqualsCubicMeter()
        {
            IPhysicalQuantity InLitre = new PhysicalQuantity(123, Physics.SI_Units.ConvertibleUnits[3]);
            IPhysicalQuantity InCubicMeter = new PhysicalQuantity(0.123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)].Pow(3));

            Assert.AreEqual(InLitre, InCubicMeter);
        }



        /// <summary>
        ///A test for convertible unit liter
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestSomeLitreNotEqualsAnyCubicMeter()
        {
            IPhysicalQuantity InLitre = new PhysicalQuantity(123, (IPhysicalUnit)(Physics.SI_Units.ConvertibleUnits[3]));
            IPhysicalQuantity InCubicMeter = new PhysicalQuantity(0.1234, (Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)].Pow(3)));

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

            IPhysicalUnit expected = new MixedUnit(Physics.MGD_Units.UnitFromSymbol("d"), ":" , new MixedUnit(Physics.SI_Units.UnitFromSymbol("h"), ":" , new MixedUnit(Physics.MGD_Units.UnitFromSymbol("min"), ":" , SI.s)));

            IPhysicalUnit actual = PhysicalUnit.Parse(MixedUnitStr);

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
            PhysicalQuantity m = new PhysicalQuantity(1, SI.g);
            PhysicalQuantity h = new PhysicalQuantity(10, SI.m);

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            // PhysicalUnit MeterPerSecond2 = new DerivedUnit(Physic.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            PhysicalUnit MeterPerSecond2 = (PhysicalUnit)(SI.m / (SI.s ^ 2)).Unit;
            PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPerSecond2);

            PhysicalQuantity E = m * g * h;

            double ENumProd = (0.001 * 9.81 * 10);
            PhysicalUnit EUnitProd = (PhysicalUnit)(SI.Kg * MeterPerSecond2 * SI.m).Unit;

            PhysicalQuantity Eexpected = new PhysicalQuantity(ENumProd, EUnitProd);
            Assert.AreEqual(Eexpected, E);

            PhysicalQuantity v = (2 * E / m) % 2; /* OBS! The '%' operator is used for root. "% 2" is square root, */

            PhysicalUnit MeterPerSecond = (PhysicalUnit)(SI.m / SI.s).Unit;

            double speed = Math.Pow(2 * (0.001 * 9.81 * 10) / 0.001, 0.5);
            PhysicalQuantity expected = new PhysicalQuantity(speed, MeterPerSecond);

            Assert.AreEqual(expected, v);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_version_1()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, SI.Kg);
            PhysicalQuantity c = new PhysicalQuantity(299792458, SI.m / SI.s);

            PhysicalQuantity E = m * c.Pow(2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, SI.J);
            
            Assert.AreEqual(expected, E);

            //IPhysicalUnit TJ = new PrefixedUnit(Prefix.T, SI.J);
            PhysicalUnit TJ = Prefix.T * SI.J;

            IPhysicalQuantity E_in_TJ = E.ConvertTo(TJ);
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
            PhysicalQuantity m = new PhysicalQuantity(0.001, SI.Kg);
            PhysicalQuantity c = new PhysicalQuantity(299792458, SI.m / SI.s);

            PhysicalQuantity E = m * c.Pow(2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, SI.J);
        
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_Version_3()
        {
            PhysicalQuantity M = 0.001 * SI.Kg;

            PhysicalUnit MeterPerSecond = SI.m / SI.s;
            PhysicalQuantity c = 299792458 * MeterPerSecond;

            PhysicalQuantity expected = (0.001 * 299792458 * 299792458) * SI.J;

            PhysicalQuantity E = M * (c^2);

            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for the SI static class
        ///</summary>
        [TestMethod()]
        public void StaticsTest_CalculateEnergyIn1Gram_Version_4()
        {
            // using static PhysicalMeasure.SI;

            PhysicalQuantity M = 0.001 * Kg;

            PhysicalUnit MeterPerSecond = m/s;
            PhysicalQuantity c = 299792458 * MeterPerSecond;

            PhysicalQuantity expected = (0.001 * 299792458 * 299792458) * J;

            PhysicalQuantity E = M * c.Pow(2);

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

            BaseUnit Euro = new BaseUnit(null, (SByte)MonetaryBaseQuantityKind.Currency, "Euro", "€");
            ConvertibleUnit Cent = new ConvertibleUnit("Euro-cent", "¢", Euro, new ScaledValueConversion(100));  /* [¢] = 100 * [€] */

            UnitSystem EuroUnitSystem = new UnitSystem("Euros", Physics.UnitPrefixes, Euro, null, new ConvertibleUnit[] { Cent } );

            PhysicalUnit EurosAndCents = new MixedUnit(Euro, " ", Cent,"00", true);

            PhysicalUnit kWh = Prefix.k * W * SI.h; // Kilo Watt hour

            PhysicalQuantity EnergyUnitPrice = 31.75 * Cent / kWh;

            PhysicalQuantity EnergyConsumed = 1234.56 * kWh;

            PhysicalQuantity PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice;

            IPhysicalQuantity PriceEnergyConsumedEurosAndCents = PriceEnergyConsumed.ConvertTo(EurosAndCents);

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
            PhysicalQuantity distanceSunEarth = new PhysicalQuantity(150E12, SI.m);
            PhysicalQuantity lightspeed = Constants.c;
            PhysicalQuantity time = distanceSunEarth / lightspeed;

            PhysicalQuantity expected = new PhysicalQuantity(150E12 / 299792458, SI.s);
            Assert.AreEqual(expected, time);
        }

        #endregion Physical Constants Statics test
    }
}
