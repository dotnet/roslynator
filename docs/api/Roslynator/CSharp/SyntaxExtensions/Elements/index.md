---
sidebar_label: Elements
---

# SyntaxExtensions\.Elements\(DocumentationCommentTriviaSyntax, String\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets a list of xml elements with the specified local name\.

```csharp
public static System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax> Elements(this Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax documentationComment, string localName)
```

### Parameters

**documentationComment** &ensp; [DocumentationCommentTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.documentationcommenttriviasyntax)

**localName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[XmlElementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.xmlelementsyntax)&gt;

