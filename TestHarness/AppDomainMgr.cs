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
using System.Runtime.Remoting;
using System.Collections.Generic;

namespace TestHarness
{
    using TestInterface;
    using XMLParser;
    using AssemblyLoader;
    using FileManager;

    class TestInfo
    {
        public string Version;
        public string Author;
        public DateTime TimeStamp;
        public string TestName;
        public string TestDriverDLL;
        public string TestDriverClass;
        public List<string> TestCodeDLL;

        public TestInfo()
        {
            Version = "1";
        }

        public void Display()
        {
            Console.WriteLine("  {0,-12} : {1}", "Version", Version);
            Console.WriteLine("  {0,-12} : {1}", "Author", Author);
            Console.WriteLine("  {0,-12} : {1}", "TimeStamp", TimeStamp);
            Console.WriteLine("  {0,-12} : {1}", "TestName", TestName);
            Console.WriteLine("  {0,-12} : {1}", "TestDriverDLL", TestDriverDLL);
            Console.WriteLine("  {0,-12} : {1}", "TestDriverClass", TestDriverClass);

            foreach (string DLL in TestCodeDLL)
            {
                Console.WriteLine("  {0,-12} : {1}", "TestCodeDLL", DLL);
            }
            Console.WriteLine("");
        }
    }

    class AppDomainMgr : MarshalByRefObject
    {
        /**********************************************************************
                                 M E M B E R S
         **********************************************************************/
        FileMgr Database;

        public List<TestInfo> TestInfoList = null;

        public string RepositoryPath { get; set; }

        /**********************************************************************
                                 P U B L I C   M E T H O D S
         **********************************************************************/

        public AppDomainMgr(string path)
        {
            RepositoryPath = path;
            Database = new FileMgr(path + "\\Log.txt");
        }

        public bool ProcessTestRequest(string sTestRequest)
        {
            try
            {
                bool bRet;

                //Console.WriteLine("\n>>>>Processing Test Request (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

                /* Parse Test Request to extract data */
                XmlParser Parser = new XmlParser();
                bRet = Parser.ParseTestRequest(sTestRequest);
                if (false == bRet)
                {
                    Console.WriteLine("Parser.ParseTestRequest({0})...FAILED", sTestRequest);
                    return false;
                }

                Parser.DisplayTestRequest();

                /* Convert */
                TestInfoList = GetTestInfoFromXmlTestInfo(Parser.xmlTestInfoList);

                /* Invoke Loader */
                Loader load = new Loader(RepositoryPath);
                foreach (var TestInfo in TestInfoList)
                {
                    TestInfo.TestDriverClass = load.LoadAssemblyAndGetItestClass(TestInfo.TestDriverDLL);
                    if (null == TestInfo.TestDriverClass)
                    {
                        Console.WriteLine("load.LoadAssemblyAndGetItestClass({0})...FAILED", TestInfo.TestDriverDLL);
                        //return false;
                    }

                }

                DisplayTestInfoList();

                /* Execute Test */
                ExecuteTest(); 
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
            return true;
        }

        private List<TestInfo> GetTestInfoFromXmlTestInfo(List<xmlTestInfo> xmlTestInfoList)
        {
            if(0 == xmlTestInfoList.Count)
            {
                return null;
            }

            List<TestInfo> TestInfo = new List<TestInfo>();

            foreach (xmlTestInfo xmlInfo in xmlTestInfoList)
            {
                TestInfo newTestInfo = new TestInfo();
                newTestInfo.Author = xmlInfo.Author;
                newTestInfo.TimeStamp = xmlInfo.TimeStamp;
                newTestInfo.TestName = xmlInfo.TestName;
                newTestInfo.TestDriverDLL = xmlInfo.TestDriver;
                //TestDriverClass;

                newTestInfo.TestCodeDLL = new List<string>();
                foreach (var DLL in xmlInfo.TestCode)
                {
                    newTestInfo.TestCodeDLL.Add(DLL);
                }

                TestInfo.Add(newTestInfo);
            }

            return TestInfo;
        }


        public void ExecuteTest()
        {
            try
            {
                if (0 == TestInfoList.Count)
                {
                    return;
                }

                Console.WriteLine("\n>>>>Executing Tests (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

                foreach (TestInfo Info in TestInfoList)
                {
                    bool TestStatus;

                    Console.WriteLine("Testing ({0})", Info.TestName);
                    Console.WriteLine("--------------------------");

                    string TestDriverAssemblyPath;
                    TestStatus = Loader.GetFilePath(RepositoryPath, Info.TestDriverDLL, out TestDriverAssemblyPath);
                    if(false == TestStatus)
                    {
                        Console.WriteLine("Error: Loader.GetFilePath({0})...FAILED.", Info.TestDriverDLL);
                        continue;
                    }

                    ITest TestDriverInstance = GetITestInstance(TestDriverAssemblyPath, Info.TestDriverClass);
                    if(null == TestDriverInstance)
                    {
                        Console.WriteLine("Error:GetITestInstance({0})...FAILED.", Info.TestDriverClass);
                        continue;
                    }

                    TestStatus = TestDriverInstance.test();
                    if (TestStatus == true)
                    {
                        Console.WriteLine("--------------------------");
                        Console.WriteLine("#### Test Status : PASS ####\n");
                    }
                    else
                    {
                        Console.WriteLine("--------------------------");
                        Console.WriteLine("#### Test Status : FAIL ####\n");
                    }

                    Console.WriteLine("{0}", TestDriverInstance.getLog());

                    Database.WriteLog(Info.TestDriverClass, TestDriverInstance.getLog(), TestStatus ? "PASS" : "FAIL");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

        }

        public static ITest GetITestInstance(string AssemblyPath, string ClassName)
        {
            ObjectHandle oh = Activator.CreateInstanceFrom(AssemblyPath, ClassName);
            object ob = oh.Unwrap();

            return (ITest)ob;
        }

        public void DisplayTestInfoList()
        {
            Console.WriteLine("\nAssemblies Loaded:");
            foreach (var Info in TestInfoList)
            {
                Info.Display();
            }
        }
    }
}