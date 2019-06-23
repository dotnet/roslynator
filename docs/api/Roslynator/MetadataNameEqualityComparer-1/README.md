# MetadataNameEqualityComparer\<TSymbol> Class

[Home](../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Provides equality comparison for **TSymbol** by comparing [ISymbol.MetadataName](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.metadataname),
metadata name of [ISymbol.ContainingType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.containingtype)\(s\) and metadata name of [ISymbol.ContainingNamespace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.containingnamespace)\(s\)\.

```csharp
public sealed class MetadataNameEqualityComparer<TSymbol> : System.Collections.Generic.EqualityComparer<TSymbol> where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [EqualityComparer\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1) &#x2192; MetadataNameEqualityComparer\<TSymbol>

### Implements

* [IEqualityComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.iequalitycomparer)
* [IEqualityComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)\<TSymbol>

## Properties

| Property | Summary |
| -------- | ------- |
| [Instance](Instance/README.md) | Get the instance of [MetadataNameEqualityComparer\<TSymbol>](./README.md) for the specified **TSymbol**\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Equals(TSymbol, TSymbol)](Equals/README.md) | When overridden in a derived class, determines whether two objects of type **TSymbol** are equal\. \(Overrides [EqualityComparer\<TSymbol>.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1.equals)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode(TSymbol)](GetHashCode/README.md) | Serves as a hash function for the specified symbol\. \(Overrides [EqualityComparer\<TSymbol>.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

