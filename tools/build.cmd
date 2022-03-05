@echo off

set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591
set _outDir=..\out\Release
set _version=4.0.2
set _version4=4.0.2.0

orang replace "..\src\VisualStudio\source.extension.vsixmanifest" ^
 -c "patterns\vsix_manifest_version.txt" from-file -t m r ^
 -r %_version%

orang replace ^
  "..\src\VisualStudio\Properties\AssemblyInfo.cs" ^
  "..\src\VisualStudio.Common\Properties\AssemblyInfo.cs" ^
 -c "patterns\assembly_info_version.txt" from-file -t m r ^
 -r %_version4%

orang delete "..\src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s

echo.

dotnet restore "..\src\Roslynator.sln"
dotnet restore "..\src\VisualStudio.sln"
dotnet restore "..\src\Tools\Tools.sln"

%_msbuildPath% "..\src\Tools\Tools.sln" ^
 /t:Build ^
 /p:%_properties% ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

"..\src\Tools\MetadataGenerator\bin\Release\net48\Roslynator.MetadataGenerator.exe" "..\src"
dotnet "..\src\Tools\CodeGenerator\bin\Release\netcoreapp3.1\Roslynator.CodeGenerator.dll" "..\src"

%_msbuildPath% "..\src\Roslynator.sln" ^
 /t:Build ^
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

md "%_outDir%"
orang delete "%_outDir%"

orang rename "../src/VisualStudio" -n "(?=\.vsix\z)" -r ".%_version%"

dotnet pack -c Release --no-build -v normal "..\src\Core\Core.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Workspaces.Core\Workspaces.Core.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp\CSharp.csproj"
dotnet pack -c Release --no-build -v normal "..\src\CSharp.Workspaces\CSharp.Workspaces.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.Common\Testing.Common.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.CSharp\Testing.CSharp.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.CSharp.Xunit\Testing.CSharp.Xunit.csproj"

orang copy "../src" "%_outDir%" -e nupkg,vsix --flat -i packages e ne

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src\Roslynator.sln"
%_msbuildPath% "../src\Roslynator.sln" /t:Build /p:%_properties%,RoslynatorAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "..\src\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj"
copy "..\src\Analyzers.CodeFixes\bin\Release\Roslynator.Analyzers.*.nupkg" "%_outDir%"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_CodeAnalysis_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src\Roslynator.sln"
%_msbuildPath% "../src\Roslynator.sln" /t:Build /p:%_properties%,RoslynatorCodeAnalysisAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "..\src\CodeAnalysis.Analyzers.CodeFixes\CodeAnalysis.Analyzers.CodeFixes.csproj"
copy "..\src\CodeAnalysis.Analyzers.CodeFixes\bin\Release\Roslynator.CodeAnalysis.Analyzers.*.nupkg" "%_outDir%"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_Formatting_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src\Roslynator.sln"
%_msbuildPath% "../src\Roslynator.sln" /t:Build /p:%_properties%,RoslynatorFormattingAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "..\src\Formatting.Analyzers.CodeFixes\Formatting.Analyzers.CodeFixes.csproj"
copy "..\src\Formatting.Analyzers.CodeFixes\bin\Release\Roslynator.Formatting.Analyzers.*.nupkg" "%_outDir%"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

echo OK
pause
