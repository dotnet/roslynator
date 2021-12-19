// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Rename
{
    internal class SymbolRenamerOptions
    {
        internal SymbolRenamerOptions(
            RenameScopeFilter scopeFilter = RenameScopeFilter.All,
            VisibilityFilter visibilityFilter = VisibilityFilter.All,
            RenameErrorResolution errorResolution = RenameErrorResolution.None,
            IEnumerable<string> ignoredCompilerDiagnosticIds = null,
            int codeContext = -1,
            bool includeGeneratedCode = false,
            bool ask = false,
            bool dryRun = false,
            bool interactive = false)
        {
            ScopeFilter = scopeFilter;
            VisibilityFilter = visibilityFilter;
            ErrorResolution = errorResolution;
            IgnoredCompilerDiagnosticIds = ignoredCompilerDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            CodeContext = codeContext;
            IncludeGeneratedCode = includeGeneratedCode;
            Ask = ask;
            DryRun = dryRun;
            Interactive = interactive;
        }

        public static SymbolRenamerOptions Default { get; } = new();

        public RenameScopeFilter ScopeFilter { get; }

        public VisibilityFilter VisibilityFilter { get; }

        public RenameErrorResolution ErrorResolution { get; }

        public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; }

        public int CodeContext { get; }

        public bool IncludeGeneratedCode { get; }

        public bool Ask { get; }

        public bool DryRun { get; }

        public bool Interactive { get; }
    }
}
