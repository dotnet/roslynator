// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;

namespace Roslynator.Documentation
{
    internal class DefaultUrlSegmentProvider : UrlSegmentProvider
    {
        internal static DefaultUrlSegmentProvider Hierarchical { get; } = new(FilesLayout.Hierarchical);

        public DefaultUrlSegmentProvider(
            FilesLayout filesLayout,
            IEnumerable<INamespaceSymbol> commonNamespaces = null)
        {
            FilesLayout = filesLayout;
            CommonNamespaces = commonNamespaces?.ToImmutableHashSet(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                ?? ImmutableHashSet<INamespaceSymbol>.Empty;
        }

        public FilesLayout FilesLayout { get; }

        public ImmutableHashSet<INamespaceSymbol> CommonNamespaces { get; }

        public override ImmutableArray<string> GetSegments(ISymbol symbol)
        {
            if (symbol is INamespaceSymbol namespaceSymbol)
            {
                if (!DocumentationUtility.ShouldGenerateNamespaceFile(namespaceSymbol, CommonNamespaces))
                    return ImmutableArray<string>.Empty;

                if (namespaceSymbol.IsGlobalNamespace)
                    return ImmutableArray.Create(WellKnownNames.GlobalNamespaceName);
            }

            if (FilesLayout == FilesLayout.FlatNamespaces
                && CommonNamespaces.Count == 0)
            {
                return ImmutableArray.Create(symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
            }

            ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();

            if (symbol.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).MethodKind == MethodKind.Constructor)
            {
                builder.Add(WellKnownNames.ConstructorName);
            }
            else if (symbol.Kind == SymbolKind.Field)
            {
                builder.Add(symbol.Name);
            }
            else if (symbol.IsKind(SymbolKind.Method, SymbolKind.Property, SymbolKind.Event))
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
                else if ((symbol as IPropertySymbol)?.IsIndexer == true)
                {
                    builder.Add("Item");
                }
                else
                {
                    builder.Add(symbol.Name);
                }
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

            INamedTypeSymbol containingType = symbol.ContainingType;

            while (containingType != null)
            {
                int arity = containingType.Arity;

                builder.Add((arity > 0) ? containingType.Name + "-" + arity.ToString(CultureInfo.InvariantCulture) : containingType.Name);

                containingType = containingType.ContainingType;
            }

            namespaceSymbol = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;

            Debug.Assert(namespaceSymbol != null, symbol.ToDisplayString(SymbolDisplayFormats.Test));

            if (namespaceSymbol != null)
            {
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    if (symbol.Kind != SymbolKind.Namespace)
                        builder.Add(WellKnownNames.GlobalNamespaceName);
                }
                else if (FilesLayout == FilesLayout.Hierarchical)
                {
                    do
                    {
                        if (CommonNamespaces.Contains(namespaceSymbol))
                        {
                            builder.Add(namespaceSymbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
                            break;
                        }
                        else
                        {
                            builder.Add(namespaceSymbol.Name);

                            namespaceSymbol = namespaceSymbol.ContainingNamespace;
                        }
                    }
                    while (namespaceSymbol?.IsGlobalNamespace == false);
                }
                else if (FilesLayout == FilesLayout.FlatNamespaces)
                {
                    var sb = new StringBuilder();

                    do
                    {
                        if (CommonNamespaces.Contains(namespaceSymbol))
                        {
                            if (sb.Length > 0)
                                builder.Add(sb.ToString());

                            builder.Add(namespaceSymbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
                            break;
                        }
                        else
                        {
                            if (sb.Length > 0)
                                sb.Append(".");

                            sb.Append(namespaceSymbol.Name);

                            namespaceSymbol = namespaceSymbol.ContainingNamespace;
                        }
                    }
                    while (namespaceSymbol?.IsGlobalNamespace == false);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown enum value '{FilesLayout}'.");
                }
            }

            builder.Reverse();

            return builder.ToImmutableArray();
        }
    }
}
