dotnet restore "$PSScriptRoot/../src/Tools/Tools.slnx" --force
dotnet build "$PSScriptRoot/../src/Tools/Tools.slnx" --no-restore /p:"Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=`"1573,1591,RS1025,RS1026`"" /m

if(!$?) { Read-Host; Exit }

& "$PSScriptRoot/../src/Tools/MetadataGenerator/bin/Release/net9.0/Roslynator.MetadataGenerator" "../src" "build"
