/**
 * XML Parser
 * Parses Test Request implemented as XML file to extract test driver & library
 * 
 * FileName     : XmlParser.cs
 * Author       : Sahil Gupta
 * Source       : Jim Fawcett
 * Date         : 24 September 2016 
 * Version      : 1.0
 */

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace XMLParser
{
    public class xmlTestInfo
    {
        /**********************************************************************
                             M E M B E R S
        **********************************************************************/

        public int _Version { get; set; }
        public string _TestName { get; set; }
        public string _Author { get; set; }
        public DateTime _TimeStamp { get; set; }
        public string _TestDriver { get; set; }
        public List<string> _TestCode { get; set; }

        /**********************************************************************
                             P U B L I C   M E T H O D S
        **********************************************************************/

        /**
         * Display
         * Display on console contents of current xmlTestInfo instance
         */
        public void Display()
        {
            Console.WriteLine("  {0,-12} : {1}", "Version", _Version);
            Console.WriteLine("  {0,-12} : {1}", "Author", _Author);
            Console.WriteLine("  {0,-12} : {1}", "TimeStamp", _TimeStamp);
            Console.WriteLine("  {0,-12} : {1}", "TestName", _TestName);
            Console.WriteLine("  {0,-12} : {1}", "TestDriver", _TestDriver);

            foreach (string Library  in _TestCode)
            {
                Console.WriteLine("  {0,-12} : {1}", "Library", Library);
            }
            Console.WriteLine("");
        }
    }

    public class XmlParser
    {
        /**********************************************************************
                         M E M B E R S
         **********************************************************************/
        
        private XDocument _xDoc;
        public List<xmlTestInfo> _xmlTestInfoList;

        /**********************************************************************
                     P U B L I C   M E T H O D S
        **********************************************************************/

        public XmlParser()
        {
            _xDoc = new XDocument();
            _xmlTestInfoList = new List<xmlTestInfo>();
        }

        /**
         * ParseXmlFile
         * Read XML file and fill in-memory data-structure xmlTestInfoList to access it
         */
        public bool ParseXmlFile(string XmlFilePath)
        {
            FileStream XML = null;
            try
            {
                //Console.WriteLine("\n>>>>Parsing Test Request File (AD:{0})<<<<", AppDomain.CurrentDomain.FriendlyName);

                /* Open XML file */
                XML = new FileStream(XmlFilePath, System.IO.FileMode.Open);
                if (null == XML)
                {
                    Console.WriteLine("Error: File Open({0})...FAILED.", XmlFilePath);
                    return false;
                }

                /* Load contents of XML file */
                _xDoc = XDocument.Load(XML);
                if (null == _xDoc)
                {
                    Console.WriteLine("Error: XDocument.Load({0})...FAILED.", XmlFilePath);
                    return false;
                }

                /* Get XML tags */
                string Version = _xDoc.Descendants("Version").First().Value;
                string Author = _xDoc.Descendants("Author").First().Value;
                XElement[] xElement = _xDoc.Descendants("Test").ToArray();

                /* Loop for each test driver, extract and store information in xmlTestList */
                int TestCnt = xElement.Count();
                for (int i = 0; i < TestCnt; i++)
                {
                    xmlTestInfo TestInfo = GetNewTestInfo(Author, Version, xElement[i]);
                    _xmlTestInfoList.Add(TestInfo);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
                return false;
            }
            finally
            {
                if (null != XML)
                    XML.Close();
            }

            return true;
        }

        /**
         * GetNewTestInfo
         * Creates new TestInfo object and fills it with data from XML element
         */
        private xmlTestInfo GetNewTestInfo(string Author, string Version, XElement xElement)
        {
            xmlTestInfo TestInfo = null;

            try
            {
                TestInfo = new xmlTestInfo();

                TestInfo._Version = Int32.Parse(Version);
                TestInfo._Author = Author;
                TestInfo._TimeStamp = DateTime.Now;
                TestInfo._TestName = xElement.Attribute("Name").Value;
                TestInfo._TestDriver = xElement.Element("TestDriver").Value;

                TestInfo._TestCode = new List<string>();
                IEnumerable<XElement> xTestCode = xElement.Elements("Library");
                foreach (var library in xTestCode)
                {
                    TestInfo._TestCode.Add(library.Value);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception : {0}", Ex.Message);
            }

            return TestInfo;
        }

        /**
         * DisplayTestRequest
         * Prints on console, information from _xmlTestLinfoList of current instance
         */
        public void DisplayTestRequest()
        {
            Console.WriteLine("\n-------------------- XML FILE---------------------");
            foreach (xmlTestInfo TestInfo in _xmlTestInfoList)
            {
                TestInfo.Display();
            }
            Console.WriteLine("--------------------------------------------------");
        }

        static void Main(string[] args)
        {
            bool bRet;
            string XmlPath = @"..\..\..\TestRequest\TestRequest.xml";

            XmlParser Parser = new XmlParser();

            bRet = Parser.ParseXmlFile(XmlPath);
            if (false == bRet)
            {
                Console.WriteLine("Error: Parser.ParseTestRequest({0})...FAILED", XmlPath);
                return;
            }

            Parser.DisplayTestRequest();
        }
    }
}