// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal sealed class MicrosoftDocsUrlProvider : ExternalUrlProvider
    {
        private MicrosoftDocsUrlProvider()
        {
        }

        public static MicrosoftDocsUrlProvider Instance { get; } = new MicrosoftDocsUrlProvider();

        public override string Name => "Microsoft Docs";

        internal static ImmutableArray<string> GetFolders(ISymbol symbol)
        {
            ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();

            if (symbol.Kind == SymbolKind.Namespace
                && ((INamespaceSymbol)symbol).IsGlobalNamespace)
            {
                builder.Add(WellKnownNames.GlobalNamespaceName);
            }
            else if (symbol.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).MethodKind == MethodKind.Constructor)
            {
                builder.Add(WellKnownNames.ConstructorName);
            }
            else if (symbol.Kind == SymbolKind.Property
                && ((IPropertySymbol)symbol).IsIndexer)
            {
                builder.Add("Item");
            }
            else
            {
                ISymbol explicitImplementation = symbol.GetFirstExplicitInterfaceImplementation();

                if (explicitImplementation != null)
                {
                    string name = explicitImplementation
                        .ToDisplayParts(SymbolDisplayFormats.ExplicitImplementationFullName, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName)
                        .Where(part => part.Kind != SymbolDisplayPartKind.Space)
                        .Select(part => (part.IsPunctuation()) ? part.WithText("-") : part)
                        .ToImmutableArray()
                        .ToDisplayString();

                    builder.Add(name);
                }
                else
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
                    {
                        builder.Add(WellKnownNames.GlobalNamespaceName);
                    }
                }
                else
                {
                    do
                    {
                        builder.Add(containingNamespace.Name);

                        containingNamespace = containingNamespace.ContainingNamespace;
                    }
                    while (containingNamespace?.IsGlobalNamespace == false);
                }
            }

            builder.Reverse();

            return builder.ToImmutableArray();
        }

        public override DocumentationUrlInfo CreateUrl(ISymbol symbol)
        {
            ImmutableArray<string> parts = GetFolders(symbol);

            return CreateUrl(parts);
        }

        public override DocumentationUrlInfo CreateUrl(ImmutableArray<string> folders)
        {
            switch (folders[0])
            {
                case "System":
                case "Microsoft":
                    {
                        const string baseUrl = "https://docs.microsoft.com/en-us/dotnet/api/";

                        int capacity = baseUrl.Length;

                        foreach (string name in folders)
                            capacity += name.Length;

                        capacity += folders.Length - 1;

                        StringBuilder sb = StringBuilderCache.GetInstance(capacity);

                        sb.Append(baseUrl);

                        sb.Append(folders[0].ToLowerInvariant());

                        for (int i = 1; i < folders.Length; i++)
                        {
                            sb.Append(".");
                            sb.Append(folders[i].ToLowerInvariant());
                        }

                        return new DocumentationUrlInfo(StringBuilderCache.GetStringAndFree(sb), DocumentationUrlKind.External);
                    }
            }

            return default;
        }
    }
}
