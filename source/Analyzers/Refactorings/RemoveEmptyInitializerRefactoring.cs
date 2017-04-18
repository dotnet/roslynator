// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyInitializerRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreationExpression)
        {
            TypeSyntax type = objectCreationExpression.Type;
            InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

            if (type?.IsMissing == false
                && initializer?.Expressions.Any() == false
                && initializer.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && initializer.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEmptyInitializer,
                    initializer);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

            ObjectCreationExpressionSyntax newNode = objectCreationExpression
                .WithInitializer(null);

            if (newNode.ArgumentList == null)
            {
                newNode = newNode
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList()
                            .WithTrailingTrivia(objectCreationExpression.Type.GetTrailingTrivia()));
            }

            newNode = newNode
                .WithTriviaFrom(objectCreationExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(objectCreationExpression, newNode, cancellationToken);
        }
    }
}
