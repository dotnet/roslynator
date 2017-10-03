// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringContainsWithStringIndexOfRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            MethodInfo info;
            if (semanticModel.TryGetMethodInfo(invocation, out info, context.CancellationToken)
                && info.IsName("Contains")
                && info.IsContainingType(SpecialType.System_String)
                && info.Symbol.Parameters.SingleOrDefault(throwException: false)?.Type.IsString() == true)
            {
                context.RegisterRefactoring(
                    "Replace Contains with IndexOf",
                    cancellationToken => RefactorAsync(context.Document, invocation, context.CancellationToken));
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            InvocationExpressionSyntax newInvocationExpression = invocation
                .WithExpression(memberAccess.WithName(IdentifierName("IndexOf")))
                .AddArgumentListArguments(
                    Argument(
                        ParseName("System.StringComparison.OrdinalIgnoreCase").WithSimplifierAnnotation()));

            SyntaxNode parent = invocation.Parent;

            if (parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                BinaryExpressionSyntax equalsExpression = EqualsExpression(newInvocationExpression, NumericLiteralExpression(-1))
                    .WithTriviaFrom(parent)
                    .WithFormatterAnnotation();

                return document.ReplaceNodeAsync(parent, equalsExpression, cancellationToken);
            }
            else
            {
                BinaryExpressionSyntax notEqualsExpression = NotEqualsExpression(newInvocationExpression, NumericLiteralExpression(-1))
                    .WithTriviaFrom(invocation)
                    .WithFormatterAnnotation();

                return document.ReplaceNodeAsync(invocation, notEqualsExpression, cancellationToken);
            }
        }
    }
}