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
            MemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            TextSpan span = TextSpan.FromBounds(invocationInfo.Name.Span.Start, invocationExpression.Span.End);

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

            SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(expression);

            if (!lambdaInfo.Success)
                return;

            CastExpressionSyntax castExpression = GetCastExpression(lambdaInfo.Body);

            if (castExpression == null)
                return;

            if (!(castExpression.Expression is IdentifierNameSyntax identifierName))
                return;

            if (!string.Equals(lambdaInfo.Parameter.Identifier.ValueText, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                return;

            var castSymbol = semanticModel.GetSymbol(castExpression, cancellationToken) as IMethodSymbol;

            if (castSymbol?.MethodKind == MethodKind.Conversion)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.CallCastInsteadOfSelect,
                Location.Create(invocationExpression.SyntaxTree, span));
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

                        var returnStatement = block.Statements.SingleOrDefault(shouldThrow: false) as ReturnStatementSyntax;

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
