// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal static class InlineMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (CheckSpan(invocation, context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IMethodSymbol methodSymbol = GetMethodSymbol(invocation, semanticModel, context.CancellationToken);

                if (methodSymbol != null)
                {
                    MethodDeclarationSyntax methodDeclaration = await GetMethodDeclarationAsync(methodSymbol, context.CancellationToken).ConfigureAwait(false);

                    if (methodDeclaration != null
                        && !invocation.Ancestors().Any(f => f == methodDeclaration))
                    {
                        ExpressionSyntax expression = GetMethodExpression(methodDeclaration);

                        if (expression != null)
                        {
                            ImmutableArray<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol, semanticModel, context.CancellationToken);

                            if (!parameterInfos.IsDefault)
                            {
                                INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(invocation.SpanStart, context.CancellationToken);

                                SemanticModel declarationSemanticModel = (invocation.SyntaxTree == methodDeclaration.SyntaxTree)
                                    ? semanticModel
                                    : await context.Solution.GetDocument(methodDeclaration.SyntaxTree).GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                                var refactoring = new InlineMethodExpressionRefactoring(context.Document, invocation, enclosingType, methodSymbol, methodDeclaration, parameterInfos, semanticModel, declarationSemanticModel, context.CancellationToken);

                                context.RegisterRefactoring("Inline method", c => refactoring.InlineMethodAsync(invocation, expression));

                                context.RegisterRefactoring("Inline and remove method", c => refactoring.InlineAndRemoveMethodAsync(invocation, expression));
                            }
                        }
                        else if (methodSymbol.ReturnsVoid
                            && invocation.IsParentKind(SyntaxKind.ExpressionStatement))
                        {
                            BlockSyntax body = methodDeclaration.Body;

                            if (body != null)
                            {
                                SyntaxList<StatementSyntax> statements = body.Statements;

                                if (statements.Any())
                                {
                                    ImmutableArray<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol, semanticModel, context.CancellationToken);

                                    if (!parameterInfos.IsDefault)
                                    {
                                        var expressionStatement = (ExpressionStatementSyntax)invocation.Parent;

                                        INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(invocation.SpanStart, context.CancellationToken);

                                        SemanticModel declarationSemanticModel = (invocation.SyntaxTree == methodDeclaration.SyntaxTree)
                                            ? semanticModel
                                            : await context.Solution.GetDocument(methodDeclaration.SyntaxTree).GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                                        var refactoring = new InlineMethodStatementsRefactoring(context.Document, invocation, enclosingType, methodSymbol, methodDeclaration, parameterInfos, semanticModel, declarationSemanticModel, context.CancellationToken);

                                        context.RegisterRefactoring("Inline method", c => refactoring.InlineMethodAsync(expressionStatement, statements));

                                        context.RegisterRefactoring("Inline and remove method", c => refactoring.InlineAndRemoveMethodAsync(expressionStatement, statements));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckSpan(InvocationExpressionSyntax invocation, TextSpan span)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression != null)
            {
                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList != null)
                {
                    if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        return ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(span) == true;
                    }
                    else if (expression.Span.Contains(span))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static ExpressionSyntax GetMethodExpression(MethodDeclarationSyntax method)
        {
            BlockSyntax body = method.Body;

            if (body != null)
            {
                StatementSyntax statement = body.Statements.SingleOrDefault(throwException: false);

                switch (statement?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    case SyntaxKind.ExpressionStatement:
                        return ((ExpressionStatementSyntax)statement).Expression;
                }

                return null;
            }

            return method.ExpressionBody?.Expression;
        }

        private static async Task<MethodDeclarationSyntax> GetMethodDeclarationAsync(IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            foreach (SyntaxReference reference in methodSymbol.DeclaringSyntaxReferences)
            {
                SyntaxNode node = await reference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

                if (node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var method = (MethodDeclarationSyntax)node;

                    if (method.Body != null
                        || method.ExpressionBody != null)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private static IMethodSymbol GetMethodSymbol(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var methodSymbol = semanticModel.GetSymbol(invocation, cancellationToken) as IMethodSymbol;

            if (methodSymbol?.Language == LanguageNames.CSharp)
            {
                MethodKind methodKind = methodSymbol.MethodKind;

                if (methodKind == MethodKind.Ordinary)
                {
                    if (methodSymbol.IsStatic)
                    {
                        return methodSymbol;
                    }
                    else
                    {
                        INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(invocation.SpanStart, cancellationToken);

                        if (methodSymbol.ContainingType?.Equals(enclosingType) == true)
                        {
                            ExpressionSyntax expression = invocation.Expression;

                            if (expression != null)
                            {
                                if (!expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                    || ((MemberAccessExpressionSyntax)expression).Expression.IsKind(SyntaxKind.ThisExpression))
                                {
                                    return methodSymbol;
                                }
                            }
                        }
                    }
                }
                else if (methodKind == MethodKind.ReducedExtension
                    && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    return methodSymbol;
                }
            }

            return null;
        }

        private static ImmutableArray<ParameterInfo> GetParameterInfos(
            InvocationExpressionSyntax invocation,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            List<ParameterInfo> parameterInfos;
            if (TryGetParameterInfos(invocation.ArgumentList, semanticModel, cancellationToken, out parameterInfos))
            {
                foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                {
                    if (parameterInfos == null
                        || parameterInfos.FindIndex(f => f.ParameterSymbol == parameterSymbol) == -1)
                    {
                        if (parameterSymbol.HasExplicitDefaultValue)
                        {
                            var parameterInfo = new ParameterInfo(parameterSymbol, parameterSymbol.GetDefaultValueSyntax());

                            (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
                        }
                        else
                        {
                            return default(ImmutableArray<ParameterInfo>);
                        }
                    }
                }

                if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                    var parameterInfo = new ParameterInfo(methodSymbol.ReducedFrom.Parameters[0], memberAccess.Expression);

                    (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
                }

                return (parameterInfos != null)
                    ? parameterInfos.ToImmutableArray()
                    : ImmutableArray<ParameterInfo>.Empty;
            }

            return default(ImmutableArray<ParameterInfo>);
        }

        private static bool TryGetParameterInfos(
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out List<ParameterInfo> parameterInfos)
        {
            List<ParameterInfo> list = null;

            foreach (ArgumentSyntax argument in argumentList.Arguments)
            {
                IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(argument, cancellationToken: cancellationToken);

                if (parameterSymbol != null)
                {
                    (list ?? (list = new List<ParameterInfo>())).Add(new ParameterInfo(parameterSymbol, argument.Expression));
                }
                else
                {
                    parameterInfos = null;
                    return false;
                }
            }

            parameterInfos = list;
            return true;
        }
    }
}
