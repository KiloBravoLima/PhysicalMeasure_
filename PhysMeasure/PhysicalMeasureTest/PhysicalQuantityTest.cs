using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using PhysicalMeasure;
using PhysicalMeasure.Constants;

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
            PhysicalUnit u = (PhysicalUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            String expected = "Kg";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for named derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitToStringTest()
        {
            PhysicalUnit u = (PhysicalUnit)(Physics.SI_Units.NamedDerivedUnits[5]);

            String expected = "J";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitToStringTest()
        {
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            //String expected = "SI.m·s-2";
            String expected = "m·s-2";

            String actual = MeterPrSecond2.ToString();

            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitBaseUnitStringTest()
        {
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            String expected = "m·s-2";

            IPhysicalQuantity pq = MeterPrSecond2.ConvertToBaseUnit();

            Assert.AreEqual(pq.Value, 1d);

            String actual = pq.Unit.ToString();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitBaseUnitStringTest()
        {
            IPhysicalUnit Watt = Physics.SI_Units.NamedDerivedUnits[6];

            String expected = "1 m2·Kg·s-3";

            String actual = Watt.ConvertToBaseUnit().ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitReducedUnitStringTest()
        {
            IPhysicalUnit Watt = Physics.SI_Units.NamedDerivedUnits[6];

            String expected = "W";

            String actual = Watt.ReducedUnitString().ToString();

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void MixedUnitBaseUnitStringTest()
        {
            IPhysicalUnit HourMin= new MixedUnit(Physics.SI_Units.ConvertibleUnits[2], ":",  Physics.MGD_Units.ConvertibleUnits[1]);

            String expected = "3600 s";

            String actual = HourMin.ConvertToBaseUnit().ToString();

            Assert.AreEqual(expected, actual);
        }


        #endregion PhysicalUnit StringTo tests

        #region PhysicalUnit math tests

        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_MultBaseunitAndDerivedUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)];
            PhysicalUnit pu2 = (PhysicalUnit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 2, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 * pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_DivBaseunitAndDerivedUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)];
            PhysicalUnit pu2 = (PhysicalUnit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { -2, 0, 2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_DivDerivedunitAndBaseUnitTest()
        {
            PhysicalUnit pu1 = (PhysicalUnit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2
            PhysicalUnit pu2 = (PhysicalUnit)Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)];

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_PowerOfBaseUnit()
        {
            PhysicalUnit pu = (PhysicalUnit)Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Length)];

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 3, 0, 0, 0, 0, 0, 0 });

            PhysicalUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_PowerOfDerivedUnitTest()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 3, 0, -3, 0, 0, 0, 0 });

            PhysicalUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for root operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_RootOfDerivedUnitTest()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            PhysicalUnit actual = pu % 2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for mult and div operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_KmPrHourUnitTest()
        {
            PhysicalQuantity KmPrHour_example1 = Prefix.K * SI.m / SI.h;

            PhysicalQuantity Speed1 = 123 * KmPrHour_example1;

            //IPhysicalUnit KmPrHour_example2 = Prefix.K * SI.m / SI.h;

            IPhysicalUnit Km = PhysicalUnit.Parse("Km");
            IPhysicalUnit h = PhysicalUnit.Parse("h");
            IPhysicalUnit KmPrHour_example2 = Km.Divide(h);
            PhysicalQuantity Speed2 = new PhysicalQuantity(123, KmPrHour_example2);

            string KmPrHour_Str = "Km/h";

            IPhysicalUnit KmPrHour_example3 = PhysicalUnit.Parse(KmPrHour_Str);
            PhysicalQuantity Speed3 = new PhysicalQuantity(123, KmPrHour_example3);

            PhysicalQuantity expected = new PhysicalQuantity(123 * 1000.0/(60 * 60) , SI.m / SI.s);

            Assert.AreEqual(expected, Speed2, "Speed2");

            Assert.AreEqual(expected, Speed1, "Speed1");
            Assert.AreEqual(expected, Speed2, "Speed2");
            Assert.AreEqual(expected, Speed3, "Speed3");
        }

        /// <summary>
        ///A test for mult and div operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_WattHourUnitTest()
        {
            IPhysicalQuantity WattHour_example1 = SI.W * SI.h;

            PhysicalUnit WattHour_example2 = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */

            PhysicalQuantity E_1 = new PhysicalQuantity(1, WattHour_example1 ); // 1 Wh
            PhysicalQuantity E_2 = new PhysicalQuantity(0.001, Prefix.K * WattHour_example2); // 0.001 KWh
            //IPhysicalQuantity actual_1 = E_1.ConvertTo(SI.J);
            //IPhysicalQuantity actual_2 = E_2.ConvertTo(SI.J);

            PhysicalQuantity expected = new PhysicalQuantity(3600, SI.J);

            Assert.AreEqual(expected, E_1);
            Assert.AreEqual(expected, E_2);
        }

        #endregion PhysicalUnit math tests

        #region PhysicalUnit Convert tests


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitConvertToUnitSystemMGD()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            PhysicalQuantity expected = new PhysicalQuantity(1/Math.Pow(24*60*60, -4), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(Physics.MGD_Units);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitConvertToUnitSystemSI()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            PhysicalQuantity expected = new PhysicalQuantity(1 / Math.Pow(24 * 60 * 60, -4), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(Physics.MGD_Units);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitConvertToMGDUnit()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            PhysicalQuantity expected = new PhysicalQuantity(1/Math.Pow(24 * 60 * 60, -4), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitConvertToSIUnit()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.SI_Units, pu.Exponents);
            PhysicalQuantity expected = new PhysicalQuantity( Math.Pow(24 * 60 * 60, -4), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }




        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitConvertToThroughOtherUnitSystems()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.MGM_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.CGS_Units, pu.Exponents);
            PhysicalQuantity expected = new PhysicalQuantity(Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitReversedConvertToThroughOtherUnitSystems()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.CGS_Units , new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);
            PhysicalQuantity expected = new PhysicalQuantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitReversedConvertToThroughOtherUnitSystems2()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.CGS_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);
            
            // VS fails PhysicalQuantity expected = 1D / new PhysicalQuantity( (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), 1D/expectedunit);
            PhysicalQuantity expected = new PhysicalQuantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IPhysicalQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalUnitReversedConvertToThroughOtherUnitSystems2SquareOperator()
        {
            PhysicalUnit pu = new DerivedUnit(Physics.CGS_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            PhysicalUnit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);

            // VS fails PhysicalQuantity expected = 1D / new PhysicalQuantity( (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), 1D/expectedunit);
            PhysicalQuantity expected = new PhysicalQuantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IPhysicalQuantity actual = pu [expectedunit];
            Assert.AreEqual(expected, actual);
        }


        #endregion PhysicalUnit Convert tests


        #region BaseUnit tests

        /// <summary>
        ///A test for BaseUnit BaseUnitNumber access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestBaseUnitNumberAccessMass()
        {
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

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
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.LuminousIntensity)]);

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
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

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
        public void TestMilliGramParseString()
        {
            String s = "123.000 mg"; 
            NumberStyles styles = NumberStyles.Float;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(123, (IPhysicalUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(-3, PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)], 1)))));
            //IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(123, (IPhysicalUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(-3, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("g"), 1)))));
            IPhysicalQuantity expected = new PhysicalQuantity(123, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("g"), 1)));
            IPhysicalQuantity actual;
            actual = PhysicalQuantity.Parse(s, styles, provider);
            Assert.AreEqual(expected, actual);
        }

        #endregion PhysicalQuantity.Parse test


        #region PhysicalQuantity.Equal test

        /// <summary>
        ///A test for equal
        ///</summary>
        [TestMethod()]
        public void TestMilligramEqualKilogram()
        {
            //String s = "123.000 mg";
            IPhysicalQuantity InKiloGram = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]); // In kilogram
            IPhysicalQuantity InMilliGram = new PhysicalQuantity(123, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("g"), 1))); // In milli gram

            Assert.AreEqual(InKiloGram, InMilliGram);
        }

        /// <summary>
        ///A test for equal
        ///</summary>
        [TestMethod()]
        public void TestMilliKelvinEqualKiloCelsiusSpecificConversion()
        {
            //String s = "594.15 mK";
            //String s = "3.21 K°C";
            /*
            IPhysicalQuantity InMilliKelvin = (IPhysicalQuantity)(new PhysicalQuantity(321273.15, (IPhysicalUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)]))); // In Kelvin
            IPhysicalQuantity InKiloCelsius = (IPhysicalQuantity)(new PhysicalQuantity(321, (IPhysicalUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(3, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("°C"), 1))))); // In Kilo Celsius
            */
            IPhysicalQuantity InMilliKelvin = new PhysicalQuantity(321273.15, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.ThermodynamicTemperature)]); // In Kelvin
            IPhysicalQuantity InKiloCelsius = new PhysicalQuantity(321, new CombinedUnit(new PrefixedUnitExponent(Prefix.k, Physics.SI_Units.UnitFromSymbol("°C"), 1))); // In Kilo Celsius

            Assert.AreEqual(InMilliKelvin, InKiloCelsius);
        }

        /// <summary>
        ///A test for conversion between temperatures
        ///</summary>
        [TestMethod()]
        public void TestAdd2KelvinWithCelsiusSpecificConversion2()
        {

            IPhysicalQuantity InMilliKelvin = new PhysicalQuantity(321273150, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("K"), 1))); // In miliKelvin
            IPhysicalQuantity InKelvin      = new PhysicalQuantity(321273.15, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.ThermodynamicTemperature)]); // In Kelvin
            IPhysicalQuantity InKiloCelsius = new PhysicalQuantity(321, new CombinedUnit(new PrefixedUnitExponent(Prefix.K, Physics.SI_Units.UnitFromSymbol("°C"), 1))); // In Kilo Celsius

            // Check all quantities in all combinations as first and second:
            Assert.AreEqual(InMilliKelvin, InKelvin, "(InMilliKelvin, InKelvin)");
            Assert.AreEqual(InKelvin, InMilliKelvin, "(InKelvin, InMilliKelvin)");
            Assert.AreEqual(InKelvin, InKiloCelsius, "(InKelvin, InKiloCelsius)");
            Assert.AreEqual(InKiloCelsius, InKelvin, "(InKiloCelsius, InKelvin)");
            Assert.AreEqual(InMilliKelvin, InKiloCelsius, "(InMilliKelvin, InKiloCelsius)");
            Assert.AreEqual(InKiloCelsius, InMilliKelvin, "(InKiloCelsius, InMilliKelvin)");
        }


        /// <summary>
        ///A test for conversion of temperatures from Celsius to Kelvin and back using CombineUnit
        ///</summary>
        [TestMethod()]
        public void TestKelvinPerSecondConvertedToCe_per_s()
        {
            // 2013-09-05  From CodePlex User JuricaGrcic

            // Define Celsius per second - °C/s
            IPhysicalUnit Ce_per_s = SI.Ce.CombineDivide(SI.s);

            // Define Kelvin per second - K/s
            IPhysicalUnit Kelvin_per_s = SI.K.CombineDivide(SI.s);

            // Create value in units °C/s
            PhysicalQuantity valueOfCelsiusPerSecond = new PhysicalQuantity(2, Ce_per_s);
            //Console.WriteLine("Base value : {0}", valueOfCelsiusPerSecond); 
            // prints 2 °C/s
            string valueOfCelsiusPerSecond_str = valueOfCelsiusPerSecond.ToString();
            string valueOfCelsiusPerSecond_str_expected = "2 °C/s";  
            
            // Convert °C/s to K/s
            IPhysicalQuantity valueOfKelvinPerSecond = valueOfCelsiusPerSecond.ConvertTo(Kelvin_per_s);
            //Console.WriteLine("Base value converted to {0} : {1}", Ce_per_s, valueOfKelvinPerSecond);
            // prints 275.15 K/s - correct conversion or not??
            // 2013-10-29  Corrected to print 2 K/s
            string valueOfKelvinPerSecond_str = valueOfKelvinPerSecond.ToString();
            string valueOfKelvinPerSecond_str_expected = "2 K/s";

            // Convert K/s back to °C/s 
            IPhysicalQuantity valueOfKelvinPerSecondConvertedToCe_per_s = valueOfKelvinPerSecond.ConvertTo(Ce_per_s);

            //Console.WriteLine("{0} converted back to {1}: {2}", Kelvin_per_s, Ce_per_s, valueOfKelvinPerSecond.ConvertTo(Ce_per_s));
            // prints 1.0036476381543 °C/s - should print 2 °C/s - incorrect conversion
            string valueOfKelvinPerSecondConvertedToCe_per_s_str = valueOfKelvinPerSecondConvertedToCe_per_s.ToString();
            string valueOfKelvinPerSecondConvertedToCe_per_s_str_expected = "2 °C/s";

            Assert.AreEqual(valueOfCelsiusPerSecond, valueOfKelvinPerSecondConvertedToCe_per_s);

            Assert.AreEqual(valueOfCelsiusPerSecond_str, valueOfCelsiusPerSecond_str_expected);
            Assert.AreEqual(valueOfKelvinPerSecond_str, valueOfKelvinPerSecond_str_expected);
            Assert.AreEqual(valueOfKelvinPerSecondConvertedToCe_per_s_str, valueOfKelvinPerSecondConvertedToCe_per_s_str_expected);
        }

        /// <summary>
        ///A test for conversion of temperatures from Celsius to Kelvin and back using CombineUnit
        ///</summary>
        [TestMethod()]
        public void TestMeterKelvinPerSecondConvertedToCe_per_s()
        {
            // 2013-09-05  From CodePlex User JuricaGrcic but modified to not have °C as first element in denominators

            // Define Celsius per second - m·°C/s
            IPhysicalUnit meter_Ce_per_s = SI.m.CombineMultiply(SI.Ce).CombineDivide(SI.s);

            // Define Kelvin per second - m·K/s
            IPhysicalUnit meter_Kelvin_per_s = SI.m.CombineMultiply(SI.K).CombineDivide(SI.s);

            // Create value in units m·°C/s
            PhysicalQuantity valueOfmeterCelsiusPerSecond = new PhysicalQuantity(2, meter_Ce_per_s);
            //Console.WriteLine("Base value : {0}", valueOfmeterCelsiusPerSecond); 
            // prints 2 m·°C/s

            // Convert m·°C/s to m·K/s
            IPhysicalQuantity valueOfmeterKelvinPerSecond = valueOfmeterCelsiusPerSecond.ConvertTo(meter_Kelvin_per_s);
            //Console.WriteLine("Base value converted to {0} : {1}", meter_Ce_per_s, valueOfmeterKelvinPerSecond);
            // prints 548.3 m·K/s - correct conversion ??

            // Convert m·K/s back to m·°C/s 
            IPhysicalQuantity valueOfmeter_KelvinPerSecondConvertedToMeter_Ce_per_s = valueOfmeterKelvinPerSecond.ConvertTo(meter_Ce_per_s);

            //Console.WriteLine("{0} converted back to {1}: {2}", meter_Kelvin_per_s, meter_Ce_per_s, valueOfmeterKelvinPerSecond.ConvertTo(meter_Ce_per_s));
            // prints 1.0036476381543 m·°C/s - should print 2 m·°C/s - incorrect conversion ??

            Assert.AreEqual(valueOfmeterCelsiusPerSecond, valueOfmeter_KelvinPerSecondConvertedToMeter_Ce_per_s);
        }



        #endregion PhysicalQuantity.Equal test


        #region PhysicalQuantity ConvertTo test

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityOfConvertibleUnitBasedOnDerivedUnitConvertToDerivedUnit_kWh()
        {
            PhysicalUnit kWh = (PhysicalUnit)new ConvertibleUnit("kiloWattHour", "kWh", SI.J, new ScaledValueConversion(1.0/3600000)); /* [kWh] = 1/3600000 * [J] */
            PhysicalQuantity conso = new PhysicalQuantity(1, kWh);
            IPhysicalQuantity actual = conso.ConvertTo(SI.J);

            PhysicalQuantity expected = new PhysicalQuantity(3600000, SI.J);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityOfConvertibleUnitBasedOnDerivedUnitConvertToDerivedUnit_Wh()
        {
            PhysicalUnit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0/3600)); /* [Wh] = 1/3600 * [J] */
            PhysicalQuantity E_1 = new PhysicalQuantity(1, Prefix.K * Wh);
            PhysicalQuantity E_2 = new PhysicalQuantity(0.001, Prefix.M * Wh);
            IPhysicalQuantity actual_1 = E_1.ConvertTo(SI.J);
            IPhysicalQuantity actual_2 = E_2.ConvertTo(SI.J);

            PhysicalQuantity expected = new PhysicalQuantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityOfConvertibleUnitBasedOnConvertibleUnitConvertToDerivedUnit()
        {
            /* It is NOT encouraged to do like this. Just for test */
            PhysicalUnit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */
            PhysicalUnit kWh = new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            PhysicalUnit MWh = new ConvertibleUnit("MegaWattHour", "MWh", kWh, new ScaledValueConversion(1.0 / 1000)); /* [MWh] = 1/1000 * [kWh] */
            PhysicalQuantity E_1 = new PhysicalQuantity(1000, Wh);
            PhysicalQuantity E_2 = new PhysicalQuantity(1, kWh);
            PhysicalQuantity E_3 = new PhysicalQuantity(0.001, MWh);
            IPhysicalQuantity actual_1 = E_1.ConvertTo(SI.J);
            IPhysicalQuantity actual_2 = E_2.ConvertTo(SI.J);
            IPhysicalQuantity actual_3 = E_3.ConvertTo(SI.J);

            PhysicalQuantity expected = new PhysicalQuantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
            Assert.AreEqual(expected, actual_3);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityOfConvertibleUnitBasedOnConvertibleUnitConvertToDerivedUnitSquareOperator()
        {
            /* It is NOT encouraged to do like this. Just for test */
            PhysicalUnit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */
            PhysicalUnit kWh = new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            PhysicalUnit MWh = new ConvertibleUnit("MegaWattHour", "MWh", kWh, new ScaledValueConversion(1.0 / 1000)); /* [MWh] = 1/1000 * [kWh] */
            PhysicalQuantity E_1 = new PhysicalQuantity(1000, Wh);
            PhysicalQuantity E_2 = new PhysicalQuantity(1, kWh);
            PhysicalQuantity E_3 = new PhysicalQuantity(0.001, MWh);
            IPhysicalQuantity actual_1 = E_1 [SI.J];
            IPhysicalQuantity actual_2 = E_2 [SI.J];
            IPhysicalQuantity actual_3 = E_3 [SI.J];

            PhysicalQuantity expected = new PhysicalQuantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
            Assert.AreEqual(expected, actual_3);
        }

        #endregion PhysicalQuantity ConvertTo test

        #region PhysicalQuantity compare operation test

        /// <summary>
        ///A test for == operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorEqualsTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, (Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]));
            PhysicalQuantity pg2 = new PhysicalQuantity(456, (Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]));

            PhysicalQuantity expected = new PhysicalQuantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            IPhysicalQuantity actual = pg1 + pg2;

            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for != operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorNotEqualsTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalQuantity expected = new PhysicalQuantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            IPhysicalQuantity actual = pg1 + pg2;

            Assert.IsFalse(expected != actual);
        }

        /// <summary>
        ///A test for < operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorLessTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            Assert.IsTrue(pg1 < pg2);
        }

        /// <summary>
        ///A test for <= operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorLessOrEqualsTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            Assert.IsTrue(pg1 <= pg2);
        }

        /// <summary>
        ///A test for > operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorLargerTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            Assert.IsFalse(pg1 > pg2);
        }

        /// <summary>
        ///A test for > operator
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_CompareOperatorLargerOrEqualTest()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            Assert.IsFalse(pg1 > pg2);
        }

        #endregion PhysicalQuantity compare operation test

        #region PhysicalQuantity math test

        /// <summary>
        ///A test for add operator
        ///</summary>
        [TestMethod()]
        public void AddKiloGramToMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalQuantity expected = new PhysicalQuantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            IPhysicalQuantity actual = pg1 + pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sub operator
        ///</summary>
        [TestMethod()]
        public void SubKiloGramFromMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(789, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalQuantity expected = new PhysicalQuantity(0.000123- 789, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalQuantity actual = pg1 - pg2;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultKiloGramToMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalUnit MassSquared = new DerivedUnit(Physics.SI_Units, new SByte[] { 0, 2, 0, 0, 0, 0, 0 });

            PhysicalQuantity expected = new PhysicalQuantity(0.000123 * 456 , MassSquared);

            PhysicalQuantity actual = pg1 * pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void DivKiloGramFromMilliGram()
        {
            PhysicalQuantity pg1 = new PhysicalQuantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);
            PhysicalQuantity pg2 = new PhysicalQuantity(789, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalQuantity expected = new PhysicalQuantity(0.000123 / 789, Physics.dimensionless);

            PhysicalQuantity actual = pg1 / pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PowerOperatorCalculateEnergyIn1Gram()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalUnit MeterPrSecond = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });
            PhysicalQuantity c = new PhysicalQuantity(299792458, MeterPrSecond);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, Physics.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * c.Pow(2);
            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for DerivedUnit with exponent of absolute value larger than 1
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyOf1GramAfterFalling10MeterAtEarthSurface()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            PhysicalUnit MeterPrSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPrSecond2);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 9.81 * 10, Physics.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = m * g * h;

            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for power operator in PhysicalQuantity parse
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyOf1GramAfterFalling10MeterAtEarthSurfaceParsePowerOperator()
        {
            PhysicalQuantity m = PhysicalQuantity.Parse("1 g") as PhysicalQuantity;
            PhysicalQuantity h = PhysicalQuantity.Parse("10 m") as PhysicalQuantity;

            //!!! To do: make this work: PhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");
            //PhysicalUnit MeterPrSecond2 = new DerivedUnit(PhysicalMeasure.Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            //PhysicalQuantity g = new PhysicalQuantity(9.81, MeterPrSecond2);
            IPhysicalQuantity g = PhysicalQuantity.Parse("9.81 m/s^2");

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 9.81 * 10, Physics.SI_Units.UnitFromSymbol("J"));

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

            // Must not compile:  
            // PhysicalQuantity m_plus_h = m + h;

            // Must not compile:  
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

            // Must not compile:  
            // PhysicalQuantity m_sub_h = m - h;

            // Must not compile:  
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

            //String expected = (123.4).ToString()+" SI.Kg";
            //String expected = (123.4).ToString(CultureInfo.InvariantCulture) + " Kg";
            String expected = (123.4).ToString() + " Kg";

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

            //String expected = (0.0981).ToString()+" SI.J";
            //String expected = (0.0981).ToString(CultureInfo.InvariantCulture) + " J";
            String expected = (0.0981).ToString() + " J";

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

            //String expected = (0.00987654321).ToString() + " SI.ms-2";
            //String expected = (0.00987654321).ToString(CultureInfo.InvariantCulture) + " m·s-2";
            String expected = (0.00987654321).ToString() + " m·s-2";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }


        #endregion PhysicalQuantity ToString test

        #region PhysicalQuantity functions test

        /// <summary>
        ///A test for PhysicalQuantity 
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_PhysicalQuantityGeVTest()
        {

            PhysicalQuantity GeV = Prefix.G * Constants.e * SI.V;

            PhysicalQuantity GeVPowMinus2 = GeV ^ -2;
            PhysicalQuantity GF = new PhysicalQuantity(1.16639E-5 * GeVPowMinus2.Value, GeVPowMinus2.Unit);

            Assert.IsNotNull(GF);
        }


        PhysicalQuantity EnergyEquivalentOfMass(PhysicalQuantity mass)
        {
            /* Assert. ...(mass.Unit). ... == MeasureKind.Mass); */
            PhysicalQuantity E = mass * Constants.c.Pow(2);
            return E;
        }

        /// <summary>
        ///A test for PhysicalQuantity 
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityTest_PhysicalQuantityFunctionTest()
        {
            PhysicalQuantity m = new PhysicalQuantity(0.001, Physics.SI_Units.BaseUnits[(int)(PhysicsBaseQuantityKind.Mass)]);

            PhysicalUnit MeterPrSecond = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });
            PhysicalQuantity c = new PhysicalQuantity(299792458, MeterPrSecond);

            PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, Physics.SI_Units.UnitFromSymbol("J"));

            PhysicalQuantity E = EnergyEquivalentOfMass(m);
            Assert.AreEqual(expected, E);
        }


        /// <summary>
        ///A test for PhysicalQuantity 
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityHectoLitreTest()
        {

            PhysicalUnit cubicmeter = new NamedDerivedUnit(Physics.SI_Units, "cubicmeter", "m3", new SByte[] { 3, 0, 0, 0, 0, 0, 0 });

            // PhysicalUnit hl = new ConvertibleUnit("hectolitre", "hl", SI.m3, new ScaledValueConversion(1/10));
            //PhysicalUnit kWh = (PhysicalUnit)new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            PhysicalUnit hl = new ConvertibleUnit("hectolitre", "hl", cubicmeter, new ScaledValueConversion(10)); /* [hl] = 10 * [cubicmeter] */

            PhysicalQuantity _10_hectolitre = new PhysicalQuantity(10, hl);

            //IPhysicalQuantity hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(SI.m3);
            IPhysicalQuantity _10_hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(cubicmeter);

            PhysicalQuantity expected = new PhysicalQuantity(1, Physics.SI_Units.UnitFromSymbol("m").Pow(3));
            Assert.AreEqual(expected, _10_hektoLiterIncubicmeters);
        }


        /// <summary>
        ///A test for PhysicalQuantity 
        ///</summary>
        [TestMethod()]
        public void PhysicalQuantityLitreTest()
        {

            PhysicalUnit hl = new ConvertibleUnit("hectolitre", "hl", SI.l, new ScaledValueConversion(1.0/100)); /* [hl] = 1/100 * [l] */
        
            PhysicalQuantity _10_hectolitre = new PhysicalQuantity(10, hl);

            PhysicalUnit cubicmeter = SI.m^3;
            IPhysicalQuantity _10_hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(SI.m^3);

            PhysicalQuantity expected = new PhysicalQuantity(1, SI.m^3);
            Assert.AreEqual(expected, _10_hektoLiterIncubicmeters);
        }


        #endregion PhysicalQuantity functions test

    }
}
