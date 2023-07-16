$properties="Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591"

dotnet restore "../src/Tools/Tools.sln" --force
dotnet build "../src/Tools/Tools.sln" --no-restore /p:$properties /m

if(!$?) { Read-Host; Exit }

dotnet "../src/Tools/CodeGenerator/bin/Release/netcoreapp3.1/Roslynator.CodeGenerator.dll" "../src"
