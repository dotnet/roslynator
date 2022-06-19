// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal sealed class GitHubDocumentationUrlProvider : CommonDocumentationUrlProvider
    {
        private Dictionary<ISymbol, ImmutableArray<string>> _symbolToFoldersMap;

        private GitHubDocumentationUrlProvider()
        {
        }

        public GitHubDocumentationUrlProvider(IEnumerable<ExternalUrlProvider> externalProviders = null)
            : base(externalProviders)
        {
        }

        public static GitHubDocumentationUrlProvider Instance { get; } = new(ImmutableArray.Create(MicrosoftDocsUrlProvider.Instance));

        public override ImmutableArray<string> GetFolders(ISymbol symbol)
        {
            if (_symbolToFoldersMap == null)
                _symbolToFoldersMap = new Dictionary<ISymbol, ImmutableArray<string>>();

            if (_symbolToFoldersMap.TryGetValue(symbol, out ImmutableArray<string> folders))
                return folders;

            folders = MicrosoftDocsUrlProvider.GetSegments(symbol);

            _symbolToFoldersMap[symbol] = folders;

            return folders;
        }

        public override string ReadMeFileName => "README.md";
    }
}
