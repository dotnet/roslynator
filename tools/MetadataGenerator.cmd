@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tools\Tools.sln" ^
 /t:Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

"..\src\Tools\MetadataGenerator\bin\Debug\net472\Roslynator.MetadataGenerator.exe" "..\src"

pause
