// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public abstract class DocumentationUrlProvider
    {
        private static readonly Regex _notWordCharOrUnderscoreRegex = new Regex(@"[^\w_]");

        protected DocumentationUrlProvider(IEnumerable<ExternalUrlProvider> externalProviders = null)
        {
            ExternalProviders = (externalProviders != null)
                ? ImmutableArray.CreateRange(externalProviders)
                : ImmutableArray<ExternalUrlProvider>.Empty;
        }

        public ImmutableArray<ExternalUrlProvider> ExternalProviders { get; }

        public abstract string GetFileName(DocumentationFileKind kind);

        public abstract DocumentationUrlInfo GetLocalUrl(ImmutableArray<string> folders, ImmutableArray<string> containingFolders = default, string fragment = null);

        public abstract string GetFragment(string value);

        public virtual ImmutableArray<string> GetFolders(ISymbol symbol)
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

        public DocumentationUrlInfo GetExternalUrl(ImmutableArray<string> folders)
        {
            foreach (ExternalUrlProvider provider in ExternalProviders)
            {
                DocumentationUrlInfo urlInfo = provider.CreateUrl(folders);

                if (urlInfo.Url != null)
                    return urlInfo;
            }

            return default;
        }

        internal static string GetUrl(string fileName, ImmutableArray<string> folders, char separator)
        {
            int capacity = fileName.Length + 1;

            foreach (string name in folders)
                capacity += name.Length;

            capacity += folders.Length - 1;

            StringBuilder sb = StringBuilderCache.GetInstance(capacity);

            sb.Append(folders[0]);

            for (int i = 1; i < folders.Length; i++)
            {
                sb.Append(separator);
                sb.Append(folders[i]);
            }

            sb.Append(separator);
            sb.Append(fileName);

            return StringBuilderCache.GetStringAndFree(sb);
        }

        internal string GetUrlToRoot(int depth, char separator, bool scrollToContent = false)
        {
            string fileName = GetFileName(DocumentationFileKind.Root);

            if (depth == 0)
                return fileName + ((scrollToContent) ? "#" + WellKnownNames.TopFragmentName : null);

            int capacity = (depth * 3) + fileName.Length;

            StringBuilder sb = StringBuilderCache.GetInstance(capacity);

            sb.Append("..");

            for (int i = 1; i < depth; i++)
            {
                sb.Append(separator);
                sb.Append("..");
            }

            sb.Append(separator);
            sb.Append(fileName);

            if (scrollToContent)
            {
                sb.Append("#");
                sb.Append(WellKnownNames.TopFragmentName);
            }

            return StringBuilderCache.GetStringAndFree(sb);
        }

        internal static string GetFragment(ISymbol symbol)
        {
            string id = symbol.GetDocumentationCommentId();

            id = TextUtility.RemovePrefixFromDocumentationCommentId(id);

            return _notWordCharOrUnderscoreRegex.Replace(id, "_");
        }
    }
}
