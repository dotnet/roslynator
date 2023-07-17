---
sidebar_label: ConditionalExpressionInfo
---

# ConditionalExpressionInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about conditional expression\.

```csharp
public readonly struct ConditionalExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ConditionalExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ColonToken](ColonToken/index.md) | The token representing the colon\. |
| [Condition](Condition/index.md) | The condition expression\. |
| [ConditionalExpression](ConditionalExpression/index.md) | The conditional expression\. |
| [QuestionToken](QuestionToken/index.md) | The token representing the question mark\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |
| [WhenFalse](WhenFalse/index.md) | The expression to be executed when the expression is false\. |
| [WhenTrue](WhenTrue/index.md) | The expression to be executed when the expression is true\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

