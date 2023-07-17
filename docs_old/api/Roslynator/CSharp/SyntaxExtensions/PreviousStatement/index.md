---
sidebar_label: PreviousStatement
---

# SyntaxExtensions\.PreviousStatement\(StatementSyntax\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets the previous statement of the specified statement\.
If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax PreviousStatement(this Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

