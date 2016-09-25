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

namespace TestHarness
{
    class AppDomainMgr
    {
        public string RespoitoryPath { get; set; }

        public void ProcessTestRequests(string[] sTestRequests)
        {
            /* @todo Dequeue Test Request
             */

            foreach (string sTestRequest in sTestRequests)
            {
                ProcessTestRequest(sTestRequest);
            }
        }

        bool ProcessTestRequest(string sTestRequest)
        {
            bool bRet;

            try
            {
                Console.WriteLine("\nParsing({0})", sTestRequest);

                /* Parse Test Request to extract data */
                XmlParser Parser = new XmlParser();
                bRet = Parser.ParseTestRequest(sTestRequest);
                if (false == bRet)
                {
                    Console.WriteLine("Parser.ParseTestRequest({0})...FAILED", sTestRequest);
                    return false;
                }

                Parser.DisplayTestRequest();

                /* Loader */

            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return true;
        }
    }
}