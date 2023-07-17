---
sidebar_label: HasConstantValue
---

# CSharpExtensions\.HasConstantValue\(SemanticModel, ExpressionSyntax, CancellationToken\) Method

**Containing Type**: [CSharpExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the specified node has a constant value\.

```csharp
public static bool HasConstantValue(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

