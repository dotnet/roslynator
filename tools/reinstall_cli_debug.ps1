Remove-Item -Path "$PSScriptRoot/../src/CommandLine/bin/Debug/net10.0" -Recurse
Remove-Item -Path "$PSScriptRoot/../src/CommandLine/bin/Debug/Roslynator.DotNet.Cli.*.nupkg"

dotnet pack "$PSScriptRoot/../src/CommandLine/CommandLine.csproj" -c Debug -v minimal /p:RoslynatorDotNetCli=true,Deterministic=true

dotnet tool uninstall roslynator.dotnet.cli -g
dotnet tool install roslynator.dotnet.cli -g --add-source "$PSScriptRoot/../src/CommandLine/bin/Debug" --version 1.0.0
