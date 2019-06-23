# StringLiteralExpressionInfo Struct

[Home](../../../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp.Syntax](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Provides information about string literal expression\.

```csharp
public readonly struct StringLiteralExpressionInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; StringLiteralExpressionInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ContainsEscapeSequence](ContainsEscapeSequence/README.md) | True if the string literal expression contains escape sequence\. |
| [ContainsLinefeed](ContainsLinefeed/README.md) | True if the string literal contains linefeed\. |
| [Expression](Expression/README.md) | The string literal expression\. |
| [InnerText](InnerText/README.md) | The token text, not including leading ampersand, if any, and enclosing quotation marks\. |
| [IsRegular](IsRegular/README.md) | True if this instance is regular string literal expression\. |
| [IsVerbatim](IsVerbatim/README.md) | True if this instance is verbatim string literal expression\. |
| [Success](Success/README.md) | Determines whether this struct was initialized with an actual syntax\. |
| [Text](Text/README.md) | The token text\. |
| [Token](Token/README.md) | The token representing the string literal expression\. |
| [ValueText](ValueText/README.md) | The token value text\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

