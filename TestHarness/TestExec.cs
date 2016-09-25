/**
 * Test Executive
 * Holds test requests and passes them to AppDomainMgr for processing
 *      
 * FileName     : TestExec.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace TestHarness
{
    class TestExec
    {
        Queue<string> TestQueue;
        public string RespoitoryPath { get; set; }

        public TestExec(string path)
        {
            RespoitoryPath = path;
            TestQueue = new Queue<string>();
        }

        public void EnqueueTestRequest(string TestRequestFile)
        {
            TestQueue.Enqueue(TestRequestFile);
            Console.WriteLine("En-queued ({0})", TestRequestFile);
        }

        public string DequeueTestRequest()
        {
            string TestRequestFile;

            TestRequestFile = TestQueue.Dequeue();
            Console.WriteLine("De-queued ({0})", TestRequestFile);

            return TestRequestFile;
        }

        public void ProcessTestRequests()
        {
            bool bRet;
            AppDomainMgr appDomainMgr = new AppDomainMgr();

            do
            {
                string TestRequest = DequeueTestRequest();

                /* Create AppDomain to process individual test request */
                bRet = appDomainMgr.ProcessTestRequest(TestRequest);
                if (!bRet)
                {
                    Console.WriteLine("appDomainMgr.ProcessTestRequest({0})...FAILED.", TestRequest);
                }

            } while (TestQueue.Count != 0);
        }
    }
}