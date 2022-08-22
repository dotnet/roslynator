// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal abstract class CommonDocumentationUrlProvider : DocumentationUrlProvider
    {
        protected CommonDocumentationUrlProvider(UrlSegmentProvider segmentProvider, IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(segmentProvider, externalProviders)
        {
        }

        private string LinkToSelf
        {
            get
            {
                if (_linkToSelf == null)
                    _linkToSelf = "./" + IndexFileName;

                return _linkToSelf;
            }
        }

        private static readonly Regex _notWordCharOrHyphenOrSpaceRegex = new(@"[^\w- ]");

        private string _linkToSelf;

        public override string GetFileName(DocumentationFileKind kind)
        {
            switch (kind)
            {
                case DocumentationFileKind.Root:
                case DocumentationFileKind.Namespace:
                case DocumentationFileKind.Type:
                case DocumentationFileKind.Member:
                    return IndexFileName;
                case DocumentationFileKind.Extensions:
                    return ExtensionsFileName;
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
                    return GetUrl(IndexFileName, folders, '/') + fragment;

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
                sb.Append(IndexFileName);

                return StringBuilderCache.GetStringAndFree(sb) + fragment;
            }

            static bool FoldersEqual(ImmutableArray<string> folders1, ImmutableArray<string> folders2)
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
