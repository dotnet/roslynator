@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Debug,Deterministic=true

%_msbuildPath% "..\src\CommandLine.DocumentationGenerator\CommandLine.DocumentationGenerator.csproj" ^
 /t:Build ^
 /p:%_properties% ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

"..\src\CommandLine.DocumentationGenerator\bin\Debug\net48\Roslynator.CommandLine.DocumentationGenerator.exe" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
