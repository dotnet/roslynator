---
sidebar_label: GetSymbol
---

# SemanticModelExtensions\.GetSymbol\(SemanticModel, SyntaxNode, CancellationToken\) Method

**Containing Type**: [SemanticModelExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns what symbol, if any, the specified node bound to\.

```csharp
public static Microsoft.CodeAnalysis.ISymbol GetSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.SyntaxNode node, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

