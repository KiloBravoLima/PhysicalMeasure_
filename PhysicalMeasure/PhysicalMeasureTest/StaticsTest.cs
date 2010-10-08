using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysicalMeasure;

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
            Assert.AreEqual((int)MeasureKind.Length, 0);
            Assert.AreEqual((int)MeasureKind.Mass, 1);
            Assert.AreEqual((int)MeasureKind.Time, 2);
            Assert.AreEqual((int)MeasureKind.Electric_current, 3);
            Assert.AreEqual((int)MeasureKind.Thermodynamic_temperature, 4);
            Assert.AreEqual((int)MeasureKind.Amount_of_substance, 5);
            Assert.AreEqual((int)MeasureKind.Luminous_intensity, 6);
        }

        #region Units must differ test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthMass()
        {
            IUnit LengthUnit = (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Length)]);
            IUnit MassUnit = (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);

            Assert.AreNotEqual(LengthUnit, MassUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthElectricCharge()
        {
            IUnit LengthUnit = (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Length)]);
            IUnit ElectricChargeUnit  = (IUnit)(Physics.SI_Units.NamedDerivedUnits[7]);

            Assert.AreNotEqual(LengthUnit, ElectricChargeUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKilogramGram()
        {
            IUnit MassUnit = (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelciusTemperatureUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestMassCelcius()
        {
            IUnit MassUnit = (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(MassUnit, CelciusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestCoulombCelcius()
        {
            IUnit Coulomb = (IUnit)(Physics.SI_Units.NamedDerivedUnits[7]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[0]);

            Assert.AreNotEqual(Coulomb, CelciusTemperatureUnit);
        }

        /// <summary>
        ///A test for Equal
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestKelvinCelcius()
        {
            IUnit KelvinUnit = (IUnit)(Physics.SI_Units.NamedDerivedUnits[5]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);

            Assert.AreNotEqual(KelvinUnit, CelciusTemperatureUnit);
        }

        #endregion Units must differ test

        #region GetUnitSystemConversion test
        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void GetUnitSystemConversionTest()
        {
            IUnitSystem SomeUnitSystem = Physics.SI_Units;
            IUnitSystem SomeOtherUnitSystem = Physics.MGD_Units;
            UnitSystemConversion expected = Physics.SItoMGDConversion; 
            UnitSystemConversion actual = Physics.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(expected, actual);
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
            actual = Physics.UnitFromSymbol(SymbolStr);
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
            actual = Physics.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCelcius()
        {
            String SymbolStr = "°C";
            IUnit expected = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = Physics.UnitFromSymbol(SymbolStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
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
            actual = Physics.UnitFromName(NameStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromName on ConvertibleUnit
        ///</summary>
        [TestMethod()]
        public void UnitFromNameTestCelcius()
        {
            String NameStr = "Celsius";
            IUnit expected = (IUnit)(Physics.SI_Units.ConvertibleUnits[1]);
            IUnit actual;
            actual = Physics.UnitFromName(NameStr);
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
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Thermodynamic_temperature)])));
            IPhysicalQuantity actual;
            actual = Physics.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestGram()
        {
            String ScaledSymbolStr = "g";
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            IPhysicalQuantity actual;
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
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(1, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            IPhysicalQuantity actual;
            actual = Physics.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion ScaledUnitFromSymbol test


        #region Convertible unit test

        /// <summary>
        ///A test for convertible unit Celsius
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestCelsius()
        {
            String ConvertibleUnitQuantityStr = "-173 °C";

            IPhysicalQuantity expected = new PhysicalQuantity(100+0.15, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Thermodynamic_temperature)]));

            IPhysicalQuantity actual   = PhysicalQuantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void ConvertibleUnitTestGram()
        {
            String ConvertibleUnitQuantityStr = "123 g";

            IPhysicalQuantity expected = new PhysicalQuantity(0.123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            IPhysicalQuantity actual = PhysicalQuantity.Parse(ConvertibleUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        #endregion Convertible unit test

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
        public void CalculateEnergyIn1Gram()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, SI.kg);
            PhysicalQuantity c = new PhysicalQuantity(299792458, SI.m / SI.s);

            PhysicalQuantity E = m * c.Pow(2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, SI.J);
            Assert.AreEqual(expected, E);
        }
      
        #endregion SI unit symbol test
    }
}
