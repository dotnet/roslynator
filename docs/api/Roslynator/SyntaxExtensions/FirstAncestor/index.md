<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/FirstAncestor/index.md
---
sidebar_label: FirstAncestor
---
========
# SyntaxExtensions\.FirstAncestor\<TNode\>\(SyntaxNode, Func\<TNode, Boolean\>, Boolean\) Method
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/FirstAncestor/README.md

# SyntaxExtensions\.FirstAncestor&lt;TNode&gt;\(SyntaxNode, Func&lt;TNode, Boolean&gt;, Boolean\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns the first node of type **TNode** that matches the predicate\.

```csharp
public static TNode FirstAncestor<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Func<TNode, bool> predicate = null, bool ascendOutOfTrivia = true) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/FirstAncestor/index.md
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;
========
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/FirstAncestor/README.md

**ascendOutOfTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

