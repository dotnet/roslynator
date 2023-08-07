// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings;

internal static class SimplifyNullCheckRefactoring
{
    public static async Task<Document> RefactorAsync(
        Document document,
        ConditionalExpressionSyntax conditionalExpression,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        ConditionalExpressionInfo conditionalExpressionInfo = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

        NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(conditionalExpressionInfo.Condition, semanticModel: semanticModel, cancellationToken: cancellationToken);

        ExpressionSyntax whenNotNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenTrue : conditionalExpressionInfo.WhenFalse;

        ExpressionSyntax whenNull = (nullCheck.IsCheckingNotNull) ? conditionalExpressionInfo.WhenFalse : conditionalExpressionInfo.WhenTrue;

        var castExpression = whenNotNull as CastExpressionSyntax;

        ExpressionSyntax expression = (castExpression is not null)
            ? UseConditionalAccessAnalyzer.FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, castExpression.Expression, semanticModel, cancellationToken)
            : UseConditionalAccessAnalyzer.FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull, semanticModel, cancellationToken);

        var coalesce = false;

        ExpressionSyntax newNode = null;

        if (CSharpFactory.AreEquivalent(nullCheck.Expression, whenNotNull))
        {
            //RCS1084 UseCoalesceExpressionInsteadOfConditionalExpression
            newNode = nullCheck.Expression;
            coalesce = true;

            // If the types are polymorphic then the LHS of the null coalesce must be cast to the base type.
            ITypeSymbol newNodeType = semanticModel.GetTypeSymbol(newNode, cancellationToken);
            ITypeSymbol whenNullType = semanticModel.GetTypeSymbol(whenNull, cancellationToken);
            ITypeSymbol overallType = semanticModel.GetTypeInfo(conditionalExpression, cancellationToken).ConvertedType;

            if (overallType?.SupportsExplicitDeclaration() == true
                && !SymbolEqualityComparer.Default.Equals(newNodeType, whenNullType))
            {
                TypeSyntax castType = overallType.ToTypeSyntax().WithSimplifierAnnotation();

                if ((semanticModel.GetNullableContext(conditionalExpression.SpanStart) & NullableContext.AnnotationsEnabled) != 0)
                    castType = NullableType(castType);

                newNode = CastExpression(
                    castType,
                    newNode.WithoutTrivia())
                    .WithTriviaFrom(newNode);
            }
        }
        else if (semanticModel
            .GetTypeSymbol(nullCheck.Expression, cancellationToken)
            .IsNullableType())
        {
            if (expression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression.Parent;

                if (!memberAccessExpression.IsParentKind(SyntaxKind.InvocationExpression)
                    && memberAccessExpression.Name is IdentifierNameSyntax { Identifier.ValueText: "Value" })
                {
                    if (memberAccessExpression == whenNotNull)
                    {
                        //RCS1084 UseCoalesceExpressionInsteadOfConditionalExpression
                        newNode = nullCheck.Expression;
                        coalesce = true;
                    }
                    else
                    {
                        newNode = ParseExpression($"{expression}?{whenNotNull.ToString().Substring(memberAccessExpression.Span.End - whenNotNull.SpanStart)}");

                        if (castExpression is not null)
                        {
                            newNode = castExpression
                                .WithExpression(newNode.Parenthesize())
                                .WithSimplifierAnnotation();
                        }
                    }
                }
            }
        }

        if (newNode is null)
            newNode = ParseExpression(whenNotNull.ToString().Insert(expression.Span.End - whenNotNull.SpanStart, "?"));

        if (coalesce
            || (!semanticModel.GetTypeSymbol(whenNotNull, cancellationToken).IsReferenceTypeOrNullableType()
                && (whenNull as DefaultExpressionSyntax)?.Type.IsKind(SyntaxKind.NullableType) != true))
        {
            newNode = CoalesceExpression(newNode.Parenthesize(), whenNull.Parenthesize());
        }

        newNode = newNode
            .WithTriviaFrom(conditionalExpression)
            .Parenthesize();

        return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
    }
}
