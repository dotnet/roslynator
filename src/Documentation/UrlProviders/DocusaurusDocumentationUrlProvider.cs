// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Documentation
{
    internal sealed class DocusaurusDocumentationUrlProvider : HierarchicalFilesUrlProvider
    {
        private DocusaurusDocumentationUrlProvider()
        {
        }

        public DocusaurusDocumentationUrlProvider(IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(externalProviders)
        {
        }

        public static DocusaurusDocumentationUrlProvider Instance { get; } = new(ImmutableArray.Create(MicrosoftDocsUrlProvider.Instance));

        public override string ReadMeFileName => "index.md";
    }
}
