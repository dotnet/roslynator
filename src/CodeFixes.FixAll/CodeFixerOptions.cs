// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.CodeFixes
{
    //TODO: DiagnosticCodeFixProviderMap RCS1155=Roslynator.CodeFixes.MyCodeFixProvider
    //TODO: DiagnosticEquivalenceKeyMap RCS1155=Roslynator.RCS1155.CurrentCultureIgnoreCase
    public class CodeFixerOptions
    {
        public static CodeFixerOptions Default { get; } = new CodeFixerOptions();

        public CodeFixerOptions(
            bool ignoreCompilerErrors = false,
            bool ignoreAnalyzerReferences = false,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> ignoredCompilerDiagnosticIds = null,
            IEnumerable<string> ignoredProjectNames = null,
            int batchSize = -1)
        {
            IgnoreCompilerErrors = ignoreCompilerErrors;
            IgnoreAnalyzerReferences = ignoreAnalyzerReferences;
            IgnoredDiagnosticIds = ignoredDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredCompilerDiagnosticIds = ignoredCompilerDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredProjectNames = ignoredProjectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            BatchSize = batchSize;
        }

        public bool IgnoreCompilerErrors { get; }

        public bool IgnoreAnalyzerReferences { get; }

        public ImmutableHashSet<string> IgnoredDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredProjectNames { get; }

        public int BatchSize { get; }
    }
}
