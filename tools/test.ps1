dotnet build "../src/Tests/Tests.sln" -c Debug -v minimal `
 /p:RunCodeAnalysis=false `
 /m

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/Core.Tests/Core.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/CSharp.Tests/CSharp.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/CSharp.Workspaces.Tests/CSharp.Workspaces.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/Analyzers.Tests/Analyzers.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/CodeAnalysis.Analyzers.Tests/CodeAnalysis.Analyzers.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/Formatting.Analyzers.Tests/Formatting.Analyzers.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/CodeFixes.Tests/CodeFixes.Tests.csproj"

if(!$?) { Read-Host; Exit }

dotnet test -c Debug --no-build "../src/Tests/Refactorings.Tests/Refactorings.Tests.csproj"

if(!$?) { Read-Host; Exit }

write-Host DONE
