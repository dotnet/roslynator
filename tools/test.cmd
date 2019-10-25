@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Tests\Tests.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\Core.Tests\Core.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\CSharp.Tests\CSharp.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\CSharp.Workspaces.Tests\CSharp.Workspaces.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\Analyzers.Tests\Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\CodeAnalysis.Analyzers.Tests\CodeAnalysis.Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\Formatting.Analyzers.Tests\Formatting.Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\CodeFixes.Tests\CodeFixes.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Debug --no-build "..\src\Tests\Refactorings.Tests\Refactorings.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
