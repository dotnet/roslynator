---
sidebar_label: IsEmbedded
---

# SyntaxExtensions\.IsEmbedded\(StatementSyntax, Boolean, Boolean, Boolean\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the specified statement is an embedded statement\.

```csharp
public static bool IsEmbedded(this Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool canBeBlock = false, bool canBeIfInsideElse = true, bool canBeUsingInsideUsing = true)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**canBeBlock** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

Block can be considered as embedded statement

**canBeIfInsideElse** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If statement that is a child of an else statement can be considered as an embedded statement\.

**canBeUsingInsideUsing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

Using statement that is a child of an using statement can be considered as en embedded statement\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

