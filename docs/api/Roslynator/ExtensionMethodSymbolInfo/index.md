---
sidebar_label: ExtensionMethodSymbolInfo
---

# ExtensionMethodSymbolInfo Struct

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents an extension method symbol\.

```csharp
public readonly struct ExtensionMethodSymbolInfo : IEquatable<Roslynator.ExtensionMethodSymbolInfo>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ExtensionMethodSymbolInfo

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[ExtensionMethodSymbolInfo](./index.md)&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [IsReduced](IsReduced/index.md) | True if the symbol was reduced\. |
| [ReducedSymbol](ReducedSymbol/index.md) | The definition of extension method from which this symbol was reduced, or null, if the symbol was not reduced\. |
| [ReducedSymbolOrSymbol](ReducedSymbolOrSymbol/index.md) | The reduced symbol or the symbol if the reduced symbol is null\. |
| [Symbol](Symbol/index.md) | The extension method symbol\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(ExtensionMethodSymbolInfo)](Equals/index.md#2771632092) |  \(Implements [IEquatable&lt;ExtensionMethodSymbolInfo&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#3073297291) |  \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/index.md) |  \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(ExtensionMethodSymbolInfo, ExtensionMethodSymbolInfo)](op_Equality/index.md) | |
| [Inequality(ExtensionMethodSymbolInfo, ExtensionMethodSymbolInfo)](op_Inequality/index.md) | |

