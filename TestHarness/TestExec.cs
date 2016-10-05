/**
 * Test Executive
 * Serializes test requests and creates new app domain to process individual test request
 *      
 * FileName     : TestExec.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace TestHarness
{
    using XMLParser;
    using FileManager;

    class TestExec
    {
        /**********************************************************************
                                 M E M B E R S
         **********************************************************************/

        /**
         * Define queue that will serialize test requests
         * The queue holds xml file path denoting a test request
         * 
         * Do we want this queue to be static? We could have multiple TestExec instances 
         * but would want only one queue to manage test requests
         */
        Queue<string> TestQueue;

        public string RepositoryPath { get; set; }

        /**********************************************************************
                                 P U B L I C   M E T H O D S
         **********************************************************************/

        public TestExec(string path)
        {
            RepositoryPath = path;
            TestQueue = new Queue<string>();
        }

        public void EnqueueTestRequest(string TestRequestFile)
        {
            TestQueue.Enqueue(TestRequestFile);
            Console.WriteLine("REQUIREMENT 3 : En-queued ({0})", TestRequestFile);
        }

        public string DequeueTestRequest()
        {
            string TestRequestFile;

            TestRequestFile = TestQueue.Dequeue();
            Console.WriteLine("REQUIREMENT 3 : De-queued ({0})", TestRequestFile);

            return TestRequestFile;
        }

        public void ProcessTestRequests()
        {
            try
            {
                AppDomainMgr AppDMgr = new AppDomainMgr(RepositoryPath);

                do
                {
                    /**
                     * @todo We may want to create threads and process test request in that
                     */

                    /* De-queue Test Request */
                    string TestRequest = DequeueTestRequest();

                    Console.WriteLine("\n====START TEST REQUEST ({0})====", TestRequest);

                    /* Pass test request to app domain */
                    bool bRet = AppDMgr.ProcessTestRequest(TestRequest);
                    if (false == bRet)
                    {
                        Console.WriteLine("Error: {0}({1})...FAILED.", AppDMgr.GetType().FullName, TestRequest);
                        continue;
                    }

                    Console.WriteLine("====END TEST REQUEST ({0})====\n", TestRequest);

                } while (TestQueue.Count != 0);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }

        public void GetLogTestRequest(string TestRequest)
        {
            bool bRet;
            FileMgr Database = new FileMgr(RepositoryPath + "\\Log.txt");

            XmlParser Parser = new XmlParser();
            bRet = Parser.ParseXmlFile(TestRequest);
            if (false == bRet)
            {
                Console.WriteLine("Error: Parser.ParseTestRequest({0})...FAILED", TestRequest);
                return;
            }

            foreach (xmlTestInfo test in Parser._xmlTestInfoList)
            {
                Console.WriteLine("<<<{0}>>>", test._TestDriver);
                Console.WriteLine("{0}", Database.GetDriverTestResult(test._TestDriver));
            }
        }

        public void GetLogSummary()
        {
            FileMgr Database = new FileMgr(RepositoryPath + "\\Log.txt");

            Console.WriteLine("REQUIREMENT 8 : SIMPLE SUMMARY INFORMATION");
            Database.DisplayTestSummary();
        }

        public void GetLogFile()
        {
            FileMgr Database = new FileMgr(RepositoryPath + "\\Log.txt");

            Console.WriteLine("REQUIREMENT 8 : ENTIRE LOG FILE");
            Database.DisplayLog();
        }
    }
}