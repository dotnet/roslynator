@echo off

dotnet remove "..\src\analyzers\analyzers.csproj" package Roslynator.Analyzers
dotnet remove "..\src\analyzers.codefixes\analyzers.codefixes.csproj" package Roslynator.Analyzers
dotnet remove "..\src\codefixes\codefixes.csproj" package Roslynator.Analyzers
dotnet remove "..\src\common\common.csproj" package Roslynator.Analyzers
dotnet remove "..\src\common.workspaces\common.workspaces.csproj" package Roslynator.Analyzers
dotnet remove "..\src\core\core.csproj" package Roslynator.Analyzers
dotnet remove "..\src\csharp\csharp.csproj" package Roslynator.Analyzers
dotnet remove "..\src\csharp.workspaces\csharp.workspaces.csproj" package Roslynator.Analyzers
dotnet remove "..\src\refactorings\refactorings.csproj" package Roslynator.Analyzers

dotnet remove "..\src\tests\analyzers.tests\analyzers.tests.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tests\codefixes.tests\codefixes.tests.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tests\refactorings.tests\refactorings.tests.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tests\tests\tests.csproj" package Roslynator.Analyzers

dotnet remove "..\src\tools\addcodefileheader\addcodefileheader.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\cleaner\cleaner.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\codegeneration\codegeneration.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\codegenerator\codegenerator.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\logparser\logparser.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\metadata\metadata.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\metadatagenerator\metadatagenerator.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\utilities\utilities.csproj" package Roslynator.Analyzers
dotnet remove "..\src\tools\versionupdater\versionupdater.csproj" package Roslynator.Analyzers
