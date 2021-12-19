@echo off

set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591

%_msbuildPath% "..\src\CommandLine.DocumentationGenerator\CommandLine.DocumentationGenerator.csproj" ^
 /t:Build ^
 /p:%_properties% ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

"..\src\CommandLine.DocumentationGenerator\bin\Release\net48\Roslynator.CommandLine.DocumentationGenerator.exe" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
