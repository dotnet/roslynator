// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxTokenCodeFixProvider))]
    [Shared]
    public sealed class SyntaxTokenCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddBlankLineBetweenClosingBraceAndNextStatement,
                    DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator,
                    DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken,
                    DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeEqualsToken,
                    DiagnosticIdentifiers.PutAttributeListOnItsOwnLine,
                    DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddBlankLineBetweenClosingBraceAndNextStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct => CodeFixHelpers.AppendEndOfLineAsync(document, token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator:
                    {
                        if (DiagnosticProperties.ContainsInvert(diagnostic.Properties))
                        {
                            var conditionalExpression = (ConditionalExpressionSyntax)token.Parent;

                            string title = null;
                            if (token.IsKind(SyntaxKind.QuestionToken))
                            {
                                title = (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(conditionalExpression.WhenTrue, conditionalExpression.ColonToken, conditionalExpression.WhenFalse))
                                    ? "Place new line after '?' and ':'"
                                    : "Place new line after '?'";
                            }
                            else
                            {
                                title = "Place new line after ':'";
                            }

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                ct => AddNewLineAfterConditionalOperatorInsteadOfBeforeItAsync(document, conditionalExpression, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else
                        {
                            var conditionalExpression = (ConditionalExpressionSyntax)token.Parent;

                            string title = null;
                            if (token.IsKind(SyntaxKind.QuestionToken))
                            {
                                title = (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(conditionalExpression.WhenTrue, conditionalExpression.ColonToken, conditionalExpression.WhenFalse))
                                    ? "Place new line before '?' and ':'"
                                    : "Place new line before '?'";
                            }
                            else
                            {
                                title = "Place new line before ':'";
                            }

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                ct => AddNewLineBeforeConditionalOperatorInsteadOfAfterItAsync(document, conditionalExpression, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }

                        break;
                    }
                case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken:
                case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeEqualsToken:
                    {
                        AddNewLineBeforeOrAfter();
                        break;
                    }
                case DiagnosticIdentifiers.PutAttributeListOnItsOwnLine:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Put attribute on its own line",
                            ct => CodeFixHelpers.AddNewLineBeforeAsync(document, token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement:
                    {
                        if (DiagnosticProperties.ContainsInvert(diagnostic.Properties))
                        {
                            CodeAction codeAction = CodeAction.Create(
                                CodeFixTitles.RemoveNewLine,
                                ct => CodeFixHelpers.ReplaceTriviaBetweenAsync(document, token, token.GetNextToken(), cancellationToken: ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else
                        {
                            CodeAction codeAction = CodeAction.Create(
                                CodeFixTitles.AddNewLine,
                                ct => CodeFixHelpers.AddNewLineBeforeAsync(document, token, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }

                        break;
                    }
            }

            void AddNewLineBeforeOrAfter()
            {
                if (DiagnosticProperties.ContainsInvert(diagnostic.Properties))
                {
                    CodeAction codeAction = CodeAction.Create(
                        $"Place new line after '{token}'",
                        ct => CodeFixHelpers.AddNewLineAfterInsteadOfBeforeAsync(document, token, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
                else
                {
                    CodeAction codeAction = CodeAction.Create(
                        $"Place new line before '{token}'",
                        ct => CodeFixHelpers.AddNewLineBeforeInsteadOfAfterAsync(document, token, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
            }
        }

        private static Task<Document> AddNewLineBeforeConditionalOperatorInsteadOfAfterItAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;
            SyntaxToken questionToken = conditionalExpression.QuestionToken;
            SyntaxToken colonToken = conditionalExpression.ColonToken;

            ExpressionSyntax newCondition = condition;
            ExpressionSyntax newWhenTrue = whenTrue;
            ExpressionSyntax newWhenFalse = whenFalse;
            SyntaxToken newQuestionToken = questionToken;
            SyntaxToken newColonToken = colonToken;

            if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(condition, questionToken, whenTrue))
            {
                (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) = CodeFixHelpers.AddNewLineBeforeTokenInsteadOfAfterIt(condition, questionToken, whenTrue);

                newCondition = left;
                newQuestionToken = token;
                newWhenTrue = right;
            }

            if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(whenTrue, colonToken, whenFalse))
            {
                (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) = CodeFixHelpers.AddNewLineBeforeTokenInsteadOfAfterIt(newWhenTrue, colonToken, whenFalse);

                newWhenTrue = left;
                newColonToken = token;
                newWhenFalse = right;
            }

            ConditionalExpressionSyntax newConditionalExpression = ConditionalExpression(
                newCondition,
                newQuestionToken,
                newWhenTrue,
                newColonToken,
                newWhenFalse);

            return document.ReplaceNodeAsync(conditionalExpression, newConditionalExpression, cancellationToken);
        }

        private static Task<Document> AddNewLineAfterConditionalOperatorInsteadOfBeforeItAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;
            SyntaxToken questionToken = conditionalExpression.QuestionToken;
            SyntaxToken colonToken = conditionalExpression.ColonToken;

            ExpressionSyntax newCondition = condition;
            ExpressionSyntax newWhenTrue = whenTrue;
            ExpressionSyntax newWhenFalse = whenFalse;
            SyntaxToken newQuestionToken = questionToken;
            SyntaxToken newColonToken = colonToken;

            if (SyntaxTriviaAnalysis.IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(condition, questionToken, whenTrue))
            {
                (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) = CodeFixHelpers.AddNewLineAfterTokenInsteadOfBeforeIt(condition, questionToken, whenTrue);

                newCondition = left;
                newQuestionToken = token;
                newWhenTrue = right;
            }

            if (SyntaxTriviaAnalysis.IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(whenTrue, colonToken, whenFalse))
            {
                (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) = CodeFixHelpers.AddNewLineAfterTokenInsteadOfBeforeIt(newWhenTrue, colonToken, whenFalse);

                newWhenTrue = left;
                newColonToken = token;
                newWhenFalse = right;
            }

            ConditionalExpressionSyntax newConditionalExpression = ConditionalExpression(
                newCondition,
                newQuestionToken,
                newWhenTrue,
                newColonToken,
                newWhenFalse);

            return document.ReplaceNodeAsync(conditionalExpression, newConditionalExpression, cancellationToken);
        }
    }
}
