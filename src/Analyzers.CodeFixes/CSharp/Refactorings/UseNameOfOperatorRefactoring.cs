// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseNameOfOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            string identifier,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = NameOfExpression(identifier)
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            InvocationExpressionSyntax newNode = NameOfExpression(memberAccessExpression.Expression)
                .WithTriviaFrom(invocationExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }

        private readonly struct ParameterInfo
        {
            public static ParameterInfo Empty { get; } = new ParameterInfo();

            private ParameterInfo(SyntaxNode node, ParameterSyntax parameter)
            {
                Node = node;
                Parameter = parameter;
                ParameterList = null;
            }

            private ParameterInfo(SyntaxNode node, BaseParameterListSyntax parameterList)
            {
                Node = node;
                Parameter = null;
                ParameterList = parameterList;
            }

            public ParameterSyntax Parameter { get; }

            public BaseParameterListSyntax ParameterList { get; }

            public SyntaxNode Node { get; }

            public bool Success
            {
                get { return Node != null; }
            }

            public static ParameterInfo Create(SyntaxNode node)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return new ParameterInfo(node, ((MethodDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.ConstructorDeclaration:
                        return new ParameterInfo(node, ((ConstructorDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.IndexerDeclaration:
                        return new ParameterInfo(node, ((IndexerDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.SimpleLambdaExpression:
                        return new ParameterInfo(node, ((SimpleLambdaExpressionSyntax)node).Parameter);
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return new ParameterInfo(node, ((ParenthesizedLambdaExpressionSyntax)node).ParameterList);
                    case SyntaxKind.AnonymousMethodExpression:
                        return new ParameterInfo(node, ((AnonymousMethodExpressionSyntax)node).ParameterList);
                    case SyntaxKind.LocalFunctionStatement:
                        return new ParameterInfo(node, ((LocalFunctionStatementSyntax)node).ParameterList);
                }

                return Empty;
            }
        }
    }
}
