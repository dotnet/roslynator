// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.FindSymbols
{
    internal static class LocalSymbolFinder
    {
        public static ImmutableArray<ISymbol> FindLocalSymbols(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            var walker = new LocalSymbolCollector(semanticModel, cancellationToken);

            switch (node.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructor = (ConstructorDeclarationSyntax)node;
                        walker.Visit(constructor.BodyOrExpressionBody());

                        break;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)node;

                        foreach (AccessorDeclarationSyntax accessor in eventDeclaration.AccessorList.Accessors)
                            walker.Visit(accessor.BodyOrExpressionBody());

                        break;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;

                        foreach (AccessorDeclarationSyntax accessor in indexerDeclaration.AccessorList.Accessors)
                            walker.Visit(accessor.BodyOrExpressionBody());

                        break;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        walker.Visit(methodDeclaration.BodyOrExpressionBody());

                        break;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;

                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            walker.Visit(expressionBody);
                        }
                        else
                        {
                            foreach (AccessorDeclarationSyntax accessor in propertyDeclaration.AccessorList.Accessors)
                                walker.Visit(accessor.BodyOrExpressionBody());
                        }

                        break;
                    }
                case SyntaxKind.VariableDeclarator:
                    {
                        var declarator = (VariableDeclaratorSyntax)node;

                        ExpressionSyntax expression = declarator.Initializer?.Value;

                        if (expression != null)
                            walker.Visit(expression);

                        break;
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)node;

                        walker.Visit(declaration.BodyOrExpressionBody());
                        break;
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)node;

                        walker.Visit(declaration.BodyOrExpressionBody());
                        break;
                    }
                default:
                    {
                        SyntaxDebug.Fail(node);

                        walker.Visit(node);
                        break;
                    }
            }

            return walker.Definitions.ToImmutableArray();
        }

        private class LocalSymbolCollector : LocalWalker
        {
            public LocalSymbolCollector(
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
            {
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;

                Definitions = ImmutableArray.CreateBuilder<ISymbol>();
            }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public ImmutableArray<ISymbol>.Builder Definitions { get; }

            public override void VisitLocal(SyntaxNode node)
            {
                ISymbol symbol = SemanticModel.GetDeclaredSymbol(node, CancellationToken);

                if (symbol != null)
                {
                    Debug.Assert(symbol.IsKind(SymbolKind.Local, SymbolKind.Method), symbol.Kind.ToString());

                    if (symbol.Kind == SymbolKind.Local)
                    {
                        VisitSymbol(symbol);
                    }
                    else if (symbol is IMethodSymbol methodSymbol
                        && methodSymbol.MethodKind == MethodKind.LocalFunction)
                    {
                        VisitSymbol(symbol);

                        foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                            VisitSymbol(parameterSymbol);

                        foreach (ITypeParameterSymbol typeParameterSymbol in methodSymbol.TypeParameters)
                            VisitSymbol(typeParameterSymbol);
                    }
                }
                else
                {
                    symbol = SemanticModel.GetSymbol(node, CancellationToken);

                    if (symbol is IMethodSymbol methodSymbol
                        && methodSymbol.MethodKind == MethodKind.AnonymousFunction)
                    {
                        foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                            VisitSymbol(parameterSymbol);
                    }
                }
            }

            private void VisitSymbol(ISymbol symbol)
            {
                CancellationToken.ThrowIfCancellationRequested();

                Definitions.Add(symbol);
            }
        }
    }
}
