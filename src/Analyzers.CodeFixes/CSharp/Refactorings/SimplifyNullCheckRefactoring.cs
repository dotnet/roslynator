// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNullCheckRefactoring
    {
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

            ExpressionSyntax expression = UseConditionalAccessAnalyzer.FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull);

            bool coalesce = false;

            ExpressionSyntax newNode = null;

            if (CSharpFactory.AreEquivalent(nullCheck.Expression, whenNotNull))
            {
                newNode = nullCheck.Expression;
                coalesce = true;
            }
            else if (semanticModel
                .GetTypeSymbol(nullCheck.Expression, cancellationToken)
                .IsNullableType())
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
