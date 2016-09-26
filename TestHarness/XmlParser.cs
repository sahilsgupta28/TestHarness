/**
 * XML Parser
 * Parses Test Request implemented as XML file to extract test driver & library
 * 
 * FileName     : XmlParser.cs
 * Author       : Sahil Gupta
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TestHarness
{
    [Serializable]
    public class TestCaseData
    {
        public int Version { get; set; }
        public string TestName { get; set; }
        public string Author { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TestDriver { get; set; }
        public List<string> TestCode { get; set; }

        public void Display()
        {
            Console.WriteLine("{0,12} : {1}", "Version", Version);
            Console.WriteLine("{0,12} : {1}", "Author", Author);
            Console.WriteLine("{0,12} : {1}", "TimeStamp", TimeStamp);
            Console.WriteLine("{0,12} : {1}", "TestName", TestName);
            Console.WriteLine("{0,12} : {1}", "TestDriver", TestDriver);
            foreach (string Library  in TestCode)
            {
                Console.WriteLine("{0,12} : {1}", "Library", Library);
            }
            Console.WriteLine("");
        }
    }

    public class XmlParser : MarshalByRefObject
    {
        private XDocument xDoc;
        public List<TestCaseData> TestCase;

        public XmlParser()
        {
            xDoc = new XDocument();
            TestCase = new List<TestCaseData>();
        }

        public bool ParseTestRequest(string sTestRequest)
        {
            FileStream XML = null;
            try
            {
                Console.WriteLine("\nParsing Test Request File ({0})", sTestRequest);
                Console.WriteLine("Current Domain : {0}", AppDomain.CurrentDomain.FriendlyName);

                XML = new FileStream(sTestRequest, System.IO.FileMode.Open);
                xDoc = XDocument.Load(XML);
                if (xDoc == null)
                {
                    Console.WriteLine("Error: XDocument.Load({0})... FAILED.", XML);
                    return false;
                }

                string Version = xDoc.Descendants("Version").First().Value;
                string Author = xDoc.Descendants("Author").First().Value;
                XElement[] xtests = xDoc.Descendants("Test").ToArray();

                int numTests = xtests.Count();
                for (int i = 0; i < numTests; i++)
                {
                    TestCaseData TestData = new TestCaseData();

                    /* Fill in test case data */
                    TestData.Version = Int32.Parse(Version);
                    TestData.Author = Author;
                    TestData.TimeStamp = DateTime.Now;

                    TestData.TestName = xtests[i].Attribute("Name").Value;
                    TestData.TestDriver = xtests[i].Element("TestDriver").Value;

                    TestData.TestCode = new List<string>();
                    IEnumerable<XElement> xTestCode = xtests[i].Elements("Library");
                    foreach (var library in xTestCode)
                    {
                        TestData.TestCode.Add(library.Value);
                    }

                    /* Add this test data to list of test cases */
                    TestCase.Add(TestData);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }
            finally
            {
                if (null != XML)
                {
                    XML.Close();
                }
                
            }
            return true;
        }

        public void DisplayTestRequest()
        {
            foreach (TestCaseData TestData in TestCase)
            {
                TestData.Display();
            }
        }

    }
}