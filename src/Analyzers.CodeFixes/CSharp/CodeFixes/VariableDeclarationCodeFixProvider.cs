// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VariableDeclarationCodeFixProvider))]
    [Shared]
    public sealed class VariableDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SplitVariableDeclaration); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out VariableDeclarationSyntax variableDeclaration))
                return;

            CodeAction codeAction = CodeAction.Create(
                SplitVariableDeclarationRefactoring.GetTitle(variableDeclaration),
                ct => SplitVariableDeclarationRefactoring.RefactorAsync(context.Document, variableDeclaration, ct),
                GetEquivalenceKey(DiagnosticIdentifiers.SplitVariableDeclaration));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
