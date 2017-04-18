// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLogicalNotExpressionRefactoring
    {
        public static void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var logicalNot = (PrefixUnaryExpressionSyntax)context.Node;

            ExpressionSyntax operand = logicalNot.Operand;

            switch (operand?.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLogicalNotExpression, logicalNot);
                        break;
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        var innerLogicalNot = (PrefixUnaryExpressionSyntax)operand;

                        TextSpan span = TextSpan.FromBounds(logicalNot.OperatorToken.Span.Start, innerLogicalNot.OperatorToken.Span.End);
                        Location location = Location.Create(logicalNot.SyntaxTree, span);

                        context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLogicalNotExpression, location);
                        break;
                    }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(logicalNot, GetNewNode(logicalNot), cancellationToken);
        }

        private static ExpressionSyntax GetNewNode(PrefixUnaryExpressionSyntax logicalNot)
        {
            SyntaxToken operatorToken = logicalNot.OperatorToken;
            ExpressionSyntax operand = logicalNot.Operand;

            switch (operand.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        SyntaxTriviaList leadingTrivia = operatorToken.LeadingTrivia
                            .AddRange(operatorToken.TrailingTrivia)
                            .AddRange(operand.GetLeadingTrivia());

                        LiteralExpressionSyntax expression = (operand.IsKind(SyntaxKind.TrueLiteralExpression))
                            ? FalseLiteralExpression()
                            : TrueLiteralExpression();

                        return expression
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(logicalNot.GetTrailingTrivia());
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        var logicalNot2 = (PrefixUnaryExpressionSyntax)operand;
                        SyntaxToken operatorToken2 = logicalNot2.OperatorToken;
                        ExpressionSyntax operand2 = logicalNot2.Operand;

                        SyntaxTriviaList leadingTrivia = operatorToken.LeadingTrivia
                            .AddRange(operatorToken.TrailingTrivia)
                            .AddRange(operatorToken2.LeadingTrivia)
                            .AddRange(operatorToken2.TrailingTrivia)
                            .AddRange(operand2.GetLeadingTrivia());

                        return operand2
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(logicalNot.GetTrailingTrivia());
                    }
            }

            return null;
        }
    }
}
