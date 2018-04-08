// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceExpressionWithConstantValueRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax)
                return;

            if (CSharpUtility.IsStringLiteralConcatenation(expression as BinaryExpressionSyntax))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            Optional<object> optional = semanticModel.GetConstantValue(expression, context.CancellationToken);

            if (!optional.HasValue)
                return;

            SyntaxNode oldNode = expression;

            if (oldNode.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)oldNode.Parent;

                if (memberAccessExpression.Name == expression)
                    oldNode = memberAccessExpression;
            }

            ExpressionSyntax newNode = LiteralExpression(optional.Value);

            context.RegisterRefactoring(
                $"Replace expression with '{newNode}'",
                cancellationToken => context.Document.ReplaceNodeAsync(oldNode, newNode.WithTriviaFrom(expression).Parenthesize(), cancellationToken));
        }
    }
}