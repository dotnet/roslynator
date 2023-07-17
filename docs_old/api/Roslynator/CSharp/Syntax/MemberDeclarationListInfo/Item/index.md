---
sidebar_label: Item[]
---

# MemberDeclarationListInfo\.Item\[Int32\] Indexer

**Containing Type**: [MemberDeclarationListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets the member at the specified index in the list\.

```csharp
public Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax this[int index] { get; }
```

### Parameters

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

The zero\-based index of the member to get\. 

### Property Value

[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

The member at the specified index in the list\.

### Implements

* [IReadOnlyList&lt;MemberDeclarationSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)
