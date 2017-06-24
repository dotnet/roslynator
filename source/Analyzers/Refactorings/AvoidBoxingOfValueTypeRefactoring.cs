// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidBoxingOfValueTypeRefactoring
    {
        public static void AddExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            var addExpression = (BinaryExpressionSyntax)context.Node;

            MethodInfo methodInfo;
            if (context.SemanticModel.TryGetMethodInfo(addExpression, out methodInfo, context.CancellationToken)
                && methodInfo.MethodKind == MethodKind.BuiltinOperator
                && methodInfo.Name == WellKnownMemberNames.AdditionOperatorName
                && methodInfo.IsContainingType(SpecialType.System_String))
            {
                ImmutableArray<IParameterSymbol> parameters = methodInfo.Parameters;

                if (parameters[0].Type.IsObject())
                {
                    Analyze(context, addExpression.Left);
                }
                else if (parameters[1].Type.IsObject())
                {
                    Analyze(context, addExpression.Right);
                }
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            ExpressionSyntax expression2 = expression.WalkDownParentheses();

            if (!expression2.IsKind(SyntaxKind.AddExpression))
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression2, context.CancellationToken);

                if (typeSymbol?.IsValueType == true)
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, expression);
            }
        }

        public static void Interpolation(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            var interpolation = (InterpolationSyntax)context.Node;

            ExpressionSyntax expression = interpolation.Expression;

            if (expression != null)
            {
                expression = expression.WalkDownParentheses();

                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsValueType == true)
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidBoxingOfValueType, expression);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            ParenthesizedExpressionSyntax newExpression = expression
                .WithoutTrivia()
                .Parenthesize()
                .WithSimplifierAnnotation();

            ExpressionSyntax newNode = null;

            if (typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
            {
                newNode = ConditionalAccessExpression(
                    newExpression,
                    InvocationExpression(MemberBindingExpression(IdentifierName("ToString")), ArgumentList()));
            }
            else
            {
                newNode = SimpleMemberInvocationExpression(
                    newExpression,
                    IdentifierName("ToString"),
                    ArgumentList());
            }

            newNode = newNode.WithTriviaFrom(expression);

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
