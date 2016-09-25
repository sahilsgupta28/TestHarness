/**
 * Loader
 * Loads assembly from repository
 * 
 * FileName     : Loader.cs
 * Author       : Sahil Gupta
 * Date         : 25 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using TestInterface;

namespace TestHarness
{
    class Loader : MarshalByRefObject
    {
        public struct TestData
        {
            public string Name;
            public ITest TestDriver;
        }

        public List<TestData> TestDrivers = new List<TestData>();

        /******************************** Member Functions **********************************/

        public bool LoadAssemblies(string RepositoryPath, List<TestCaseData> TestCase)
        {
            try
            {
                Console.WriteLine("\nLoading Assemblies...");
                Console.WriteLine("Current Domain : {0}", AppDomain.CurrentDomain.FriendlyName);

                foreach (var test in TestCase)
                {
                    bool bRet;
                    string filepath;

                    bRet = GetFilePath(RepositoryPath, test.TestDriver, out filepath);
                    if (false == bRet)
                    {
                        Console.WriteLine("GetFilePath({0})...FAILED.", test.TestDriver);
                        continue;
                    }

                    Console.WriteLine("Found assembly: {0}", filepath);

                    Assembly assem = Assembly.LoadFrom(filepath);
                    if (assem == null)
                    {
                        Console.WriteLine("Could not load assembly ({0})", filepath);
                        continue;
                    }

                    Type[] types = assem.GetExportedTypes();

                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                        {
                            Console.WriteLine("Loading Class: {0}", t.FullName);
                            ITest testdriver = (ITest)Activator.CreateInstance(t);    // create instance of test driver

                            //save type name and reference to created type on managed heap
                            TestData testdata = new TestData();
                            testdata.Name = t.Name;
                            testdata.TestDriver = testdriver;
                            TestDrivers.Add(testdata);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.Write("Exception : {0}", Ex.Message);
                return false;
            }

            return (TestDrivers.Count > 0);   // if we have items in list then Load succeeded
        }

        private bool GetFilePath(string repo, string filename, out string filepath)
        {
            DirectoryInfo folder = new DirectoryInfo(repo);
            FileInfo[] file = folder.GetFiles(filename, SearchOption.AllDirectories);

            if (file.Length == 0)
            {
                Console.WriteLine("File ({0}) not found.", filename);
                filepath = null;
                return false;
            }

            Console.WriteLine("File ({0}) found.", file[0].FullName);
            filepath = file[0].FullName;
            return true;
        }

        public void Display()
        {
            Console.WriteLine("\nAssemblies Loaded:");
            foreach (var TestData in TestDrivers)
            {
                Console.WriteLine("{0}", TestData.Name);
            }
        }
    }
}