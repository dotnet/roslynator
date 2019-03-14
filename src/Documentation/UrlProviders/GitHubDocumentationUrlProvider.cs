// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal sealed class GitHubDocumentationUrlProvider : DocumentationUrlProvider
    {
        private GitHubDocumentationUrlProvider()
        {
        }

        public GitHubDocumentationUrlProvider(IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(externalProviders)
        {
        }

        public static GitHubDocumentationUrlProvider Instance { get; } = new GitHubDocumentationUrlProvider(ImmutableArray.Create(MicrosoftDocsUrlProvider.Instance));

        public const string ReadMeFileName = "README.md";

        private const string LinkToSelf = "./" + ReadMeFileName;

        private static readonly Regex _notWordCharOrHyphenOrSpaceRegex = new Regex(@"[^\w- ]");

        private Dictionary<ISymbol, ImmutableArray<string>> _symbolToFoldersMap;

        public override ImmutableArray<string> GetFolders(ISymbol symbol)
        {
            if (_symbolToFoldersMap == null)
                _symbolToFoldersMap = new Dictionary<ISymbol, ImmutableArray<string>>();

            if (_symbolToFoldersMap.TryGetValue(symbol, out ImmutableArray<string> folders))
                return folders;

            folders = base.GetFolders(symbol);

            _symbolToFoldersMap[symbol] = folders;

            return folders;
        }

        public override string GetFileName(DocumentationFileKind kind)
        {
            switch (kind)
            {
                case DocumentationFileKind.Root:
                case DocumentationFileKind.Namespace:
                case DocumentationFileKind.Type:
                case DocumentationFileKind.Member:
                    return ReadMeFileName;
                case DocumentationFileKind.Extensions:
                    return WellKnownNames.Extensions;
                default:
                    throw new ArgumentException("", nameof(kind));
            }
        }

        public override DocumentationUrlInfo GetLocalUrl(ImmutableArray<string> folders, ImmutableArray<string> containingFolders = default, string fragment = null)
        {
            string url = CreateLocalUrl();

            return new DocumentationUrlInfo(url, DocumentationUrlKind.Local);

            string CreateLocalUrl()
            {
                if (containingFolders.IsDefault)
                    return GetUrl(ReadMeFileName, folders, '/') + fragment;

                if (FoldersEqual(containingFolders, folders))
                    return (string.IsNullOrEmpty(fragment)) ? LinkToSelf : fragment;

                int count = 0;

                int i = 0;
                int j = 0;

                while (i < folders.Length
                    && j < containingFolders.Length
                    && string.Equals(folders[i], containingFolders[j], StringComparison.Ordinal))
                {
                    count++;
                    i++;
                    j++;
                }

                int diff = containingFolders.Length - count;

                StringBuilder sb = StringBuilderCache.GetInstance();

                if (diff > 0)
                {
                    sb.Append("..");
                    diff--;

                    while (diff > 0)
                    {
                        sb.Append("/..");
                        diff--;
                    }
                }

                i = count;

                if (i < folders.Length)
                {
                    if (sb.Length > 0)
                        sb.Append("/");

                    sb.Append(folders[i]);
                    i++;

                    while (i < folders.Length)
                    {
                        sb.Append("/");
                        sb.Append(folders[i]);
                        i++;
                    }
                }

                sb.Append("/");
                sb.Append(ReadMeFileName);

                return StringBuilderCache.GetStringAndFree(sb) + fragment;
            }

            bool FoldersEqual(ImmutableArray<string> folders1, ImmutableArray<string> folders2)
            {
                int length = folders1.Length;

                if (length != folders2.Length)
                    return false;

                for (int i = 0; i < length; i++)
                {
                    if (folders1[i] != folders2[i])
                        return false;
                }

                return true;
            }
        }

        public override string GetFragment(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            value = _notWordCharOrHyphenOrSpaceRegex.Replace(value, "");

            value = value.Replace(' ', '-');

            return "#" + value.ToLowerInvariant();
        }
    }
}
