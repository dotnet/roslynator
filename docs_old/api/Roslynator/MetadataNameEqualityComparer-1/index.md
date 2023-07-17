---
sidebar_label: MetadataNameEqualityComparer<TSymbol>
---

# MetadataNameEqualityComparer&lt;TSymbol&gt; Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Provides equality comparison for **TSymbol** by comparing [ISymbol.MetadataName](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.metadataname),
metadata name of [ISymbol.ContainingType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.containingtype)\(s\) and metadata name of [ISymbol.ContainingNamespace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.containingnamespace)\(s\)\.

```csharp
public sealed class MetadataNameEqualityComparer<TSymbol> : System.Collections.Generic.EqualityComparer<TSymbol> where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [EqualityComparer&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1) &#x2192; MetadataNameEqualityComparer&lt;TSymbol&gt;

### Implements

* [IEqualityComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.iequalitycomparer)
* [IEqualityComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)&lt;TSymbol&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [Instance](Instance/index.md) | Get the instance of [MetadataNameEqualityComparer&lt;TSymbol&gt;](./index.md) for the specified **TSymbol**\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Equals(TSymbol, TSymbol)](Equals/index.md) | When overridden in a derived class, determines whether two objects of type **TSymbol** are equal\. \(Overrides [EqualityComparer&lt;TSymbol&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1.equals)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode(TSymbol)](GetHashCode/index.md) | Serves as a hash function for the specified symbol\. \(Overrides [EqualityComparer&lt;TSymbol&gt;.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

