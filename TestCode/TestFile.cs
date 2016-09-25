using System;

namespace TestCode
{
    public class TestCode
    {
        int data;

        public TestCode()
        {
            data = 0;
        }

        public bool IsNonZero(int i)
        {
            data = i;

            return (0 != data) ? true : false;
        }

        public void Display(string msg)
        {
            Console.WriteLine("TestCode::Display({0})", msg);
        }

        static void Main(string[] args)
        {
            TestCode tc = new TestCode();

            tc.Display("HelloWorld");
            Console.WriteLine("tc.IsNonZero(5) : {0}", tc.IsNonZero(5) ? "TRUE" : "FALSE");
            Console.WriteLine("tc.IsNonZero(0) : {0}", tc.IsNonZero(0) ? "TRUE" : "FALSE");
            
        }
    }
}
