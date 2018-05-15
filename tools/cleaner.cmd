@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Tools\Cleaner\Cleaner.csproj" ^
 /t:Build ^
 /p:Configuration=Release,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

dotnet "..\src\Tools\Cleaner\bin\Release\netcoreapp2.0\Cleaner.dll" "..\src"
dotnet "..\src\Tools\Cleaner\bin\Release\netcoreapp2.0\Cleaner.dll" "..\src\tools"

pause
