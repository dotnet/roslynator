@echo off

set _outDir=..\out\Debug

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\VisualStudio.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false,DeployExtension=false ^
 /nr:false ^
 /v:normal ^
 /m

md "%_outDir%"

del /Q "%_outDir%\*"

copy "..\src\VisualStudio\bin\Debug\Roslynator.VisualStudio.vsix" "%_outDir%"

pause