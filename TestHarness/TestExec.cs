/**
 * Test Executive
 * Entry point for Test Harness
 * 
 * Usage
 *      TestHarness.exe repository_path testrequest1_path [(...)]
 *      
 * FileName     : TestExec.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;

namespace TestHarness
{
    class TestExec
    {
        static void Main(string[] args)
        {
            /*  Validate input
             *  Checks if file exists 
             */
            if (0 == args.Length)
            {
                Console.WriteLine("Enter Repository path");
                return;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Invalid Path {0}", args[0]);
                return;
            }

            Console.WriteLine("Repository Path : {0}", args[0]);

            /* @todo En-queue Test Request
             */

            /* Create AppDomain Manager
             *  - @todo De-queues Test Request
             *  - Creates AppDomain to process TestRequest
             */
            AppDomainMgr appDomainMgr = new AppDomainMgr();

            appDomainMgr.RespoitoryPath = args[0];

            if (args.Length > 1)
            {
                string[] TestReq = new string[args.Length - 1];

                for (int i = 0; i < args.Length - 1; i++)
                {
                    TestReq[i] = args[i + 1];
                }

                appDomainMgr.ProcessTestRequests(TestReq);
            }


        }
    }
}
