// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;
using static Roslynator.CodeGeneration.CSharp.RuntimeMetadataReference;

namespace Roslynator.CodeGeneration.CSharp
{
    internal static partial class Symbols
    {
        private static Compilation _compilation;

        private static ImmutableArray<IMethodSymbol> _visitMethods;
        private static ImmutableArray<INamedTypeSymbol> _syntaxSymbols;
        private static readonly ImmutableDictionary<ITypeSymbol, IMethodSymbol> _typeSymbolMethodSymbolMap = VisitMethodSymbols.ToImmutableDictionary(f => f.Parameters.Single().Type);

        private static INamedTypeSymbol _csharpSyntaxWalkerSymbol;
        private static INamedTypeSymbol _syntaxNodeSymbol;
        private static INamedTypeSymbol _syntaxListSymbol;
        private static INamedTypeSymbol _separatedSyntaxListSymbol;
        private static INamedTypeSymbol _syntaxTokenSymbol;
        private static INamedTypeSymbol _syntaxTokenListSymbol;

        public static INamedTypeSymbol CSharpSyntaxWalkerSymbol => _csharpSyntaxWalkerSymbol ?? (_csharpSyntaxWalkerSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker"));
        public static INamedTypeSymbol SyntaxNodeSymbol => _syntaxNodeSymbol ?? (_syntaxNodeSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode"));
        public static INamedTypeSymbol SyntaxListSymbol => _syntaxListSymbol ?? (_syntaxListSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SyntaxList`1"));
        public static INamedTypeSymbol SeparatedSyntaxListSymbol => _separatedSyntaxListSymbol ?? (_separatedSyntaxListSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SeparatedSyntaxList`1"));
        public static INamedTypeSymbol SyntaxTokenSymbol => _syntaxTokenSymbol ?? (_syntaxTokenSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SyntaxToken"));
        public static INamedTypeSymbol SyntaxTokenListSymbol => _syntaxTokenListSymbol ?? (_syntaxTokenListSymbol = Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SyntaxTokenList"));

        public static ImmutableArray<IMethodSymbol> VisitMethodSymbols
        {
            get
            {
                if (_visitMethods.IsDefault)
                {
                    _visitMethods = CSharpSyntaxWalkerSymbol
                        .BaseType
                        .GetMembers()
                        .Where(f => f.Kind == SymbolKind.Method
                            && f.DeclaredAccessibility == Accessibility.Public
                            && f.IsVirtual
                            && f.Name.Length > 5
                            && f.Name.StartsWith("Visit", StringComparison.Ordinal))
                        .OrderBy(f => f.Name)
                        .Cast<IMethodSymbol>()
                        .ToImmutableArray();

                    Debug.Assert(_visitMethods.All(f => f.Parameters.SingleOrDefault(shouldThrow: false).Type.EqualsOrInheritsFrom(SyntaxNodeSymbol)));
                }

                return _visitMethods;
            }
        }

        public static ImmutableArray<INamedTypeSymbol> SyntaxSymbols
        {
            get
            {
                if (_syntaxSymbols.IsDefault)
                {
                    _syntaxSymbols = Compilation
                        .GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax")
                        .ContainingNamespace
                        .GetTypeMembers()
                        .Where(f => f.TypeKind == TypeKind.Class && f.InheritsFrom(SyntaxNodeSymbol))
                        .OrderBy(f => f.Name)
                        .ToImmutableArray();
                }

                return _syntaxSymbols;
            }
        }

        internal static Compilation Compilation
        {
            get
            {
                return _compilation ?? (_compilation = CSharpCompilation.Create(
                    assemblyName: "Temp",
                    syntaxTrees: null,
                    references: new MetadataReference[]
                    {
                        CorLibReference,
                        CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                        CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll")
                    }));
            }
        }

        public static IMethodSymbol FindVisitMethod(ITypeSymbol typeSymbol)
        {
            return (_typeSymbolMethodSymbolMap.TryGetValue(typeSymbol, out IMethodSymbol methodSymbol))
                ? methodSymbol
                : null;
        }

        public static IEnumerable<IPropertySymbol> GetPropertySymbols(
            ITypeSymbol typeSymbol,
            string name = null,
            bool skipObsolete = true)
        {
            foreach (ISymbol symbol in (name != null)
                ? typeSymbol.GetMembers(name)
                : typeSymbol.GetMembers())
            {
                if (symbol.Kind != SymbolKind.Property)
                    continue;

                if (symbol.DeclaredAccessibility != Accessibility.Public)
                    continue;

                if (skipObsolete && symbol.HasAttribute(MetadataNames.System_ObsoleteAttribute))
                    continue;

                var propertySymbol = (IPropertySymbol)symbol;

                if (propertySymbol.IsIndexer)
                    continue;

                yield return propertySymbol;
            }
        }

        public static IPropertySymbol FindListPropertySymbol(IPropertySymbol propertySymbol)
        {
            string propertyName = propertySymbol.Name;

            string name = GetListPropertyName();

            foreach (IPropertySymbol symbol in GetPropertySymbols(propertySymbol.Type, name))
            {
                if (symbol.Type.OriginalDefinition.Equals(SyntaxListSymbol)
                    || symbol.Type.OriginalDefinition.Equals(SeparatedSyntaxListSymbol))
                {
                    return symbol;
                }
            }

            return null;

            string GetListPropertyName()
            {
                switch (propertyName)
                {
                    case "TypeArgumentList":
                        return "Arguments";
                    case "TypeParameterList":
                        return "Parameters";
                    default:
                        return propertyName.Remove(propertyName.Length - 4) + "s";
                }
            }
        }

        public static bool IsSyntaxTypeSymbol(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.Equals(SyntaxTokenListSymbol))
                return true;

            if (typeSymbol.Equals(SyntaxTokenSymbol))
                return true;

            ITypeSymbol originalDefinition = typeSymbol.OriginalDefinition;

            if (originalDefinition.Equals(SyntaxListSymbol))
                return true;

            if (originalDefinition.Equals(SeparatedSyntaxListSymbol))
                return true;

            if (typeSymbol.EqualsOrInheritsFrom(SyntaxNodeSymbol))
                return true;

            return false;
        }

        public static IEnumerable<INamedTypeSymbol> GetDerivedTypes(INamedTypeSymbol syntaxSymbol, Func<INamedTypeSymbol, bool> predicate = null)
        {
            foreach (INamedTypeSymbol symbol in SyntaxSymbols)
            {
                if (symbol.InheritsFrom(syntaxSymbol)
                    && predicate?.Invoke(symbol) != false)
                {
                    yield return symbol;
                }
            }
        }
    }
}
