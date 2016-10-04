TestHarness\bin\Release\TestHarness -repo .\Repository -test .\TestRequest\TestRequest.xml
pause
cls

TestHarness\bin\Release\TestHarness -repo .\Repository -query .\TestRequest\TestRequest.xml
pause
cls

TestHarness\bin\Release\TestHarness -repo .\Repository  -querySummary
pause
cls

TestHarness\bin\Release\TestHarness -repo .\Repository  -queryall