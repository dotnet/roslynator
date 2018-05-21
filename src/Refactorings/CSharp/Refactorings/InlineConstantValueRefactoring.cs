// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineConstantValueRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax)
                return;

            if (CSharpUtility.IsStringLiteralConcatenation(expression as BinaryExpressionSyntax))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var optional = default(Optional<object>);

            ISymbol symbol = semanticModel.GetSymbol(expression, context.CancellationToken);

            if (symbol?.IsErrorType() != false)
                return;

            if (symbol is IFieldSymbol fieldSymbol)
            {
                if (!fieldSymbol.HasConstantValue)
                    return;

                if (fieldSymbol.ContainingType?.TypeKind == TypeKind.Enum)
                    return;
            }
            else if (symbol is ILocalSymbol localSymbol)
            {
                if (!localSymbol.HasConstantValue)
                    return;
            }
            else
            {
                optional = semanticModel.GetConstantValue(expression, context.CancellationToken);

                if (!optional.HasValue)
                    return;
            }

            context.RegisterRefactoring(
                "Inline constant value",
                ct => RefactorAsync(context.Document, expression, symbol, optional, semanticModel, ct),
                RefactoringIdentifiers.InlineConstantValue);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ISymbol symbol,
            Optional<object> optional,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            LiteralExpressionSyntax literalExpression = (optional.HasValue)
                ? LiteralExpression(optional.Value)
                : GetLiteralExpression(symbol, semanticModel, cancellationToken);

            SyntaxNode oldNode = expression;

            if (oldNode.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)oldNode.Parent;

                if (memberAccessExpression.Name == expression)
                    oldNode = memberAccessExpression;
            }

            return document.ReplaceNodeAsync(oldNode, literalExpression.WithTriviaFrom(expression).Parenthesize(), cancellationToken);
        }

        private static LiteralExpressionSyntax GetLiteralExpression(ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (symbol is IFieldSymbol fieldSymbol)
            {
                while (fieldSymbol.GetSyntaxOrDefault(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
                {
                    ExpressionSyntax value = variableDeclarator.Initializer.Value;

                    if (value.WalkDownParentheses() is LiteralExpressionSyntax literalExpression)
                        return literalExpression;

                    if (semanticModel.SyntaxTree != value.SyntaxTree)
                        semanticModel = semanticModel.Compilation.GetSemanticModel(value.SyntaxTree);

                    if (semanticModel.GetSymbol(value, cancellationToken) is IFieldSymbol fieldSymbol2)
                    {
                        fieldSymbol = fieldSymbol2;
                    }
                    else
                    {
                        break;
                    }
                }

                return LiteralExpression(fieldSymbol.ConstantValue);
            }
            else
            {
                var localSymbol = (ILocalSymbol)symbol;

                while (localSymbol.GetSyntaxOrDefault(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
                {
                    ExpressionSyntax value = variableDeclarator.Initializer.Value;

                    if (value.WalkDownParentheses() is LiteralExpressionSyntax literalExpression)
                        return literalExpression;

                    if (semanticModel.GetSymbol(value, cancellationToken) is ILocalSymbol localSymbol2)
                    {
                        localSymbol = localSymbol2;
                    }
                    else
                    {
                        break;
                    }
                }

                return LiteralExpression(localSymbol.ConstantValue);
            }
        }
    }
}