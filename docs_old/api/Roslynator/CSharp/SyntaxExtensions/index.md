---
sidebar_label: SyntaxExtensions
---

# SyntaxExtensions Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of extension methods for syntax \(types derived from [CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)\)\.

```csharp
public static class SyntaxExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](Add/index.md) | Creates a new list with the specified node added or inserted\. |
| [AddUsings(CompilationUnitSyntax, Boolean, UsingDirectiveSyntax\[\])](AddUsings/index.md) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the specified using directives added\. |
| [AsCascade(IfStatementSyntax)](AsCascade/index.md) | Returns [IfStatementCascade](../IfStatementCascade/index.md) that enables to enumerate if\-else cascade\. |
| [AsChain(BinaryExpressionSyntax, TextSpan?)](AsChain/index.md) | Returns [ExpressionChain](../ExpressionChain/index.md) that enables to enumerate expressions of a binary expression\. |
| [BodyOrExpressionBody(AccessorDeclarationSyntax)](BodyOrExpressionBody/index.md#4213369124) | Returns accessor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConstructorDeclarationSyntax)](BodyOrExpressionBody/index.md#3583146349) | Returns constructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConversionOperatorDeclarationSyntax)](BodyOrExpressionBody/index.md#2593258085) | Returns conversion operator body or an expression body if the body is null\. |
| [BodyOrExpressionBody(DestructorDeclarationSyntax)](BodyOrExpressionBody/index.md#1634292077) | Returns destructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(LocalFunctionStatementSyntax)](BodyOrExpressionBody/index.md#278134350) | Returns local function body or an expression body if the body is null\. |
| [BodyOrExpressionBody(MethodDeclarationSyntax)](BodyOrExpressionBody/index.md#3526085830) | Returns method body or an expression body if the body is null\. |
| [BodyOrExpressionBody(OperatorDeclarationSyntax)](BodyOrExpressionBody/index.md#390059482) | Returns operator body or an expression body if the body is null\. |
| [BracesSpan(ClassDeclarationSyntax)](BracesSpan/index.md#3880606548) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(EnumDeclarationSyntax)](BracesSpan/index.md#3795869771) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(InterfaceDeclarationSyntax)](BracesSpan/index.md#2155454711) | The absolute span of the braces, not including it leading and trailing trivia\. |
| [BracesSpan(NamespaceDeclarationSyntax)](BracesSpan/index.md#1419181908) | The absolute span of the braces, not including leading and trailing trivia\. |
| [BracesSpan(RecordDeclarationSyntax)](BracesSpan/index.md#1878584640) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(StructDeclarationSyntax)](BracesSpan/index.md#1044020368) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [Contains(SyntaxTokenList, SyntaxKind)](Contains/index.md#1379670452) | Returns true if a token of the specified kind is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains(SyntaxTriviaList, SyntaxKind)](Contains/index.md#4171339383) | Returns true if a trivia of the specified kind is in the [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [Contains&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](Contains/index.md#434684779) | Searches for a node of the specified kind and returns the zero\-based index of the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](Contains/index.md#4292341455) | Returns true if a node of the specified kind is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/index.md#974978756) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/index.md#442211563) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/index.md#3044041729) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind)](ContainsAny/index.md#1563993783) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsDefaultLabel(SwitchSectionSyntax)](ContainsDefaultLabel/index.md) | Returns true if the specified switch section contains default switch label\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode, TextSpan)](ContainsUnbalancedIfElseDirectives/index.md#1293653798) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode)](ContainsUnbalancedIfElseDirectives/index.md#3758239446) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsYield(LocalFunctionStatementSyntax)](ContainsYield/index.md#1371029737) | Returns true if the specified local function contains yield statement\. Nested local functions are excluded\. |
| [ContainsYield(MethodDeclarationSyntax)](ContainsYield/index.md#1794607794) | Returns true if the specified method contains yield statement\. Nested local functions are excluded\. |
| [DeclarationOrExpression(UsingStatementSyntax)](DeclarationOrExpression/index.md) | Returns using statement's declaration or an expression if the declaration is null\. |
| [DefaultSection(SwitchStatementSyntax)](DefaultSection/index.md) | Returns a section that contains default label, or null if the specified switch statement does not contains section with default label\. |
| [Elements(DocumentationCommentTriviaSyntax, String)](Elements/index.md) | Gets a list of xml elements with the specified local name\. |
| [Find(SyntaxTokenList, SyntaxKind)](Find/index.md#849057854) | Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Find(SyntaxTriviaList, SyntaxKind)](Find/index.md#972702330) | Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Find&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](Find/index.md#3431504454) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](Find/index.md#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [FirstAncestor(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstAncestor/index.md#4122783476) | Gets the first ancestor that matches the predicate\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, Boolean)](FirstAncestor/index.md#2319682747) | Gets the first ancestor of the specified kind\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, SyntaxKind, Boolean)](FirstAncestor/index.md#3436882574) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, Boolean)](FirstAncestor/index.md#2233565705) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestorOrSelf(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](FirstAncestorOrSelf/index.md#3332998863) | Gets the first ancestor that matches the predicate\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, Boolean)](FirstAncestorOrSelf/index.md#706908351) | Gets the first ancestor of the specified kind\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, SyntaxKind, Boolean)](FirstAncestorOrSelf/index.md#1839797174) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, Boolean)](FirstAncestorOrSelf/index.md#280718491) | Gets the first ancestor of the specified kinds\. |
| [GetCascadeInfo(IfStatementSyntax)](GetCascadeInfo/index.md) | Returns [IfStatementCascadeInfo](../IfStatementCascadeInfo/index.md) that summarizes information about if\-else cascade\. |
| [GetDocumentationComment(MemberDeclarationSyntax)](GetDocumentationComment/index.md) | Returns documentation comment syntax that is part of the specified declaration\. |
| [GetDocumentationCommentTrivia(MemberDeclarationSyntax)](GetDocumentationCommentTrivia/index.md) | Returns documentation comment that is part of the specified declaration\. |
| [GetEndRegionDirective(RegionDirectiveTriviaSyntax)](GetEndRegionDirective/index.md) | Returns endregion directive that is related to the specified region directive\. Returns null if no matching endregion directive is found\. |
| [GetFirstDirective(SyntaxNode, TextSpan, Func&lt;DirectiveTriviaSyntax, Boolean&gt;)](GetFirstDirective/index.md) | Gets the first directive of the tree rooted by this node\. |
| [GetNextRelatedDirective(DirectiveTriviaSyntax)](GetNextRelatedDirective/index.md) | Returns the next related directive\. |
| [GetPreprocessingMessageTrivia(EndRegionDirectiveTriviaSyntax)](GetPreprocessingMessageTrivia/index.md#3549782897) | Gets preprocessing message for the specified endregion directive if such message exists\. |
| [GetPreprocessingMessageTrivia(RegionDirectiveTriviaSyntax)](GetPreprocessingMessageTrivia/index.md#1660117599) | Gets preprocessing message for the specified region directive if such message exists\. |
| [GetRegionDirective(EndRegionDirectiveTriviaSyntax)](GetRegionDirective/index.md) | Returns region directive that is related to the specified endregion directive\. Returns null if no matching region directive is found\. |
| [GetSingleLineDocumentationComment(MemberDeclarationSyntax)](GetSingleLineDocumentationComment/index.md) | Returns single\-line documentation comment syntax that is part of the specified declaration\. |
| [GetSingleLineDocumentationCommentTrivia(MemberDeclarationSyntax)](GetSingleLineDocumentationCommentTrivia/index.md) | Returns single\-line documentation comment that is part of the specified declaration\. |
| [Getter(AccessorListSyntax)](Getter/index.md#3749591364) | Returns a get accessor contained in the specified list\. |
| [Getter(IndexerDeclarationSyntax)](Getter/index.md#2491107778) | Returns a get accessor that is contained in the specified indexer declaration\. |
| [Getter(PropertyDeclarationSyntax)](Getter/index.md#2677777844) | Returns property get accessor, if any\. |
| [GetTopmostIf(ElseClauseSyntax)](GetTopmostIf/index.md#2176362029) | Returns topmost if statement of the if\-else cascade the specified else clause is part of\. |
| [GetTopmostIf(IfStatementSyntax)](GetTopmostIf/index.md#210946778) | Returns topmost if statement of the if\-else cascade the specified if statement is part of\. |
| [HasDocumentationComment(MemberDeclarationSyntax)](HasDocumentationComment/index.md) | Returns true if the specified declaration has a documentation comment\. |
| [HasSingleLineDocumentationComment(MemberDeclarationSyntax)](HasSingleLineDocumentationComment/index.md) | Returns true if the specified declaration has a single\-line documentation comment\. |
| [IsAutoImplemented(AccessorDeclarationSyntax)](IsAutoImplemented/index.md) | Returns true is the specified accessor is auto\-implemented accessor\. |
| [IsDescendantOf(SyntaxNode, SyntaxKind, Boolean)](IsDescendantOf/index.md) | Returns true if a node is a descendant of a node with the specified kind\. |
| [IsEmbedded(StatementSyntax, Boolean, Boolean, Boolean)](IsEmbedded/index.md) | Returns true if the specified statement is an embedded statement\. |
| [IsEmptyOrWhitespace(SyntaxTriviaList)](IsEmptyOrWhitespace/index.md) | Returns true if the list of either empty or contains only whitespace \([SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) or [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\)\. |
| [IsEndOfLineTrivia(SyntaxTrivia)](IsEndOfLineTrivia/index.md) | Returns true if the trivia is [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\. |
| [IsHexNumericLiteral(LiteralExpressionSyntax)](IsHexNumericLiteral/index.md) | Returns true if the specified literal expression is a hexadecimal numeric literal expression\. |
| [IsInExpressionTree(SyntaxNode, SemanticModel, CancellationToken)](IsInExpressionTree/index.md) | Determines if the specified node is contained in an expression tree\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#1975403005) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3028059531) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#580360109) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3185459241) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind)](IsKind/index.md#1541609469) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3094724497) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3356796170) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#907715724) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3793139866) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind)](IsKind/index.md#3033402641) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#2923191543) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3366590052) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#839254370) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/index.md#3487796432) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind)](IsKind/index.md#94153864) | Returns true if a trivia's kind is one of the specified kinds\. |
| [IsLast(SyntaxList&lt;StatementSyntax&gt;, StatementSyntax, Boolean)](IsLast/index.md) | Returns true if the specified statement is a last statement in the list\. |
| [IsParams(ParameterSyntax)](IsParams/index.md) | Returns true if the specified parameter has "params" modifier\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#3570688798) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#2131485272) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#1935129073) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#2974797413) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind)](IsParentKind/index.md#93375457) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind)](IsParentKind/index.md#1778613464) | Returns true if a node parent's kind is the specified kind\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#3000055021) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#3691311238) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#3746288239) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/index.md#2940773750) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind)](IsParentKind/index.md#4201641965) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind)](IsParentKind/index.md#2159706118) | Returns true if a token parent's kind is the specified kind\. |
| [IsSimpleIf(IfStatementSyntax)](IsSimpleIf/index.md) | Returns true if the specified if statement is a simple if statement\. Simple if statement is defined as follows: it is not a child of an else clause and it has no else clause\. |
| [IsTopmostIf(IfStatementSyntax)](IsTopmostIf/index.md) | Returns true if the specified if statement is not a child of an else clause\. |
| [IsVerbatim(InterpolatedStringExpressionSyntax)](IsVerbatim/index.md) | Returns true if the specified interpolated string is a verbatim\. |
| [IsVoid(TypeSyntax)](IsVoid/index.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [IsWhitespaceOrEndOfLineTrivia(SyntaxTrivia)](IsWhitespaceOrEndOfLineTrivia/index.md) | Returns true if the trivia is either [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) or [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\. |
| [IsWhitespaceTrivia(SyntaxTrivia)](IsWhitespaceTrivia/index.md) | Returns true if the trivia is [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia)\. |
| [IsYieldBreak(YieldStatementSyntax)](IsYieldBreak/index.md) | Returns true if the specified statement is a yield break statement\. |
| [IsYieldReturn(YieldStatementSyntax)](IsYieldReturn/index.md) | Returns true if the specified statement is a yield return statement\. |
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](LastIndexOf/index.md#2989371063) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](LastIndexOf/index.md#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](LastIndexOf/index.md#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [NextStatement(StatementSyntax)](NextStatement/index.md) | Gets the next statement of the specified statement\. If the specified statement is not contained in the list, or if there is no next statement, then this method returns null\. |
| [ParenthesesSpan(CastExpressionSyntax)](ParenthesesSpan/index.md#1201715952) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(CommonForEachStatementSyntax)](ParenthesesSpan/index.md#1008516473) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(ForStatementSyntax)](ParenthesesSpan/index.md#3518600528) | Absolute span of the parentheses, not including the leading and trailing trivia\. |
| [PreviousStatement(StatementSyntax)](PreviousStatement/index.md) | Gets the previous statement of the specified statement\. If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null\. |
| [RemoveRange(SyntaxTokenList, Int32, Int32)](RemoveRange/index.md#560377099) | Creates a new list with tokens in the specified range removed\. |
| [RemoveRange(SyntaxTriviaList, Int32, Int32)](RemoveRange/index.md#2543741306) | Creates a new list with trivia in the specified range removed\. |
| [RemoveRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32)](RemoveRange/index.md#1305034856) | Creates a new list with elements in the specified range removed\. |
| [RemoveRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32)](RemoveRange/index.md#3807495140) | Creates a new list with elements in the specified range removed\. |
| [RemoveTrivia&lt;TNode&gt;(TNode, TextSpan?)](RemoveTrivia/index.md) | Creates a new node with the trivia removed\. |
| [RemoveWhitespace&lt;TNode&gt;(TNode, TextSpan?)](RemoveWhitespace/index.md) | Creates a new node with the whitespace removed\. |
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable&lt;SyntaxToken&gt;)](ReplaceRange/index.md#4257224275) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable&lt;SyntaxTrivia&gt;)](ReplaceRange/index.md#4063342571) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](ReplaceRange/index.md#607003656) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](ReplaceRange/index.md#2148171151) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](ReplaceRange/index.md#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](ReplaceRange/index.md#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceWhitespace&lt;TNode&gt;(TNode, SyntaxTrivia, TextSpan?)](ReplaceWhitespace/index.md) | Creates a new node with the whitespace replaced\. |
| [ReturnsVoid(DelegateDeclarationSyntax)](ReturnsVoid/index.md#1910724647) | Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(LocalFunctionStatementSyntax)](ReturnsVoid/index.md#3225843639) | Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(MethodDeclarationSyntax)](ReturnsVoid/index.md#2714446372) | Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [Setter(AccessorListSyntax)](Setter/index.md#1651493367) | Returns a set accessor contained in the specified list\. |
| [Setter(IndexerDeclarationSyntax)](Setter/index.md#1041810977) | Returns a set accessor that is contained in the specified indexer declaration\. |
| [Setter(PropertyDeclarationSyntax)](Setter/index.md#2111161647) | Returns property set accessor, if any\. |
| [ToSeparatedSyntaxList&lt;TNode&gt;(IEnumerable&lt;SyntaxNodeOrToken&gt;)](ToSeparatedSyntaxList/index.md#3594200340) | Creates a separated list of syntax nodes from a sequence of nodes and tokens\. |
| [ToSeparatedSyntaxList&lt;TNode&gt;(IEnumerable&lt;TNode&gt;)](ToSeparatedSyntaxList/index.md#2814099200) | Creates a separated list of syntax nodes from a sequence of nodes\. |
| [ToSyntaxList&lt;TNode&gt;(IEnumerable&lt;TNode&gt;)](ToSyntaxList/index.md) | Creates a list of syntax nodes from a sequence of nodes\. |
| [ToSyntaxTokenList(IEnumerable&lt;SyntaxToken&gt;)](ToSyntaxTokenList/index.md) | Creates a list of syntax tokens from a sequence of tokens\. |
| [TrimLeadingTrivia(SyntaxToken)](TrimLeadingTrivia/index.md#1084780771) | Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimLeadingTrivia&lt;TNode&gt;(TNode)](TrimLeadingTrivia/index.md#1018285907) | Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TrimTrailingTrivia(SyntaxToken)](TrimTrailingTrivia/index.md#3749666890) | Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrailingTrivia&lt;TNode&gt;(TNode)](TrimTrailingTrivia/index.md#2450702114) | Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TrimTrivia(SyntaxToken)](TrimTrivia/index.md#3557770056) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new token with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrivia&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;)](TrimTrivia/index.md#1776013108) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;)](TrimTrivia/index.md#92538413) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia&lt;TNode&gt;(TNode)](TrimTrivia/index.md#3568210656) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new node with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TryGetContainingList(StatementSyntax, SyntaxList&lt;StatementSyntax&gt;)](TryGetContainingList/index.md) | Gets a list the specified statement is contained in\. This method succeeds if the statement is in a block's statements or a switch section's statements\. |
| [WalkDownParentheses(ExpressionSyntax)](WalkDownParentheses/index.md) | Returns lowest expression in parentheses or self if the expression is not parenthesized\. |
| [WalkUpParentheses(ExpressionSyntax)](WalkUpParentheses/index.md) | Returns topmost parenthesized expression or self if the expression if not parenthesized\. |
| [WithMembers(ClassDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#509233246) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(ClassDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/index.md#1261436636) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#1993657641) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, MemberDeclarationSyntax)](WithMembers/index.md#1847012895) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#3092068364) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/index.md#3435655262) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#1938122577) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/index.md#1822379855) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#546601379) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/index.md#2335486806) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#3849653050) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/index.md#2059906427) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |

