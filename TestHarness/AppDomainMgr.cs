/**
 * Application Domain Manager
 * Creates application domain for to process individual test request
 * 
 * FileName     : AppDomainMgr.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.Collections.Generic;

namespace TestHarness
{
    class AppDomainMgr
    {
        public string RepositoryPath { get; set; }

        public AppDomainMgr(string path)
        {
            RepositoryPath = path;
        }

        public bool ProcessTestRequest(string sTestRequest)
        {
            bool bRet;

            try
            {
                Console.WriteLine("Processing Test Request ({0})", sTestRequest);

                /* @todo Create App Domain */

                /* Parse Test Request to extract data */
                XmlParser Parser = new XmlParser();
                bRet = Parser.ParseTestRequest(sTestRequest);
                if (false == bRet)
                {
                    Console.WriteLine("Parser.ParseTestRequest({0})...FAILED", sTestRequest);
                    return false;
                }

                Parser.DisplayTestRequest();

                /* @todo Load Assembly */
                Loader load = new Loader();
                load.LoadAssemblies(RepositoryPath, Parser.TestCase);

                load.Display();

                /* @todo Execute Test */
                ExecuteTest(load.TestDrivers);

            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return true;
        }

        void ExecuteTest(List<Loader.TestData> TestCase)
        {
            if (TestCase.Count == 0)
            {
                return;
            }

            foreach (Loader.TestData td in TestCase)
            {
                Console.WriteLine("Testing {0}", td.Name);

                if (td.TestDriver.test() == true)
                {
                    Console.WriteLine("Test Passed");
                }
                else
                {
                    Console.WriteLine("Test Failed");
                }
            }
        }
    }
}