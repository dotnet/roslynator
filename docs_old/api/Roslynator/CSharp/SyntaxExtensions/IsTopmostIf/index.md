---
sidebar_label: IsTopmostIf
---

# SyntaxExtensions\.IsTopmostIf\(IfStatementSyntax\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the specified if statement is not a child of an else clause\.

```csharp
public static bool IsTopmostIf(this Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

