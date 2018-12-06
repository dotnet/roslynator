@echo off

dotnet restore --force "..\src\Roslynator.sln"

if errorlevel 1 (
 pause
 exit
)

