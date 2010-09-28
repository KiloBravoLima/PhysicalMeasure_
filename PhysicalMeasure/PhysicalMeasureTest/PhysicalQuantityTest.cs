using PhysicalMeasure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace PhysicalMeasureTest
{

    /// <summary>
    ///This is a test class for PhysicalUnitTest and is intended
    ///to contain all PhysicalUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhysicalUnitTest
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

        #region PhysicalUnit StringTo tests

        /// <summary>
        ///A test for base unit ToString
        ///</summary>
        [TestMethod()]
        public void BaseUnitToStringTest()
        {
            PhysicalUnit u = (PhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);

            String expected = "SI.Kg";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for named derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitToStringTest()
        {
            PhysicalUnit u = (PhysicalUnit)(PhysicalMeasure.Physics.SI_Units.NamedDerivedUnits[5]);

            String expected = "SI.J";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitToStringTest()
        {
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            String expected = "SI.ms-2";

            String actual = MeterPrSecond2.ToString();

            Assert.AreEqual(expected, actual);
        }

        #endregion PhysicalUnit StringTo tests

        #region PhysicalUnit math tests

        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultBaseunitAndDerivedUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)];
            PhysicalUnit pu2 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 2, 2, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 * pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void DivBaseunitAndDerivedUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)];
            PhysicalUnit pu2 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { -2, 0, 2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void DivDerivedunitAndBaseUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2
            PhysicalUnit pu2 = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)];

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 2, 0, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PowerOfBaseUnit()
        {
            PhysicalUnit pu = (PhysicalUnit)PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Length)];

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 3, 0, 0, 0, 0, 0, 0 });

            PhysicalUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PowerOfDerivedUnit()
        {
            PhysicalUnit pu = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 3, 0, -3, 0, 0, 0, 0 });

            PhysicalUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for root operator
        ///</summary>
        [TestMethod()]
        public void RootOfDerivedUnit()
        {
            PhysicalUnit pu = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expected = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu % 2;
            Assert.AreEqual(expected, actual);
        }


        #endregion PhysicalUnit math tests

        #region BaseUnit tests

        /// <summary>
        ///A test for BaseUnit BaseUnitNumber access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestBaseUnitNumberAccessMass()
        {
            BaseUnit u = (BaseUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);

            SByte expected = 1;

            SByte actual = u.BaseUnitNumber;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BaseUnit BaseUnitNumber access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestBaseUnitNumberAccessLuminousIntensity()
        {
            BaseUnit u = (BaseUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Luminous_intensity)]);

            SByte expected = 6;

            SByte actual = u.BaseUnitNumber;

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for BaseUnit Name access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestNameAccessMass()
        {
            BaseUnit u = (BaseUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]);

            String expected = "kilogram";

            String actual = u.Name;

            Assert.AreEqual(expected, actual);
        }


        #endregion BaseUnit tests

    }

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
            String s = "123.000 mg"; 
            NumberStyles styles = NumberStyles.Float;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
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
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(456, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(456.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            IPhysicalQuantity actual = pg1 + pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sub operator
        ///</summary>
        [TestMethod()]
        public void SubKiloGramFromMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(789, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(0.000123- 789, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity actual = pg1 - pg2;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultKiloGramToMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(456, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalUnit MassSquared = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 0, 2, 0, 0, 0, 0, 0 });

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
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(789, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(0.000123 / 789, PhysicalMeasure.Physics.dimensionless);

            PhysicalQuantity actual = pg1 / pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyIn1Gram()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)]));

            PhysicalUnit MeterPrSecond = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });
            PhysicalQuantity c = new PhysicalQuantity(299792458, MeterPrSecond);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * c.Pow(2);
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyOf1GramAfterFalling10MeterAtEarthSurface()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPrSecond2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 9.81 * 10, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * g * h;

            Assert.AreEqual(expected, E);
        }



        /// <summary>
        ///A test for adding quantities of different units
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentException))]
        public void AdditionOfDifferentUnitsTest()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            PhysicalQuantity m_plus_h = m + h;
        }

        /// <summary>
        ///A test for subtracting quantities of different units
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentException))]
        public void SubtractionOfDifferentUnitsTest()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            PhysicalQuantity m_minus_h = m - h;
        }

        /// <summary>
        ///A test for adding quantity with a number
        ///</summary>
        [TestMethod()]
        public void AdditionOfUnitsWithNumbersTest()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            double h = 10.0; 

            // Will not compile:  
            // PhysicalQuantity m_plus_h = m + h;

            // Will not compile:  
            // PhysicalQuantity h_plus_m = h + m;

        }

        /// <summary>
        ///A test for adding quantity with a number
        ///</summary>
        [TestMethod()]
        public void SubtractionOfUnitsWithNumbersTest()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            double h = 10.0;

            // Will not compile:  
            // PhysicalQuantity m_sub_h = m - h;

            // Will not compile:  
            // PhysicalQuantity h_sub_m = h - m;

        }

        #endregion PhysicalQuantity math test

        #region PhysicalQuantity ToString test

        /// <summary>
        ///A test for base unit PhysicalQuantity ToString
        ///</summary>
        [TestMethod()]
        public void BaseUnitPhysicalQuantityToStringTest()
        {
            PhysicalQuantity pq = new PhysicalQuantity(123.4, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("Kg"));

            String expected = (123.4).ToString()+" SI.Kg";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for named derived unit PhysicalQuantity ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitPhysicalQuantityToStringTest()
        {
            PhysicalQuantity pq = new PhysicalQuantity(0.001 * 9.81 * 10, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("J"));

            String expected = (0.0981).ToString()+" SI.J";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit PhysicalQuantity ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitPhysicalQuantityToStringTest()
        {
            PhysicalQuantity pq = new PhysicalQuantity(0.00987654321, new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 }));

            String expected = (0.00987654321).ToString() + " SI.ms-2";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }


        #endregion PhysicalQuantity ToString test
    }
}
