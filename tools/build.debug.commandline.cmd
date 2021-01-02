@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

rem dotnet restore "..\src\CommandLine.sln"

"%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\CommandLine.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:minimal ^
 /m

echo OK
pause
