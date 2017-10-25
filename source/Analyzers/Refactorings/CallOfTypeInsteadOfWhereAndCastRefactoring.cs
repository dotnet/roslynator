// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallOfTypeInsteadOfWhereAndCastRefactoring
    {
        public static bool Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax expression = memberAccess?.Expression;

            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
            {
                var invocation2 = (InvocationExpressionSyntax)expression;

                ArgumentListSyntax argumentList = invocation2.ArgumentList;

                if (argumentList?.IsMissing == false)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                    if (arguments.Count == 1
                        && invocation2.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                        if (string.Equals(memberAccess2.Name?.Identifier.ValueText, "Where", StringComparison.Ordinal))
                        {
                            SemanticModel semanticModel = context.SemanticModel;
                            CancellationToken cancellationToken = context.CancellationToken;

                            MethodInfo methodInfo;
                            if (semanticModel.TryGetExtensionMethodInfo(invocation, out methodInfo, ExtensionMethodKind.Reduced, cancellationToken)
                                && methodInfo.IsLinqCast())
                            {
                                MethodInfo methodInfo2;
                                if (semanticModel.TryGetExtensionMethodInfo(invocation2, out methodInfo2, ExtensionMethodKind.Reduced, cancellationToken)
                                    && methodInfo2.IsLinqWhere())
                                {
                                    BinaryExpressionSyntax isExpression = GetIsExpression(arguments.First().Expression);

                                    if (isExpression != null)
                                    {
                                        var type = isExpression.Right as TypeSyntax;

                                        if (type != null)
                                        {
                                            TypeSyntax type2 = GetTypeArgument(memberAccess.Name);

                                            if (type2 != null)
                                            {
                                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type);

                                                if (typeSymbol != null)
                                                {
                                                    ITypeSymbol typeSymbol2 = semanticModel.GetTypeSymbol(type2);

                                                    if (typeSymbol.Equals(typeSymbol2))
                                                    {
                                                        TextSpan span = TextSpan.FromBounds(memberAccess2.Name.Span.Start, invocation.Span.End);

                                                        if (!invocation.ContainsDirectives(span))
                                                        {
                                                            context.ReportDiagnostic(
                                                                DiagnosticDescriptors.SimplifyLinqMethodChain,
                                                                Location.Create(invocation.SyntaxTree, span));
                                                        }

                                                        return true;
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

            return false;
        }

        private static TypeSyntax GetTypeArgument(SimpleNameSyntax name)
        {
            if (name.IsKind(SyntaxKind.GenericName))
            {
                var genericName = (GenericNameSyntax)name;

                TypeArgumentListSyntax typeArgumentList = genericName.TypeArgumentList;

                if (typeArgumentList?.IsMissing == false)
                {
                    SeparatedSyntaxList<TypeSyntax> typeArguments = typeArgumentList.Arguments;

                    if (typeArguments.Count == 1)
                        return typeArguments.First();
                }
            }

            return null;
        }

        private static BinaryExpressionSyntax GetIsExpression(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var lambda = (LambdaExpressionSyntax)expression;

                        return GetIsExpression(lambda.Body);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        private static BinaryExpressionSyntax GetIsExpression(CSharpSyntaxNode body)
        {
            switch (body?.Kind())
            {
                case SyntaxKind.IsExpression:
                    {
                        return (BinaryExpressionSyntax)body;
                    }
                case SyntaxKind.Block:
                    {
                        StatementSyntax statement = ((BlockSyntax)body).Statements.SingleOrDefault(shouldThrow: false);

                        if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
                        {
                            var returnStatement = (ReturnStatementSyntax)statement;

                            ExpressionSyntax returnExpression = returnStatement.Expression;

                            if (returnExpression?.IsKind(SyntaxKind.IsExpression) == true)
                                return (BinaryExpressionSyntax)returnExpression;
                        }

                        break;
                    }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            var genericName = (GenericNameSyntax)memberAccess.Name;

            InvocationExpressionSyntax newInvocation = invocation2.Update(
                memberAccess2.WithName(genericName.WithIdentifier(Identifier("OfType"))),
                invocation.ArgumentList.WithArguments(SeparatedList<ArgumentSyntax>()));

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }
    }
}
