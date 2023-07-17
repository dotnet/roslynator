---
sidebar_label: SeparatedSyntaxList<TNode>
---

# [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1) Struct Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [All&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/All/index.md#1104261355) | Returns true if all nodes in a list matches the predicate\. |
| [Any&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/Any/index.md#3469033055) | Returns true if any node in a list matches the predicate\. |
| [Contains&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Contains/index.md#434684779) | Searches for a node of the specified kind and returns the zero\-based index of the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/Contains/index.md#1049481281) | Returns true if the specified node is in the [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Find/index.md#3431504454) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [GetTrailingSeparator&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](../../Roslynator/SyntaxExtensions/GetTrailingSeparator/index.md) | Returns the trailing separator, if any\. |
| [HasTrailingSeparator&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](../../Roslynator/SyntaxExtensions/HasTrailingSeparator/index.md) | Returns true if the specified list contains trailing separator\. |
| [IsFirst&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsFirst/index.md#1292391442) | Returns true if the specified node is a first node in the list\. |
| [IsLast&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsLast/index.md#3058017669) | Returns true if the specified node is a last node in the list\. |
| [LastIndexOf&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [RemoveRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32)](../../Roslynator/CSharp/SyntaxExtensions/RemoveRange/index.md#1305034856) | Creates a new list with elements in the specified range removed\. |
| [ReplaceAt&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode)](../../Roslynator/SyntaxExtensions/ReplaceAt/index.md#3499086875) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#607003656) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#2148171151) | Creates a new list with the elements in the specified range replaced with new node\. |
| [TrimTrivia&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/TrimTrivia/index.md#1776013108) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [WithTriviaFrom&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxNode)](../../Roslynator/SyntaxExtensions/WithTriviaFrom/index.md#2087578213) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |

