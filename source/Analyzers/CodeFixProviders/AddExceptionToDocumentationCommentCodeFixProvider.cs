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
using Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddExceptionToDocumentationCommentCodeFixProvider))]
    [Shared]
    public class AddExceptionToDocumentationCommentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddExceptionToDocumentationComment); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddExceptionToDocumentationComment:
                        {
                            switch (node.Kind())
                            {
                                case SyntaxKind.ThrowStatement:
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add exception to documentation comment",
                                            cancellationToken => AddExceptionToDocumentationCommentRefactoring.RefactorAsync(context.Document, (ThrowStatementSyntax)node, cancellationToken),
                                            diagnostic.Id + EquivalenceKeySuffix);

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                case SyntaxKind.ThrowExpression:
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add exception to documentation comment",
                                            cancellationToken => AddExceptionToDocumentationCommentRefactoring.RefactorAsync(context.Document, (ThrowExpressionSyntax)node, cancellationToken),
                                            diagnostic.Id + EquivalenceKeySuffix);

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }
    }
}
