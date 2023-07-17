---
sidebar_label: AddUsings
---

# SyntaxExtensions\.AddUsings\(CompilationUnitSyntax, Boolean, UsingDirectiveSyntax\[\]\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the specified using directives added\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax AddUsings(this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, bool keepSingleLineCommentsOnTop, params Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax[] usings)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**keepSingleLineCommentsOnTop** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**usings** &ensp; [UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)\[\]

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

