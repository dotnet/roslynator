// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareUsingDirectiveOnTopLevelRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, NamespaceDeclarationSyntax declaration)
        {
            SyntaxList<UsingDirectiveSyntax> usings = declaration.Usings;

            if (usings.Any())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.DeclareUsingDirectiveOnTopLevel,
                    Location.Create(declaration.SyntaxTree, usings.Span));
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            NamespaceDeclarationSyntax namespaceDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives)
                .AddUsings(
                    keepSingleLineCommentsOnTop: true,
                    usings: usings.Select(f => f.WithFormatterAnnotation()).ToArray());

            return document.WithSyntaxRoot(newCompilationUnit);
        }
    }
}
