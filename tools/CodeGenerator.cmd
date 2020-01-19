@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tools\Tools.sln" ^
 /t:Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

dotnet "..\src\Tools\CodeGenerator\bin\Debug\netcoreapp2.0\CodeGenerator.dll" "..\src"

pause
