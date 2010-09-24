using PhysicalMeasure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace PhysicalMeasureTest
{
    
    
    /// <summary>
    ///This is a test class for PhysicalQuantityTest and is intended
    ///to contain all PhysicalQuantityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhysicalQuantityTest
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

        #region PhysicalQuantity.Parse test

        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTestMilliGram()
        {
            string s = "123.000 mg"; 
            NumberStyles styles = NumberStyles.Float;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            IPhysicalQuantity actual;
            actual = PhysicalQuantity.Parse(s, styles, provider);
            Assert.AreEqual(expected, actual);
        }

        #endregion PhysicalQuantity.Parse test


        #region PhysicalQuantity math test

        /// <summary>
        ///A test for add operator
        ///</summary>
        [TestMethod()]
        public void AddKiloGramToMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(456, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(456.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            IPhysicalQuantity actual = pg1 + pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sub operator
        ///</summary>
        [TestMethod()]
        public void SubKiloGramFromMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(789, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(0.000123- 789, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity actual = pg1 - pg2;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultKiloGramToMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(456, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalUnit MassSquared = new DerivedUnit(PhysicalMeasure.Physic.SI_Units, new sbyte[] { 0, 2, 0, 0, 0, 0, 0 });

            PhysicalQuantity expected = new PhysicalQuantity(0.000123 * 456 , (IPhysicalUnit)MassSquared);

            PhysicalQuantity actual = pg1 * pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void DivKiloGramFromMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(789, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(0.000123 / 789, PhysicalMeasure.Physic.dimensionless);

            PhysicalQuantity actual = pg1 / pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void CalcEnergyIn1Gram()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalUnit MeterPrSecond = new DerivedUnit(PhysicalMeasure.Physic.SI_Units, new sbyte[] { 1, 0, -1, 0, 0, 0, 0 });
            PhysicalQuantity c = new PhysicalQuantity(299792458, MeterPrSecond);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, PhysicalMeasure.Physic.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * c.Pow(2);
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void CalcEnergyIn1GramFalling10MeterAtEarthSurface()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(PhysicalMeasure.Physic.SI_Units, new sbyte[] { 1, 0, -2, 0, 0, 0, 0 });
            PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPrSecond2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 9.81 * 10, PhysicalMeasure.Physic.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * g * h;

            Assert.AreEqual(expected, E);
        }
       

        #endregion PhysicalQuantity math test
    }
}
