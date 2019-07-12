# CSharpFacts Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

```csharp
public static class CSharpFacts
```

## Methods

| Method | Summary |
| ------ | ------- |
| [CanBeEmbeddedStatement(SyntaxKind)](CanBeEmbeddedStatement/README.md) | Returns true if a syntax of the specified kind can be an embedded statement\. |
| [CanHaveEmbeddedStatement(SyntaxKind)](CanHaveEmbeddedStatement/README.md) | Returns true if a syntax of the specified kind can have an embedded statement\. |
| [CanHaveExpressionBody(SyntaxKind)](CanHaveExpressionBody/README.md) | Returns true if a syntax of the specified kind can have expression body\. |
| [CanHaveMembers(SyntaxKind)](CanHaveMembers/README.md) | Returns true if a syntax of the specified kind can have members\. |
| [CanHaveModifiers(SyntaxKind)](CanHaveModifiers/README.md) | Returns true if a syntax of the specified kind can have modifiers\. |
| [CanHaveStatements(SyntaxKind)](CanHaveStatements/README.md) | Returns true if a syntax of the specified kind can have statements\. It can be either [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax) or [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)\. |
| [IsAnonymousFunctionExpression(SyntaxKind)](IsAnonymousFunctionExpression/README.md) | Returns true if a syntax of the specified kind is an anonymous method or lambda expression\. |
| [IsBooleanExpression(SyntaxKind)](IsBooleanExpression/README.md) | Returns true if a syntax of the specified kind is a boolean expression\. |
| [IsBooleanLiteralExpression(SyntaxKind)](IsBooleanLiteralExpression/README.md) | Returns true if a syntax of the specified kind is true or false literal expression\. |
| [IsCommentTrivia(SyntaxKind)](IsCommentTrivia/README.md) | Returns true if a syntax of the specified kind is comment trivia\. |
| [IsCompoundAssignmentExpression(SyntaxKind)](IsCompoundAssignmentExpression/README.md) | Returns true if a syntax of the specified kind is a compound assignment expression\. |
| [IsConstraint(SyntaxKind)](IsConstraint/README.md) | Returns true if a syntax of the specified kind is a constraint\. |
| [IsFunction(SyntaxKind)](IsFunction/README.md) | Returns true if a syntax of the specified kind if local function or anonymous function\. |
| [IsIfElseDirective(SyntaxKind)](IsIfElseDirective/README.md) | Returns true if a syntax of the specified kind is \#if, \#else, \#elif or \#endif directive\. |
| [IsIncrementOrDecrementExpression(SyntaxKind)](IsIncrementOrDecrementExpression/README.md) | Returns true if a syntax of the specified kind is pre/post increment/decrement expression\. |
| [IsIterationStatement(SyntaxKind)](IsIterationStatement/README.md) | Returns true if a syntax of the specified kind is a for, foreach, while or do statement\. |
| [IsJumpStatement(SyntaxKind)](IsJumpStatement/README.md) | Returns true if a syntax of the specified kind is a jump statement\. |
| [IsLambdaExpression(SyntaxKind)](IsLambdaExpression/README.md) | Returns true if a syntax of the specified kind is a lambda expression\. |
| [IsLiteralExpression(SyntaxKind)](IsLiteralExpression/README.md) | Returns true if a syntax of the specified kind is a literal expression\. |
| [IsPredefinedType(SpecialType)](IsPredefinedType/README.md) | Returns true if a syntax of the specified kind is a predefined type\. |
| [IsSimpleType(SpecialType)](IsSimpleType/README.md) | Returns true if a syntax of the specified kind is a simple type\. |
| [IsSwitchLabel(SyntaxKind)](IsSwitchLabel/README.md) | Returns true if a syntax of the specified kind is a switch label\. |
| [SupportsPrefixOrPostfixUnaryOperator(SpecialType)](SupportsPrefixOrPostfixUnaryOperator/README.md) | Returns true if an expression of the specified type can be used in a prefix or postfix unary operator\. |

