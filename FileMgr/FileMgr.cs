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

        string _Repository;

        /**********************************************************************
                                P U B L I C   M E T H O D S
         **********************************************************************/

        public FileMgr(string path)
        {
            _Repository = path;
            System.IO.Directory.CreateDirectory(GetLogRepo());
        }

        /**
         * GetLogRepo
         * Get directory path for storing logs
         */
        private string GetLogRepo()
        {
            return _Repository + "\\TEST LOGS";
        }

        /**
         * GetFileName
         * Generate file name for a particular author on a given day
         */
        private string GetFileName(string Author)
        {
            return (GetLogRepo() + "\\" + Author + "_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt");
        }

        /**
         * GetFileAuthor
         * Get Author of given file
         * The file name has relative path and date stamp, extract author from filename
         */
        public string GetFileAuthor(string FileName)
        {
            return FileName.Substring(FileName.LastIndexOf('\\') + 1, FileName.IndexOf('_') - FileName.LastIndexOf('\\') - 1);
        }

        /**
         * WriteLog
         * Write Test Request info to log
         */
        public string WriteLog(string Author, string DriverDLL, string DriverName, string TestLog, string TestStatus)
        {
            string filename = GetFileName(Author);
            try
            {
                // The using statement automatically flushes AND CLOSES the stream and calls 
                // IDisposable.Dispose on the stream object.
                using (StreamWriter w = new StreamWriter(filename, true))
                {
                    w.WriteLine("Author : {0}", Author);
                    w.WriteLine("DriverDLL : {0}", DriverDLL);
                    w.WriteLine("DriverName : {0}", DriverName);
                    w.WriteLine("Test Status : {0}", TestStatus);
                    w.WriteLine("TimeStamp  : {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());
                    w.WriteLine("\n==T.E.S.T  L.O.G.==");
                    w.WriteLine("{0}", TestLog);
                    w.WriteLine("===================");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return filename;
        }

        /**
         * GetDriverTestResult
         * Queries the log file to find logs for a specific test case
         * If file is not found, returns false other wise true
         */
        public bool GetDriverTestResult(string Author, string DriverName)
        {
            bool bFileFound = false;
            string dbRow;

            try
            {
                /* Get all files in log directory */
                foreach (string sFile in Directory.EnumerateFiles(GetLogRepo(), "*.txt"))
                {
                    /* Check if file is of given Author */
                    if (Author != GetFileAuthor(sFile))
                        continue;

                    using (StreamReader sr = new StreamReader(sFile))
                    {
                        while (sr.Peek() > -1)
                        {
                            dbRow = sr.ReadLine();

                            //Search for Driver DLL in this file.
                            //If it is, this the file containing out test request
                            if (dbRow.StartsWith("DriverDLL") && DriverName == dbRow.Substring("DriverDLL : ".Length))
                            {
                                DisplayFile(sFile);
                            }
                        }
                        bFileFound = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return bFileFound;
        }
        
        /**
         * DisplayTestSummary
         * Prints a summary of all tests executed on console.
         * Summary includes timestamp, author name, driver name, pass or failed status
         */
        public void DisplayTestSummary()
        {
            Console.WriteLine("-------------------------- T E S T   S U M M A R Y ----------------------------");
            Console.WriteLine("{0,-25}{1,-20}{2,-10}{3,-20}", "Time Stamp", "Author", "Status", "Driver Name");

            /* Get all files in log directory */
            foreach (string sFile in Directory.EnumerateFiles(GetLogRepo(), "*.txt"))
            {
                using (StreamReader sr = new StreamReader(sFile))
                {
                    while (sr.Peek() > -1)
                    {
                        string dbRow = sr.ReadLine();
                        if (dbRow.StartsWith("Author"))
                        {
                            string Author = dbRow.Substring("Author : ".Length);

                            dbRow = sr.ReadLine(); //Skip DriverDLL

                            dbRow = sr.ReadLine();
                            string DriverName = dbRow.Substring("DriverName : ".Length);

                            dbRow = sr.ReadLine();
                            string TestStatus = dbRow.Substring("Test Status : ".Length);

                            dbRow = sr.ReadLine();
                            string TimeStamp = dbRow.Substring("TimeStamp  : ".Length);
                            Console.WriteLine("{0,-25}{1,-20}{2,-10}{3,-20}", TimeStamp, Author, TestStatus, DriverName);
                        }
                    }
                 
                }
            }
            Console.WriteLine("-------------------------------------------------------------------------------");
        }

        /**
         * DisplayAuthorTestDetails
         * Display details of all tests an Author has performed till date
         */
        public void DisplayAuthorTestDetails(string Author)
        {
            /* Get all files in log directory */
            foreach (string sFile in Directory.EnumerateFiles(GetLogRepo(), "*.txt"))
            {
                /* Check if file is of given Author */
                if (Author != GetFileAuthor(sFile))
                    continue;

                DisplayFile(sFile);
            }
        }

        /**
         * DisplayFile
         * Display contents of a log file
         */
        public void DisplayFile(string FileName)
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                while (sr.Peek() >= 0)
                {
                    Console.WriteLine(sr.ReadLine());
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                string path = @"..\..\..\Repository";
                FileMgr Logger = new FileMgr(path);

                string FileName = Logger.WriteLog("Sahil", "Dummy.DLL", "DummyDriver", "Dummy String", "PASS");
                Console.WriteLine("FileName : ({0})", FileName);

                Console.WriteLine("\nTest Results of Sahil's Dummy.DLL");
                Logger.GetDriverTestResult("Sahil", "Dummy.DLL");

                Console.WriteLine("\nTest Results of Sahil's Tests");
                Logger.DisplayAuthorTestDetails("Sahil");

                Logger.DisplayTestSummary();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }
    }
}
