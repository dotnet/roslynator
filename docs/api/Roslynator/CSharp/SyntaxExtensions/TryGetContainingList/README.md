# SyntaxExtensions\.TryGetContainingList\(StatementSyntax, SyntaxList\<StatementSyntax>\) Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Gets a list the specified statement is contained in\.
This method succeeds if the statement is in a block's statements or a switch section's statements\.

```csharp
public static bool TryGetContainingList(this Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, out Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**statements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the statement is contained in the list; otherwise, false\.