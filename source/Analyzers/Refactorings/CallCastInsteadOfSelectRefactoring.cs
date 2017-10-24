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
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallCastInsteadOfSelectRefactoring
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpression memberInvocation)
        {
            InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

            TextSpan span = TextSpan.FromBounds(memberInvocation.Name.Span.Start, invocationExpression.Span.End);

            if (invocationExpression.ContainsDirectives(span))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var methodSymbol = semanticModel.GetSymbol(invocationExpression, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            if (!ExtensionMethodInfo.TryCreate(methodSymbol, semanticModel, out ExtensionMethodInfo extensionMethodInfo))
                return;

            if (!extensionMethodInfo.MethodInfo.IsLinqSelect(allowImmutableArrayExtension: true))
                return;

            ITypeSymbol typeArgument = extensionMethodInfo.ReducedSymbolOrSymbol.TypeArguments[0];

            if (!typeArgument.IsReferenceType)
                return;

            if (typeArgument.SpecialType == SpecialType.System_Object)
                return;

            ExpressionSyntax expression = invocationExpression.ArgumentList?.Arguments.Last().Expression;

            if (expression == null)
                return;

            SyntaxKind kind = expression.Kind();

            //TODO: SingleParameterLambdaExpressionInfo
            if (kind == SyntaxKind.SimpleLambdaExpression)
            {
                var lambda = (SimpleLambdaExpressionSyntax)expression;

                if (!IsFixableLambda(lambda.Parameter, lambda.Body, semanticModel, cancellationToken))
                    return;
            }
            else if (kind == SyntaxKind.ParenthesizedLambdaExpression)
            {
                var lambda = (ParenthesizedLambdaExpressionSyntax)expression;

                ParameterSyntax parameter = lambda.ParameterList?.Parameters.SingleOrDefault(throwException: false);

                if (!IsFixableLambda(parameter, lambda.Body, semanticModel, cancellationToken))
                    return;
            }
            else
            {
                return;
            }

            context.ReportDiagnostic(
                DiagnosticDescriptors.CallCastInsteadOfSelect,
                Location.Create(invocationExpression.SyntaxTree, span));
        }

        private static bool IsFixableLambda(
            ParameterSyntax parameter,
            CSharpSyntaxNode body,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (parameter == null)
                return false;

            if (body == null)
                return false;

            CastExpressionSyntax castExpression = GetCastExpression(body);

            if (castExpression == null)
                return false;

            if (!(castExpression.Expression is IdentifierNameSyntax identifierName))
                return false;

            if (!string.Equals(
                parameter.Identifier.ValueText,
                identifierName.Identifier.ValueText,
                StringComparison.Ordinal))
            {
                return false;
            }

            var methodSymbol = semanticModel.GetSymbol(castExpression, cancellationToken) as IMethodSymbol;

            return methodSymbol?.MethodKind != MethodKind.Conversion;
        }

        private static CastExpressionSyntax GetCastExpression(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.CastExpression:
                    {
                        return (CastExpressionSyntax)node;
                    }
                case SyntaxKind.Block:
                    {
                        var block = (BlockSyntax)node;

                        var returnStatement = block.Statements.SingleOrDefault(throwException: false) as ReturnStatementSyntax;

                        return returnStatement?.Expression as CastExpressionSyntax;
                    }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            ArgumentSyntax lastArgument = invocationExpression.ArgumentList.Arguments.Last();

            var lambdaExpression = (LambdaExpressionSyntax)lastArgument.Expression;

            GenericNameSyntax newName = GenericName(
                Identifier("Cast"),
                GetCastExpression(lambdaExpression.Body).Type);

            InvocationExpressionSyntax newInvocationExpression = invocationExpression
                .RemoveNode(lastArgument, RemoveHelper.GetRemoveOptions(lastArgument))
                .WithExpression(memberAccessExpression.WithName(newName));

            return document.ReplaceNodeAsync(invocationExpression, newInvocationExpression, cancellationToken);
        }
    }
}
