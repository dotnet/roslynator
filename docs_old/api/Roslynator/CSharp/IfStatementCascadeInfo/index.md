---
sidebar_label: IfStatementCascadeInfo
---

# IfStatementCascadeInfo Struct

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Summarizes information about [IfStatementCascade](../IfStatementCascade/index.md)\.

```csharp
public readonly struct IfStatementCascadeInfo : IEquatable<Roslynator.CSharp.IfStatementCascadeInfo>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; IfStatementCascadeInfo

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[IfStatementCascadeInfo](./index.md)&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [IfStatementCascadeInfo(IfStatementSyntax)](-ctor/index.md) | Initializes a new instance of [IfStatementCascadeInfo](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/index.md) | Gets a number of 'if' statements plus optional 'else' clause at the end of a cascade\. |
| [EndsWithElse](EndsWithElse/index.md) | Determines whether the cascade ends with 'else' clause\. |
| [EndsWithIf](EndsWithIf/index.md) | Determines whether the cascade ends with 'if' statement\. |
| [IfStatement](IfStatement/index.md) | Gets the topmost 'if' statement\. |
| [IsSimpleIf](IsSimpleIf/index.md) | Determines whether the cascade consists of single 'if' statement\. |
| [IsSimpleIfElse](IsSimpleIfElse/index.md) | Determines whether the cascade consists of single if\-else\. |
| [Last](Last/index.md) | Gets a last 'if' or 'else' in a cascade\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(IfStatementCascadeInfo)](Equals/index.md#1103005347) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable&lt;IfStatementCascadeInfo&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#454128297) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/index.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](ToString/index.md) | Returns the string representation of the underlying syntax, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(IfStatementCascadeInfo, IfStatementCascadeInfo)](op_Equality/index.md) | |
| [Inequality(IfStatementCascadeInfo, IfStatementCascadeInfo)](op_Inequality/index.md) | |

