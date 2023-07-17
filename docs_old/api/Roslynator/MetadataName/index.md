---
sidebar_label: MetadataName
---

# MetadataName Struct

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents fully qualified metadata name of a symbol\.

```csharp
public readonly struct MetadataName : IEquatable<Roslynator.MetadataName>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; MetadataName

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[MetadataName](./index.md)&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [MetadataName(IEnumerable&lt;String&gt;, IEnumerable&lt;String&gt;, String)](-ctor/index.md#2683869985) | Initializes a new instance of [MetadataName](./index.md)\. |
| [MetadataName(IEnumerable&lt;String&gt;, String)](-ctor/index.md#2380643901) | Initializes a new instance of [MetadataName](./index.md)\. |
| [MetadataName(ImmutableArray&lt;String&gt;, ImmutableArray&lt;String&gt;, String)](-ctor/index.md#1108504764) | Initializes a new instance of [MetadataName](./index.md)\. |
| [MetadataName(ImmutableArray&lt;String&gt;, String)](-ctor/index.md#3069058406) | Initializes a new instance of [MetadataName](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [ContainingNamespaces](ContainingNamespaces/index.md) | Gets metadata names of containing namespaces |
| [ContainingTypes](ContainingTypes/index.md) | Get metadata names of containing types\. |
| [IsDefault](IsDefault/index.md) | Determines whether this struct was initialized with an actual names\. |
| [Name](Name/index.md) | Get metadata name\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(MetadataName)](Equals/index.md#3467406204) | Indicates whether this instance and a specified [MetadataName](./index.md) are equal\. \(Implements [IEquatable&lt;MetadataName&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#185373986) | Indicates whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/index.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Parse(String)](Parse/index.md) | Converts the string representation of a fully qualified metadata name to its [MetadataName](./index.md) equivalent\. |
| [ToString()](ToString/index.md) | Returns the fully qualified metadata name\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |
| [TryParse(String, MetadataName)](TryParse/index.md) | Converts the string representation of a fully qualified metadata name to its [MetadataName](./index.md) equivalent\. A return value indicates whether the parsing succeeded\. |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(MetadataName, MetadataName)](op_Equality/index.md) | |
| [Inequality(MetadataName, MetadataName)](op_Inequality/index.md) | |

