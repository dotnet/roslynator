// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Documentation
{
    internal sealed class GitHubDocumentationUrlProvider : HierarchicalFilesUrlProvider
    {
        private GitHubDocumentationUrlProvider()
        {
        }

        public GitHubDocumentationUrlProvider(IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(externalProviders)
        {
        }

        public static GitHubDocumentationUrlProvider Instance { get; } = new(ImmutableArray.Create(MicrosoftDocsUrlProvider.Instance));

        public override string ReadMeFileName => "README.md";
    }
}
