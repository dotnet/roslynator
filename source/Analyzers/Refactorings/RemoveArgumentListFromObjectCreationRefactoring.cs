// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveArgumentListFromObjectCreationRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreationExpression)
        {
            if (objectCreationExpression.Type?.IsMissing == false
                && objectCreationExpression.Initializer?.IsMissing == false)
            {
                ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;

                if (argumentList?.Arguments.Any() == false)
                {
                    SyntaxToken openParen = argumentList.OpenParenToken;
                    SyntaxToken closeParen = argumentList.CloseParenToken;

                    if (!openParen.IsMissing
                        && !closeParen.IsMissing
                        && openParen.TrailingTrivia.IsEmptyOrWhitespace()
                        && closeParen.LeadingTrivia.IsEmptyOrWhitespace())
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.RemoveArgumentListFromObjectCreation, argumentList);
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            return document.RemoveNodeAsync(argumentList, SyntaxRemoveOptions.KeepExteriorTrivia, cancellationToken);
        }
    }
}
