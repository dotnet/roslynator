// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Configuration
{
    internal class VisualStudioCodeAnalysisConfig
    {
        internal static VisualStudioCodeAnalysisConfig Empty { get; } = new();

        public VisualStudioCodeAnalysisConfig(
            bool prefixFieldIdentifierWithUnderscore = ConfigOptionDefaultValues.PrefixFieldIdentifierWithUnderscore,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null)
        {
            Refactorings = refactorings?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;
            CodeFixes = codeFixes?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;
            PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

        public VisualStudioCodeAnalysisConfig WithPrefixfieldIdentifierWithUnderscore(bool value)
        {
            return new VisualStudioCodeAnalysisConfig(
                value,
                Refactorings,
                CodeFixes);
        }

        public VisualStudioCodeAnalysisConfig WithRefactorings(IEnumerable<KeyValuePair<string, bool>> refactorings)
        {
            return new VisualStudioCodeAnalysisConfig(
                PrefixFieldIdentifierWithUnderscore,
                refactorings,
                CodeFixes);
        }

        public VisualStudioCodeAnalysisConfig WithCodeFixes(IEnumerable<KeyValuePair<string, bool>> codeFixes)
        {
            return new VisualStudioCodeAnalysisConfig(
                PrefixFieldIdentifierWithUnderscore,
                Refactorings,
                codeFixes);
        }
    }
}
