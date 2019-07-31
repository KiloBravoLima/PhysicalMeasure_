using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KBL.ExtensionsTest
{
    [TestClass]
    public class UnitTest_ByteArrayExtensions
    {
        [TestMethod]
        public void TestMethodByteArrayExtensions1()
        {
            Byte[] TestData = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Byte[] TestData2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            Byte[] result = TestData.MaskAnd(TestData2);

        }

        [TestMethod]
        public void TestMethodByteArrayExtensions2()
        {
            Byte[] TestData = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Byte[] TestData2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            Byte[] result = TestData.MaskAnd(TestData2);
            Byte[] result2 = TestData2.MaskAnd(TestData);

            CollectionAssert.AreEqual(result, result2);
        }

        [TestMethod]
        public void TestMethodByteArrayExtensions3()
        {
            Byte[] TestData = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Byte[] TestData2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            Byte[] result = TestData.MaskNot();

        }

        [TestMethod]
        public void TestMethodByteArrayExtensions4()
        {
            Byte[] TestData = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Byte[] TestData2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            Byte[] result = TestData.MaskNot();
            Byte[] result2 = TestData.MaskXor(result);
            Byte[] result3 = result2.MaskNot();


            Assert.IsTrue(result2.All(e => e == 255));
            Assert.IsTrue(result3.All(e => e == 0));
        }
    }

    [TestClass]
    public class UnitTest_MathRounding
    {
        [TestMethod]
        public void TestMethod_BankersRounding_DefaultRounding()
        {
            Double[] TestData = { 0.1, 1.5, 2.5, 3.5, 4.5, 5.5, 6.7, 7.8, 8.9, 9.0, 10.1, 11.2, 12.3, 13.4, 14.5, 15.6, 16.7 };
            Double[] TestDataRounded;
            Int16[] ExpectedResults = { 0, 2, 2, 4, 4, 6, 7, 8, 9, 9, 10, 11, 12, 13, 14, 16, 17 };

            TestDataRounded = TestData.Select(e => Math.Round(e)).ToArray();

            for (int i = 0; i < TestData.Count(); i++)
            {
                Assert.IsTrue(TestDataRounded[i] == ExpectedResults[i], $"{i} {TestData[i]} {TestDataRounded[i]} =!= {ExpectedResults[i]}");
            }
        }

        [TestMethod]
        public void TestMethod_RoundHalfAwayFromZero()
        {
            Double[] TestData = { 0.1, 1.5, 2.5, 3.5, 4.5, 5.5, 6.7, 7.8, 8.9, 9.0, 10.1, 11.2, 12.3, 13.4, 14.5, 15.6, 16.7 };
            Double[] TestDataRounded;
            Int16[] ExpectedResults = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 11, 12, 13, 15, 16, 17 };

            TestDataRounded = TestData.Select(e => Math.Round(e, MidpointRounding.AwayFromZero)).ToArray();

            for (int i = 0; i < TestData.Count(); i++)
            {
                Assert.IsTrue(TestDataRounded[i] == ExpectedResults[i], $"{i} {TestData[i]} {TestDataRounded[i]} =!= {ExpectedResults[i]}");
            }
        }

        [TestMethod]
        public void TestMethod_RoundHalfToEven()
        {
            Double[] TestData = { 0.1, 1.5, 2.5, 3.5, 4.5, 5.5, 6.7, 7.8, 8.9, 9.0, 10.1, 11.2, 12.3, 13.4, 14.5, 15.6, 16.7 };
            Double[] TestDataRounded;
            Int16[] ExpectedResults = { 0, 2, 2, 4, 4, 6, 7, 8, 9, 9, 10, 11, 12, 13, 14, 16, 17 };

            TestDataRounded = TestData.Select(e => Math.Round(e, MidpointRounding.ToEven)).ToArray();

            for (int i = 0; i < TestData.Count(); i++)
            {
                Assert.IsTrue(TestDataRounded[i] == ExpectedResults[i], $"{i} {TestData[i]} {TestDataRounded[i]} =!= {ExpectedResults[i]}");
            }
        }
    }
}