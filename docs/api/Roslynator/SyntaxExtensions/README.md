# SyntaxExtensions Class

[Home](../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension method for a syntax\.

```csharp
public static class SyntaxExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [All(SyntaxTokenList, Func\<SyntaxToken, Boolean\>)](All/README.md#3911797928) | Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [All(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean\>)](All/README.md#1935784235) | Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [All\<TNode\>(SeparatedSyntaxList\<TNode\>, Func\<TNode, Boolean\>)](All/README.md#1104261355) | Returns true if all nodes in a list matches the predicate\. |
| [All\<TNode\>(SyntaxList\<TNode\>, Func\<TNode, Boolean\>)](All/README.md#1644057626) | Returns true if all nodes in a list matches the predicate\. |
| [Any(SyntaxTokenList, Func\<SyntaxToken, Boolean\>)](Any/README.md#3052208275) | Returns true if any token in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [Any(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean\>)](Any/README.md#1292593986) | Returns true if any trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [Any\<TNode\>(SeparatedSyntaxList\<TNode\>, Func\<TNode, Boolean\>)](Any/README.md#3469033055) | Returns true if any node in a list matches the predicate\. |
| [Any\<TNode\>(SyntaxList\<TNode\>, Func\<TNode, Boolean\>)](Any/README.md#2324683886) | Returns true if any node in a list matches the predicate\. |
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](AppendToLeadingTrivia/README.md#2690812841) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](AppendToLeadingTrivia/README.md#2537170499) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](AppendToLeadingTrivia/README.md#3159857401) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode\>(TNode, SyntaxTrivia)](AppendToLeadingTrivia/README.md#161505572) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](AppendToTrailingTrivia/README.md#682978820) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](AppendToTrailingTrivia/README.md#2809312657) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](AppendToTrailingTrivia/README.md#782693212) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, SyntaxTrivia)](AppendToTrailingTrivia/README.md#3044430369) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [Contains(SyntaxTokenList, SyntaxToken)](Contains/README.md#4268781350) | Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains\<TNode\>(SeparatedSyntaxList\<TNode\>, TNode)](Contains/README.md#1049481281) | Returns true if the specified node is in the [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains\<TNode\>(SyntaxList\<TNode\>, TNode)](Contains/README.md#2402474003) | Returns true if the specified node is in the [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [ContainsDirectives(SyntaxNode, TextSpan)](ContainsDirectives/README.md) | Returns true if the node contains any preprocessor directives inside the specified span\. |
| [DescendantTrivia\<TNode\>(SyntaxList\<TNode\>, Func\<SyntaxNode, Boolean\>, Boolean)](DescendantTrivia/README.md#2025931486) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia\<TNode\>(SyntaxList\<TNode\>, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean)](DescendantTrivia/README.md#2885561996) | Get a list of all the trivia associated with the nodes in the list\. |
| [FirstAncestor\<TNode\>(SyntaxNode, Func\<TNode, Boolean\>, Boolean)](FirstAncestor/README.md) | Returns the first node of type **TNode** that matches the predicate\. |
| [FirstDescendant\<TNode\>(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean)](FirstDescendant/README.md#3727489774) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendant\<TNode\>(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean)](FirstDescendant/README.md#2271502195) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf\<TNode\>(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean)](FirstDescendantOrSelf/README.md#4205056015) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf\<TNode\>(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean)](FirstDescendantOrSelf/README.md#3421526450) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [GetLeadingAndTrailingTrivia(SyntaxNode)](GetLeadingAndTrailingTrivia/README.md) | Returns leading and trailing trivia of the specified node in a single list\. |
| [GetTrailingSeparator\<TNode\>(SeparatedSyntaxList\<TNode\>)](GetTrailingSeparator/README.md) | Returns the trailing separator, if any\. |
| [HasTrailingSeparator\<TNode\>(SeparatedSyntaxList\<TNode\>)](HasTrailingSeparator/README.md) | Returns true if the specified list contains trailing separator\. |
| [IndexOf(SyntaxTokenList, Func\<SyntaxToken, Boolean\>)](IndexOf/README.md#3314040654) | Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [IndexOf(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean\>)](IndexOf/README.md#2746233850) | Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [IsFirst\<TNode\>(SeparatedSyntaxList\<TNode\>, TNode)](IsFirst/README.md#1292391442) | Returns true if the specified node is a first node in the list\. |
| [IsFirst\<TNode\>(SyntaxList\<TNode\>, TNode)](IsFirst/README.md#1691317763) | Returns true if the specified node is a first node in the list\. |
| [IsLast\<TNode\>(SeparatedSyntaxList\<TNode\>, TNode)](IsLast/README.md#3058017669) | Returns true if the specified node is a last node in the list\. |
| [IsLast\<TNode\>(SyntaxList\<TNode\>, TNode)](IsLast/README.md#554961423) | Returns true if the specified node is a last node in the list\. |
| [LeadingAndTrailingTrivia(SyntaxToken)](LeadingAndTrailingTrivia/README.md) | Returns leading and trailing trivia of the specified node in a single list\. |
| [PrependToLeadingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](PrependToLeadingTrivia/README.md#640281292) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia(SyntaxToken, SyntaxTrivia)](PrependToLeadingTrivia/README.md#2404824921) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](PrependToLeadingTrivia/README.md#3915245611) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia\<TNode\>(TNode, SyntaxTrivia)](PrependToLeadingTrivia/README.md#3276186521) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](PrependToTrailingTrivia/README.md#3817969325) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](PrependToTrailingTrivia/README.md#1356374860) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](PrependToTrailingTrivia/README.md#1111873538) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode\>(TNode, SyntaxTrivia)](PrependToTrailingTrivia/README.md#3683468027) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](ReplaceAt/README.md#2421566925) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](ReplaceAt/README.md#3526169581) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, TNode)](ReplaceAt/README.md#3499086875) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt\<TNode\>(SyntaxList\<TNode\>, Int32, TNode)](ReplaceAt/README.md#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |
| [SpanContainsDirectives(SyntaxNode)](SpanContainsDirectives/README.md) | Returns true if the node's span contains any preprocessor directives\. |
| [TryGetContainingList(SyntaxTrivia, SyntaxTriviaList, Boolean, Boolean)](TryGetContainingList/README.md) | Gets a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) the specified trivia is contained in\. |
| [WithoutLeadingTrivia(SyntaxNodeOrToken)](WithoutLeadingTrivia/README.md#3431085438) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\. |
| [WithoutLeadingTrivia(SyntaxToken)](WithoutLeadingTrivia/README.md#43937718) | Creates a new token from this token with the leading trivia removed\. |
| [WithoutTrailingTrivia(SyntaxNodeOrToken)](WithoutTrailingTrivia/README.md#3602009992) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\. |
| [WithoutTrailingTrivia(SyntaxToken)](WithoutTrailingTrivia/README.md#3451371873) | Creates a new token from this token with the trailing trivia removed\. |
| [WithoutTrivia(SyntaxNodeOrToken)](WithoutTrivia/README.md) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) from this node without leading and trailing trivia\. |
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](WithTriviaFrom/README.md#3436644061) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxNode)](WithTriviaFrom/README.md#2087578213) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode\>(SyntaxList\<TNode\>, SyntaxNode)](WithTriviaFrom/README.md#301376900) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode\>(TNode, SyntaxToken)](WithTriviaFrom/README.md#441639473) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |

