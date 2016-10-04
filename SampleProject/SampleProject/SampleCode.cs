using System;

namespace SampleProject
{
    public class SampleCode
    {
        public void Display(string msg)
        {
            Console.WriteLine("TestCode::Display({0})", msg);
        }

        public bool Simulate_Pass_Test()
        {
            Console.WriteLine("Simulating a pass test");
            return true;
        }

        public bool Simulate_Fail_Test()
        {
            Console.WriteLine("Simulating a fail test");
            return false;
        }

        public bool Simulate_Handled_Exception()
        {
            try
            {
                Console.WriteLine("Simulating Handled Exception");
                int i = 0;
                int j = 5 / i;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exeception : {0}", Ex.Message);
                return false;
            }
            return true;
        }

        public bool Simulate_UnHandled_Exception()
        {
            Console.WriteLine("Simulating Un-Handled Exception");
            int i = 0;
            int j = 5 / i;

            return true;
        }

        static void Main(string[] args)
        {
            SampleCode tc = new SampleCode();

            tc.Display("Simulating Tests");
            Console.WriteLine("tc.Simulate_Pass_Test() : {0}", tc.Simulate_Pass_Test() ? "TRUE" : "FALSE");
            Console.WriteLine("tc.Simulate_Fail_Test() : {0}", tc.Simulate_Fail_Test() ? "TRUE" : "FALSE");
            Console.WriteLine("tc.Simulate_Handled_Exception() : {0}", tc.Simulate_Handled_Exception() ? "TRUE" : "FALSE");
            //Console.WriteLine("tc.Simulate_UnHandled_Exception() : {0}", tc.Simulate_UnHandled_Exception() ? "TRUE" : "FALSE");

        }
    }
}

