@echo off

rd /S /Q "..\src\CommandLine\bin\Debug\netcoreapp3.1"

del /Q "..\src\CommandLine\bin\Debug\Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Debug -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

dotnet tool uninstall roslynator.dotnet.cli -g

dotnet tool install roslynator.dotnet.cli --version 0.1.0-rc3 -g --add-source "..\src\CommandLine\bin\Debug"

echo OK
pause
