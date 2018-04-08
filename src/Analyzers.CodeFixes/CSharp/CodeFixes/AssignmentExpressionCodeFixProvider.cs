// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
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
                    DiagnosticIdentifiers.UseCompoundAssignment,
                    DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment,
                    DiagnosticIdentifiers.RemoveRedundantDelegateCreation,
                    DiagnosticIdentifiers.RemoveRedundantAssignment);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AssignmentExpressionSyntax assignment))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.UseCompoundAssignment:
                        {
                            var binaryExpression = (BinaryExpressionSyntax)assignment.Right;

                            string operatorText = UseCompoundAssignmentAnalysis.GetCompoundOperatorText(binaryExpression);

                            CodeAction codeAction = CodeAction.Create(
                                $"Use {operatorText} operator",
                                cancellationToken => UseCompoundAssignmentRefactoring.RefactorAsync(context.Document, assignment, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment:
                        {
                            string operatorText = UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.GetOperatorText(assignment);

                            CodeAction codeAction = CodeAction.Create(
                                $"Use {operatorText} operator",
                                c => UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.RefactorAsync(context.Document, assignment, c),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantDelegateCreation:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant delegate creation",
                                cancellationToken =>
                                {
                                    return RemoveRedundantDelegateCreationRefactoring.RefactorAsync(
                                        context.Document,
                                        (ObjectCreationExpressionSyntax)assignment.Right,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantAssignment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant assignment",
                                cancellationToken =>
                                {
                                    return RemoveRedundantAssignmentRefactoring.RefactorAsync(
                                        context.Document,
                                        assignment,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
