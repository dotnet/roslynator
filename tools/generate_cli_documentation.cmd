@echo off

dotnet restore "..\src\CommandLine.DocumentationGenerator\CommandLine.DocumentationGenerator.csproj" 

dotnet build "..\src\CommandLine.DocumentationGenerator\CommandLine.DocumentationGenerator.csproj" ^
 /p:Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /v:minimal

if errorlevel 1 (
 pause
 exit
)

"..\src\CommandLine.DocumentationGenerator\bin\Release\net7.0\Roslynator.CommandLine.DocumentationGenerator.exe" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
