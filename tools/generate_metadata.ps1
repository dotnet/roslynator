
dotnet build "../src/Tools/Tools.sln" -c Debug `
 /p:RunCodeAnalysis=false `
 /v:minimal `
 /m

& "../src/Tools/MetadataGenerator/bin/Debug/net7.0/Roslynator.MetadataGenerator.exe" "../src"

