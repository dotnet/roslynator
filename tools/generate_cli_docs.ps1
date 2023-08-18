dotnet restore "../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" 

dotnet build "../src/CommandLine.DocumentationGenerator/CommandLine.DocumentationGenerator.csproj" -c Release -v minimal `
 /p:Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591,RS1024,RS1025,RS1026" `

 if(!$?) { Read-Host; Exit }

& "../src/CommandLine.DocumentationGenerator/bin/Release/net7.0/Roslynator.CommandLine.DocumentationGenerator.exe" `
 build `
 "../src/CommandLine.DocumentationGenerator/data" `
 "help,migrate,generate-doc-root"
