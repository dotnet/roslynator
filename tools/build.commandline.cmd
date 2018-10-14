@echo off

dotnet restore --force "..\src\CommandLine.sln"

rd /S /Q "..\src\CommandLine\bin\Release\publish"

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" ^
 /t:Clean,Publish ^
 /p:Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

del /Q "..\src\CommandLine\bin\Release\Roslynator.CommandLine.*.nupkg"

dotnet pack -c Release --no-build -v normal "..\src\CommandLine\CommandLine.csproj"

echo OK
pause
