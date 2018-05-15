@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\VisualStudio\VisualStudio.csproj" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:normal ^
 /m

pause