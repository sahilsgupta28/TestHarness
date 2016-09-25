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
        public struct TestData
        {
            public string Name;
            public ITest TestDriver;
        }

        public List<TestData> TestDrivers = new List<TestData>();

        public bool LoadAssemblies(string RepositoryPath, List<TestCaseData> TestCase)
        {
            try
            {
                string[] files = Directory.GetFiles(RepositoryPath, "*.dll");

                /**
                 * @todo Search file in Repository and only load those files which are in TestCase
                 */
                foreach (string file in files)
                {
                    Console.WriteLine("Found: {0}", file);

                    Assembly assem = Assembly.LoadFrom(file);
                    Type[] types = assem.GetExportedTypes();

                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                        {
                            Console.WriteLine("Loading: {0}", t.Name);
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
            return false;//(TestDrivers.Count > 0);   // if we have items in list then Load succeeded
        }

        public void Display()
        {
            foreach (var TestData in TestDrivers)
            {
                Console.WriteLine("{0}", TestData.Name);
            }
        }
    }
}