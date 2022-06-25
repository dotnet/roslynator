---
sidebar_label: IsValidAccessibility
---

# SyntaxAccessibility\.IsValidAccessibility\(SyntaxNode, Accessibility, Boolean\) Method

**Containing Type**: [SyntaxAccessibility](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the node can have specified accessibility\.

```csharp
public static bool IsValidAccessibility(Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Accessibility accessibility, bool ignoreOverride = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**accessibility** &ensp; [Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

**ignoreOverride** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

Ignore "override" modifier\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

