@echo off

set _visualStudioPath=C:\Program Files\Microsoft Visual Studio\2019\Community
set _msbuildPath="%_visualStudioPath%\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net472\roslynator" fix "..\src\Roslynator.sln" ^
 --msbuild-path %_msbuildPath% ^
 --analyzer-assemblies ^
  "..\src\Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CSharp.Analyzers.dll" ^
  "..\src\Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CSharp.Analyzers.CodeFixes.dll" ^
  "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CodeAnalysis.Analyzers.dll" ^
  "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.CodeAnalysis.Analyzers.CodeFixes.dll" ^
  "..\src\Formatting.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.Formatting.Analyzers.dll" ^
  "..\src\Formatting.Analyzers.CodeFixes\bin\Debug\netstandard2.0\Roslynator.Formatting.Analyzers.CodeFixes.dll" ^
  "%_visualStudioPath%\Common7\IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.CSharp.EditorFeatures.dll" ^
  "%_visualStudioPath%\Common7\IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.CSharp.Features.dll" ^
  "%_visualStudioPath%\Common7\IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.EditorFeatures.dll" ^
  "%_visualStudioPath%\Common7\IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices\Microsoft.CodeAnalysis.Features.dll" ^
 --format ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag ^
 --diagnostic-fix-map "RCS1155=Roslynator.RCS1155.OrdinalIgnoreCase" ^
 --file-banner " Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."

pause
