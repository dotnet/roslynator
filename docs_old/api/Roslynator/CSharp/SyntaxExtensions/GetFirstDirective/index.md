---
sidebar_label: GetFirstDirective
---

# SyntaxExtensions\.GetFirstDirective\(SyntaxNode, TextSpan, Func&lt;DirectiveTriviaSyntax, Boolean&gt;\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets the first directive of the tree rooted by this node\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax GetFirstDirective(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span, Func<Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax, bool> predicate = null)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[DirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.directivetriviasyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[DirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.directivetriviasyntax)

