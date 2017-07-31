// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallThenByInsteadOfOrderByRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpression memberInvocation)
        {
            ExpressionSyntax expression = memberInvocation.Expression;

            if (expression.IsKind(SyntaxKind.InvocationExpression))
            {
                MemberInvocationExpression memberInvocation2;
                if (MemberInvocationExpression.TryCreate((InvocationExpressionSyntax)expression, out memberInvocation2))
                {
                    switch (memberInvocation2.NameText)
                    {
                        case "OrderBy":
                        case "OrderByDescending":
                            {
                                if (IsLinqExtensionOfIEnumerableOfT(context, memberInvocation.InvocationExpression)
                                    && IsLinqExtensionOfIEnumerableOfT(context, memberInvocation2.InvocationExpression))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.CallThenByInsteadOfOrderBy,
                                        memberInvocation.Name,
                                        (memberInvocation.NameText == "OrderByDescending") ? "Descending" : null);
                                }

                                break;
                            }
                    }
                }
            }
        }

        private static bool IsLinqExtensionOfIEnumerableOfT(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            MethodInfo methodInfo;
            return context.SemanticModel.TryGetExtensionMethodInfo(invocationExpression, out methodInfo, ExtensionMethodKind.None, context.CancellationToken)
                && methodInfo.IsName("OrderBy", "OrderByDescending")
                && methodInfo.IsLinqExtensionOfIEnumerableOfT();
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            string newName,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocationExpression = RefactoringHelper.ChangeInvokedMethodName(invocationExpression, newName);

            return document.ReplaceNodeAsync(invocationExpression, newInvocationExpression, cancellationToken);
        }
    }
}
