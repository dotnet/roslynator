$properties="Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591"

dotnet restore "../src/Tools/Tools.sln" --force
dotnet build "../src/Tools/Tools.sln" --no-restore /p:$properties /m

if(!$?) { Read-Host; Exit }

& "../src/Tools/MetadataGenerator/bin/Release/net7.0/Roslynator.MetadataGenerator.exe" "../src"
