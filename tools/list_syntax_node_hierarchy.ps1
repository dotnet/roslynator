#dotnet tool install -g roslynator.dotnet.cli

dotnet build "$PSScriptRoot/../src/CommandLine.slnx" -c Debug -v minimal /m

roslynator list-symbols "$PSScriptRoot/../src/CommandLine/CommandLine.csproj" `
 --depth type `
 --visibility public `
 --external-assemblies Microsoft.CodeAnalysis.dll Microsoft.CodeAnalysis.CSharp.dll `
 --hierarchy-root Microsoft.CodeAnalysis.SyntaxNode `
 --layout type-hierarchy `
 --ignored-parts assemblies containing-namespace-in-type-hierarchy assembly-attributes accessibility base-type constraints `
 --output syntax_node.txt `
 --verbosity d `
 --file-log "roslynator.log" `
 --file-log-verbosity diag
