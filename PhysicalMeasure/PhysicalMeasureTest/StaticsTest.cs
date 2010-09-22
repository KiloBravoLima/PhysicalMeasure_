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
            Assert.AreEqual((int)MeasureKind.length, 0);
            Assert.AreEqual((int)MeasureKind.mass, 1);
            Assert.AreEqual((int)MeasureKind.time, 2);
            Assert.AreEqual((int)MeasureKind.electric_current, 3);
            Assert.AreEqual((int)MeasureKind.thermodynamic_temperature, 4);
            Assert.AreEqual((int)MeasureKind.amount_of_substance, 5);
            Assert.AreEqual((int)MeasureKind.luminous_intensity, 6);
        }

        #region Units must differ test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthMass()
        {
            IUnit LengthUnit = (IUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.length)]);
            IUnit MassUnit = (IUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)]);

            Assert.AreNotEqual(LengthUnit, MassUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestLengthElectricCharge()
        {
            IUnit LengthUnit = (IUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.length)]);
            IUnit ElectricChargeUnit  = (IUnit)(Physic.SI_Units.NamedDerivedUnits[7]);

            Assert.AreNotEqual(LengthUnit, ElectricChargeUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestMassCelcius()
        {
            IUnit MassUnit = (IUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physic.SI_Units.ConvertabelUnits[0]);

            Assert.AreNotEqual(MassUnit, CelciusTemperatureUnit);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitsMustDifferTestCoulombCelcius()
        {
            IUnit Coulomb = (IUnit)(Physic.SI_Units.NamedDerivedUnits[7]);
            IUnit CelciusTemperatureUnit = (IUnit)(Physic.SI_Units.ConvertabelUnits[0]);

            Assert.AreNotEqual(Coulomb, CelciusTemperatureUnit);
        }

        #endregion Units must differ test

        #region GetUnitSystemConversion test
        /// <summary>
        ///A test for GetUnitSystemConversion
        ///</summary>
        [TestMethod()]
        public void GetUnitSystemConversionTest()
        {
            IUnitSystem SomeUnitSystem = Physic.SI_Units;
            IUnitSystem SomeOtherUnitSystem = Physic.MGD_Units;
            UnitSystemConversion expected = Physic.SItoMGDConversion; 
            UnitSystemConversion actual;
            actual = Physic.GetUnitSystemConversion(SomeUnitSystem, SomeOtherUnitSystem);
            Assert.AreEqual(expected, actual);
        }

        #endregion GetUnitSystemConversion test

        #region UnitFromSymbol test

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCoulomb()
        {
            string SymbolStr = "C";
            IUnit expected = (IUnit)(Physic.SI_Units.NamedDerivedUnits[7]);
            IUnit actual;
            actual = Physic.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void UnitFromSymbolTestCelcius()
        {
            string SymbolStr = "°C";
            IUnit expected = (IUnit)(Physic.SI_Units.ConvertabelUnits[1]);
            IUnit actual;
            actual = Physic.UnitFromSymbol(SymbolStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion UnitFromSymbol test

        #region ScaledUnitFromSymbol test
        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestMilliKelvin()
        {
            string ScaledSymbolStr = "mK";
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.thermodynamic_temperature)])));
            IPhysicalQuantity actual;
            actual = Physic.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestGram()
        {
            string ScaledSymbolStr = "g";
            /* IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)]))); */
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)])));
            IPhysicalQuantity actual;
            actual = Physic.SI_Units.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ScaledUnitFromSymbol
        ///</summary>
        [TestMethod()]
        public void ScaledUnitFromSymbolTestKiloGram()
        {
            string ScaledSymbolStr = "Kg";
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(1, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)])));
            IPhysicalQuantity actual;
            actual = Physic.ScaledUnitFromSymbol(ScaledSymbolStr);
            Assert.AreEqual(expected, actual);
        }

        #endregion ScaledUnitFromSymbol test


        #region Convertabel unit test

        /// <summary>
        ///A test for convertible unit Celsius
        ///</summary>
        [TestMethod()]
        public void convertibleUnitTestCelsius()
        {
            //string BaseUnitQuantityStr = "100 K";
            // !!! To do : make this work : string ConverterbelUnitQuantityStr = "-173.15 C";
            string ConverterbelUnitQuantityStr = "-173 °C";

            // IPhysicalQuantity expected = PhysicalQuantity.Parse(BaseUnitQuantityStr);
            IPhysicalQuantity expected = new PhysicalQuantity(100+0.15, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.thermodynamic_temperature)]));

            IPhysicalQuantity actual   = PhysicalQuantity.Parse(ConverterbelUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for convertible unit gram
        ///</summary>
        [TestMethod()]
        public void convertibleUnitTestGram()
        {
            //string BaseUnitQuantityStr = "0.123 Kg";
            string ConverterbelUnitQuantityStr = "123 g";

            // IPhysicalQuantity expected = PhysicalQuantity.Parse(BaseUnitQuantityStr);
            IPhysicalQuantity expected = new PhysicalQuantity(0.123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)]));

            IPhysicalQuantity actual = PhysicalQuantity.Parse(ConverterbelUnitQuantityStr);

            Assert.AreEqual(expected, actual);
        }

        #endregion Convertabel unit test

        #region SI unit symbol test


        /// <summary>
        ///A test for power operator
        ///</summary>
        public void CalcEnergyIn1Gram_using_symbolnames()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, Physic.SI.kg);

            PhysicalUnit MeterPrSecond = (PhysicalUnit)(Physic.SI.m / Physic.SI.s).Unit;
            PhysicalQuantity c = new PhysicalQuantity(299792458, MeterPrSecond);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, Physic.SI.J);

            PhysicalQuantity E = m * c.Pow(2);
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for root operator
        ///</summary>
        [TestMethod()]
        public void CalcSpeedOf1GramFalling10MeterAtEarthSurface()
        {
            PhysicalQuantity m =  new PhysicalQuantity(1 , Physic.SI.g);
            PhysicalQuantity h = new PhysicalQuantity(10, Physic.SI.m);

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            // PhysicalUnit MeterPrSecond2 = new DerivedUnit(Physic.SI_Units, new sbyte[] { 1, 0, -2, 0, 0, 0, 0 });
            PhysicalUnit MeterPrSecond2 = (PhysicalUnit)(Physic.SI.m / (Physic.SI.s ^ 2)).Unit ;
            PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPrSecond2);

            PhysicalQuantity E = m * g * h;

            PhysicalQuantity v = (2 * E / m) % 2; /* OBS! The '%' operator is used for root. "% 2" is square root, */

            PhysicalUnit MeterPrSecond = (PhysicalUnit)(Physic.SI.m / Physic.SI.s).Unit;

            double speed = Math.Pow(2 * (0.001 * 9.81 * 10) / 0.001, 0.5);
            PhysicalQuantity expected = new PhysicalQuantity(speed, MeterPrSecond);

            Assert.AreEqual(expected, v);
        }
       
        #endregion SI unit symbol test
    }
}
