@echo off

set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net48\roslynator" analyze "..\src\Roslynator.sln" ^
 -m %_msbuildPath% ^
 --analyzer-assemblies ^
  "..\src\Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CSharp.Analyzers.dll" ^
  "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CodeAnalysis.Analyzers.dll" ^
  "..\src\Formatting.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.Formatting.Analyzers.dll" ^
 --ignore-analyzer-references ^
 --ignored-diagnostics CS1591 ^
 --severity-level info ^
 --culture en ^
 --verbosity n ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause
