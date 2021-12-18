@echo off

set _programFiles=%ProgramFiles%

dotnet restore "..\src\CommandLine.sln"

"%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild" "..\src\CommandLine.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug ^
 /v:minimal ^
 /m

echo OK
pause
