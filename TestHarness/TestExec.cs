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

            Console.WriteLine("\nCurrent AppDomain : {0}", AppDomain.CurrentDomain.FriendlyName);

            try
            {
                do
                {
                    /* De-queue Test Request */
                    string TestRequest = DequeueTestRequest();

                    /* Create Application Domain 
                     * The name of application domain is of format AppDomain followed by current timestamp with milliseconds
                     * AppDomain - YYMMDD - HHMMSS - FFF
                     */
                    AppDomain ChildDomain = AppDomain.CreateDomain("AppDomain-" + DateTime.Now.ToString("yyMMdd-HHmmss-fff"));
                    Console.WriteLine("Created new application domain ({0})", ChildDomain.FriendlyName);

                    /* Instantiate AppDomainMgr to process individual test request */
                    Type tAppDMgr = typeof(AppDomainMgr);
                    AppDomainMgr AppDMgr = (AppDomainMgr)ChildDomain.CreateInstanceAndUnwrap(
                        Assembly.GetAssembly(tAppDMgr).FullName,
                        tAppDMgr.ToString(),
                        false,
                        BindingFlags.Default,
                        null,
                        new object[] { RepositoryPath },
                        null,
                        null
                        );
                    if (null == AppDMgr)
                    {
                        Console.WriteLine("ChildDomain.CreateInstanceAndUnwrap(AppDomainMgr) for ({0})...FAILED.", TestRequest);
                        continue;
                    }

                    /* Pass test request to app domain */
                    bRet = AppDMgr.ProcessTestRequest(TestRequest);
                    if (false == bRet)
                    {
                        Console.WriteLine("{0}({1})...FAILED.", AppDMgr.GetType().FullName, TestRequest);
                        continue;
                    }

                    AppDMgr.DisplayAssemblies(ChildDomain);

                    Console.WriteLine("Unloading Child AppDomain ({0})", ChildDomain.FriendlyName);
                    AppDomain.Unload(ChildDomain);


                } while (TestQueue.Count != 0);

                DisplayAssemblies(AppDomain.CurrentDomain);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }

        public void DisplayAssemblies(AppDomain Domain)
        {
            Console.WriteLine("\nListing Assemblies in Domain ({0})", Domain.FriendlyName);
            Assembly[] loadedAssemblies = Domain.GetAssemblies();

            foreach (Assembly a in loadedAssemblies)
                Console.WriteLine("Assembly -> Name: ({0}) Version: ({1})", a.GetName().Name, a.GetName().Version);
        }
    }
}