@echo off

set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net48\roslynator" list-symbols "..\src\CommandLine\CommandLine.csproj" ^
 -m %_msbuildPath% ^
 --depth type ^
 --visibility public ^
 --external-assemblies Microsoft.CodeAnalysis.dll Microsoft.CodeAnalysis.CSharp.dll ^
 --hierarchy-root Microsoft.CodeAnalysis.SyntaxNode ^
 --layout type-hierarchy ^
 --ignored-parts assemblies containing-namespace-in-type-hierarchy assembly-attributes accessibility base-type constraints ^
 --output syntax_node.txt ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause
