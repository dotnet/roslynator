Remove-Item -Path "../src/CommandLine/bin/Debug/net7.0" -Recurse
Remove-Item -Path "../src/CommandLine/bin/Debug/Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "../src/CommandLine/CommandLine.csproj" -c Debug -v minimal `
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

dotnet tool uninstall roslynator.dotnet.cli -g

dotnet tool install roslynator.dotnet.cli -g --add-source "../src/CommandLine/bin/Debug" --version 1.0.0

Write-Host DONE
