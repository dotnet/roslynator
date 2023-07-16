---
sidebar_label: LastIndexOf
---

# UsingDirectiveListInfo\.LastIndexOf Method

**Containing Type**: [UsingDirectiveListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func&lt;UsingDirectiveSyntax, Boolean&gt;)](#3449962221) | Searches for an using directive that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(UsingDirectiveSyntax)](#646248764) | Searches for an using directive and returns zero\-based index of the last occurrence in the list\. |

<a id="3449962221"></a>

## LastIndexOf\(Func&lt;UsingDirectiveSyntax, Boolean&gt;\) 

  
Searches for an using directive that matches the predicate and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="646248764"></a>

## LastIndexOf\(UsingDirectiveSyntax\) 

  
Searches for an using directive and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax usingDirective)
```

### Parameters

**usingDirective** &ensp; [UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

