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
using System.Security.Policy;

namespace TestHarness
{
    class AppDomainMgr
    {
        public string RepositoryPath { get; set; }

        public AppDomainMgr(string path)
        {
            RepositoryPath = path;
        }

        public bool ProcessTestRequest(string sTestRequest)
        {
            bool bRet;
            AppDomain ChildDomain = null;

            try
            {
                Console.WriteLine("Processing Test Request ({0})", sTestRequest);

                /* Create App Domain */
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = Environment.CurrentDirectory;
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                ChildDomain = AppDomain.CreateDomain("ChildDomain", adevidence, domaininfo);

                Console.WriteLine("\nCurrent Domain : {0}", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine("New Domain : {0}", ChildDomain.FriendlyName);

                /* Parse Test Request to extract data */
                //XmlParser Parser = new XmlParser();
                Type tXmlParser = typeof(XmlParser);
                XmlParser Parser = (XmlParser)ChildDomain.CreateInstanceAndUnwrap(Assembly.GetAssembly(tXmlParser).FullName, tXmlParser.ToString());
                bRet = Parser.ParseTestRequest(sTestRequest);
                if (false == bRet)
                {
                    Console.WriteLine("Parser.ParseTestRequest({0})...FAILED", sTestRequest);
                    return false;
                }

                Parser.DisplayTestRequest();

                /* Load Assembly */
                //Loader load = new Loader();
                Type tLoader= typeof(Loader);
                Loader load= (Loader)ChildDomain.CreateInstanceAndUnwrap(Assembly.GetAssembly(tLoader).FullName, tLoader.ToString()); 
                bRet = load.LoadAssemblies(RepositoryPath, Parser.TestCase);
                if (false == bRet)
                {
                    Console.WriteLine(" load.LoadAssemblies({0})...FAILED", sTestRequest);
                    return false;
                }

                load.Display();

                /* Display Loaded Assemblies */
                DisplayAssemblies(AppDomain.CurrentDomain);
                //DisplayAssemblies(ChildDomain);

                /* Execute Test */
                //ExecuteTest(load.TestDrivers);

            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
            finally
            {
                if (ChildDomain != null)
                {
                    AppDomain.Unload(ChildDomain);
                    Console.WriteLine("Unloaded Child AppDomain");
                }
            }
            return true;
        }

        void ExecuteTest(List<Loader.TestData> TestCase)
        {
            if (TestCase.Count == 0)
            {
                return;
            }

            Console.WriteLine("\nExecuting Tests...");
            Console.WriteLine("\nCurrent Domain : {0}", AppDomain.CurrentDomain.FriendlyName);

            foreach (Loader.TestData td in TestCase)
            {
                Console.WriteLine("Testing {0}", td.Name);

                if (td.TestDriver.test() == true)
                {
                    Console.WriteLine("Test Passed\n");
                }
                else
                {
                    Console.WriteLine("Test Failed\n");
                }
            }
        }

        private void DisplayAssemblies(AppDomain Domain)
        {
            Console.WriteLine("\nListing Assemblies in Domain ({0})", Domain.FriendlyName);
            Assembly[] loadedAssemblies = Domain.GetAssemblies();

            foreach (Assembly a in loadedAssemblies)
                Console.WriteLine("Assembly -> Name: ({0}) Version: ({1})", a.GetName().Name, a.GetName().Version);
        }
    }
}