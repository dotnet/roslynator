// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidNullReferenceExceptionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpression memberInvocation)
        {
            switch (memberInvocation.NameText)
            {
                case "ElementAtOrDefault":
                case "FirstOrDefault":
                case "LastOrDefault":
                    {
                        InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

                        ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

                        if (IsExpressionOfAccessExpression(expression))
                        {
                            MethodInfo methodInfo;
                            if (context.SemanticModel.TryGetMethodInfo(invocationExpression, out methodInfo, context.CancellationToken)
                                && methodInfo.ReturnType.IsReferenceType
                                && methodInfo.IsContainingType(MetadataNames.System_Linq_Enumerable))
                            {
                                ReportDiagnostic(context, expression);
                            }
                        }

                        break;
                    }
            }
        }

        public static void AnalyzeAsExpression(SyntaxNodeAnalysisContext context)
        {
            var asExpression = (BinaryExpressionSyntax)context.Node;

            if (asExpression.IsParentKind(SyntaxKind.ParenthesizedExpression))
            {
                var expression = (ExpressionSyntax)asExpression.Parent;

                expression = expression.WalkUpParentheses();

                if (IsExpressionOfAccessExpression(expression))
                    ReportDiagnostic(context, expression);
            }
        }

        private static bool IsExpressionOfAccessExpression(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    return expression == ((MemberAccessExpressionSyntax)parent).Expression;
                case SyntaxKind.ElementAccessExpression:
                    return expression == ((ElementAccessExpressionSyntax)parent).Expression;
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.AvoidNullReferenceException,
                Location.Create(expression.SyntaxTree, new TextSpan(expression.Span.End, 1)));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            var textChange = new TextChange(new TextSpan(expression.Span.End, 0), "?");

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }
    }
}
