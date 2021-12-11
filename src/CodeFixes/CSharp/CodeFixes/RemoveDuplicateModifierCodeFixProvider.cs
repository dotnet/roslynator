// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveDuplicateModifierCodeFixProvider))]
    [Shared]
    public sealed class RemoveDuplicateModifierCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1004_DuplicateModifier); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveDuplicateModifier, context.Document, root.SyntaxTree))
                return;

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            ModifiersCodeFixRegistrator.RemoveModifier(
                context,
                diagnostic,
                token.Parent,
                token,
                additionalKey: CodeFixIdentifiers.RemoveDuplicateModifier);
        }
    }
}
