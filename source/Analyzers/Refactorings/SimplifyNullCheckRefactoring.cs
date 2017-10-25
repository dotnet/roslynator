// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNullCheckRefactoring
    {
        public static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            ConditionalExpressionInfo conditionalExpressionInfo = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

            if (!conditionalExpressionInfo.Success)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(conditionalExpressionInfo.Condition, semanticModel: semanticModel, cancellationToken: cancellationToken);

            if (!nullCheck.Success)
                return;

            ExpressionSyntax whenNotNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenTrue : conditionalExpressionInfo.WhenFalse;

            ExpressionSyntax whenNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenFalse : conditionalExpressionInfo.WhenTrue;

            if (SyntaxComparer.AreEquivalent(nullCheck.Expression, whenNotNull))
            {
                if (semanticModel
                    .GetTypeSymbol(nullCheck.Expression, cancellationToken)?
                    .IsReferenceTypeOrNullableType() == true)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression,
                        conditionalExpression);
                }
            }
            else if (whenNotNull.IsKind(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.ElementAccessExpression,
                SyntaxKind.ConditionalAccessExpression,
                SyntaxKind.InvocationExpression))
            {
                ExpressionSyntax expression = UseConditionalAccessRefactoring.FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull);

                if (expression == null)
                    return;

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(nullCheck.Expression, cancellationToken);

                if (typeSymbol == null)
                    return;

                if (typeSymbol.IsReferenceType)
                {
                    Analyze(context, conditionalExpressionInfo, whenNull, whenNotNull, semanticModel, cancellationToken);
                }
                else if (typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
                {
                    if (expression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        var memberAccessExpression = (MemberAccessExpressionSyntax)expression.Parent;

                        if (!memberAccessExpression.IsParentKind(SyntaxKind.InvocationExpression)
                            && (memberAccessExpression.Name as IdentifierNameSyntax)?.Identifier.ValueText == "Value")
                        {
                            if (memberAccessExpression == whenNotNull)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression,
                                    conditionalExpression);
                            }
                            else
                            {
                                Analyze(context, conditionalExpressionInfo, whenNull, whenNotNull, semanticModel, cancellationToken);
                            }
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            ConditionalExpressionInfo conditionalExpressionInfo,
            ExpressionSyntax whenNull,
            ExpressionSyntax whenNotNull,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(whenNotNull, cancellationToken);

            if (typeSymbol?.IsErrorType() == false
                && (typeSymbol.IsReferenceType || typeSymbol.IsValueType)
                && semanticModel.IsDefaultValue(typeSymbol, whenNull, cancellationToken)
                && !RefactoringHelper.ContainsOutArgumentWithLocal(whenNotNull, semanticModel, cancellationToken)
                && !conditionalExpressionInfo.ConditionalExpression.IsInExpressionTree(semanticModel, cancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseConditionalAccessInsteadOfConditionalExpression,
                    conditionalExpressionInfo.ConditionalExpression);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            ConditionalExpressionInfo conditionalExpressionInfo = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(conditionalExpressionInfo.Condition, semanticModel: semanticModel, cancellationToken: cancellationToken);

            ExpressionSyntax whenNotNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenTrue : conditionalExpressionInfo.WhenFalse;

            ExpressionSyntax whenNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenFalse : conditionalExpressionInfo.WhenTrue;

            ExpressionSyntax expression = UseConditionalAccessRefactoring.FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull);

            bool coalesce = false;

            ExpressionSyntax newNode = null;

            if (SyntaxComparer.AreEquivalent(nullCheck.Expression, whenNotNull))
            {
                newNode = nullCheck.Expression;
                coalesce = true;
            }
            else if (semanticModel
                .GetTypeSymbol(nullCheck.Expression, cancellationToken)
                .IsConstructedFrom(SpecialType.System_Nullable_T))
            {
                if (expression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    var memberAccessExpression = (MemberAccessExpressionSyntax)expression.Parent;

                    if (!memberAccessExpression.IsParentKind(SyntaxKind.InvocationExpression)
                        && (memberAccessExpression.Name as IdentifierNameSyntax)?.Identifier.ValueText == "Value")
                    {
                        if (memberAccessExpression == whenNotNull)
                        {
                            newNode = nullCheck.Expression;
                            coalesce = true;
                        }
                        else
                        {
                            newNode = ParseExpression($"{expression}?{whenNotNull.ToString().Substring(memberAccessExpression.Span.End - whenNotNull.SpanStart)}");
                        }
                    }
                }
            }

            if (newNode == null)
                newNode = ParseExpression(whenNotNull.ToString().Insert(expression.Span.End - whenNotNull.SpanStart, "?"));

            if (coalesce || !semanticModel.GetTypeSymbol(whenNotNull, cancellationToken).IsReferenceType)
                newNode = CoalesceExpression(newNode.Parenthesize(), whenNull.Parenthesize());

            newNode = newNode
                .WithTriviaFrom(conditionalExpression)
                .Parenthesize();

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
