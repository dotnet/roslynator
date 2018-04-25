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

            LiteralExpressionSyntax literalExpression = null;

            var optional = default(Optional<object>);

            CancellationToken cancellationToken = context.CancellationToken;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol?.IsErrorType() != false)
                return;

            if (symbol is IFieldSymbol fieldSymbol)
            {
                if (!fieldSymbol.HasConstantValue)
                    return;

                if (fieldSymbol.ContainingType?.TypeKind == TypeKind.Enum)
                    return;

                optional = new Optional<object>(fieldSymbol.ConstantValue);

                while (fieldSymbol.GetSyntaxOrDefault(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
                {
                    ExpressionSyntax value = variableDeclarator.Initializer.Value;

                    literalExpression = value.WalkDownParentheses() as LiteralExpressionSyntax;

                    if (literalExpression != null)
                        break;

                    fieldSymbol = semanticModel.GetSymbol(value, cancellationToken) as IFieldSymbol;

                    if (fieldSymbol == null)
                        break;
                }
            }
            else if (symbol is ILocalSymbol localSymbol)
            {
                if (!localSymbol.HasConstantValue)
                    return;

                optional = new Optional<object>(localSymbol.ConstantValue);

                while (localSymbol.GetSyntaxOrDefault(cancellationToken) is VariableDeclaratorSyntax variableDeclarator)
                {
                    ExpressionSyntax value = variableDeclarator.Initializer.Value;

                    literalExpression = value.WalkDownParentheses() as LiteralExpressionSyntax;

                    if (literalExpression != null)
                        break;

                    localSymbol = semanticModel.GetSymbol(value, cancellationToken) as ILocalSymbol;

                    if (localSymbol == null)
                        break;
                }
            }
            else
            {
                optional = semanticModel.GetConstantValue(expression, cancellationToken);
            }

            if (literalExpression == null)
            {
                if (!optional.HasValue)
                    return;

                literalExpression = LiteralExpression(optional.Value);
            }

            context.RegisterRefactoring(
                "Inline constant value",
                ct =>
                {
                    SyntaxNode oldNode = expression;

                    if (oldNode.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        var memberAccessExpression = (MemberAccessExpressionSyntax)oldNode.Parent;

                        if (memberAccessExpression.Name == expression)
                            oldNode = memberAccessExpression;
                    }

                    return context.Document.ReplaceNodeAsync(oldNode, literalExpression.WithTriviaFrom(expression).Parenthesize(), ct);
                },
                RefactoringIdentifiers.InlineConstantValue);
        }
    }
}