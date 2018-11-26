// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeFixes
{
    internal readonly struct DiagnosticFixResult
    {
        public DiagnosticFixResult(DiagnosticFixKind kind, ImmutableArray<Diagnostic> fixedDiagnostics)
        {
            Kind = kind;
            FixedDiagnostics = fixedDiagnostics;
        }

        public DiagnosticFixKind Kind { get; }

        public ImmutableArray<Diagnostic> FixedDiagnostics { get; }
    }
}
