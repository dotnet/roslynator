Remove-Item -Path "$PSScriptRoot/../src/CommandLine/bin/Release/net8.0" -Recurse
Remove-Item -Path "$PSScriptRoot/../src/CommandLine/bin/Release/Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "$PSScriptRoot/../src/CommandLine/CommandLine.csproj" -c Release -v minimal /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1573,1591"

dotnet tool uninstall roslynator.dotnet.cli -g
dotnet tool install roslynator.dotnet.cli -g --add-source "$PSScriptRoot/../src/CommandLine/bin/Release" --version 1.0.0
