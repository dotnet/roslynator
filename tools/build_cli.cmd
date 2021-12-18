@echo off

set _programFiles=%ProgramFiles%

set _outDir=..\out\Release
md "%_outDir%"
del /Q "%_outDir%\Roslynator.CommandLine.*.nupkg"
del /Q "%_outDir%\Roslynator.DotNet.Cli.*.nupkg"

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_Cli_"
orang replace "../src/commandline/commandline.csproj" -c "<TargetFramework>net5.0</TargetFramework>" l -r "<TargetFrameworks>net5.0;net6.0</TargetFrameworks>"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s

dotnet restore --force "..\src\CommandLine.sln"

rd /S /Q "..\src\CommandLine\bin\Release"

"%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild" "..\src\CommandLine.sln" ^
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

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Release -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

copy "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg" "%_outDir%"

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""
orang replace "../src/commandline/commandline.csproj" -c "<TargetFrameworks>net5.0;net6.0</TargetFrameworks>" l -r "<TargetFramework>net5.0</TargetFramework>"

echo OK
pause
