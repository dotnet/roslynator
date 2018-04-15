// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddEmptyLineBetweenDeclarationsCodeFixProvider))]
    [Shared]
    public class AddEmptyLineBetweenDeclarationsCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Add empty line";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxToken token = root.FindToken(context.Span.Start);

            if (token.IsKind(SyntaxKind.CommaToken))
            {
                CodeAction codeAction = CodeAction.Create(
                    Title,
                    cancellationToken =>
                    {
                        SyntaxToken newToken = token.AppendToTrailingTrivia(CSharpFactory.NewLine());

                        return context.Document.ReplaceTokenAsync(token, newToken, cancellationToken);
                    },
                    base.GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                    return;

                CodeAction codeAction = CodeAction.Create(
                    Title,
                    cancellationToken =>
                    {
                        MemberDeclarationSyntax newMemberDeclaration = memberDeclaration.AppendToTrailingTrivia(CSharpFactory.NewLine());

                        return context.Document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
                    },
                    base.GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
