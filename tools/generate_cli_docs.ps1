dotnet restore "$PSScriptRoot/../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" 

dotnet build "$PSScriptRoot/../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" -c Release -v minimal `
 /p:Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1573,1591,RS1024,RS1025,RS1026" `

 if(!$?) { Read-Host; Exit }

& "$PSScriptRoot/../src/CommandLine.DocumentationGenerator/bin/Release/net8.0/Roslynator.CommandLine.DocumentationGenerator.exe" `
 build `
 "$PSScriptRoot/../src/CommandLine.DocumentationGenerator/data" `
 "help,migrate"
