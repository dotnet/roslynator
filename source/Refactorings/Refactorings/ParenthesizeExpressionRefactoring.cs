// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParenthesizeExpressionRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, ExpressionSyntax expression)
        {
            if (!CSharpUtility.AreParenthesesRedundantOrInvalid(expression)
                && !expression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                try
                {
                    Refactor(context.Root, expression);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debug.Assert(false, $"{nameof(ParenthesizeExpressionRefactoring)}\r\n{expression.Kind()}");
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = Refactor(root, expression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode Refactor(SyntaxNode root, ExpressionSyntax expression)
        {
            return root.ReplaceNode(
                expression,
                expression.Parenthesize(cutCopyTrivia: true));
        }
    }
}
