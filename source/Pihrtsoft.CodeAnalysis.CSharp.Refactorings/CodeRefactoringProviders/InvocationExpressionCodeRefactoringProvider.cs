// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeRefactoringProvider))]
    public class InvocationExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InvocationExpressionSyntax invocationExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            if (invocationExpression != null
                && invocationExpression.Expression != null
                && invocationExpression.ArgumentList != null
                && invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name?.Span.Contains(context.Span) == true
                && context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                ConvertEnumerableMethodToElementAccessRefactoring.Refactor(context, invocationExpression, semanticModel);

                var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol as IMethodSymbol;

                if (methodSymbol == null)
                    return;

                INamedTypeSymbol enumerable = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");

                if (enumerable == null)
                    return;

                int argumentIndex = (methodSymbol.ReducedFrom != null) ? 0 : 1;

                methodSymbol = methodSymbol.ReducedFrom ?? methodSymbol.ConstructedFrom;

                if (methodSymbol.Equals(GetMethod(enumerable, "Any")))
                {
                    ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);

                    if (expression != null)
                    {
                        context.RegisterRefactoring(
                            "Change 'Any' to 'All'",
                            cancellationToken =>
                            {
                                return CreateChangedDocumentAsync(
                                    context.Document,
                                    invocationExpression,
                                    "All",
                                    expression,
                                    cancellationToken);
                            });
                    }
                }
                else if (methodSymbol.Equals(GetMethod(enumerable, "All")))
                {
                    ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);

                    if (expression != null)
                    {
                        context.RegisterRefactoring(
                            "Change 'All' to 'Any'",
                            cancellationToken =>
                            {
                                return CreateChangedDocumentAsync(
                                    context.Document,
                                    invocationExpression,
                                    "Any",
                                    expression,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static ISymbol GetMethod(INamedTypeSymbol enumerable, string methodName)
        {
            return enumerable
                .GetMembers(methodName)
                .Where(f => f.Kind == SymbolKind.Method)
                .FirstOrDefault(f => ((IMethodSymbol)f).Parameters.Length == 2);
        }

        private static ExpressionSyntax GetExpression(InvocationExpressionSyntax invocationExpression, int argumentIndex)
        {
            ArgumentSyntax argument = invocationExpression
                .ArgumentList?
                .Arguments
                .ElementAtOrDefault(argumentIndex);

            switch (argument?.Expression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        return ((SimpleLambdaExpressionSyntax)argument.Expression).Body as ExpressionSyntax;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        return ((ParenthesizedLambdaExpressionSyntax)argument.Expression).Body as ExpressionSyntax;
                    }
            }

            return null;
        }

        private static async Task<Document> CreateChangedDocumentAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            string memberName,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            MemberAccessExpressionSyntax newMemberAccessExpression = memberAccessExpression
                .WithName(SyntaxFactory.IdentifierName(memberName).WithTriviaFrom(memberAccessExpression.Name));

            InvocationExpressionSyntax newNode = invocationExpression
                .ReplaceNode(expression, expression.Negate())
                .WithExpression(newMemberAccessExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(invocationExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
