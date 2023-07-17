---
sidebar_label: WithNavigationAnnotation
---

# WorkspaceSyntaxExtensions\.WithNavigationAnnotation\(SyntaxToken\) Method

**Containing Type**: [WorkspaceSyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
Adds navigation annotation to the specified token, creating a new token of the same type with the navigation annotation on it\.
Navigation annotation allows to mark a token that should be selected after the code action is applied\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithNavigationAnnotation(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

