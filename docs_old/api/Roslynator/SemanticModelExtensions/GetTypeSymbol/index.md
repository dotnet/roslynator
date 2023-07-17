---
sidebar_label: GetTypeSymbol
---

# SemanticModelExtensions\.GetTypeSymbol\(SemanticModel, SyntaxNode, CancellationToken\) Method

**Containing Type**: [SemanticModelExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns type information about the specified node\.

```csharp
public static Microsoft.CodeAnalysis.ITypeSymbol GetTypeSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.SyntaxNode node, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

