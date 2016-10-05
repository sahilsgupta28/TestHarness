Test Harness

Syntax:

UI.exe /r repository_path /t testrequest1_path testrequest2_path [...]
UI.exe /r repository_path /q testrequest1_path
UI.exe /r repository_path /a AuthorName
UI.exe /r repository_path /s 

	/r Repository   - Path to Repository containing assemblies to be tested
	/t Test Request - XML file specifying test code
	/q Query        - XML file for which results are to be displayed
 	/a Author       - Author Name for which tests are to be displayed
	/s Summary      - Get Summary of all tests executed


Demo:

For Demonstration, 3 test drivers are included

1. SampleProject - Sample Test Driver
   This contains a test driver which simulates positive test, negative test and a test case with exception handling.
   This test driver return a failure as we simulate negative test.

2. SampleProject - ExceptionTestDriver
   This contains a function which does not have exception handling. The test harness should be able to continue operation
   despite the exception thrown by this test code.
   This test driver causes the AppDomain to crash and ultimately mark the test as filed.

3. TestHarness - XmlParserTestDriver
   This test driver tests the XmlParser code used in the test harness.
   It parses "SampleCodeTestRequest.xml" file and verifies its contents.
   This test driver returns a positive test.

   Limitation : SampleCodeTestRequest.xml must be in the directory from where executable is running,
                or from where batch file is executed.


