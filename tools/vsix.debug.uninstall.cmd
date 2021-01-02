@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2019\Community\Common7\IDE\VSIXInstaller" ^
 /q ^
 /u:d42db039-5432-4399-bb62-67a9b4c3b838

pause