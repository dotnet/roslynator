@echo off

set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\VSIXInstaller" ^
 /q ^
 "..\src\VisualStudio\bin\Debug\Roslynator.VisualStudio.vsix"

pause