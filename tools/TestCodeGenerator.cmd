@echo off

set /p _ids=Enter ID(s):

dotnet "..\src\Tools\TestCodeGenerator\bin\Debug\netcoreapp2.0\TestCodeGenerator.dll" "..\src" %_ids%

pause
