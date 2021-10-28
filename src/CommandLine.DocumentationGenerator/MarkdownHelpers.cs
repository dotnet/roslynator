// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal static class MarkdownHelpers
    {
        public static string CreateGitHubHeadingLink(string value)
        {
            return "#"
                + value
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("|", "")
                    .Replace(" ", "-")
                    .ToLowerInvariant();
        }
    }
}
