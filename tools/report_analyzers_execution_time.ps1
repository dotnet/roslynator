dotnet build "../src/Roslynator.sln" -c Release `
 /p:ReportAnalyzer=True `
 /fl `
 /flp:Verbosity=diagnostic `
 /m

if(!$?) { Read-Host; Exit }

dotnet build "../src/Tools/LogParser/LogParser.csproj" -c Release -v minimal /m

dotnet "../src/Tools/LogParser/bin/Release/netcoreapp3.1/LogParser.dll" "msbuild.log"

Write-Host DONE
