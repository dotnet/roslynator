@echo off

set /p _apiKey=Enter API Key:
set _source=https://api.nuget.org/v3/index.json

set _version=2.0.0

set _filePath=..\src\Analyzers.CodeFixes\bin\Release\Roslynator.Analyzers.%_version%.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

set _filePath=..\src\CodeFixes\bin\Release\Roslynator.CodeFixes.%_version%.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

set _version=1.0.0-rc5

set _filePath=..\src\Core\bin\Release\Roslynator.Core.%_version%.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

set _filePath=..\src\CSharp\bin\Release\Roslynator.CSharp.%_version%.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

set _filePath=..\src\CSharp.Workspaces\bin\Release\Roslynator.CSharp.Workspaces.%_version%.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

set _filePath=..\src\CommandLine\bin\Release\Roslynator.CommandLine.0.1.0-beta2.nupkg
rem dotnet nuget push "%_filePath%" -k %_apiKey% -s %_source%

echo OK
pause
