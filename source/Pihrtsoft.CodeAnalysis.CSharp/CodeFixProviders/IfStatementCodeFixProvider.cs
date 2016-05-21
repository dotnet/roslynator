// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeFixProvider))]
    [Shared]
    public class IfStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.MergeIfStatementWithContainedIfStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            IfStatementSyntax ifStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement == null)
                return;

            IfStatementSyntax ifStatement2 = GetContainedIfStatement(ifStatement);

            if (CheckTrivia(ifStatement, ifStatement2))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Merge if with contained if",
                    cancellationToken =>
                    {
                        return MergeIfStatementWithContainedIfStatementAsync(
                            context.Document,
                            ifStatement,
                            ifStatement2,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.MergeIfStatementWithContainedIfStatement + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        private static bool CheckTrivia(IfStatementSyntax ifStatement, IfStatementSyntax ifStatement2)
        {
            TextSpan span = TextSpan.FromBounds(
                ifStatement2.FullSpan.Start,
                ifStatement2.CloseParenToken.FullSpan.End);

            if (ifStatement2.DescendantTrivia(span).All(f => f.IsWhitespaceOrEndOfLine()))
            {
                if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                    && ifStatement2.Statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)ifStatement2.Statement;

                    return block.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                        && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                        && block.CloseBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLine());
                }

                return true;
            }

            return false;
        }

        private static async Task<Document> MergeIfStatementWithContainedIfStatementAsync(
            Document document,
            IfStatementSyntax ifStatement,
            IfStatementSyntax ifStatement2,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BinaryExpressionSyntax newCondition = SyntaxFactory.BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                AddParenthesesIfNecessary(ifStatement.Condition),
                AddParenthesesIfNecessary(ifStatement2.Condition));

            IfStatementSyntax newNode = GetNewIfStatement(ifStatement, ifStatement2)
                .WithCondition(newCondition)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IfStatementSyntax GetNewIfStatement(IfStatementSyntax ifStatement, IfStatementSyntax ifStatement2)
        {
            if (ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                if (ifStatement2.Statement.IsKind(SyntaxKind.Block))
                {
                    return ifStatement.ReplaceNode(ifStatement2, ((BlockSyntax)ifStatement2.Statement).Statements);
                }
                else
                {
                    return ifStatement.ReplaceNode(ifStatement2, ifStatement2.Statement);
                }
            }
            else
            {
                return ifStatement.ReplaceNode(ifStatement.Statement, ifStatement2.Statement);
            }
        }

        private static IfStatementSyntax GetContainedIfStatement(IfStatementSyntax ifStatement)
        {
            switch (ifStatement.Statement.Kind())
            {
                case SyntaxKind.Block:
                    return (IfStatementSyntax)((BlockSyntax)ifStatement.Statement).Statements[0];
                default:
                    return (IfStatementSyntax)ifStatement.Statement;
            }
        }

        private static ExpressionSyntax AddParenthesesIfNecessary(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.LogicalAndExpression:
                    return expression;
                default:
                    {
                        return SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia())
                           .WithTriviaFrom(expression);
                    }
            }
        }
    }
}
