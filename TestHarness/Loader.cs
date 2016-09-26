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
    class Loader
    {
        /**********************************************************************
                                 P U B L I C   M E T H O D S
         **********************************************************************/
        public List<TestData> LoadAssemblies(string RepositoryPath, List<TestCaseData> TestCase)
        {
            List<TestData> TestDrivers = null;

            try
            {
                TestDrivers = new List<TestData>();

                Console.WriteLine("\nLoading Assemblies...");
                Console.WriteLine("Current Domain : {0}", AppDomain.CurrentDomain.FriendlyName);

                foreach (var test in TestCase)
                {
                    bool bRet;
                    string filepath;

                    Console.WriteLine("\nAttempting to Load assembly ({0})", test.TestDriver);

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
                        /* Does this type derive from ITest ? */
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))
                        {
                            /* Create instance of test driver */
                            Console.WriteLine("Instantiating Interface: ({0})", t);
                            ITest testdriver = (ITest)Activator.CreateInstance(t);

                            /* Save type name and reference to created type on managed heap */
                            TestData testdata = new TestData();
                            testdata.Name = t.FullName;
                            testdata.TestDriver = testdriver;
                            TestDrivers.Add(testdata);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.Write("Exception : {0}", Ex.Message);
                return TestDrivers;
            }

            return TestDrivers;
        }

        private bool GetFilePath(string repo, string filename, out string filepath)
        {
            DirectoryInfo folder = new DirectoryInfo(repo);
            FileInfo[] file = folder.GetFiles(filename, SearchOption.AllDirectories);

            if (file.Length == 0)
            {
                //Console.WriteLine("File ({0}) not found.", filename);
                filepath = null;
                return false;
            }

            //Console.WriteLine("File ({0}) found.", file[0].FullName);
            filepath = file[0].FullName;
            return true;
        }
    }
}