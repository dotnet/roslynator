// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class GenerateBaseConstructorsAnalysis
    {
        public static List<IMethodSymbol> GetMissingBaseConstructors(
            ClassDeclarationSyntax classDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            if (symbol?.IsStatic == false)
            {
                INamedTypeSymbol baseSymbol = symbol.BaseType;

                if (baseSymbol?.IsObject() == false)
                    return GetMissingBaseConstructors(symbol, baseSymbol);
            }

            return null;
        }

        private static List<IMethodSymbol> GetMissingBaseConstructors(INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = symbol.InstanceConstructors.RemoveAll(f => f.IsImplicitlyDeclared);

            List<IMethodSymbol> missing = null;

            foreach (IMethodSymbol baseConstructor in GetBaseConstructors(baseSymbol))
            {
                if (IsAccessibleFromDerivedClass(baseConstructor)
                    && constructors.IndexOf(baseConstructor, ParametersComparer.Instance) == -1)
                {
                    (missing ?? (missing = new List<IMethodSymbol>())).Add(baseConstructor);
                }
            }

            return missing;
        }

        public static bool IsAnyBaseConstructorMissing(INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = symbol.InstanceConstructors;

            foreach (IMethodSymbol baseConstructor in GetBaseConstructors(baseSymbol))
            {
                if (!baseConstructor.IsImplicitlyDeclared
                    && IsAccessibleFromDerivedClass(baseConstructor)
                    && constructors.IndexOf(baseConstructor, ParametersComparer.Instance) == -1)
                {
                    return true;
                }
            }

            return false;
        }

        private static ImmutableArray<IMethodSymbol> GetBaseConstructors(INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = baseSymbol.InstanceConstructors;

            while (constructors.Length == 1
                && constructors[0].IsImplicitlyDeclared
                && baseSymbol.BaseType?.IsObject() == false)
            {
                baseSymbol = baseSymbol.BaseType;

                constructors = baseSymbol.InstanceConstructors;
            }

            return constructors;
        }

        private static bool IsAccessibleFromDerivedClass(IMethodSymbol methodSymbol)
        {
            return methodSymbol.DeclaredAccessibility.Is(
                Accessibility.Public,
                Accessibility.Protected,
                Accessibility.ProtectedOrInternal);
        }

        private class ParametersComparer : EqualityComparer<IMethodSymbol>
        {
            public static ParametersComparer Instance { get; } = new ParametersComparer();

            public override bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                ImmutableArray<IParameterSymbol> parameters1 = x.Parameters;
                ImmutableArray<IParameterSymbol> parameters2 = y.Parameters;

                if (parameters1.Length != parameters2.Length)
                    return false;

                for (int i = 0; i < parameters1.Length; i++)
                {
                    if (!parameters1[i].Type.Equals(parameters2[i].Type))
                        return false;
                }

                return true;
            }

            public override int GetHashCode(IMethodSymbol obj)
            {
                return 0;
            }
        }
    }
}