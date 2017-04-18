// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BinaryExpressionCodeFixProvider))]
    [Shared]
    public class BinaryExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.SimplifyBooleanComparison,
                    DiagnosticIdentifiers.UseAnyMethodInsteadOfCountMethod,
                    DiagnosticIdentifiers.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression,
                    DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod,
                    DiagnosticIdentifiers.SimplifyCoalesceExpression,
                    DiagnosticIdentifiers.RemoveRedundantAsOperator,
                    DiagnosticIdentifiers.UseStringLengthInsteadOfComparisonWithEmptyString,
                    DiagnosticIdentifiers.UnconstrainedTypeParameterCheckedForNull,
                    DiagnosticIdentifiers.ValueTypeCheckedForNull,
                    DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            BinaryExpressionSyntax binaryExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            if (binaryExpression == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyBooleanComparison:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify boolean comparison",
                                cancellationToken => SimplifyBooleanComparisonRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case DiagnosticIdentifiers.UseAnyMethodInsteadOfCountMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Call 'Any' instead of 'Count'",
                                cancellationToken => UseAnyMethodInsteadOfCountMethodRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Swap '{binaryExpression.Left}' and '{binaryExpression.Right}'",
                                cancellationToken => SwapExpressionsInBinaryExpressionRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use 'string.IsNullOrEmpty' method",
                                cancellationToken => UseStringIsNullOrEmptyMethodRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.SimplifyCoalesceExpression:
                        {
                            ExpressionSyntax expression = binaryExpression.Left;

                            if (expression == null
                                || !context.Span.Contains(expression.Span))
                            {
                                expression = binaryExpression.Right;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Simplify coalesce expression",
                                cancellationToken => SimplifyCoalesceExpressionRefactoring.RefactorAsync(context.Document, binaryExpression, expression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantAsOperator:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant 'as' operator",
                                cancellationToken => RemoveRedundantAsOperatorRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseStringLengthInsteadOfComparisonWithEmptyString:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use string.Length",
                                cancellationToken => UseStringLengthInsteadOfComparisonWithEmptyStringRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UnconstrainedTypeParameterCheckedForNull:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(binaryExpression.Left, context.CancellationToken);

                            CodeAction codeAction = CodeAction.Create(
                                $"Use EqualityComparer<{typeSymbol.Name}>.Default",
                                cancellationToken => UnconstrainedTypeParameterCheckedForNullRefactoring.RefactorAsync(context.Document, binaryExpression, typeSymbol, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ValueTypeCheckedForNull:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(binaryExpression.Left, context.CancellationToken);

                            string title = null;

                            if (typeSymbol.IsPredefinedValueType()
                                || typeSymbol.ExistsMethod(WellKnownMemberNames.EqualityOperatorName))
                            {
                                ExpressionSyntax expression = typeSymbol.ToDefaultValueSyntax(semanticModel, binaryExpression.Right.SpanStart);

                                title = $"Replace 'null' with '{expression}'";
                            }
                            else
                            {
                                title = $"Use EqualityComparer<{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, binaryExpression.Right.SpanStart)}>.Default";
                            }

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => ValueTypeCheckedForNullRefactoring.RefactorAsync(context.Document, binaryExpression, typeSymbol, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use is operator",
                                cancellationToken => UseIsOperatorInsteadOfAsOperatorRefactoring.
                                RefactorAsync(context.Document, binaryExpression, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}