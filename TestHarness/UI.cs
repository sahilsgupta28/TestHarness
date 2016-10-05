/**
 * UI
 * Implements command line interface to accept test requests
 * Entry point for Test Harness
 * 
 * Usage
 *      TestHarness.exe /r repository_path /t testrequest1_path [(...)]
 *      TestHarness.exe /r repository_path /q testrequest1_path
 *      TestHarness.exe /r repository_path /a AuthorName
 *      TestHarness.exe /r repository_path /s 
 *      
 *      /r Repository   - Path to Repository containing assemblies to be tested
 *      /t Test Request - XML file specifying test code
 *      /q Query        - Get test result of specific Test Request
 *      /a Author       - Get test results of a particular author
 *      /s Summary      - Get Summary of all tests executed
 *
 *      
 * FileName     : TestExec.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.Diagnostics;
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
                if (args.Length < 3)
                {
                    Console.WriteLine("Error: The syntax of the command is incorrect.");
                    return;
                }

                if (@"/r" != args[0])
                {
                    Console.WriteLine("Error: Must have Repository Path");
                    return;
                }

                /* Check if Repository is valid */
                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine("Error: Invalid Path {0}", args[1]);
                    return;
                }

                /* Instantiate new Test Executive to process Test Requests */
                testexe = new TestExec(args[1]);

                /** @todo 
                 *  Ideally, we would want to create a thread to processing test requests here
                 *  This thread waits on a blocking queue and processes a test request as soon as it is enqueued.
                 */

                /************************************************************************************************
                 *                                          USE CASES
                 ************************************************************************************************/

                /* Client provides Test Request to process */
                if (@"/t" == args[2])
                {
                    int ReqCnt = 3; // Position of 1st Test Request in Command Line Argument string "args"                

                    do
                    {
                        Console.WriteLine("REQUIREMENT 2 : Accepted Test Requests: ({0})", args[ReqCnt]);

                        /* Enqueue test request */
                        testexe.EnqueueTestRequest(args[ReqCnt]);

                    } while (++ReqCnt != args.Length);

                    /* Process all queued test requests */
                    testexe.ProcessTestRequests();
                }

                /* Client wants test result of a particular test request */
                if (@"/q" == args[2])
                {
                    testexe.GetTestRequestResult(args[3]);
                }

                /* Client wants test results of his tests */
                if (@"/a" == args[2])
                {
                    testexe.GetAuthorTestResult(args[3]);
                }

                /* Client wants summary of all test requests */
                if (@"/s" == args[2])
                {
                    testexe.GetTestsSummary();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception: {0}", Ex.Message);
            }
        }
    }
}