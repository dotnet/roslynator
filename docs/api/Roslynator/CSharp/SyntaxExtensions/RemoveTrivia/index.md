<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/RemoveTrivia/index.md
---
sidebar_label: RemoveTrivia
---
========
# SyntaxExtensions\.RemoveTrivia\<TNode\>\(TNode, TextSpan?\) Method
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/RemoveTrivia/README.md

# SyntaxExtensions\.RemoveTrivia&lt;TNode&gt;\(TNode, TextSpan?\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a new node with the trivia removed\.

```csharp
public static TNode RemoveTrivia<TNode>(this TNode node, Microsoft.CodeAnalysis.Text.TextSpan? span = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)?

### Returns

TNode

