@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net461\roslynator" analyze "..\src\Roslynator.sln" ^
 --use-roslynator-analyzers ^
 --ignore-analyzer-references ^
 --ignored-diagnostics CS1591 ^
 --severity-level info ^
 --culture en ^
 --verbosity n ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag ^
 --output "diagnostics.xml"

pause
