// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public abstract class CodeVerificationOptions
    {
        protected CodeVerificationOptions(
            ParseOptions parseOptions,
            CompilationOptions compilationOptions,
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
        {
            ParseOptions = parseOptions ?? throw new ArgumentNullException(nameof(parseOptions));
            CompilationOptions = compilationOptions ?? throw new ArgumentNullException(nameof(compilationOptions));
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;

            AllowedCompilerDiagnosticIds = (allowedCompilerDiagnosticIds != null)
                ? ImmutableArray.CreateRange(allowedCompilerDiagnosticIds)
                : ImmutableArray<string>.Empty;
        }

        public ParseOptions ParseOptions { get; }

        public CompilationOptions CompilationOptions { get; }

        public bool AllowNewCompilerDiagnostics { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; }

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; }

        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId);

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds);
    }
}
