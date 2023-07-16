---
sidebar_label: SyntaxList<TNode>
---

# [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1) Struct Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [Add(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](../../Roslynator/CSharp/SyntaxExtensions/Add/index.md) | Creates a new list with the specified node added or inserted\. |
| [All&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/All/index.md#1644057626) | Returns true if all nodes in a list matches the predicate\. |
| [Any&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/Any/index.md#2324683886) | Returns true if any node in a list matches the predicate\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Contains/index.md#4292341455) | Returns true if a node of the specified kind is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/Contains/index.md#2402474003) | Returns true if the specified node is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](../../Roslynator/SyntaxExtensions/DescendantTrivia/index.md#2025931486) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](../../Roslynator/SyntaxExtensions/DescendantTrivia/index.md#2885561996) | Get a list of all the trivia associated with the nodes in the list\. |
| [Find&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Find/index.md#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [IsFirst&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsFirst/index.md#1691317763) | Returns true if the specified node is a first node in the list\. |
| [IsLast(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](../../Roslynator/CSharp/SyntaxExtensions/IsLast/index.md) | Returns true if the specified statement is a last statement in the list\. |
| [IsLast&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsLast/index.md#554961423) | Returns true if the specified node is a last node in the list\. |
| [LastIndexOf&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [RemoveRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32)](../../Roslynator/CSharp/SyntaxExtensions/RemoveRange/index.md#3807495140) | Creates a new list with elements in the specified range removed\. |
| [ReplaceAt&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, TNode)](../../Roslynator/SyntaxExtensions/ReplaceAt/index.md#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |
| [TrimTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/TrimTrivia/index.md#92538413) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [WithTriviaFrom&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxNode)](../../Roslynator/SyntaxExtensions/WithTriviaFrom/index.md#301376900) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |

