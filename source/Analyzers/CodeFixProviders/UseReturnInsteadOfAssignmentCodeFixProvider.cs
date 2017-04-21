// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseReturnInsteadOfAssignmentCodeFixProvider))]
    [Shared]
    public class UseReturnInsteadOfAssignmentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseReturnInsteadOfAssignment); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var statement = (StatementSyntax)root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.IfStatement, SyntaxKind.SwitchStatement));

            Debug.Assert(statement != null, $"{nameof(statement)} is null");

            if (statement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use return instead of assignment",
                cancellationToken =>
                {
                    return UseReturnInsteadOfAssignmentRefactoring.RefactorAsync(
                        context.Document,
                        statement,
                        cancellationToken);
                },
                DiagnosticIdentifiers.UseReturnInsteadOfAssignment + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
