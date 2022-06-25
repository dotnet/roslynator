---
sidebar_label: ContainsDirectives
---

# SyntaxExtensions\.ContainsDirectives\(SyntaxNode, TextSpan\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns true if the node contains any preprocessor directives inside the specified span\.

```csharp
public static bool ContainsDirectives(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

