dotnet restore "../src/Tools/ConfigurationFileGenerator/ConfigurationFileGenerator.csproj" --force
dotnet build "../src/Tools/ConfigurationFileGenerator/ConfigurationFileGenerator.csproj" --no-restore /p:Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591,RS1024,RS1025,RS1026"

if(!$?) { Read-Host; Exit }

dotnet "../src/Tools/ConfigurationFileGenerator/bin/Release/net7.0/Roslynator.ConfigurationFileGenerator.dll" `
 "../src" `
 "../src/Tools/ConfigurationFileGenerator/configuration.md" `
 "build/configuration.md" 
