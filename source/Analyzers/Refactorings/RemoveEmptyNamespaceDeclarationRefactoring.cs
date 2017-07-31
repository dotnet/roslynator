// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyNamespaceDeclarationRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, NamespaceDeclarationSyntax declaration)
        {
            if (!declaration.Members.Any())
            {
                SyntaxToken openBrace = declaration.OpenBraceToken;
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!openBrace.IsMissing
                    && !closeBrace.IsMissing
                    && openBrace.TrailingTrivia.IsEmptyOrWhitespace()
                    && closeBrace.LeadingTrivia.IsEmptyOrWhitespace())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveEmptyNamespaceDeclaration,
                        declaration);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            NamespaceDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            return document.RemoveNodeAsync(declaration, GetRemoveOptions(declaration), cancellationToken);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(NamespaceDeclarationSyntax declaration)
        {
            if (declaration.GetLeadingTrivia().IsEmptyOrWhitespace())
            {
                if (declaration.GetTrailingTrivia().IsEmptyOrWhitespace())
                {
                    return SyntaxRemoveOptions.KeepNoTrivia;
                }
                else
                {
                    return SyntaxRemoveOptions.KeepTrailingTrivia;
                }
            }
            else if (declaration.GetTrailingTrivia().IsEmptyOrWhitespace())
            {
                return SyntaxRemoveOptions.KeepLeadingTrivia;
            }
            else
            {
                return SyntaxRemoveOptions.KeepExteriorTrivia;
            }
        }
    }
}
