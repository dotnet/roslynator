@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Roslynator.sln" ^
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

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Tools\LogParser\LogParser.csproj" ^
 /t:Clean,Build ^
 /p:Configuration=Release ^
 /v:minimal ^
 /m

dotnet "..\src\Tools\LogParser\bin\Release\netcoreapp2.0\LogParser.dll" "msbuild.log"

pause
