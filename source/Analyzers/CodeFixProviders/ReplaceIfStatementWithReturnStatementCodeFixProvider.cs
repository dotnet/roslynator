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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceIfStatementWithReturnStatementCodeFixProvider))]
    [Shared]
    public class ReplaceIfStatementWithReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var ifStatement = (IfStatementSyntax)root.DescendantNodes(context.Span)
                .FirstOrDefault(f => f.IsKind(SyntaxKind.IfStatement) && context.Span.Contains(f.Span));

            if (ifStatement == null)
                return;

            ReturnStatementSyntax returnStatement = ReplaceIfStatementWithReturnStatementRefactoring.CreateReturnStatement(ifStatement);

            CodeAction codeAction = CodeAction.Create(
                $"Replace if with '{returnStatement}'",
                cancellationToken => ReplaceIfStatementWithReturnStatementRefactoring.RefactorAsync(context.Document, ifStatement, returnStatement, cancellationToken),
                DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
