# ConditionalExpressionInfo Struct

[Home](../../../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp.Syntax](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Provides information about conditional expression\.

```csharp
public readonly struct ConditionalExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ConditionalExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ColonToken](ColonToken/README.md) | The token representing the colon\. |
| [Condition](Condition/README.md) | The condition expression\. |
| [ConditionalExpression](ConditionalExpression/README.md) | The conditional expression\. |
| [QuestionToken](QuestionToken/README.md) | The token representing the question mark\. |
| [Success](Success/README.md) | Determines whether this struct was initialized with an actual syntax\. |
| [WhenFalse](WhenFalse/README.md) | The expression to be executed when the expression is false\. |
| [WhenTrue](WhenTrue/README.md) | The expression to be executed when the expression is true\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

