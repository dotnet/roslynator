@echo off

set _outDir=..\out\Release
md "%_outDir%"
del /Q "%_outDir%\Roslynator.CommandLine.*.nupkg"
del /Q "%_outDir%\Roslynator.DotNet.Cli.*.nupkg"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u

dotnet restore --force "..\src\CommandLine.sln" /p:RoslynatorCommandLine=true

rd /S /Q "..\src\CommandLine\bin\Release"

dotnet build "..\src\CommandLine.sln" ^
 /t:Clean,Publish ^
 /p:Configuration=Release,RoslynatorCommandLine=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet pack -c Release --no-build -v normal /p:RoslynatorCommandLine=true "..\src\CommandLine\CommandLine.csproj"

copy "..\src\CommandLine\bin\Release\Roslynator.CommandLine.*.nupkg" "%_outDir%"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Release -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

copy "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg" "%_outDir%"

echo OK
pause
