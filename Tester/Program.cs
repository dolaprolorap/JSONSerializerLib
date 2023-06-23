using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using JSONSerializerLib;

namespace Tester
{
    class MyClass
    {
        public int a;
        public int b;
        public Dictionary<string, int> dict = new Dictionary<string, int>();
        public Dictionary<string, int> d1 = new Dictionary<string, int>();
        public List<float> ll = new List<float>();
        public cl c1 = new cl();
        public cl c2;

        public MyClass() 
        {
            dict["1"] = 234;
            dict["fds"] = 1029;
            dict["314"] = 888;
            c1.bb = 123;
            for(int i = 0; i < 4; i++)
            {
                ll.Add((float)Math.Sin(i));
            }
        }
    }

    class cl
    {
        public int bb = 2;
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

    internal class Program
    {
        static void Main(string[] args)
        {
            List<TestClass1> l6 = new List<TestClass1> { new TestClass1(0, 0, ""), new TestClass1(120, decimal.MinValue, "  11  11  11  ") };

            string str = JSONSerializer.Serialize(l6);

            Console.WriteLine(str);
        }
    }
}