// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertHasFlagCallToBitwiseOperationRefactoring
    {
        public const string Title = "Use '&' operator";

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            return await RefactorAsync(document, invocation, semanticModel, cancellationToken).ConfigureAwait(false);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ExpressionSyntax expression = invocation.ArgumentList.Arguments[0].Expression;

            var enumTypeSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(expression, cancellationToken);

            Optional<object> constantValue = semanticModel.GetConstantValue(expression, cancellationToken);

            ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValue.Value, enumTypeSymbol);

            bool isComposite = FlagsUtility<ulong>.Instance.IsComposite(value);

            ParenthesizedExpressionSyntax parenthesizedExpression = ParenthesizedExpression(
                BitwiseAndExpression(
                    ((MemberAccessExpressionSyntax)invocation.Expression).Expression.Parenthesize(),
                    expression.Parenthesize()).Parenthesize());

            SyntaxKind binaryExpressionKind = (isComposite) ? SyntaxKind.EqualsExpression : SyntaxKind.NotEqualsExpression;

            SyntaxNode nodeToReplace = invocation;

            SyntaxNode parent = invocation.Parent;

            if (!parent.SpanContainsDirectives())
            {
                switch (parent.Kind())
                {
                    case SyntaxKind.LogicalNotExpression:
                        {
                            binaryExpressionKind = (isComposite) ? SyntaxKind.NotEqualsExpression : SyntaxKind.EqualsExpression;
                            nodeToReplace = parent;
                            break;
                        }
                    case SyntaxKind.EqualsExpression:
                        {
                            switch (((BinaryExpressionSyntax)parent).Right?.Kind())
                            {
                                case SyntaxKind.TrueLiteralExpression:
                                    {
                                        binaryExpressionKind = (isComposite) ? SyntaxKind.EqualsExpression : SyntaxKind.NotEqualsExpression;
                                        nodeToReplace = parent;
                                        break;
                                    }
                                case SyntaxKind.FalseLiteralExpression:
                                    {
                                        binaryExpressionKind = (isComposite) ? SyntaxKind.NotEqualsExpression : SyntaxKind.EqualsExpression;
                                        nodeToReplace = parent;
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }

            ExpressionSyntax right;
            if (isComposite)
            {
                right = expression.Parenthesize();
            }
            else
            {
                right = NumericLiteralExpression(0);
            }

            ParenthesizedExpressionSyntax newNode = BinaryExpression(binaryExpressionKind, parenthesizedExpression, right).WithTriviaFrom(nodeToReplace)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(nodeToReplace, newNode, cancellationToken);
        }
    }
}
