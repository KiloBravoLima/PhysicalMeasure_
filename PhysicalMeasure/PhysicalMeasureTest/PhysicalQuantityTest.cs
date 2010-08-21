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


        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTestMilliGram()
        {
            string s = "123.000 mg"; 
            NumberStyles styles = NumberStyles.Float;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            IPhysicalQuantity expected = (IPhysicalQuantity)(new PhysicalQuantity(0.000123, (IPhysicalUnit)(PhysicalMeasure.Physic.SI_Units.BaseUnits[(int)(MeasureKind.mass)])));
            IPhysicalQuantity actual;
            actual = PhysicalQuantity.Parse(s, styles, provider);
            Assert.AreEqual(expected, actual);
        }
    }
}
