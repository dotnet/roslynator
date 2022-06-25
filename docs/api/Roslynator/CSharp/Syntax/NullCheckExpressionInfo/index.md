---
sidebar_label: NullCheckExpressionInfo
---

# NullCheckExpressionInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about a null check expression\.

```csharp
public readonly struct NullCheckExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; NullCheckExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [Expression](Expression/index.md) | The expression that is evaluated whether is \(not\) null\. for example "x" in "x == null"\. |
| [IsCheckingNotNull](IsCheckingNotNull/index.md) | Determines whether this null check is checking if the expression is not null\. |
| [IsCheckingNull](IsCheckingNull/index.md) | Determines whether this null check is checking if the expression is null\. |
| [NullCheckExpression](NullCheckExpression/index.md) | The null check expression, e\.g\. "x == null"\. |
| [Style](Style/index.md) | The style of this null check\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

