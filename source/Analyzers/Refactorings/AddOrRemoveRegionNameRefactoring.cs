// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddOrRemoveRegionNameRefactoring
    {
        internal static void AnalyzeEndRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            var endRegionDirective = (EndRegionDirectiveTriviaSyntax)context.Node;

            RegionDirectiveTriviaSyntax regionDirective = endRegionDirective.GetRegionDirective();

            if (regionDirective != null)
            {
                SyntaxTrivia trivia = regionDirective.GetPreprocessingMessageTrivia();

                SyntaxTrivia endTrivia = endRegionDirective.GetPreprocessingMessageTrivia();

                if (trivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
                {
                    if (!endTrivia.IsKind(SyntaxKind.PreprocessingMessageTrivia)
                        || !string.Equals(trivia.ToString(), endTrivia.ToString(), StringComparison.Ordinal))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AddOrRemoveRegionName,
                            endRegionDirective,
                            "Add", "to");
                    }
                }
                else if (endTrivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddOrRemoveRegionName,
                        endRegionDirective,
                        "Remove", "from");
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            EndRegionDirectiveTriviaSyntax endRegionDirective,
            SyntaxTrivia trivia,
            CancellationToken cancellationToken)
        {
            SyntaxToken endRegionKeyword = endRegionDirective.EndRegionKeyword;

            EndRegionDirectiveTriviaSyntax newNode = endRegionDirective;

            if (trivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
            {
                SyntaxTriviaList trailingTrivia = endRegionKeyword.TrailingTrivia;

                if (trailingTrivia.Any())
                {
                    if (endRegionDirective.HasPreprocessingMessageTrivia())
                        newNode = newNode.WithEndOfDirectiveToken(newNode.EndOfDirectiveToken.WithoutLeadingTrivia());

                    newNode = newNode.WithEndRegionKeyword(endRegionKeyword.WithTrailingTrivia(SyntaxFactory.Space, trivia));
                }
                else
                {
                    newNode = endRegionDirective.Update(
                        endRegionDirective.HashToken,
                        endRegionKeyword.WithTrailingTrivia(SyntaxFactory.Space),
                        endRegionDirective.EndOfDirectiveToken.WithLeadingTrivia(trivia),
                        endRegionDirective.IsActive);
                }
            }
            else
            {
                newNode = endRegionDirective.WithEndOfDirectiveToken(endRegionDirective.EndOfDirectiveToken.WithoutLeadingTrivia());
            }

            return document.ReplaceNodeAsync(endRegionDirective, newNode, cancellationToken);
        }
    }
}
