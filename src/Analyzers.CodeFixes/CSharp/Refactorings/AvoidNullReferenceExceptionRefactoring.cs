// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidNullReferenceExceptionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            var textChange = new TextChange(new TextSpan(expression.Span.End, 0), "?");

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }
    }
}
