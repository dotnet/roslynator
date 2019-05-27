@echo off

dotnet restore --force "..\src\CommandLine.sln"

rd /S /Q "..\src\CommandLine\bin\Release\publish"

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" ^
 /t:Clean,Publish ^
 /p:Configuration=Release,RoslynatorCommandLine=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

del /Q "..\src\CommandLine\bin\Release\Roslynator.CommandLine.*.nupkg"

dotnet pack -c Release --no-build -v normal /p:RoslynatorCommandLine=true "..\src\CommandLine\CommandLine.csproj"

if errorlevel 1 (
 pause
 exit
)

del /Q "..\src\CommandLine\bin\Release\Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "..\src\CommandLine\CommandLine.csproj" -c Release -v normal ^
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

echo OK
pause
