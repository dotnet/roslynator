@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2019\Community\Common7\IDE\VSIXInstaller" ^
 /q ^
 "..\src\VisualStudio\bin\Debug\Roslynator.VisualStudio.vsix"

pause