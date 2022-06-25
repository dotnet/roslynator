---
sidebar_label: StatementSyntax
---

# [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax) Class Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [IsEmbedded(StatementSyntax, Boolean, Boolean, Boolean)](../../../../Roslynator/CSharp/SyntaxExtensions/IsEmbedded/index.md) | Returns true if the specified statement is an embedded statement\. |
| [NextStatement(StatementSyntax)](../../../../Roslynator/CSharp/SyntaxExtensions/NextStatement/index.md) | Gets the next statement of the specified statement\. If the specified statement is not contained in the list, or if there is no next statement, then this method returns null\. |
| [PreviousStatement(StatementSyntax)](../../../../Roslynator/CSharp/SyntaxExtensions/PreviousStatement/index.md) | Gets the previous statement of the specified statement\. If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null\. |
| [TryGetContainingList(StatementSyntax, SyntaxList&lt;StatementSyntax&gt;)](../../../../Roslynator/CSharp/SyntaxExtensions/TryGetContainingList/index.md) | Gets a list the specified statement is contained in\. This method succeeds if the statement is in a block's statements or a switch section's statements\. |

