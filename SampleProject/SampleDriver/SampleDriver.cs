using System;
using System.Text;

namespace SampleProject
{
    using TestInterface;

    public class TestDriver : MarshalByRefObject, ITest
    {
        SampleCode demoTestCode;
        StringBuilder ResultLog;

        public TestDriver()
        {
            demoTestCode = new SampleCode();
            ResultLog = new StringBuilder();
        }

        public static ITest create()
        {
            return new TestDriver();
        }

        public bool test()
        {
            bool bTestResult = true;

            // Call Test Function 1
            demoTestCode.Display("Simulating Tests");
            ResultLog.AppendLine("demoTestCode.Display...PASS.");

            // Call Test Function 2 ...Positive Test
            if (false == demoTestCode.Simulate_Pass_Test())
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Pass_Test()...FAIL.");
                bTestResult = false;
            }
            else
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Pass_Test()...PASS.");
            }

            // Call Test Function 2 ...Negative Test
            if (false == demoTestCode.Simulate_Fail_Test())
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Fail_Test()...FAIL.");
                bTestResult = false;
            }
            else
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Fail_Test()...PASS.");
            }

            // Call Test Function 3 ...Handled Exception
            if (false == demoTestCode.Simulate_Handled_Exception())
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Handled_Exception()...FAIL.");
                bTestResult = false;
            }
            else
            {
                ResultLog.AppendLine("demoTestCode.Simulate_Handled_Exception()...PASS.");
            }

            // Call Test Function 4 ...UnHandled Exception
            if (false == demoTestCode.Simulate_UnHandled_Exception())
            {
                ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...FAIL.");
                bTestResult = false;
            }
            else
            {
                ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...PASS.");
            }

            return bTestResult;
        }

        public string getLog()
        {
            return ResultLog.ToString();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Local test:\n");

            ITest test = TestDriver.create();

            if (test.test() == true)
            {
                Console.WriteLine("\n>>>Test PASS<<<");
            }
            else
            {
                Console.WriteLine("\n>>>Test FAIL<<<");
            }

            Console.WriteLine("\nTest Logs:");
            Console.WriteLine("{0}", test.getLog());
        }
    }
}
