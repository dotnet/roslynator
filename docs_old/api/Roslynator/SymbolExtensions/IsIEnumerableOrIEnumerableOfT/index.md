---
sidebar_label: IsIEnumerableOrIEnumerableOfT
---

# SymbolExtensions\.IsIEnumerableOrIEnumerableOfT\(ITypeSymbol\) Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns true if the type is [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) or [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\.

```csharp
public static bool IsIEnumerableOrIEnumerableOfT(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

