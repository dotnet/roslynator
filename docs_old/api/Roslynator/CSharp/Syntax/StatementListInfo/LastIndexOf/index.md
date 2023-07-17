---
sidebar_label: LastIndexOf
---

# StatementListInfo\.LastIndexOf Method

**Containing Type**: [StatementListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func&lt;StatementSyntax, Boolean&gt;)](#3886378507) | Searches for a statement that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(StatementSyntax)](#1794847222) | Searches for a statement and returns zero\-based index of the last occurrence in the list\. |

<a id="3886378507"></a>

## LastIndexOf\(Func&lt;StatementSyntax, Boolean&gt;\) 

  
Searches for a statement that matches the predicate and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="1794847222"></a>

## LastIndexOf\(StatementSyntax\) 

  
Searches for a statement and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

