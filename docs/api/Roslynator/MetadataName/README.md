# MetadataName Struct

[Home](../../README.md) &#x2022; [Constructors](#constructors) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Operators](#operators)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents fully qualified metadata name of a symbol\.

```csharp
public readonly struct MetadataName : IEquatable<Roslynator.MetadataName>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; MetadataName

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)\<[MetadataName](./README.md)\>

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [MetadataName(IEnumerable\<String\>, IEnumerable\<String\>, String)](-ctor/README.md#2683869985) | Initializes a new instance of [MetadataName](./README.md)\. |
| [MetadataName(IEnumerable\<String\>, String)](-ctor/README.md#2380643901) | Initializes a new instance of [MetadataName](./README.md)\. |
| [MetadataName(ImmutableArray\<String\>, ImmutableArray\<String\>, String)](-ctor/README.md#1108504764) | Initializes a new instance of [MetadataName](./README.md)\. |
| [MetadataName(ImmutableArray\<String\>, String)](-ctor/README.md#3069058406) | Initializes a new instance of [MetadataName](./README.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [ContainingNamespaces](ContainingNamespaces/README.md) | Gets metadata names of containing namespaces |
| [ContainingTypes](ContainingTypes/README.md) | Get metadata names of containing types\. |
| [IsDefault](IsDefault/README.md) | Determines whether this struct was initialized with an actual names\. |
| [Name](Name/README.md) | Get metadata name\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(MetadataName)](Equals/README.md#3467406204) | Indicates whether this instance and a specified [MetadataName](./README.md) are equal\. \(Implements [IEquatable\<MetadataName\>.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/README.md#185373986) | Indicates whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/README.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Parse(String)](Parse/README.md) | Converts the string representation of a fully qualified metadata name to its [MetadataName](./README.md) equivalent\. |
| [ToString()](ToString/README.md) | Returns the fully qualified metadata name\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |
| [TryParse(String, MetadataName)](TryParse/README.md) | Converts the string representation of a fully qualified metadata name to its [MetadataName](./README.md) equivalent\. A return value indicates whether the parsing succeeded\. |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(MetadataName, MetadataName)](op_Equality/README.md) | |
| [Inequality(MetadataName, MetadataName)](op_Inequality/README.md) | |

