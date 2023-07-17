---
sidebar_label: XmlElementInfo
---

# XmlElementInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about a [XmlElementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.xmlelementsyntax) or [XmlEmptyElementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.xmlemptyelementsyntax)\.

```csharp
public readonly struct XmlElementInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; XmlElementInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [Element](Element/index.md) | The xml element\. |
| [IsEmptyElement](IsEmptyElement/index.md) | Determines whether the element is [SyntaxKind.XmlEmptyElement](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.xmlemptyelement)\. |
| [Kind](Kind/index.md) | Element kind\. |
| [LocalName](LocalName/index.md) | Local name of the element\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

