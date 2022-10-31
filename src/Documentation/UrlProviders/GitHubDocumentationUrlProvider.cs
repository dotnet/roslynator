// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Documentation
{
    internal sealed class GitHubDocumentationUrlProvider : CommonDocumentationUrlProvider
    {
        public GitHubDocumentationUrlProvider(UrlSegmentProvider segmentProvider, IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(segmentProvider, externalProviders)
        {
        }

        public override string IndexFileName => "README.md";
    }
}
