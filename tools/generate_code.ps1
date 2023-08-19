dotnet restore "../src/Tools/Tools.sln" --force
dotnet build "../src/Tools/Tools.sln" --no-restore /p:Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591,RS1024,RS1025,RS1026" /m

if(!$?) { Read-Host; Exit }

dotnet "../src/Tools/CodeGenerator/bin/Release/net7.0/Roslynator.CodeGenerator.dll" "../src"
