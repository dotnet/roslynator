$version="4.3.0"

orang replace "../src/VisualStudioCode/package/package.json" `
 -c "patterns/vscode_version.txt" from-file -t m r `
 -r $version

dotnet restore --force "../src/VisualStudioCode.sln"

dotnet build "../src/VisualStudioCode.sln" -c Release -v normal `
 /p:Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591,DefineConstants=VSCODE `
 /m

if(!$?) { Read-Host; Exit }

Set-Location ../src/VisualStudioCode/package

Remove-Item roslyn/*.dll

Set-Location roslyn

mkdir common -Force
mkdir analyzers -Force
mkdir fixes -Force
mkdir refactorings -Force

Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Core.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Common.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Workspaces.Core.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Workspaces.Common.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.Workspaces.dll -Destination common
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.Analyzers.dll -Destination analyzers
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.Analyzers.CodeFixes.dll -Destination analyzers
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Formatting.Analyzers.dll -Destination analyzers
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.Formatting.Analyzers.CodeFixes.dll -Destination analyzers
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.Refactorings.dll -Destination refactorings
Copy-Item -Path ../../bin/Release/netstandard2.0/Roslynator.CSharp.CodeFixes.dll -Destination fixes

Set-Location ..

npm install

Write-Host Package is being created
vsce package
Write-Host Package successfully created

Set-Location ../../../tools
