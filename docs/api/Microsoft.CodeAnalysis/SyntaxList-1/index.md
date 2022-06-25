---
sidebar_label: SyntaxList<TNode>
---

# [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1) Struct Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [Add(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](../../Roslynator/CSharp/SyntaxExtensions/Add/index.md) | Creates a new list with the specified node added or inserted\. |
| [All&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/All/index.md#Roslynator_SyntaxExtensions_All__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Func___0_System_Boolean__) | Returns true if all nodes in a list matches the predicate\. |
| [Any&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](../../Roslynator/SyntaxExtensions/Any/index.md#Roslynator_SyntaxExtensions_Any__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Func___0_System_Boolean__) | Returns true if any node in a list matches the predicate\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Contains/index.md#Roslynator_CSharp_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node of the specified kind is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/Contains/index.md#Roslynator_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](../../Roslynator/SyntaxExtensions/DescendantTrivia/index.md#Roslynator_SyntaxExtensions_DescendantTrivia__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](../../Roslynator/SyntaxExtensions/DescendantTrivia/index.md#Roslynator_SyntaxExtensions_DescendantTrivia__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_Text_TextSpan_System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_) | Get a list of all the trivia associated with the nodes in the list\. |
| [Find&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/Find/index.md#Roslynator_CSharp_SyntaxExtensions_Find__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [IsFirst&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsFirst/index.md#Roslynator_SyntaxExtensions_IsFirst__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is a first node in the list\. |
| [IsLast(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](../../Roslynator/CSharp/SyntaxExtensions/IsLast/index.md) | Returns true if the specified statement is a last statement in the list\. |
| [IsLast&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](../../Roslynator/SyntaxExtensions/IsLast/index.md#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |
| [LastIndexOf&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](../../Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md#Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [RemoveRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32)](../../Roslynator/CSharp/SyntaxExtensions/RemoveRange/index.md#Roslynator_CSharp_SyntaxExtensions_RemoveRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_) | Creates a new list with elements in the specified range removed\. |
| [ReplaceAt&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, TNode)](../../Roslynator/SyntaxExtensions/ReplaceAt/index.md#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_) | Creates a new list with the node at the specified index replaced with a new node\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](../../Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32___0_) | Creates a new list with the elements in the specified range replaced with new node\. |
| [TrimTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;)](../../Roslynator/CSharp/SyntaxExtensions/TrimTrivia/index.md#Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1_Microsoft_CodeAnalysis_SyntaxList___0__) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [WithTriviaFrom&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxNode)](../../Roslynator/SyntaxExtensions/WithTriviaFrom/index.md#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
