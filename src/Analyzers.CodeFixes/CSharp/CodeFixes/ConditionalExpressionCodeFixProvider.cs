// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeActions;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConditionalExpressionCodeFixProvider))]
    [Shared]
    public class ConditionalExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.ParenthesizeConditionInConditionalExpression,
                    DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfConditionalExpression,
                    DiagnosticIdentifiers.SimplifyConditionalExpression,
                    DiagnosticIdentifiers.FormatConditionalExpression,
                    DiagnosticIdentifiers.UseConditionalAccessInsteadOfConditionalExpression,
                    DiagnosticIdentifiers.AvoidNestedConditionalOperators);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ConditionalExpressionSyntax conditionalExpression))
                return;

            Document document = context.Document;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.ParenthesizeConditionInConditionalExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Wrap condition in parentheses",
                                cancellationToken => ParenthesizeConditionInConditionalExpressionRefactoring.RefactorAsync(document, conditionalExpression, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfConditionalExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use coalesce expression",
                                cancellationToken =>
                                {
                                    return SimplifyNullCheckRefactoring.RefactorAsync(
                                        document,
                                        conditionalExpression,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.SimplifyConditionalExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify conditional expression",
                                cancellationToken =>
                                {
                                    return SimplifyConditionalExpressionRefactoring.RefactorAsync(
                                        document,
                                        conditionalExpression,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.FormatConditionalExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format ? and : on next line",
                                cancellationToken =>
                                {
                                    return FormatConditionalExpressionRefactoring.RefactorAsync(
                                        document,
                                        conditionalExpression,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseConditionalAccessInsteadOfConditionalExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use conditional access",
                                cancellationToken =>
                                {
                                    return SimplifyNullCheckRefactoring.RefactorAsync(
                                        document,
                                        conditionalExpression,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidNestedConditionalOperators:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            (CodeAction codeAction, CodeAction recursiveCodeAction) = ConvertConditionalOperatorToIfElseRefactoring.ComputeRefactoring(
                                document,
                                conditionalExpression,
                                data: default,
                                recursiveData: new CodeActionData(ConvertConditionalOperatorToIfElseRefactoring.Title, GetEquivalenceKey(diagnostic)),
                                semanticModel,
                                context.CancellationToken);

                            if (recursiveCodeAction != null)
                                context.RegisterCodeFix(recursiveCodeAction, diagnostic);

                            break;
                        }
                }
            }
        }
    }
}
