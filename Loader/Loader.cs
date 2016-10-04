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

namespace AssemblyLoader
{
    public class Loader
    {
        /**********************************************************************
                                M E M B E R S
         **********************************************************************/

        string _RepositoryPath;

        /**********************************************************************
                                 P U B L I C   M E T H O D S
         **********************************************************************/

        public Loader(string path)
        {
            _RepositoryPath = path;
        }

        public string LoadAssemblyAndGetItestClass(string AssemblyName)
        {
            bool bRet;
            string AssemblyAbsPath;

            Console.WriteLine("\nAttempting to Load assembly ({0})", AssemblyName);

            bRet = GetFilePath(_RepositoryPath, AssemblyName, out AssemblyAbsPath);
            if (false == bRet)
            {
                Console.WriteLine("GetFilePath({0})...FAILED.", AssemblyName);
                return null;
            }

            Console.WriteLine("Found assembly: {0}", AssemblyAbsPath);

            Assembly assemblyInstance = Assembly.LoadFrom(AssemblyAbsPath);
            if (assemblyInstance == null)
            {
                Console.WriteLine("Could not load assembly ({0})", AssemblyAbsPath);
                return null;
            }

            Type[] types = assemblyInstance.GetExportedTypes();

            foreach (Type t in types)
            {
                /* Does this type derive from ITest ? */
                if (t.IsClass && typeof(ITest).IsAssignableFrom(t))
                {
                    return t.FullName;
                }
            }
            return null;
        }

        public List<string> LoadAssemblyAndGetItestClass(List<string> Assemblies)
        {
            List<string> lsAssemblyClassName = null;

            Console.WriteLine("\n>>>>Loading Assemblies (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

            try
            {
                lsAssemblyClassName = new List<string>();
                foreach (var AssemblyName in Assemblies)
                {
                    string AssemblyClassName = LoadAssemblyAndGetItestClass(AssemblyName);
                    if (null == AssemblyName)
                    {
                        Console.WriteLine("Error: LoadAndGetAssemblyITestClass({0})...FAILED.", AssemblyName);
                        continue;
                    }

                    lsAssemblyClassName.Add(AssemblyClassName);
                }
            }
            catch (Exception Ex)
            {
                Console.Write("Exception : {0}", Ex.Message);
                return null;
            }

            return lsAssemblyClassName;
        }

        public static bool GetFilePath(string repo, string filename, out string filepath)
        {
            DirectoryInfo folder = new DirectoryInfo(repo);
            FileInfo[] file = folder.GetFiles(filename, SearchOption.AllDirectories);

            if (file.Length == 0)
            {
                Console.WriteLine("File ({0}) not found.", filename);
                filepath = null;
                return false;
            }

            //Console.WriteLine("File ({0}) found.", file[0].FullName);
            filepath = file[0].FullName;
            return true;
        }

        public static void DisplayAssemblies(AppDomain Domain)
        {
            Console.WriteLine("\nListing Assemblies in Domain ({0})", Domain.FriendlyName);
            Assembly[] loadedAssemblies = Domain.GetAssemblies();

            foreach (Assembly a in loadedAssemblies)
                Console.WriteLine("Assembly -> Name: ({0}) Version: ({1})", a.GetName().Name, a.GetName().Version);
        }

        static void Main(string[] args)
        {
            string path = @"E:\Sahil\Syracuse\Study\CSE 681 - SMA\Project\Project 2\TestHarness\Repository";
            Loader DemoLoader = new Loader(path);
            List<string> AssemblyName = new List<string>();

            AssemblyName.Add("TestCode_TestDriver.dll");
            AssemblyName.Add("TestCode_TestDriver.dll");

            List<string> ClassName = DemoLoader.LoadAssemblyAndGetItestClass(AssemblyName);

            Console.WriteLine("\nListing ClassName");
            foreach (var Name  in ClassName)
            {
                Console.WriteLine("Class({0})", Name);
            }

            Loader.DisplayAssemblies(AppDomain.CurrentDomain);
        }
    }
}

