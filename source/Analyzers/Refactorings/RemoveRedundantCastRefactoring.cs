// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    internal static class RemoveRedundantCastRefactoring
    {
        public static void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax)context.Node;

            if (castExpression.ContainsDiagnostics)
                return;

            if (!(castExpression.Parent is ParenthesizedExpressionSyntax parenthesizedExpression))
                return;

            ExpressionSyntax accessedExpression = GetAccessedExpression(parenthesizedExpression.Parent);

            if (accessedExpression == null)
                return;

            TypeSyntax type = castExpression.Type;

            if (type == null)
                return;

            ExpressionSyntax expression = castExpression.Expression;

            if (expression == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (expressionTypeSymbol?.IsErrorType() != false)
                return;

            if (expressionTypeSymbol.TypeKind == TypeKind.Interface)
                return;

            if (typeSymbol.TypeKind != TypeKind.Interface
                && !typeSymbol.EqualsOrInheritsFrom(expressionTypeSymbol, includeInterfaces: true))
            {
                return;
            }

            ISymbol accessedSymbol = semanticModel.GetSymbol(accessedExpression, cancellationToken);

            INamedTypeSymbol containingType = accessedSymbol?.ContainingType;

            if (containingType == null)
                return;

            if (typeSymbol.TypeKind == TypeKind.Interface)
            {
                if (!CheckExplicitImplementation(expressionTypeSymbol, accessedSymbol))
                    return;
            }
            else
            {
                if (!CheckAccessibility(expressionTypeSymbol.OriginalDefinition, accessedSymbol, expression.SpanStart, semanticModel, cancellationToken))
                    return;

                if (!expressionTypeSymbol.EqualsOrInheritsFrom(containingType, includeInterfaces: true))
                    return;
            }

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantCast,
                Location.Create(castExpression.SyntaxTree, castExpression.ParenthesesSpan()));
        }

        private static bool CheckExplicitImplementation(ITypeSymbol typeSymbol, ISymbol symbol)
        {
            ISymbol implementation = typeSymbol.FindImplementationForInterfaceMember(symbol);

            switch (implementation?.Kind)
            {
                case SymbolKind.Property:
                    {
                        foreach (IPropertySymbol propertySymbol in ((IPropertySymbol)implementation).ExplicitInterfaceImplementations)
                        {
                            if (propertySymbol.Equals(symbol))
                                return false;
                        }

                        break;
                    }
                case SymbolKind.Method:
                    {
                        foreach (IMethodSymbol methodSymbol in ((IMethodSymbol)implementation).ExplicitInterfaceImplementations)
                        {
                            if (methodSymbol.Equals(symbol))
                                return false;
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(implementation?.Kind.ToString());
                        return false;
                    }
            }

            return true;
        }

        private static bool CheckAccessibility(
            ITypeSymbol expressionTypeSymbol,
            ISymbol accessedSymbol,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Accessibility accessibility = accessedSymbol.DeclaredAccessibility;

            if (accessibility == Accessibility.Protected)
            {
                INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

                while (containingType != null)
                {
                    if (containingType.Equals(expressionTypeSymbol))
                        return true;

                    containingType = containingType.ContainingType;
                }

                return false;
            }
            else if (accessibility == Accessibility.ProtectedOrInternal)
            {
                INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

                if (containingType?.ContainingAssembly?.Equals(expressionTypeSymbol.ContainingAssembly) == true)
                    return true;

                while (containingType != null)
                {
                    if (containingType.Equals(expressionTypeSymbol))
                        return true;

                    containingType = containingType.ContainingType;
                }

                return false;
            }

            return true;
        }

        private static ExpressionSyntax GetAccessedExpression(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.ElementAccessExpression:
                    return (ExpressionSyntax)node;
                case SyntaxKind.ConditionalAccessExpression:
                    return ((ConditionalAccessExpressionSyntax)node).WhenNotNull;
                default:
                    return null;
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var methodSymbol = semanticModel.GetSymbol(invocationExpression, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            if (!ExtensionMethodInfo.TryCreate(methodSymbol, semanticModel, out ExtensionMethodInfo extensionMethodInfo, ExtensionMethodKind.Reduced))
                return;

            if (!extensionMethodInfo.MethodInfo.IsLinqCast())
                return;

            ITypeSymbol typeArgument = extensionMethodInfo.ReducedSymbol.TypeArguments.SingleOrDefault(throwException: false);

            if (typeArgument == null)
                return;

            var memberAccessExpressionType = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken) as INamedTypeSymbol;

            if (memberAccessExpressionType?.IsConstructedFromIEnumerableOfT() != true)
                return;

            if (!typeArgument.Equals(memberAccessExpressionType.TypeArguments[0]))
                return;

            if (invocationExpression.ContainsDirectives(TextSpan.FromBounds(invocationInfo.Expression.Span.End, invocationExpression.Span.End)))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantCast,
                Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End)));
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
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax expression = memberAccessExpression.Expression;

            IEnumerable<SyntaxTrivia> trailingTrivia = invocation
                .DescendantTrivia(TextSpan.FromBounds(expression.SpanStart, invocation.Span.End))
                .Where(f => !f.IsWhitespaceOrEndOfLineTrivia())
                .Concat(invocation.GetTrailingTrivia());

            ExpressionSyntax newNode = expression
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }
    }
}
