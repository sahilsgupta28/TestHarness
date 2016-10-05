/**
 * Sample Exception Test Driver
 * Simulates a Test Driver for projects with unhandled exception
 * 
 * FileName     : ExceptionTD.cs
 * Author       : Sahil Gupta
 * Date         : 5 October 2016 
 * Version      : 1.0
 */


using System;
using System.Text;

namespace ExceptionTestDriver
{
    using TestInterface;
    using SampleProject;
    using System.Reflection;

    public class ExceptionTD : MarshalByRefObject, ITest
    {
        /**********************************************************************
                                M E M B E R S
         **********************************************************************/

        SampleCode demoTestCode;
        StringBuilder ResultLog;

        /**********************************************************************
                                P U B L I C   M E T H O D S
         **********************************************************************/

        public ExceptionTD()
        {
            demoTestCode = new SampleCode();
            ResultLog = new StringBuilder();
        }

        /**
         * create
         * Return a new instance of TestDriver
         */
        public static ITest create()
        {
            return new ExceptionTD();
        }

        /**
         * test()
         * Implements iTest Interface function to test code
         */
        public bool test()
        {
            bool bTestResult = true;

            Console.WriteLine("REQUIREMENT 5: Test Driver ({0}) Current AppDomain ({1})", Assembly.GetExecutingAssembly().ToString(), AppDomain.CurrentDomain.FriendlyName);

            // Call Test Function 1
            demoTestCode.Display("Simulating Tests");
            ResultLog.AppendLine("demoTestCode.Display...PASS.");

            // Call Test Function 4 ...UnHandled Exception
            if (false == demoTestCode.Simulate_UnHandled_Exception())
            {
                ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...FAIL.");
                bTestResult = false;
            }
            else
                ResultLog.AppendLine("demoTestCode.Simulate_UnHandled_Exception()...PASS.");

            return bTestResult;
        }

        /**
         * getLog
         * Implements ITest interface function to return string result for test driver
         */
        public string getLog()
        {
            return ResultLog.ToString();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Local test:\n");

                ITest test = ExceptionTD.create();

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
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }
    }
}
