---
sidebar_label: IndexOf
---

# StatementListInfo\.IndexOf Method

**Containing Type**: [StatementListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func&lt;StatementSyntax, Boolean&gt;)](#2005140688) | Searches for a statement that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(StatementSyntax)](#2921438069) | The index of the statement in the list\. |

<a id="2005140688"></a>

## IndexOf\(Func&lt;StatementSyntax, Boolean&gt;\) 

  
Searches for a statement that matches the predicate and returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="2921438069"></a>

## IndexOf\(StatementSyntax\) 

  
The index of the statement in the list\.

```csharp
public int IndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

