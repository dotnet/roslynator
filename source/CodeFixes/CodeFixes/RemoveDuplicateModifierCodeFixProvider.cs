// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveDuplicateModifierCodeFixProvider))]
    [Shared]
    public class RemoveDuplicateModifierCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.DuplicateModifier); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveDuplicateModifier))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxToken token = root.FindToken(context.Span.Start);

            Debug.Assert(!token.IsKind(SyntaxKind.None), $"{nameof(token)} is none");

            if (token.IsKind(SyntaxKind.None))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.DuplicateModifier:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove '{token}' modifier",
                                cancellationToken => context.Document.RemoveModifierAsync(token.Parent, token, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
