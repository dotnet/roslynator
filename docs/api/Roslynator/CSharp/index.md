---
sidebar_label: CSharp
---

# Roslynator\.CSharp Namespace

**Containing Namespace**: [Roslynator](../index.md)

## Classes

| Class | Summary |
| ----- | ------- |
| [CSharpExtensions](CSharpExtensions/index.md) | A set of extension methods for a [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)\. |
| [CSharpFactory](CSharpFactory/index.md) | A factory for syntax nodes, tokens and trivia\. This class is built on top of [SyntaxFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxfactory) members\. |
| [CSharpFacts](CSharpFacts/index.md) | |
| [MemberDeclarationListSelection](MemberDeclarationListSelection/index.md) | Represents selected member declarations in a [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [ModifierList](ModifierList/index.md) | A set of static methods that allows manipulation with modifiers\. |
| [ModifierList&lt;TNode&gt;](ModifierList-1/index.md) | Represents a list of modifiers\. |
| [Modifiers](Modifiers/index.md) | Serves as a factory for a modifier list\. |
| [StatementListSelection](StatementListSelection/index.md) | Represents selected statements in a [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [SymbolExtensions](SymbolExtensions/index.md) | A set of static methods for [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol) and derived types\. |
| [SyntaxAccessibility](SyntaxAccessibility/index.md) | A set of static methods that are related to C\# accessibility\. |
| [SyntaxExtensions](SyntaxExtensions/index.md) | A set of extension methods for syntax \(types derived from [CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)\)\. |
| [SyntaxInfo](SyntaxInfo/index.md) | Serves as a factory for types in Roslynator\.CSharp\.Syntax namespace\. |
| [SyntaxInverter](SyntaxInverter/index.md) | \[deprecated\] Provides static methods for syntax inversion\. |
| [SyntaxLogicalInverter](SyntaxLogicalInverter/index.md) | Provides static methods for syntax inversion\. |
| [SyntaxLogicalInverterOptions](SyntaxLogicalInverterOptions/index.md) | |
| [WorkspaceExtensions](WorkspaceExtensions/index.md) | A set of extension methods for the workspace layer\. |
| [WorkspaceSymbolExtensions](WorkspaceSymbolExtensions/index.md) | |
| [WorkspaceSyntaxExtensions](WorkspaceSyntaxExtensions/index.md) | A set of extension methods for syntax\. These methods are dependent on the workspace layer\. |

## Structs

| Struct | Summary |
| ------ | ------- |
| [ExpressionChain](ExpressionChain/index.md) | Enables to enumerate expressions of a binary expression and expressions of nested binary expressions of the same kind as parent binary expression\. |
| [ExpressionChain.Enumerator](ExpressionChain/Enumerator/index.md) | |
| [ExpressionChain.Reversed](ExpressionChain/Reversed/index.md) | Enables to enumerate expressions of [ExpressionChain](ExpressionChain/index.md) in a reversed order\. |
| [ExpressionChain.Reversed.Enumerator](ExpressionChain/Reversed/Enumerator/index.md) | |
| [IfStatementCascade](IfStatementCascade/index.md) | Enables to enumerate if statement cascade\. |
| [IfStatementCascade.Enumerator](IfStatementCascade/Enumerator/index.md) | |
| [IfStatementCascadeInfo](IfStatementCascadeInfo/index.md) | Summarizes information about [IfStatementCascade](IfStatementCascade/index.md)\. |
| [IfStatementOrElseClause](IfStatementOrElseClause/index.md) | A wrapper for either an [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax) or an [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)\. |

## Enums

| Enum | Summary |
| ---- | ------- |
| [CommentFilter](CommentFilter/index.md) | Specifies C\# comments\. |
| [DefaultSyntaxOptions](DefaultSyntaxOptions/index.md) | Defines how a syntax representing a default value of a type should look like\. |
| [ModifierFilter](ModifierFilter/index.md) | Specifies C\# modifier\. |
| [NullCheckStyles](NullCheckStyles/index.md) | Specifies a null check\. |
| [PreprocessorDirectiveFilter](PreprocessorDirectiveFilter/index.md) | Specifies C\# preprocessor directives\. |

