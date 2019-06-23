# SyntaxListSelection\<TNode>\.TryCreate\(SyntaxList\<TNode>, TextSpan, SyntaxListSelection\<TNode>\) Method

[Home](../../../README.md)

**Containing Type**: [SyntaxListSelection\<TNode>](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Creates a new [SyntaxListSelection\<TNode>](../README.md) based on the specified list and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.SyntaxListSelection<TNode> selection)
```

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selection** &ensp; [SyntaxListSelection\<TNode>](../README.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one node; otherwise, false\.