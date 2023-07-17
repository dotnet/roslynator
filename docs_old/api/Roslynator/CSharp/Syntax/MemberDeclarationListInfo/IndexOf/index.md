---
sidebar_label: IndexOf
---

# MemberDeclarationListInfo\.IndexOf Method

**Containing Type**: [MemberDeclarationListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func&lt;MemberDeclarationSyntax, Boolean&gt;)](#442472242) | Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(MemberDeclarationSyntax)](#3381813943) | The index of the member in the list\. |

<a id="442472242"></a>

## IndexOf\(Func&lt;MemberDeclarationSyntax, Boolean&gt;\) 

  
Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="3381813943"></a>

## IndexOf\(MemberDeclarationSyntax\) 

  
The index of the member in the list\.

```csharp
public int IndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

