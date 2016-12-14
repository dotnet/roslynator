// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class ReplaceAnonymousMethodWithLambdaExpressionRefactoring
    {
        public static bool CanRefactor(AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (anonymousMethod == null)
                throw new ArgumentNullException(nameof(anonymousMethod));

            return anonymousMethod.ParameterList?.IsMissing == false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AnonymousMethodExpressionSyntax anonymousMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (anonymousMethod == null)
                throw new ArgumentNullException(nameof(anonymousMethod));

            LambdaExpressionSyntax lambda = ParenthesizedLambdaExpression(
                anonymousMethod.AsyncKeyword,
                anonymousMethod.ParameterList,
                Token(SyntaxKind.EqualsGreaterThanToken),
                anonymousMethod.Block);

            lambda = lambda
                .WithTriviaFrom(anonymousMethod)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(anonymousMethod, lambda, cancellationToken).ConfigureAwait(false);
        }
    }
}
