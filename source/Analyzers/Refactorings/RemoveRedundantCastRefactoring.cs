// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantCastRefactoring
    {
        public static void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax)context.Node;

            SyntaxNode parent = castExpression.Parent;

            if (parent?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)parent;

                parent = parenthesizedExpression.Parent;

                if (parent != null)
                {
                    ExpressionSyntax accessedExpression = GetAccessedExpression(parent);

                    if (accessedExpression != null)
                    {
                        TypeSyntax type = castExpression.Type;

                        if (type != null)
                        {
                            ExpressionSyntax expression = castExpression.Expression;

                            if (expression != null
                                && CanRefactor(type, expression, accessedExpression, context.SemanticModel, context.CancellationToken)
                                && !parenthesizedExpression.SpanContainsDirectives())
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.RemoveRedundantCast,
                                    Location.Create(castExpression.SyntaxTree, castExpression.ParenthesesSpan()));
                            }
                        }
                    }
                }
            }
        }

        private static bool CanRefactor(
            TypeSyntax type,
            ExpressionSyntax expression,
            ExpressionSyntax accessedExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol?.IsErrorType() == false)
            {
                ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

                if (expressionTypeSymbol?.IsErrorType() == false
                    && !expressionTypeSymbol.IsInterface())
                {
                    bool isInterface = typeSymbol.IsInterface();

                    if (isInterface
                        || typeSymbol.EqualsOrInheritsFrom(expressionTypeSymbol, includeInterfaces: true))
                    {
                        ISymbol accessedSymbol = semanticModel.GetSymbol(accessedExpression, cancellationToken);

                        INamedTypeSymbol containingType = accessedSymbol?.ContainingType;

                        if (containingType != null)
                        {
                            if (isInterface)
                            {
                                ISymbol implementation = expressionTypeSymbol.FindImplementationForInterfaceMember(accessedSymbol);

                                switch (implementation?.Kind)
                                {
                                    case SymbolKind.Property:
                                        return !((IPropertySymbol)implementation).ExplicitInterfaceImplementations.Any(f => f.Equals(accessedSymbol));
                                    case SymbolKind.Method:
                                        return !((IMethodSymbol)implementation).ExplicitInterfaceImplementations.Any(f => f.Equals(accessedSymbol));
                                }
                            }
                            else
                            {
                                return expressionTypeSymbol.EqualsOrInheritsFrom(containingType, includeInterfaces: true);
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static ExpressionSyntax GetAccessedExpression(SyntaxNode parent)
        {
            switch (parent.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.ElementAccessExpression:
                    return (ExpressionSyntax)parent;
                case SyntaxKind.ConditionalAccessExpression:
                    return ((ConditionalAccessExpressionSyntax)parent).WhenNotNull;
                default:
                    return null;
            }
        }

        internal static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList?.IsMissing == false)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                    if (arguments.Count == 0)
                    {
                        SimpleNameSyntax name = memberAccess.Name;

                        if (name != null)
                        {
                            string methodName = name.Identifier.ValueText;

                            if (methodName == "Cast")
                            {
                                SemanticModel semanticModel = context.SemanticModel;
                                CancellationToken cancellationToken = context.CancellationToken;

                                ISymbol symbol = semanticModel.GetSymbol(invocation, cancellationToken);

                                if (symbol?.IsMethod() == true)
                                {
                                    ExtensionMethodInfo extensionMethodInfo;
                                    if (ExtensionMethodInfo.TryCreate((IMethodSymbol)symbol, semanticModel, out extensionMethodInfo, ExtensionMethodKind.Reduced)
                                        && extensionMethodInfo.MethodInfo.IsLinqCast())
                                    {
                                        ImmutableArray<ITypeSymbol> typeArguments = extensionMethodInfo.ReducedSymbol.TypeArguments;

                                        if (typeArguments.Length == 1)
                                        {
                                            ExpressionSyntax memberAccessExpression = memberAccess.Expression;

                                            if (memberAccessExpression != null)
                                            {
                                                var memberAccessExpressionType = semanticModel.GetTypeSymbol(memberAccessExpression, cancellationToken) as INamedTypeSymbol;

                                                if (memberAccessExpressionType?.IsConstructedFromIEnumerableOfT() == true
                                                    && typeArguments[0].Equals(memberAccessExpressionType.TypeArguments[0])
                                                    && !invocation.ContainsDirectives(TextSpan.FromBounds(memberAccessExpression.Span.End, invocation.Span.End)))
                                                {
                                                    context.ReportDiagnostic(
                                                        DiagnosticDescriptors.RemoveRedundantCast,
                                                        Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(name.SpanStart, argumentList.Span.End)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            CastExpressionSyntax castExpression,
            CancellationToken cancellationToken)
        {
            var parenthesizedExpression = (ParenthesizedExpressionSyntax)castExpression.Parent;

            ParenthesizedExpressionSyntax newNode = parenthesizedExpression
                .WithExpression(castExpression.Expression.WithTriviaFrom(castExpression))
                .WithFormatterAnnotation()
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(parenthesizedExpression, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax expression = memberAccess.Expression;

            IEnumerable<SyntaxTrivia> trailing = invocation.DescendantTrivia(TextSpan.FromBounds(expression.SpanStart, invocation.Span.End));

            if (trailing.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                trailing = invocation.GetTrailingTrivia();
            }
            else
            {
                trailing = trailing.Concat(invocation.GetTrailingTrivia());
            }

            ExpressionSyntax newNode = expression
                .WithTrailingTrivia(trailing)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }
    }
}
