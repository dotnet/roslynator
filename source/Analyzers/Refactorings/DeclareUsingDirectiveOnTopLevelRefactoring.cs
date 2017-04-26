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

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            UsingDirectiveSyntax[] newUsings = usings
                .Select(f => CheckUnqualifiedUsingStatic(f, semanticModel, cancellationToken).WithFormatterAnnotation())
                .ToArray();

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives)
                .AddUsings(
                    keepSingleLineCommentsOnTop: true,
                    usings: newUsings);

            return document.WithSyntaxRoot(newCompilationUnit);
        }

        private static UsingDirectiveSyntax CheckUnqualifiedUsingStatic(
            UsingDirectiveSyntax usingDirective,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
            {
                NameSyntax name = usingDirective.Name;

                if (name?.IsKind(SyntaxKind.IdentifierName) == true)
                {
                    ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                    INamespaceSymbol containingNamespace = symbol?.ContainingNamespace;

                    if (containingNamespace != null)
                    {
                        var identifierName = (IdentifierNameSyntax)name;

                        name = SyntaxFactory.QualifiedName(
                            SyntaxFactory.ParseName(containingNamespace.ToString()),
                            identifierName.WithoutTrivia());

                        return usingDirective.WithName(name.WithTriviaFrom(identifierName));
                    }
                }
            }

            return usingDirective;
        }
    }
}
