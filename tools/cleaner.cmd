@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tools\Cleaner\Cleaner.csproj" ^
 /t:Build ^
 /p:Configuration=Release,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

dotnet "..\src\Tools\Cleaner\bin\Release\netcoreapp2.0\Cleaner.dll" "..\src"
dotnet "..\src\Tools\Cleaner\bin\Release\netcoreapp2.0\Cleaner.dll" "..\src\tools"

pause
