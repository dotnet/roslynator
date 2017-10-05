// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal struct OverrideInfo
    {
        public OverrideInfo(ISymbol symbol, ISymbol overriddenSymbol)
        {
            Symbol = symbol;
            OverriddenSymbol = overriddenSymbol;
        }

        private static OverrideInfo Default { get; } = new OverrideInfo();

        public ISymbol Symbol { get; }

        public ISymbol OverriddenSymbol { get; }

        public bool Success
        {
            get { return Symbol != null && OverriddenSymbol != null; }
        }

        public static OverrideInfo Create(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            switch (node)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        return CreateCore(methodDeclaration, semanticModel, cancellationToken);
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        return CreateCore(propertyDeclaration, semanticModel, cancellationToken);
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        return CreateCore(indexerDeclaration, semanticModel, cancellationToken);
                    }
                case EventDeclarationSyntax eventDeclaration:
                    {
                        return CreateCore(eventDeclaration, semanticModel, cancellationToken);
                    }
                case VariableDeclaratorSyntax variableDeclarator:
                    {
                        return CreateCore(variableDeclarator, semanticModel, cancellationToken);
                    }
                default:
                    {
                        return Default;
                    }
            }
        }

        public static bool CanCreate(SyntaxNode node)
        {
            return node != null
                && CanCreate(node.Kind());
        }

        public static bool CanCreate(SyntaxKind kind)
        {
            return kind.Is(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.VariableDeclarator);
        }

        public static OverrideInfo Create(
            MethodDeclarationSyntax methodDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (methodDeclaration == null)
                return Default;

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateCore(methodDeclaration, semanticModel, cancellationToken);
        }

        public static OverrideInfo Create(
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (propertyDeclaration == null)
                return Default;

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateCore(propertyDeclaration, semanticModel, cancellationToken);
        }

        public static OverrideInfo Create(
            IndexerDeclarationSyntax indexerDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (indexerDeclaration == null)
                return Default;

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateCore(indexerDeclaration, semanticModel, cancellationToken);
        }

        public static OverrideInfo Create(
            EventDeclarationSyntax eventDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (eventDeclaration == null)
                return Default;

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateCore(eventDeclaration, semanticModel, cancellationToken);
        }

        public static OverrideInfo Create(
            VariableDeclaratorSyntax variableDeclarator,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (variableDeclarator == null)
                return Default;

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateCore(variableDeclarator, semanticModel, cancellationToken);
        }

        private static OverrideInfo CreateCore(
            MethodDeclarationSyntax methodDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            if (methodSymbol == null)
                return Default;

            IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod;

            if (overriddenMethod == null)
                return Default;

            return new OverrideInfo(methodSymbol, overriddenMethod);
        }

        private static OverrideInfo CreateCore(
            BasePropertyDeclarationSyntax basePropertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var propertySymbol = (IPropertySymbol)semanticModel.GetDeclaredSymbol(basePropertyDeclaration, cancellationToken);

            if (propertySymbol == null)
                return Default;

            IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

            if (overriddenProperty == null)
                return Default;

            return new OverrideInfo(propertySymbol, overriddenProperty);
        }

        private static OverrideInfo CreateCore(
            EventDeclarationSyntax eventDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IEventSymbol eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration, cancellationToken);

            if (eventSymbol == null)
                return Default;

            IEventSymbol overriddenEvent = eventSymbol.OverriddenEvent;

            if (overriddenEvent == null)
                return Default;

            return new OverrideInfo(eventSymbol, overriddenEvent);
        }

        private static OverrideInfo CreateCore(
            VariableDeclaratorSyntax variableDeclarator,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var eventSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, cancellationToken) as IEventSymbol;

            if (eventSymbol == null)
                return Default;

            IEventSymbol overriddenEvent = eventSymbol.OverriddenEvent;

            if (overriddenEvent == null)
                return Default;

            return new OverrideInfo(eventSymbol, overriddenEvent);
        }
    }
}
