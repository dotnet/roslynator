---
sidebar_label: ExpressionChain.Reversed
---

# ExpressionChain\.Reversed Struct

**Namespace**: [Roslynator.CSharp](../../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Enables to enumerate expressions of [ExpressionChain](../index.md) in a reversed order\.

```csharp
public readonly struct ExpressionChain.Reversed : IEquatable<Roslynator.CSharp.ExpressionChain.Reversed>,
    System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ExpressionChain\.Reversed

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)&gt;
* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[ExpressionChain.Reversed](./index.md)&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [Reversed(ExpressionChain)](-ctor/index.md) | |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](Equals/index.md#4203233237) |  \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [Equals(Reversed)](Equals/index.md#2039282915) |  \(Implements [IEquatable&lt;Reversed&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [GetEnumerator()](GetEnumerator/index.md) | |
| [GetHashCode()](GetHashCode/index.md) |  \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](ToString/index.md) |  \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(Reversed, Reversed)](op_Equality/index.md) | |
| [Inequality(Reversed, Reversed)](op_Inequality/index.md) | |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;ExpressionSyntax&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-ExpressionSyntax--GetEnumerator/index.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/index.md) | |

