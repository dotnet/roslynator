# SyntaxExtensions\.ReplaceWhitespace\<TNode>\(TNode, SyntaxTrivia, TextSpan?\) Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Creates a new node with the whitespace replaced\.

```csharp
public static TNode ReplaceWhitespace<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxTrivia replacement, Microsoft.CodeAnalysis.Text.TextSpan? span = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**replacement** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)?

### Returns

TNode

