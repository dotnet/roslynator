@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Roslynator.sln" ^
 /t:Clean,Build ^
 /p:Configuration=ReleaseTools,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\Tools\AddCodeFileHeader\bin\Release\netcoreapp2.0\AddCodeFileHeader.dll" "..\src"
dotnet "..\src\Tools\MetadataGenerator\bin\Release\netcoreapp2.0\MetadataGenerator.dll" "..\src"
dotnet "..\src\Tools\CodeGenerator\bin\Release\netcoreapp2.0\CodeGenerator.dll" "..\src"
dotnet "..\src\Tools\VersionUpdater\bin\Release\netcoreapp2.0\VersionUpdater.dll" "1.9.0.0"

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Roslynator.sln" ^
 /t:Clean ^
 /p:Configuration=Debug ^
 /v:minimal ^
 /m

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Roslynator.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\Analyzers.Tests\Analyzers.Tests.csproj"

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

dotnet pack -c Release --no-build -v normal "..\src\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CodeFixes\CodeFixes.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp\CSharp.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp.Workspaces\CSharp.Workspaces.csproj"

echo OK
pause
