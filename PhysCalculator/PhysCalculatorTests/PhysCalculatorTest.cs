using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using PhysicalMeasure;
using CommandParser;
using PhysicalCalculator;


namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for PhysCalculatorTest and is intended
    ///to contain all PhysCalculatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhysCalculatorTest
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
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest()
        {
            string[] PhysCalculatorConfig_args = {"read test", "print 1 m + 2 Km"}; 
            PhysCalculator target = new PhysCalculator(PhysCalculatorConfig_args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest1()
        {
            Commandreader CommandLineReader = new Commandreader(); 
            string[] PhysCalculatorConfig_args = { "read test", "print 1 m + 2 Km" }; 
            PhysCalculator target = new PhysCalculator(CommandLineReader, PhysCalculatorConfig_args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest2()
        {
            PhysCalculator target = new PhysCalculator();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest3()
        {
            Commandreader CommandLineReader = null; 
            PhysCalculator target = new PhysCalculator(CommandLineReader);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Command
        ///</summary>
        [TestMethod()]
        public void CommandTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "Print 13 g + 99.987 Kg - 10 mg";
            string ResultLine = string.Empty;
            //string ResultLineExpected = "99.99999 Kg"; 
            string ResultLineExpected = "99999.99 g"; 
            bool expected = true; 
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandClear
        ///</summary>
        [TestMethod()]
        public void CommandClearTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = string.Empty; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty; 
            string ResultLineExpected = string.Empty; 
            bool expected = true; 
            bool actual;
            actual = target.CommandClear(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandComment
        ///</summary>
        [TestMethod()]
        public void CommandCommentTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "// test of comment"; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty;
            //string ResultLineExpected = "// test of comment"; 
            string ResultLineExpected = string.Empty;
            bool expected = true; 
            bool actual;
            actual = target.CommandComment(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandHelp
        ///</summary>
        [TestMethod()]
        public void CommandHelpTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = string.Empty; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty;
            /***
            string ResultLineExpected = 
                     "Commands:\n"
                    +"    Read <filename>\n"
                    +"    Save <filename>\n"
                    +"    Set <varname> = <expression>\n"
                    +"    Print <expression> [, <expression> ]* \n"
                    +"    Store <varname>\n"
                    +"    Remove <varname>\n"
                    +"    Clear\n"
                    +"    List\n"
                    +"    Read <filename>"; 
            ***/

            /***
            List<String> ResultLineExpected = new List<String>();
            ResultLineExpected.Add("Include <filename>");
            ResultLineExpected.Add("Save <filename>");
            ResultLineExpected.Add("Set <varname> [ = ] <expression> [, <varname> [ = ] <expression> ]*");
            ResultLineExpected.Add("[ Print ] <expression> [, <expression> ]*");
            ResultLineExpected.Add("List");
            ResultLineExpected.Add("Store <varname>");
            ResultLineExpected.Add("Remove <varname>");
            ResultLineExpected.Add("Clear");
            ***/

            List<String> ResultLineExpected = new List<String>();
            ResultLineExpected.Add("Include <filename>");
            ResultLineExpected.Add("Save <filename>");
            ResultLineExpected.Add("Set <varname> [ = ] <expression> [, <varname> [ = ] <expression> ]*");
            ResultLineExpected.Add("[ Print ] <expression> [, <expression> ]*");
            ResultLineExpected.Add("List");
            ResultLineExpected.Add("Store <varname>");
            ResultLineExpected.Add("Remove <varname>");
            ResultLineExpected.Add("Clear");

            bool expected = true; 
            bool actual;
            actual = target.CommandHelp(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < ResultLineExpected.Count; i++)
            {
                Assert.IsTrue(ResultLine.Contains(ResultLineExpected[i]), "ResultLine expected to contain " + ResultLineExpected[i] + ", but contains \"" + ResultLine + "\"");
            }

        }

        /// <summary>
        ///A test for CommandList
        ///</summary>
        [TestMethod()]
        public void CommandListTest()
        {
            PhysCalculator target = new PhysCalculator(); 

            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            //string ResultLineExpected = "12 m2·Kg·s-2";

            bool expected = true;
            bool actual;

            actual = target.CommandSet(ref CommandLine, ref ResultLine);


            CommandLine = string.Empty; 
            CommandLineExpected = string.Empty; 
            ResultLine = string.Empty;
            //ResultLineExpected = " Global:\r\nvar varname = 12 m2·Kg·s-2";
            List<String> ResultLinesExpected = new List<String>();
            ResultLinesExpected.Add("Global:");
            ResultLinesExpected.Add("var varname = 12 m2·Kg·s-2");

            expected = true;
            actual = target.CommandList(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            //Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < ResultLinesExpected.Count; i++)
            {
                Assert.IsTrue(ResultLine.Contains(ResultLinesExpected[i]), "ResultLine expected to contain " + ResultLinesExpected[i]);
            }

        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrintTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "123 J·S·Ω/m + 0.456 m·Kg·s-2"; 
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "123.456 J·S·Ω/m"; 
            bool expected = true; 
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandReadFromFile
        ///</summary>
        [TestMethod()]
        public void CommandReadFromFileTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = string.Empty; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty; 
            string ResultLineExpected = ""; 
            bool expected = true; 
            bool actual;
            actual = target.CommandReadFromFile(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandRemove
        ///</summary>
        [TestMethod()]
        public void CommandRemoveTest_VarNotFound()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "varname"; 
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "'varname' not known";
            bool expected = false; 
            bool actual;
            actual = target.CommandRemove(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandRemove
        ///</summary>
        [TestMethod()]
        public void CommandRemoveTest_VarFound()
        {
            PhysCalculator target = new PhysCalculator();

            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "12 m2Kgs-2";
            bool expected = true;
            bool actual;

            actual = target.CommandSet(ref CommandLine, ref ResultLine);

            CommandLine = "varname";
            CommandLineExpected = string.Empty;
            ResultLine = string.Empty;
            ResultLineExpected = string.Empty;
            expected = true;
            actual = target.CommandRemove(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandSaveToFile
        ///</summary>
        [TestMethod()]
        public void CommandSaveToFileTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "savetest.cal"; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty; 
            string ResultLineExpected = string.Empty; 
            bool expected = true; 
            bool actual;
            actual = target.CommandSaveToFile(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandSet
        ///</summary>
        [TestMethod()]
        public void CommandSetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "varname = 12 m2·Kg·s-2";
            bool expected = true; 
            bool actual;
            actual = target.CommandSet(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "CommandLine not as expected");
            Assert.AreEqual(ResultLineExpected, ResultLine, "ResultLine not as expected");
            Assert.AreEqual(expected, actual, "CommandSet() retur value not as expected");
        }

        /// <summary>
        ///A test for CommandStore
        ///</summary>
        [TestMethod()]
        public void CommandStoreTest()
        {
            PhysCalculator target = new PhysCalculator();

            // Set some value in accumulator
            string CommandLine = "3 N * 4 m";
            string ResultLine = string.Empty;
            target.CommandPrint(ref CommandLine, ref ResultLine);

            // Store accumulator value to var
            CommandLine = "Varname";
            string CommandLineExpected = string.Empty; 
            ResultLine = string.Empty;
            string ResultLineExpected = "12 m2·Kg·s-2";
            bool expected = true; 
            bool actual;
            actual = target.CommandStore(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /*****************
        /// <summary>
        ///A test for GetPhysicalQuantity
        ///</summary>
        [TestMethod()]
        public void GetPhysicalQuantityTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.GetPhysicalQuantity(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseConvertedExpression
        ///</summary>
        [TestMethod()]
        public void ParseConvertedExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseConvertedExpression(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseExpression
        ///</summary>
        [TestMethod()]
        public void ParseExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseExpression(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        *****************/

        /**
        /// <summary>
        ///A test for ParseExpressionList
        ///</summary>
        [TestMethod()]
        public void ParseExpressionListTest_1()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = " 1 , 2 m, 3 KgN, 4.5 J/K , 6 h,7 g, 8 Km , 9 mm , 10 mKgs-2 ";
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty; 
            string ResultLineExpected = string.Empty; 
            List<IPhysicalQuantity> expected = new List<IPhysicalQuantity>();
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse(" 1 "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse(" 2 m "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("3 KgN "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("4.5 J/K "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("6 h "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("7 g "));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("8 Km"));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("9 mm"));
            expected.Add(PhysicalMeasure.PhysicalQuantity.Parse("10 mKgs-2"));

            List<IPhysicalQuantity> actual;
            actual = target.ParseExpressionList(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);

            for (int i = 0; i < expected.Count; i++ )
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
        **/

        /// <summary>
        ///A test for ParseExpressionList
        ///</summary>
        [TestMethod()]
        public void RunArgCommandsTest()
        {

            string[] PhysCalculatorConfig_args =
            {
                "set SoundSpeed = 340 m/s",
                "set SoundSpeedInKmPrHour = SoundSpeed [ Km/h ]",
                "list;",
                "save unittest_1"
            }; 
            PhysCalculator target = new PhysCalculator(PhysCalculatorConfig_args);
            Assert.IsNotNull(target);

            target.Run();

            IPhysicalQuantity VarValue;
            IPhysicalQuantity VarValueExpected = new PhysicalQuantity(340, SI.m/SI.s);
            String ResultLine = null;
            bool GetVarRes = target.VariableGet(target.CurrentContext, "SoundSpeed", out VarValue, ref ResultLine);
            Assert.IsTrue(GetVarRes);
            Assert.AreEqual<IPhysicalQuantity>( VarValueExpected, VarValue);
      
            VarValueExpected = new PhysicalQuantity(1224, new CombinedUnit() *  new PrefixedUnitExponent(Prefix.K.PrefixExponent, SI.m, 1) /SI.h);
            GetVarRes = target.VariableGet(target.CurrentContext, "SoundSpeedInKmPrHour", out VarValue, ref ResultLine);
            Assert.IsTrue(GetVarRes);
            Assert.AreEqual<IPhysicalQuantity>( VarValueExpected, VarValue);
        }

        /*****************
        /// <summary>
        ///A test for ParseFactor
        ///</summary>
        [TestMethod()]
        public void ParseFactorTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseFactor(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalConvertedExpression
        ///</summary>
        [TestMethod()]
        public void ParseOptionalConvertedExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IPhysicalQuantity pq = null; // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseOptionalConvertedExpression(pq, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalExpression
        ///</summary>
        [TestMethod()]
        public void ParseOptionalExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IPhysicalQuantity pq = null; // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseOptionalExpression(pq, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalTerm
        ///</summary>
        [TestMethod()]
        public void ParseOptionalTermTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IPhysicalQuantity pq = null; // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseOptionalTerm(pq, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        

        /// <summary>
        ///A test for ParseTerm
        ///</summary>
        [TestMethod()]
        public void ParseTermTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string CommandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IPhysicalQuantity expected = null; // TODO: Initialize to an appropriate value
            IPhysicalQuantity actual;
            actual = target.ParseTerm(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        *****************/

        /// <summary>
        ///A test for VariableGet
        ///</summary>
        [TestMethod()]
        public void VariableGetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            IPhysicalQuantity VariableValue = 2 * SI.N * SI.m;
            bool expected = true;
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);

            IPhysicalQuantity VariableValueExpected = VariableValue;
            String ResultLine = null;
            actual = target.VariableGet(target.CurrentContext, VariableName, out VariableValue, ref ResultLine);
            Assert.AreEqual(VariableValueExpected, VariableValue);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariableRemove
        ///</summary>
        [TestMethod()]
        public void VariableRemoveTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            IPhysicalQuantity VariableValue = 2 * SI.N * SI.m;
            bool expected = true;
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);

            //actual = target..VariableRemove(VariableName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariableSet
        ///</summary>
        [TestMethod()]
        public void VariableSetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            IPhysicalQuantity VariableValue = 2 * SI.N * SI.m; 
            bool expected = true; 
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariablesClear
        ///</summary>
        [TestMethod()]
        public void VariablesClearTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            bool expected = true; 
            bool actual;
            // 
            actual = target.IdentifiersClear();
            //actual = target.IdentifiersClear();
            Assert.AreEqual(expected, actual);
        }
    }
}
