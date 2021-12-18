@echo off

set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild" "..\src\Roslynator.sln" ^
 /t:Clean ^
 /p:Configuration=Debug ^
 /v:minimal ^
 /m
