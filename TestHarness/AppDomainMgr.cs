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
using System.Reflection;
using System.Collections.Generic;

namespace TestHarness
{
    using TestInterface;

    public struct TestData
    {
        public string DriverName;
        public ITest TestDriver;
    }

    class AppDomainMgr : MarshalByRefObject
    {
        /**********************************************************************
                                 M E M B E R S
         **********************************************************************/
        FileMgr Database;

        public List<TestData> TestDrivers;

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

                /* Load Assembly */
                Loader load = new Loader();
                TestDrivers = load.LoadAssemblies(RepositoryPath, Parser.TestCase);
                if (null == TestDrivers)
                {
                    Console.WriteLine("load.LoadAssemblies({0})...FAILED", sTestRequest);
                    return false;
                }

                DisplayTestDrivers();

                /* Execute Test */
                ExecuteTest(); 
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
            return true;
        }

        public void ExecuteTest()
        {
            try
            {
                if (TestDrivers.Count == 0)
                {
                    return;
                }

                Console.WriteLine("\n>>>>Executing Tests (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

                foreach (TestData td in TestDrivers)
                {
                    bool TestStatus;

                    Console.WriteLine("Testing ({0})", td.DriverName);
                    Console.WriteLine("--------------------------");
                    TestStatus = td.TestDriver.test();
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

                    Console.WriteLine("{0}", td.TestDriver.getLog());

                    Database.WriteLog(td.DriverName, td.TestDriver.getLog(), TestStatus?"PASS":"FAIL");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
           
        }

        public void DisplayTestDrivers()
        {
            Console.WriteLine("\nAssemblies Loaded:");
            foreach (var TestData in TestDrivers)
            {
                Console.WriteLine("iTest Interface Name : {0}", TestData.DriverName);
            }
        }

        public void DisplayAssemblies(AppDomain Domain)
        {
            Console.WriteLine("\nListing Assemblies in Domain ({0})", Domain.FriendlyName);
            Assembly[] loadedAssemblies = Domain.GetAssemblies();

            foreach (Assembly a in loadedAssemblies)
                Console.WriteLine("Assembly -> Name: ({0}) Version: ({1})", a.GetName().Name, a.GetName().Version);
        }

    }
}