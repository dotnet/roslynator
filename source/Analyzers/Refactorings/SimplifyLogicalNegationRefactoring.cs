// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLogicalNegationRefactoring
    {
        public static void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var logicalNot = (PrefixUnaryExpressionSyntax)context.Node;

            ExpressionSyntax expression = logicalNot.Operand?.WalkDownParentheses();

            if (expression?.IsMissing != false)
                return;

            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.LogicalNotExpression:
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLogicalNegation, logicalNot);
                        break;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        MemberDeclarationSyntax memberDeclaration = logicalNot.FirstAncestor<MemberDeclarationSyntax>();

                        if (memberDeclaration is OperatorDeclarationSyntax operatorDeclaration
                            && operatorDeclaration.OperatorToken.Kind() == SyntaxKind.ExclamationEqualsToken)
                        {
                            return;
                        }

                        context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLogicalNegation, logicalNot);
                        break;
                    }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            SyntaxNode parent = invocationInfo.InvocationExpression.WalkUpParentheses().Parent;

            if (parent.Kind() != SyntaxKind.LogicalNotExpression)
                return;

            SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(invocationInfo.Arguments.First().Expression.WalkDownParentheses());

            if (!lambdaInfo.Success)
                return;

            ExpressionSyntax expression = GetReturnExpression(lambdaInfo.Body)?.WalkDownParentheses();

            if (expression?.IsKind(SyntaxKind.LogicalNotExpression) != true)
                return;

            if (!context.SemanticModel.TryGetExtensionMethodInfo(invocationInfo.InvocationExpression, out MethodInfo methodInfo, ExtensionMethodKind.Reduced, context.CancellationToken))
                return;

            if (!methodInfo.IsLinqExtensionOfIEnumerableOfTWithPredicate())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLogicalNegation, parent);
        }

        private static ExpressionSyntax GetReturnExpression(CSharpSyntaxNode node)
        {
            if (node is BlockSyntax block)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

                return (statement as ReturnStatementSyntax)?.Expression;
            }

            return node as ExpressionSyntax;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newNode = GetNewNode(logicalNot)
                .WithTriviaFrom(logicalNot)
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(logicalNot, newNode, cancellationToken);
        }

        private static ExpressionSyntax GetNewNode(PrefixUnaryExpressionSyntax logicalNot)
        {
            ExpressionSyntax operand = logicalNot.Operand;
            ExpressionSyntax expression = operand.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        LiteralExpressionSyntax newNode = BooleanLiteralExpression(expression.Kind() == SyntaxKind.FalseLiteralExpression);

                        newNode = newNode.WithTriviaFrom(expression);

                        return operand.ReplaceNode(expression, newNode);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)expression;

                        BinaryExpressionSyntax notEqualsExpression = NotEqualsExpression(
                            equalsExpression.Left,
                            ExclamationEqualsToken().WithTriviaFrom(equalsExpression.OperatorToken),
                            equalsExpression.Right);

                        return operand.ReplaceNode(equalsExpression, notEqualsExpression);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocationExpression = (InvocationExpressionSyntax)expression;

                        var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                        ExpressionSyntax lambdaExpression = invocationExpression.ArgumentList.Arguments.First().Expression.WalkDownParentheses();

                        SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(lambdaExpression);

                        var logicalNot2 = (PrefixUnaryExpressionSyntax)GetReturnExpression(lambdaInfo.Body).WalkDownParentheses();

                        InvocationExpressionSyntax newNode = invocationExpression.ReplaceNode(logicalNot2, logicalNot2.Operand.WithTriviaFrom(logicalNot2));

                        newNode = RefactoringHelper.ChangeInvokedMethodName(newNode, (memberAccessExpression.Name.Identifier.ValueText == "All") ? "Any" : "All");

                        return newNode;
                    }
            }

            return null;
        }
    }
}
