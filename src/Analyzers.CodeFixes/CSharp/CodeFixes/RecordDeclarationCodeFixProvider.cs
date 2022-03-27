// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RecordDeclarationCodeFixProvider))]
    [Shared]
    public class RecordDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveUnnecessaryBraces); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out RecordDeclarationSyntax recordDeclaration))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Remove unnecessary braces",
                ct =>
                {
                    RecordDeclarationSyntax newRecordDeclaration = recordDeclaration.Update(
                        recordDeclaration.AttributeLists,
                        recordDeclaration.Modifiers,
                        recordDeclaration.Keyword,
                        recordDeclaration.Identifier,
                        recordDeclaration.TypeParameterList,
                        recordDeclaration.ParameterList.WithoutTrailingTrivia(),
                        recordDeclaration.BaseList,
                        recordDeclaration.ConstraintClauses,
                        default,
                        default,
                        default,
                        Token(SyntaxKind.SemicolonToken));

                    return document.ReplaceNodeAsync(recordDeclaration, newRecordDeclaration, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
