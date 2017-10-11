// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal static class InlineMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (!CheckSpan(invocation, context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = GetMethodSymbol(invocation, semanticModel, context.CancellationToken);

            if (methodSymbol == null)
                return;

            MethodDeclarationSyntax methodDeclaration = await GetMethodDeclarationAsync(methodSymbol, context.CancellationToken).ConfigureAwait(false);

            if (methodDeclaration == null)
                return;

            if (invocation.Ancestors().Any(f => f == methodDeclaration))
                return;

            ExpressionSyntax expression = GetMethodExpression(methodDeclaration);

            if (expression != null)
            {
                ImmutableArray<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol);

                if (parameterInfos.IsDefault)
                    return;

                INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(invocation.SpanStart, context.CancellationToken);

                SemanticModel declarationSemanticModel = (invocation.SyntaxTree == methodDeclaration.SyntaxTree)
                    ? semanticModel
                    : await context.Solution.GetDocument(methodDeclaration.SyntaxTree).GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var refactoring = new InlineMethodExpressionRefactoring(context.Document, invocation, enclosingType, methodSymbol, methodDeclaration, parameterInfos, semanticModel, declarationSemanticModel, context.CancellationToken);

                context.RegisterRefactoring("Inline method", c => refactoring.InlineMethodAsync(invocation, expression));

                context.RegisterRefactoring("Inline and remove method", c => refactoring.InlineAndRemoveMethodAsync(invocation, expression));
            }
            else if (methodSymbol.ReturnsVoid
                && invocation.IsParentKind(SyntaxKind.ExpressionStatement))
            {
                BlockSyntax body = methodDeclaration.Body;

                if (body == null)
                    return;

                SyntaxList<StatementSyntax> statements = body.Statements;

                if (!statements.Any())
                    return;

                ImmutableArray<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol);

                if (parameterInfos.IsDefault)
                    return;

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

        private static bool CheckSpan(InvocationExpressionSyntax invocation, TextSpan span)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression == null)
                return false;

            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList == null)
                return false;

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                return ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(span) == true;
            }
            else
            {
                return expression.Span.Contains(span);
            }
        }

        private static ExpressionSyntax GetMethodExpression(MethodDeclarationSyntax method)
        {
            BlockSyntax body = method.Body;

            if (body == null)
                return method.ExpressionBody?.Expression;

            switch (body.Statements.SingleOrDefault(throwException: false))
            {
                case ReturnStatementSyntax returnStatement:
                    return returnStatement.Expression;
                case ExpressionStatementSyntax expressionStatement:
                    return expressionStatement.Expression;
                default:
                    return null;
            }
        }

        private static async Task<MethodDeclarationSyntax> GetMethodDeclarationAsync(IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            foreach (SyntaxReference reference in methodSymbol.DeclaringSyntaxReferences)
            {
                SyntaxNode node = await reference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

                if ((node is MethodDeclarationSyntax methodDeclaration)
                    && methodDeclaration.BodyOrExpressionBody() != null)
                {
                    return methodDeclaration;
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

            if (methodSymbol?.Language != LanguageNames.CSharp)
                return null;

            MethodKind methodKind = methodSymbol.MethodKind;

            if (methodKind == MethodKind.Ordinary)
            {
                if (methodSymbol.IsStatic)
                    return methodSymbol;

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
            else if (methodKind == MethodKind.ReducedExtension
                && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                return methodSymbol;
            }

            return null;
        }

        private static ImmutableArray<ParameterInfo> GetParameterInfos(
            InvocationExpressionSyntax invocation,
            IMethodSymbol methodSymbol)
        {
            bool isReduced = methodSymbol.MethodKind == MethodKind.ReducedExtension;

            if (isReduced)
                methodSymbol = methodSymbol.GetConstructedReducedFrom();

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (isReduced)
                parameters = parameters.RemoveAt(0);

            List<ParameterInfo> parameterInfos = null;

            SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;

            foreach (ArgumentSyntax argument in arguments)
            {
                IParameterSymbol parameterSymbol = DetermineParameterHelper.DetermineParameter(argument, arguments, parameters);

                if (parameterSymbol != null)
                {
                    var parameterInfo = new ParameterInfo(parameterSymbol, argument.Expression);

                    (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
                }
                else
                {
                    return default(ImmutableArray<ParameterInfo>);
                }
            }

            foreach (IParameterSymbol parameterSymbol in parameters)
            {
                if (parameterInfos == null
                    || parameterInfos.FindIndex(f =>
                    {
                        Debug.WriteLine(f.ParameterSymbol == parameterSymbol);
                        Debug.WriteLine(f.ParameterSymbol.Equals(parameterSymbol));
                        Debug.WriteLine(f.ParameterSymbol.Name == parameterSymbol.Name);
                        return f.ParameterSymbol.Equals(parameterSymbol);
                    }) == -1)
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

            if (isReduced)
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                var parameterInfo = new ParameterInfo(methodSymbol.Parameters[0], memberAccess.Expression.TrimTrivia());

                (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
            }

            return (parameterInfos != null)
                ? parameterInfos.ToImmutableArray()
                : ImmutableArray<ParameterInfo>.Empty;
        }
    }
}
