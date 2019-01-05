@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\MSBuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net461\roslynator" format "..\src\Roslynator.sln" ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag ^
 --end-of-line crlf
pause
