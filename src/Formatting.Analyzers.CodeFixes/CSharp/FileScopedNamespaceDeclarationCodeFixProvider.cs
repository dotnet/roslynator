// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileScopedNamespaceDeclarationCodeFixProvider))]
    [Shared]
    public sealed class FileScopedNamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out FileScopedNamespaceDeclarationSyntax fileScopedNamespace))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddEmptyLineAfterFileScopedNamespace:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add empty line",
                            ct =>
                            {
                                MemberDeclarationSyntax member = fileScopedNamespace.Members[0];

                                MemberDeclarationSyntax newMember;
                                if (!fileScopedNamespace.SemicolonToken.TrailingTrivia.Contains(SyntaxKind.EndOfLineTrivia))
                                {
                                    newMember = member.PrependToLeadingTrivia(new SyntaxTrivia[] { CSharpFactory.NewLine(), CSharpFactory.NewLine() });
                                }
                                else
                                {
                                    newMember = member.PrependEndOfLineToLeadingTrivia();
                                }

                                return document.ReplaceNodeAsync(member, newMember, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
