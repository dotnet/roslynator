// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public static class AsyncAnalysis
    {
        public static bool ContainsAwait(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                .DescendantNodes(node => !node.IsKind(
                    SyntaxKind.SimpleLambdaExpression,
                    SyntaxKind.ParenthesizedLambdaExpression,
                    SyntaxKind.AnonymousMethodExpression))
                .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
        }

        public static bool IsPartOfAsyncBlock(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            SyntaxNode method = node.GetContainingMethod();

            switch (method?.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)method, cancellationToken);

                        return methodSymbol?.IsAsync == true;
                    }
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        ISymbol symbol = semanticModel.GetSymbolInfo(method, cancellationToken).Symbol;

                        return symbol?.IsMethod() == true
                            && ((IMethodSymbol)symbol).IsAsync;
                    }
            }

            return false;
        }
    }
}
