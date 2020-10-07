// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwitchExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchExpressionSyntax switchExpression)
        {
            SeparatedSyntaxList<SwitchExpressionArmSyntax> arms = switchExpression.Arms;

            if (!context.Span.IsEmptyAndContainedInSpan(switchExpression.SwitchKeyword))
            {
                return;
            }

            if (!switchExpression.IsParentKind(SyntaxKind.ReturnStatement))
            {
                return;
            }

            if (switchExpression.GoverningExpression.IsMissing)
            {
                return;
            }

            if (!arms.Any())
            {
                return;
            }

            if (!arms.All(f => f.Pattern.IsKind(SyntaxKind.ConstantPattern, SyntaxKind.DiscardPattern)))
            {
                return;
            }

            context.RegisterRefactoring(
                "Convert to 'switch' statement",
                ct => ConvertSwitchExpressionToSwitchStatement(context.Document, switchExpression, ct),
                RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement);
        }

        private static Task<Document> ConvertSwitchExpressionToSwitchStatement(
            Document document,
            SwitchExpressionSyntax switchExpression,
            CancellationToken cancellationToken)
        {
            IEnumerable<SwitchSectionSyntax> sections = switchExpression.Arms.Select((arm, i) =>
            {
                SyntaxToken separator = switchExpression.Arms.GetSeparator(i);
                SyntaxToken semicolon = Token(SyntaxKind.SemicolonToken);

                if (!separator.IsMissing)
                    semicolon = semicolon.WithTriviaFrom(separator);

                PatternSyntax pattern = arm.Pattern;

                switch (pattern.Kind())
                {
                    case SyntaxKind.ConstantPattern:
                        {
                            CaseSwitchLabelSyntax label = CaseSwitchLabel(
                                Token(SyntaxKind.CaseKeyword).WithLeadingTrivia(pattern.GetLeadingTrivia()),
                                ((ConstantPatternSyntax)pattern).Expression.WithoutLeadingTrivia(),
                                Token(SyntaxKind.ColonToken).WithTriviaFrom(arm.EqualsGreaterThanToken));

                            return SwitchSection(label, CreateStatement(arm.Expression, semicolon));
                        }
                    case SyntaxKind.DiscardPattern:
                        {
                            DefaultSwitchLabelSyntax label = DefaultSwitchLabel(Token(SyntaxKind.DefaultKeyword), Token(SyntaxKind.ColonToken));

                            return SwitchSection(label, CreateStatement(arm.Expression, semicolon));
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            });

            var returnStatement = (ReturnStatementSyntax)switchExpression.Parent;

            SwitchStatementSyntax switchStatement = SwitchStatement(
                switchExpression.SwitchKeyword.WithTriviaFrom(returnStatement.ReturnKeyword),
                OpenParenToken(),
                switchExpression.GoverningExpression,
                CloseParenToken().WithTrailingTrivia(switchExpression.SwitchKeyword.TrailingTrivia),
                switchExpression.OpenBraceToken,
                sections.ToSyntaxList(),
                switchExpression.CloseBraceToken);

            switchStatement = switchStatement.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(returnStatement, switchStatement, cancellationToken);

            static StatementSyntax CreateStatement(ExpressionSyntax expression, SyntaxToken semicolon)
            {
                if (expression.IsKind(SyntaxKind.ThrowExpression))
                {
                    var throwExpression = (ThrowExpressionSyntax)expression;

                    return ThrowStatement(
                        throwExpression.ThrowKeyword,
                        throwExpression.Expression,
                        semicolon);
                }
                else
                {
                    return ReturnStatement(
                        Token(SyntaxKind.ReturnKeyword).WithLeadingTrivia(expression.GetLeadingTrivia()),
                        expression.WithoutLeadingTrivia(),
                        semicolon);
                }
            }
        }
    }
}