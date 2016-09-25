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

                /* @todo Execute Test */
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return true;
        }
    }
}