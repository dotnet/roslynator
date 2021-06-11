@echo off

dotnet tool uninstall roslynator.dotnet.cli -g

dotnet tool install roslynator.dotnet.cli -g --add-source "..\src\CommandLine\bin\Release"

pause