Remove-Item -Path "../src/CommandLine/bin/Release/net7.0" -Recurse
Remove-Item -Path "../src/CommandLine/bin/Release/Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "../src/CommandLine/CommandLine.csproj" -c Release -v minimal `
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"

dotnet tool uninstall roslynator.dotnet.cli -g

dotnet tool install roslynator.dotnet.cli -g --add-source "../src/CommandLine/bin/Release" --version 1.0.0

Write-Host DONE
