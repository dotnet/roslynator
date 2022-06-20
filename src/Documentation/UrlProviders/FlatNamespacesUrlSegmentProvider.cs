// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal class FlatNamespacesUrlSegmentProvider : UrlSegmentProvider
    {
        public override ImmutableArray<string> GetSegments(ISymbol symbol)
        {
            if (symbol is INamespaceSymbol namespaceSymbol)
            {
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    return ImmutableArray.Create(WellKnownNames.GlobalNamespaceName);
                }
                else
                {
                    return ImmutableArray.Create(symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
                }
            }

            ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();

            if (symbol.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).MethodKind == MethodKind.Constructor)
            {
                builder.Add(WellKnownNames.ConstructorName);
            }
            else if (symbol.Kind == SymbolKind.Property
                && ((IPropertySymbol)symbol).IsIndexer)
            {
                builder.Add("Item");
            }
            else if (!symbol.IsKind(SymbolKind.Namespace))
            {
                ISymbol explicitImplementation = symbol.GetFirstExplicitInterfaceImplementation();

                if (explicitImplementation != null)
                {
                    string name = explicitImplementation
                        .ToDisplayParts(DocumentationDisplayFormats.ExplicitImplementationFullName, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName)
                        .Where(part => part.Kind != SymbolDisplayPartKind.Space)
                        .Select(part => (part.IsPunctuation()) ? part.WithText("-") : part)
                        .ToImmutableArray()
                        .ToDisplayString();

                    builder.Add(name);
                }
                else if (symbol.IsKind(SymbolKind.NamedType))
                {
                    int arity = symbol.GetArity();

                    if (arity > 0)
                    {
                        builder.Add(symbol.Name + "-" + arity.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        builder.Add(symbol.Name);
                    }
                }
                else
                {
                    builder.Add(symbol.Name);
                }
            }

            INamedTypeSymbol containingType = symbol.ContainingType;

            while (containingType != null)
            {
                int arity = containingType.Arity;

                builder.Add((arity > 0) ? containingType.Name + "-" + arity.ToString(CultureInfo.InvariantCulture) : containingType.Name);

                containingType = containingType.ContainingType;
            }

            INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

            if (containingNamespace != null)
            {
                if (containingNamespace.IsGlobalNamespace)
                {
                    if (symbol.Kind != SymbolKind.Namespace)
                        builder.Add(WellKnownNames.GlobalNamespaceName);
                }
                else
                {
                    builder.Add(containingNamespace.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
                }
            }

            builder.Reverse();

            return builder.ToImmutableArray();
        }
    }
}
