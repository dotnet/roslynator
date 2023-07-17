---
sidebar_label: IsLast
---

# SyntaxExtensions\.IsLast\(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the specified statement is a last statement in the list\.

```csharp
public static bool IsLast(this Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool ignoreLocalFunctions)
```

### Parameters

**statements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**ignoreLocalFunctions** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

Ignore local function statements at the end of the list\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

