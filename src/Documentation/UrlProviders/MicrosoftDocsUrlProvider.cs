// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal sealed class MicrosoftDocsUrlProvider : ExternalUrlProvider
    {
        private MicrosoftDocsUrlProvider()
        {
        }

        public static MicrosoftDocsUrlProvider Instance { get; } = new();

        public override string Name => "Microsoft Docs";

        public override DocumentationUrlInfo CreateUrl(ISymbol symbol)
        {
            if (!CanCreateUrl(symbol))
                return default;

            ImmutableArray<string> folders = DefaultUrlSegmentProvider.Hierarchic.GetSegments(symbol);

            const string baseUrl = "https://docs.microsoft.com/en-us/dotnet/api/";

            int capacity = baseUrl.Length;

            foreach (string name in folders)
                capacity += name.Length;

            capacity += folders.Length - 1;

            StringBuilder sb = StringBuilderCache.GetInstance(capacity);

            sb.Append(baseUrl);

            sb.Append(folders[0].ToLowerInvariant());

            for (int i = 1; i < folders.Length; i++)
            {
                sb.Append(".");
                sb.Append(folders[i].ToLowerInvariant());
            }

            return new DocumentationUrlInfo(StringBuilderCache.GetStringAndFree(sb), DocumentationUrlKind.External);
        }

        public override bool CanCreateUrl(ISymbol symbol)
        {
            return IsWellKnownRootNamespace(symbol.GetRootNamespace()?.Name);
        }

        private static bool IsWellKnownRootNamespace(string name)
        {
            switch (name)
            {
                case "System":
                case "Microsoft":
                    return true;
                default:
                    return false;
            }
        }
    }
}
