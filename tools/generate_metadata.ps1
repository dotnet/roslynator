dotnet restore "../src/Tools/Tools.sln" --force
dotnet build "../src/Tools/Tools.sln" --no-restore /p:"Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591" /m

& "../src/Tools/MetadataGenerator/bin/Release/net7.0/Roslynator.MetadataGenerator" "../src" "../docs"
