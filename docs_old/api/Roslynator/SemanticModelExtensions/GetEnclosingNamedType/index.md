---
sidebar_label: GetEnclosingNamedType
---

# SemanticModelExtensions\.GetEnclosingNamedType\(SemanticModel, Int32, CancellationToken\) Method

**Containing Type**: [SemanticModelExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns the innermost named type symbol that the specified position is considered inside of\.

```csharp
public static Microsoft.CodeAnalysis.INamedTypeSymbol GetEnclosingNamedType(this Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

