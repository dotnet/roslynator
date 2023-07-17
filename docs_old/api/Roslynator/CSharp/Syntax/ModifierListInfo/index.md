---
sidebar_label: ModifierListInfo
---

# ModifierListInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about modifier list\.

```csharp
public readonly struct ModifierListInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; ModifierListInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ExplicitAccessibility](ExplicitAccessibility/index.md) | The explicit accessibility\. |
| [IsAbstract](IsAbstract/index.md) | True if the modifier list contains "abstract" modifier\. |
| [IsAsync](IsAsync/index.md) | True if the modifier list contains "async" modifier\. |
| [IsConst](IsConst/index.md) | True if the modifier list contains "const" modifier\. |
| [IsExtern](IsExtern/index.md) | True if the modifier list contains "extern" modifier\. |
| [IsIn](IsIn/index.md) | True if the modifier list contains "in" modifier\. |
| [IsNew](IsNew/index.md) | True if the modifier list contains "new" modifier\. |
| [IsOut](IsOut/index.md) | True if the modifier list contains "out" modifier\. |
| [IsOverride](IsOverride/index.md) | True if the modifier list contains "override" modifier\. |
| [IsParams](IsParams/index.md) | True if the modifier list contains "params" modifier\. |
| [IsPartial](IsPartial/index.md) | True if the modifier list contains "partial" modifier\. |
| [IsReadOnly](IsReadOnly/index.md) | True if the modifier list contains "readonly" modifier\. |
| [IsRef](IsRef/index.md) | True if the modifier list contains "ref" modifier\. |
| [IsSealed](IsSealed/index.md) | True if the modifier list contains "sealed" modifier\. |
| [IsStatic](IsStatic/index.md) | True if the modifier list contains "static" modifier\. |
| [IsUnsafe](IsUnsafe/index.md) | True if the modifier list contains "unsafe" modifier\. |
| [IsVirtual](IsVirtual/index.md) | True if the modifier list contains "virtual" modifier\. |
| [IsVolatile](IsVolatile/index.md) | True if the modifier list contains "volatile" modifier\. |
| [Modifiers](Modifiers/index.md) | The modifier list\. |
| [Parent](Parent/index.md) | The node that contains the modifiers\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetFilter()](GetFilter/index.md) | Gets the modifier filter\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithExplicitAccessibility(Accessibility, IComparer&lt;SyntaxKind&gt;)](WithExplicitAccessibility/index.md) | Creates a new [ModifierListInfo](./index.md) with accessibility modifiers updated\. |
| [WithModifiers(SyntaxTokenList)](WithModifiers/index.md) | Creates a new [ModifierListInfo](./index.md) with the specified modifiers updated\. |
| [WithoutExplicitAccessibility()](WithoutExplicitAccessibility/index.md) | Creates a new [ModifierListInfo](./index.md) with accessibility modifiers removed\. |

