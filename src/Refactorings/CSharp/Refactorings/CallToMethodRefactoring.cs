// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallToMethodRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            string methodName)
        {
            IMethodSymbol methodSymbol = GetMethodSymbol(destinationType, methodName);

            if (methodSymbol == null)
                return;

            context.RegisterRefactoring(
                $"Call '{methodSymbol.Name}()'",
                cancellationToken => RefactorAsync(context.Document, expression, methodSymbol, cancellationToken));
        }

        private static IMethodSymbol GetMethodSymbol(ITypeSymbol destinationType, string methodName)
        {
            foreach (ISymbol symbol in destinationType.GetMembers(methodName))
            {
                if (symbol.Kind == SymbolKind.Method)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.DeclaredAccessibility == Accessibility.Public)
                    {
                        if (methodSymbol.IsStatic)
                        {
                            if (methodSymbol.IsExtensionMethod
                                && methodSymbol.Parameters.Length == 1)
                            {
                                return methodSymbol;
                            }
                        }
                        else if (!methodSymbol.Parameters.Any())
                        {
                            return methodSymbol;
                        }
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

            InvocationExpressionSyntax invocation = SimpleMemberInvocationExpression(
                expression
                    .WithoutTrailingTrivia()
                    .Parenthesize(),
                IdentifierName(methodSymbol.Name));

            invocation = invocation.WithTrailingTrivia(expression.GetTrailingTrivia());

            SyntaxNode newRoot = root.ReplaceNode(expression, invocation);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (methodSymbol.IsExtensionMethod
                && newRoot.IsKind(SyntaxKind.CompilationUnit))
            {
                INamespaceSymbol namespaceSymbol = methodSymbol.ContainingNamespace;

                if (namespaceSymbol != null
                    && !CSharpUtility.IsNamespaceInScope(expression, namespaceSymbol, semanticModel, cancellationToken))
                {
                    newRoot = ((CompilationUnitSyntax)newRoot)
                        .AddUsings(UsingDirective(ParseName(namespaceSymbol.ToString())));
                }
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}