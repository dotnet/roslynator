// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringFormatWithInterpolatedStringRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            invocation = await FindOutermostFormatMethodAsync(context, invocation).ConfigureAwait(false);

            if (invocation != null)
            {
                context.RegisterRefactoring(
                    $"Replace '{invocation.Expression}' with interpolated string",
                    cancellationToken => CreateInterpolatedStringAsync(context.Document, invocation, cancellationToken));
            }
        }

        private static async Task<InvocationExpressionSyntax> FindOutermostFormatMethodAsync(
            RefactoringContext context,
            InvocationExpressionSyntax invocation)
        {
            ImmutableArray<ISymbol>? formatMethods = null;

            while (invocation != null)
            {
                if (invocation.ArgumentList != null)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

                    if (arguments.Count >= 2)
                    {
                        var firstArgument = arguments[0]?.Expression as LiteralExpressionSyntax;

                        if (firstArgument?.Token.IsKind(SyntaxKind.StringLiteralToken) == true)
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol invocationSymbol = semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;

                            if (formatMethods == null)
                            {
                                formatMethods = GetFormatMethods(semanticModel);

                                if (formatMethods.Value.Length == 0)
                                    return null;
                            }

                            if (formatMethods.Value.Contains(invocationSymbol))
                                break;
                        }
                    }
                }

                invocation = invocation.FirstAncestor<InvocationExpressionSyntax>();
            }

            return invocation;
        }

        private static ImmutableArray<ISymbol> GetFormatMethods(SemanticModel semanticModel)
        {
            INamedTypeSymbol stringType = semanticModel.Compilation.GetTypeByMetadataName("System.String");

            if (stringType != null)
            {
                return stringType
                    .GetMembers("Format")
                    .RemoveAll(symbol => !IsValidFormatMethod(symbol));
            }

            return ImmutableArray<ISymbol>.Empty;
        }

        private static async Task<Document> CreateInterpolatedStringAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<ExpressionSyntax> expandedArguments = ImmutableArray.CreateRange(GetExpandedArguments(arguments, semanticModel));

            string formatText = ((LiteralExpressionSyntax)arguments[0].Expression).Token.ToString();

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + formatText);

            var rewriter = new InterpolatedStringSyntaxRewriter(expandedArguments);

            var newInterpolatedString = (InterpolatedStringExpressionSyntax)rewriter.Visit(interpolatedString);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(invocation, newInterpolatedString);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<ExpressionSyntax> GetExpandedArguments(SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            for (int i = 1; i < arguments.Count; i++)
            {
                ITypeSymbol targetType = semanticModel.GetTypeInfo(arguments[i].Expression).ConvertedType;

                ExpressionSyntax expression = Cast(arguments[i].Expression, targetType);

                yield return Parenthesize(expression);
            }
        }

        private static ExpressionSyntax Parenthesize(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                return expression;

            return ParenthesizedExpression(
                    Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
                    expression,
                    Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty))
                .WithSimplifierAnnotation();
        }

        private static ExpressionSyntax Cast(ExpressionSyntax expression, ITypeSymbol targetType)
        {
            if (targetType == null)
                return expression;

            TypeSyntax type = ParseTypeName(targetType.ToDisplayString());

            return CastExpression(type, Parenthesize(expression))
                .WithSimplifierAnnotation();
        }

        private static bool IsValidFormatMethod(ISymbol symbol)
        {
            if (!symbol.IsMethod())
                return false;

            if (!symbol.IsStatic)
                return false;

            var methodSymbol = (IMethodSymbol)symbol;

            if (methodSymbol.Parameters.Length == 0)
                return false;

            if (methodSymbol.Parameters[0]?.Name != "format")
                return false;

            return true;
        }

        private class InterpolatedStringSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly ImmutableArray<ExpressionSyntax> _expandedArguments;

            public InterpolatedStringSyntaxRewriter(ImmutableArray<ExpressionSyntax> expandedArguments)
            {
                _expandedArguments = expandedArguments;
            }

            public override SyntaxNode VisitInterpolation(InterpolationSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                var literalExpression = node.Expression as LiteralExpressionSyntax;

                if (literalExpression != null && literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    var index = (int)literalExpression.Token.Value;

                    if (index >= 0 && index < _expandedArguments.Length)
                        return node.WithExpression(_expandedArguments[index]);
                }

                return base.VisitInterpolation(node);
            }
        }
    }
}
