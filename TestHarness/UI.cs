/**
 * UI
 * Implements command line interface to accept test requests
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
    class UI
    {
        static void Main(string[] args)
        {
            try
            {
                TestExec testexe;

                /*  Validate input */
                if (args.Length < 2)
                {
                    Console.WriteLine("Enter Repository Path and Test Request Path");
                    return;
                }

                /* Check if Repository is valid */
                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine("Invalid Path {0}", args[0]);
                    return;
                }
                Console.WriteLine("Repository Path : {0}\n", args[0]);

                /* Instantiate new Test Executive to process Test Requests 
                 * 
                 * @todo
                 * Ideally, we would want to create a thread and start processing test requests here
                 * The thread waits on queue and processes the test request as soon at it is enqueued.
                 */
                testexe = new TestExec(args[0]);

                /* Enqueue all test requests */
                int ReqCnt = 1;
                do
                {
                    testexe.EnqueueTestRequest(args[ReqCnt]);
                } while (++ReqCnt != args.Length);

                /* Process all queued test requests */
                testexe.ProcessTestRequests();
            }
            catch(Exception Ex)
            {
                Console.WriteLine("Exception: {0}", Ex.Message);
            }
        }
    }
}