dotnet build "$PSScriptRoot/../src/Roslynator.slnx" -c Release `
 /p:ReportAnalyzer=True `
 /fl `
 /flp:Verbosity=diagnostic `
 /m

if(!$?) { Read-Host; Exit }

dotnet build "$PSScriptRoot/../src/Tools/LogParser/LogParser.csproj" -c Release -v minimal /m

dotnet "$PSScriptRoot/../src/Tools/LogParser/bin/Release/netcoreapp3.1/LogParser.dll" "msbuild.log"

Write-Host DONE
