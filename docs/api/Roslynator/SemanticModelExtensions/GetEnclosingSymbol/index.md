---
sidebar_label: GetEnclosingSymbol
---

# SemanticModelExtensions\.GetEnclosingSymbol&lt;TSymbol&gt;\(SemanticModel, Int32, CancellationToken\) Method

**Containing Type**: [SemanticModelExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns the innermost symbol of type **TSymbol** that the specified position is considered inside of\.

```csharp
public static TSymbol GetEnclosingSymbol<TSymbol>(this Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, System.Threading.CancellationToken cancellationToken = default) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

TSymbol

