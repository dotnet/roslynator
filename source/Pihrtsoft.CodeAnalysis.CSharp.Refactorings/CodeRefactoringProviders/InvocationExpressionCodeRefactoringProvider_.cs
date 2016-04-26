// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
#if DEBUG
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeRefactoringProvider))]
    public class InvocationExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InvocationExpressionSyntax invocationExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            if (invocationExpression == null
                || invocationExpression.Expression == null
                || invocationExpression.ArgumentList == null)
            {
                return;
            }

            if (!invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            if (!invocationExpression.Expression.Span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            if (semanticModel == null)
                return;

            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol as IMethodSymbol;
            if (methodSymbol == null)
                return;

            INamedTypeSymbol enumerable = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");
            if (enumerable == null)
                return;

            int argumentIndex = (methodSymbol.ReducedFrom != null) ? 0 : 1;

            methodSymbol = methodSymbol.ReducedFrom ?? methodSymbol.ConstructedFrom;

            if (methodSymbol.Equals(GetAnyMethod(enumerable)))
            {
                ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);
                if (expression != null)
                {
                    context.RegisterRefactoring(
                        "Swap 'Any' with 'All'",
                        cancellationToken => CreateChangedDocumentAsync(context.Document, invocationExpression, "All", expression, cancellationToken));
                }
            }
            else if (methodSymbol.Equals(GetAllMethod(enumerable)))
            {
                ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);
                if (expression != null)
                {
                    context.RegisterRefactoring(
                        "Swap 'All' with 'Any'",
                        cancellationToken => CreateChangedDocumentAsync(context.Document, invocationExpression, "Any", expression, cancellationToken));
                }
            }
        }

        private static ISymbol GetAnyMethod(INamedTypeSymbol enumerable)
        {
            return enumerable
                .GetMembers("Any")
                .Where(f => f.Kind == SymbolKind.Method)
                .FirstOrDefault(f => ((IMethodSymbol)f).Parameters.Length == 2);
        }

        private static ISymbol GetAllMethod(INamedTypeSymbol enumerable)
        {
            return enumerable
                .GetMembers("All")
                .Where(f => f.Kind == SymbolKind.Method)
                .FirstOrDefault(f => ((IMethodSymbol)f).Parameters.Length == 2);
        }

        private static ExpressionSyntax GetExpression(InvocationExpressionSyntax invocationExpression, int argumentIndex)
        {
            ArgumentSyntax argument = invocationExpression.ArgumentList?.Arguments.ElementAtOrDefault(argumentIndex);

            switch (argument?.Expression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var lambda = (SimpleLambdaExpressionSyntax)argument.Expression;
                        return lambda.Body as ExpressionSyntax;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var lambda = (ParenthesizedLambdaExpressionSyntax)argument.Expression;
                        return lambda.Body as ExpressionSyntax;
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
#endif
}
