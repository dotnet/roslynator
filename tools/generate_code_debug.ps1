dotnet build "../src/Tools/Tools.sln" -c Debug -v minimal `
 /p:RunCodeAnalysis=false `
 /m

dotnet "../src/Tools/CodeGenerator/bin/Debug/netcoreapp3.1/Roslynator.CodeGenerator.dll" "../src"

Write-Host DONE
