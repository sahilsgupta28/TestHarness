using System;
using System.Text;

namespace TestDrivers 
{
    using TestInterface;
    using TestCode;

    public class TestDriver : ITest
    {
        TestCode demoTestCode;
        StringBuilder ResultLog;

        public TestDriver()
        {
            demoTestCode = new TestCode();
            ResultLog = new StringBuilder();
        }

        public static ITest create()
        {
            return new TestDriver();
        }

        public bool test()
        {
            bool bTestResult = true;

            ResultLog.AppendLine("==================T.E.S.T  R.E.S.U.L.T.===================");

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
            //if (false == demoTestCode.Simulate_UnHandled_Exception())
            //{
            //    ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...FAIL.");
            //    bTestResult = false;
            //}
            //else
            //{
            //    ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...PASS.");
            //}

            ResultLog.AppendLine("==========================================================");

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
                Console.WriteLine("Test passed");
            }
            else
            {
                Console.WriteLine("Test failed");
            }

            Console.WriteLine("{0}", test.getLog());
        }
    }
}
