# IfStatementCascadeInfo Struct

[Home](../../../README.md) &#x2022; [Constructors](#constructors) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Operators](#operators)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Summarizes information about [IfStatementCascade](../IfStatementCascade/README.md)\.

```csharp
public readonly struct IfStatementCascadeInfo : IEquatable<Roslynator.CSharp.IfStatementCascadeInfo>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; IfStatementCascadeInfo

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)\<[IfStatementCascadeInfo](./README.md)>

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [IfStatementCascadeInfo(IfStatementSyntax)](-ctor/README.md) | Initializes a new instance of [IfStatementCascadeInfo](./README.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/README.md) | Gets a number of 'if' statements plus optional 'else' clause at the end of a cascade\. |
| [EndsWithElse](EndsWithElse/README.md) | Determines whether the cascade ends with 'else' clause\. |
| [EndsWithIf](EndsWithIf/README.md) | Determines whether the cascade ends with 'if' statement\. |
| [IfStatement](IfStatement/README.md) | Gets the topmost 'if' statement\. |
| [IsSimpleIf](IsSimpleIf/README.md) | Determines whether the cascade consists of single 'if' statement\. |
| [IsSimpleIfElse](IsSimpleIfElse/README.md) | Determines whether the cascade consists of single if\-else\. |
| [Last](Last/README.md) | Gets a last 'if' or 'else' in a cascade\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(IfStatementCascadeInfo)](Equals/README.md#Roslynator_CSharp_IfStatementCascadeInfo_Equals_Roslynator_CSharp_IfStatementCascadeInfo_) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable\<IfStatementCascadeInfo>.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/README.md#Roslynator_CSharp_IfStatementCascadeInfo_Equals_System_Object_) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/README.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](ToString/README.md) | Returns the string representation of the underlying syntax, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(IfStatementCascadeInfo, IfStatementCascadeInfo)](op_Equality/README.md) | |
| [Inequality(IfStatementCascadeInfo, IfStatementCascadeInfo)](op_Inequality/README.md) | |

