@echo off

rem dotnet restore "..\src\CommandLine.sln"

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\CommandLine.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:minimal ^
 /m

echo OK
pause
