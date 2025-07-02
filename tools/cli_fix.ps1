#dotnet tool install -g roslynator.dotnet.cli
#Install-Module VSSetup -Scope CurrentUser

$visualStudioPath = Get-VSSetupInstance -All | Select-VSSetupInstance -Require 'Microsoft.VisualStudio.Workload.ManagedDesktop' -Latest | Select-Object -ExpandProperty "InstallationPath"

dotnet build "$PSScriptRoot/../src/CommandLine.slnx" /p:Configuration=Debug /v:m /m

roslynator fix "$PSScriptRoot/../src/Roslynator.sln" `
    --analyzer-assemblies `
    "$PSScriptRoot/../src/Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.CSharp.Analyzers.dll" `
    "$PSScriptRoot/../src/Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.CSharp.Analyzers.CodeFixes.dll" `
    "$PSScriptRoot/../src/CodeAnalysis.Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.CodeAnalysis.Analyzers.dll" `
    "$PSScriptRoot/../src/CodeAnalysis.Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.CodeAnalysis.Analyzers.CodeFixes.dll" `
    "$PSScriptRoot/../src/Formatting.Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.Formatting.Analyzers.dll" `
    "$PSScriptRoot/../src/Formatting.Analyzers.CodeFixes/bin/Debug/netstandard2.0/Roslynator.Formatting.Analyzers.CodeFixes.dll" `
    "$visualStudioPath/Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Microsoft.CodeAnalysis.CSharp.EditorFeatures.dll" `
    "$visualStudioPath/Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Microsoft.CodeAnalysis.CSharp.Features.dll" `
    "$visualStudioPath/Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Microsoft.CodeAnalysis.EditorFeatures.dll" `
    "$visualStudioPath/Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Microsoft.CodeAnalysis.Features.dll" `
    --format `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag `
    --diagnostic-fix-map "RCS1155=Roslynator.RCS1155.OrdinalIgnoreCase"
