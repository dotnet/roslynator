// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Documentation
{
    internal sealed class SphinxDocumentationUrlProvider : CommonDocumentationUrlProvider
    {
        public SphinxDocumentationUrlProvider(UrlSegmentProvider segmentProvider, IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(segmentProvider, externalProviders)
        {
        }

        public override string IndexFileName => "index.md";

        public override DocumentationUrlInfo GetLocalUrl(
            ImmutableArray<string> folders,
            ImmutableArray<string> containingFolders = default,
            string fragment = null)
        {
            return (!string.IsNullOrEmpty(fragment))
                ? new DocumentationUrlInfo(fragment, DocumentationUrlKind.Local)
                : base.GetLocalUrl(folders, containingFolders, fragment);
        }
    }
}
