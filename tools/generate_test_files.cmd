@echo off

set /p _ids=Enter ID(s):

dotnet "..\src\Tools\TestCodeGenerator\bin\Debug\netcoreapp3.1\TestCodeGenerator.dll" "..\src" %_ids%

pause
