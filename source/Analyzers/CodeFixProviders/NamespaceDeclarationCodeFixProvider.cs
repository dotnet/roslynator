// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceDeclarationCodeFixProvider))]
    [Shared]
    public class NamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration,
                    DiagnosticIdentifiers.AvoidUsageOfUsingDirectiveInsideNamespaceDeclaration);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            NamespaceDeclarationSyntax namespaceDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            Debug.Assert(namespaceDeclaration != null, $"{nameof(namespaceDeclaration)} is null");

            if (namespaceDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove empty namespace declaration",
                                cancellationToken => RemoveEmptyNamespaceDeclarationAsync(context.Document, namespaceDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidUsageOfUsingDirectiveInsideNamespaceDeclaration:
                        {
                            string title = (namespaceDeclaration.Usings.Count == 1)
                                ? "Declare using on document level"
                                : "Declare usings on document level";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => MoveUsingDirectivesOnDocumentLevelAsync(context.Document, namespaceDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> RemoveEmptyNamespaceDeclarationAsync(
            Document document,
            NamespaceDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.RemoveNode(declaration, GetRemoveOptions(declaration));

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(NamespaceDeclarationSyntax declaration)
        {
            if (declaration.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                if (declaration.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    return SyntaxRemoveOptions.KeepNoTrivia;
                }
                else
                {
                    return SyntaxRemoveOptions.KeepTrailingTrivia;
                }
            }
            else if (declaration.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return SyntaxRemoveOptions.KeepLeadingTrivia;
            }
            else
            {
                return SyntaxRemoveOptions.KeepExteriorTrivia;
            }
        }

        private async Task<Document> MoveUsingDirectivesOnDocumentLevelAsync(
            Document document,
            NamespaceDeclarationSyntax namespaceDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            if (!compilationUnit.Usings.Any())
            {
                SyntaxTriviaList leadingTrivia = compilationUnit.GetLeadingTrivia();

                SyntaxTrivia[] topTrivia = GetTopSingleLineComments(leadingTrivia).ToArray();

                if (topTrivia.Length > 0)
                {
                    newCompilationUnit = newCompilationUnit.WithoutLeadingTrivia();

                    usings = usings.Replace(
                        usings.First(),
                        usings.First().WithLeadingTrivia(topTrivia));

                    usings = usings.Replace(
                        usings.Last(),
                        usings.Last().WithTrailingTrivia(leadingTrivia.Skip(topTrivia.Length)));
                }
            }

            newCompilationUnit = newCompilationUnit.AddUsings(usings.Select(f => f.WithFormatterAnnotation()));

            return document.WithSyntaxRoot(newCompilationUnit);
        }

        private IEnumerable<SyntaxTrivia> GetTopSingleLineComments(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.IsSingleLineCommentTrivia())
                {
                    SyntaxTrivia trivia = en.Current;

                    if (en.MoveNext() && en.Current.IsEndOfLineTrivia())
                    {
                        yield return trivia;
                        yield return en.Current;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
