---
sidebar_label: ISelection<T>
---

# ISelection&lt;T&gt; Interface

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents consecutive sequence of selected items in a collection\.

```csharp
public interface ISelection<T> : System.Collections.Generic.IReadOnlyList<T>
```

### Type Parameters

**T**

### Derived

* [SeparatedSyntaxListSelection&lt;TNode&gt;](../SeparatedSyntaxListSelection-1/index.md)
* [SyntaxListSelection&lt;TNode&gt;](../SyntaxListSelection-1/index.md)
* [TextLineCollectionSelection](../Text/TextLineCollectionSelection/index.md)

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;T&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;T&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;T&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [FirstIndex](FirstIndex/index.md) | Gets an index of the first selected item\. |
| [LastIndex](LastIndex/index.md) | Gets an index of the last selected item\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [First()](First/index.md) | Gets the first selected item\. |
| [Last()](Last/index.md) | Gets the last selected item\. |

