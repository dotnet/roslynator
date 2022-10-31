#dotnet tool install -g orang.dotnet.cli

$outDir = "../out/Release"

New-Item -Path $outDir -ItemType directory
Remove-Item "$outDir/Roslynator.CommandLine.*.nupkg"
Remove-Item "$outDir/Roslynator.DotNet.Cli.*.nupkg"
Remove-Item -Path "../src/CommandLine/bin/Release" -Recurse

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet clean "../src/CommandLine.sln" `

dotnet publish "../src/CommandLine.sln" -c Release `
 /p:RoslynatorCommandLine=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" `
 /v:normal `
 /m

dotnet pack -c Release --no-build -v normal /p:RoslynatorCommandLine=true "../src/CommandLine/CommandLine.csproj"

Copy-Item -Path "../src/CommandLine/bin/Release/Roslynator.CommandLine.*.nupkg" -Destination "$outDir"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet clean "../src/CommandLine.sln"

dotnet pack "../src/CommandLine/CommandLine.csproj" -c Release -v normal `
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"
 
Copy-Item -Path "../src/CommandLine/bin/Release/Roslynator.DotNet.Cli.*.nupkg" -Destination "$outDir"

Write-Host "DONE"
Read-Host
