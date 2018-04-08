// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringFormatWithInterpolatedStringRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = null;

            ImmutableArray<ISymbol> formatMethods;

            while (invocation != null)
            {
                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList != null)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                    if (arguments.Count >= 2
                        && (arguments[0].Expression?.Kind() == SyntaxKind.StringLiteralExpression))
                    {
                        if (semanticModel == null)
                            semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ISymbol invocationSymbol = semanticModel.GetSymbol(invocation, context.CancellationToken);

                        if (formatMethods.IsDefault)
                        {
                            formatMethods = GetFormatMethods(semanticModel);

                            if (!formatMethods.Any())
                                return;
                        }

                        if (formatMethods.Contains(invocationSymbol))
                            break;
                    }
                }

                invocation = invocation.FirstAncestor<InvocationExpressionSyntax>();
            }

            if (invocation == null)
                return;

            context.RegisterRefactoring(
                $"Replace {invocation.Expression} with interpolated string",
                cancellationToken => RefactorAsync(context.Document, invocation, semanticModel, cancellationToken));
        }

        private static ImmutableArray<ISymbol> GetFormatMethods(SemanticModel semanticModel)
        {
            INamedTypeSymbol stringType = semanticModel.Compilation.GetSpecialType(SpecialType.System_String);

            if (stringType == null)
                return ImmutableArray<ISymbol>.Empty;

            return stringType
                .GetMembers("Format")
                .RemoveAll(symbol =>
                {
                    return !symbol.IsStatic
                        || symbol.Kind != SymbolKind.Method
                        || ((IMethodSymbol)symbol)
                            .Parameters
                            .FirstOrDefault()?
                            .Name != "format";
                });
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

            var formatExpression = (LiteralExpressionSyntax)arguments[0].Expression;

            string formatText = formatExpression.Token.Text;

            bool isVerbatim = formatText.StartsWith("@", StringComparison.Ordinal);

            string text = "$" + formatText;

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression(text);

            if (CanReplaceInterpolationWithStringLiteralInnerText(arguments, isVerbatim))
                interpolatedString = ReplaceInterpolationWithStringLiteralInnerText(arguments, interpolatedString, text);

            IEnumerable<ExpressionSyntax> interpolationExpressions = GetInterpolationExpressions(arguments, semanticModel, cancellationToken);

            var rewriter = new InterpolatedStringSyntaxRewriter(interpolationExpressions);

            var newNode = (InterpolatedStringExpressionSyntax)rewriter.Visit(interpolatedString);

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }

        private static bool CanReplaceInterpolationWithStringLiteralInnerText(SeparatedSyntaxList<ArgumentSyntax> arguments, bool isVerbatim)
        {
            bool result = false;

            for (int i = 1; i < arguments.Count; i++)
            {
                ExpressionSyntax expression = arguments[i].Expression;

                StringLiteralExpressionInfo info = SyntaxInfo.StringLiteralExpressionInfo(expression);

                if (info.Success)
                {
                    if (isVerbatim == info.IsVerbatim
                        && info.Expression.GetLeadingTrivia().IsEmptyOrWhitespace()
                        && info.Expression.GetTrailingTrivia().IsEmptyOrWhitespace())
                    {
                        result = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return result;
        }

        private static InterpolatedStringExpressionSyntax ReplaceInterpolationWithStringLiteralInnerText(
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            InterpolatedStringExpressionSyntax interpolatedString,
            string text)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            int pos = 0;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i].Kind() != SyntaxKind.Interpolation)
                    continue;

                var interpolation = (InterpolationSyntax)contents[i];

                ExpressionSyntax expression = interpolation.Expression;

                if (expression?.Kind() != SyntaxKind.NumericLiteralExpression)
                    continue;

                var index = (int)((LiteralExpressionSyntax)expression).Token.Value;

                if (index < 0)
                    continue;

                if (index >= arguments.Count)
                    continue;

                ExpressionSyntax argumentExpression = arguments[index + 1].Expression;

                StringLiteralExpressionInfo stringLiteral = SyntaxInfo.StringLiteralExpressionInfo(argumentExpression);

                if (!stringLiteral.Success)
                    continue;

                sb.Append(text, pos, interpolation.SpanStart - pos);

                int startIndex = sb.Length;
                sb.Append(stringLiteral.InnerText);
                sb.Replace("{", "{{", startIndex);
                sb.Replace("}", "}}", startIndex);

                pos = interpolation.Span.End;
            }

            sb.Append(text, pos, text.Length - pos);

            return (InterpolatedStringExpressionSyntax)ParseExpression(StringBuilderCache.GetStringAndFree(sb));
        }

        private static IEnumerable<ExpressionSyntax> GetInterpolationExpressions(
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = 1; i < arguments.Count; i++)
            {
                ExpressionSyntax expression = arguments[i].Expression;

                ITypeSymbol targetType = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

                if (targetType != null)
                {
                    TypeSyntax type = targetType.ToMinimalTypeSyntax(semanticModel, expression.SpanStart);

                    expression = CastExpression(type, expression.WithoutTrivia().Parenthesize())
                        .WithTriviaFrom(expression)
                        .WithSimplifierAnnotation();
                }

                yield return expression.Parenthesize();
            }
        }

        private class InterpolatedStringSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly ImmutableArray<ExpressionSyntax> _interpolationExpressions;

            public InterpolatedStringSyntaxRewriter(IEnumerable<ExpressionSyntax> interpolationExpressions)
            {
                _interpolationExpressions = interpolationExpressions.ToImmutableArray();
            }

            public override SyntaxNode VisitInterpolation(InterpolationSyntax node)
            {
                ExpressionSyntax expression = node.Expression;

                if (expression?.Kind() == SyntaxKind.NumericLiteralExpression)
                {
                    var literalExpression = (LiteralExpressionSyntax)expression;

                    var index = (int)literalExpression.Token.Value;

                    if (index >= 0 && index < _interpolationExpressions.Length)
                        return node.WithExpression(_interpolationExpressions[index]);
                }

                return base.VisitInterpolation(node);
            }
        }
    }
}
