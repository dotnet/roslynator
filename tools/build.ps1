$properties="Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591"
$outDir="../out/Release"
$version="4.3.0"
$version4="4.3.0.0"

$msbuildExe = ./vswhere.exe -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
Write-Host "MSBuild location: $msbuildExe"

orang replace "../src/VisualStudio/source.extension.vsixmanifest" -c "patterns/vsix_manifest_version.txt" from-file -t m r -r $version
orang replace "../src/VisualStudio/Properties/AssemblyInfo.cs" -c "patterns/assembly_info_version.txt" from-file -t m r -r $version4
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u

Write-Host

dotnet restore "../src/Roslynator.sln"
dotnet restore "../src/VisualStudio.sln"
dotnet restore "../src/Tools/Tools.sln"

dotnet build "../src/Tools/Tools.sln" `
 /p:$properties `
 /v:normal `
 /m

 if(!$?) { Read-Host; Exit }

& "../src/Tools/MetadataGenerator/bin/Release/net7.0/Roslynator.MetadataGenerator.exe" "../src"
dotnet "../src/Tools/CodeGenerator/bin/Release/netcoreapp3.1/Roslynator.CodeGenerator.dll" "../src"

$msbuildExe = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

& $msbuildExe "../src/Roslynator.sln" /t:Build /p:$properties /m

if(!$?) { Read-Host; Exit }

dotnet test -c Release --no-build "../src/Tests/Core.Tests/Core.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/CSharp.Tests/CSharp.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/CSharp.Workspaces.Tests/CSharp.Workspaces.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/Analyzers.Tests/Analyzers.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/CodeAnalysis.Analyzers.Tests/CodeAnalysis.Analyzers.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/Formatting.Analyzers.Tests/Formatting.Analyzers.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/CodeFixes.Tests/CodeFixes.Tests.csproj"
if(!$?) { Read-Host; Exit }
dotnet test -c Release --no-build "../src/Tests/Refactorings.Tests/Refactorings.Tests.csproj"
if(!$?) { Read-Host; Exit }

mkdir "$outDir" -Force
orang delete "$outDir"

orang rename "../src/VisualStudio" -n "(?=/.vsix/z)" -r ".$version"

orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -u

dotnet restore "../src/Roslynator.sln"

dotnet pack -c Release -v normal "../src/Core/Core.csproj"
dotnet pack -c Release -v normal "../src/Workspaces.Core/Workspaces.Core.csproj"
dotnet pack -c Release -v normal "../src/CSharp/CSharp.csproj"
dotnet pack -c Release -v normal "../src/CSharp.Workspaces/CSharp.Workspaces.csproj"
dotnet pack -c Release -v normal "../src/Tests/Testing.Common/Testing.Common.csproj"
dotnet pack -c Release -v normal "../src/Tests/Testing.CSharp/Testing.CSharp.csproj"
dotnet pack -c Release -v normal "../src/Tests/Testing.CSharp.Xunit/Testing.CSharp.Xunit.csproj"
dotnet pack -c Release -v normal "../src/Tests/Testing.CSharp.MSTest/Testing.CSharp.MSTest.csproj"

orang copy "../src" "$outDir" -e nupkg,vsix --flat -i packages e ne

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src/Roslynator.sln"
dotnet build "../src/Roslynator.sln" /p:$properties,RoslynatorAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "../src/Analyzers.CodeFixes/Analyzers.CodeFixes.csproj"
Copy-Item "../src/Analyzers.CodeFixes/bin/Release/Roslynator.Analyzers.*.nupkg" "$outDir"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_CodeAnalysis_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src/Roslynator.sln"
dotnet build "../src/Roslynator.sln" /p:$properties,RoslynatorCodeAnalysisAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "../src/CodeAnalysis.Analyzers.CodeFixes/CodeAnalysis.Analyzers.CodeFixes.csproj"
Copy-Item "../src/CodeAnalysis.Analyzers.CodeFixes/bin/Release/Roslynator.CodeAnalysis.Analyzers.*.nupkg" "$outDir"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r "Roslynator_Formatting_Analyzers_"
orang delete "../src" -a d -n "bin,obj" l li e -i "packages,node_modules" l li e ne -t n --content-only -y su s
dotnet restore --force "../src/Roslynator.sln"
dotnet build "../src/Roslynator.sln" /p:$properties,RoslynatorFormattingAnalyzersNuGet=true /v:normal /m
dotnet pack -c Release --no-build -v normal "../src/Formatting.Analyzers.CodeFixes/Formatting.Analyzers.CodeFixes.csproj"
Copy-Item "../src/Formatting.Analyzers.CodeFixes/bin/Release/Roslynator.Formatting.Analyzers.*.nupkg" "$outDir"
orang replace "../src" -n "AssemblyInfo.cs" e -c "patterns/assembly_names_to_be_prefixed.txt" f -r ""

Write-Host DONE
