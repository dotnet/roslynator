# SyntaxExtensions Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of extension methods for syntax \(types derived from [CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)\)\.

```csharp
public static class SyntaxExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(SyntaxList\<StatementSyntax\>, StatementSyntax, Boolean)](Add/README.md) | Creates a new list with the specified node added or inserted\. |
| [AddUsings(CompilationUnitSyntax, Boolean, UsingDirectiveSyntax\[\])](AddUsings/README.md) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the specified using directives added\. |
| [AsCascade(IfStatementSyntax)](AsCascade/README.md) | Returns [IfStatementCascade](../IfStatementCascade/README.md) that enables to enumerate if\-else cascade\. |
| [AsChain(BinaryExpressionSyntax, TextSpan?)](AsChain/README.md) | Returns [ExpressionChain](../ExpressionChain/README.md) that enables to enumerate expressions of a binary expression\. |
| [BodyOrExpressionBody(AccessorDeclarationSyntax)](BodyOrExpressionBody/README.md#4213369124) | Returns accessor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConstructorDeclarationSyntax)](BodyOrExpressionBody/README.md#3583146349) | Returns constructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConversionOperatorDeclarationSyntax)](BodyOrExpressionBody/README.md#2593258085) | Returns conversion operator body or an expression body if the body is null\. |
| [BodyOrExpressionBody(DestructorDeclarationSyntax)](BodyOrExpressionBody/README.md#1634292077) | Returns destructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(LocalFunctionStatementSyntax)](BodyOrExpressionBody/README.md#278134350) | Returns local function body or an expression body if the body is null\. |
| [BodyOrExpressionBody(MethodDeclarationSyntax)](BodyOrExpressionBody/README.md#3526085830) | Returns method body or an expression body if the body is null\. |
| [BodyOrExpressionBody(OperatorDeclarationSyntax)](BodyOrExpressionBody/README.md#390059482) | Returns operator body or an expression body if the body is null\. |
| [BracesSpan(ClassDeclarationSyntax)](BracesSpan/README.md#3880606548) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(EnumDeclarationSyntax)](BracesSpan/README.md#3795869771) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(InterfaceDeclarationSyntax)](BracesSpan/README.md#2155454711) | The absolute span of the braces, not including it leading and trailing trivia\. |
| [BracesSpan(NamespaceDeclarationSyntax)](BracesSpan/README.md#1419181908) | The absolute span of the braces, not including leading and trailing trivia\. |
| [BracesSpan(RecordDeclarationSyntax)](BracesSpan/README.md#1878584640) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [BracesSpan(StructDeclarationSyntax)](BracesSpan/README.md#1044020368) | The absolute span of the braces, not including its leading and trailing trivia\. |
| [Contains(SyntaxTokenList, SyntaxKind)](Contains/README.md#1379670452) | Returns true if a token of the specified kind is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains(SyntaxTriviaList, SyntaxKind)](Contains/README.md#4171339383) | Returns true if a trivia of the specified kind is in the [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [Contains\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](Contains/README.md#434684779) | Searches for a node of the specified kind and returns the zero\-based index of the first occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](Contains/README.md#4292341455) | Returns true if a node of the specified kind is in the [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/README.md#974978756) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/README.md#442211563) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind, SyntaxKind)](ContainsAny/README.md#3044041729) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsAny(SyntaxTokenList, SyntaxKind, SyntaxKind)](ContainsAny/README.md#1563993783) | Returns true if a token of the specified kinds is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [ContainsDefaultLabel(SwitchSectionSyntax)](ContainsDefaultLabel/README.md) | Returns true if the specified switch section contains default switch label\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode, TextSpan)](ContainsUnbalancedIfElseDirectives/README.md#1293653798) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode)](ContainsUnbalancedIfElseDirectives/README.md#3758239446) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsYield(LocalFunctionStatementSyntax)](ContainsYield/README.md#1371029737) | Returns true if the specified local function contains yield statement\. Nested local functions are excluded\. |
| [ContainsYield(MethodDeclarationSyntax)](ContainsYield/README.md#1794607794) | Returns true if the specified method contains yield statement\. Nested local functions are excluded\. |
| [DeclarationOrExpression(UsingStatementSyntax)](DeclarationOrExpression/README.md) | Returns using statement's declaration or an expression if the declaration is null\. |
| [DefaultSection(SwitchStatementSyntax)](DefaultSection/README.md) | Returns a section that contains default label, or null if the specified switch statement does not contains section with default label\. |
| [Elements(DocumentationCommentTriviaSyntax, String)](Elements/README.md) | Gets a list of xml elements with the specified local name\. |
| [Find(SyntaxTokenList, SyntaxKind)](Find/README.md#849057854) | Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Find(SyntaxTriviaList, SyntaxKind)](Find/README.md#972702330) | Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Find\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](Find/README.md#3431504454) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](Find/README.md#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [FirstAncestor(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean)](FirstAncestor/README.md#4122783476) | Gets the first ancestor that matches the predicate\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, Boolean)](FirstAncestor/README.md#2319682747) | Gets the first ancestor of the specified kind\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, SyntaxKind, Boolean)](FirstAncestor/README.md#3436882574) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestor(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, Boolean)](FirstAncestor/README.md#2233565705) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestorOrSelf(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean)](FirstAncestorOrSelf/README.md#3332998863) | Gets the first ancestor that matches the predicate\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, Boolean)](FirstAncestorOrSelf/README.md#706908351) | Gets the first ancestor of the specified kind\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, SyntaxKind, Boolean)](FirstAncestorOrSelf/README.md#1839797174) | Gets the first ancestor of the specified kinds\. |
| [FirstAncestorOrSelf(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, Boolean)](FirstAncestorOrSelf/README.md#280718491) | Gets the first ancestor of the specified kinds\. |
| [GetCascadeInfo(IfStatementSyntax)](GetCascadeInfo/README.md) | Returns [IfStatementCascadeInfo](../IfStatementCascadeInfo/README.md) that summarizes information about if\-else cascade\. |
| [GetDocumentationComment(MemberDeclarationSyntax)](GetDocumentationComment/README.md) | Returns documentation comment syntax that is part of the specified declaration\. |
| [GetDocumentationCommentTrivia(MemberDeclarationSyntax)](GetDocumentationCommentTrivia/README.md) | Returns documentation comment that is part of the specified declaration\. |
| [GetEndRegionDirective(RegionDirectiveTriviaSyntax)](GetEndRegionDirective/README.md) | Returns endregion directive that is related to the specified region directive\. Returns null if no matching endregion directive is found\. |
| [GetFirstDirective(SyntaxNode, TextSpan, Func\<DirectiveTriviaSyntax, Boolean\>)](GetFirstDirective/README.md) | Gets the first directive of the tree rooted by this node\. |
| [GetNextRelatedDirective(DirectiveTriviaSyntax)](GetNextRelatedDirective/README.md) | Returns the next related directive\. |
| [GetPreprocessingMessageTrivia(EndRegionDirectiveTriviaSyntax)](GetPreprocessingMessageTrivia/README.md#3549782897) | Gets preprocessing message for the specified endregion directive if such message exists\. |
| [GetPreprocessingMessageTrivia(RegionDirectiveTriviaSyntax)](GetPreprocessingMessageTrivia/README.md#1660117599) | Gets preprocessing message for the specified region directive if such message exists\. |
| [GetRegionDirective(EndRegionDirectiveTriviaSyntax)](GetRegionDirective/README.md) | Returns region directive that is related to the specified endregion directive\. Returns null if no matching region directive is found\. |
| [GetSingleLineDocumentationComment(MemberDeclarationSyntax)](GetSingleLineDocumentationComment/README.md) | Returns single\-line documentation comment syntax that is part of the specified declaration\. |
| [GetSingleLineDocumentationCommentTrivia(MemberDeclarationSyntax)](GetSingleLineDocumentationCommentTrivia/README.md) | Returns single\-line documentation comment that is part of the specified declaration\. |
| [Getter(AccessorListSyntax)](Getter/README.md#3749591364) | Returns a get accessor contained in the specified list\. |
| [Getter(IndexerDeclarationSyntax)](Getter/README.md#2491107778) | Returns a get accessor that is contained in the specified indexer declaration\. |
| [Getter(PropertyDeclarationSyntax)](Getter/README.md#2677777844) | Returns property get accessor, if any\. |
| [GetTopmostIf(ElseClauseSyntax)](GetTopmostIf/README.md#2176362029) | Returns topmost if statement of the if\-else cascade the specified else clause is part of\. |
| [GetTopmostIf(IfStatementSyntax)](GetTopmostIf/README.md#210946778) | Returns topmost if statement of the if\-else cascade the specified if statement is part of\. |
| [HasDocumentationComment(MemberDeclarationSyntax)](HasDocumentationComment/README.md) | Returns true if the specified declaration has a documentation comment\. |
| [HasSingleLineDocumentationComment(MemberDeclarationSyntax)](HasSingleLineDocumentationComment/README.md) | Returns true if the specified declaration has a single\-line documentation comment\. |
| [IsAutoImplemented(AccessorDeclarationSyntax)](IsAutoImplemented/README.md) | Returns true is the specified accessor is auto\-implemented accessor\. |
| [IsDescendantOf(SyntaxNode, SyntaxKind, Boolean)](IsDescendantOf/README.md) | Returns true if a node is a descendant of a node with the specified kind\. |
| [IsEmbedded(StatementSyntax, Boolean, Boolean, Boolean)](IsEmbedded/README.md) | Returns true if the specified statement is an embedded statement\. |
| [IsEmptyOrWhitespace(SyntaxTriviaList)](IsEmptyOrWhitespace/README.md) | Returns true if the list of either empty or contains only whitespace \([SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) or [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\)\. |
| [IsEndOfLineTrivia(SyntaxTrivia)](IsEndOfLineTrivia/README.md) | Returns true if the trivia is [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\. |
| [IsHexNumericLiteral(LiteralExpressionSyntax)](IsHexNumericLiteral/README.md) | Returns true if the specified literal expression is a hexadecimal numeric literal expression\. |
| [IsInExpressionTree(SyntaxNode, SemanticModel, CancellationToken)](IsInExpressionTree/README.md) | Determines if the specified node is contained in an expression tree\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#1975403005) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3028059531) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#580360109) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3185459241) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind)](IsKind/README.md#1541609469) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3094724497) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3356796170) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#907715724) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3793139866) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind)](IsKind/README.md#3033402641) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#2923191543) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3366590052) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#839254370) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind)](IsKind/README.md#3487796432) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind)](IsKind/README.md#94153864) | Returns true if a trivia's kind is one of the specified kinds\. |
| [IsLast(SyntaxList\<StatementSyntax\>, StatementSyntax, Boolean)](IsLast/README.md) | Returns true if the specified statement is a last statement in the list\. |
| [IsParams(ParameterSyntax)](IsParams/README.md) | Returns true if the specified parameter has "params" modifier\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#3570688798) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#2131485272) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#1935129073) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#2974797413) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind)](IsParentKind/README.md#93375457) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind)](IsParentKind/README.md#1778613464) | Returns true if a node parent's kind is the specified kind\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#3000055021) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#3691311238) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#3746288239) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](IsParentKind/README.md#2940773750) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind)](IsParentKind/README.md#4201641965) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind)](IsParentKind/README.md#2159706118) | Returns true if a token parent's kind is the specified kind\. |
| [IsSimpleIf(IfStatementSyntax)](IsSimpleIf/README.md) | Returns true if the specified if statement is a simple if statement\. Simple if statement is defined as follows: it is not a child of an else clause and it has no else clause\. |
| [IsTopmostIf(IfStatementSyntax)](IsTopmostIf/README.md) | Returns true if the specified if statement is not a child of an else clause\. |
| [IsVerbatim(InterpolatedStringExpressionSyntax)](IsVerbatim/README.md) | Returns true if the specified interpolated string is a verbatim\. |
| [IsVoid(TypeSyntax)](IsVoid/README.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [IsWhitespaceOrEndOfLineTrivia(SyntaxTrivia)](IsWhitespaceOrEndOfLineTrivia/README.md) | Returns true if the trivia is either [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) or [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\. |
| [IsWhitespaceTrivia(SyntaxTrivia)](IsWhitespaceTrivia/README.md) | Returns true if the trivia is [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia)\. |
| [IsYieldBreak(YieldStatementSyntax)](IsYieldBreak/README.md) | Returns true if the specified statement is a yield break statement\. |
| [IsYieldReturn(YieldStatementSyntax)](IsYieldReturn/README.md) | Returns true if the specified statement is a yield return statement\. |
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](LastIndexOf/README.md#2989371063) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](LastIndexOf/README.md#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](LastIndexOf/README.md#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [NextStatement(StatementSyntax)](NextStatement/README.md) | Gets the next statement of the specified statement\. If the specified statement is not contained in the list, or if there is no next statement, then this method returns null\. |
| [ParenthesesSpan(CastExpressionSyntax)](ParenthesesSpan/README.md#1201715952) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(CommonForEachStatementSyntax)](ParenthesesSpan/README.md#1008516473) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(ForStatementSyntax)](ParenthesesSpan/README.md#3518600528) | Absolute span of the parentheses, not including the leading and trailing trivia\. |
| [PreviousStatement(StatementSyntax)](PreviousStatement/README.md) | Gets the previous statement of the specified statement\. If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null\. |
| [RemoveRange(SyntaxTokenList, Int32, Int32)](RemoveRange/README.md#560377099) | Creates a new list with tokens in the specified range removed\. |
| [RemoveRange(SyntaxTriviaList, Int32, Int32)](RemoveRange/README.md#2543741306) | Creates a new list with trivia in the specified range removed\. |
| [RemoveRange\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, Int32)](RemoveRange/README.md#1305034856) | Creates a new list with elements in the specified range removed\. |
| [RemoveRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32)](RemoveRange/README.md#3807495140) | Creates a new list with elements in the specified range removed\. |
| [RemoveTrivia\<TNode\>(TNode, TextSpan?)](RemoveTrivia/README.md) | Creates a new node with the trivia removed\. |
| [RemoveWhitespace\<TNode\>(TNode, TextSpan?)](RemoveWhitespace/README.md) | Creates a new node with the whitespace removed\. |
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable\<SyntaxToken\>)](ReplaceRange/README.md#4257224275) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable\<SyntaxTrivia\>)](ReplaceRange/README.md#4063342571) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>)](ReplaceRange/README.md#607003656) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, Int32, TNode)](ReplaceRange/README.md#2148171151) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>)](ReplaceRange/README.md#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, TNode)](ReplaceRange/README.md#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceWhitespace\<TNode\>(TNode, SyntaxTrivia, TextSpan?)](ReplaceWhitespace/README.md) | Creates a new node with the whitespace replaced\. |
| [ReturnsVoid(DelegateDeclarationSyntax)](ReturnsVoid/README.md#1910724647) | Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(LocalFunctionStatementSyntax)](ReturnsVoid/README.md#3225843639) | Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(MethodDeclarationSyntax)](ReturnsVoid/README.md#2714446372) | Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [Setter(AccessorListSyntax)](Setter/README.md#1651493367) | Returns a set accessor contained in the specified list\. |
| [Setter(IndexerDeclarationSyntax)](Setter/README.md#1041810977) | Returns a set accessor that is contained in the specified indexer declaration\. |
| [Setter(PropertyDeclarationSyntax)](Setter/README.md#2111161647) | Returns property set accessor, if any\. |
| [ToSeparatedSyntaxList\<TNode\>(IEnumerable\<SyntaxNodeOrToken\>)](ToSeparatedSyntaxList/README.md#3594200340) | Creates a separated list of syntax nodes from a sequence of nodes and tokens\. |
| [ToSeparatedSyntaxList\<TNode\>(IEnumerable\<TNode\>)](ToSeparatedSyntaxList/README.md#2814099200) | Creates a separated list of syntax nodes from a sequence of nodes\. |
| [ToSyntaxList\<TNode\>(IEnumerable\<TNode\>)](ToSyntaxList/README.md) | Creates a list of syntax nodes from a sequence of nodes\. |
| [ToSyntaxTokenList(IEnumerable\<SyntaxToken\>)](ToSyntaxTokenList/README.md) | Creates a list of syntax tokens from a sequence of tokens\. |
| [TrimLeadingTrivia(SyntaxToken)](TrimLeadingTrivia/README.md#1084780771) | Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimLeadingTrivia\<TNode\>(TNode)](TrimLeadingTrivia/README.md#1018285907) | Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TrimTrailingTrivia(SyntaxToken)](TrimTrailingTrivia/README.md#3749666890) | Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrailingTrivia\<TNode\>(TNode)](TrimTrailingTrivia/README.md#2450702114) | Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TrimTrivia(SyntaxToken)](TrimTrivia/README.md#3557770056) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new token with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrivia\<TNode\>(SeparatedSyntaxList\<TNode\>)](TrimTrivia/README.md#1776013108) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia\<TNode\>(SyntaxList\<TNode\>)](TrimTrivia/README.md#92538413) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia\<TNode\>(TNode)](TrimTrivia/README.md#3568210656) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new node with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
| [TryGetContainingList(StatementSyntax, SyntaxList\<StatementSyntax\>)](TryGetContainingList/README.md) | Gets a list the specified statement is contained in\. This method succeeds if the statement is in a block's statements or a switch section's statements\. |
| [WalkDownParentheses(ExpressionSyntax)](WalkDownParentheses/README.md) | Returns lowest expression in parentheses or self if the expression is not parenthesized\. |
| [WalkUpParentheses(ExpressionSyntax)](WalkUpParentheses/README.md) | Returns topmost parenthesized expression or self if the expression if not parenthesized\. |
| [WithMembers(ClassDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#509233246) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(ClassDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/README.md#1261436636) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#1993657641) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, MemberDeclarationSyntax)](WithMembers/README.md#1847012895) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#3092068364) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/README.md#3435655262) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#1938122577) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/README.md#1822379855) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#546601379) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/README.md#2335486806) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax\>)](WithMembers/README.md#3849653050) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, MemberDeclarationSyntax)](WithMembers/README.md#2059906427) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |

