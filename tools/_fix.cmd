@echo off

set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net472\roslynator" fix "..\src\Roslynator.sln" ^
 --msbuild-path %_msbuildPath% ^
 --analyzer-assemblies ^
  "..\src\CommandLine\bin\Debug\net472\Roslynator.CSharp.Analyzers.dll" ^
  "..\src\CommandLine\bin\Debug\net472\Roslynator.CSharp.Analyzers.CodeFixes.dll" ^
 --format ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag ^
 --diagnostic-fix-map "RCS1155=Roslynator.RCS1155.OrdinalIgnoreCase" ^
 --file-banner " Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."

pause
