call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"

devenv.exe TestHarness.sln /rebuild release

devenv.exe .\SampleProject\SampleProject.sln /rebuild release
