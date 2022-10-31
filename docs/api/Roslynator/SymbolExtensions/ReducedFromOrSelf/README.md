# SymbolExtensions\.ReducedFromOrSelf\(IMethodSymbol\) Method

[Home](../../../README.md)

**Containing Type**: [SymbolExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
If this method is a reduced extension method, returns the definition of extension method from which this was reduced\. Otherwise, returns this symbol\.

```csharp
public static Microsoft.CodeAnalysis.IMethodSymbol ReducedFromOrSelf(this Microsoft.CodeAnalysis.IMethodSymbol methodSymbol)
```

### Parameters

**methodSymbol** &ensp; [IMethodSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol)

### Returns

[IMethodSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol)

