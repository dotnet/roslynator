// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CodeMetrics
{
    internal class CodeMetricsOptions
    {
        public CodeMetricsOptions(
            bool includeGenerated = false,
            bool includeWhitespace = false,
            bool includeComments = false,
            bool includePreprocessorDirectives = false,
            bool ignoreBlockBoundary = false)
        {
            IncludeGeneratedCode = includeGenerated;
            IncludeWhitespace = includeWhitespace;
            IncludeComments = includeComments;
            IncludePreprocessorDirectives = includePreprocessorDirectives;
            IgnoreBlockBoundary = ignoreBlockBoundary;
        }

        public static CodeMetricsOptions Default { get; } = new CodeMetricsOptions();

        public bool IncludeGeneratedCode { get; }

        public bool IncludeWhitespace { get; }

        public bool IncludeComments { get; }

        public bool IncludePreprocessorDirectives { get; }

        public bool IgnoreBlockBoundary { get; }
    }
}
