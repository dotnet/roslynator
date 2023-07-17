---
sidebar_label: CSharpFacts
---

# CSharpFacts Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

```csharp
public static class CSharpFacts
```

## Methods

| Method | Summary |
| ------ | ------- |
| [CanBeEmbeddedStatement(SyntaxKind)](CanBeEmbeddedStatement/index.md) | Returns true if a syntax of the specified kind can be an embedded statement\. |
| [CanHaveEmbeddedStatement(SyntaxKind)](CanHaveEmbeddedStatement/index.md) | Returns true if a syntax of the specified kind can have an embedded statement\. |
| [CanHaveExpressionBody(SyntaxKind)](CanHaveExpressionBody/index.md) | Returns true if a syntax of the specified kind can have expression body\. |
| [CanHaveMembers(SyntaxKind)](CanHaveMembers/index.md) | Returns true if a syntax of the specified kind can have members\. |
| [CanHaveModifiers(SyntaxKind)](CanHaveModifiers/index.md) | Returns true if a syntax of the specified kind can have modifiers\. |
| [CanHaveStatements(SyntaxKind)](CanHaveStatements/index.md) | Returns true if a syntax of the specified kind can have statements\. It can be either [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax) or [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)\. |
| [IsAnonymousFunctionExpression(SyntaxKind)](IsAnonymousFunctionExpression/index.md) | Returns true if a syntax of the specified kind is an anonymous method or lambda expression\. |
| [IsBooleanExpression(SyntaxKind)](IsBooleanExpression/index.md) | Returns true if a syntax of the specified kind is a boolean expression\. |
| [IsBooleanLiteralExpression(SyntaxKind)](IsBooleanLiteralExpression/index.md) | Returns true if a syntax of the specified kind is true or false literal expression\. |
| [IsCommentTrivia(SyntaxKind)](IsCommentTrivia/index.md) | Returns true if a syntax of the specified kind is comment trivia\. |
| [IsCompoundAssignmentExpression(SyntaxKind)](IsCompoundAssignmentExpression/index.md) | Returns true if a syntax of the specified kind is a compound assignment expression\. |
| [IsConstraint(SyntaxKind)](IsConstraint/index.md) | Returns true if a syntax of the specified kind is a constraint\. |
| [IsFunction(SyntaxKind)](IsFunction/index.md) | Returns true if a syntax of the specified kind if local function or anonymous function\. |
| [IsIfElseDirective(SyntaxKind)](IsIfElseDirective/index.md) | Returns true if a syntax of the specified kind is \#if, \#else, \#elif or \#endif directive\. |
| [IsIncrementOrDecrementExpression(SyntaxKind)](IsIncrementOrDecrementExpression/index.md) | Returns true if a syntax of the specified kind is pre/post increment/decrement expression\. |
| [IsIterationStatement(SyntaxKind)](IsIterationStatement/index.md) | Returns true if a syntax of the specified kind is a for, foreach, while or do statement\. |
| [IsJumpStatement(SyntaxKind)](IsJumpStatement/index.md) | Returns true if a syntax of the specified kind is a jump statement\. |
| [IsLambdaExpression(SyntaxKind)](IsLambdaExpression/index.md) | Returns true if a syntax of the specified kind is a lambda expression\. |
| [IsLiteralExpression(SyntaxKind)](IsLiteralExpression/index.md) | Returns true if a syntax of the specified kind is a literal expression\. |
| [IsNumericType(SpecialType)](IsNumericType/index.md) | Returns true if the specified type is a numeric type\. |
| [IsPredefinedType(SpecialType)](IsPredefinedType/index.md) | Returns true if a syntax of the specified kind is a predefined type\. |
| [IsSimpleType(SpecialType)](IsSimpleType/index.md) | Returns true if a syntax of the specified kind is a simple type\. |
| [IsSwitchLabel(SyntaxKind)](IsSwitchLabel/index.md) | Returns true if a syntax of the specified kind is a switch label\. |
| [SupportsPrefixOrPostfixUnaryOperator(SpecialType)](SupportsPrefixOrPostfixUnaryOperator/index.md) | Returns true if an expression of the specified type can be used in a prefix or postfix unary operator\. |

