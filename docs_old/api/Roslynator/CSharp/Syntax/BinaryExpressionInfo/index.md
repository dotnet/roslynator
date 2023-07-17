---
sidebar_label: BinaryExpressionInfo
---

# BinaryExpressionInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about binary expression\.

```csharp
public readonly struct BinaryExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; BinaryExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [BinaryExpression](BinaryExpression/index.md) | The binary expression\. |
| [Kind](Kind/index.md) | The kind of the binary expression\. |
| [Left](Left/index.md) | The expression on the left of the binary operator\. |
| [OperatorToken](OperatorToken/index.md) | The operator of the binary expression\. |
| [Right](Right/index.md) | The expression on the right of the binary operator\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [AsChain()](AsChain/index.md) | Returns [ExpressionChain](../../ExpressionChain/index.md) that enables to enumerate expressions of a binary expression\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

