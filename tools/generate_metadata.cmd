@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tools\Tools.sln" ^
 /t:Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

"..\src\Tools\MetadataGenerator\bin\Debug\net48\Roslynator.MetadataGenerator.exe" "..\src"

pause
