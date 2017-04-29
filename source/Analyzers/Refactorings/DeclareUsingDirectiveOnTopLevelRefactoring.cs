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
                for (int i = 0; i < usings.Count; i++)
                {
                    if (usings[i].ContainsDiagnostics)
                        return;

                    if (i == 0)
                    {
                        if (usings[i].SpanOrTrailingTriviaContainsDirectives())
                            return;
                    }
                    else if (i == usings.Count - 1)
                    {
                        if (usings[i].SpanOrLeadingTriviaContainsDirectives())
                            return;
                    }
                    else if (usings[i].ContainsDirectives)
                    {
                        return;
                    }
                }

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

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            UsingDirectiveSyntax[] newUsings = usings
                .Select(f => EnsureFullyQualifiedName(f, semanticModel, cancellationToken))
                .ToArray();

            newUsings[0] = newUsings[0].WithoutLeadingTrivia();
            newUsings[newUsings.Length - 1] = newUsings[newUsings.Length - 1].WithoutTrailingTrivia();

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives)
                .AddUsings(
                    keepSingleLineCommentsOnTop: true,
                    usings: newUsings);

            return document.WithSyntaxRoot(newCompilationUnit);
        }

        private static UsingDirectiveSyntax EnsureFullyQualifiedName(UsingDirectiveSyntax usingDirective, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            NameSyntax name = usingDirective.Name;

            NameSyntax newName = CSharpUtility.EnsureFullyQualifiedName(name, semanticModel, cancellationToken);

            newName = newName.WithTriviaFrom(name);

            return usingDirective.WithName(newName).WithFormatterAnnotation();
        }
    }
}
