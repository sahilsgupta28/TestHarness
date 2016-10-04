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
        string filepath;

        public FileMgr(string path)
        {
            filepath = path;
        }

        string GetTestStartTag(string DriverName)
        {
            return DriverName + ".StartLog";
        }

        string GetTestEndTag(string DriverName)
        {
            return DriverName + ".EndLog";
        }

        public void WriteLog(string LogMsg)
        {
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (StreamWriter w = new StreamWriter(filepath, true))
            {
                w.WriteLine("{0} {1}\t{2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString(), LogMsg);
            }
        }

        public void WriteLog(string DriverName, string TestLog, string TestStatus)
        {
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (StreamWriter w = new StreamWriter(filepath, true))
            {
                w.WriteLine("{0}", GetTestStartTag(DriverName));
                w.WriteLine("DriverName : {0}", DriverName);
                w.WriteLine("Test Status : {0}", TestStatus);
                w.WriteLine("TimeStamp  : {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());
                w.WriteLine("\n==================T.E.S.T  L.O.G.======================");
                w.WriteLine("{0}", TestLog);
                w.WriteLine("=========================================================");
                w.WriteLine("{0}", GetTestEndTag(DriverName));
            }
        }

        public void DisplayLog()
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                while (sr.Peek() >= 0)
                {
                    Console.WriteLine(sr.ReadLine());
                }
            }
        }

        /**
         * Queries the database to find logs for a specific test driver 
         */
        public string GetDriverTestResult(string DriverName)
        {
            string dbTestStartTag, dbTestEndTag, dbRow;
            StringBuilder TestResult = new StringBuilder();

            dbTestStartTag = GetTestStartTag(DriverName);
            dbTestEndTag = GetTestEndTag(DriverName);

            /**
             * A basic algorithm which reads log file line by line
             * and searches for driver name.
             * Once found, it reads the contents between start and end tag
             * and appends them to a single string.
             */
            using (StreamReader sr = new StreamReader(filepath))
            {
                while (sr.Peek() > -1)
                {
                    dbRow = sr.ReadLine();
                    if (dbRow == dbTestStartTag)
                    {
                        // Skip start tag
                        if(sr.Peek() > -1)
                            dbRow = sr.ReadLine();

                        while (sr.Peek() > -1 && dbRow != dbTestEndTag)
                        {
                            TestResult.AppendLine(dbRow);
                            dbRow = sr.ReadLine();
                        } 
                        break;
                    }
                }
            }

            return TestResult.ToString();
        }

        public void DisplayTestSummary()
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                /* Display below formatting if there is something in file to write */
                if (sr.Peek() > -1)
                {
                    Console.WriteLine("---------------------------------------------------------");
                    Console.WriteLine("Time Stamp\t\tDriver Name\t\tTest Status");
                }

                while (sr.Peek() > -1)
                {
                    string dbRow = sr.ReadLine();
                    if (dbRow.StartsWith("DriverName"))
                    {
                        string DriverName = dbRow.Substring("DriverName : ".Length);
                        dbRow = sr.ReadLine();
                        string TestStatus = dbRow.Substring("Test Status : ".Length);
                        dbRow = sr.ReadLine();
                        string TimeStamp = dbRow.Substring("TimeStamp  : ".Length);
                        Console.WriteLine("{0}\t{1}\t\t{2}", TimeStamp, DriverName, TestStatus);
                    }
                }
                Console.WriteLine("---------------------------------------------------------");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                string path = @"c:\Log.txt";
                FileMgr Logger = new FileMgr(path);

                /* Write Simple Log*/
                Logger.WriteLog("Test Log Msg");

                /* Write Test Log */
                Logger.WriteLog("DummyDriver6", "Dummy String6", "PASS");

                /* Get details of individual Test */
                string result = Logger.GetDriverTestResult("DummyDriver5");
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
