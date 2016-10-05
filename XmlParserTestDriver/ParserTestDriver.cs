/**
 * Sample Test Driver
 * Simulates a Test Driver for projects
 * 
 * FileName     : SampleDriver.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.Text;

namespace XmlParserTestDriver
{
    using System.Reflection;
    using TestInterface;
    using XMLParser;

    public class ParserTestDriver : MarshalByRefObject, ITest
    {
        /**********************************************************************
                                M E M B E R S
         **********************************************************************/

        XmlParser Parser;
        StringBuilder ResultLog;

        /**********************************************************************
                                P U B L I C   M E T H O D S
         **********************************************************************/

        public ParserTestDriver()
        {
            Parser = new XmlParser();
            ResultLog = new StringBuilder();
        }

        /**
         * test()
         * Implements iTest Interface function to test code
         */
        public bool test()
        {
            bool bTestResult = true;

            Console.WriteLine("REQUIREMENT 5: Test Driver ({0}) Current AppDomain ({1})", Assembly.GetExecutingAssembly().ToString(), AppDomain.CurrentDomain.FriendlyName);

            //string XmlPath = @"..\..\..\TestRequest\SampleCodeTestRequest.xml";
            string XmlPath = @".\TestRequest\SampleCodeTestRequest.xml";

            bTestResult = Parser.ParseXmlFile(XmlPath);
            if (false == bTestResult)
            {
                ResultLog.AppendLine("Error: Parser.ParseTestRequest()...FAILED");
                ResultLog.AppendLine("XML Path :");
                ResultLog.Append(XmlPath);
                return false;
            }

            ResultLog.AppendLine("Parser.ParseXmlFile...PASS.");

            if ("Sahil Gupta" != Parser._xmlTestInfoList[0]._Author)
            {
                ResultLog.AppendLine("Error: Aurthor Name Mismatch.");
                ResultLog.AppendLine("Expected : Sahil Gupta");
                ResultLog.AppendLine("Found: ");
                ResultLog.Append(Parser._xmlTestInfoList[0]._Author);
                return false;
            }

            ResultLog.AppendLine("Parser Integrity Verified");

            return true;
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
                Console.WriteLine("Local test:");

                ITest test = new ParserTestDriver();

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
