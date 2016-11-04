// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddToMethodInvocationRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            string methodName)
        {
            IMethodSymbol methodSymbol = GetMethodSymbol(expression, destinationType, methodName);

            if (methodSymbol != null)
            {
                context.RegisterRefactoring(
                    $"Add '{methodSymbol.Name}()'",
                    cancellationToken =>
                    {
                        return RefactorAsync(
                            context.Document,
                            expression,
                            methodSymbol,
                            cancellationToken);
                    });
            }
        }

        private static IMethodSymbol GetMethodSymbol(ExpressionSyntax expression, ITypeSymbol destinationType, string methodName)
        {
            foreach (ISymbol member in destinationType.GetMembers(methodName))
            {
                if (member.IsMethod()
                    && member.IsPublic())
                {
                    var methodSymbol = (IMethodSymbol)member;

                    if (member.IsStatic)
                    {
                        if (methodSymbol.IsExtensionMethod
                            && methodSymbol.Parameters.Length == 1)
                        {
                            return methodSymbol;
                        }
                    }
                    else if (methodSymbol.Parameters.Length == 0)
                    {
                        return methodSymbol;
                    }
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            InvocationExpressionSyntax invocation = InvocationExpression(expression.WithoutTrailingTrivia(), IdentifierName(methodSymbol.Name))
                .WithTrailingTrivia(expression.GetTrailingTrivia());

            SyntaxNode newRoot = root.ReplaceNode(expression, invocation);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (methodSymbol.IsExtensionMethod)
            {
                INamespaceSymbol namespaceSymbol = methodSymbol.ContainingNamespace;

                if (namespaceSymbol != null
                    && !SyntaxUtility.IsUsingDirectiveInScope(expression, namespaceSymbol, semanticModel, cancellationToken)
                    && newRoot.IsKind(SyntaxKind.CompilationUnit))
                {
                    newRoot = ((CompilationUnitSyntax)newRoot)
                        .AddUsings(UsingDirective(ParseName(namespaceSymbol.ToString())));
                }
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}