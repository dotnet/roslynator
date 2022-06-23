// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        private static readonly Regex _notWordCharOrUnderscoreRegex = new(@"[^\w_]");

        protected DocumentationUrlProvider(UrlSegmentProvider segmentProvider, IEnumerable<ExternalUrlProvider> externalProviders = null)
        {
            SegmentProvider = segmentProvider;

            ExternalProviders = (externalProviders != null)
                ? ImmutableArray.CreateRange(externalProviders)
                : ImmutableArray<ExternalUrlProvider>.Empty;
        }

        public abstract string IndexFileName { get; }

        public string ExtensionsFileName => "Extensions.md";

        public UrlSegmentProvider SegmentProvider { get; }

        public ImmutableArray<ExternalUrlProvider> ExternalProviders { get; }

        public abstract string GetFileName(DocumentationFileKind kind);

        public abstract DocumentationUrlInfo GetLocalUrl(ImmutableArray<string> folders, ImmutableArray<string> containingFolders = default, string fragment = null);

        public abstract string GetFragment(string value);

        public DocumentationUrlInfo GetExternalUrl(ISymbol symbol)
        {
            foreach (ExternalUrlProvider provider in ExternalProviders)
            {
                DocumentationUrlInfo urlInfo = provider.CreateUrl(symbol);

                if (urlInfo.Url != null)
                    return urlInfo;
            }

            return default;
        }

        public bool HasExternalUrl(ISymbol symbol)
        {
            return MicrosoftDocsUrlProvider.Instance.CanCreateUrl(symbol);
        }

        internal string GetUrl(ISymbol symbol, string fileName, char separator)
        {
            return GetUrl(fileName, SegmentProvider.GetSegments(symbol), separator);
        }

        internal static string GetUrl(string fileName, ImmutableArray<string> segments, char separator)
        {
            int capacity = fileName.Length + 1;

            foreach (string name in segments)
                capacity += name.Length;

            capacity += segments.Length - 1;

            StringBuilder sb = StringBuilderCache.GetInstance(capacity);

            sb.Append(segments[0]);

            for (int i = 1; i < segments.Length; i++)
            {
                sb.Append(separator);
                sb.Append(segments[i]);
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
