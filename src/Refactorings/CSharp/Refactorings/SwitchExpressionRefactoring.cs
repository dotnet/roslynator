// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                RefactoringDescriptors.ConvertSwitchExpressionToSwitchStatement);
        }

        private static Task<Document> ConvertSwitchExpressionToSwitchStatement(
            Document document,
            SwitchExpressionSyntax switchExpression,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<SwitchExpressionArmSyntax> arms = switchExpression.Arms;
            SyntaxToken[] separators = arms.GetSeparators().ToArray();

            IEnumerable<SwitchSectionSyntax> sections = arms.Select((arm, i) =>
            {
                PatternSyntax pattern = arm.Pattern;
                ExpressionSyntax expression = arm.Expression;
                SyntaxToken semicolon = Token(SyntaxKind.SemicolonToken);
                SyntaxToken separator = default;

                if (i < separators.Length)
                    separator = separators[i];

                if (separator.IsKind(SyntaxKind.None)
                    || separator.IsMissing)
                {
                    semicolon = semicolon.WithTrailingTrivia(arm.GetTrailingTrivia());
                    expression = expression.WithoutTrailingTrivia();
                }
                else
                {
                    semicolon = semicolon.WithTriviaFrom(separator);
                }

                SyntaxKind kind = pattern.Kind();
                SwitchLabelSyntax label = default;

                if (kind == SyntaxKind.ConstantPattern)
                {
                    label = CaseSwitchLabel(
                        Token(SyntaxKind.CaseKeyword).WithLeadingTrivia(pattern.GetLeadingTrivia()),
                        ((ConstantPatternSyntax)pattern).Expression.WithoutLeadingTrivia(),
                        Token(SyntaxKind.ColonToken).WithTriviaFrom(arm.EqualsGreaterThanToken));
                }
                else if (kind == SyntaxKind.DiscardPattern)
                {
                    label = DefaultSwitchLabel(Token(SyntaxKind.DefaultKeyword), Token(SyntaxKind.ColonToken));
                }
                else
                {
                    throw new InvalidOperationException();
                }

                StatementSyntax statement = CreateStatement(expression, semicolon);

                return SwitchSection(label, statement);
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
