---
sidebar_label: CanHaveStatements
---

# CSharpFacts\.CanHaveStatements\(SyntaxKind\) Method

**Containing Type**: [CSharpFacts](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if a syntax of the specified kind can have statements\. It can be either [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax) or [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)\.

```csharp
public static bool CanHaveStatements(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

