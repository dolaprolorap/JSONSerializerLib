using JSONSerializerLib;

namespace JSONSerializerLibTests
{
    [TestClass]
    public class JSONSerializerTests
    {
        [TestMethod]
        public void TestInt()
        {
            int i1 = 1;
            int i2 = -12415;
            int i3 = 0;
            int i4 = 12515;
            int i5 = int.MaxValue;
            int i6 = int.MinValue;

            string str1 = JSONSerializer.Serialize(i1);
            string str2 = JSONSerializer.Serialize(i2);
            string str3 = JSONSerializer.Serialize(i3);
            string str4 = JSONSerializer.Serialize(i4);
            string str5 = JSONSerializer.Serialize(i5);
            string str6 = JSONSerializer.Serialize(i6);

            Assert.AreEqual("1", str1);
            Assert.AreEqual("-12415", str2);
            Assert.AreEqual("0", str3);
            Assert.AreEqual("12515", str4);
            Assert.AreEqual(int.MaxValue.ToString(), str5);
            Assert.AreEqual(int.MinValue.ToString(), str6);

            int new_i1 = JSONSerializer.Deserialize<int>(str1);
            int new_i2 = JSONSerializer.Deserialize<int>(str2);
            int new_i3 = JSONSerializer.Deserialize<int>(str3);
            int new_i4 = JSONSerializer.Deserialize<int>(str4);
            int new_i5 = JSONSerializer.Deserialize<int>(str5);
            int new_i6 = JSONSerializer.Deserialize<int>(str6);

            Assert.AreEqual(i1, new_i1);
            Assert.AreEqual(i2, new_i2);
            Assert.AreEqual(i3, new_i3);
            Assert.AreEqual(i4, new_i4);
            Assert.AreEqual(i5, new_i5);
            Assert.AreEqual(i6, new_i6);
        }

        [TestMethod]
        public void TestDouble()
        {
            double d1 = 1.124124;
            double d2 = 0.0000001;
            double d3 = -0.0000001;
            double d4 = 12412412;
            double d5 = -12412412;
            double d6 = 12412412.00000001;
            double d7 = -12412412.00000001;
            double d8 = double.Epsilon;
            double d9 = double.MaxValue;
            double d10 = double.MinValue;

            string str1 = JSONSerializer.Serialize(d1);
            string str2 = JSONSerializer.Serialize(d2);
            string str3 = JSONSerializer.Serialize(d3);
            string str4 = JSONSerializer.Serialize(d4);
            string str5 = JSONSerializer.Serialize(d5);
            string str6 = JSONSerializer.Serialize(d6);
            string str7 = JSONSerializer.Serialize(d7);
            string str8 = JSONSerializer.Serialize(d8);
            string str9 = JSONSerializer.Serialize(d9);
            string str10 = JSONSerializer.Serialize(d10);

            Assert.AreEqual(d1.ToString(), str1);
            Assert.AreEqual(d2.ToString(), str2);
            Assert.AreEqual(d3.ToString(), str3);
            Assert.AreEqual(d4.ToString(), str4);
            Assert.AreEqual(d5.ToString(), str5);
            Assert.AreEqual(d6.ToString(), str6);
            Assert.AreEqual(d7.ToString(), str7);
            Assert.AreEqual(d8.ToString(), str8);
            Assert.AreEqual(d9.ToString(), str9);
            Assert.AreEqual(d10.ToString(), str10);

            double new_d1 = JSONSerializer.Deserialize<double>(str1);
            double new_d2 = JSONSerializer.Deserialize<double>(str2);
            double new_d3 = JSONSerializer.Deserialize<double>(str3);
            double new_d4 = JSONSerializer.Deserialize<double>(str4);
            double new_d5 = JSONSerializer.Deserialize<double>(str5);
            double new_d6 = JSONSerializer.Deserialize<double>(str6);
            double new_d7 = JSONSerializer.Deserialize<double>(str7);
            double new_d8 = JSONSerializer.Deserialize<double>(str8);
            double new_d9 = JSONSerializer.Deserialize<double>(str9);
            double new_d10 = JSONSerializer.Deserialize<double>(str10);

            Assert.AreEqual(new_d1, d1);
            Assert.AreEqual(new_d2, d2);
            Assert.AreEqual(new_d3, d3);
            Assert.AreEqual(new_d4, d4);
            Assert.AreEqual(new_d5, d5);
            Assert.AreEqual(new_d6, d6);
            Assert.AreEqual(new_d7, d7);
            Assert.AreEqual(new_d8, d8);
            Assert.AreEqual(new_d9, d9);
            Assert.AreEqual(new_d10, d10);
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
        }

        [TestMethod]
        public void TestList()
        {
            List<int> l1 = new List<int> { 1, 2, 5 };
            List<double> l2 = new List<double> { 1.001, 21.01, double.MaxValue, 0.00000001 };
            List<byte> l3 = new List<byte> { 1, 3, 4, 4, 4, 4 };
            List<short> l4 = new List<short> { -1, 3, 2 };
            List<bool> l5 = new List<bool> { true, false, true };
            List<TestClass1> l6 = new List<TestClass1> { new TestClass1(0, 0, ""), new TestClass1(120, decimal.MinValue, "  11  11  11  ") };
            List<List<int>> l7 = new List<List<int>> { new List<int> { 1, 2, 4, 4 }, new List<int> { 9, 2, 5 }, new List<int> { }, new List<int> { 0, 0, 0 } };
            List<Dictionary<int, string>> l8 = new List<Dictionary<int, string>> { new Dictionary<int, string> { { 1, "asd" }, { 2, "adf" }, { 3, "fsda" } },
                new Dictionary<int, string> { { 1, "asd" }, { 2, "adf" }, { 3, "fsda" } },
                new Dictionary<int, string> { { 1, "14" }, { 2, "124" }, { 3, "1240q" } } };

            string str1 = "[\"System.Int32\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1, 2, 5]";
            string str2 = "[\"System.Double\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1.001, 21.01, 1.7976931348623157E+308, 1E-08]";
            string str3 = "[\"System.Byte\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", 1, 3, 4, 4, 4, 4]";
            string str4 = "[\"System.Int16\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", -1, 3, 2]";
            string str5 = "[\"System.Boolean\", \"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", true, false, true]";
            string str6 = "[\"Tester.TestClass1\", \"Tester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\", {\r\n\t\"_classname\": \"Tester.TestClass1\",\r\n\t\"_assemblyname\": \"Tester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\r\n\t\"a\": 0,\r\n\t\"b\": 0,\r\n\t\"c\": \"\"\r\n}, {\r\n\t\"_classname\": \"Tester.TestClass1\",\r\n\t\"_assemblyname\": \"Tester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\r\n\t\"a\": 120,\r\n\t\"b\": -7.922816251426434E+28,\r\n\t\"c\": \"  11  11  11  \"\r\n}]";
        }
    }
}