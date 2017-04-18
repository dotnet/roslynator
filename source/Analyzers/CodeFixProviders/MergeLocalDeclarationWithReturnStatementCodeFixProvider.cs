// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MergeLocalDeclarationWithReturnStatementCodeFixProvider))]
    [Shared]
    public class MergeLocalDeclarationWithReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            var localDeclaration = (LocalDeclarationStatementSyntax)root.DescendantNodes(context.Span)
                .FirstOrDefault(f => f.IsKind(SyntaxKind.LocalDeclarationStatement) && f.Span.Start == context.Span.Start);

            if (localDeclaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Merge local declaration with return statement",
                cancellationToken => MergeLocalDeclarationWithReturnStatementRefactoring.RefactorAsync(context.Document, localDeclaration, cancellationToken),
                DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
