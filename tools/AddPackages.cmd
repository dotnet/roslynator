@echo off

dotnet add "..\src\analyzers\analyzers.csproj" package Roslynator.Analyzers
dotnet add "..\src\analyzers.codefixes\analyzers.codefixes.csproj" package Roslynator.Analyzers
dotnet add "..\src\codefixes\codefixes.csproj" package Roslynator.Analyzers
dotnet add "..\src\common\common.csproj" package Roslynator.Analyzers
dotnet add "..\src\common.workspaces\common.workspaces.csproj" package Roslynator.Analyzers
dotnet add "..\src\core\core.csproj" package Roslynator.Analyzers
dotnet add "..\src\csharp\csharp.csproj" package Roslynator.Analyzers
dotnet add "..\src\csharp.workspaces\csharp.workspaces.csproj" package Roslynator.Analyzers
dotnet add "..\src\refactorings\refactorings.csproj" package Roslynator.Analyzers

dotnet add "..\src\tests\analyzers.tests\analyzers.tests.csproj" package Roslynator.Analyzers
dotnet add "..\src\tests\codefixes.tests\codefixes.tests.csproj" package Roslynator.Analyzers
dotnet add "..\src\tests\refactorings.tests\refactorings.tests.csproj" package Roslynator.Analyzers
dotnet add "..\src\tests\tests\tests.csproj" package Roslynator.Analyzers

dotnet add "..\src\tools\addcodefileheader\addcodefileheader.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\cleaner\cleaner.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\codegeneration\codegeneration.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\codegenerator\codegenerator.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\logparser\logparser.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\metadata\metadata.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\metadatagenerator\metadatagenerator.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\utilities\utilities.csproj" package Roslynator.Analyzers
dotnet add "..\src\tools\versionupdater\versionupdater.csproj" package Roslynator.Analyzers
