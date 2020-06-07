@echo off

dotnet tool uninstall roslynator.dotnet.cli -g

dotnet tool install roslynator.dotnet.cli --version 0.1.0-rc3 -g --add-source "..\src\CommandLine\bin\Release"

pause