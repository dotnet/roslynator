$outDir = "../out/Release"

New-Item -Path $outDir -ItemType directory -Force
Remove-Item "$outDir/Roslynator.CommandLine.*.nupkg"
Remove-Item "$outDir/Roslynator.DotNet.Cli.*.nupkg"
Remove-Item -Path "../src/CommandLine/bin/Release" -Recurse

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u
dotnet clean "../src/CommandLine.sln" `

dotnet publish "../src/CommandLine.sln" -c Release `
 /p:RoslynatorCommandLine=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" `
 /v:normal `
 /m

if(!$?) { Read-Host; Exit }

dotnet pack -c Release --no-build -v normal /p:RoslynatorCommandLine=true "../src/CommandLine/CommandLine.csproj"

Copy-Item -Path "../src/CommandLine/bin/Release/Roslynator.CommandLine.*.nupkg" -Destination "$outDir"

& "../src/CommandLine.DocumentationGenerator/bin/Release/net7.0/Roslynator.CommandLine.DocumentationGenerator.exe" `
 "../docs/cli" `
 "../src/CommandLine.DocumentationGenerator/data" `
 "help,migrate"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u
dotnet clean "../src/CommandLine.sln"

dotnet pack "../src/CommandLine/CommandLine.csproj" -c Release -v normal `
 /p:RoslynatorDotNetCli=true,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591"
 
Copy-Item -Path "../src/CommandLine/bin/Release/Roslynator.DotNet.Cli.*.nupkg" -Destination "$outDir"
