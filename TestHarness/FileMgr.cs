using System;
using System.IO;

namespace TestHarness
{
    class FileMgr
    {
        string filepath;

        public FileMgr(string path)
        {
            filepath = path;
        }

        public void WriteLog(string LogMsg)
        {
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (StreamWriter w = new StreamWriter(filepath, true))
            {
                w.WriteLine("{0} {1}\t{2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), LogMsg);
            }
        }

        public void WriteLog(string DriverName, string TestLog, string TestStatus)
        {
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (StreamWriter w = new StreamWriter(filepath, true))
            {
                w.WriteLine("{0}.StartLog", DriverName);
                w.WriteLine("DriverName : {0}", DriverName);
                w.WriteLine("Test Status : {0}", TestStatus);
                w.WriteLine("TimeStamp  : {0}\t{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.WriteLine("\n==================T.E.S.T  L.O.G.===================");
                w.WriteLine("{0}", TestLog);
                w.WriteLine("==========================================================");
                w.WriteLine("{0}.EndLog", DriverName);
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

        static void FileMgrMain(string[] args)
        {
            try
            {
                string path = @"c:\Log.txt";
                FileMgr Logger = new FileMgr(path);

                Logger.WriteLog("Test Log Msg");

                Logger.DisplayLog();

            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
        }
    }
}
