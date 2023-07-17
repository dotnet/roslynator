---
sidebar_label: StringLiteralExpressionInfo
---

# StringLiteralExpressionInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about string literal expression\.

```csharp
public readonly struct StringLiteralExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; StringLiteralExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ContainsEscapeSequence](ContainsEscapeSequence/index.md) | True if the string literal expression contains escape sequence\. |
| [ContainsLinefeed](ContainsLinefeed/index.md) | True if the string literal contains linefeed\. |
| [Expression](Expression/index.md) | The string literal expression\. |
| [InnerText](InnerText/index.md) | The token text, not including leading ampersand, if any, and enclosing quotation marks\. |
| [IsRegular](IsRegular/index.md) | True if this instance is regular string literal expression\. |
| [IsVerbatim](IsVerbatim/index.md) | True if this instance is verbatim string literal expression\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |
| [Text](Text/index.md) | The token text\. |
| [Token](Token/index.md) | The token representing the string literal expression\. |
| [ValueText](ValueText/index.md) | The token value text\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

