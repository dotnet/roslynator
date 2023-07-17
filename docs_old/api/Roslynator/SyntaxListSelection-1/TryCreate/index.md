---
sidebar_label: TryCreate
---

# SyntaxListSelection&lt;TNode&gt;\.TryCreate\(SyntaxList&lt;TNode&gt;, TextSpan, SyntaxListSelection&lt;TNode&gt;\) Method

**Containing Type**: [SyntaxListSelection&lt;TNode&gt;](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Creates a new [SyntaxListSelection&lt;TNode&gt;](../index.md) based on the specified list and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.SyntaxListSelection<TNode> selection)
```

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selection** &ensp; [SyntaxListSelection&lt;TNode&gt;](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one node; otherwise, false\.