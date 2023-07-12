dotnet restore "../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" 

dotnet build "../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" -c Release -v minimal `
 /p:Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 `

 if(!$?) { Read-Host; Exit }

& "../src/CommandLine.DocumentationGenerator/bin/Release/net7.0/Roslynator.CommandLine.DocumentationGenerator.exe" "../docs/cli"

if(!$?) { Read-Host; Exit }

Write-Host DONE
