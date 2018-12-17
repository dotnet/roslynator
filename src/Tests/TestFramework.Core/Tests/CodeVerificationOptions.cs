// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public abstract class CodeVerificationOptions
    {
        protected CodeVerificationOptions(
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
        {
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;

            AllowedCompilerDiagnosticIds = (allowedCompilerDiagnosticIds != null)
                ? ImmutableArray.CreateRange(allowedCompilerDiagnosticIds)
                : ImmutableArray<string>.Empty;
        }

        public bool AllowNewCompilerDiagnostics { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; }

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; }

        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId);

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds);
    }
}
