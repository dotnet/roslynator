@echo off

dotnet restore "..\src\CommandLine.sln"

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
