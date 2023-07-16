---
sidebar_label: ExpressionChain
---

# ExpressionChain Struct

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Enables to enumerate expressions of a binary expression and expressions of nested binary expressions of the same kind as parent binary expression\.

```csharp
public readonly struct ExpressionChain : IEquatable<Roslynator.CSharp.ExpressionChain>,
    System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ExpressionChain

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)&gt;
* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[ExpressionChain](./index.md)&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [BinaryExpression](BinaryExpression/index.md) | The binary expression\. |
| [Span](Span/index.md) | The span that represents selected expressions\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(ExpressionChain)](Equals/index.md#3497933564) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable&lt;ExpressionChain&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#3026618345) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetEnumerator()](GetEnumerator/index.md) | Gets the enumerator for the expressions\. |
| [GetHashCode()](GetHashCode/index.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Reverse()](Reverse/index.md) | Returns a chain which contains all expressions of [ExpressionChain](./index.md) in reversed order\. |
| [ToString()](ToString/index.md) | Returns the string representation of the expressions, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(ExpressionChain, ExpressionChain)](op_Equality/index.md) | |
| [Inequality(ExpressionChain, ExpressionChain)](op_Inequality/index.md) | |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;ExpressionSyntax&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-ExpressionSyntax--GetEnumerator/index.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/index.md) | |
| [Reversed](Reversed/index.md) | Enables to enumerate expressions of [ExpressionChain](./index.md) in a reversed order\. |

