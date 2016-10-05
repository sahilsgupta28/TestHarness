/**
 * Test Executive
 * Test Harness program to execute test drivers in isolated Application Domains
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

    public class TestExec
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

        /**
         * EnqueueTestRequest
         * Places a Test Request in a Queue
         */
        public void EnqueueTestRequest(string TestRequestFile)
        {
            TestQueue.Enqueue(TestRequestFile);
            Console.WriteLine("REQUIREMENT 3 : En-queued ({0})", TestRequestFile);
        }

        /**
         * DequeueTestRequest
         * Removes a test request from queue
         */
        public string DequeueTestRequest()
        {
            string TestRequestFile;

            TestRequestFile = TestQueue.Dequeue();
            Console.WriteLine("REQUIREMENT 3 : De-queued ({0})", TestRequestFile);

            return TestRequestFile;
        }

        /**
         * ProcessTestRequests
         * Starts processing of test request
         */
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

        /**
         * GetTestRequestResult
         * Displays result of a particular test request
         */
        public void GetTestRequestResult(string TestRequest)
        {
            bool bRet;
            FileMgr Database = new FileMgr(RepositoryPath);

            XmlParser Parser = new XmlParser();
            bRet = Parser.ParseXmlFile(TestRequest);
            if (false == bRet)
            {
                Console.WriteLine("Error: Parser.ParseTestRequest({0})...FAILED", TestRequest);
                return;
            }

            /**
             * We have the following cases 
             *  Test is not yet executed 
             *  Test is still executing
             *  Test has finished execution
             */

            /** 
             * CASE 1 : Test is not yet executed
             * We may want to check in queue if the test is not yet executed
             * 
             * Acquire Lock
             * Check if TestRequest is in Queue
             * If yes, release lock and Return
             */

            /**
             * CASE 2: Test has finished execution
             * In this case we have the results on log storage. 
             * Try accessing storage..
             */
            Console.WriteLine("REQUIREMENT 6 & 8 : CLIENT QUERY");
            bRet = Database.GetDriverTestResult(Parser._xmlTestInfoList[0]._Author, Parser._xmlTestInfoList[0]._TestDriver);
            if(true == bRet)
            {
                return;
            }

            /** 
             * CASE 3: Test is still executing
             * Return proper error saying execution is in progress
             */
        }

        /**
         * GetAuthorTestResult
         * Gets all test results of a particular author
         */
        public void GetAuthorTestResult(string Author)
        {
            FileMgr Database = new FileMgr(RepositoryPath);

            Console.WriteLine("REQUIREMENT 8 : ENTIRE LOG FILE (Authors Test)");
            Database.DisplayAuthorTestDetails(Author);
        }

        /**
         * GetTestsSummary
         * Displays a summary of all tests executed till date
         */
        public void GetTestsSummary()
        {
            FileMgr Database = new FileMgr(RepositoryPath);

            Console.WriteLine("REQUIREMENT 8 : SIMPLE SUMMARY INFORMATION");
            Database.DisplayTestSummary();
        }

        static void Main(string[] args)
        {
            string path = @"..\..\..\Repository";
            string XmlPath = @"..\..\..\TestRequest\SampleCodeTestRequest.xml";

            TestExec testexe = new TestExec(path);

            testexe.EnqueueTestRequest(XmlPath);
            testexe.ProcessTestRequests();
            testexe.GetTestRequestResult(XmlPath);
            testexe.GetAuthorTestResult("Sahil");
            testexe.GetTestsSummary();
        }
    }
}