call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"

devenv.exe TestHarness.sln /build release

copy TestCode\bin\Release\TestCode.dll Repository
copy TestCode_TestDriver\bin\Release\TestCode_TestDriver.dll Repository
copy TestInterface\bin\Release\TestInterface.dll Repository
