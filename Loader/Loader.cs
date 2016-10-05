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

namespace AssemblyLoader
{
    using TestInterface;

    public class Loader : MarshalByRefObject
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

        /**
         * LoadAssembly
         * Loads Assembly and reurns ITest Class name in that assembly 
         */
        public string LoadAssembly(string AssemblyName)
        {
            bool bRet;
            string AssemblyAbsPath;

            try
            {
                //Console.WriteLine("\nAttempting to Load assembly ({0})", AssemblyName);

                /* Get Absolute File Path */
                bRet = GetFilePath(_RepositoryPath, AssemblyName, out AssemblyAbsPath);
                if (false == bRet)
                {
                    Console.WriteLine("Error: GetFilePath({0})...FAILED.", AssemblyName);
                    return null;
                }

                /* Load Assembly */
                Assembly assemblyInstance = Assembly.LoadFrom(AssemblyAbsPath);
                if (null == assemblyInstance)
                {
                    Console.WriteLine("Error: Assembly.LoadFrom({0})...FAILED.", AssemblyAbsPath);
                    return null;
                }

                /* Iterate over all exported types to find the type that derives from ITest Interface */
                Type[] types = assemblyInstance.GetExportedTypes();
                foreach (Type t in types)
                {
                    /* Check if type derives from ITest Interface */
                    if (t.IsClass && typeof(ITest).IsAssignableFrom(t))
                    {
                        return t.FullName;
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return null;
        }

        /**
         * LoadMultipleAssemblies
         * Loads a list of assemblies and returns list of corresponding ITest class name
         */
        public List<string> LoadMultipleAssemblies(List<string> Assemblies)
        {
            List<string> lsAssemblyClassName = null;

            //Console.WriteLine("\n>>>>Loading Assemblies (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

            try
            {
                lsAssemblyClassName = new List<string>();
                foreach (var AssemblyName in Assemblies)
                {
                    string AssemblyClassName = LoadAssembly(AssemblyName);
                    if (null == AssemblyName)
                    {
                        Console.WriteLine("Error: LoadAndGetAssemblyITestClass({0})...FAILED.", AssemblyName);
                        continue; /* Continue to load other assemblies */
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

        /**
         * GetFilePath
         * Returns absolute file path from file name and its containing directory
         */
        public static bool GetFilePath(string directory, string filename, out string filepath)
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(directory);
                FileInfo[] file = folder.GetFiles(filename, SearchOption.AllDirectories);

                if (file.Length == 0)
                {
                    Console.WriteLine("Error: File ({0}) not found.", filename);
                    filepath = null;
                    return false;
                }

                //Console.WriteLine("File ({0}) found.", file[0].FullName);
                filepath = file[0].FullName;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
                filepath = null;
            }

            return true;
        }

        /**
         * DisplayAssemblies
         * Display all assemblies loaded in given AppDomain
         */
        public static void DisplayAssemblies(AppDomain Domain)
        {
            Console.WriteLine("\nAssemblies in Domain ({0})", Domain.FriendlyName);
            Assembly[] loadedAssemblies = Domain.GetAssemblies();

            foreach (Assembly a in loadedAssemblies)
                Console.WriteLine("Assembly -> Name: ({0}) Version: ({1})", a.GetName().Name, a.GetName().Version);
        }

        static void Main(string[] args)
        {
            string path = @"..\..\..\Repository";
            Loader DemoLoader = new Loader(path);
            List<string> AssemblyName = new List<string>();

            AssemblyName.Add("SampleDriver.dll");
            AssemblyName.Add("SampleDriver.dll");

            List<string> ClassName = DemoLoader.LoadMultipleAssemblies(AssemblyName);

            Console.WriteLine("Listing Class Name");
            foreach (var Name  in ClassName)
            {
                Console.WriteLine("Class({0})", Name);
            }

            Loader.DisplayAssemblies(AppDomain.CurrentDomain);
        }
    }
}

