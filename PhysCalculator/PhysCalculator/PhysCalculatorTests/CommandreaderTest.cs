﻿using PhysCalc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for CommandreaderTest and is intended
    ///to contain all CommandreaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommandreaderTest
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
        ///A test for Commandreader Constructor
        ///</summary>
        [TestMethod()]
        public void CommandreaderConstructorTest_WithoutParams()
        {
            Commandreader target = new Commandreader();
            Assert.IsNotNull(target);
        }


        /// <summary>
        ///A test for Commandreader Constructor
        ///</summary>
        [TestMethod()]
        public void CommandreaderConstructorTest_NoArgs()
        {
            string[] args = {};
            Commandreader target = new Commandreader(args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Commandreader Constructor
        ///</summary>
        [TestMethod()]
        public void CommandreaderConstructorTest_ReadFileArgs()
        {
            string[] args = {"read test" };
            Commandreader target = new Commandreader(args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Commandreader Constructor
        ///</summary>
        [TestMethod()]
        public void CommandreaderConstructorTest_CommandArgs()
        {
            string[] args = { "1 + 2" };
            Commandreader target = new Commandreader(args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Commandreader Constructor
        ///</summary>
        [TestMethod()]
        public void CommandreaderConstructorTest_MixedArgs()
        {
            string[] args = { "read test", "1 + 2" };
            Commandreader target = new Commandreader(args);
            Assert.IsNotNull(target);
            //Assert.AreEqual("test.cal", target.GetFile());
        }
        
        /// <summary>
        ///A test for GetFile
        ///</summary>
        [TestMethod()]
        public void GetFileTest_FileNotFound()
        {
            string[] args = { };
            Commandreader target = new Commandreader(args);
            string actual;
            actual = target.GetFile();
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for GetFile
        ///</summary>
        [TestMethod()]
        public void GetFileTest_FileFound()
        {
            string[] args = { "read test" };
            Commandreader target = new Commandreader(args);
            string expected = null;
            string actual;
            actual = target.GetFile();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HasFile
        ///</summary>
        [TestMethod()]
        public void HasFileTest_NoParams()
        {
            Commandreader target = new Commandreader();
            bool expected = false; 
            bool actual;
            actual = target.HasFile();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HasFile
        ///</summary>
        [TestMethod()]
        public void HasFileTest_FileFound()
        {
            string[] args = { "read test" };
            Commandreader target = new Commandreader(args);
            bool expected = false; 
            bool actual;
            actual = target.HasFile();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ReadCommand
        ///</summary>
        [TestMethod()]
        public void ReadCommandTest()
        {
            string[] args = { "read test" };
            Commandreader target = new Commandreader(args);
            string ResultLine = string.Empty;
            string ResultLineExpected = string.Empty;
            string CommandLine = string.Empty; 
            string CommandLineExpected = "read test"; 
            bool expected = true; 
            bool actual;
            actual = target.ReadCommand(ref ResultLine, out CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReadFromFile
        ///</summary>
        [TestMethod()]
        public void ReadFromFileTest()
        {
            string[] args = { "read test" };
            Commandreader target = new Commandreader(args);
            string ResultLine = string.Empty; 
            string ResultLineExpected = ""; 
            string expected = null;
            string actual;
            actual = target.ReadFromFile(ref ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SetFile
        ///</summary>
        [TestMethod()]
        public void SetFileTest()
        {
            Commandreader target = new Commandreader(); 
            string filename = "testfilename";
            target.SetFile(filename);
            string expected = "testfilename";
            string actual = target.GetFile();
            Assert.AreEqual(expected, actual);
        }

        /*****************
        /// <summary>
        ///A test for FileName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void FileNameTest()
        {
            Commandreader_Accessor target = new Commandreader_Accessor(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.FileName = expected;
            actual = target.FileName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        *****************/
    }
}
