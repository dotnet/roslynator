# ModifierList\.RemoveAll Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveAll\<TNode>(TNode)](#Roslynator_CSharp_ModifierList_RemoveAll__1___0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll\<TNode>(TNode, Func\<SyntaxToken, Boolean>)](#Roslynator_CSharp_ModifierList_RemoveAll__1___0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |

## RemoveAll\<TNode>\(TNode\) <a id="Roslynator_CSharp_ModifierList_RemoveAll__1___0_"></a>

\
Creates a new node with all modifiers removed\.

```csharp
public static TNode RemoveAll<TNode>(TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

## RemoveAll\<TNode>\(TNode, Func\<SyntaxToken, Boolean>\) <a id="Roslynator_CSharp_ModifierList_RemoveAll__1___0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__"></a>

\
Creates a new node with modifiers that matches the predicate removed\.

```csharp
public static TNode RemoveAll<TNode>(TNode node, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)>

### Returns

TNode

