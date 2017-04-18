// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LockStatementCodeFixProvider))]
    [Shared]
    public class LockStatementCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            LockStatementSyntax lockStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LockStatementSyntax>();

            if (lockStatement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Introduce field to lock on",
                cancellationToken => AvoidLockingOnPubliclyAccessibleInstanceRefactoring.RefactorAsync(context.Document, lockStatement, cancellationToken),
                DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
