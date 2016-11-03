// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultValueToReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = GetMethodOrLambdaSymbol(returnStatement, semanticModel, context.CancellationToken);

            if (symbol?.IsErrorType() == false)
            {
                ITypeSymbol typeSymbol = GetTypeSymbol(symbol, semanticModel);

                if (typeSymbol?.IsErrorType() == false)
                {
                    context.RegisterRefactoring(
                        "Return default value",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                returnStatement,
                                typeSymbol,
                                symbol,
                                cancellationToken);
                        });
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            ITypeSymbol typeSymbol,
            ISymbol symbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax expression = SyntaxUtility.CreateDefaultValue(typeSymbol);

            root = root.ReplaceNode(
                returnStatement,
                returnStatement.WithExpression(expression));

            return document.WithSyntaxRoot(root);
        }

        private static ISymbol GetMethodOrLambdaSymbol(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        {
                            return semanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ancestor, cancellationToken);
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            return semanticModel.GetDeclaredSymbol((PropertyDeclarationSyntax)ancestor, cancellationToken);
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            return semanticModel.GetDeclaredSymbol((IndexerDeclarationSyntax)ancestor, cancellationToken);
                        }
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                        {
                            return semanticModel.GetSymbolInfo(ancestor, cancellationToken).Symbol;
                        }
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.IncompleteMember:
                        return null;
                }
            }

            return null;
        }

        private static ITypeSymbol GetTypeSymbol(
            ISymbol symbol,
            SemanticModel semanticModel)
        {
            if (symbol.IsProperty())
            {
                return ((IPropertySymbol)symbol).Type;
            }
            else if (symbol.IsMethod())
            {
                var methodSymbol = (IMethodSymbol)symbol;

                if (!methodSymbol.ReturnsVoid)
                {
                    if (methodSymbol.IsAsync)
                    {
                        INamedTypeSymbol taskOfTSymbol = semanticModel
                            .Compilation
                            .GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                        if (methodSymbol.ReturnType.IsNamedType())
                        {
                            var returnType = (INamedTypeSymbol)methodSymbol.ReturnType;

                            if (returnType.ConstructedFrom.Equals(taskOfTSymbol))
                                return returnType.TypeArguments[0];
                        }
                    }
                    else
                    {
                        return methodSymbol.ReturnType;
                    }
                }
            }

            return null;
        }
    }
}
