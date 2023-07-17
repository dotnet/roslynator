---
sidebar_label: Item[]
---

# UsingDirectiveListInfo\.Item\[Int32\] Indexer

**Containing Type**: [UsingDirectiveListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets the using directive at the specified index in the list\.

```csharp
public Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax this[int index] { get; }
```

### Parameters

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

The zero\-based index of the using directive to get\. 

### Property Value

[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

The using directive at the specified index in the list\.

### Implements

* [IReadOnlyList&lt;UsingDirectiveSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)
