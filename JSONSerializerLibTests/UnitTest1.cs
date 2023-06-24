using JSONSerializerLib;
using System.Globalization;

namespace JSONSerializerLibTests
{
    [TestClass]
    public class JSONSerializerTests
    {
        [TestMethod]
        public void TestInt1()
        {
            int i = 1;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestInt2()
        {
            int i = -124251;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestInt3()
        {
            int i = 0;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestInt4()
        {
            int i = 1421241;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestInt5()
        {
            int i = int.MaxValue;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestInt6()
        {
            int i = int.MinValue;

            string str = JSONSerializer.Serialize(i);

            Assert.AreEqual(i.ToString(), str);

            int new_i = JSONSerializer.Deserialize<int>(str);

            Assert.AreEqual(i, new_i);
        }

        [TestMethod]
        public void TestDouble1()
        {
            double d = 1.124124;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble2()
        {
            double d = double.Epsilon;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble3()
        {
            double d = -0.0000001;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble4()
        {
            double d = 12412412;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble5()
        {
            double d = -12412412;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble6()
        {
            double d = double.MaxValue;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        [TestMethod]
        public void TestDouble7()
        {
            double d = double.MinValue;

            string str = JSONSerializer.Serialize(d);

            NumberFormatInfo ti = new();
            ti.NumberDecimalSeparator = ".";

            Assert.AreEqual(d.ToString(ti), str);

            double new_d = JSONSerializer.Deserialize<double>(str);

            Assert.AreEqual(new_d, d);
        }

        class TestClass1
        {
            public long a;
            public decimal b;
            public string c;

            public TestClass1(long a, decimal b, string c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public TestClass1()
            {

            }
        }

        [TestMethod]
        public void TestList1()
        {
            List<int> l = new List<int> { 1, 2, 5 };
            string ser_str_excepted = "[\"System.Int32\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1, 2, 5]";

            string ser_str = JSONSerializer.Serialize(l);

            Assert.AreEqual(ser_str_excepted, ser_str);

            List<int> des_l = JSONSerializer.Deserialize<List<int>>(ser_str);

            Assert.IsTrue(l.SequenceEqual(des_l));
        }

        [TestMethod]
        public void TestList2()
        {
            List<double> l = new List<double> { 1.001, 21.01, double.MaxValue, 0.00000001 };
            string ser_str_excepted = "[\"System.Double\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1.001, 21.01, 1.7976931348623157E+308, 1E-08]";

            string ser_str = JSONSerializer.Serialize(l);

            Assert.AreEqual(ser_str_excepted, ser_str);

            List<double> des_l = JSONSerializer.Deserialize<List<double>>(ser_str);

            Assert.IsTrue(l.SequenceEqual(des_l));
        }

        [TestMethod]
        public void TestList3()
        {
            List<byte> l = new List<byte> { 1, 3, 4, 4, 4, 4 };
            string ser_str_excepted = "[\"System.Byte\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1, 3, 4, 4, 4, 4]";

            string ser_str = JSONSerializer.Serialize(l);

            Assert.AreEqual(ser_str_excepted, ser_str);

            List<byte> des_l = JSONSerializer.Deserialize<List<byte>>(ser_str);

            Assert.IsTrue(l.SequenceEqual(des_l));
        }

        [TestMethod]
        public void TestList4()
        {
            List<short> l = new List<short> { -1, 3, 2 };
            string ser_str_excepted = "[\"System.Int16\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", -1, 3, 2]";

            string ser_str = JSONSerializer.Serialize(l);

            Assert.AreEqual(ser_str_excepted, ser_str);

            List<short> des_l = JSONSerializer.Deserialize<List<short>>(ser_str);

            Assert.IsTrue(l.SequenceEqual(des_l));
        }

        [TestMethod]
        public void TestList5()
        {
            List<bool> l = new List<bool> { true, false, true };
            string ser_str_excepted = "[\"System.Boolean\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", true, false, true]";

            string ser_str = JSONSerializer.Serialize(l);

            Assert.AreEqual(ser_str_excepted, ser_str);

            List<bool> des_l = JSONSerializer.Deserialize<List<bool>>(ser_str);

            Assert.IsTrue(l.SequenceEqual(des_l));
        }
    }
}