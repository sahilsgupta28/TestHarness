﻿/**
 * Application Domain Manager
 * Creates application domain for to process individual test request
 * 
 * FileName     : AppDomainMgr.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

/* Define if you want to enable debug logs */
//#define ENABLE_DEBUG_LOGS

using System;
using System.Runtime.Remoting;
using System.Collections.Generic;

namespace TestHarness
{
    using TestInterface;
    using XMLParser;
    using AssemblyLoader;
    using FileManager;
    using System.Reflection;

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

                #if ENABLE_DEBUG_LOGS
                Console.WriteLine("\n>>>>Processing Test Request (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);
                #endif

                /* Parse Test Request to extract data */
                XmlParser Parser = new XmlParser();
                bRet = Parser.ParseXmlFile(sTestRequest);
                if (false == bRet)
                {
                    Console.WriteLine("Parser.ParseTestRequest({0})...FAILED", sTestRequest);
                    return false;
                }

                Console.WriteLine("REQUIREMENT 2 : XML File Specifying developer id, test drivers DLL, code DLL");
                Parser.DisplayTestRequest();

                /* Fill TestInfoList with data from XML */
                TestInfoList = GetTestInfoFromXmlTestInfo(Parser._xmlTestInfoList);

                /* Create Application Domain 
                * The name of application domain is of format AppDomain followed by current timestamp with milliseconds
                * AppDomain - YYMMDD - HHMMSS - FFF
                */
                AppDomain ChildDomain = AppDomain.CreateDomain("AppDomain-" + DateTime.Now.ToString("yyMMdd-HHmmss-fff"));
                Console.WriteLine("Created new application domain ({0})", ChildDomain.FriendlyName);

                /* Instantiate Loader */
                Type tLoader = typeof(Loader);
                Loader load = (Loader)ChildDomain.CreateInstanceAndUnwrap(Assembly.GetAssembly(tLoader).FullName,
                    tLoader.ToString(), false, BindingFlags.Default, null, new object[] { RepositoryPath }, null, null);
                if (null == load)
                {
                    Console.WriteLine("ChildDomain.CreateInstanceAndUnwrap(Loader) for ({0})...FAILED.", sTestRequest);
                    return false;
                }

                /* Load Assemblies */
                foreach (var TestInfo in TestInfoList)
                {
                    TestInfo.TestDriverClass = load.LoadAssembly(TestInfo.TestDriverDLL);
                    if (null == TestInfo.TestDriverClass)
                    {
                        Console.WriteLine("load.LoadAssembly({0})...FAILED", TestInfo.TestDriverDLL);
                        //return false;
                    }

                }

                //DisplayTestInfoList();

                /* Execute Test */
                foreach (TestInfo Info in TestInfoList)
                {
                    bool TestStatus = false;
                    string ResultLog;

                    try
                    {
                        Console.WriteLine("\nTesting ({0})", Info.TestDriverDLL);
                        Console.WriteLine("--------------------------");

                        ITest TestDriverInstance = GetITestInstance(ChildDomain, Info.TestDriverDLL, Info.TestDriverClass);
                        if (null == TestDriverInstance)
                        {
                            Console.WriteLine("Error:GetITestInstance({0})...FAILED.", Info.TestDriverClass);
                            continue;
                        }

                        TestStatus = TestDriverInstance.test();
                        if (TestStatus)
                            Console.WriteLine("<<<Test PASS>>>");
                        else
                            Console.WriteLine("<<<Test FAIL>>>");

                        ResultLog = TestDriverInstance.getLog();
                    }
                    catch (Exception Ex)
                    {
                        ResultLog = "Exception : " + Ex.Message;
                        Console.WriteLine("<<<Test FAIL>>>");
                    }

                    Database.WriteLog(Info.Author, Info.TestDriverClass, ResultLog, TestStatus ? "PASS" : "FAIL");
                }

                Console.WriteLine("\nUnloading Child AppDomain ({0})", ChildDomain.FriendlyName);
                AppDomain.Unload(ChildDomain);
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
                newTestInfo.Author = xmlInfo._Author;
                newTestInfo.TimeStamp = xmlInfo._TimeStamp;
                newTestInfo.TestName = xmlInfo._TestName;
                newTestInfo.TestDriverDLL = xmlInfo._TestDriver;
                //TestDriverClass;

                newTestInfo.TestCodeDLL = new List<string>();
                foreach (var DLL in xmlInfo._TestCode)
                {
                    newTestInfo.TestCodeDLL.Add(DLL);
                }

                TestInfo.Add(newTestInfo);
            }

            return TestInfo;
        }

        public ITest GetITestInstance(AppDomain ad, string AssemblyName, string ClassName)
        {
            bool bRet;
            string TestDriverAssemblyPath;

            bRet = Loader.GetFilePath(RepositoryPath, AssemblyName, out TestDriverAssemblyPath);
            if (false == bRet)
            {
                Console.WriteLine("Error: Loader.GetFilePath({0})...FAILED.", AssemblyName);
                return null;
            }

            ObjectHandle oh = ad.CreateInstanceFrom(TestDriverAssemblyPath, ClassName);
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