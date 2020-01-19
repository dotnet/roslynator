@echo off

set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591
set _version=2.3.1

dotnet restore --force "..\src\Roslynator.sln"

%_msbuildPath% "..\src\Tools\Tools.sln" ^
 /t:Clean,Build ^
 /p:%_properties% ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

"..\src\Tools\MetadataGenerator\bin\Release\net472\Roslynator.MetadataGenerator.exe" "..\src"
dotnet "..\src\Tools\CodeGenerator\bin\Release\netcoreapp2.0\CodeGenerator.dll" "..\src"

%_msbuildPath% "..\src\Roslynator.sln" ^
 /t:Clean ^
 /p:Configuration=Debug ^
 /v:minimal ^
 /m

%_msbuildPath% "..\src\Roslynator.sln" ^
 /t:Clean,Build ^
 /p:%_properties% ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\Core.Tests\Core.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\CSharp.Tests\CSharp.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\CSharp.Workspaces.Tests\CSharp.Workspaces.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\Analyzers.Tests\Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\CodeAnalysis.Analyzers.Tests\CodeAnalysis.Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\Formatting.Analyzers.Tests\Formatting.Analyzers.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\CodeFixes.Tests\CodeFixes.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\Refactorings.Tests\Refactorings.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

del /Q "..\src\Analyzers.CodeFixes\bin\Release\Roslynator.Analyzers.*.nupkg"
del /Q "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Release\Roslynator.CodeAnalysis.Analyzers.*.nupkg"
del /Q "..\src\Formatting.Analyzers.CodeFixes\bin\Release\Roslynator.Formatting.Analyzers.*.nupkg"
del /Q "..\src\CodeFixes\bin\Release\Roslynator.CodeFixes.*.nupkg"
del /Q "..\src\Core\bin\Release\Roslynator.Core.*.nupkg"
del /Q "..\src\Workspaces.Core\bin\Release\Roslynator.Workspaces.Core.*.nupkg"
del /Q "..\src\CSharp\bin\Release\Roslynator.CSharp.*.nupkg"
del /Q "..\src\CSharp.Workspaces\bin\Release\Roslynator.CSharp.Workspaces.*.nupkg"

dotnet pack -c Release --no-build -v normal "..\src\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CodeAnalysis.Analyzers.CodeFixes\CodeAnalysis.Analyzers.CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Formatting.Analyzers.CodeFixes\Formatting.Analyzers.CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CodeFixes\CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Core\Core.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Workspaces.Core\Workspaces.Core.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp\CSharp.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp.Workspaces\CSharp.Workspaces.csproj"

del /Q "..\src\VisualStudio\bin\Release\Roslynator.VisualStudio.*.vsix"
ren    "..\src\VisualStudio\bin\Release\Roslynator.VisualStudio.vsix" "Roslynator.VisualStudio.%_version%.vsix"

echo OK
pause
