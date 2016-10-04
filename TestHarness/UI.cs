/**
 * UI
 * Implements command line interface to accept test requests
 * Entry point for Test Harness
 * 
 * Usage
 *      TestHarness.exe -repo repository_path -test testrequest1_path [(...)]
 *      TestHarness.exe -repo repository_path -query testrequest1_path
 *      TestHarness.exe -repo repository_path -querySummary
 *      TestHarness.exe -repo repository_path -queryall
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
    class UI
    {
        static void Main(string[] args)
        {
            try
            {
                TestExec testexe;

                /*  Validate input 
                 *  We Expect Repository Path as first input followed by one or more test requests
                 */
                if (args.Length < 2)
                {
                    Console.WriteLine("Enter Repository Path and one or more Test Request Path");
                    return;
                }

                if ("-repo" == args[0])
                {
                    /* Check if Repository is valid */
                    if (!Directory.Exists(args[1]))
                    {
                        Console.WriteLine("Invalid Path {0}", args[1]);
                        return;
                    }
                    Console.WriteLine("Repository Path : {0}\n", args[1]);

                    /* Instantiate new Test Executive to process Test Requests */
                    testexe = new TestExec(args[1]);
                }
                else
                {
                    Console.WriteLine("Must have Repository Path");
                    return;
                }

                /** @todo 
                 *  Ideally, we would want to create a thread to processing test requests here
                 *  This thread waits on a blocking queue and processes a test request as soon as it is enqueued.
                 */

                if ("-test" == args[2])
                {
                    /* Enqueue all test requests */
                    int ReqCnt = 3;
                    do
                    {
                        /* @todo 
                         * We would want to continue accepting new test requests here
                         */

                        /* Enqueue test request */
                        testexe.EnqueueTestRequest(args[ReqCnt]);

                    } while (++ReqCnt != args.Length);

                    /* Process all queued test requests */
                    testexe.ProcessTestRequests();
                }

                if ("-query" == args[2])
                {
                    testexe.ProcessQuery(args[3]);
                }

                if ("-querySummary" == args[2])
                {
                    testexe.ProcessQuerySummary();
                }

                if ("-queryall" == args[2])
                {
                    testexe.ProcessQueryAll();
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine("Exception: {0}", Ex.Message);
            }
        }
    }
}