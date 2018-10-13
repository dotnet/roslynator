// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text;

namespace Roslynator.Documentation
{
    public static class WellKnownExternalUrlProviders
    {
        public static ExternalUrlProvider MicrosoftDocs { get; } = new MicrosoftDocsUrlProvider();

        private class MicrosoftDocsUrlProvider : ExternalUrlProvider
        {
            public override string Name => "Microsoft Docs";

            public override DocumentationUrlInfo CreateUrl(ImmutableArray<string> folders)
            {
                switch (folders[0])
                {
                    case "System":
                    case "Microsoft":
                        {
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
                }

                return default;
            }
        }
    }
}
