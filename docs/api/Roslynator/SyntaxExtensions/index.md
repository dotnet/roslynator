---
sidebar_label: SyntaxExtensions
---

# SyntaxExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension method for a syntax\.

```csharp
public static class SyntaxExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [All(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](All/index.md#3911797928) | Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [All(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](All/index.md#1935784235) | Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [All&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](All/index.md#1104261355) | Returns true if all nodes in a list matches the predicate\. |
| [All&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](All/index.md#1644057626) | Returns true if all nodes in a list matches the predicate\. |
| [Any(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](Any/index.md#3052208275) | Returns true if any token in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [Any(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](Any/index.md#1292593986) | Returns true if any trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [Any&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](Any/index.md#3469033055) | Returns true if any node in a list matches the predicate\. |
| [Any&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](Any/index.md#2324683886) | Returns true if any node in a list matches the predicate\. |
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](AppendToLeadingTrivia/index.md#2690812841) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](AppendToLeadingTrivia/index.md#2537170499) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](AppendToLeadingTrivia/index.md#3159857401) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](AppendToLeadingTrivia/index.md#161505572) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](AppendToTrailingTrivia/index.md#682978820) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](AppendToTrailingTrivia/index.md#2809312657) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](AppendToTrailingTrivia/index.md#782693212) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](AppendToTrailingTrivia/index.md#3044430369) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [Contains(SyntaxTokenList, SyntaxToken)](Contains/index.md#4268781350) | Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](Contains/index.md#1049481281) | Returns true if the specified node is in the [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](Contains/index.md#2402474003) | Returns true if the specified node is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [ContainsDirectives(SyntaxNode, TextSpan)](ContainsDirectives/index.md) | Returns true if the node contains any preprocessor directives inside the specified span\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](DescendantTrivia/index.md#2025931486) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](DescendantTrivia/index.md#2885561996) | Get a list of all the trivia associated with the nodes in the list\. |
| [FirstAncestor&lt;TNode&gt;(SyntaxNode, Func&lt;TNode, Boolean&gt;, Boolean)](FirstAncestor/index.md) | Returns the first node of type **TNode** that matches the predicate\. |
| [FirstDescendant&lt;TNode&gt;(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstDescendant/index.md#3727489774) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendant&lt;TNode&gt;(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstDescendant/index.md#2271502195) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf&lt;TNode&gt;(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstDescendantOrSelf/index.md#4205056015) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf&lt;TNode&gt;(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstDescendantOrSelf/index.md#3421526450) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [GetLeadingAndTrailingTrivia(SyntaxNode)](GetLeadingAndTrailingTrivia/index.md) | Returns leading and trailing trivia of the specified node in a single list\. |
| [GetTrailingSeparator&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](GetTrailingSeparator/index.md) | Returns the trailing separator, if any\. |
| [HasTrailingSeparator&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](HasTrailingSeparator/index.md) | Returns true if the specified list contains trailing separator\. |
| [IndexOf(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](IndexOf/index.md#3314040654) | Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [IndexOf(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](IndexOf/index.md#2746233850) | Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [IsFirst&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](IsFirst/index.md#1292391442) | Returns true if the specified node is a first node in the list\. |
| [IsFirst&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](IsFirst/index.md#1691317763) | Returns true if the specified node is a first node in the list\. |
| [IsLast&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](IsLast/index.md#3058017669) | Returns true if the specified node is a last node in the list\. |
| [IsLast&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](IsLast/index.md#554961423) | Returns true if the specified node is a last node in the list\. |
| [LeadingAndTrailingTrivia(SyntaxToken)](LeadingAndTrailingTrivia/index.md) | Returns leading and trailing trivia of the specified node in a single list\. |
| [PrependToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](PrependToLeadingTrivia/index.md#640281292) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia(SyntaxToken, SyntaxTrivia)](PrependToLeadingTrivia/index.md#2404824921) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](PrependToLeadingTrivia/index.md#3915245611) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](PrependToLeadingTrivia/index.md#3276186521) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](PrependToTrailingTrivia/index.md#3817969325) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](PrependToTrailingTrivia/index.md#1356374860) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](PrependToTrailingTrivia/index.md#1111873538) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](PrependToTrailingTrivia/index.md#3683468027) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](ReplaceAt/index.md#2421566925) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](ReplaceAt/index.md#3526169581) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode)](ReplaceAt/index.md#3499086875) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, TNode)](ReplaceAt/index.md#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |
| [SpanContainsDirectives(SyntaxNode)](SpanContainsDirectives/index.md) | Returns true if the node's span contains any preprocessor directives\. |
| [TryGetContainingList(SyntaxTrivia, SyntaxTriviaList, Boolean, Boolean)](TryGetContainingList/index.md) | Gets a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) the specified trivia is contained in\. |
| [WithoutLeadingTrivia(SyntaxNodeOrToken)](WithoutLeadingTrivia/index.md#3431085438) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\. |
| [WithoutLeadingTrivia(SyntaxToken)](WithoutLeadingTrivia/index.md#43937718) | Creates a new token from this token with the leading trivia removed\. |
| [WithoutTrailingTrivia(SyntaxNodeOrToken)](WithoutTrailingTrivia/index.md#3602009992) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\. |
| [WithoutTrailingTrivia(SyntaxToken)](WithoutTrailingTrivia/index.md#3451371873) | Creates a new token from this token with the trailing trivia removed\. |
| [WithoutTrivia(SyntaxNodeOrToken)](WithoutTrivia/index.md) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) from this node without leading and trailing trivia\. |
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](WithTriviaFrom/index.md#3436644061) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxNode)](WithTriviaFrom/index.md#2087578213) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxNode)](WithTriviaFrom/index.md#301376900) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom&lt;TNode&gt;(TNode, SyntaxToken)](WithTriviaFrom/index.md#441639473) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |

