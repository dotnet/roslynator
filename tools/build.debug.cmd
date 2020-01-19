@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\VisualStudio.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false,DeployExtension=false ^
 /nr:false ^
 /v:normal ^
 /m

pause