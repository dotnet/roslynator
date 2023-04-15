#dotnet tool install -g roslynator.dotnet.cli

dotnet build "..\src\CommandLine.sln" /p:Configuration=Debug /v:m /m

roslynator analyze "..\src\Roslynator.sln" `
    --analyzer-assemblies `
    "..\src\Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CSharp.Analyzers.dll" `
    "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CodeAnalysis.Analyzers.dll" `
    "..\src\Formatting.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.Formatting.Analyzers.dll" `
    --ignore-analyzer-references `
    --ignored-diagnostics CS1591 `
    --severity-level info `
    --culture en `
    --verbosity n `
    --file-log "roslynator.log" `
    --file-log-verbosity diag
