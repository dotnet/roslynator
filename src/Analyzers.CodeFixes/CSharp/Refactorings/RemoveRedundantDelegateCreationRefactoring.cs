// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantDelegateCreationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = objectCreation
                .ArgumentList
                .Arguments
                .First()
                .Expression;

            IEnumerable<SyntaxTrivia> leadingTrivia = objectCreation
                .DescendantTrivia(TextSpan.FromBounds(objectCreation.FullSpan.Start, expression.SpanStart));

            IEnumerable<SyntaxTrivia> trailingTrivia = objectCreation
                .DescendantTrivia(TextSpan.FromBounds(expression.Span.End, objectCreation.FullSpan.End));

            ExpressionSyntax newExpression = expression
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(objectCreation, newExpression, cancellationToken);
        }
    }
}
