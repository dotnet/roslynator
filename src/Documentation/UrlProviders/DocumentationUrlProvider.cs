// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation;

public abstract class DocumentationUrlProvider
{
    internal string ExtensionsFileName = "Extensions.md";

    private readonly Dictionary<string, string> _symbolToLinkMap = new();

    protected DocumentationUrlProvider(UrlSegmentProvider segmentProvider, IEnumerable<ExternalUrlProvider> externalProviders = null)
    {
        SegmentProvider = segmentProvider;

        ExternalProviders = (externalProviders is not null)
            ? ImmutableArray.CreateRange(externalProviders)
            : ImmutableArray<ExternalUrlProvider>.Empty;
    }

    public abstract string IndexFileName { get; }

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

            if (urlInfo.Url is not null)
                return urlInfo;
        }

        return default;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
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
            sb.Append('#');
            sb.Append(WellKnownNames.TopFragmentName);
        }

        return StringBuilderCache.GetStringAndFree(sb);
    }

    internal string GetFragment(ISymbol symbol)
    {
        string id = symbol.GetDocumentationCommentId();

        if (!_symbolToLinkMap.TryGetValue(id, out string link))
        {
            int hashCode = GetDeterministicHashCode(id);

            long linkCode;
            if (hashCode >= 0)
            {
                linkCode = (uint)hashCode;
            }
            else
            {
                linkCode = int.MaxValue + (uint)Math.Abs(hashCode);
            }

            link = linkCode.ToString(CultureInfo.InvariantCulture);

            if (_symbolToLinkMap.ContainsValue(link))
            {
                Debug.Fail(id);

                linkCode += uint.MaxValue;
                link = linkCode.ToString(CultureInfo.InvariantCulture);

                while (_symbolToLinkMap.ContainsValue(link))
                {
                    linkCode++;
                    link = linkCode.ToString(CultureInfo.InvariantCulture);
                }
            }

            _symbolToLinkMap.Add(id, link);
        }

        return link;
    }

    // https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/#a-deterministic-gethashcode-implementation
    private static int GetDeterministicHashCode(string s)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < s.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ s[i];

                if (i == s.Length - 1)
                    break;

                hash2 = ((hash2 << 5) + hash2) ^ s[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
