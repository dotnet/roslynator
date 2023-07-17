---
sidebar_label: SemanticModelExtensions
---

# SemanticModelExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension methods for a [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)\.

```csharp
public static class SemanticModelExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetEnclosingNamedType(SemanticModel, Int32, CancellationToken)](GetEnclosingNamedType/index.md) | Returns the innermost named type symbol that the specified position is considered inside of\. |
| [GetEnclosingSymbol&lt;TSymbol&gt;(SemanticModel, Int32, CancellationToken)](GetEnclosingSymbol/index.md) | Returns the innermost symbol of type **TSymbol** that the specified position is considered inside of\. |
| [GetSymbol(SemanticModel, SyntaxNode, CancellationToken)](GetSymbol/index.md) | Returns what symbol, if any, the specified node bound to\. |
| [GetTypeByMetadataName(SemanticModel, String)](GetTypeByMetadataName/index.md) | Returns the type within the compilation's assembly using its canonical CLR metadata name\. |
| [GetTypeSymbol(SemanticModel, SyntaxNode, CancellationToken)](GetTypeSymbol/index.md) | Returns type information about the specified node\. |

