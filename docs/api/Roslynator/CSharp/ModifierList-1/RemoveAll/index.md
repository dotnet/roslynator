---
sidebar_label: RemoveAll
---

# ModifierList\.RemoveAll Method

**Containing Type**: [ModifierList&lt;TNode&gt;](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveAll(TNode)](#Roslynator_CSharp_ModifierList_1_RemoveAll__0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll(TNode, Func&lt;SyntaxToken, Boolean&gt;)](#Roslynator_CSharp_ModifierList_1_RemoveAll__0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |

## RemoveAll\(TNode\) <a id="Roslynator_CSharp_ModifierList_1_RemoveAll__0_"></a>

  
Creates a new node with all modifiers removed\.

```csharp
public TNode RemoveAll(TNode node)
```

### Parameters

**node** &ensp; TNode

### Returns

TNode

## RemoveAll\(TNode, Func&lt;SyntaxToken, Boolean&gt;\) <a id="Roslynator_CSharp_ModifierList_1_RemoveAll__0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__"></a>

  
Creates a new node with modifiers that matches the predicate removed\.

```csharp
public TNode RemoveAll(TNode node, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**node** &ensp; TNode

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

TNode

