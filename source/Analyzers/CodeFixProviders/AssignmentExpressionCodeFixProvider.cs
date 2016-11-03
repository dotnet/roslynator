// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssignmentExpressionCodeFixProvider))]
    [Shared]
    public class AssignmentExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.SimplifyAssignmentExpression,
                    DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            AssignmentExpressionSyntax assignment = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AssignmentExpressionSyntax>();

            if (assignment == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyAssignmentExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify assignment expression",
                                cancellationToken =>
                                {
                                    return SimplifyAssignmentExpressionRefactoring.RefactorAsync(
                                        context.Document,
                                        assignment,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment:
                        {
                            SyntaxKind kind = UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.GetPostfixUnaryOperatorKind(assignment);

                            string operatorText = UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.GetOperatorText(kind);

                            CodeAction codeAction = CodeAction.Create(
                                $"Use {operatorText} operator",
                                cancellationToken =>
                                {
                                    return UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.RefactorAsync(
                                        context.Document,
                                        assignment,
                                        kind,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
