@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Roslynator.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,ReportAnalyzer=True ^
 /v:normal ^
 /fl ^
 /flp:Verbosity=diagnostic ^
 /m

if errorlevel 1 (
 pause
 exit
)

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tools\LogParser\LogParser.csproj" ^
 /t:Clean,Build ^
 /p:Configuration=Release ^
 /v:minimal ^
 /m

dotnet "..\src\Tools\LogParser\bin\Release\netcoreapp2.1\LogParser.dll" "msbuild.log"

pause
