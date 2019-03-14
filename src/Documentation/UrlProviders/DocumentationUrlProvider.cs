// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

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
            return MicrosoftDocsUrlProvider.GetFolders(symbol);
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
