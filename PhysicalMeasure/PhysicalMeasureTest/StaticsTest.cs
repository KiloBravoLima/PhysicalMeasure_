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
            IUnit expected = (IUnit)(Physic.SI_Units.ConvertabelUnits[0]);
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


    }
}
