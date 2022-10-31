# [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1) Struct Extensions

[Home](../../../README.md)

| Extension Method | Summary |
| ---------------- | ------- |
| [Add(SyntaxList\<StatementSyntax\>, StatementSyntax, Boolean)](../../../Roslynator/CSharp/SyntaxExtensions/Add/README.md) | Creates a new list with the specified node added or inserted\. |
| [All\<TNode\>(SyntaxList\<TNode\>, Func\<TNode, Boolean\>)](../../../Roslynator/SyntaxExtensions/All/README.md#1644057626) | Returns true if all nodes in a list matches the predicate\. |
| [Any\<TNode\>(SyntaxList\<TNode\>, Func\<TNode, Boolean\>)](../../../Roslynator/SyntaxExtensions/Any/README.md#2324683886) | Returns true if any node in a list matches the predicate\. |
| [Contains\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](../../../Roslynator/CSharp/SyntaxExtensions/Contains/README.md#4292341455) | Returns true if a node of the specified kind is in the [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Contains\<TNode\>(SyntaxList\<TNode\>, TNode)](../../../Roslynator/SyntaxExtensions/Contains/README.md#2402474003) | Returns true if the specified node is in the [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [DescendantTrivia\<TNode\>(SyntaxList\<TNode\>, Func\<SyntaxNode, Boolean\>, Boolean)](../../../Roslynator/SyntaxExtensions/DescendantTrivia/README.md#2025931486) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia\<TNode\>(SyntaxList\<TNode\>, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean)](../../../Roslynator/SyntaxExtensions/DescendantTrivia/README.md#2885561996) | Get a list of all the trivia associated with the nodes in the list\. |
| [Find\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](../../../Roslynator/CSharp/SyntaxExtensions/Find/README.md#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [IsFirst\<TNode\>(SyntaxList\<TNode\>, TNode)](../../../Roslynator/SyntaxExtensions/IsFirst/README.md#1691317763) | Returns true if the specified node is a first node in the list\. |
| [IsLast(SyntaxList\<StatementSyntax\>, StatementSyntax, Boolean)](../../../Roslynator/CSharp/SyntaxExtensions/IsLast/README.md) | Returns true if the specified statement is a last statement in the list\. |
| [IsLast\<TNode\>(SyntaxList\<TNode\>, TNode)](../../../Roslynator/SyntaxExtensions/IsLast/README.md#554961423) | Returns true if the specified node is a last node in the list\. |
| [LastIndexOf\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](../../../Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [RemoveRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32)](../../../Roslynator/CSharp/SyntaxExtensions/RemoveRange/README.md#3807495140) | Creates a new list with elements in the specified range removed\. |
| [ReplaceAt\<TNode\>(SyntaxList\<TNode\>, Int32, TNode)](../../../Roslynator/SyntaxExtensions/ReplaceAt/README.md#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>)](../../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, TNode)](../../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |
| [TrimTrivia\<TNode\>(SyntaxList\<TNode\>)](../../../Roslynator/CSharp/SyntaxExtensions/TrimTrivia/README.md#92538413) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [WithTriviaFrom\<TNode\>(SyntaxList\<TNode\>, SyntaxNode)](../../../Roslynator/SyntaxExtensions/WithTriviaFrom/README.md#301376900) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |

