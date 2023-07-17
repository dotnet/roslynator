---
sidebar_label: TryCreate
---

# SeparatedSyntaxListSelection&lt;TNode&gt;\.TryCreate\(SeparatedSyntaxList&lt;TNode&gt;, TextSpan, SeparatedSyntaxListSelection&lt;TNode&gt;\) Method

**Containing Type**: [SeparatedSyntaxListSelection&lt;TNode&gt;](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Creates a new [SeparatedSyntaxListSelection&lt;TNode&gt;](../index.md) based on the specified list and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.SeparatedSyntaxListSelection<TNode> selection)
```

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selection** &ensp; [SeparatedSyntaxListSelection&lt;TNode&gt;](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one node; otherwise, false\.