// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class NegateOperatorRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxToken token)
        {
            switch (token.Kind())
            {
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                    {
                        context.RegisterRefactoring(
                            "Negate operator",
                            cancellationToken => NegateOperatorAsync(context.Document, token, cancellationToken));

                        break;
                    }
            }
        }

        private static async Task<Document> NegateOperatorAsync(
            Document document,
            SyntaxToken operatorToken,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.ReplaceToken(
                operatorToken,
                SyntaxFactory.Token(GetNegatedOperatorKind(operatorToken)).WithTriviaFrom(operatorToken));

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxKind GetNegatedOperatorKind(SyntaxToken operatorToken)
        {
            switch (operatorToken.Kind())
            {
                case SyntaxKind.AmpersandAmpersandToken:
                    return SyntaxKind.BarBarToken;
                case SyntaxKind.BarBarToken:
                    return SyntaxKind.AmpersandAmpersandToken;
                case SyntaxKind.EqualsEqualsToken:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.ExclamationEqualsToken:
                    return SyntaxKind.EqualsEqualsToken;
                case SyntaxKind.GreaterThanToken:
                    return SyntaxKind.LessThanEqualsToken;
                case SyntaxKind.GreaterThanEqualsToken:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.LessThanToken:
                    return SyntaxKind.GreaterThanEqualsToken;
                case SyntaxKind.LessThanEqualsToken:
                    return SyntaxKind.GreaterThanToken;
                default:
                    {
                        Debug.Assert(false, operatorToken.Kind().ToString());
                        return operatorToken.Kind();
                    }
            }
        }
    }
}