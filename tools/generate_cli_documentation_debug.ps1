dotnet build "../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" -c Debug -v minimal `
 /p:Deterministic=true `
 /m

 if(!$?) { Read-Host; Exit }

& "../src/CommandLine.DocumentationGenerator/bin/Debug/net7.0/Roslynator.CommandLine.DocumentationGenerator.exe" "../docs/cli"

if(!$?) { Read-Host; Exit }
