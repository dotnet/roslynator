---
sidebar_label: IsOrdinaryExtensionMethod
---

# SymbolExtensions\.IsOrdinaryExtensionMethod\(IMethodSymbol\) Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns true if this method is an ordinary extension method \(i\.e\. "this" parameter has not been removed\)\.

```csharp
public static bool IsOrdinaryExtensionMethod(this Microsoft.CodeAnalysis.IMethodSymbol methodSymbol)
```

### Parameters

**methodSymbol** &ensp; [IMethodSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

