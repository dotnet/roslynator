---
sidebar_label: LastIndexOf
---

# MemberDeclarationListInfo\.LastIndexOf Method

**Containing Type**: [MemberDeclarationListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func&lt;MemberDeclarationSyntax, Boolean&gt;)](#2832811949) | Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(MemberDeclarationSyntax)](#3105192583) | Searches for a member and returns zero\-based index of the last occurrence in the list\. |

<a id="2832811949"></a>

## LastIndexOf\(Func&lt;MemberDeclarationSyntax, Boolean&gt;\) 

  
Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="3105192583"></a>

## LastIndexOf\(MemberDeclarationSyntax\) 

  
Searches for a member and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

