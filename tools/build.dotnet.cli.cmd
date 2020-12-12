@echo off

rd /S /Q "..\src\CommandLine\bin\Release\netcoreapp3.1"

del /Q "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg"

dotnet restore --force "..\src\CommandLine.sln"

dotnet clean "..\src\CommandLine.sln"

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Release -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

set _outDir=..\out\Release
md "%_outDir%"
del /Q "%_outDir%\Release\Roslynator.DotNet.Cli.*.nupkg"
copy "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg" "%_outDir%"

echo OK
pause
