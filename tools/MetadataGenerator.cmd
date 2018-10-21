@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Tools\Tools.sln" ^
 /t:Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

"..\src\Tools\MetadataGenerator\bin\Debug\net461\Roslynator.MetadataGenerator.exe" "..\src"

pause
