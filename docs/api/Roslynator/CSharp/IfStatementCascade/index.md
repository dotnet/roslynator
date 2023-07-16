---
sidebar_label: IfStatementCascade
---

# IfStatementCascade Struct

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Enables to enumerate if statement cascade\.

```csharp
public readonly struct IfStatementCascade : IEquatable<Roslynator.CSharp.IfStatementCascade>,
    System.Collections.Generic.IEnumerable<Roslynator.CSharp.IfStatementOrElseClause>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; IfStatementCascade

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[IfStatementOrElseClause](../IfStatementOrElseClause/index.md)&gt;
* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[IfStatementCascade](./index.md)&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [IfStatement](IfStatement/index.md) | The if statement\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(IfStatementCascade)](Equals/index.md#3739133020) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable&lt;IfStatementCascade&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#3247102308) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetEnumerator()](GetEnumerator/index.md) | Gets the enumerator for the if\-else cascade\. |
| [GetHashCode()](GetHashCode/index.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](ToString/index.md) | Returns the string representation of the underlying syntax, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(IfStatementCascade, IfStatementCascade)](op_Equality/index.md) | |
| [Inequality(IfStatementCascade, IfStatementCascade)](op_Inequality/index.md) | |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;IfStatementOrElseClause&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Roslynator-CSharp-IfStatementOrElseClause--GetEnumerator/index.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/index.md) | |

