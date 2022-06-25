---
sidebar_label: GetReducedExtensionMethodInfo
---

# CSharpExtensions\.GetReducedExtensionMethodInfo\(SemanticModel, ExpressionSyntax, CancellationToken\) Method

**Containing Type**: [CSharpExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns what extension method symbol, if any, the specified expression syntax bound to\.

```csharp
public static Roslynator.ExtensionMethodSymbolInfo GetReducedExtensionMethodInfo(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ExtensionMethodSymbolInfo](../../../ExtensionMethodSymbolInfo/index.md)

