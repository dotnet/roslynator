// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareTypeInsideNamespaceCodeFixProvider))]
    [Shared]
    public class DeclareTypeInsideNamespaceCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareTypeInsideNamespace); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxToken identifier = root.FindToken(context.Span.Start);

            Debug.Assert(!identifier.IsKind(SyntaxKind.None), identifier.Kind().ToString());

            CodeAction codeAction = CodeAction.Create(
                $"Declare '{identifier.ValueText}' inside namespace",
                cancellationToken => RefactorAsync(context.Document, (MemberDeclarationSyntax)identifier.Parent, cancellationToken),
                DiagnosticIdentifiers.DeclareTypeInsideNamespace + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (member.IsParentKind(SyntaxKind.CompilationUnit))
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                string name = SyntaxUtility.GetUniqueName("Namespace", semanticModel, member.Span.Start);

                NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(
                    IdentifierName(Identifier(name).WithRenameAnnotation()),
                    default(SyntaxList<ExternAliasDirectiveSyntax>),
                    default(SyntaxList<UsingDirectiveSyntax>),
                    SingletonList(member));

                SyntaxNode newRoot = root.ReplaceNode(member, namespaceDeclaration.WithFormatterAnnotation());

                return document.WithSyntaxRoot(newRoot);
            }

            return document;
        }
    }
}