using System;

namespace TestDrivers 
{
    using TestInterface;
    using TestCode;

    public class TestDriver : ITest
    {
        TestCode demoTestCode;

        public TestDriver()
        {
            demoTestCode = new TestCode();
        }

        public static ITest create()
        {
            return new TestDriver();
        }

        public bool test()
        {
            // Call Test Function 1
            demoTestCode.Display("Test 1");
            
            // Call Test Function 2
            if (false == demoTestCode.IsNonZero(5))
            {
                // return false if a test fails
                return false;
            }

            //
            // Call additional test functions
            //
            
            // Return true if all tests pass.
            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Local test");

            ITest test = TestDriver.create();

            if (test.test() == true)
            {
                Console.WriteLine("Test passed");
            }
            else
            {
                Console.WriteLine("Test failed");
            }
        }
    }
}
