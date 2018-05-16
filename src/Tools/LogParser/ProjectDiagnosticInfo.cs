// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Diagnostics
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public readonly struct ProjectDiagnosticInfo
    {
        public ProjectDiagnosticInfo(int total, ImmutableArray<AnalyzerDiagnosticInfo> analyzerDiagnostics)
        {
            Total = total;
            AnalyzerDiagnostics = analyzerDiagnostics;
        }

        public int Total { get; }

        public ImmutableArray<AnalyzerDiagnosticInfo> AnalyzerDiagnostics { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Total}ms Count = {AnalyzerDiagnostics.Length}"; }
        }
    }
}
