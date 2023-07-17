---
sidebar_label: Add
---

# SyntaxExtensions\.Add\(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a new list with the specified node added or inserted\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> Add(this Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool ignoreLocalFunctions)
```

### Parameters

**statements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**ignoreLocalFunctions** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

Insert statement before local function statements at the end of the list\.

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

