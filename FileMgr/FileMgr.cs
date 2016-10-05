/**
 * File Manager
 * Implements read and write function to file
 * 
 * FileName     : FileMgr.cs
 * Author       : Sahil Gupta
 * Date         : 25 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Text;

namespace FileManager
{
    public class FileMgr
    {
        /**********************************************************************
                                M E M B E R S
        **********************************************************************/

        string _FilePath;

        /**********************************************************************
                                P U B L I C   M E T H O D S
         **********************************************************************/

        public FileMgr(string path)
        {
            _FilePath = path;
        }

        /**
         * GetTestStartTag
         * Return Identifier in log for start of new test request
         */
        private string GetTestStartTag(string DriverName)
        {
            return DriverName + ".StartLog";
        }

        /**
         * GetTestEndTag
         * Return Identifier in log for end of log test request
         */
        private string GetTestEndTag(string DriverName)
        {
            return DriverName + ".EndLog";
        }

        /**
         * WriteLog
         * Write simple message to log
         */
        public void WriteLog(string LogMsg)
        {
            try
            {
                // The using statement automatically flushes AND CLOSES the stream and calls 
                // IDisposable.Dispose on the stream object.
                using (StreamWriter w = new StreamWriter(_FilePath, true))
                {
                    w.WriteLine("{0} {1}\t{2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString(), LogMsg);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

        }

        /**
         * WriteLog
         * Write Test Request info to log
         */
        public void WriteLog(string Author, string DriverName, string TestLog, string TestStatus)
        {
            try
            {
                // The using statement automatically flushes AND CLOSES the stream and calls 
                // IDisposable.Dispose on the stream object.
                using (StreamWriter w = new StreamWriter(_FilePath, true))
                {
                    w.WriteLine("\n{0}", GetTestStartTag(DriverName));
                    w.WriteLine("Author : {0}", Author);
                    w.WriteLine("DriverName : {0}", DriverName);
                    w.WriteLine("Test Status : {0}", TestStatus);
                    w.WriteLine("TimeStamp  : {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());
                    w.WriteLine("\n==T.E.S.T  L.O.G.==");
                    w.WriteLine("{0}", TestLog);
                    w.WriteLine("===================");
                    w.WriteLine("{0}", GetTestEndTag(DriverName));
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }

        /**
         * GetDriverTestResult
         * Queries the log file to find logs for a specific test case
         */
        public string GetDriverTestResult(string DriverName)
        {
            string dbTestStartTag, dbTestEndTag, dbRow;
            StringBuilder TestResult = new StringBuilder();

            try
            {
                dbTestStartTag = GetTestStartTag(DriverName);
                dbTestEndTag = GetTestEndTag(DriverName);

                /**
                 * A basic algorithm which reads log file line by line and searches for driver name.
                 * Once found, it reads the contents between start and end tag and appends them to a single string.
                 */
                using (StreamReader sr = new StreamReader(_FilePath))
                {
                    while (sr.Peek() > -1)
                    {
                        dbRow = sr.ReadLine();

                        //Search for start tag
                        if (dbRow == dbTestStartTag)
                        {
                            // Skip start tag
                            if (sr.Peek() > -1)
                                dbRow = sr.ReadLine();

                            // While Not end tag add contents to a string
                            while (sr.Peek() > -1 && dbRow != dbTestEndTag)
                            {
                                TestResult.AppendLine(dbRow);
                                dbRow = sr.ReadLine();
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return TestResult.ToString();
        }

        /**
         * DisplayTestSummary
         * Prints a summary of all tests executed on console.
         * Summary includes timestamp, author name, driver name, pass or failed status
         */
        public void DisplayTestSummary()
        {
            using (StreamReader sr = new StreamReader(_FilePath))
            {
                /* Display below formatting if there is something in file to write */

                if (sr.Peek() > -1)
                {

                    Console.WriteLine("-------------------------- T E S T   S U M M A R Y ----------------------------");
                    Console.WriteLine("{0,-25}{1,-20}{2,-10}{3,-20}","Time Stamp","Author","Status", "Driver Name");
                }
                else
                {
                    Console.WriteLine("Log File Empty");
                }

                while (sr.Peek() > -1)
                {
                    string dbRow = sr.ReadLine();
                    if (dbRow.StartsWith("Author"))
                    {
                        string Author = dbRow.Substring("Author : ".Length);
                        dbRow = sr.ReadLine();
                        string DriverName = dbRow.Substring("DriverName : ".Length);
                        dbRow = sr.ReadLine();
                        string TestStatus = dbRow.Substring("Test Status : ".Length);
                        dbRow = sr.ReadLine();
                        string TimeStamp = dbRow.Substring("TimeStamp  : ".Length);
                        Console.WriteLine("{0,-25}{1,-20}{2,-10}{3,-20}", TimeStamp, Author, TestStatus, DriverName);
                    }
                }
                Console.WriteLine("-------------------------------------------------------------------------------");
            }
        }

        /**
         * DisplayLog
         * Display contents of entire log file on console
         */
        public void DisplayLog()
        {
            Console.WriteLine("\n#### LOG FILE START ###");
            using (StreamReader sr = new StreamReader(_FilePath))
            {
                while (sr.Peek() >= 0)
                {
                    Console.WriteLine(sr.ReadLine());
                }
            }
            Console.WriteLine("#### LOG FILE END ###");
        }

        static void Main(string[] args)
        {
            try
            {
                string path = @"..\..\..\Repository\Log.txt";
                FileMgr Logger = new FileMgr(path);

                /* Write Simple Log*/
                Logger.WriteLog("Test Log Msg");

                /* Write Test Log */
                Logger.WriteLog("Author", "DummyDriver", "Dummy String", "PASS");

                /* Get details of individual Test */
                string result = Logger.GetDriverTestResult("DummyDriver");
                if (result == "")
                    Console.WriteLine("Empty");
                else
                    Console.WriteLine("{0}", result);

                /* Display Summary of Tests */
                Logger.DisplayTestSummary();

                /* Display Entire Log File */
                Logger.DisplayLog();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }
    }
}
